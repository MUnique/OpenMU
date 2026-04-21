#Requires -Version 5.1
<#
.SYNOPSIS
    Boot the OpenMU all-in-one Docker stack (Postgres + OpenMU + nginx).
.DESCRIPTION
    Wraps `docker compose up -d --no-build` in deploy/all-in-one and then
    follows the logs so you can see startup progress.
.PARAMETER Logs
    After starting, follow the openmu-startup logs. Default: true.
.PARAMETER Build
    Build images locally instead of pulling munique/openmu. Default: false.
#>
param(
    [switch]$NoLogs,
    [switch]$Build
)

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$composeDir = Join-Path $repoRoot 'deploy/all-in-one'

if (-not (Test-Path $composeDir)) {
    Write-Host "Cannot find $composeDir" -ForegroundColor Red
    exit 1
}

Push-Location $composeDir
try {
    $args = @('compose', 'up', '-d')
    if (-not $Build) { $args += '--no-build' }

    Write-Host ("docker {0}" -f ($args -join ' ')) -ForegroundColor Cyan
    & docker @args
    if ($LASTEXITCODE -ne 0) { throw "docker compose up failed" }

    Write-Host "`nContainers:" -ForegroundColor Cyan
    docker compose ps

    Write-Host "`nAdmin panel will be at http://localhost/ (default admin/openmu)" -ForegroundColor Green
    Write-Host "Allow ~60 seconds on first boot while the DB seeds Season 6 data.`n"

    if (-not $NoLogs) {
        Write-Host "Tailing openmu-startup logs (Ctrl+C to stop — containers keep running):`n" -ForegroundColor Cyan
        docker compose logs -f openmu-startup
    }
} finally {
    Pop-Location
}
