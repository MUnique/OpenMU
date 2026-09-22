---
title: Solution structure
sidebar_position: 2
description: What lives in which project of the OpenMU solution.
---

# Solution structure

The solution is at `src/MUnique.OpenMU.sln`. Each project has a `Readme.md` next
to its code with the details; this page is the map.

## Servers and game logic

| Project | Contents |
|---|---|
| `Startup` | The console program which glues everything together and starts the server as a single process — see [Startup parameters](../deployment/startup-parameters.md) |
| `GameServer` | The game server: packet handlers (`MessageHandler`) and the remote views (`RemoteView`) |
| `GameLogic` | The actual game: players, items, skills, maps, attacks, quests, mini games, player actions |
| `ConnectServer` | Serves the server list and redirects clients to a game server |
| `LoginServer` | Keeps track of which account is logged in on which server |
| `ChatServer` | The chat rooms used by the in-game messenger |
| `FriendServer` | Friend lists and the messenger |
| `GuildServer` | Guilds |
| `Interfaces` | The interfaces between the subsystems, so they can run in one or many processes |
| `Dapr` | The glue for the distributed deployment |

## Foundations

| Project | Contents |
|---|---|
| `Network` | The MU Online network protocol, connections, encryption — [Readme](https://github.com/MUnique/OpenMU/blob/master/src/Network/Readme.md) |
| `Network/Packets` | The packet definitions; the packet markdown documentation is generated from their XML |
| `AttributeSystem` | The attribute/power-up system used for damage and stat calculation — [Readme](https://github.com/MUnique/OpenMU/blob/master/src/AttributeSystem/Readme.md) |
| `Pathfinding` | Pathfinding for monsters and NPCs — [Readme](https://github.com/MUnique/OpenMU/blob/master/src/Pathfinding/Readme.md) |
| `PlugIns` | The plugin infrastructure — [Readme](https://github.com/MUnique/OpenMU/blob/master/src/PlugIns/Readme.md) |
| `Annotations` / `SourceGenerators` | Attributes and source generators used across the solution |

## Data

| Project | Contents |
|---|---|
| `DataModel` | The entities and the configuration classes — this is what the admin panel edits |
| `Persistence` | The persistence abstractions (contexts, repositories) — [Readme](https://github.com/MUnique/OpenMU/blob/master/src/Persistence/Readme.md) |
| `Persistence/EntityFramework` | The EF Core + PostgreSQL implementation — [Readme](https://github.com/MUnique/OpenMU/blob/master/src/Persistence/EntityFramework/Readme.md) |
| `Persistence/Initialization` | The data initialization per game version — [Readme](https://github.com/MUnique/OpenMU/blob/master/src/Persistence/Initialization/Readme.md) |

## Web

| Project | Contents |
|---|---|
| `Web/AdminPanel` | The [admin panel](../admin-panel/overview.md) |
| `Web/Map` | The [live map](../admin-panel/live-map.md) |
| `Web/ItemEditor` | The graphical item and item storage editor |
| `Web/Shared` | Components shared by the web projects: forms, modals, the map editor, the theme and culture selectors |

## Tools

| Project | Contents |
|---|---|
| `ClientLauncher` | The launcher which starts the game client with your server's address |
| `Network/Analyzer` | The platform independent part of the network analysis: captured connections and their packet analysis — [Readme](https://github.com/MUnique/OpenMU/blob/master/src/Network/Analyzer/Readme.md) |
| `Network/Analyzer.WinForms` | The tool which captures the traffic between client and server by acting as a proxy — [Readme](https://github.com/MUnique/OpenMU/blob/master/src/Network/Analyzer.WinForms/Readme.md) |
| `ClientErrorLogDecryptor` | Decrypts the client's error logs — [Readme](https://github.com/MUnique/OpenMU/blob/master/src/ClientErrorLogDecryptor/Readme.md) |
| `SimpleModulusKeyGenerator` | Generates keys for the SimpleModulus encryption |
| `ChatServer/ExDbConnector` | Connector for the ExDB protocol — [Readme](https://github.com/MUnique/OpenMU/blob/master/src/ChatServer/ExDbConnector/Readme.md) |

## Tests

The tests are in the `tests/` folder, one project per area
(`MUnique.OpenMU.GameLogic.Benchmarks`, `MUnique.OpenMU.Network.Tests`,
`MUnique.OpenMU.Persistence.Initialization.Tests`, …). Run them with:

```bash
dotnet test
```

See also
[Packet structure tests](https://github.com/MUnique/OpenMU/blob/master/docs/PacketStructureTests.md).

## This documentation

The site you are reading is built with [Docusaurus](https://docusaurus.io/) from
the `docs-website/` folder — see its
[README](https://github.com/MUnique/OpenMU/blob/master/docs-website/README.md)
for how to run it locally and how it is deployed.
