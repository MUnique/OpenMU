---
title: Architecture
sidebar_position: 1
description: How the OpenMU server is structured internally.
---

# Architecture

> Disclaimer (by [sven-n](https://github.com/sven-n)): *This is not a perfect
> architecture, if such an architecture exists anyway. However, it makes sense to
> me for this purpose, as an enterprise business application developer. I tried
> to make it flexible and I hope it's not too complicated.*

For the big picture, have a look at the architecture overview:

![Architecture overview](/img/architecture-overview.png)

There are interfaces for the interoperability between the different "servers" or
subsystems in
[MUnique.OpenMU.Interfaces](https://github.com/MUnique/OpenMU/tree/master/src/Interfaces).

## Communication between game client and server

The network communication between game client and game server takes place through
the [Connection class](https://github.com/MUnique/OpenMU/tree/master/src/Network/Connection.cs).
`MUnique.OpenMU.Network` contains everything required to connect from and to a
game server using the MU Online network protocol. It also contains the message
structs of the messages in `MUnique.OpenMU.Network.Packets`.

### Client → Server

When data is received from the game client, it is forwarded to the packet
handlers in the namespace `MUnique.OpenMU.GameServer.MessageHandler`. Every
handler is an implementation of an `IPacketHandlerPlugIn`.

These message handlers parse the data packets and then call the player actions in
`MUnique.OpenMU.GameLogic.PlayerActions`, which have no knowledge of the packet
structure or how the communication took place.

### Server → Client

The other way — data sent to the game client — is done by views
(`MUnique.OpenMU.GameServer.RemoteView`). These views use the `Connection` class
to send the data in the specified protocol. The game logic has no knowledge about
this protocol and just works with the
[view interface plugins](https://github.com/MUnique/OpenMU/tree/master/src/GameLogic/Views/IViewPlugIn.cs).

### Benefits of this architecture

The game logic itself does not know how the player actions are triggered or how
the "view" looks like.

Instead of working with the network, there could be an implementation of
[view plugins](https://github.com/MUnique/OpenMU/tree/master/src/GameLogic/Views/IViewPlugIn.cs)
which is literally a graphical user interface. Also, instead of calling the
player actions by packet handler plugins, a user interface could call them
instead. So this project could be a base for a (non-MU) game client which could
then also support multiplayer and co-op with the existing server components.

All plugins are configurable over the [admin panel](../admin-panel/plugins.md).
They can be activated or deactivated, so they can be replaced by extended or
modified versions.

It is also possible to offer different protocols working on the same game world,
by implementing multiple view and packet handlers with different client version
attributes. Each game server can have multiple TCP listeners bound to separate
TCP ports for different client versions, too.

## Data access

The access pattern is mainly this:

* At server start, the game configuration is loaded.
* When a game client logs in, its account is loaded.
* During the game, the account data is saved at specific points and in a time
  interval.

### Design goal

It should be possible to support multiple different databases, also NoSQL ones,
without changing the game logic. For this purpose we don't just use SQL and
database-specific code in the game logic.

The access patterns keep this in mind: instead of a lot of single calls to a lot
of different repositories in the game logic, one big call should be done. For
example, if we want to use a document based database, an account could be one
document and the game configuration as a whole could also be one document.

### Abstractions

To accomplish the design goals, the game logic (and other parts) use
abstractions, [repositories](https://martinfowler.com/eaaCatalog/repository.html),
to access data. These abstractions are located in the
`MUnique.OpenMU.Persistence` namespace.

We use a context-based approach to access data: the
[GameConfiguration](https://github.com/MUnique/OpenMU/tree/master/src/DataModel/Configuration/GameConfiguration.cs)
is loaded through a
[GameConfigurationRepository](https://github.com/MUnique/OpenMU/tree/master/src/Persistence/EntityFramework/GameConfigurationRepository.cs)
while "using" a
[context](https://github.com/MUnique/OpenMU/tree/master/src/Persistence/IContext.cs),
and each connected player uses its own
[player context](https://github.com/MUnique/OpenMU/tree/master/src/Persistence/IPlayerContext.cs)
to load its
[Account](https://github.com/MUnique/OpenMU/tree/master/src/DataModel/Entities/Account.cs).

When saving an account, we actually save its context. The context then takes care
that every required change is done at the database. When accessing or creating
new persistent objects, the context needs to be "in use" on the current thread,
because the actual context implementation may need to track these objects. It
takes care of a lot of things, e.g. creating new objects. Contexts can be created
with the
[PersistenceContextProvider](https://github.com/MUnique/OpenMU/tree/master/src/Persistence/IPersistenceContextProvider.cs).

### Current implementation and supported database

At the moment the persistence layer is implemented by
[MUnique.OpenMU.Persistence.EntityFramework](https://github.com/MUnique/OpenMU/tree/master/src/Persistence/EntityFramework),
which uses [Entity Framework Core](https://github.com/dotnet/efcore) and
[PostgreSQL](https://www.postgresql.org/) as database.

### Future

Because the data model is pretty complicated (which is required if the
configuration should be that flexible), a full relational model on a database is
probably not the best thing to do, performance wise.

Currently, we use an approach to load the game configuration or account with just
one big dynamically generated query which gives us the data as JSON. That's
surprisingly fast — the query to load the really complex game configuration
finishes in about 1.5 seconds.

If there is a problem in the future, we could go further and mix relational
tables with JSON columns, or fully switch to a document based database.

## Further reading

On this site:

* [Solution structure](solution-structure.md) — what lives in which project
* [Packet documentation](../reference/packets.md) — the structures exchanged
  between client and server

The deeper, code-bound technical documentation stays next to the code in the
repository:

* [The plugin system](https://github.com/MUnique/OpenMU/blob/master/src/PlugIns/Readme.md) — how to define a plugin point, implement a
  plugin, and load custom ones
* [Master skill system](https://github.com/MUnique/OpenMU/blob/master/docs/MasterSystem.md)
* [GameMap](https://github.com/MUnique/OpenMU/blob/master/docs/GameMap.md) — how
  the area of interest management works
* [Packet structure tests](https://github.com/MUnique/OpenMU/blob/master/docs/PacketStructureTests.md)
* [Progress](https://github.com/MUnique/OpenMU/blob/master/docs/Progress.md) —
  the feature implementation progress
* [Attribute system](https://github.com/MUnique/OpenMU/tree/master/src/AttributeSystem)
  — damage calculation and player attributes are based on it
