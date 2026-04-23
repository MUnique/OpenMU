# OpenMU Setup Journal & FAQ

Running log of setting up a MU Online private server from the [MUnique/OpenMU](https://github.com/MUnique/OpenMU) repository on Windows. Captures what was done, bugs encountered, and the exact fix for each.

**Machine:** Windows, PowerShell
**Repo location:** `C:\Users\Lvl 100 Mafia Boss\Downloads\Telegram Desktop\Rotating Earth\Rotating Earth\OpenMU\`
**Path chosen:** Manual build with .NET SDK (Docker installation was not possible).
**Phase approach:**
- **Phase A** = in-memory demo run (no database) — smoke test that the server + admin panel come up.
- **Phase B** = real run with PostgreSQL (persistent accounts/characters) — *not done yet*.

---

## Progress log

| # | Status | Step | Evidence |
|---|---|---|---|
| 1 | done | Read top-level docs: `OpenMU/README.md`, `OpenMU/QuickStart.md`, `OpenMU/CONTRIBUTING.md`, `OpenMU/docs/Readme.md`, `OpenMU/docs/Progress.md` | — |
| 2 | done | Read + committed to follow `OpenMU/.cursorrules` | — |
| 3 | done | Decided on manual-build path (Docker not installable on this machine) | — |
| 4 | done | Verified no .NET SDK was present (`dotnet --list-sdks` returned "No .NET SDKs were found") | — |
| 5 | done | Installed **.NET SDK 10.0.203** via `winget install Microsoft.DotNet.SDK.10` | `dotnet --list-sdks` now reports `10.0.203 [C:\Program Files\dotnet\sdk]` |
| 6 | done | Confirmed all 10 required TCP ports free: `80, 44405, 44406, 55901–55906, 55980` | — |
| 7 | done | Built the solution in **Release** configuration | 0 errors, 1000 warnings (all package-vuln / XML-doc warnings, non-blocking). Build time ~58s on warm cache |
| 8 | done | Launched server in demo mode | All 10 sockets listening under PID of `MUnique.OpenMU.Startup`, admin panel returns `HTTP 200 OK` at `http://localhost/` |
| 9 | pending | Load admin panel in browser, verify servers listed | — |
| 10 | pending | Connect real MU Online client to `127.127.127.127:44405`, log in with `test0`/`test0` | — |
| 11 | pending | **Phase B:** install PostgreSQL, edit `ConnectionSettings.xml`, run without `-demo` | — |

---

## Bugs encountered & fixes

### Bug 1 — `dotnet build` fails on first run: `MUnique.OpenMU.Persistence.SourceGenerator.exe` not found

#### Symptom

```
Unhandled exception: An error occurred trying to start process
  'C:\...\src\Persistence\SourceGenerator\bin\Release\MUnique.OpenMU.Persistence.SourceGenerator.exe'
  with working directory 'C:\...\src\Persistence\EntityFramework'.
  The system cannot find the file specified.

error MSB3073: The command "dotnet run -p ../SourceGenerator/... -c:Release
  MUnique.OpenMU.Persistence.EntityFramework ... --no-build" exited with code 1.
```

Build output at end: `Build FAILED. 2 Error(s)`.

#### Root cause

Two projects have MSBuild `<Target Name="PreBuild">` steps that invoke the source-generator tool with `--no-build`:

- `src/Persistence/EntityFramework/MUnique.OpenMU.Persistence.EntityFramework.csproj` line **52**
- `src/Persistence/MUnique.OpenMU.Persistence.csproj` line **45**

`--no-build` tells `dotnet run` to assume the target executable already exists. But there is **no `ProjectReference` edge from these projects to the SourceGenerator project** — they call it via CLI. Because `dotnet build SLN` builds projects in parallel, MSBuild can start the PreBuild steps of the Persistence projects before the SourceGenerator has finished compiling, so the `.exe` doesn't exist yet.

#### Fix

Build the SourceGenerator project explicitly first, then build the full solution:

```powershell
cd "C:\Users\Lvl 100 Mafia Boss\Downloads\Telegram Desktop\Rotating Earth\Rotating Earth\OpenMU\src"

# 1. Build the source generator first
dotnet build Persistence\SourceGenerator\MUnique.OpenMU.Persistence.SourceGenerator.csproj -c Release

# 2. Now build the full solution
dotnet build MUnique.OpenMU.sln -c Release
```

Second build succeeded: `0 Error(s)`, ~58 s.

#### Alternatives (not used)

- Set `-p:ci=true` to skip the PreBuild target (but then auto-generated EF model files may be stale/missing and compile will fail elsewhere).
- Force serial build: `dotnet build MUnique.OpenMU.sln -m:1` (works but dramatically slower).

---

### Bug 2 — Server crashes on startup with `-resolveIP:loopback`

#### Symptom

```
[Fatal] Unhandled exception leading to terminating application:
  System.ArgumentException: When using a custom resolver type, a parameter with an IP
  or host name is required. (Parameter 'parameter')
    at MUnique.OpenMU.Network.ConfigurableIpResolver.ApplyConfiguration(
       IpResolverType resolverType, String parameter, Boolean raiseEvent)
       in ...\src\Network\ConfigurableIpResolver.cs:line 70
```

But `-resolveIP:loopback` is documented as a valid flag in `OpenMU/src/Startup/Readme.md` (lines 44–50) and the code at `src/Network/IpAddressResolverFactory.cs` lines 54–60 handles it as a built-in (no parameter needed).

#### Root cause

**PowerShell's native-command argument parser mangles `-flag:value` patterns.** When you type:

```powershell
dotnet run --project Startup -- -demo -autostart -resolveIP:loopback
```

PowerShell can split `-resolveIP:loopback` and drop the part after the colon, so the program actually receives the bare string `"-resolveIP:"`. The factory's switch statement doesn't match any built-in (`-resolveIP:loopback`, `-resolveIP:local`, `-resolveIP:public`), falls into the default `Custom` case (`src/Network/IpAddressResolverFactory.cs` line 59), which then extracts the substring after the colon — an **empty string**. `ConfigurableIpResolver` sees `IpResolverType.Custom` with a null/empty parameter and throws at `ConfigurableIpResolver.cs` line 68–70.

#### Fix

Use the `RESOLVE_IP` environment variable instead. The factory reads it at `src/Network/IpAddressResolverFactory.cs` lines 63–78 and it takes bare values (`loopback`, `local`, `public`) — no colon parsing involved.

```powershell
$env:RESOLVE_IP = "loopback"
dotnet run --project Startup -c Release --no-build -- -demo -autostart -deamon
```

The server now binds correctly and registers each game server with the ConnectServer using endpoint `127.127.127.127:55901–55906`, confirmed in the logs.

#### Alternatives

- **Quote the argument:** `dotnet run ... -- "-resolveIP:loopback"` — should also work in PowerShell but not tested this session.
- **Put the arg after `--%`:** PowerShell's stop-parsing symbol — also avoids the native parser entirely.
- **Configure via admin panel** (`Configuration → System`) instead of CLI: the setting persists (in-memory in demo mode, in DB otherwise).

---

### Gotcha: `-deamon` (typo) is the flag to skip stdin handling

`src/Startup/Readme.md` line 32 documents the flag as **`-deamon`** — note the typo (should be `daemon`). When running detached / in the background on Windows, pass `-deamon` or the process will sit waiting for keyboard input.

---

## Working commands (known good)

### Build from scratch (one-time fix for Bug 1)

```powershell
cd "C:\Users\Lvl 100 Mafia Boss\Downloads\Telegram Desktop\Rotating Earth\Rotating Earth\OpenMU\src"
dotnet build Persistence\SourceGenerator\MUnique.OpenMU.Persistence.SourceGenerator.csproj -c Release
dotnet build MUnique.OpenMU.sln -c Release
```

### Run in demo mode (no PostgreSQL)

```powershell
cd "C:\Users\Lvl 100 Mafia Boss\Downloads\Telegram Desktop\Rotating Earth\Rotating Earth\OpenMU\src"
$env:RESOLVE_IP = "loopback"
dotnet run --project Startup -c Release --no-build -- -demo -autostart -deamon
```

Flag reference (from `src/Startup/Readme.md`):

| Flag | Effect |
|---|---|
| `-demo` | In-memory database, no PostgreSQL. **Progress is not persisted.** |
| `-autostart` | Start all TCP listeners automatically (no manual click in admin panel) |
| `-deamon` | Don't read stdin (needed when running detached) |
| `-reinit` | Wipe and recreate the database (ignored with `-demo`) |
| `-version:season6` / `-version:0.75` / `-version:0.95d` | Only with `-reinit` or `-demo`, picks the initial game data |
| `-adminpanel:disabled` | Hide the admin panel (implies `-autostart`) |

Env var alternative for `-resolveIP:` (avoids PowerShell colon bug):

| Value | Meaning |
|---|---|
| `RESOLVE_IP=loopback` | Reports `127.127.127.127` to clients (use for local-only play) |
| `RESOLVE_IP=local` | Auto-picks a LAN IP (use for play within your home network) |
| `RESOLVE_IP=public` | Queries ipify.org for your public IP (use for internet-facing hosting) |
| `RESOLVE_IP=203.0.113.42` | A literal IPv4 or host name |

### Verify the running server

```powershell
# All 10 listen sockets
$ports = 80,44405,44406,55901,55902,55903,55904,55905,55906,55980
foreach ($p in $ports) {
  $hit = Get-NetTCPConnection -LocalPort $p -State Listen -ErrorAction SilentlyContinue
  if ($hit) {
    $proc = Get-Process -Id $hit[0].OwningProcess -ErrorAction SilentlyContinue
    "LISTEN $p -> PID $($hit[0].OwningProcess) ($($proc.ProcessName))"
  } else {
    "NO     $p"
  }
}

# Admin panel HTTP
Invoke-WebRequest -Uri "http://localhost/" -UseBasicParsing -MaximumRedirection 0
```

### Tail the live log

Serilog writes a file sink per `src/Startup/appsettings.json` lines 23–33:

```powershell
Get-Content "C:\Users\Lvl 100 Mafia Boss\Downloads\Telegram Desktop\Rotating Earth\Rotating Earth\OpenMU\src\Startup\logs\log.txt" -Tail 50 -Wait
```

### Stop the server

```powershell
# Find PID
Get-Process -Name "MUnique.OpenMU.Startup"

# Kill it
Stop-Process -Name "MUnique.OpenMU.Startup"
```

---

## Important file/path references

| What | Where |
|---|---|
| Solution | `OpenMU/src/MUnique.OpenMU.sln` |
| Startup project (main entry) | `OpenMU/src/Startup/MUnique.OpenMU.Startup.csproj` |
| Startup docs (flags, env vars) | `OpenMU/src/Startup/Readme.md` |
| Logging config | `OpenMU/src/Startup/appsettings.json` |
| DB connection strings | `OpenMU/src/Persistence/EntityFramework/ConnectionSettings.xml` |
| EF PreBuild target (Bug 1) | `OpenMU/src/Persistence/EntityFramework/MUnique.OpenMU.Persistence.EntityFramework.csproj` line 52 |
| Persistence PreBuild target (Bug 1) | `OpenMU/src/Persistence/MUnique.OpenMU.Persistence.csproj` line 45 |
| IP resolver parser (Bug 2) | `OpenMU/src/Network/IpAddressResolverFactory.cs` |
| Error source (Bug 2) | `OpenMU/src/Network/ConfigurableIpResolver.cs` line 70 |
| Docker path (not used) | `OpenMU/deploy/all-in-one/docker-compose.yml` |

---

## Default credentials & baked-in test accounts

From `OpenMU/QuickStart.md` lines 131–145 and `OpenMU/deploy/all-in-one/README.md` line 81:

- **Admin panel** (only when running via Docker / nginx): user `admin` / password `openmu`. The manual-run path exposes the admin panel directly without basic-auth.
- **Game accounts** (password = username):
  - `test0`–`test9` — levels 1, 10, 20, …, 90
  - `test300` — level 300
  - `test400` — level 400 with master characters
  - `testgm` — game master
  - `testgm2` — game master with summoner + rage fighter
  - `testunlock` — no characters, all classes unlocked
  - `quest1`, `quest2`, `quest3` — quests at levels 150, 220, 400
  - `ancient` — level 330 with ancient sets
  - `socket` — level 380 with socket sets

---

## Open questions / things to investigate later

- [ ] Does the PowerShell argument-quoting workaround (`"-resolveIP:loopback"`) actually work? Not tested — env var was used instead.
- [ ] Do the source-generator pre-build targets produce code that gets committed, or is it always regenerated? (Affects whether a fresh clone on another machine needs the two-step build or a clean git pull is enough.)
- [ ] What does the `Web/ItemEditor` project expose? Spotted during build, not yet explored.
- [ ] Phase B: exact Postgres version tested with — docs just say "PostgreSQL". Try latest LTS.
- [ ] `OpenMU/deploy/distributed/` is marked **broken and unsupported** in `OpenMU/deploy/README.md` line 60 — skip unless we want to fix it.
