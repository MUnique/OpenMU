# OpenMU — Run with VS Code (Second PC Guide)

> Step-by-step for cloning, building, and running this fork on a second Windows PC using Visual Studio Code.
> Companion docs: `SETUP-WINDOWS.md` (general fresh-PC setup), `SERVER.md` (daily start/stop cheat-sheet), `faq.md` (deep gotchas), `GMchat.md` (in-game GM commands).

---

## Quick reference — Start / Stop the server

Run these from the repo root (`...\Rotating Earth\OpenMU`) in a **PowerShell** terminal inside VS Code (`` Ctrl+` ``).

**Start:**

```powershell
./scripts/windows/dev-run.ps1
```

Wait ~12 seconds for the `Host started` log line. Leave that terminal open — it's your live log.

**Stop (server is in your current terminal):**

Press **Ctrl+C** in that terminal. Wait for graceful shutdown.

**Stop (terminal got closed or server is detached):**

```powershell
Get-Process -Name MUnique.OpenMU.Startup -ErrorAction SilentlyContinue | Stop-Process -Force
```

**Check whether it's running:**

```powershell
Get-Process -Name MUnique.OpenMU.Startup -ErrorAction SilentlyContinue | Select-Object Id, StartTime
Get-NetTCPConnection -LocalPort 80 -State Listen -ErrorAction SilentlyContinue
```

If both return rows, it's up. If both are empty, it's down.

**Start variations:**

```powershell
./scripts/windows/dev-run.ps1 -Lan       # bind to LAN IP so friends can connect
./scripts/windows/dev-run.ps1 -Reinit    # WIPE + reseed DB (back up first!)
./scripts/windows/dev-run.ps1 -Demo      # in-memory DB, nothing persists
```

> **First time on a fresh clone?** You must run `dotnet build src\MUnique.OpenMU.sln -c Release` **once** before the start command will work. See §6.

---

## 1. Prerequisites (one-time per PC)

Open **PowerShell as Administrator** (right-click Start → "Terminal (Admin)") and run:

```powershell
winget install --id Git.Git -e
winget install --id Microsoft.DotNet.SDK.10 -e
winget install --id PostgreSQL.PostgreSQL.17 -e --override "--unattendedmodeui none --mode unattended --superpassword admin"
winget install --id Microsoft.VisualStudioCode -e
```

What this installs:

- **Git** — to clone/pull the repo
- **.NET 10 SDK** — `dotnet` command
- **PostgreSQL 17** — superuser `postgres` / password `admin`, service `postgresql-x64-17` auto-starting, installed to `C:\Program Files\PostgreSQL\17\`
- **VS Code**

Close the admin window. Open a **new normal** PowerShell window and verify:

```powershell
git --version
dotnet --version                  # should print 10.x.x
Get-Service postgresql-x64-17     # Status: Running
```

---

## 2. Pull the repo

**First time on this PC:**

```powershell
cd $HOME\source\repos       # or wherever you keep code
git clone https://github.com/edwin-cajayon/OpenMU.git
cd OpenMU
git checkout my-plan
```

**Already cloned before:**

```powershell
cd <path-to-your-OpenMU-folder>
git checkout my-plan
git pull
```

Confirm latest commit is present:

```powershell
git log --oneline -5
```

---

## 3. Open in VS Code

From the same terminal:

```powershell
code .
```

Or **File → Open Folder...** → pick the `OpenMU` folder.

Install recommended extensions when prompted:

- **C# Dev Kit** (Microsoft) — C# IntelliSense, build, debug
- **PowerShell** (Microsoft) — `.ps1` syntax

If asked, click **Yes, I trust the authors**.

---

## 4. Open a PowerShell terminal inside VS Code

Press `` Ctrl+` `` (backtick), or **Terminal → New Terminal**.

Make sure the shell dropdown on the right side of the terminal panel says **PowerShell** (not Command Prompt or Git Bash). If wrong:

1. Click the dropdown → **Select Default Profile** → **PowerShell**
2. Trash the current terminal (trash icon) and open a fresh one

If the very first script run gets blocked by execution policy, run once:

```powershell
Set-ExecutionPolicy -Scope CurrentUser RemoteSigned
```

---

## 5. Create the database (one-time per PC)

```powershell
$env:PGPASSWORD = "admin"
& "C:\Program Files\PostgreSQL\17\bin\psql.exe" -U postgres -c "CREATE DATABASE openmu;"
```

You should see `CREATE DATABASE`. "Already exists" is also fine.

The server creates schemas, tables, and the per-context login roles (`config`, `account`, `friend`, `guild`) automatically on first launch.

---

## 6. First-time build

```powershell
dotnet build src\MUnique.OpenMU.sln -c Release
```

Takes 2–4 minutes the first time (NuGet downloads). Wait for `Build succeeded`. Ignore the ~1500 style/StyleCop warnings — harmless.

**Why this step is required on a fresh clone:** the Persistence projects use a Roslyn source generator that gets compiled into `bin\Release` only by a Release build. If you skip this, `dev-run.ps1` will fail with `MUnique.OpenMU.Persistence.SourceGenerator.exe not found` or an `MSB3073` error.

---

## 7. Start the server

```powershell
./scripts/windows/dev-run.ps1
```

Wait ~12 seconds. Success looks like:

```
Host started, elapsed time: 00:00:11.x
Admin Panel bound to urls: "http://[::]:80"
Starting Server Listener, port 55901
```

**Leave that terminal open — it's your live log.**

Variations:

```powershell
./scripts/windows/dev-run.ps1 -Lan       # bind to LAN IP for friends on Wi-Fi
./scripts/windows/dev-run.ps1 -Reinit    # WIPE + reseed DB (always backup first)
./scripts/windows/dev-run.ps1 -Demo      # in-memory DB, nothing persists
```

---

## 8. Verify it's actually running

In a browser: <http://localhost/>

Check:

- **Servers** page → 3 game servers + 1 chat server + 2 connect servers, all green
- **Accounts** page → ~20 seeded test accounts (`test0`–`test9`, `testgm`, `testgm2`, etc.)

If **Accounts** is empty, the DB schema didn't seed. Stop the server (`Ctrl+C`) and run once:

```powershell
./scripts/windows/dev-run.ps1 -Reinit
```

Subsequent starts use plain `dev-run.ps1`.

---

## 9. Stop the server

In that same VS Code terminal: **Ctrl+C**. Wait for graceful shutdown.

If the terminal got closed:

```powershell
Get-Process -Name MUnique.OpenMU.Startup -ErrorAction SilentlyContinue | Stop-Process -Force
```

Check if it's running:

```powershell
Get-Process -Name MUnique.OpenMU.Startup -ErrorAction SilentlyContinue | Select-Object Id, StartTime
Get-NetTCPConnection -LocalPort 80 -State Listen -ErrorAction SilentlyContinue
```

If both return rows, the server is up. If both are empty, it's down.

---

## 10. Daily workflow from now on

```powershell
git pull                                  # grab any updates
./scripts/windows/dev-run.ps1             # start
# ... work / play ...
# Ctrl+C in that terminal to stop
```

You only need to rebuild manually if something on the **Rebuild triggers** list below changes.

---

## 11. When to build vs. when to "repack"

### When you do **NOT** need to manually rebuild

`dev-run.ps1` runs `dotnet run -c Release`, and `dotnet run` automatically:

- Detects changed C# files and recompiles them
- Restores any new NuGet packages
- Then launches the server

So just `./scripts/windows/dev-run.ps1` is enough for:

- Editing C# code and restarting
- Editing config (`appsettings.json`)
- Pulling commits that touch C# / config / NuGet

It picks up changes on the next start. No extra step needed.

### When you **DO** need to manually `dotnet build -c Release`

| Trigger | Why |
|---|---|
| Fresh `git clone` on a new PC | The source generator EXE doesn't exist yet → `dev-run` fails. |
| You deleted `bin/` or `obj/` folders | Source generator binaries are gone. |
| You did `git clean -fdx` | Same. |
| You switched between Debug and Release in some manual run | Missing the other config's generator binaries. (Stick with Release via `dev-run.ps1` and this won't happen.) |
| You edited project references (`.csproj` files) | Incremental build sometimes misses these. |
| You pulled commits that added a brand-new `.csproj` to the solution | Same. |

