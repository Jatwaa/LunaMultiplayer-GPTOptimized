#Requires -Version 5.1
<#
.SYNOPSIS
    LMP release packager -- packages client + server ZIPs and publishes a
    GitHub release using the GitHub REST API (no external tools required).

.USAGE
    powershell -ExecutionPolicy Bypass -File release.ps1

.PREREQUISITES
    - Project already compiled in Release mode locally
    - Committed and pushed via commit.ps1 first
    - A GitHub Personal Access Token with 'Contents: write' scope
      Create one at: https://github.com/settings/tokens
      The token is saved to .github_token in the repo root (gitignored).

.PACKAGE LAYOUT

    LMP-{ver}-Client.zip
      GameData\
        LunaMultiplayer\          <- LmpClient\bin\Release\*.dll
        000_Harmony\              <- LmpClient\bin\Release\Harmony\000_Harmony\

    LMP-{ver}-Server.zip
      LMPServer\                  <- Server\bin\Release\net8.0\  (no .pdb / runtime dirs)
      LMPServerGUI\               <- LmpServerGUI\bin\Release\net8.0-windows\  (no .pdb)
#>

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# ---------------------------------------------------------------------------- #
#  Config                                                                       #
# ---------------------------------------------------------------------------- #

$GH_REPO = "Jatwaa/LunaMultiplayer-GPTOptimized"
$GH_API  = "https://api.github.com"

# ---------------------------------------------------------------------------- #
#  Helpers                                                                      #
# ---------------------------------------------------------------------------- #

function Write-Header([string]$text) {
    Write-Host ""
    Write-Host "  $text" -ForegroundColor Magenta
    Write-Host ("  " + ("-" * $text.Length)) -ForegroundColor DarkMagenta
}

function Write-Success([string]$text) { Write-Host "  [OK] $text" -ForegroundColor Green  }
function Write-Warn([string]$text)    { Write-Host "  [!!] $text" -ForegroundColor Yellow }
function Write-Err([string]$text)     { Write-Host "  [XX] $text" -ForegroundColor Red    }
function Write-Info([string]$text)    { Write-Host "  $text"      -ForegroundColor Gray   }

function Prompt-YN([string]$label) {
    Write-Host "  > $label [Y/n] : " -ForegroundColor White -NoNewline
    $val = Read-Host
    return ($val -match '^[Yy]?$')
}

function Invoke-GH {
    param(
        [string]$Method,
        [string]$Url,
        [hashtable]$Headers,
        [object]$Body
    )
    $params = @{
        Method      = $Method
        Uri         = $Url
        Headers     = $Headers
        ContentType = "application/json"
        ErrorAction = 'Stop'
    }
    if ($Body) { $params.Body = ($Body | ConvertTo-Json -Depth 10) }
    return Invoke-RestMethod @params
}

function Upload-Asset {
    param([string]$ZipPath, [string]$UploadBase, [hashtable]$Headers, [array]$ExistingAssets)
    $name = [System.IO.Path]::GetFileName($ZipPath)
    $url  = "${UploadBase}?name=$name"

    # Remove existing asset with same name
    $existing = $ExistingAssets | Where-Object { $_.name -eq $name }
    if ($existing) {
        Write-Info "  Replacing existing asset: $name"
        Invoke-GH -Method DELETE -Url "$GH_API/repos/$GH_REPO/releases/assets/$($existing.id)" -Headers $Headers | Out-Null
    }

    $bytes = [System.IO.File]::ReadAllBytes($ZipPath)
    $upHeaders = $Headers.Clone()
    $upHeaders["Content-Type"] = "application/zip"

    Write-Info "  Uploading $name ($([math]::Round($bytes.Length/1KB)) KB)..."
    $result = Invoke-RestMethod -Method POST -Uri $url -Headers $upHeaders -Body $bytes -ErrorAction Stop
    Write-Success "$name -> $($result.browser_download_url)"
}

# ---------------------------------------------------------------------------- #
#  Main                                                                         #
# ---------------------------------------------------------------------------- #

Clear-Host
Write-Host ""
Write-Host "  +======================================================+" -ForegroundColor Magenta
Write-Host "  |      LMP 0.30 -- Release Packager & Publisher       |" -ForegroundColor Magenta
Write-Host "  +======================================================+" -ForegroundColor Magenta
Write-Host ""

$repoRoot = $PSScriptRoot
Set-Location $repoRoot

# -- 1. GitHub token ----------------------------------------------------------
Write-Header "GitHub authentication"

$tokenFile = Join-Path $repoRoot ".github_token"
$token = ""

