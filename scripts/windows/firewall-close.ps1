#Requires -Version 5.1
#Requires -RunAsAdministrator
<#
.SYNOPSIS
    Remove Windows Firewall inbound rules installed by firewall-open.ps1.
#>

$ErrorActionPreference = 'Stop'

$names = @(
    'OpenMU AdminPanel',
    'OpenMU ConnectServer',
    'OpenMU GameServers',
    'OpenMU ChatServer'
)

foreach ($name in $names) {
    $rule = Get-NetFirewallRule -DisplayName $name -ErrorAction SilentlyContinue
    if (-not $rule) {
        Write-Host ("[SKIP] Not present: {0}" -f $name) -ForegroundColor Yellow
        continue
    }

    Remove-NetFirewallRule -DisplayName $name
    Write-Host ("[DEL]  {0}" -f $name) -ForegroundColor Green
}

Write-Host "`nInbound rules removed." -ForegroundColor Cyan
