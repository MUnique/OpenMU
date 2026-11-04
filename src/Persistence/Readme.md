# MUnique.OpenMU.Persistence

This project contains the main interface of the data persistence layer, [**IPersistenceContextProvider**](IPersistenceContextProvider.cs).
All application logic just uses the "contexts" provided by the PersistentContextProvider.

A context basically provides the functionality to load, create and modify data.
The creation of a new object needs to be requested from a context.
That's because the context might want to track the changes, but also we're
creating persistence-specific classes (unknown to the application) which are
derived from the DataModel (e.g. subfolder *EntityFramework/Model*).

There are currently two implementations:

1. *MUnique.OpenMU.Persistence.EntityFramework*: Implements it by using the
   Entity Framework Core.

2. *MUnique.OpenMU.Persistence.InMemory*: For testing and demo purposes. Changes
   are directly "persistent" to all contexts of the same IPersistenceContextProvider.
   All application logic uses the contexts provided by the InMemoryPersistentContextProvider,
   until the application closes, of course.

This projects also contains some base classes for a repository pattern - these
are however only implementation details and don't necessarily need to be used
to implement the persistence.