if (Test-Path $tokenFile) {
    $token = (Get-Content $tokenFile -Raw).Trim()
    Write-Success "Token loaded from .github_token"
} else {
    Write-Info "No .github_token file found."
    Write-Info "Create a token at: https://github.com/settings/tokens"
    Write-Info "Required scope: Contents (write)"
    Write-Host ""
    Write-Host "  > Paste your GitHub Personal Access Token : " -ForegroundColor White -NoNewline
    $token = Read-Host
    $token = $token.Trim()
    if ([string]::IsNullOrWhiteSpace($token)) { Write-Err "No token provided."; exit 1 }
    if (Prompt-YN "Save token to .github_token for future use? (gitignored)") {
        Set-Content $tokenFile $token -Encoding ASCII
        Write-Success "Token saved."
    }
}

$authHeaders = @{
    Authorization = "token $token"
    Accept        = "application/vnd.github+json"
    "User-Agent"  = "LMP-release-script/1.0"
}

try {
    $me = Invoke-GH -Method GET -Url "$GH_API/user" -Headers $authHeaders
    Write-Success "Authenticated as: $($me.login)"
} catch {
    Write-Err "Token validation failed. Check it has 'Contents: write' scope."
    exit 1
}

# -- 2. Read version ----------------------------------------------------------
Write-Header "Detecting version"

$asmInfo = Get-Content (Join-Path $repoRoot "LmpCommon\Properties\AssemblyInfo.cs") -Raw
if ($asmInfo -match '\[assembly: AssemblyVersion\("(\d+\.\d+\.\d+)"\)\]') {
    $version = $Matches[1]
} else {
    Write-Err "Could not read version from AssemblyInfo.cs"; exit 1
}

$tag   = "v$version"
$title = "LMP $version"

Write-Info "Version : $version  |  Tag : $tag"

# -- 3. Verify build outputs --------------------------------------------------
Write-Header "Verifying build outputs"

$clientBin = Join-Path $repoRoot "LmpClient\bin\Release"
$serverBin = Join-Path $repoRoot "Server\bin\Release\net8.0"
$guiBin    = Join-Path $repoRoot "LmpServerGUI\bin\Release\net8.0-windows"

$missing = @()
if (-not (Test-Path (Join-Path $clientBin "LmpClient.dll")))    { $missing += "LmpClient\bin\Release\LmpClient.dll" }
if (-not (Test-Path (Join-Path $serverBin "Server.exe")))        { $missing += "Server\bin\Release\net8.0\Server.exe" }
if (-not (Test-Path (Join-Path $guiBin    "LunaServerGUI.exe"))) { $missing += "LmpServerGUI\bin\Release\net8.0-windows\LunaServerGUI.exe" }

if ($missing.Count -gt 0) {
    Write-Err "Missing build outputs -- build the solution in Release mode first:"
    $missing | ForEach-Object { Write-Info "  $_" }
    exit 1
}

Write-Success "Client  : $clientBin"
Write-Success "Server  : $serverBin"
Write-Success "GUI     : $guiBin"

# -- 4. Package ZIPs ----------------------------------------------------------
Write-Header "Preparing release packages"

$staging = Join-Path $repoRoot "_release_staging"
if (Test-Path $staging) { Remove-Item $staging -Recurse -Force }
New-Item $staging -ItemType Directory | Out-Null

# ---- 4a. Client ZIP ---------------------------------------------------------
Write-Info "Packaging client..."

$lunaDir    = Join-Path $staging "Client\GameData\LunaMultiplayer"
$harmonyDir = Join-Path $staging "Client\GameData\000_Harmony"
New-Item $lunaDir    -ItemType Directory -Force | Out-Null
New-Item $harmonyDir -ItemType Directory -Force | Out-Null

# All DLLs directly in LmpClient\bin\Release\ (no subdirs)
Get-ChildItem $clientBin -File | Where-Object { $_.Extension -eq ".dll" } | ForEach-Object {
    Copy-Item $_.FullName $lunaDir
}

# Harmony from the built output
$harmonySrc = Join-Path $clientBin "Harmony\000_Harmony"
Get-ChildItem $harmonySrc -File | ForEach-Object {
    Copy-Item $_.FullName $harmonyDir
}

$clientZip = Join-Path $staging "LMP-$version-Client.zip"
Compress-Archive -Path (Join-Path $staging "Client\*") -DestinationPath $clientZip -Force
Write-Success "Client ZIP  -> LMP-$version-Client.zip  ($([math]::Round((Get-Item $clientZip).Length/1KB)) KB)"

# ---- 4b. Server ZIP ---------------------------------------------------------
Write-Info "Packaging server..."

