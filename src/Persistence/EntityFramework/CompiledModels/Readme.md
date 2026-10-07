# Compiled models

This folder contains the [compiled models](https://learn.microsoft.com/ef/core/performance/advanced-performance-topics#compiled-models)
of the entity framework core contexts. With them, the models don't need to be built
at runtime, which saves several seconds when the server starts.

| Context | Compiled model |
|---|---|
| `EntityDataContext` | `EntityDataContext` |
| `ConfigurationContext` | uses the one of the `EntityDataContext`, because the model is the same |
| `AccountContext` | `AccountContext` |
| `TradeContext` | `TradeContext` |
| `TypedContext` for the edit types which are used by the servers | `<EditType>TypedContext`, see below |

The other contexts build their models at runtime: the models of the `GuildContext` and
`FriendContext` are small.

### Typed contexts

The `TypedContext` builds one model per edited type. For the edit types which are used by
the servers outside of the admin panel (e.g. by the castle siege, gens and mini games),
there is a subclass with a parameterless constructor in the folder `TypedContexts`, e.g.
`CastleSiegeDataTypedContext`, which has a compiled model. `TypedContext.Create` returns
such a subclass for these edit types. The other edit types, e.g. the ones of the admin panel,
still build their models at runtime.

To add a compiled model for another edit type:

1. Add a subclass in the folder `TypedContexts`, like the existing ones.
2. Add it to `ContextsWithCompiledModel` in `TypedContext`.
3. Generate its compiled model (see below) and add it to the `CompiledModelTests`.

## When to generate them again

Whenever the model changes, i.e. when the data model or the `OnModelCreating` methods of the
contexts change. That's usually also the case when a migration is added.

The `CompiledModelTests` in `tests/MUnique.OpenMU.Tests` fail when a compiled model doesn't
match the model which is built by `OnModelCreating`.

## How to generate them

Use the project `MUnique.OpenMU.Persistence.EntityFramework.DesignTime` as startup project.
It contains design time services which make sure that the generated code references the
types of OpenMU by their full name. Without it, the generated code doesn't compile, because
the entity types of this project have the same names as their base types in the data model.

With the .NET CLI and the [dotnet-ef tool](https://learn.microsoft.com/ef/core/cli/dotnet),
in the folder `src/Persistence/EntityFramework`:

```
dotnet build ../EntityFramework.DesignTime -c Release -p:ci=true
dotnet ef dbcontext optimize --no-build --configuration Release --project . --startup-project ../EntityFramework.DesignTime --context EntityDataContext --output-dir CompiledModels/EntityDataContext --namespace MUnique.OpenMU.Persistence.EntityFramework.CompiledModels.ForEntityDataContext
dotnet ef dbcontext optimize --no-build --configuration Release --project . --startup-project ../EntityFramework.DesignTime --context AccountContext --output-dir CompiledModels/AccountContext --namespace MUnique.OpenMU.Persistence.EntityFramework.CompiledModels.ForAccountContext
dotnet ef dbcontext optimize --no-build --configuration Release --project . --startup-project ../EntityFramework.DesignTime --context TradeContext --output-dir CompiledModels/TradeContext --namespace MUnique.OpenMU.Persistence.EntityFramework.CompiledModels.ForTradeContext
```

For each typed context, e.g. `CastleSiegeDataTypedContext`:

```
dotnet ef dbcontext optimize --no-build --configuration Release --project . --startup-project ../EntityFramework.DesignTime --context CastleSiegeDataTypedContext --output-dir CompiledModels/CastleSiegeDataTypedContext --namespace MUnique.OpenMU.Persistence.EntityFramework.CompiledModels.ForCastleSiegeDataTypedContext
```

In the Package Manager Console of Visual Studio, select *MUnique.OpenMU.Persistence.EntityFramework*
as default project and run:

```
Optimize-DbContext -Context EntityDataContext -StartupProject MUnique.OpenMU.Persistence.EntityFramework.DesignTime -OutputDir CompiledModels/EntityDataContext -Namespace MUnique.OpenMU.Persistence.EntityFramework.CompiledModels.ForEntityDataContext
Optimize-DbContext -Context AccountContext -StartupProject MUnique.OpenMU.Persistence.EntityFramework.DesignTime -OutputDir CompiledModels/AccountContext -Namespace MUnique.OpenMU.Persistence.EntityFramework.CompiledModels.ForAccountContext
Optimize-DbContext -Context TradeContext -StartupProject MUnique.OpenMU.Persistence.EntityFramework.DesignTime -OutputDir CompiledModels/TradeContext -Namespace MUnique.OpenMU.Persistence.EntityFramework.CompiledModels.ForTradeContext
Optimize-DbContext -Context CastleSiegeDataTypedContext -StartupProject MUnique.OpenMU.Persistence.EntityFramework.DesignTime -OutputDir CompiledModels/CastleSiegeDataTypedContext -Namespace MUnique.OpenMU.Persistence.EntityFramework.CompiledModels.ForCastleSiegeDataTypedContext
```

and the same for the other typed contexts in the folder `TypedContexts`.

Files of entity types which were removed from the model are not deleted by the tools.
With the .NET CLI, delete the subfolders of the compiled models after the build and before
generating them. Don't delete them before the build: the `ConfigurationContext` references the
compiled model of the `EntityDataContext`, so the project doesn't compile without it.
`Optimize-DbContext` builds the project itself, so in the Package Manager Console, delete
the files of removed entity types afterwards.

The namespaces start with `For`, because a namespace which is named like the context class,
or like an entity type, would hide these types in the generated code.
