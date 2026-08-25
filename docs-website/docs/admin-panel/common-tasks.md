---
title: Common tasks
sidebar_position: 15
description: Short how-tos for the things server operators do most often.
---

# Common tasks

Short recipes which combine several pages of the panel. Each step links to the
page with the details.

## Change the experience rate

1. Go to **Configuration → General** ([Game configuration](game-configuration.md#general)).
2. Change **Experience rate** (and **Master experience rate** if you want).
3. Save.
4. Click **Reload configuration and restart all game servers** on the
   [Servers page](servers.md).

The rate can also be set **per game server**, which is how you run one server
with different rates: open the game server on the
[Servers page](servers.md#editing-a-server) and change its experience rate there.

## Add a game server

1. [Servers page](servers.md#add-a-game-server) → **+ Game server**.
2. Fill in server id, description, experience rate, PvP, server configuration,
   client and network port.
3. Save, then start it in the server list.
4. Open the new port in your firewall/router and, in a docker deployment, publish
   it in the compose file. See [Ports](../reference/ports.md).

## Ban a player

1. If the player is online: disconnect them on the
   [Online accounts](online-accounts.md) page — or use **Temporarily ban** on the
   [live map](live-map.md), which does both at once.
2. Open the account on the [Accounts page](accounts.md) with **Edit**.
3. Set the **state** to `Banned` or `TemporarilyBanned` and save.

## Make somebody a game master

1. [Accounts page](accounts.md) → **Edit** the account.
2. Set the **state** to `GameMaster` (or `GameMasterInvisible`).
3. Save. The account can now use the game master
   [chat commands](chat-commands.md).

## Add a monster spawn

1. Go to **Configuration → Map editor** ([Map editor](map-editor.md)).
2. Select the map.
3. Click **+ Monster spawn area** and draw the area.
4. Set the monster, the quantity and the details of the focused object.
5. **Save**, then reload the configuration on the
   [Servers page](servers.md).

## Change a drop

1. Go to **Configuration → Drop item groups**
   ([Game configuration](game-configuration.md#drop-item-groups)).
2. Edit the group — or duplicate an existing one and adapt the copy.
3. Assign the group where it should apply: globally, on a map, or on a monster
   ([Monsters](game-configuration.md#monsters)).
4. Save and reload the configuration.

## Switch to another game version

1. Go to the [Setup page](setup.md).
2. Click **Reinstall**, pick the game version, the number of game servers and
   whether test accounts should be created.
3. Click **Install** and wait.

:::danger[This deletes your data]
Reinstalling drops the existing database contents — accounts, characters and all
configuration changes. Back up your PostgreSQL database first.
:::

## Update OpenMU to a newer version

1. Pull the new docker image (or build the new source) and restart the server.
2. Open the [Setup page](setup.md) — if it says *Update required*, run the schema
   update.
3. Open the [Configuration updates](configuration-updates.md) page and apply the
   available updates.
4. Restart the server process so the updates take effect.

## Fix "players get disconnected after selecting a server"

This is almost always the IP resolver reporting an address the client cannot
reach.

1. Go to **Configuration → System**
   ([System configuration](game-configuration.md#system)).
2. Set the **IP resolver**:
   * server and client on the same machine → `Loopback`
   * server in your LAN → `Local`
   * public server → `Public`, or `Custom` with your domain/IP
3. Save and restart the server process.

See [Connect a game client](../getting-started/game-client.md#disconnects-after-selecting-a-server).

## Turn a feature off

* A **chat command** → deactivate it on the
  [Chat commands](chat-commands.md) page.
* A **game mechanic implemented as a plugin** → deactivate it on the
  [Plugins](plugins.md) page.

## Reset the database

1. [Setup page](setup.md) → **Reinstall**.
2. Alternatively, start the server with the `-reinit`
   [start parameter](../deployment/startup-parameters.md).

For a throwaway test server, `-demo` starts the server with in-memory data which
is recreated at each start and never persisted.