$serverStage = Join-Path $staging "Server\LMPServer"
$guiStage    = Join-Path $staging "Server\LMPServerGUI"
New-Item $serverStage -ItemType Directory -Force | Out-Null
New-Item $guiStage    -ItemType Directory -Force | Out-Null

# Server binaries (exclude .pdb and runtime-generated dirs)
$excludeDirs = @("logs","Universe","Config","runtimes",
                 "cs","de","es","fr","it","ja","ko","pl","pt-BR","ru","tr","zh-Hans","zh-Hant")

Get-ChildItem $serverBin | ForEach-Object {
    if ($_.PSIsContainer) {
        if ($excludeDirs -notcontains $_.Name) {
            Copy-Item $_.FullName (Join-Path $serverStage $_.Name) -Recurse -Force
        }
    } elseif ($_.Extension -ne ".pdb") {
        Copy-Item $_.FullName $serverStage
    }
}

# Server GUI binaries (exclude .pdb)
Get-ChildItem $guiBin | ForEach-Object {
    if ($_.PSIsContainer) {
        Copy-Item $_.FullName (Join-Path $guiStage $_.Name) -Recurse -Force
    } elseif ($_.Extension -ne ".pdb") {
        Copy-Item $_.FullName $guiStage
    }
}

$serverZip = Join-Path $staging "LMP-$version-Server.zip"
Compress-Archive -Path (Join-Path $staging "Server\*") -DestinationPath $serverZip -Force
Write-Success "Server ZIP  -> LMP-$version-Server.zip  ($([math]::Round((Get-Item $serverZip).Length/1KB)) KB)"

# -- 5. Show layout summary ---------------------------------------------------
Write-Header "Package layout"
Write-Info "LMP-$version-Client.zip"
Write-Info "  GameData\LunaMultiplayer\  <- LmpClient\bin\Release\*.dll"
Write-Info "  GameData\000_Harmony\      <- LmpClient\bin\Release\Harmony\000_Harmony\"
Write-Info ""
Write-Info "LMP-$version-Server.zip"
Write-Info "  LMPServer\                 <- Server\bin\Release\net8.0\"
Write-Info "  LMPServerGUI\              <- LmpServerGUI\bin\Release\net8.0-windows\"

# -- 6. Confirm ---------------------------------------------------------------
Write-Host ""
if (-not (Prompt-YN "Publish release '$title' to GitHub now?")) {
    Write-Warn "Release skipped. ZIPs are ready in: $staging"
    exit 0
}

# -- 7. Pull release notes ----------------------------------------------------
$changelog = Get-Content (Join-Path $repoRoot "LMP_0.30_2026.md") -Raw
$releaseNotes = "LMP $version release. See LMP_0.30_2026.md for full change log."
if ($changelog -match '(?s)---\r?\n\r?\n(### \[.*?)---') {
    $releaseNotes = $Matches[1].Trim()
}

# -- 8. Create or fetch the release -------------------------------------------
Write-Header "Creating GitHub release"

$releaseBody = @{
    tag_name         = $tag
    target_commitish = "master"
    name             = $title
    body             = $releaseNotes
    draft            = $false
    prerelease       = $false
}

$release = $null
try {
    $release = Invoke-GH -Method POST -Url "$GH_API/repos/$GH_REPO/releases" `
        -Headers $authHeaders -Body $releaseBody
    Write-Success "Release created: $($release.html_url)"
} catch {
    Write-Warn "Release may already exist -- fetching existing release for $tag..."
    try {
        $release = Invoke-GH -Method GET -Url "$GH_API/repos/$GH_REPO/releases/tags/$tag" `
            -Headers $authHeaders
        Write-Info "Found: $($release.html_url)"
    } catch {
        Write-Err "Could not create or find release: $_"; exit 1
    }
}

# -- 9. Upload assets ---------------------------------------------------------
Write-Header "Uploading assets"

$uploadBase     = $release.upload_url -replace '\{.*\}', ''
$existingAssets = $release.assets

Upload-Asset -ZipPath $clientZip -UploadBase $uploadBase -Headers $authHeaders -ExistingAssets $existingAssets
Upload-Asset -ZipPath $serverZip -UploadBase $uploadBase -Headers $authHeaders -ExistingAssets $existingAssets

# -- 10. Cleanup --------------------------------------------------------------
Write-Host ""
if (Prompt-YN "Clean up staging folder?") {
    Remove-Item $staging -Recurse -Force
    Write-Success "Staging folder removed."
} else {
    Write-Info "Staging kept at: $staging"
}

Write-Host ""
Write-Host "  $($release.html_url)" -ForegroundColor Cyan
Write-Success "Done! Release $title is live."
Write-Host ""
