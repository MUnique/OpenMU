# Documentation

This directory contains the technical documentation which is bound to the code:
the [packet descriptions](Packets/Readme.md), game mechanics and implementation
notes.

## Where the documentation lives

We split the documentation by how closely it follows the code:

* **The documentation website**, built from the [docs-website](../docs-website)
  folder, holds everything a user, server operator or new contributor reads:
  getting started, deployment, the [admin panel](../docs-website/docs/admin-panel/overview.md),
  the [server features](../docs-website/docs/server-features/bots.md) and the
  [architecture](../docs-website/docs/development/architecture.md).

* **This folder and the `Readme.md` files under [src](../src)** hold the
  documentation which is derived from the code itself. A lot of it — the packet
  descriptions above all — is generated from the sources, so it belongs next to
  them and is versioned with them. The implementation details live here too, for
  example [the plugin system](../src/PlugIns/Readme.md).

That is also why there is no wiki: documentation which is based on the actual
code should live with the code, and everything else is on the website, where it
can be searched and navigated.

## Contents of this folder

* [Packets](Packets/Readme.md): the structures of the messages which are
  exchanged between client and server, generated from the XML sources at
  [src/Network/Packets](../src/Network/Packets)

* [Master Skill System](MasterSystem.md): the master skill tree, its rules and
  how it is implemented on client and server side

* [GameMap](GameMap.md): how the game map and the area of interest management
  work

* [Packet structure tests](PacketStructureTests.md): how the packet structures
  are verified

* [Progress](Progress.md): the implementation progress of the packet handlers.
  See also the [normal skill progress](https://github.com/MUnique/OpenMU/projects/9)
  and the [master skill progress](https://github.com/MUnique/OpenMU/projects/10)

* [Player refactoring plan](PlayerRefactoringPlan.md): an internal plan for the
  refactoring of the player class

* [architecture overview.png](architecture%20overview.png): the big picture,
  explained on the
  [architecture page](../docs-website/docs/development/architecture.md) of the
  website

## Documentation in the projects

Several projects document their implementation next to their code:

* [Network](../src/Network/Readme.md) and its
  [analyzer](../src/Network/Analyzer/Readme.md)
* [Persistence](../src/Persistence/Readme.md), its
  [EntityFramework implementation](../src/Persistence/EntityFramework/Readme.md)
  with the [migrations cheatsheet](../src/Persistence/EntityFramework/Migrations/Cheatsheet.md),
  and the [data initialization](../src/Persistence/Initialization/Readme.md)
* [AttributeSystem](../src/AttributeSystem/Readme.md)
* [Pathfinding](../src/Pathfinding/Readme.md)
* [Interfaces](../src/Interfaces/Readme.md)
* [ClientErrorLogDecryptor](../src/ClientErrorLogDecryptor/Readme.md)
* [ExDbConnector](../src/ChatServer/ExDbConnector/Readme.md)
