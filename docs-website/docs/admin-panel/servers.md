---
title: Servers
sidebar_position: 4
description: Start and stop the sub-servers, add game and connect servers, send global messages.
---

# Servers

**Navigation:** *Servers* — route `/servers`

This page lists every sub-server of your OpenMU installation — connect servers,
game servers and the chat server — and is where you start and stop them.

## The server list

| Column | Meaning |
|---|---|
| (icon) | For game servers: a link which opens the [live map](live-map.md) of that server |
| Server name | The description of the server. Clicking it opens the server's configuration |
| Player count | Current connections / maximum connections (`∞` when unlimited) |
| Current state | `Stopped`, `Starting`, `Started`, `Stopping`, … |
| Action | Start, stop and remove buttons |

The footer of the table shows the **total** number of online players over all
game servers.

### Starting and stopping

* ▶ **Start** — starts the TCP listener of that server. Players can connect from
  this moment.
* ⏸ **Stop** — stops the listener and disconnects the players of this server.

A freshly installed server does not start its listeners automatically. On a
minimal setup, start **both connect servers and at least one game server**;
otherwise the client cannot even reach the server selection.

:::tip[Start the listeners automatically]
Enable *Auto Start* in the [System configuration](game-configuration.md#system),
or use the `-autostart`
[start parameter](../deployment/startup-parameters.md). In a distributed
deployment the listeners always start automatically.
:::

### Removing a server

Game servers and connect servers can be removed with the 🗑 button, which is
only enabled while the server is stopped. You are asked to confirm, and then both
the running instance **and its configuration** are deleted.

### Reload configuration and restart all game servers

The button at the bottom of the table reloads the game configuration from the
database and restarts all game servers with it. Use it after you changed
configuration data (rates, drops, monsters) and want it to take effect without
restarting the whole process.

:::warning[This disconnects the players]
Restarting the game servers disconnects everyone who is playing on them.
:::

## Editing a server

Clicking the name of a server opens its configuration:

* **Game server** → the generic edit page of its `GameServerDefinition`:
  experience rate, PvP, the game maps it hosts, its endpoints (port and expected
  client), the maximum number of players, and so on.
* **Connect server** → the connect server configuration: the client it serves,
  its port, timeouts, the maximum number of connections, and the limits which
  protect it against connection floods.
* **Chat server** → its endpoints and settings.

## Adding a server

### Add a game server

The **+ Game server** button opens a small form:

| Field | Meaning |
|---|---|
| Server id | The numeric id of the new server. It must be unique. |
| Description | The name shown in the panel |
| Experience rate | The experience multiplier of this server |
| PvP enabled | Whether players can attack each other on this server |
| Server configuration | Which `GameServerConfiguration` to use — this defines the maps the server hosts |
| Client | The `GameClientDefinition` which is expected to connect |
| Network port | The TCP port of the listener, e.g. 55907 |

After saving, the new server appears in the list, stopped, and can be started
right away.

### Add a connect server

The **+ Connect server** button asks for the server id, a description, the
expected client and the network port. A connect server serves the server list to
the game client and then redirects it to the chosen game server.

:::note[Open the port]
A new listener only helps if the port is reachable — open it in your firewall and
forward it in your router or docker compose file. See [Ports](../reference/ports.md).
:::

## Global messages

The card at the top of the page sends a message to the players — the golden
message in the centre of the screen.

1. Type the text.
2. Choose the target: *All game servers*, or one specific started game server.
3. Click **Send**.

Only started game servers can be targeted. If the target stopped in the meantime,
the panel keeps your text so you can pick another target and retry.

:::note[Not available in every deployment]
The global message card is only shown when the panel runs in the same process as
the game servers, i.e. in the all-in-one deployment.
:::
