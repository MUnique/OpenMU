---
title: Requirements
sidebar_position: 1
description: What you need before you start an OpenMU server.
---

# Requirements

## Free TCP ports

The server needs these ports. They are the defaults of a freshly initialized
database and can all be changed later in the [admin panel](../admin-panel/servers.md).

| Port | Used by |
|---|---|
| 80 | Admin panel |
| 44405 | Connect server — default connection port for the original client |
| 44406 | Connect server — port for the [open source client](https://github.com/sven-n/MuMain) |
| 55901 – 55906 | Game servers |
| 55980 | Chat server |

See [Ports](../reference/ports.md) for the complete list, including the ports of
the distributed deployment.

:::note[Two connect servers]
The database is initialized for two different clients by default. They connect to
the same game servers through different ports. If you connect to the "wrong"
port, it may currently still work, you will just get warnings in the logs.
However, as soon as encryption keys or methods are changed, this will no longer
be the case.
:::

## A game client

OpenMU is a server — it does not ship a game client. Check the FAQs of
[our Discord](https://discord.gg/2u5Agkd) for where to get a client.

You also need a way to start the client so that it connects to your server. Our
launcher does that for you:

* [MUnique.OpenMU.ClientLauncher v0.9.6.zip](https://github.com/MUnique/OpenMU/releases/download/v0.9.0/MUnique.OpenMU.ClientLauncher_0.9.6.zip)
* It requires the [.NET 10 runtime](https://dotnet.microsoft.com/download/dotnet/10.0)
  or higher.

:::tip[Local testing]
If server and client run on the same machine, use any IP of `127.x.x.x` **except**
`127.0.0.1` — that one is blocked by the client. `127.127.127.127` is a good
choice, and it is also what the `loopback` IP resolver reports.
:::

## Choosing how to run the server

| | [Docker](docker.md) | [From source](from-source.md) |
|---|---|---|
| Want to play around | ✅ recommended | |
| Want to host for others | ✅ recommended, see [Deployment](../deployment/overview.md) | |
| Want to develop or debug OpenMU | | ✅ recommended |
| Needs a .NET SDK | no | yes |
| Needs a PostgreSQL installation | no (container) | yes |
