# Documentation / Documentación

*Read this documentation in [English](#english) or [Español](#espanol).* 

<a id="english"></a>
## English

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

<a id="espanol"></a>
## Español

Este directorio debe contener toda la documentación técnica de OpenMU,
incluyendo [descripciones de packets](Packets/Readme.md), mecánicas del
game y arquitectura de software.

## ¿Por qué no usar la wiki?

Pensamos que gran parte de la documentación (especialmente la de packets)
se basa en el código. Por eso tiene sentido que toda la documentación
técnica esté disponible junto al código, sin necesidad de la wiki.

Sin embargo, documentación como la guía de usuario para poner en marcha el
proyecto podría estar en la wiki.

## Arquitectura del Game Server

Descargo de responsabilidad (por [sven-n](https://github.com/sven-n)):
*No es una arquitectura perfecta, si es que existe. Sin embargo, tiene
sentido para este propósito como desarrollador de aplicaciones de negocio.
Intenté hacerla flexible y espero que no sea demasiado complicada.*

Para ver el panorama general, puedes mirar el
[architecture overview](architecture%20overview.png).

Existen interfaces para la interoperabilidad entre los diferentes
"servers" o subsistemas en
[MUnique.OpenMU.Interfaces](https://github.com/MUnique/OpenMU/tree/master/src/Interfaces).

### Comunicación entre el game client y el server

La comunicación de red entre el game client y el game server se realiza a
través de la [Connection class](https://github.com/MUnique/OpenMU/tree/master/src/Network/Connection.cs).
MUnique.OpenMU.Network contiene todo lo necesario para conectar desde y
hacia un game server usando el protocolo de red de MU Online. También
contiene las estructuras de mensaje en
MUnique.OpenMU.Network.Packets.

#### Client -> Server

Cuando se reciben datos del game client, se redirigen a los packet
handlers ubicados en el espacio de nombres
MUnique.OpenMU.GameServer.MessageHandler. Cada handler implementa un
IPacketHandlerPlugIn. Estos packet handlers analizan los data packets y
luego llaman a las player actions en
MUnique.OpenMU.GameLogic.PlayerActions, las cuales no deberían tener
conocimiento de la estructura del packet ni del medio de comunicación.

#### Server -> Client

El envío de datos al game client se realiza mediante views
(MUnique.OpenMU.GameServer.RemoteView). Estas views utilizan la Connection
class para enviar los datos en el protocolo especificado. GameLogic no
conoce este protocolo y solo trabaja con los
[view interface plugins](https://github.com/MUnique/OpenMU/tree/master/src/GameLogic/Views/IViewPlugIn.cs).

#### Beneficios de esta arquitectura

Como puedes ver, GameLogic en sí no sabe cómo se desencadenan las player
actions ni cómo luce la "view". En lugar de trabajar con la red, podría
haber una implementación de
[view plugins](https://github.com/MUnique/OpenMU/tree/master/src/GameLogic/Views/IViewPlugIn.cs)
que literalmente sea una interfaz gráfica. En vez de llamar las player
actions mediante packet handler plugins, una interfaz de usuario podría
llamarlas. Así, este proyecto podría ser la base de un game client (no MU)
que también pueda soportar multiplayer y co-op con los componentes de
server existentes.

Todos los plugins son configurables desde el AdminPanel. Pueden activarse
o desactivarse para reemplazarlos por versiones extendidas o modificadas.
También es posible ofrecer diferentes protocolos para trabajar en el mismo
game world implementando múltiples views y packet handlers con atributos
de versiones de client diferentes. Cada game server puede tener varios
tcp listeners que se vinculan a puertos tcp separados para distintas
versiones de client.

### Acceso a datos

El patrón de acceso es principalmente el siguiente:

* Al iniciar el server, se carga la game configuration
* Cuando un game client inicia sesión, se carga su account
* Durante el game, los datos del account se guardan en puntos
  específicos y en un intervalo de tiempo

#### Objetivo de diseño

Debería ser posible soportar múltiples bases de datos diferentes, incluso
NoSQL, sin cambiar la game logic. Para ello no usamos SQL ni código
específico de la base de datos en la game logic. Los patrones de acceso
deben tenerlo en cuenta. En lugar de muchas llamadas individuales a
diferentes repositories en la game logic, debería hacerse una gran
llamada.

Por ejemplo, si queremos usar una base de datos basada en documentos, un
account podría ser un documento y la game configuration completa otro.

#### Abstracciones

Para lograr estos objetivos de diseño, la game logic (y otras partes) usan
abstracciones, [Repositories](https://martinfowler.com/eaaCatalog/repository.html),
para acceder a datos. Estas abstracciones se encuentran en el namespace
MUnique.OpenMU.Persistence.

Utilizamos un enfoque basado en contextos para acceder a los datos, por
ejemplo, la
[GameConfiguration](https://github.com/MUnique/OpenMU/tree/master/src/DataModel/Configuration/GameConfiguration.cs)
se carga a través del
[GameConfigurationRepository](https://github.com/MUnique/OpenMU/tree/master/src/Persistence/EntityFramework/GameConfigurationRepository.cs)
mientras se "usa" un
[context](https://github.com/MUnique/OpenMU/tree/master/src/Persistence/IContext.cs),
y cada player conectado utiliza su propio
[player context](https://github.com/MUnique/OpenMU/tree/master/src/Persistence/IPlayerContext.cs)
para cargar su
[Account](https://github.com/MUnique/OpenMU/tree/master/src/DataModel/Entities/Account.cs).
Al guardar un account, guardamos su context, que se encarga de aplicar los
cambios requeridos en la base de datos. Cuando se accede o se crean nuevos
objetos persistentes, el
[context](https://github.com/MUnique/OpenMU/tree/master/src/Persistence/IContext.cs)
debe estar "en uso" en el hilo actual, porque la implementación del
context puede necesitar rastrear estos objetos. Se encarga de muchas
cosas, por ejemplo crear nuevos objetos. Los contexts pueden crearse con
el
[PersistenceContextProvider](https://github.com/MUnique/OpenMU/tree/master/src/Persistence/IPersistenceContextProvider.cs).

#### Implementación actual y base de datos soportada

Por el momento, la capa de persistencia está implementada por
[MUnique.OpenMU.Persistence.EntityFramework](../src/Persistence/EntityFramework/Readme.md)
que utiliza [Entity Framework Core](https://github.com/aspnet/EntityFrameworkCore)
y [PostgreSQL](https://www.postgresql.org/) como base de datos.

#### Futuro

Debido a que el data model es bastante complicado (lo cual es necesario si
la configuration debe ser tan flexible), un modelo relacional completo en
una base de datos probablemente no sea lo mejor en términos de
performance. Actualmente usamos una aproximación para cargar la game
configuration o el account con una consulta grande y generada
dinámicamente que nos da los datos como json. Sorprendentemente es rápido,
ya que la consulta para cargar la compleja game configuration termina en
aproximadamente 1.5 segundos.

Si hubiera un problema en el futuro, podríamos mezclar tablas relacionales
con columnas json o cambiar completamente a una base de datos basada en
documentos (por ejemplo, RavenDB).

### Más información

* [Packets](Packets/Readme.md): Información sobre las estructuras de packet
* [Master Skill System](MasterSystem.md): Descripción del master skill system
* [GameMap](GameMap.md): Descripción de la implementación de GameMap
* [Progress](Progress.md): Información sobre el progreso de implementación
  de las features del proyecto. Ver también:
  * [Normal skill progress](https://github.com/MUnique/OpenMU/projects/9)
  * [Master skill progress](https://github.com/MUnique/OpenMU/projects/10)
* [Admin Panel](https://github.com/MUnique/OpenMU/tree/master/src/Web/AdminPanel):
  La user inferface del server
* [Attribute System](https://github.com/MUnique/OpenMU/tree/master/src/AttributeSystem):
  El cálculo de damage y los atributos de player se basan en él
* [Network](https://github.com/MUnique/OpenMU/tree/master/src/Network): Sobre la
  comunicación de red
* [Startup](https://github.com/MUnique/OpenMU/tree/master/src/Startup): El
  proyecto para el ejecutable que junta todas las piezas del rompecabezas
