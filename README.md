# OpenMU Project

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/d0f57e29e7524dadb677561389256d8b)](https://www.codacy.com/gh/MUnique/OpenMU/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=MUnique/OpenMU&amp;utm_campaign=Badge_Grade)
[![Gitter chat](https://badges.gitter.im/OpenMU-Project/gitter.svg)](https://gitter.im/OpenMU-Project/Lobby)
[![Discord chat](https://img.shields.io/discord/669595902750490698?logo=discord)](https://discord.gg/2u5Agkd)

| Platform       |Build Status          |
|----------------|----------------------|
| Windows        | ![Windows Build Status](https://dev.azure.com/MUnique/OpenMU/_apis/build/status/MUnique.OpenMU?branchName=master) |
| Linux (Docker) | [![Docker Build Status](https://dev.azure.com/MUnique/OpenMU/_apis/build/status/MUnique.OpenMU%20Docker?branchName=master)](https://hub.docker.com/r/munique/openmu)  |

| NuGet Packages |   |
|----------------|---|
| MUnique.OpenMU.Network | [![NuGet Badge](https://img.shields.io/nuget/v/MUnique.OpenMU.Network)](https://www.nuget.org/packages/MUnique.OpenMU.Network/) |
| MUnique.OpenMU.Network.Packets | [![NuGet Badge](https://img.shields.io/nuget/v/MUnique.OpenMU.Network.Packets)](https://www.nuget.org/packages/MUnique.OpenMU.Network.Packets/) |

This project aims to create an easy to use, extendable and customizable server
for a MMORPG called "MU Online".
The server supports multiple versions of the game, but the main focus is
version of Season 6 Episode 3 using the ENG (english) protocol.
However, parts of the software can also be suitable for the development of
other games, even for other kind of games.

The code is a complete rewrite from scratch - it's not based on pre-existing
projects, and it's also explicitly not based on decompiled server sources or
their countless derivates.

There also exists a [blog](https://munique.net) which may contain some valuable
information about this development.

## Current project state

This project is currently under development without any release.
You can try the current state by using the available docker image, also
mentioned in the [quick start guide](QuickStart.md).

## Licensing

This project is released under the MIT license (see LICENSE file).

## Used technologies

The project is mainly written in C# and targets .NET 8.0.

The servers admin panel is hosted on an embedded ASP.NET Core webserver (Kestrel)
and implemented as Blazor Server App.

At the moment the persistence layer uses the [Entity Framework Core](https://github.com/aspnet/EntityFrameworkCore)
and [PostgreSQL](https://www.postgresql.org) as database. Additionally, it's
also possible to start it in a non-persistent in-memory mode.

The project supports distributed hosting based on Dapr. Alternatively, it can be
hosted in one process as well.

## Deployment

We provide Docker images and docker-compose files for easy deployment.
Please take a look at the deploy-folder of this project.

## Contributions

Contributions are welcome if they meet the following criteria:

* Language is english.

* Code should be StyleCop compliant - this project uses the [StyleCop.Analyzers](https://www.nuget.org/packages/StyleCop.Analyzers/)
  for VS2022 so you should see issues directly as warnings.

* Coding style (naming, etc.) and quality should fit to the current state.

* No code copied/converted from the well-known decompiled source of the
    original server.

If you want to contribute, please create a new issue for the feature or bug (if
the issue doesn't exist yet) so we can see who is working on something and can
discuss possible solutions. If it's a small thing, you can also just send a
pull request without adding an issue.

Apart of that, contributions from non-developers are welcome as well. You can
test the server, submit issues or suggestions, packet descriptions or
documentations about the concepts and mechanics of the game itself. Please use
markdown files/syntax for this purpose.

If you have questions about that, don't hesitate to ask in our [discord channel](https://discord.gg/2u5Agkd)
or by submitting an issue.

## How to contribute code

If you want to contribute code, please do the following steps:

1. fork this project from the original MUnique OpenMU Project.
2. create a feature branch from the master branch
3. commit your changes to your feature branch
4. submit a pull request to the original master branch
5. lean back, wait for the code review and merge :)

## How to use

Please have a look at the [quick start guide](QuickStart.md).

## Gameplay differences to the original server

This project doesn't have the goal to copy the original MU Online server
behavior to 100 %. This is not entirely possible, because the original server
is written in another programming language and has a completely different
architecture.
With some points we make our life easier in this project, with other points we
try to improve the gameplay.

### Calculations

The calculations of attribute values (like character damage decrement etc.) are
done with 32 bit float numbers and without rounding off, like the original
server does at some places.
E.g. distributed stat points always have effect, while in the original server
effects might get rounded down. For example, when 4 points of strength gives 1
base damage, the original server doesn't calculate a fraction of 1 damage for
3 points, while OpenMU calculates 0.75 damage. This damage
has then an effect in further calculations.

### Countdown when changing character or sub-server

The original server uses a five second countdown when a player wants to change
his character or the sub-server. Maybe this was done for some performance
reasons, as the original server would then save the character/account data.
We think that's really annoying and see no real value in that, so we don't use
a countdown.
