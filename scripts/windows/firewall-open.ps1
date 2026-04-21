#Requires -Version 5.1
#Requires -RunAsAdministrator
<#
.SYNOPSIS
    Open Windows Firewall inbound rules for OpenMU (LAN play).
.DESCRIPTION
    Opens TCP 80 (admin panel), 44405-44406 (connect servers),
    55901-55906 (game servers), 55980 (chat server). MUST be run in an
    elevated PowerShell.
#>

$ErrorActionPreference = 'Stop'

$rules = @(
    @{ Name = 'OpenMU AdminPanel';    Ports = 80 },
    @{ Name = 'OpenMU ConnectServer'; Ports = @(44405, 44406) },
    @{ Name = 'OpenMU GameServers';   Ports = '55901-55906' },
    @{ Name = 'OpenMU ChatServer';    Ports = 55980 }
)

foreach ($rule in $rules) {
    $existing = Get-NetFirewallRule -DisplayName $rule.Name -ErrorAction SilentlyContinue
    if ($existing) {
        Write-Host ("[SKIP] Rule already exists: {0}" -f $rule.Name) -ForegroundColor Yellow
        continue
    }

    New-NetFirewallRule `
        -DisplayName $rule.Name `
        -Direction Inbound `
        -Protocol TCP `
        -LocalPort $rule.Ports `
        -Action Allow | Out-Null

    Write-Host ("[ADD]  {0} (TCP {1})" -f $rule.Name, ($rule.Ports -join ',')) -ForegroundColor Green
}

Write-Host "`nInbound rules installed. Friends on your LAN can now reach your server." -ForegroundColor Cyan
Write-Host "Remember to set Admin Panel -> Configuration -> System -> IP Resolver = Local" -ForegroundColor Cyan
