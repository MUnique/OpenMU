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
| 55901 | Game server 1 | |
| 55902 | Game server 2 | |
| 55903 | Game server 3 | |
| 55904 – 55906 | Game servers 4 – 6 | Only when the database was initialized with more game servers |
| 55980 | Chat server | Used by the in-game messenger |

The game servers are not contacted directly by the client at first: the client
connects to a connect server, picks a server in the server selection screen, and
is then redirected to the game server address which the
[IP resolver](../getting-started/game-client.md#disconnects-after-selecting-a-server)
reports.

## Web

| Port | Purpose |
|---|---|
| 80 | Admin panel (and the reverse proxy in the docker deployments) |
| 443 | HTTPS, when configured — see [All-in-one deployment](../deployment/all-in-one.md) |

In the [distributed deployment](../deployment/distributed.md), the reverse proxy
also serves Grafana, Prometheus and Zipkin under sub-paths of the same port.
