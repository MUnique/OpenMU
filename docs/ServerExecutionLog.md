# Server Execution Log (Private MU)

## 2026-04-21 - Step 0 Implementation

### Environment Preflight

- Docker: `28.2.2` - OK
- Docker Compose: `v2.37.1-desktop.1` - OK
- Dotnet CLI: **NOT FOUND** (`dotnet: command not found`)
- Required ports before startup (`80`, `55901-55906`, `44405-44406`, `55980`): no listener detected

### Docker All-in-one Boot

- Command: `docker compose up -d --no-build` at `deploy/all-in-one` - OK
- Containers up:
  - `database`
  - `openmu-startup`
  - `nginx-80`
- Startup logs confirmed:
  - database initialization finished
  - game servers initialized
  - listeners started (`55901-55906`, `44405`, `44406`)

### Admin Panel Reachability

- `http://localhost/` responds with `401 Unauthorized` (nginx basic auth active) - OK
- `http://localhost:8081/` responds with `200 OK` (Kestrel endpoint reachable) - OK

### Git Work Preparation

- Created branch: `feature/server-local-boot`
- Created branch: `feature/server-season6-baseline`

### Pending Manual Validation

- Connect via Season 6 client and verify:
  - Login pass
  - Character select pass
  - Enter map pass

### Mac-only Smoke Test (No Game Client)

- `curl -I http://localhost:8081/` => `200 OK`
- `nc -vz 127.0.0.1 44405` => success
- `nc -vz 127.0.0.1 44406` => success
- `nc -vz 127.0.0.1 55901` => success
- Recent logs checked (`docker compose logs --since=10m openmu-startup`):
  - no repeating severe exception
  - observed stop/start cycles caused by server reload actions in admin panel

### Blockers / Notes

- If running from source (non-Docker), install .NET SDK 10 and ensure `dotnet` is available in PATH.
- For local-only game client routing, use loopback resolver (`127.127.127.127`) instead of `127.0.0.1` when needed.

## 2026-04-21 - Mac Dev Tooling Installed

### Installed

- .NET SDK: `10.0.202` (local install to `~/.dotnet`, no sudo)
- PostgreSQL: `16.13` (`brew install postgresql@16`)
- Dart Sass CLI: `1.99.0` (`npm install -g sass`)

### Configured

- Updated shell profile `~/.zshrc`:
  - `DOTNET_ROOT=$HOME/.dotnet`
  - add `~/.dotnet` and `~/.dotnet/tools` to `PATH`
  - add `/opt/homebrew/opt/postgresql@16/bin` to `PATH`
- Started PostgreSQL service:
  - `brew services start postgresql@16`

### Verification

- `zsh -lic 'dotnet --version'` => `10.0.202`
- `zsh -lic 'psql --version'` => `16.13`
- `zsh -lic 'sass --version'` => `1.99.0`
- Build verification:
  - `dotnet build src/Startup/MUnique.OpenMU.Startup.csproj -c Debug` => success
