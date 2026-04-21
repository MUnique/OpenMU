# OpenMU Private Server — Project Plan (Windows Dev Edition)

> **Status:** Active plan
> **Owner:** Edwin Cajayon
> **Repo:** Fork/clone of [MUnique/OpenMU](https://github.com/MUnique/OpenMU)
> **End goal:** (1) Run an OpenMU MU Online private server **locally on Windows** to play with friends, then (2) deploy it **publicly** on a Linux VPS.

This file is the single source of truth for the project. It's written so that **both a human developer and an AI coding agent** can open this repo, read `PLAN.md`, and know exactly what the state is, what to do next, and what the hard constraints are.

---

## 0. Table of Contents

1. [Context & Goals](#1-context--goals)
2. [What OpenMU is (and is not)](#2-what-openmu-is-and-is-not)
3. [Windows Migration Strategy](#3-windows-migration-strategy)
4. [Windows Prerequisites (install once)](#4-windows-prerequisites-install-once)
5. [Initial Setup on Windows](#5-initial-setup-on-windows)
6. [Daily Development Workflow](#6-daily-development-workflow)
7. [Phase 1 — Local Play (Windows)](#7-phase-1--local-play-windows)
8. [Phase 2 — Public VPS Deployment](#8-phase-2--public-vps-deployment)
9. [Security Hardening: Encryption Keys](#9-security-hardening-encryption-keys)
10. [Troubleshooting Reference](#10-troubleshooting-reference)
11. [Repository Map](#11-repository-map)
12. [Ports Reference](#12-ports-reference)
13. [Test Accounts](#13-test-accounts)
14. [Instructions for the AI Agent](#14-instructions-for-the-ai-agent)
15. [Progress Log](#15-progress-log)
16. [Included Dev Helpers (Windows)](#16-included-dev-helpers-windows)

---

## 1. Context & Goals

| # | Goal | Priority | Status |
|---|------|----------|--------|
| G1 | Get the OpenMU server running locally on Windows | P0 | ☐ Not started |
| G2 | Connect an MU Online Season 6 EP3 ENG client and log in with a test account | P0 | ☐ Not started |
| G3 | Let LAN friends connect to the local server | P1 | ☐ Not started |
| G4 | Deploy the server to a public Linux VPS with HTTPS + domain | P1 | ☐ Not started |
| G5 | Regenerate encryption keys and repatch the client before going public | P1 (security) | ☐ Not started |
| G6 | Automate backups + cert renewal on the VPS | P2 | ☐ Not started |
| G7 | Learn / optionally modify the server codebase | P3 | ☐ Stretch |

Keep updating the status column as work progresses. The AI agent should update this table whenever a goal is completed.

---

## 2. What OpenMU is (and is not)

**OpenMU is a clean-room C# / .NET 10 rewrite** of an MMORPG server compatible with the MU Online network protocol. It is **not** based on decompiled Webzen binaries or any of the classical "MuServer" forks (IGCN, ExDB, JPN, etc.).

Because of that, the mental model is different from old private-server guides:

| Classical MU private-server concept | OpenMU equivalent |
|---|---|
| `ConnectServer.exe` | `src/ConnectServer` library, hosted in one process |
| `GameServer.exe` (per sub-server) | `src/GameServer` + `src/GameLogic` libraries |
| `DataServer.exe` | `src/Persistence/EntityFramework` talking to PostgreSQL |
| `JoinServer.exe` | `src/LoginServer` (in-memory only) |
| `ChatServer.exe` (Webzen) | `src/ChatServer` (open-source re-implementation) |
| ExDB | Not needed — replaced. A legacy adapter lives in `src/ChatServer/ExDbConnector`. |
| AdminTool / GM tool | `src/Web/AdminPanel` (Blazor Server UI on port 80) |
| Client | **Not in repo.** You must supply a legal Season 6 EP3 ENG client. |

All sub-servers can run **in one process** (default, "all-in-one") or be split across Docker containers via Dapr ("distributed"). **The distributed mode is currently broken upstream**, so we use all-in-one.

Two accepted deployment patterns in this repo:

- `deploy/all-in-one/` — single compose file, nginx + Postgres + OpenMU. **This is what we use.**
- `deploy/all-in-one-traefik/` — same plus Traefik reverse proxy with Let's Encrypt. **This is the public-production target.**

---

## 3. Windows Migration Strategy

The current clone on the Mac has **no local modifications** (`git status` is clean), so the right move is a **fresh clone on Windows** rather than copying the whole folder. This avoids:
- Line-ending issues (CRLF vs LF — `.gitattributes` handles this on a fresh clone).
- macOS-specific detritus (`.DS_Store`, extended attrs).
- A ~stale Git config tied to the Mac SSH key.

The only thing that must travel is **this `PLAN.md`** so you don't lose context.

### Transfer options

**Option A — recommended: GitHub fork of your own**
1. On GitHub, fork `MUnique/OpenMU` to your own account (e.g. `yourname/OpenMU`).
2. On the Mac, add PLAN.md and push it to your fork on a branch:
   ```bash
   cd /Users/edwincajayon/Cursor-only/OpenMU
   git checkout -b my-plan
   git add PLAN.md
   git commit -m "Add project plan"
   git remote add mine git@github.com:YOURUSER/OpenMU.git
   git push mine my-plan
   ```
3. On Windows, clone your fork (see §5). Your plan travels with the repo forever.

**Option B — quick: copy just PLAN.md**
1. Email / USB / iCloud / OneDrive this one file to Windows.
2. On Windows, fresh-clone upstream OpenMU (see §5) and drop PLAN.md at the repo root.

**Option C — not recommended: copy the whole Mac folder**
Works, but you'll likely have line-ending and file-permission noise on first build.

---

## 4. Windows Prerequisites (install once)

Run all of these on the Windows machine. PowerShell commands assume an elevated shell.

### Core tools
```powershell
winget install --id Git.Git -e
winget install --id GitHub.cli -e            # optional but handy
winget install --id Microsoft.DotNet.SDK.10 -e
winget install --id Microsoft.VisualStudio.2026.Community -e    # OR Professional/Enterprise
winget install --id OpenJS.NodeJS.LTS -e
winget install --id PostgreSQL.PostgreSQL.17 -e                 # or 16
winget install --id Docker.DockerDesktop -e
```
(If `Microsoft.VisualStudio.2026.Community` is not yet in winget for your system, install from https://visualstudio.microsoft.com. Enable the workloads **"ASP.NET and web development"** and **".NET desktop development"**.)

### Visual Studio workloads
- ASP.NET and web development
- .NET desktop development
- Optional: **Web Compiler 2022+** extension (only if editing SCSS for the admin panel)

### Alternative IDEs (pick one)
- Visual Studio 2026 Community/Professional (best on Windows)
- JetBrains Rider (great alternative)
- VS Code + C# Dev Kit (lightweight; fine for most work)

### Git configuration (do this once on Windows)
```powershell
git config --global user.name  "Your Name"
git config --global user.email "you@example.com"
git config --global core.autocrlf true
git config --global init.defaultBranch master
```
`core.autocrlf true` + the repo's `.gitattributes` (`* text=auto`) keeps files LF in Git and CRLF in your working tree — correct for Windows.

### PostgreSQL setup
During install, set the `postgres` superuser password. The OpenMU default `ConnectionSettings.xml` expects **user `postgres` / password `admin`**. You can either:
- Use `admin` during install (matches defaults — easiest), or
- Use any password and edit the first two `<Connection>` entries in `src/Persistence/EntityFramework/ConnectionSettings.xml`.

Add PostgreSQL's `bin` folder to your `PATH` so `psql` works in any terminal.

### Docker
Enable the WSL2 backend in Docker Desktop (this is the default on modern Windows 10/11). You don't actually need to write any WSL-specific code — OpenMU's containers just run through Docker.

---

## 5. Initial Setup on Windows

Choose **one** path. You can even do both (Docker to play, manual to code).

### Path A — Docker only (fastest, no code changes)

```powershell
git clone https://github.com/MUnique/OpenMU.git
# OR your fork:  git clone https://github.com/YOURUSER/OpenMU.git
cd OpenMU\deploy\all-in-one
docker compose up -d --no-build
```
Wait ~60 seconds, then:
```powershell
docker compose ps
docker compose logs -f openmu-startup
```
Open http://localhost/ → login `admin` / `openmu` → change password → in **Configuration → System** enable **Auto Start** and set **IP Resolver → Loopback** (for single-machine test) or **Local** (for LAN).

### Path B — Manual dev build (recommended if you want to code)

```powershell
git clone https://github.com/MUnique/OpenMU.git
cd OpenMU
./scripts/windows/bootstrap.ps1                     # verify prereqs
dotnet restore src\MUnique.OpenMU.sln
dotnet build   src\MUnique.OpenMU.sln -c Debug
```

Edit `src\Persistence\EntityFramework\ConnectionSettings.xml` **only if** your Postgres password isn't `admin`. Only the first two `<Connection>` entries need real admin credentials — the others (`config`, `account`, `friend`, `guild`) are auto-created with scoped privileges on first run.

Run the server (either way works):
```powershell
# via the helper
./scripts/windows/dev-run.ps1                       # loopback (same machine)
./scripts/windows/dev-run.ps1 -Lan                  # LAN friends
./scripts/windows/dev-run.ps1 -Demo                 # in-memory, no DB

# or manually
cd src\Startup
dotnet run -- -autostart -resolveIP:loopback
```

Useful startup flags (fully documented in `src/Startup/Readme.md`):
- `-reinit` — wipe & re-seed the database
- `-demo` — in-memory mode, no Postgres, nothing is saved
- `-version:season6` (default) / `-version:0.95d` / `-version:0.75`
- `-adminpanel:enabled|disabled`

Then open http://localhost/ and continue like Path A.

### Verifying it works
In the Admin Panel you should see:
- 3 Game Servers (sub-servers)
- 1 Chat Server
- 2 Connect Servers (ports 44405 & 44406)

Start the Connect Servers and at least one Game Server. Logs in the console / admin panel should show "Listening on ..." lines for the expected ports.

---

## 6. Daily Development Workflow

### Branching
Never commit to `master`. Create feature branches:
```powershell
git checkout master
git pull
git checkout -b feature/my-change
```

### Build / run / test
```powershell
dotnet build  src\MUnique.OpenMU.sln -c Debug
dotnet test   src\MUnique.OpenMU.sln -c Debug
cd src\Startup
dotnet run -- -autostart -resolveIP:loopback
```

### Debugging
- Visual Studio: set `MUnique.OpenMU.Startup` as Startup Project, press F5.
- Rider: same — create a Run Config for `Startup`.
- VS Code / Cursor: **F5 is pre-wired** via `.vscode/launch.json`. Four
  configs are included: loopback, LAN, demo (in-memory), reinit.
- Common build/test/docker commands are also wired into `.vscode/tasks.json`
  (Ctrl+Shift+P → "Tasks: Run Task").

Test account to log in with: `test0` / `test0` (or `test400` / `test400` for a lvl-400 character).

### Coding style
- **StyleCop** is enforced (`src/stylecop.json`). You'll see rule violations as warnings in VS.
- Project explicitly rejects AI-generated code that hasn't been tested — if you're contributing upstream, test first.

### Keeping in sync with upstream
```powershell
git remote add upstream https://github.com/MUnique/OpenMU.git    # once
git fetch upstream
git checkout master
git merge upstream/master
```

---

## 7. Phase 1 — Local Play (Windows)

**Status:** ☐ Not started

### Goals (Phase 1)
- G1 Server runs locally
- G2 Client connects and logs in
- G3 LAN friends can join

### Checklist
1. ☐ Docker Desktop running
2. ☐ Repo cloned
3. ☐ `docker compose up -d --no-build` in `deploy/all-in-one` succeeds
4. ☐ Admin panel reachable at http://localhost/
5. ☐ Changed `admin` / `openmu` password to something else
6. ☐ 3 game servers + 2 connect servers + 1 chat server visible in admin panel
7. ☐ Auto Start enabled; all listeners green
8. ☐ Downloaded `MUnique.OpenMU.ClientLauncher_0.9.6.zip` from [GitHub releases](https://github.com/MUnique/OpenMU/releases)
9. ☐ .NET 10 runtime installed (the launcher needs it)
10. ☐ Obtained a legal MU Online Season 6 EP3 ENG client
11. ☐ Launcher configured with host `127.127.127.127` (same-machine test) and port `44405`
12. ☐ Logged in as `test0` / `test0` — character appears in-game
13. ☐ (For friends) Windows Firewall inbound rules added for 80, 44405, 44406, 55901–55906, 55980
14. ☐ (For friends) Friends connect using your **LAN IPv4** (run `ipconfig` to find it)
15. ☐ In admin panel, set **IP Resolver → Local** so the connect server tells friends your LAN IP, not 127.*

### Windows Firewall rules (admin PowerShell)
```powershell
./scripts/windows/firewall-open.ps1     # add the rules
./scripts/windows/firewall-close.ps1    # remove them later
```
Or manually:
```powershell
New-NetFirewallRule -DisplayName "OpenMU AdminPanel"    -Direction Inbound -Protocol TCP -LocalPort 80 -Action Allow
New-NetFirewallRule -DisplayName "OpenMU ConnectServer" -Direction Inbound -Protocol TCP -LocalPort 44405,44406 -Action Allow
New-NetFirewallRule -DisplayName "OpenMU GameServers"   -Direction Inbound -Protocol TCP -LocalPort 55901-55906 -Action Allow
New-NetFirewallRule -DisplayName "OpenMU ChatServer"    -Direction Inbound -Protocol TCP -LocalPort 55980 -Action Allow
```

### Client launcher quick-reference
| Scenario | Launcher host |
|---|---|
| Client on same PC as server | `127.127.127.127` (**never** `127.0.0.1` — client blocks it) |
| Client on LAN | Your PC's LAN IPv4 (e.g. `192.168.1.50`) |
| Original Webzen client | Port `44405` |
| Open-source MuMain client | Port `44406` |

---

## 8. Phase 2 — Public VPS Deployment

**Status:** ☐ Not started — **do not start until Phase 1 is 100% working**.

### Target stack
- Ubuntu 22.04 / 24.04 LTS
- 2 vCPU / 4 GB RAM / 40 GB SSD (scale up as needed)
- Static IPv4
- Domain name (A record → VPS IP)
- `deploy/all-in-one-traefik/` compose (HTTPS via Let's Encrypt)

### Checklist
1. ☐ VPS provisioned; SSH access working
2. ☐ Domain A record pointed to VPS IP
3. ☐ Docker + Docker Compose installed on VPS (`curl -fsSL https://get.docker.com | sh`)
4. ☐ Repo cloned to `/opt/OpenMU` (or `/home/<user>/OpenMU`)
5. ☐ `DOMAIN_NAME=play.yourdomain.com` exported (or baked into `docker-compose.prod.yml`)
6. ☐ Certbot one-shot run succeeded and produced certificates
7. ☐ `docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d` healthy
8. ☐ https://play.yourdomain.com/ reachable with valid cert
9. ☐ Admin `admin`/`openmu` password changed
10. ☐ Auto Start enabled, IP Resolver = Public
11. ☐ UFW / firewall rules: 22 (SSH), 80, 443, 44405, 44406, 55901–55906, 55980 open; **5432 NOT open**
12. ☐ Encryption keys regenerated (see §9) — **do not skip before advertising publicly**
13. ☐ Nightly Postgres backup cron
14. ☐ Weekly certbot-renew cron

### Firewall (UFW) on the VPS
```bash
sudo ufw allow 22/tcp
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw allow 44405/tcp
sudo ufw allow 44406/tcp
sudo ufw allow 55901:55906/tcp
sudo ufw allow 55980/tcp
sudo ufw enable
sudo ufw status
```

### Cert renewal cron
```
0 3 * * 0 cd /opt/OpenMU/deploy/all-in-one-traefik && docker compose -f docker-compose.yml -f docker-compose.prod.yml run --rm certbot renew
```

### Backup cron (nightly Postgres dump)
```
0 4 * * * docker exec database pg_dumpall -U postgres > /opt/openmu-backups/openmu-$(date +\%F).sql
# Retention (keep 7 days)
5 4 * * * find /opt/openmu-backups/ -name 'openmu-*.sql' -mtime +7 -delete
```

### Sizing guidance
| Concurrent players | vCPU | RAM | Disk |
|---|---|---|---|
| ≤ 30 | 2 | 4 GB | 40 GB SSD |
| 30–200 | 4 | 8 GB | 40 GB SSD |
| 200+ | 6–8 | 16 GB+ | 80 GB SSD |

Since distributed mode is currently broken, scale **vertically** for now.

---

## 9. Security Hardening: Encryption Keys

**This is the single step most guides skip.** OpenMU's `src/Network/Readme.md` explicitly warns that the shipped SimpleModulus + Xor32 keys have been publicly known for 10+ years.

Before you let strangers connect to your server, you must:

1. Build `src/SimpleModulusKeyGenerator` and produce a new key set.
2. Replace the keys used by `MUnique.OpenMU.Network` (the `Pipelined*Encryptor`/`Decryptor` classes reference the default keys — find where they load them and swap in your new ones).
3. Patch your distributed client's binary so it uses the matching keys. This is an offline hex-edit of `Main.exe`. Community tools for this exist; the OpenMU Discord is the right place to ask. This is **outside the scope of this repo** but **inside the scope of your responsibility** if you go public.
4. Distribute **only** the repatched client to your players.

Status: ☐ Not started. **Block Phase 2 public advertising on this being done.**

---

## 10. Troubleshooting Reference

| Symptom | Cause | Fix |
|---|---|---|
| Client won't connect to `127.0.0.1` | Client blocks loopback explicitly | Use any other `127.x.x.x`, e.g. `127.127.127.127` |
| Client connects to the list but disconnects on server select | Wrong IP resolver | Admin Panel → Configuration → System → set IP Resolver appropriately (Loopback / Local / Public / custom) |
| Port 80 already in use on Windows | IIS / Skype / another app | Either stop it, or edit `deploy/all-in-one/docker-compose.yml` to map `"8080:80"` |
| `docker compose` fails with "name in use" | Previous run still there | `docker compose down` then retry |
| Postgres auth fails during migrations | Wrong pw in `ConnectionSettings.xml` | Match the superuser pw chosen during Postgres install |
| "Version mismatch" warnings in logs | Client on wrong Connect Server port (44405 vs 44406) | Correct port in launcher |
| Distributed compose doesn't boot | Mode is **broken upstream** | Don't use it; stick to all-in-one |
| Build error: SDK 10 not found | .NET 10 SDK missing | `winget install Microsoft.DotNet.SDK.10` |
| Line-ending weirdness on first commit | `core.autocrlf` not set | `git config --global core.autocrlf true` + fresh clone |
| Admin panel 500 error on first load | DB still seeding | Wait 1–2 minutes; watch `docker compose logs -f openmu-startup` |

---

## 11. Repository Map

```
OpenMU/
├── PLAN.md                   ← THIS FILE
├── README.md / QuickStart.md
├── CONTRIBUTING.md
├── deploy/
│   ├── all-in-one/            ← Phase 1 stack (Docker)
│   ├── all-in-one-traefik/    ← Phase 2 stack (Docker + HTTPS)
│   └── distributed/           ← BROKEN; do not use
├── docs/                      ← Architecture + packet docs
├── src/
│   ├── Startup/               ← Main EXE (entry point)
│   ├── ConnectServer/         ← Server list / first TCP entrypoint
│   ├── GameServer/            ← World server (sub-servers)
│   ├── GameLogic/             ← Gameplay rules
│   ├── LoginServer/           ← In-memory login arbitration
│   ├── ChatServer/            ← OS chat server (+ ExDbConnector adapter)
│   ├── FriendServer/          ← Friend list / whisper
│   ├── GuildServer/           ← Guild service
│   ├── DataModel/             ← Pure C# domain model
│   ├── Persistence/           ← IContext abstraction
│   │   ├── EntityFramework/   ← Postgres impl (+ ConnectionSettings.xml)
│   │   ├── InMemory/          ← -demo mode
│   │   └── Initialization/    ← Seeds data + test accounts
│   ├── Network/               ← Wire protocol + encryption
│   │   └── Analyzer/          ← Packet sniffer/MITM tool
│   ├── AttributeSystem/       ← Damage/stat framework
│   ├── PlugIns/               ← Plug-in host (every game feature is one)
│   ├── Pathfinding/
│   ├── Dapr/                  ← Distributed glue (unused by us)
│   ├── Web/
│   │   ├── AdminPanel/        ← Blazor Server UI (port 80)
│   │   ├── Map/               ← WebGL live map
│   │   ├── ItemEditor/
│   │   └── Shared/
│   ├── ClientLauncher/        ← WinForms launcher for Main.exe
│   ├── ClientErrorLogDecryptor/ ← CLI for MuError.log
│   ├── SimpleModulusKeyGenerator/ ← For §9 key regeneration
│   └── MUnique.OpenMU.sln
└── tests/                     ← xUnit tests
```

---

## 12. Ports Reference

| Port | Service | Exposed publicly? |
|---|---|---|
| 80 | Admin Panel (HTTP) | Phase 1: yes (LAN). Phase 2: yes (redirects to 443). |
| 443 | Admin Panel (HTTPS via Traefik) | Phase 2 only |
| 44405 | Connect Server — original client | **Yes, always** |
| 44406 | Connect Server — open-source client | **Yes, always** |
| 55901–55906 | Game Servers (one per sub-server) | **Yes, always** |
| 55980 | Chat Server | **Yes, always** |
| 5432 | PostgreSQL | **Never** — internal to Docker network |

---

## 13. Test Accounts

Created automatically on first DB init. **Username == password.**

- `test0`–`test9`: levels 1 through 90
- Season 6 only:
  - `test300` / `test400` — lvl 300 / 400 characters
  - `testgm`, `testgm2` — GM accounts
  - `testunlock` — unlocked classes, no chars
  - `quest1`, `quest2`, `quest3` — quest test chars
  - `ancient`, `socket` — item-set test chars

Admin Panel default: **`admin` / `openmu`** — change immediately.

---

## 14. Instructions for the AI Agent

> This section is for any AI coding assistant (Cursor, Claude, GPT, etc.) picking up this project. Read it before asking the user anything.

### Your operating rules for this project
1. **Default deployment is `deploy/all-in-one/`.** Do not suggest `deploy/distributed/` unless the user explicitly asks — it's broken upstream.
2. **Phase order is hard.** Phase 1 (local) must be done before Phase 2 (public). Don't skip ahead.
3. **Windows is the dev OS.** All terminal commands you give should be **PowerShell** unless the user says "on the VPS" or "on the Linux box."
4. **.NET 10, C#, StyleCop.** Respect the existing style; don't introduce new code styles.
5. **Never commit to `master`.** Always use feature branches.
6. **Never check in secrets.** `ConnectionSettings.xml` with real passwords, VPS SSH keys, certbot material, and `.env` with domain / DB creds stay out of Git.
7. **Do not modify the upstream-facing files unnecessarily.** The user's strategic files are `PLAN.md` (this file) and anything in a `local/` or `notes/` folder if created. When adding personal notes prefer new files to edits in existing upstream code.
8. **Encryption keys (§9) are a blocker for public launch.** If the user moves toward Phase 2 without completing §9, remind them.
9. **The game client is not in this repo.** Don't generate code that assumes it is; don't try to download it.
10. **Default client connection host for same-machine testing is `127.127.127.127`, NOT `127.0.0.1`** — the client blocks loopback.

### How to pick up where we left off
1. Read this whole file.
2. Read `QuickStart.md` for canonical setup.
3. Read `src/Startup/Readme.md` for CLI flags.
4. Check the **Progress Log** in §15 for the most recent status.
5. Look at the checkboxes in §7 (Phase 1) and §8 (Phase 2). Find the next unchecked item and work toward it.
6. When a step is completed, update the checkbox and append a line to §15.

### Preferred tool order when exploring
1. `Read` for specific files you already know.
2. `Grep` / `Glob` for code search.
3. `SemanticSearch` only for architectural questions.
4. `Shell` for git, dotnet, docker commands — never for file editing.
5. Use `StrReplace` / `Write` for file edits.

### When in doubt, ask the user
Only ask structured multi-choice questions about **intent** (what they want), not about **facts** you can look up in the repo.

---

## 15. Progress Log

Append newest entries at the top. Format: `YYYY-MM-DD — who — what`.

---

## 16. Included Dev Helpers (Windows)

This fork ships extra tooling so you can start coding on Windows without any
manual wiring. Everything below lives on the `my-plan` branch.

### AI agent configuration

| File | What it does |
|---|---|
| `AGENTS.md` | Standard cross-agent handoff file. Read by Codex, Claude Code, and others. Points at this PLAN.md and the Cursor rules. |
| `.cursor/rules/openmu.mdc` | Auto-loaded by every Cursor session. Contains the operating rules (Phase gating, PowerShell default, `127.127.127.127` rule, distributed-is-broken, never-commit-to-master, etc.). |

### VS Code / Cursor workspace

| File | What it does |
|---|---|
| `.vscode/launch.json` | F5 to run/debug `MUnique.OpenMU.Startup` with 4 profiles: loopback, LAN, demo (in-memory), and reinit. |
| `.vscode/tasks.json` | Ctrl+Shift+P → "Tasks: Run Task" gives you: build / build (release) / restore / test / clean / run Startup / reset DB / docker up / docker down / docker logs. |
| `.vscode/settings.json` | Format-on-save, LF line endings (CRLF for `.ps1`), search excludes for `bin/obj`, C#/JSON/Markdown/YAML formatter defaults, PowerShell as default Windows terminal. |
| `.vscode/extensions.json` | VS Code / Cursor will prompt to install the recommended extensions on first open: C# Dev Kit, Docker, PowerShell, EditorConfig, YAML, Markdown All-in-One, GitLens. |

A narrow override at the bottom of `.gitignore` un-ignores **only** those four
`.vscode/*.json` files. Any other file you drop into `.vscode/` (e.g. personal
`launch.local.json`) stays ignored automatically.

### PowerShell helper scripts

Run from repo root.

| Script | Purpose |
|---|---|
| `scripts/windows/bootstrap.ps1` | Verify Git, .NET SDK 10, Node LTS, PostgreSQL, Docker are installed; report missing tools with `winget` install hints; show `dotnet --list-sdks` and Docker daemon status. |
| `scripts/windows/docker-up.ps1` | `docker compose up -d --no-build` in `deploy/all-in-one/` and then tail the `openmu-startup` logs. `-NoLogs` to skip the tail, `-Build` to build images locally. |
| `scripts/windows/docker-down.ps1` | `docker compose down`. `-Volumes` also deletes the `dbdata` volume (DB wipe). |
| `scripts/windows/dev-run.ps1` | `dotnet run` Startup with the right flags. Defaults to loopback. `-Lan`, `-Public`, `-Demo`, `-Reinit`, and `-ExtraArgs` supported. |
| `scripts/windows/dev-reset-db.ps1` | Confirm-and-reinit helper (wraps `dev-run.ps1 -Reinit`). |
| `scripts/windows/firewall-open.ps1` | Requires admin PowerShell. Adds inbound Windows Firewall rules for 80, 44405–44406, 55901–55906, 55980. Idempotent. |
| `scripts/windows/firewall-close.ps1` | Requires admin PowerShell. Removes those same rules. |
| `scripts/windows/README.md` | Quick reference for the scripts. |

If PowerShell blocks a script with an "execution policy" error, run this once
(as your user, not admin):
```powershell
Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned
```

### Not committed on purpose

| Path | Why excluded |
|---|---|
| `local/` | Personal scratch notes, secrets, experimental configs. Gitignored. |
| `.env` | Any real deployment `.env` (domain names, admin emails for certbot). Generate from `.env.example` when Phase 2 is reached. |
| `src/Persistence/EntityFramework/ConnectionSettings.xml` (with real credentials) | If you commit real Postgres passwords, rewrite history and change the password. |

### Order of use (first time on Windows)

1. `scripts/windows/bootstrap.ps1` — green rows everywhere.
2. Pick Path A (`docker-up.ps1`) or Path B (`dev-run.ps1`) from §5.
3. Open Cursor / VS Code — accept the recommended extensions prompt.
4. F5 from `.vscode/launch.json` to debug, or Ctrl+Shift+P → Tasks for build/test.
5. Open `http://localhost/` and check Phase 1 boxes in §7 as you go.

- **2026-04-21 — AI (Claude) — added Windows-ready loadout on `my-plan` branch: `.cursor/rules/openmu.mdc` (agent rules), `AGENTS.md` (cross-agent handoff), `.vscode/{launch,tasks,settings,extensions}.json` (F5 debug + build/test/docker tasks), `scripts/windows/*.ps1` helpers (bootstrap, dev-run, dev-reset-db, docker-up/down, firewall-open/close), and a narrow `.gitignore` override to un-ignore just the four committed .vscode configs. PLAN.md §16 added.**
- **2026-04-21 — AI (Claude) — created this PLAN.md on macOS before Windows migration. Analysis of all 22 README files complete. Clone is clean (no local commits). Goals confirmed: Phase 1 local → Phase 2 public. Next action for user: re-clone on Windows (see §3, Option A or B).**
