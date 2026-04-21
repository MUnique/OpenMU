# Windows dev helpers

PowerShell 7+ scripts for the most common developer actions on Windows.
All scripts are safe to run repeatedly (idempotent where possible).

Run them from the repo root:

```powershell
# Check that all prerequisites are installed
./scripts/windows/bootstrap.ps1

# Boot the all-in-one Docker stack (Postgres + OpenMU + nginx)
./scripts/windows/docker-up.ps1
./scripts/windows/docker-down.ps1

# Run the server from source (manual dev build)
./scripts/windows/dev-run.ps1                 # -autostart -resolveIP:loopback
./scripts/windows/dev-run.ps1 -Lan            # -autostart -resolveIP:local
./scripts/windows/dev-run.ps1 -Demo           # in-memory, no DB
./scripts/windows/dev-reset-db.ps1            # -autostart -resolveIP:loopback -reinit

# Windows Firewall rules (requires admin PowerShell)
./scripts/windows/firewall-open.ps1
./scripts/windows/firewall-close.ps1
```

If PowerShell blocks script execution with an "execution policy" error, run
this once (as your user, not as admin):

```powershell
Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned
```

See `PLAN.md` §4–§7 for the full Windows setup story.
