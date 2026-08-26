---
title: Setup
sidebar_position: 2
description: Create, reinstall and update the OpenMU database from the admin panel.
---

# Setup

**Navigation:** *Setup* — route `/setup`

The setup page is where the database is created, reinstalled and updated. It is
the first page you need on a fresh installation, and it is the only page shown in
the navigation menu when no usable configuration exists yet.

## Database status

The page starts by telling you the state of the database:

| Status | Meaning | What to do |
|---|---|---|
| *Can't connect to the database, probably not created yet* | The server can reach the database server, but not the OpenMU database | Click **Create** |
| *Not created* | The database exists but has no schema yet | Click **Create** |
| *Update required* | The schema is older than the running OpenMU version | Click **Update** |
| *Up to date* | Everything is fine | Nothing |

When the database is up to date, the page also shows the **initialized game
version**, or a warning that no initialized data was found.

:::note[Connection settings]
The database connection itself is not configured here — it comes from
`ConnectionSettings.xml` and, in the docker deployments, from the
`DB_HOST`/`DB_ADMIN_USER`/`DB_ADMIN_PW`
[environment variables](../deployment/startup-parameters.md#environment-variables).
:::

## Installing

Clicking **Create** (or **Reinstall**) opens the installation dialog with three
choices:

### 1. Game version

Every supported version has its own data initialization, which creates the
matching items, monsters, maps, skills and servers. The list contains the
versions which are available in your OpenMU build — Season 6 Episode 3 is the
main one, plus older versions such as 0.75 and 0.95d.

### 2. Number of game servers

A slider from 1 to 10. This only creates the **configuration** of that many game
servers; whether they are actually started is decided on the
[Servers page](servers.md). Each game server gets its own network port, starting
at 55901.

### 3. Test accounts

A checkbox which creates a set of [test accounts](../getting-started/test-accounts.md)
with well-known passwords.

:::danger[Not for public servers]
Do not create test accounts on a server that players can reach — some of them
have game master rights and their password is identical to their user name.
:::

Then click **Install** and wait. When the installation is finished, the page says
so.

:::warning[All players are disconnected]
The installation button is disabled while players are connected — the page shows
*"First close all connections to the server"*. An installation **drops the
existing data**, so every account, character and configuration change is lost.
:::

:::note[Distributed deployment]
In a [distributed deployment](../deployment/distributed.md) you have to restart
the connect server and game server containers after the installation has
finished. The panel reminds you of that.
:::

## Reinstalling

When a database is already installed, the button is labelled **Reinstall**. It
runs the same dialog and lets you switch to another game version, another number
of game servers, or add/remove test accounts.

This is the admin panel equivalent of the `-reinit` start parameter — and it has
the same consequence: **the existing data is gone.** Take a backup of your
PostgreSQL database first if you care about it.

## Updating

If the running OpenMU version needs a newer database schema, the page offers
**Update**. This migrates the schema; it does not touch your accounts.

Schema updates are separate from **configuration** updates — new drop tables,
fixed monster stats or new plugin configurations are applied on the
[Configuration updates](configuration-updates.md) page.

In the all-in-one deployment, the schema update can also happen automatically at
startup if *Auto update schema* is enabled in the
[System configuration](game-configuration.md#system). In a distributed
deployment, the update always has to be started here manually.
