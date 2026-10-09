---
title: Ports
sidebar_position: 1
description: The TCP ports used by OpenMU.
---

# Ports

These are the ports of a freshly initialized database. Every listener port can be
changed in the [admin panel](../admin-panel/servers.md); the ports of the
monitoring tools are defined in the docker compose files of the
[deployment](../deployment/overview.md) you use.

## Game related

| Port | Server | Notes |
|---|---|---|
| 44405 | Connect server | Default connection port for the original client |
| 44406 | Connect server | Port for the [open source client](https://github.com/sven-n/MuMain) |
| 45901 | Game server 1 | |
| 45902 | Game server 2 | |
| 45903 | Game server 3 | |
| 45904 – 45906 | Game servers 4 – 6 | Only when the database was initialized with more game servers |
| 45980 | Chat server | Used by the in-game messenger |

The game servers are not contacted directly by the client at first: the client
connects to a connect server, picks a server in the server selection screen, and
is then redirected to the game server address which the
[IP resolver](../getting-started/game-client.md#disconnects-after-selecting-a-server)
reports.

### Why not 55901 and following?

Earlier versions of OpenMU used the ports 55901 and following for the game
servers, and 55980 for the chat server. These are within the *dynamic* port range
(49152 – 65535), from which operating systems like Windows pick the local ports
of outgoing connections. After the system has been running for a while, one of
these ports may already be taken by another process, and the server can't listen
on it. So when you choose your own ports, stay below 49152 as well.

### Upgrading an existing installation

Your database keeps the ports it was initialized with — updating the software
doesn't change them. The docker compose files and the Kubernetes chart, however,
publish the new ports. You have two options:

* **Move to the new ports:** apply the optional
  [configuration update](../admin-panel/configuration-updates.md) *Move server
  ports out of the dynamic port range* and restart the server. It changes every
  game and chat server port between 55900 and 55999 to the corresponding port
  between 45900 and 45999, e.g. 55901 to 45901, unless that port is already used
  by another server. Alternative published ports are kept. Then update your
  firewall and port forwardings.
* **Keep the old ports:** skip the update, and keep publishing the old ports in
  your docker compose file, or set `gameServer.listenerPort` and
  `centralServer.service.chatServerPort` of the Kubernetes chart to them.

## Web

| Port | Purpose |
|---|---|
| 80 | Admin panel (and the reverse proxy in the docker deployments) |
| 443 | HTTPS, when configured — see [All-in-one deployment](../deployment/all-in-one.md) |

In the [distributed deployment](../deployment/distributed.md), the reverse proxy
also serves Grafana under the sub-path `/grafana/` of the same port.
