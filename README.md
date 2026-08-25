# OpenMU Project

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/d0f57e29e7524dadb677561389256d8b)](https://www.codacy.com/gh/MUnique/OpenMU/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=MUnique/OpenMU&amp;utm_campaign=Badge_Grade)
[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/MUnique/OpenMU)
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
version of Season 6 Episode 3 using the ENG (english) protocol. Additionally,
the long-term focus is on the [open source client](https://github.com/sven-n/MuMain)
which supports a slightly extended network protocol.
However, parts of the software can also be suitable for the development of
other games, even for other kind of games.

The code is a complete rewrite from scratch - it's not based on pre-existing
projects, and it's also explicitly not based on decompiled server sources or
their countless derivates.

There also exists a [blog](https://munique.net) which may contain some valuable
information about this development.

## Current project state

This project is currently under development without any release.
You can try the current state by using the available docker image, see
[Run with Docker](docs-website/docs/getting-started/docker.md).

## Documentation

The documentation lives in the [docs-website](docs-website) folder and is built
as a website with [Docusaurus](https://docusaurus.io/). The links below point to
its sources, so they work on GitHub as well.

| | |
|---|---|
| [Getting started](docs-website/docs/getting-started/requirements.md) | Requirements, ports, running with Docker or from source, connecting a game client, test accounts |
| [Deployment](docs-website/docs/deployment/overview.md) | The deployment variants, HTTPS, start parameters and environment variables |
| [Admin panel](docs-website/docs/admin-panel/overview.md) | One page per screen of the server's user interface, plus task-oriented how-tos |
| [Server features](docs-website/docs/server-features/bots.md) | Features with their own configuration, e.g. the server-side AI bots |
| [Development](docs-website/docs/development/architecture.md) | Architecture, solution structure, the plugin system, contributing |
| [Reference](docs-website/docs/reference/ports.md) | Ports and the generated packet documentation |

The code-bound technical documentation stays next to the code: see [docs](docs)
for the packet descriptions and the game mechanics, and the `Readme.md` of the
individual projects under [src](src).

To run the website locally:

```bash
cd docs-website
npm install
npm start
```

## Licensing

This project is released under the MIT license (see LICENSE file).

## Contributions

Contributions are welcome — from developers and non-developers alike. Please
read [CONTRIBUTING.md](CONTRIBUTING.md) before you start, and don't hesitate to
ask in our [discord channel](https://discord.gg/2u5Agkd) or by submitting an
issue.
