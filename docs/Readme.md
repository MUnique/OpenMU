# Documentation

This directory should contain all the (technical) documentation of the OpenMU
project, including [packets descriptions](Packets/Readme.md), game mechanics
and software architecture.

## Why not using the wiki?

We think that a lot of the documentation (especially packet descriptions) is
based on the actual code. So it makes sense that all the (technical)
documentation should be available within the code, so there is no point in
using the wiki.

However, documentation like getting the project run (like a users manual)
could be put on the wiki, of course.

## Game Server Architecture

Disclaimer (by [sven-n](https://github.com/sven-n)):
*This is not a perfect architecture, if such architecture exists anyway.
However, it makes sense to me for this purpose, as an enterprise business
application developer. I tried to make it flexible and I hope it's not too
complicated.*

For the big picture, you may have a look at the
[architecture overview](architecture%20overview.png).

There are interfaces for the interoperability between the different "servers"
or sub-systems in [MUnique.OpenMU.Interfaces](https://github.com/MUnique/OpenMU/tree/master/src/Interfaces).

### Communication between game client and server

The network communication between game client and the game server takes place
through the [Connection class](https://github.com/MUnique/OpenMU/tree/master/src/Network/Connection.cs).
MUnique.OpenMU.Network contains all what's required to connect from and to a
game server, using the MU Online network protocol. It also contains the
message structs of the messages in MUnique.OpenMU.Network.Packets.

#### Client -> Server

When receiving data from the game client, it gets forwarded to the packet
handlers located at namespace MUnique.OpenMU.GameServer.MessageHandler. Every
handler is an implementation of a IPacketHandlerPlugIn.
These message handlers are parsing the data packets and then calls the player
actions located at MUnique.OpenMU.GameLogic.PlayerActions, which should have no
knowledge of the packet structure or how the communication took place.

#### Server -> Client

The other way - data sent to the game game client - is done by views
(MUnique.OpenMU.GameServer.RemoteView).
These views are using the Connection class to send the data in the specified
protocol. The GameLogic has no knowledge about this protocol and just works
with the [view interface plugins](https://github.com/MUnique/OpenMU/tree/master/src/GameLogic/Views/IViewPlugIn.cs).

#### Benefits of this architecture

As you can see, the GameLogic itself does not know how the player actions are
triggered or how the "view" look like.
Instead of working with the network, there could be an implementation of
[view plugins](https://github.com/MUnique/OpenMU/tree/master/src/GameLogic/Views/IViewPlugIn.cs)
which is literally a graphical user interface.
Also instead of calling the player actions by packet handler plugins, a user
interface could call them instead.
So this project could be a base for a (non-MU) game client which then could
also support multiplayer and co-op with the existing server components.

All plugins are configurable over the AdminPanel. They can be activated or
deactivated, so that they can be replaced by extended or modified versions.
It's also possible to offer different protocols to work on the same game world,
by implementing multiple view and packet handlers with different client
versions attributes. Each game server can have multiple tcp listeners which can
be bound to separate tcp ports for different client versions, too.

### Data access

The access pattern is mainly this:

* At server start, the game configuration is loaded

* When a game client logged in, it's account is loaded

* During the game, the account data is saved at specific points and in an
    time interval

#### Design goal

It should be possible to support multiple different databases, also NoSQL ones,
without changing the game logic.
For this purpose we don't just use SQL and database-specific code in the game
logic. The access patterns should keep this in mind. So instead of a lot single
calls to a lot of different repositories in the game logic, one big call should
be done.
For example, if we want to use a document based database, an account could be
one document and the game configuration as a whole could also be one document.

#### Abstractions

To accomplish the design goals, the game logic (and other parts) are using
abstractions, [Repositories](https://martinfowler.com/eaaCatalog/repository.html),
to access data.
These abstractions are located at the MUnique.OpenMU.Persistence namespace.

We use a context-based approach to access data, e.g. the [GameConfiguration](https://github.com/MUnique/OpenMU/tree/master/src/DataModel/Configuration/GameConfiguration.cs)
is loaded through a [GameConfigurationRepository](https://github.com/MUnique/OpenMU/tree/master/src/Persistence/EntityFramework/GameConfigurationRepository.cs)
while "using" a [context](https://github.com/MUnique/OpenMU/tree/master/src/Persistence/IContext.cs),
and each connected player uses its own [player context](https://github.com/MUnique/OpenMU/tree/master/src/Persistence/IPlayerContext.cs)
to load its [Account](https://github.com/MUnique/OpenMU/tree/master/src/DataModel/Entities/Account.cs).
When saving an account, we actually save the it's context. The context then
takes care that every required change is done at the database.
When accessing or creating new persistent objects, the [context](https://github.com/MUnique/OpenMU/tree/master/src/Persistence/IContext.cs)
needs to be "in use" on the current thread, because the actual context
implementation may need to track these objects.
It takes care of a lot of things, e.g. creating new objects. Contexts can be
created with the [PersistenceContextProvider](https://github.com/MUnique/OpenMU/tree/master/src/Persistence/IPersistenceContextProvider.cs).

#### Current implementation and supported database

At the moment the persistence layer is implemented by [MUnique.OpenMU.Persistence.EntityFramework](../src/Persistence/EntityFramework/Readme.md)
which uses the [Entity Framework Core](https://github.com/aspnet/EntityFrameworkCore)
and [PostgreSQL](https://www.postgresql.org/) as database.

#### Future

Because the data model is pretty complicated (which is required if the
configuration should be that flexible), a full relational model
on a database is probably not the best thing to do (performance wise).
Currently, we use an approach to load the game configuration or account with
just one big dynamically generated query which gives us the data as json.
That's suprisingly fast, as the query itself to load the really complex game
configuration finishes in about 1.5 seconds.

If there might be a problem in the future, we could go further and mix
relational tables with json columns or to fully switch to a document based
database (e.g. RavenDB).

### Further information

* [Packets](Packets/Readme.md): Information about the packet structures

* [Master Skill System](MasterSystem.md): Description about the master skill system

* [GameMap](GameMap.md): Description about the GameMap implementation

* [Progress](Progress.md): Information about  the feature implementation
progress of the project. See also:
  * [Normal skill progress](https://github.com/MUnique/OpenMU/projects/9)
  * [Master skill progress](https://github.com/MUnique/OpenMU/projects/10)

* [Admin Panel](https://github.com/MUnique/OpenMU/tree/master/src/Web/AdminPanel):
  The user inferface of the server

* [Attribute System](https://github.com/MUnique/OpenMU/tree/master/src/AttributeSystem):
  Damage calculation and player attributes are based on that

* [Network](https://github.com/MUnique/OpenMU/tree/master/src/Network): About the
  network communication

* [Startup](https://github.com/MUnique/OpenMU/tree/master/src/Startup): It's the
  project for the executeable which puts every piece of the puzzle together