Command:

```powershell
dotnet build src\MUnique.OpenMU.sln -c Release
```

If that still fails weirdly, nuke and redo:

```powershell
dotnet clean src\MUnique.OpenMU.sln
dotnet build src\MUnique.OpenMU.sln -c Release
```

### "Repack" — publishing for deployment

There's no pack/publish script in this repo yet. You'd only need it when you want to:

- Hand someone a standalone `.exe` server they can run **without** the .NET SDK installed
- Deploy to a VPS (future Phase D in `PLAN.md`)

When that day comes, the command is:

```powershell
dotnet publish src\Startup\MUnique.OpenMU.Startup.csproj -c Release -r win-x64 --self-contained -o publish\server
```

But for local dev on your two PCs, **you don't need this**. Just `git pull` → `./scripts/windows/dev-run.ps1`.

---

## 12. Backup & restore (around risky operations)

Always snapshot before `-Reinit` or any direct DB edit:

```powershell
./scripts/windows/db-backup.ps1          # snapshot to local/db-backups/ (gitignored)
./scripts/windows/db-restore.ps1         # restore newest snapshot (server must be stopped first)
```

---

## 13. Connecting the game client

You need an **MU Online Season 6 EP3 ENG** client (not in this repo).

1. Edit the client's connect-server IP to `127.127.127.127` (any `127.x.x.x` **except** `127.0.0.1` — the client blocks `127.0.0.1`).
2. For **windowed mode**, run once in PowerShell:

   ```powershell
   New-ItemProperty -Path "HKCU:\SOFTWARE\WebZen\Mu\Config" -Name WindowMode -Value 1 -PropertyType DWord -Force
   ```

