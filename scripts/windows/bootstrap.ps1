#Requires -Version 5.1
<#
.SYNOPSIS
    Verify that all Windows prerequisites for OpenMU development are installed.
.DESCRIPTION
    Checks for Git, .NET SDK 10, Node LTS, PostgreSQL, Docker Desktop, and
    reports which are missing with winget install hints.
.EXAMPLE
    ./scripts/windows/bootstrap.ps1
#>

$ErrorActionPreference = 'Stop'

function Test-Tool {
    param(
        [string]$Name,
        [string]$Command,
        [string[]]$VersionArgs = @('--version'),
        [string]$WingetId,
        [string]$MinVersion
    )

    $found = Get-Command $Command -ErrorAction SilentlyContinue
    if (-not $found) {
        Write-Host ("  [MISSING] {0}  -> winget install --id {1} -e" -f $Name, $WingetId) -ForegroundColor Red
        return $false
    }

    $version = & $Command @VersionArgs 2>&1 | Select-Object -First 1
    Write-Host ("  [OK]      {0}  ({1})" -f $Name, $version) -ForegroundColor Green
    return $true
}

Write-Host "`nOpenMU Windows prerequisites check`n" -ForegroundColor Cyan

$results = @()
$results += Test-Tool -Name 'Git'           -Command 'git'      -WingetId 'Git.Git'
$results += Test-Tool -Name '.NET SDK'      -Command 'dotnet'   -WingetId 'Microsoft.DotNet.SDK.10'
$results += Test-Tool -Name 'Node.js'       -Command 'node'     -WingetId 'OpenJS.NodeJS.LTS'
$results += Test-Tool -Name 'Docker'        -Command 'docker'   -WingetId 'Docker.DockerDesktop'
$results += Test-Tool -Name 'PostgreSQL'    -Command 'psql'     -WingetId 'PostgreSQL.PostgreSQL.17'
# GitHub CLI is optional but helpful
$null = Test-Tool -Name 'GitHub CLI (opt)' -Command 'gh'        -WingetId 'GitHub.cli'

Write-Host "`n.NET SDK detail:" -ForegroundColor Cyan
try {
    dotnet --list-sdks
} catch {
    Write-Host "  (dotnet not on PATH)" -ForegroundColor Yellow
}

Write-Host "`nDocker daemon status:" -ForegroundColor Cyan
try {
    docker info --format '  {{.ServerVersion}} / {{.OSType}}' 2>$null
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  Docker CLI found but daemon is not running. Start Docker Desktop." -ForegroundColor Yellow
    }
} catch {
    Write-Host "  Docker CLI not available." -ForegroundColor Yellow
}

Write-Host "`nSummary:" -ForegroundColor Cyan
$missing = $results | Where-Object { -not $_ }
if ($missing.Count -eq 0) {
    Write-Host "  All required tools present. You can proceed with PLAN.md section 5." -ForegroundColor Green
    exit 0
} else {
    Write-Host ("  {0} required tool(s) missing. See PLAN.md section 4." -f $missing.Count) -ForegroundColor Red
    exit 1
}
