#Requires -Version 5.1
<#
.SYNOPSIS
    Reinitialize the OpenMU PostgreSQL database.
.DESCRIPTION
    Wrapper around dev-run.ps1 that adds -reinit. Wipes the DB schema and
    re-seeds it (including test accounts). This is DESTRUCTIVE.
#>

$ErrorActionPreference = 'Stop'

Write-Host "This will WIPE and re-seed the OpenMU PostgreSQL database." -ForegroundColor Yellow
$confirm = Read-Host "Type 'yes' to continue"
if ($confirm -ne 'yes') {
    Write-Host "Aborted." -ForegroundColor Red
    exit 1
}

& (Join-Path $PSScriptRoot 'dev-run.ps1') -Reinit
