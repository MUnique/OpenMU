# AGENTS.md

This repository is a customized fork of [MUnique/OpenMU](https://github.com/MUnique/OpenMU)
used for running a private MU Online server (local play first, public VPS second).

## Read this first

- **`PLAN.md`** at the repo root — the single source of truth. Contains the
  Windows dev setup, Phase 1 (local) and Phase 2 (public) checklists,
  security-hardening notes, troubleshooting, repo map, ports, and test
  accounts.
- **`.cursor/rules/openmu.mdc`** — the full operating rules for AI agents.
  Every Cursor session auto-loads this. Other agents (Codex, Claude, etc.)
  should read it manually.

## TL;DR operating rules

1. Two goals, in order: **Phase 1 — local play on Windows**, then
   **Phase 2 — public VPS**. Do not help with Phase 2 until Phase 1 is
   verified complete (see `PLAN.md` §7).
2. Windows is the dev OS → give **PowerShell** commands by default.
3. Default deployment is **`deploy/all-in-one/`**. Never suggest
   `deploy/distributed/` — it is broken upstream.
4. Game client must connect via `127.127.127.127` (not `127.0.0.1` —
   the client blocks loopback), LAN IP, or public IP/domain.
5. Connect-server ports: `44405` (original client) / `44406` (open-source
   client) — don't swap.
6. Never commit to `master`. Use feature branches. Meta/plan work lives
   on `my-plan`; code work should live on its own `feature/*` branch.
7. Never commit secrets (`ConnectionSettings.xml` with real passwords,
   SSH keys, cert material, `.env` files, `local/`).
8. Encryption keys (`src/Network` SimpleModulus + Xor32) must be
   regenerated and the client repatched **before** the server is
   publicly advertised. This blocks Phase 2. See `PLAN.md` §9.
9. The MU Online game client is **not** in this repo. Don't generate
   code that assumes it is.
10. StyleCop is enforced. Respect existing code style.

## Environment assumptions

- **Development:** Windows 10/11 with Visual Studio 2026 or Cursor/VS Code,
  .NET SDK 10, Node LTS, PostgreSQL 16/17, Docker Desktop.
- **Production (future):** Ubuntu 22.04/24.04 LTS VPS, Docker + Compose,
  Traefik, Let's Encrypt.

## How to pick up where the previous agent left off

1. Read `PLAN.md` fully.
2. Check the Progress Log in `PLAN.md` §15 for the most recent entry.
3. Find the next unchecked box in §7 (Phase 1) or §8 (Phase 2) and work
   toward it.
4. When a task completes, tick the box in `PLAN.md`, append a line to §15,
   and — if non-trivial — reference the commit SHA.

## Included dev helpers

| Path | Purpose |
|---|---|
| `scripts/windows/bootstrap.ps1` | Verify Windows prerequisites |
| `scripts/windows/docker-up.ps1` | Boot `deploy/all-in-one` stack |
| `scripts/windows/docker-down.ps1` | Tear down all-in-one stack |
| `scripts/windows/dev-run.ps1` | Run `Startup` with recommended flags |
| `scripts/windows/dev-reset-db.ps1` | Reinit the database |
| `scripts/windows/firewall-open.ps1` | Open LAN firewall ports (admin) |
| `scripts/windows/firewall-close.ps1` | Close LAN firewall ports (admin) |
| `.vscode/launch.json` | F5 to run/debug Startup |
| `.vscode/tasks.json` | Build / test / docker / reset-db tasks |

## Don't

- Don't create documentation files proactively. `PLAN.md` is already
  authoritative. Only add new docs when explicitly asked.
- Don't try to modify upstream code style, .gitignore, or solution files
  unless the user asks.
- Don't push to `upstream` — you don't have access, and it's not your
  code to push.
