#Requires -Version 5.1
<#
.SYNOPSIS
    LMP release packager -- builds client + server ZIPs and publishes a
    GitHub release via the 'gh' CLI tool.

.USAGE
    powershell -ExecutionPolicy Bypass -File release.ps1

.PREREQUISITES
    - GitHub CLI (gh) installed and authenticated  ->  https://cli.github.com
    - Project already compiled in Release mode locally
    - Committed and pushed via commit.ps1 first
#>

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

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

# -- 1. Check gh CLI ----------------------------------------------------------
Write-Header "Checking prerequisites"

if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Err "GitHub CLI (gh) not found."
    Write-Info "Install from: https://cli.github.com"
    Write-Info "Then run: gh auth login"
    exit 1
}
Write-Success "gh CLI found: $(gh --version | Select-Object -First 1)"

$ghAuth = gh auth status 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Err "gh is not authenticated. Run: gh auth login"
    exit 1
}
Write-Success "gh authenticated."

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

# -- 3. Verify build outputs exist --------------------------------------------
Write-Header "Verifying build outputs"

$clientBin = Join-Path $repoRoot "LmpClient\bin\Release"
$serverBin = Join-Path $repoRoot "Server\bin\Release\net8.0"

foreach ($dir in @($clientBin, $serverBin)) {
    if (-not (Test-Path $dir)) {
        Write-Err "Build output missing: $dir"
        Write-Info "Build the solution in Release mode first, then re-run this script."
        exit 1
    }
}

$clientDll = Join-Path $clientBin "LmpClient.dll"
$serverExe = Join-Path $serverBin "Server.exe"

if (-not (Test-Path $clientDll)) { Write-Err "LmpClient.dll not found in $clientBin"; exit 1 }
if (-not (Test-Path $serverExe)) { Write-Err "Server.exe not found in $serverBin";    exit 1 }

Write-Success "Client build : $clientBin"
Write-Success "Server build : $serverBin"

# -- 4. Stage area ------------------------------------------------------------
Write-Header "Preparing release packages"

$staging = Join-Path $repoRoot "_release_staging"
if (Test-Path $staging) { Remove-Item $staging -Recurse -Force }
New-Item $staging -ItemType Directory | Out-Null

# -- 4a. Client ZIP -----------------------------------------------------------
Write-Info "Packaging client..."

$clientStage = Join-Path $staging "Client"

$lunaDir = Join-Path $clientStage "GameData\LunaMultiplayer"
New-Item $lunaDir -ItemType Directory -Force | Out-Null

$clientFiles = @(
    "LmpClient.dll",
    "LmpCommon.dll",
    "LmpGlobal.dll",
    "Lidgren.Network.dll",
    "CachedQuickLz.dll",
    "JsonFx.dll",
    "System.Runtime.Serialization.dll"
)

foreach ($f in $clientFiles) {
    $src = Join-Path $clientBin $f
    if (Test-Path $src) {
        Copy-Item $src $lunaDir
    } else {
        Write-Warn "Optional client file not found, skipping: $f"
    }
}

# GameData/000_Harmony
$harmonyDir = Join-Path $clientStage "GameData\000_Harmony"
New-Item $harmonyDir -ItemType Directory -Force | Out-Null

# Prefer the built copy; fall back to the source copy
$harmonyBinDir = Join-Path $clientBin "Harmony\000_Harmony"
$harmonySrcDir = Join-Path $repoRoot "External\Dependencies\Harmony\000_Harmony"
$harmonySrc    = if (Test-Path $harmonyBinDir) { $harmonyBinDir } else { $harmonySrcDir }

foreach ($f in @("0Harmony.dll", "HarmonyInstallChecker.dll", "Harmony.version")) {
    $src = Join-Path $harmonySrc $f
    if (Test-Path $src) { Copy-Item $src $harmonyDir }
}

$clientZip = Join-Path $staging "LMP-$version-Client.zip"
Compress-Archive -Path (Join-Path $clientStage "*") -DestinationPath $clientZip -Force
Write-Success "Client ZIP  -> LMP-$version-Client.zip  ($([math]::Round((Get-Item $clientZip).Length / 1KB)) KB)"

# -- 4b. Server ZIP -----------------------------------------------------------
Write-Info "Packaging server..."

$serverStage = Join-Path $staging "Server\LMPServer"
New-Item $serverStage -ItemType Directory -Force | Out-Null

$excludeDirs = @("logs", "Universe", "Config", "runtimes",
                 "cs","de","es","fr","it","ja","ko","pl","pt-BR","ru","tr","zh-Hans","zh-Hant")
$excludeExts = @(".pdb")

Get-ChildItem $serverBin | ForEach-Object {
    if ($_.PSIsContainer) {
        if ($excludeDirs -notcontains $_.Name) {
            Copy-Item $_.FullName (Join-Path $serverStage $_.Name) -Recurse -Force
        }
    } else {
        if ($excludeExts -notcontains $_.Extension) {
            Copy-Item $_.FullName $serverStage
        }
    }
}

$serverZip = Join-Path $staging "LMP-$version-Server.zip"
Compress-Archive -Path (Join-Path $staging "Server\*") -DestinationPath $serverZip -Force
Write-Success "Server ZIP  -> LMP-$version-Server.zip  ($([math]::Round((Get-Item $serverZip).Length / 1KB)) KB)"

# -- 5. Confirm and publish ---------------------------------------------------
Write-Header "GitHub Release"

Write-Info "Tag    : $tag"
Write-Info "Title  : $title"
Write-Info "Assets : LMP-$version-Client.zip, LMP-$version-Server.zip"
Write-Host ""

if (-not (Prompt-YN "Publish release '$title' to GitHub now?")) {
    Write-Warn "Release skipped. ZIPs are ready in: $staging"
    exit 0
}

# Pull the newest changelog entry as release notes
$changelogPath = Join-Path $repoRoot "LMP_0.30_2026.md"
$changelogContent = Get-Content $changelogPath -Raw
$releaseNotes = ""
if ($changelogContent -match '(?s)---\r?\n\r?\n(### \[.*?)---') {
    $releaseNotes = $Matches[1].Trim()
}
if ([string]::IsNullOrWhiteSpace($releaseNotes)) {
    $releaseNotes = "LMP $version release. See LMP_0.30_2026.md for full change log."
}

$notesFile = Join-Path $staging "release_notes.md"
[System.IO.File]::WriteAllText($notesFile, $releaseNotes, [System.Text.Encoding]::UTF8)

Write-Info "Creating release..."
gh release create $tag `
    --title $title `
    --notes-file $notesFile `
    $clientZip `
    $serverZip 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Success "Release published!"
    Write-Host ""
    Write-Host "  https://github.com/Jatwaa/LunaMultiplayer-GPTOptimized/releases/tag/$tag" -ForegroundColor Cyan
} else {
    Write-Warn "Release create returned non-zero (tag may already exist). Uploading assets..."
    gh release upload $tag $clientZip $serverZip --clobber 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Success "Assets uploaded to existing release $tag."
    } else {
        Write-Err "Failed to publish release. ZIPs saved at: $staging"
        exit 1
    }
}

# -- 6. Cleanup ---------------------------------------------------------------
Write-Host ""
if (Prompt-YN "Clean up staging folder?") {
    Remove-Item $staging -Recurse -Force
    Write-Success "Staging folder removed."
} else {
    Write-Info "Staging kept at: $staging"
}

Write-Host ""
Write-Success "Done! Release $title is live on GitHub."
Write-Host ""
