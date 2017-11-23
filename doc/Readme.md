# Documentation

This project should contain all the (technical) documentation of the OpenMU project, including packet descriptions, game mechanics and software architecture.

### C# Project?
As you might noticed, it's created as C#-project, but doesn't contain any code.
It's a workaround to have it available in Visual Studio as separate project.
Additionally we can later add msbuild tasks to export this documentation in other formats.


### Why not using the wiki?
We think that a lot of the documentation (especially packet descriptions) is based on the actual code.
This documentation is not added to the wiki, because we can't create a branches of a wiki.
In case we want to support other versions of the game, we branch this files together with the code so everything fits together.
We also think that all the (technical) documentation should be available within the code (and IDE), so there is no point in using the wiki.

However, documentation like getting the project run (like a users manual) could be put on the wiki, of course.

## Game Server Architecture

Disclaimer (by [sven-n](https://github.com/sven-n)):
*This is not a perfect architecture, if such architecture exists anyway.
However, it makes sense to me for this purpose, as an enterprise business application developer.
I tried to make it flexible and I hope it's not too complicated.*

For the big picture, you may have a look at the [architecture overview](architecture%20overview.png).

There are interfaces for the interoperability between the different "servers" or sub-systems in [MUnique.OpenMU.Interfaces](../src/Interfaces/Readme.md).

### Communication between game client and server
The network communication between game client and the game server takes place through the [Connection class](../src/Network/Connection.cs). MUnique.OpenMU.Network contains all what's required to connect from and to a game server, using the MU Online network protocol.

#### Client -> Server
When receiving data from the game client, it gets forwarded to the packet handlers located at namespace MUnique.OpenMU.GameServer.MessageHandler.
These message handlers are parsing the data packets and then calls the player actions located 
at MUnique.OpenMU.GameLogic.PlayerActions, which should have no knowledge of the packet structure
or how the communication took place.

The packet handlers are configured in the GameConfiguration (database), so that they can be replaced by extended or modified versions.
It's possible to offer different protocols to work on the same game, by configuring multiple main packet handlers which can be bound to separate tcp ports.

#### Server -> Client
The other way - data sent to the game game client - is done by views (MUnique.OpenMU.GameServer.RemoteView).
These views are using the Connection class to send the data in the specified protocol. The GameLogic has no
knowledge about this protocol and just works with the [view interfaces](../src/GameLogic/Views/IPlayerView.cs).

Currently, this is not as configurable as the packet handler configuration - the [RemotePlayer](../src/GameServer/RemoteView/RemotePlayer.cs)
directly creates the specific [RemoteView](../src/GameServer/RemoteView/RemoteView.cs).
A better configurability might be good goal for the future.

#### Benefits of this architecture
As you can see, the GameLogic itself does not know how the player actions are triggered or how the "view" look like.
Instead of working with the network, there could be an implementation of a [player view](../src/GameLogic/Views/IPlayerView.cs) which is literally a graphical user interface.
Also instead of calling the player actions by packet handlers, a user interface could call them instead.
So this project could be a base for a (non-MU) game client which then could also support multiplayer and co-op with the existing server components.


### Data access
The access pattern is mainly this:
  * At server start, the game configuration is loaded
  * When a game client logged in, it's account is loaded
  * During the game, the account data is saved at specific points and in an time interval

#### Design goal
It should be possible to support multiple different databases, also NoSQL ones, without changing the game logic.
For this purpose we don't just use SQL and database-specific code in the game logic.
The access patterns should keep this in mind. So instead of a lot single calls to a lot of different repositories in the game logic, one big call should be done.
For example, if we want to use a document based database, an account could be one document and the game configuration as a whole could also be one document.

#### Abstractions
To accomplish the design goals, the game logic (and other parts) are using abstractions, [Repositories](https://martinfowler.com/eaaCatalog/repository.html), to access data.
These abstractions are located at the MUnique.OpenMU.Persistence namespace.

We use a context-based approach to access data, e.g. the [GameConfiguration](../src/DataModel/Configuration/GameConfiguration.cs) is loaded through a [GameConfigurationRepository](../src/Persistence/EntityFramework/GameConfigurationRepository.cs) while "using" a [context](../src/Persistence/IContext.cs), and each connected player uses its own [context](../src/Persistence/IContext.cs) to load its [Account](../src/DataModel/Entities/Account.cs).
When saving an account, we actually save the it's context. The context then takes care that every required change is done at the database.
When accessing or creating new persistent objects, the [context](../src/Persistence/IContext.cs) needs to be "in use" on the current thread, because the actual context implementation may need to track these objects.
There is a [repository manager](../src/Persistence/IRepositoryManager.cs) which takes care of a lot of things (too many responsibilites?), e.g. creating new objects, using or creating contexts.

#### Current implementation and supported database ####
At the moment the persistence layer is implemented by [MUnique.OpenMU.Persistence.EntityFramework](../src/Persistence/EntityFramework/Readme.md) which uses the [Entity Framework Core](https://github.com/aspnet/EntityFrameworkCore) and [PostgreSQL](https://www.postgresql.org/) as database.

#### Future
Because the data model is pretty complicated (which is required if the configuration should be that flexible), a full relational model 
on a database is probably not the best thing to do (performance wise). Loading the game configuration takes about 15 seconds at the moment, and the data isn't even complete yet.
A solution could be to mix relational tables with json columns or to fully switch to a document based database (e.g. RavenDB).

### Further informations
  * [Admin Panel](../src/AdminPanel/Readme.md): The user inferface of the server
  * [Attribute System](../src/AttributeSystem/Readme.md): Damage calculation and player attributes are based on that
  * [Network](../src/Network/Readme.md): About the network communication
  * [Startup](../src/Startup/Readme.md): It's the project for the executeable which puts every piece of the puzzle together
