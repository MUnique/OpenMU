# Persistence by Entity Framework Core

This project implements the persistence layer by using the Entity Framework Core. The database should be free to choose, but we test with PostgreSQL.

## Entity Model

It's important to note, that the object model of MUnique.OpenMU.DataModel is not directly used. Instead, every class of the model gets inherited to be used with EF-Core.
We did this because the application code should not be aware of the boilerplate code of all of the persistence aspects.
Of course, this is not done manually, but with the help of T4-Templates. As some critical features are still missing by EF-Core, we can easily add some workarounds to the whole data model here.

The T4-Templates add the following stuff to the inherited entity classes:

  * Object Identifier, "Id"-Property as GUID
  * Overriding of Equals and GetHashCode, based on the Id-Property
  * Foreign Key Id-Properties for navigation properties
  * Because every type is different, it has do to the following stuff for navigation properties:
    * Overrides every property which uses the base type, adds a NotMappedAttribute and sets the foreign key property value in the setter.
    * Adds new properties which use the inherited type and maps it to the Foreign Key Id-Property. Getter and Setter is accessing the base property.
  * For the same reason, (1:n) collection properties need special handling:
    * It adds new "Raw" properites which are ICollections of the inherited classes
    * It initializes the base collection properties with [1:n collection adapters](CollectionAdapter.cs) which adapt between base and inherited classes, accessing the "Raw" collection.
  * For the n:m-collection properties (which are a bit tricky to get detected as such):
    * Additional join entity classes are created
    * Collection properties for these join entities are added
    * [Many-to-many collection adapters](ManyToManyCollectionAdapter.cs) are used
  * It adds a schema name ("config" or "data") to the TableAttribute
  * It creates a new DbContext ("ExtendedTypeContext") which defines Ignores for all base types and adds join definitions for all 1:n and n:m relationships.

If you're interested of how the result looks like, have a look at [ExtendedTypes.cs](ExtendedTypes.cs) and [ExtendedJoinEntities.cs](ExtendedJoinEntities.cs).

## Schemas

To keep access to game configuration data restricted for account contexts (= connected clients), we put configuration and account data into separate schemas.

### Configuration

The different contexts (identified by their full class name) can use different database users. These are configured in the [ConnectionSettings.xml](ConnectionSettings.xml) file.

During install, only the user for the MUnique.OpenMU.Persistence.EntityFramework.EntityDataContext should exist, the other user and their rights will be created by this user.
So this user should have the required rights to grant this rights.

## Repository Pattern

We decided to use a repository pattern to keep the application code independent of a specific ORM or database. The application itself uses just a tiny amount of these repositories (most likely AccountRepository and GameConfigurationRepository).
However, each entity type has basically one repository, some are manually implemented, some are generic.
The implementation of this pattern is probably not the same as all of the examples of what you'll find at the internet. Workarounds and different requirements result in different implementations ;-)

There is a "RepositoryManager" which holds and "manages" all of these repositories.
Because the actual persistent type is not known to the application, it also offers a method to create new objects by giving it the known base type. Of course, using a dependency injection container could also be used at a later stage.

### Loading whole object graphs

We want to keep the usage of these repositories in the application code low, so we have to load objects as a whole.

For example, when we want to load a game configuration, we want to load all collection and navigation properties and cache them for later accesses.
That seems slow at the start of the server, but it's an advantage when loading accounts which reference a lot of configuration data later.
When loading accounts, they can be used to resolve configuration navigation properties of the player's data.
Additionally, with such an access pattern it should be a no brainer to accellerate data access by implementing persistence for a NoSQL document database, too.
Here we would use one document for each Account, keeping the game configuration in another document.

All this happens in the [GenericRepository<T>](GenericRepository{T}.cs).
To load all whole object graphs, the repositories are iterating through the navigation and collection properties of the loaded object.
If they are not loaded yet (specified by each EntityEntry), it loads them by accessing the repository of the property type.
This is also the reason why every type needs it's own repository, otherwise it wouldn't work.

### Context management
The concept of a context is probably most common when working with EntityFramework ORMs.

The application code does not directly use DBContext of the entity framework. We have an abstraction by using an interface IContext,
and an [implementation](EntityFrameworkContext.cs) which holds the DBContext. Other ORMs do have similar concepts 
(like NHibernate sessions) and we try to keep the context abstraction compatible to such usage cases.

In case of this server, it gives each connected account one account context. All of the changes of one account are tracked in this context.
When we want to save an account we just call SaveChanges on this context. Sounds easy, right? :)

However, it gets difficult when more than one account is involved by an entity, e.g. Items which are changing the owner to another account, 
by trade or drop/pickup. These are challenges which are still to solve.

With this abstraction of a context we can also use other ORMs such as NHibernate or NoSQL databases which save documents with just one call - 
we would save and commit the corresponding account of such sessions at the SaveChanges method.

#### Creating new entity instances
Because the creation of a new object needs to be done on a context (for change tracking etc.), there needs to be a context to which the item is added.
We use some kind of a thread-specific stack, which means that if you want to create a new entity, a context needs to be on the stack. To put one on to the stack, there is the function UseContext at the RepositoryManager.
The usage is like the following:
```csharp
Account CreateAndSaveNewAccount(IRepositoryManager repositoryManager, GameConfiguration gameConfiguration)
{
  var accountContext = repositoryManager.CreateNewAccountContext(gameConfiguration);
  using (repositoryManager.UseContext(accountContext))
  {
    var account = repositoryManager.CreateNew<Account>(); // requires that a context is on the stack on the same thread
    // set some values...
  }

  context.SaveChanges(); // can be outside of using

  return account;
}
```