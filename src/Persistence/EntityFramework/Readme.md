# Persistence by Entity Framework Core

This project implements the persistence layer by using the Entity Framework Core.
The database should be free to choose, but we test with PostgreSQL and it might
be possible that the migrations are only working on PostgreSQL, too.

## Entity Model

It's important to note, that the object model of MUnique.OpenMU.DataModel is not
directly used. Instead, every class of the model gets inherited to be used with
EF-Core.
We did this because the application code should not be aware of the boilerplate
code of all of the persistence aspects.
Of course, this is not done manually, but with the help of automatic code generation.
As some critical features are still missing by EF-Core, we can easily add some
workarounds to the whole data model here.

The code generation adds the following stuff to the inherited entity classes:

* Object Identifier, "Id"-Property as GUID

* Overriding of Equals and GetHashCode, based on the Id-Property

* Foreign Key Id-Properties for navigation properties

* Because every type is different, it has do to the following stuff for
    navigation properties:

  * Overrides every property which uses the base type, adds a NotMappedAttribute
    and sets the foreign key property value in the setter.

  * Adds new properties which use the inherited type and maps it to the
    Foreign Key Id-Property. Getter and Setter is accessing the base property.

* For the same reason, (1:n) collection properties need special handling:

  * It adds new "Raw" properites which are ICollections of the inherited classes

  * It initializes the base collection properties with [1:n collection adapters](../CollectionAdapter.cs)
    which adapt between base and inherited classes, accessing the "Raw" collection.

* For the n:m-collection properties (which are a bit tricky to get detected as such):
  * Additional join entity classes are created
  * Collection properties for these join entities are added
  * [Many-to-many collection adapters](ManyToManyCollectionAdapter.cs) are used

* It adds a schema name ("config" or "data") to the TableAttribute

* It creates a new [DbContext](Model/ExtendedTypeContext.Generated.cs) which
  defines Ignores for all base types and adds join definitions for all 1:n
  and n:m relationships.

If you're interested of how the result looks like, have a look at the subfolder *Model*.

## Schemas

To keep access to game configuration data restricted for account contexts
(= connected clients), we put configuration and account data into separate schemas.
There might be additional schemas and users for the friend and guild servers.

### Configuration

The different contexts (identified by their full class name) can use different
database users. These are configured in the [ConnectionSettings.xml](ConnectionSettings.xml)
file.

During install, only the user for the MUnique.OpenMU.Persistence.EntityFramework.EntityDataContext
should exist, the other user and their rights will be created by this user.
So this user should have the required rights to grant this rights.

## Repository Pattern

All application logic uses the contexts provided by the [PersistenceContextProvider](PersistenceContextProvider.cs).
The provided contexts load their data not directly from ef core contexts,
but access repositories. That's because we want to eagerly load objects which
means we want to retrieve the complete object graph with all dependent data,
when we access our "Contexts".
We're also caching configuration data with this approach, by using so called [ConfigurationTypeRepositories](ConfigurationTypeRepository.cs).

Each entity type has basically one repository, some are manually implemented,
some are generic. There is a [RepositoryProvider](RepositoryProvider.cs) which
holds and "provides" all of these repositories.
The implementation of this pattern is probably not the same as all of the examples
of what you'll find at the internet. Workarounds and different requirements result
in different implementations ;-)

The [CacheAwareRepositoryProvider](CacheAwareRepositoryProvider.cs) holds two other
repository providers. One which provides repositories which actually load the data
from the database, and another one which returns repositories with cached data,
based on the loaded GameConfiguration. The CacheAwareRepositoryProvider first tries
to retrieve a repository for the cached data. If none is found, it takes the other.

### Loading whole object graphs

As the application code expects a fully loaded object when loading through the
contexts, we have to load objects as a whole.
It might also be possible to lazy-load dependent data, but this can lead to bad
performance here. As we know that we actually need the whole data in our use cases,
it makes sense to fully load the objects up-front.

For example, when we want to load a game configuration, we want to load all collection
and navigation properties and cache them for later accesses.
That seems slow at the start of the server, but it's an advantage when loading
accounts which reference a lot of configuration data later.
When loading accounts, they can be used to resolve configuration navigation
properties of the player's data.
Additionally, with such an access pattern it should be a no brainer to accellerate
data access by implementing persistence for a NoSQL document database, too.
Here we would use one document for each Account, keeping the game configuration
in another document.

All this happens in the [GenericRepository of T](GenericRepository.cs).
To load all whole object graphs, the repositories are iterating through the
navigation and collection properties of the loaded object.
If they are not loaded yet (specified by each EntityEntry), it loads them by
accessing the repository of the property type.
This is also the reason why every type needs it's own repository, otherwise it
wouldn't work.

#### Loading full objects by json queries

The repositories of Accounts and GameConfiguration objects load their objects
(and all dependencies) in one go - by using some JSON functions of PostgreSQL.
You can read about it
[here](https://munique.net/loading-complex-data-with-postgresql-json-functions/)
and [here](https://github.com/MUnique/OpenMU/issues/10).

### Context management

The concept of a context is probably most common when working with
EntityFramework ORMs.

The application code does not directly use DBContext of the entity framework.
We have an abstraction by using an interface IContext and some inherited ones
for different use-cases. The [base implementation](EntityFrameworkContext.cs)
holds the DBContext which is used by the repositories.
Other ORMs do have similar concepts (like NHibernate sessions) and we try to keep
the context abstraction compatible to such use cases.

In case of this server, it gives each connected account one account context.
All of the changes of one account are tracked in this context.
When we want to save an account we just call SaveChanges on this context.
Sounds easy, right? :)

However, it gets difficult when more than one account is involved by an entity,
e.g. Items which are changing the owner to another account, by trade or drop/pickup.
When doing a trade, the items of both trading players are attached to a new *trade context*,
which saves the changes on trade completion.
After a trade finished, the traded items are attached to their corresponding contexts
of their players.

With this abstraction of a context we can also use other ORMs such as NHibernate
or NoSQL databases which save documents with just one call - we would save and
commit the corresponding data of such sessions at the SaveChanges method.