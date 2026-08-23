---
id: intro
title: OpenMU Documentation
sidebar_label: Introduction
sidebar_position: 1
slug: /
description: Documentation for the OpenMU project - an open source MU Online server.
---

# OpenMU Documentation

OpenMU is an easy to use, extendable and customizable server for the MMORPG
*MU Online*. The server supports multiple versions of the game; the main focus is
Season 6 Episode 3 using the ENG (english) protocol. Additionally, the long-term
focus is on the [open source client](https://github.com/sven-n/MuMain), which
supports a slightly extended network protocol.

The code is a complete rewrite from scratch. It is not based on pre-existing
projects, and it is explicitly not based on decompiled server sources or any of
their derivates.

:::info[Project state]
This project is under development and there is no release yet. You can try the
current state with the available docker image — see
[Run with Docker](getting-started/docker.md).
:::

## Where to start

| I want to … | Start here |
|---|---|
| … just try it out on my machine | [Run with Docker](getting-started/docker.md) |
| … host a server for other players | [Deployment overview](deployment/overview.md) |
| … configure my server (rates, items, monsters, …) | [Admin Panel](admin-panel/overview.md) |
| … build and debug the server myself | [Run from source](getting-started/from-source.md) |
| … understand how OpenMU works internally | [Architecture](development/architecture.md) |
| … contribute | [Contributing](development/contributing.md) |

## Used technologies

The project is mainly written in C# and targets .NET 10.0.

The server's admin panel is hosted on an embedded ASP.NET Core web server
(Kestrel) and implemented as a Blazor Server App.

The persistence layer uses [Entity Framework Core](https://github.com/dotnet/efcore)
with [PostgreSQL](https://www.postgresql.org) as the database. Additionally, it
is possible to start the server in a non-persistent in-memory mode.

The project is prepared to be hosted in a single process or distributed over
multiple processes. For the communication between the processes, [Dapr](https://dapr.io/)
is used.

## Gameplay differences to the original server

This project does not have the goal to copy the original MU Online server
behaviour to 100 %. That is not entirely possible, because the original server is
written in another programming language and has a completely different
architecture. In some points we make our life easier, in other points we try to
improve the gameplay.

### Calculations

The calculations of attribute values (like character damage decrement etc.) are
done with 32 bit float numbers and without rounding off, like the original server
does at some places.

For example, distributed stat points always have an effect, while in the original
server effects might get rounded down: when 4 points of strength give 1 base
damage, the original server doesn't calculate a fraction of 1 damage for 3
points, while OpenMU calculates 0.75 damage. This damage then has an effect in
further calculations.

### Countdown when changing character or sub-server

The original server uses a five second countdown when a player wants to change
their character or the sub-server. Maybe this was done for performance reasons,
as the original server would then save the character/account data. We think
that's really annoying and see no real value in it, so we don't use a countdown.

## Getting help

* [Discord](https://discord.gg/2u5Agkd) — the fastest way to ask a question
* [GitHub issues](https://github.com/MUnique/OpenMU/issues) — bugs and feature
  requests
* [Blog](https://munique.net) — background articles about the development
