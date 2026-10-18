---
title: Run from source
sidebar_position: 3
description: Build, run and debug OpenMU from the source code.
---

# Run from source

Use this way if you want to develop or debug OpenMU. It works on Windows,
Linux and macOS. Where the steps differ per operating system, both variants
are shown.

## Requirements

* Windows 10 or higher, a current Linux distribution, or macOS
* [PostgreSQL](https://www.postgresql.org/download/) installed, or Docker to
  run it in a container (see step 3)
* An IDE: Visual Studio 2026 on Windows (with the workloads for *ASP.NET Web
  development* and *.NET Desktop development*), Rider, or VS Code with the C#
  extension. Plain command line with the SDK below works as well. Please keep
  your tools up to date to prevent issues.
* [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0) (already
  included in Visual Studio 2026)
  ```powershell
  winget install Microsoft.DotNet.SDK.10
  ```
  On Linux and macOS, install it through your package manager or the
  installer from the download page linked above.
* [NodeJS 16+](https://nodejs.org)
  ```powershell
  winget install OpenJS.NodeJS.LTS
  ```
  On Linux, install it through your package manager (for example
  `sudo apt install nodejs`) or from the download page linked above.
* This repository cloned

## Steps

1. Open the OpenMU solution in your IDE.
2. Restore the NuGet packages: Visual Studio offers *Restore NuGet Packages* on
   right click of the solution, or run this from the repository root:

   ```bash
   dotnet restore
   ```

3. Configure the postgres admin credentials (pick one option):
   * **Recommended: environment variables.** Set `DB_ADMIN_USER` and
     `DB_ADMIN_PW` to the user/password of your postgres superuser account
     (leave `DB_HOST` unset to use `localhost`). Only the admin credentials
     need to be correct. The server creates the database schemas and the
     other roles (`config`, `account`, `friend`, `guild`) itself.

     ```bash
     export DB_ADMIN_USER=postgres
     export DB_ADMIN_PW='s3cret'
     ```

     On Windows (PowerShell) instead:

     ```powershell
     $env:DB_ADMIN_USER='postgres'
     $env:DB_ADMIN_PW='s3cret'
     ```

     When the variables are not set, the defaults (`postgres` / `admin`)
     apply. Avoid `;` in the password, since it separates values in connection
     strings.
   * **Alternative:** edit
     `src/Persistence/EntityFramework/ConnectionSettings.xml` so that the
     connection strings are correct. Only the user/password of the
     **first and second** connection string need to be correct. The server
     will try to create the other roles specified by the settings.

   Whichever option you picked, if you run postgres in Docker instead of
   installing it, start it with a matching user, password and database
   name (`openmu`):

    ```bash
    docker run -d --name openmu-db \
      -e POSTGRES_USER=postgres -e POSTGRES_DB=openmu \
      -e POSTGRES_PASSWORD='s3cret' \
      -p 5432:5432 postgres
    ```

4. Build the solution, in your IDE or from the repository root with:

   ```bash
   dotnet build src/MUnique.OpenMU.sln
   ```

5. Start `MUnique.OpenMU.Startup`. The environment variables from step 3 must
   be set wherever you start it (same terminal, or your Debug launch profile).
   From a terminal:

   ```bash
   dotnet run --project src/Startup/MUnique.OpenMU.Startup.csproj -- -autostart
   ```

   (The `--` passes `-autostart` to the server instead of to `dotnet`.)
   * If required, it creates the database schemas and the required roles, and
     gives permissions to those roles.
   * Optional: you can reinitialize the database by adding the `-reinit`
     parameter.
6. When the admin panel is initialized, go to [http://localhost/](http://localhost/). You should see
   three game servers, the chat server and two connect servers. Start the connect
   servers and at least one game server — see [Servers](../admin-panel/servers.md).
7. Connect to the server with the game client, see [Game client](game-client.md).

:::tip[Updating to a newer master]
If you update to a newer state of the master branch, it is possible that the
database and the configuration have to be updated. You find those updates in the
admin panel, see [Configuration updates](../admin-panel/configuration-updates.md)
and [Setup](../admin-panel/setup.md).
:::

## Helpful optional steps

### Auto start

If you don't want to start each server listener manually after starting the
process, you can either

* activate *Auto Start* in the admin panel at **Configuration → System**, or
* use the start parameter `-autostart`.

### IP resolving

If you encounter disconnects after selecting a server, it is most likely a wrong
setting for the IP resolver. You can change it in the admin panel at
**Configuration → System**.

You may also change the setting by start parameters or environment variables,
but this is only recommended for experienced users — see
[Startup parameters](../deployment/startup-parameters.md).

### Changing the game version

If you want to play a version other than Season 6, you can initialize the
database with another game version on the
[Setup page](../admin-panel/setup.md) of the admin panel.

### In-memory mode

With the `-demo` parameter, the server uses in-memory repositories instead of an
external database and initializes the data at each start. This is handy for a
quick test, but player progress is **not saved**.

## Building from the command line

```bash
dotnet publish src/Startup/MUnique.OpenMU.Startup.csproj --configuration Release
```

The tests are run with:

```bash
dotnet test
```
