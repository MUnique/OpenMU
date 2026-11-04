---
title: Connect a game client
sidebar_position: 4
description: How to point a MU Online client at your OpenMU server.
---

# Connect a game client

OpenMU does not ship a game client. Check the FAQs of
[our Discord](https://discord.gg/2u5Agkd) for where to get one.

## Using the client launcher

The client needs to be started in a way that makes it connect to your server. Our
launcher does that:

1. Download
   [MUnique.OpenMU.ClientLauncher v0.9.6.zip](https://github.com/MUnique/OpenMU/releases/download/v0.9.0/MUnique.OpenMU.ClientLauncher_0.9.6.zip).
2. Install the [.NET 10 runtime](https://dotnet.microsoft.com/download/dotnet/10.0)
   or higher.
3. Start the launcher, enter the host/IP and port of your connect server, and
   select your client's `main.exe`.

## Which port to use

A freshly initialized database contains **two** connect servers:

| Port | Client |
|---|---|
| 44405 | The original client |
| 44406 | The [open source client](https://github.com/sven-n/MuMain), which supports a slightly extended protocol |

Both connect servers lead to the same game servers. If you connect through the
"wrong" port, it may currently still work — you will just get warnings in the
logs. That will change as soon as encryption keys or methods differ.

## Server and client on the same machine

Use any IP of `127.x.x.x` **except** `127.0.0.1`, because that one is blocked by
the client. `127.127.127.127` is the usual choice, and the `loopback` IP resolver
of the server reports exactly that address.

## Disconnects after selecting a server

The server selection screen works like this: the client asks the connect server
for the address of the selected game server, and the server answers with the
address that its **IP resolver** determined. If that address is not reachable
from the client, the client disconnects right after the server selection.

Fix it in the admin panel at **Configuration → System** by choosing a matching
IP resolver:

| Resolver | Reports |
|---|---|
| `Auto` | Detected automatically by considering the environment |
| `Public` | The public IP, determined via an external service ([ipify](https://www.ipify.org/)) |
| `Local` | A local IP of the host; falls back to the loopback IP |
| `Loopback` | `127.127.127.127` — for testing on the same machine |
| `Custom` | A fixed IP address or host name that you enter yourself |

See [Startup parameters](../deployment/startup-parameters.md) for the equivalent
start parameters and environment variables.
