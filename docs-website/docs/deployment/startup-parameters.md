---
title: Startup parameters and environment variables
sidebar_label: Startup parameters
sidebar_position: 5
description: Command line parameters, environment variables and their priority.
---

# Startup parameters and environment variables

:::tip[Most of this is not needed]
Except for `-demo` and `-adminpanel`, most of these settings can be changed more
conveniently in the admin panel at **Configuration → System** — see
[System configuration](../admin-panel/game-configuration.md#system). Start
parameters and environment variables are meant for special cases and experienced
users.
:::

## Start parameters

| Parameter | Description |
|---|---|
| `-autostart` | Automatically initializes the game servers and starts the TCP listeners of all (sub-)servers |
| `-reinit` | Recreates and reinitializes the database. Has no effect when `-demo` is used. |
| `-version:[season6\|0.75\|0.95d]` | Defines the version of the game client. Only has an effect with `-reinit` or `-demo` and affects the initial data creation. Default: `season6` |
| `-demo` | Instead of an external database, in-memory repositories are used and the data is initialized at each start. Only for testing, not for production — player progress is **not saved**. |
| `-deamon` | Deactivates handling of console inputs |
| `-adminpanel:[enabled\|disabled]` | Defines whether the admin panel is available. If disabled, `-autostart` is applied automatically. Default: `enabled` |

### `-resolveIP`

Defines how the server's own IP address is determined, which is reported back to
the game client when it requests to connect to a selected game server (server
selection screen) or to the chat server (when starting a chat with the in-game
messenger).

This is helpful if the server runs in an environment where the public IP is not
reachable from the outside (e.g. because you share an IPv4 address or are behind
a firewall) and you want to use it within your computer or private network.

| Value | Description | Example |
|---|---|---|
| `public` | Default value if nothing is specified. The public IP is determined automatically by an [external API](https://www.ipify.org/). | `-resolveIP:public` |
| `local` | Determines a local IP. If none is found, a loopback IP is used (`127.127.127.127`). | `-resolveIP:local` |
| `loopback` | For testing on the same machine, a loopback IP is used (`127.127.127.127`). | `-resolveIP:loopback` |
| *an IPv4 address* | A custom and constant IP address or a host name. | `-resolveIP:140.82.118.4` |

## Environment variables

These may be helpful when running the server in a container or under Linux.

| Variable | Description |
|---|---|
| `RESOLVE_IP` | See the `-resolveIP` parameter — same values apply. Only considered when there is no `-resolveIP` parameter. |
| `ASPNETCORE_ENVIRONMENT` | If neither a `-resolveIP` parameter nor a `RESOLVE_IP` variable is defined, this variable is considered to find the optimal IP resolver. If the value is `Development`, `loopback` is used, otherwise `public`. |
| `ASPNETCORE_URLS` | Defines the address of the admin panel, e.g. `http://+:80` |
| `DB_HOST` | Host name/address of the postgres database |
| `DB_ADMIN_USER` | User name of the admin user of the postgres database |
| `DB_ADMIN_PW` | Password of the admin user of the postgres database |
| `Database__AssumeExternallyProvisioned` | When `true`, an already-provisioned (empty) database is kept and only its schema is built via migrations, instead of dropping and recreating it. Default: `false`. See below. |

## Externally provisioned database

By default, when no database exists yet, the server drops and (re-)creates it
before building the schema. This requires the connecting database role to be
allowed to create and drop databases — upstream the connection strings use the
`postgres` superuser, optionally overridden via `DB_ADMIN_USER`/`DB_ADMIN_PW`.

In managed environments — a Kubernetes operator, infrastructure-as-code, or a
managed cloud database — the database is often provisioned ahead of time and the
connecting role is intentionally *not* permitted to create or drop databases
(least privilege; it only owns its own database). In that case, set this in
`appsettings.json`:

```json
{
  "Database": {
    "AssumeExternallyProvisioned": true
  }
}
```

…or, equivalently, the environment variable
`Database__AssumeExternallyProvisioned=true`.

The server then keeps the existing (empty) database and only builds its schema
via migrations, instead of dropping and recreating it. An explicit `-reinit`
always drops and recreates, regardless of this setting.

## Logging

Logging can be configured in the `appsettings.json` file. By default, not a lot is
configured. To extend the configuration, have a look at the
[Serilog documentation](https://github.com/serilog/serilog-settings-configuration).
The server makes good use of scopes, so you can configure it to log only the
actions of certain players, for example.

## Settings priority

The same option can be set in different ways, so there is a clear priority:

1. Start parameters
2. Environment variables
3. Settings in the admin panel (**Configuration → System**)

The idea is that start parameters and environment variables should only be used
in special cases by experienced users.
