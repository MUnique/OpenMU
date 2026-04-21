#Requires -Version 5.1
<#
.SYNOPSIS
    Run MUnique.OpenMU.Startup from source with the recommended flags.
.DESCRIPTION
    Default: -autostart -resolveIP:loopback (same-machine testing).
.PARAMETER Lan
    Use -resolveIP:local so LAN clients can reach the server.
.PARAMETER Public
    Use -resolveIP:public (VPS / internet hosting).
.PARAMETER Demo
    Use -demo (in-memory DB, progress not saved).
.PARAMETER Reinit
    Add -reinit to wipe and re-seed the database.
.PARAMETER ExtraArgs
    Additional arguments to forward to the server.
#>
param(
    [switch]$Lan,
    [switch]$Public,
    [switch]$Demo,
    [switch]$Reinit,
    [string[]]$ExtraArgs
)

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$project = Join-Path $repoRoot 'src/Startup/MUnique.OpenMU.Startup.csproj'

if (-not (Test-Path $project)) {
    Write-Host "Cannot find $project" -ForegroundColor Red
    exit 1
}

$resolveIp = 'loopback'
if ($Lan) { $resolveIp = 'local' }
if ($Public) { $resolveIp = 'public' }

$serverArgs = @('-autostart', "-resolveIP:$resolveIp")
if ($Demo)   { $serverArgs += '-demo' }
if ($Reinit) { $serverArgs += '-reinit' }
if ($ExtraArgs) { $serverArgs += $ExtraArgs }

Write-Host ("dotnet run --project {0} -- {1}" -f $project, ($serverArgs -join ' ')) -ForegroundColor Cyan
dotnet run --project $project -- @serverArgs
