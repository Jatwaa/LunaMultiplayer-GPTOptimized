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
#>

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# ---------------------------------------------------------------------------- #
#  Config                                                                       #
# ---------------------------------------------------------------------------- #

$GH_REPO  = "Jatwaa/LunaMultiplayer-GPTOptimized"
$GH_API   = "https://api.github.com"
$GH_UPLOAD = "https://uploads.github.com"

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

function Prompt-Line([string]$label, [string]$default = "") {
    $hint = if ($default) { " [default: $default]" } else { "" }
    Write-Host "  > $label$hint : " -ForegroundColor White -NoNewline
    $val = Read-Host
    if ([string]::IsNullOrWhiteSpace($val) -and $default) { return $default }
    return $val.Trim()
}

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
        [object]$Body,
        [string]$InFile,
        [string]$ContentType = "application/json"
    )
    $params = @{
        Method      = $Method
        Uri         = $Url
        Headers     = $Headers
        ContentType = $ContentType
        ErrorAction = 'Stop'
    }
    if ($Body)   { $params.Body   = ($Body | ConvertTo-Json -Depth 10) }
    if ($InFile) {
        $params.Remove('Body') | Out-Null
        $params.InFile = $InFile
    }
    return Invoke-RestMethod @params
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
    if ([string]::IsNullOrWhiteSpace($token)) {
        Write-Err "No token provided. Aborting."
        exit 1
    }
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

# Verify token
try {
    $me = Invoke-GH -Method GET -Url "$GH_API/user" -Headers $authHeaders
    Write-Success "Authenticated as: $($me.login)"
} catch {
    Write-Err "Token validation failed: $_"
    Write-Info "Check the token has 'Contents: write' scope on the correct account."
    exit 1
}

# -- 2. Read version ----------------------------------------------------------
Write-Header "Detecting version"

$assemblyInfoPath = Join-Path $repoRoot "LmpCommon\Properties\AssemblyInfo.cs"
$assemblyInfoContent = Get-Content $assemblyInfoPath -Raw
if ($assemblyInfoContent -match '\[assembly: AssemblyVersion\("(\d+\.\d+\.\d+)"\)\]') {
    $version = $Matches[1]
} else {
    Write-Err "Could not read version from $assemblyInfoPath"
    exit 1
}

$tag   = "v$version"
$title = "LMP $version"

Write-Info "Version : $version"
Write-Info "Tag     : $tag"
Write-Info "Title   : $title"

# -- 3. Verify build outputs --------------------------------------------------
Write-Header "Verifying build outputs"

$clientBin = Join-Path $repoRoot "LmpClient\bin\Release"
$serverBin = Join-Path $repoRoot "Server\bin\Release\net8.0"

foreach ($dir in @($clientBin, $serverBin)) {
    if (-not (Test-Path $dir)) {
        Write-Err "Build output missing: $dir"
        Write-Info "Build the solution in Release mode first."
        exit 1
    }
}

if (-not (Test-Path (Join-Path $clientBin "LmpClient.dll"))) {
    Write-Err "LmpClient.dll not found in $clientBin"; exit 1
}
if (-not (Test-Path (Join-Path $serverBin "Server.exe"))) {
    Write-Err "Server.exe not found in $serverBin"; exit 1
}

Write-Success "Client build : $clientBin"
Write-Success "Server build : $serverBin"

# -- 4. Package ZIPs ----------------------------------------------------------
Write-Header "Preparing release packages"

$staging = Join-Path $repoRoot "_release_staging"
if (Test-Path $staging) { Remove-Item $staging -Recurse -Force }
New-Item $staging -ItemType Directory | Out-Null

# 4a. Client ZIP
Write-Info "Packaging client..."

$clientStage = Join-Path $staging "Client"
$lunaDir     = Join-Path $clientStage "GameData\LunaMultiplayer"
New-Item $lunaDir -ItemType Directory -Force | Out-Null

foreach ($f in @("LmpClient.dll","LmpCommon.dll","LmpGlobal.dll",
                  "Lidgren.Network.dll","CachedQuickLz.dll",
                  "JsonFx.dll","System.Runtime.Serialization.dll")) {
    $src = Join-Path $clientBin $f
    if (Test-Path $src) { Copy-Item $src $lunaDir }
    else { Write-Warn "Skipping missing file: $f" }
}

$harmonyDir    = Join-Path $clientStage "GameData\000_Harmony"
$harmonyBinDir = Join-Path $clientBin "Harmony\000_Harmony"
$harmonySrcDir = Join-Path $repoRoot "External\Dependencies\Harmony\000_Harmony"
$harmonySrc    = if (Test-Path $harmonyBinDir) { $harmonyBinDir } else { $harmonySrcDir }
New-Item $harmonyDir -ItemType Directory -Force | Out-Null

