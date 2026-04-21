#Requires -Version 5.1
<#
.SYNOPSIS
    Stop the OpenMU all-in-one Docker stack.
.PARAMETER Volumes
    Also delete the `dbdata` volume (permanently wipes the database).
#>
param(
    [switch]$Volumes
)

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$composeDir = Join-Path $repoRoot 'deploy/all-in-one'

Push-Location $composeDir
try {
    if ($Volumes) {
        Write-Host "Tearing down containers AND volumes (DB will be wiped)." -ForegroundColor Yellow
        docker compose down -v
    } else {
        docker compose down
    }
} finally {
    Pop-Location
}
