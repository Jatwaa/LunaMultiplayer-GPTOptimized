#Requires -Version 5.1
# Lists all assets on a release and lets you delete selected ones.
# Usage: powershell -ExecutionPolicy Bypass -File remove-asset.ps1

$GH_REPO = "Jatwaa/LunaMultiplayer-GPTOptimized"
$GH_API  = "https://api.github.com"
$tag     = "v0.30.0"

$token = (Get-Content (Join-Path $PSScriptRoot ".github_token") -Raw).Trim()
$headers = @{
    Authorization = "token $token"
    Accept        = "application/vnd.github+json"
    "User-Agent"  = "LMP-release-script/1.0"
}

$release = Invoke-RestMethod -Uri "$GH_API/repos/$GH_REPO/releases/tags/$tag" -Headers $headers
Write-Host ""
Write-Host "  Assets on release $tag :" -ForegroundColor Cyan

$i = 0
$release.assets | ForEach-Object {
    $i++
    Write-Host "  [$i] $($_.name)  ($([math]::Round($_.size/1KB)) KB)" -ForegroundColor White
}

if ($i -eq 0) { Write-Host "  (no assets)" -ForegroundColor Gray; exit 0 }

Write-Host ""
Write-Host "  > Enter number(s) to delete (comma-separated), or ENTER to cancel : " -ForegroundColor Yellow -NoNewline
$input = Read-Host
if ([string]::IsNullOrWhiteSpace($input)) { Write-Host "  Cancelled."; exit 0 }

$input.Split(',') | ForEach-Object {
    $n = $_.Trim()
    if ($n -match '^\d+$') {
        $idx   = [int]$n - 1
        $asset = $release.assets[$idx]
        Write-Host "  Deleting: $($asset.name)..." -ForegroundColor Yellow
        Invoke-RestMethod -Method DELETE `
            -Uri "$GH_API/repos/$GH_REPO/releases/assets/$($asset.id)" `
            -Headers $headers | Out-Null
        Write-Host "  [OK] Deleted: $($asset.name)" -ForegroundColor Green
    }
}

Write-Host ""
