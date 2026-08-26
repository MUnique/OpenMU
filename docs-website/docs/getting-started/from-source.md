---
title: Run from source
sidebar_position: 3
description: Build, run and debug OpenMU from the source code.
---

# Run from source

Use this way if you want to develop or debug OpenMU. This guide describes it for
Windows; it runs under Linux and macOS as well.

## Requirements

* Windows 10 or higher (Linux/macOS work too, this guide just isn't written for them)
* [PostgreSQL](https://www.postgresql.org/download/) installed
* Visual Studio 2026, with the workloads for *ASP.NET Web development* and
  *.NET Desktop development*. Please keep it up-to-date to prevent issues.
* The Visual Studio extension
  [Web Compiler 2022+](https://marketplace.visualstudio.com/items?itemName=Failwyn.WebCompiler64),
  if you plan to edit SCSS files of the admin panel
* [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0) — it should
  already be included in Visual Studio 2026
  ```powershell
  winget install Microsoft.DotNet.SDK.10
  ```
* [NodeJS 16+](https://nodejs.org)
  ```powershell
  winget install OpenJS.NodeJS.LTS
  ```
* This repository cloned

## Steps

1. Open the OpenMU solution with Visual Studio.
2. Right click the solution and select *Restore NuGet Packages*.
3. Edit `src/Persistence/EntityFramework/ConnectionSettings.xml` so that the
   connection strings are correct. Only the user/password of the **first and
   second** connection string need to be correct — the server will try to create
   the other roles specified by the settings.
4. Build the solution.
5. Start `MUnique.OpenMU.Startup`.
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
