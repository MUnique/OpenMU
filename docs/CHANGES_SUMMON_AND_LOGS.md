# Recent changes (Summon and Admin Panel)

## Summon (Elf) fixes and scaling

- Summon base stats are now loaded and cloned correctly (EF types) so values are no longer zero.
- Energy-based scaling can be configured from the Admin Panel (plug-in "Elf Summon cfg — 30..36").
  - Formula: `scale = 1 + floor(Energy / EnergyPerStep) * PercentPerStep`.
  - If the plug-in is not active, stored configuration values are still respected.
- Classic behavior restored:
  - Summon follows the owner inside safezone.
  - Summon never aggroes the owner even if the owner hits it.
  - Direct and area skills do not hit own summon (basic attack with CTRL is handled by the client).
- Optional diagnostics: set environment variable `SUMMON_DIAG=1` to enable detailed server logs.

## Live Logs in Admin Panel

- New page: "Logs en Vivo" in the left menu (below "Archivos de Log").
- Endpoint configurable via env var `LOG_TAIL_URL`.
  - Default: `/api/logs/tail?take=200`
  - Example: `https://mu.server-pups.space/api/logs/tail?take=200`
- Includes line count, text filter and auto-refresh (3s).