3. Log in with a seeded account:

   | Login | Password | Notes |
   |---|---|---|
   | `test0` … `test9` | same as login | Levels 1, 10, 20 … 90 |
   | `testgm` | `testgm` | All 5 classes, all GameMaster |
   | `testgm2` | `testgm2` | Rage Fighter + Summoner, GM |

4. Character names must be **4+ characters** (client UI rejects 3 even though server allows).
5. Account creation has **no in-game / launcher path** — create via admin panel → Accounts → "Create new".

---

## 14. Troubleshooting cheat sheet

| Symptom | Fix |
|---|---|
| `dotnet` not found in VS Code terminal | Close VS Code completely, reopen. PATH was added after VS Code started. |
| `MUnique.OpenMU.Persistence.SourceGenerator.exe not found` or `MSB3073` | Run `dotnet build src\MUnique.OpenMU.sln -c Release` once. |
| `Cannot find path ...\OpenMU\OpenMU` | Don't `cd OpenMU` again — you're already there. |
| `Get-Service postgresql-x64-17` says Stopped | `Start-Service postgresql-x64-17` |
| Port 80 already in use | IIS or Skype legacy is grabbing it. Stop them, or change admin port in admin panel → Configuration → System. |
| Admin panel Accounts list empty | Stop, then `./scripts/windows/dev-run.ps1 -Reinit` once. |
| PowerShell errors on `&&` | Use `;` instead, or run commands on separate lines. |
| Execution policy blocks `dev-run.ps1` | `Set-ExecutionPolicy -Scope CurrentUser RemoteSigned` |

---

## 15. TL;DR

| What | Command |
|---|---|
| Fresh PC prereqs | `winget install` block in §1 |
| Pull latest | `git pull` |
| Create DB (once) | `psql.exe -U postgres -c "CREATE DATABASE openmu;"` |
| First-time build | `dotnet build src\MUnique.OpenMU.sln -c Release` |
| Start | `./scripts/windows/dev-run.ps1` |
| Stop | `Ctrl+C` in the server terminal |
| Reseed DB | `./scripts/windows/dev-run.ps1 -Reinit` |
| Backup DB | `./scripts/windows/db-backup.ps1` |