foreach ($f in @("0Harmony.dll","HarmonyInstallChecker.dll","Harmony.version")) {
    $src = Join-Path $harmonySrc $f
    if (Test-Path $src) { Copy-Item $src $harmonyDir }
}

$clientZip = Join-Path $staging "LMP-$version-Client.zip"
Compress-Archive -Path (Join-Path $clientStage "*") -DestinationPath $clientZip -Force
Write-Success "Client ZIP -> LMP-$version-Client.zip  ($([math]::Round((Get-Item $clientZip).Length/1KB)) KB)"

# 4b. Server ZIP
Write-Info "Packaging server..."

$serverStage = Join-Path $staging "Server\LMPServer"
New-Item $serverStage -ItemType Directory -Force | Out-Null

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

$serverZip = Join-Path $staging "LMP-$version-Server.zip"
Compress-Archive -Path (Join-Path $staging "Server\*") -DestinationPath $serverZip -Force
Write-Success "Server ZIP -> LMP-$version-Server.zip  ($([math]::Round((Get-Item $serverZip).Length/1KB)) KB)"

# -- 5. Resolve release notes -------------------------------------------------
$changelogPath    = Join-Path $repoRoot "LMP_0.30_2026.md"
$changelogContent = Get-Content $changelogPath -Raw
$releaseNotes     = "LMP $version release. See LMP_0.30_2026.md for full change log."
if ($changelogContent -match '(?s)---\r?\n\r?\n(### \[.*?)---') {
    $releaseNotes = $Matches[1].Trim()
}

# -- 6. Confirm ---------------------------------------------------------------
Write-Header "GitHub Release"

Write-Info "Tag    : $tag"
Write-Info "Title  : $title"
Write-Info "Assets : LMP-$version-Client.zip, LMP-$version-Server.zip"
Write-Host ""

if (-not (Prompt-YN "Publish release '$title' to GitHub now?")) {
    Write-Warn "Release skipped. ZIPs are ready in: $staging"
    exit 0
}

# -- 7. Create or fetch the release -------------------------------------------
Write-Info "Creating GitHub release..."

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
    $release = Invoke-GH -Method POST `
        -Url "$GH_API/repos/$GH_REPO/releases" `
        -Headers $authHeaders `
        -Body $releaseBody
    Write-Success "Release created: $($release.html_url)"
} catch {
    # 422 = tag/release already exists — fetch it and update assets
    Write-Warn "Release may already exist. Fetching existing release for tag $tag..."
    try {
        $release = Invoke-GH -Method GET `
            -Url "$GH_API/repos/$GH_REPO/releases/tags/$tag" `
            -Headers $authHeaders
        Write-Info "Found existing release: $($release.html_url)"
    } catch {
        Write-Err "Could not create or find release: $_"
        exit 1
    }
}

# -- 8. Upload assets ---------------------------------------------------------
Write-Info "Uploading assets..."

$uploadBase = $release.upload_url -replace '\{.*\}', ''

function Upload-Asset([string]$zipPath) {
    $name    = [System.IO.Path]::GetFileName($zipPath)
    $url     = "${uploadBase}?name=$name&label=$name"
    $bytes   = [System.IO.File]::ReadAllBytes($zipPath)

    # Delete existing asset with same name if present
    $existing = $release.assets | Where-Object { $_.name -eq $name }
    if ($existing) {
        Write-Info "Replacing existing asset: $name"
        Invoke-GH -Method DELETE `
            -Url "$GH_API/repos/$GH_REPO/releases/assets/$($existing.id)" `
            -Headers $authHeaders | Out-Null
    }

    $upHeaders = $authHeaders.Clone()
    $upHeaders["Content-Type"] = "application/zip"

    Write-Info "Uploading $name ($([math]::Round($bytes.Length/1KB)) KB)..."
    $result = Invoke-RestMethod -Method POST -Uri $url `
        -Headers $upHeaders -Body $bytes -ErrorAction Stop
    Write-Success "Uploaded: $($result.browser_download_url)"
}

Upload-Asset $clientZip
Upload-Asset $serverZip

# -- 9. Cleanup ---------------------------------------------------------------
Write-Host ""
if (Prompt-YN "Clean up staging folder?") {
    Remove-Item $staging -Recurse -Force
    Write-Success "Staging folder removed."
} else {
    Write-Info "Staging kept at: $staging"
}

Write-Host ""
Write-Host "  Release URL:" -ForegroundColor Gray
Write-Host "  $($release.html_url)" -ForegroundColor Cyan
Write-Host ""
Write-Success "Done! Release $title is live on GitHub."
Write-Host ""
