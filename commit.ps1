#Requires -Version 5.1
<#
.SYNOPSIS
    LMP interactive commit helper — stages changes, documents them in
    LMP_0.30_2026.md, commits, and pushes to origin/main.

.USAGE
    powershell -ExecutionPolicy Bypass -File commit.ps1
#>

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# ── Helpers ──────────────────────────────────────────────────────────────────

function Write-Header([string]$text) {
    Write-Host ""
    Write-Host "  $text" -ForegroundColor Cyan
    Write-Host ("  " + "─" * ($text.Length)) -ForegroundColor DarkCyan
}

function Write-Success([string]$text) { Write-Host "  ✔  $text" -ForegroundColor Green  }
function Write-Warn([string]$text)    { Write-Host "  ⚠  $text" -ForegroundColor Yellow }
function Write-Err([string]$text)     { Write-Host "  ✖  $text" -ForegroundColor Red    }
function Write-Info([string]$text)    { Write-Host "  $text"    -ForegroundColor Gray   }

function Prompt-Line([string]$label, [string]$default = "") {
    $hint = if ($default) { " [default: $default]" } else { "" }
    Write-Host "  ► $label$hint : " -ForegroundColor White -NoNewline
    $val = Read-Host
    if ([string]::IsNullOrWhiteSpace($val) -and $default) { return $default }
    return $val.Trim()
}

function Prompt-Multiline([string]$label) {
    Write-Host "  ► $label (empty line to finish):" -ForegroundColor White
    $lines = @()
    while ($true) {
        Write-Host "    > " -ForegroundColor DarkGray -NoNewline
        $line = Read-Host
        if ([string]::IsNullOrWhiteSpace($line)) { break }
        $lines += $line
    }
    return $lines -join " "
}

function Prompt-YN([string]$label) {
    Write-Host "  ► $label [Y/n] : " -ForegroundColor White -NoNewline
    $val = Read-Host
    return ($val -match '^[Yy]?$')
}

# ── Main ──────────────────────────────────────────────────────────────────────

Clear-Host
Write-Host ""
Write-Host "  ╔══════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "  ║        LMP 0.30 — Commit & Document Helper          ║" -ForegroundColor Cyan
Write-Host "  ╚══════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

$repoRoot = $PSScriptRoot
Set-Location $repoRoot

# ── 1. Verify repo ────────────────────────────────────────────────────────────
Write-Header "Repository status"

$gitDir = Join-Path $repoRoot ".git"
if (-not (Test-Path $gitDir)) {
    Write-Err "No .git directory found at $repoRoot"
    exit 1
}

$remoteUrl = git remote get-url origin 2>&1
Write-Info "Remote : $remoteUrl"

$branch = git rev-parse --abbrev-ref HEAD 2>&1
Write-Info "Branch : $branch"

# ── 2. Show changed files ─────────────────────────────────────────────────────
Write-Header "Pending changes"

$statusLines = git status --short
if (-not $statusLines) {
    Write-Warn "Nothing to commit — working tree is clean."
    if (Prompt-YN "Push anyway (e.g. to sync a previous commit)?") {
        git push origin $branch --set-upstream
        Write-Success "Pushed."
    }
    exit 0
}

$statusLines | ForEach-Object {
    $flag = $_.Substring(0,2)
    $file = $_.Substring(3)
    $color = switch ($flag.Trim()) {
        "M"  { "Yellow" }
        "A"  { "Green"  }
        "D"  { "Red"    }
        "??" { "Gray"   }
        default { "White" }
    }
    Write-Host "    $flag $file" -ForegroundColor $color
}

$changedCount = ($statusLines | Measure-Object).Count
Write-Info ""
Write-Info "$changedCount file(s) affected."

# ── 3. Collect change description ─────────────────────────────────────────────
Write-Header "Document this change"

$shortDesc = Prompt-Line "Short title (1 line)"
while ([string]::IsNullOrWhiteSpace($shortDesc)) {
    Write-Warn "Title cannot be empty."
    $shortDesc = Prompt-Line "Short title (1 line)"
}

$detail = Prompt-Multiline "Optional detail / bullet points"

$category = Prompt-Line "Category" "Fix"
# Normalise to title-case
$category = (Get-Culture).TextInfo.ToTitleCase($category.ToLower())

# ── 4. Update changelog ───────────────────────────────────────────────────────
Write-Header "Updating LMP_0.30_2026.md"

$changelogPath = Join-Path $repoRoot "LMP_0.30_2026.md"
$timestamp     = Get-Date -Format "yyyy-MM-dd HH:mm"

$entry = @"

### [$timestamp] $category — $shortDesc
"@

if (-not [string]::IsNullOrWhiteSpace($detail)) {
    $entry += "`n$detail`n"
}

# Insert after the header block (after the first H2 — "## Version") so entries
# appear at the top of the change log rather than being appended at the bottom.
$existing = Get-Content $changelogPath -Raw
$insertMarker = "---`n`n## Version"
$entry_block  = "---`n`n$($entry.Trim())`n`n---`n`n## Version"
$updated = $existing.Replace($insertMarker, $entry_block)

if ($updated -eq $existing) {
    # Fallback: just append
    Add-Content $changelogPath "`n$entry"
    Write-Warn "Could not find insert marker — entry appended at end."
} else {
    Set-Content $changelogPath $updated -NoNewline
    Write-Success "Entry added to LMP_0.30_2026.md."
}

# ── 5. Build commit message ───────────────────────────────────────────────────
Write-Header "Commit"

$defaultMsg = "$category`: $shortDesc"
$commitMsg  = Prompt-Line "Commit message" $defaultMsg

# ── 6. Stage, commit, push ────────────────────────────────────────────────────
Write-Info ""
Write-Info "Staging all changes..."
git add -A

Write-Info "Committing..."
git commit -m $commitMsg

$pushNow = Prompt-YN "Push to origin/$branch now?"
if ($pushNow) {
    Write-Info "Pushing..."
    git push origin $branch --set-upstream
    Write-Success "Pushed to origin/$branch."
} else {
    Write-Warn "Skipped push. Run: git push origin $branch"
}

# ── 7. Optional release ───────────────────────────────────────────────────────
Write-Host ""
if ($pushNow -and (Prompt-YN "Publish a GitHub release now? (runs release.ps1)")) {
    $relScript = Join-Path $PSScriptRoot "release.ps1"
    if (Test-Path $relScript) {
        & powershell -ExecutionPolicy Bypass -File $relScript
    } else {
        Write-Err "release.ps1 not found at $relScript"
    }
}

Write-Host ""
Write-Success "Done. Have a great session!"
Write-Host ""
