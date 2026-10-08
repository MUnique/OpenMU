// <copyright file="TypedContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MUnique.OpenMU.DataModel.Composition;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.EntityFramework.TypedContexts;

/// <summary>
/// A context which is used to show and edit instances of a specific type.
/// This context does not track and save data of other types, except direct object references which are marked with an <see cref="MemberOfAggregateAttribute" />.
/// </summary>
internal class TypedContext : EntityDataContext, ITypedContext
{
    private static readonly ConcurrentDictionary<Type, ContextInfo> ContextInfoPerEditType = new();

    // ReSharper disable once StaticMemberInGenericType That's okay. We don't need the behavior, but introducing a base class is just too much boilerplate.
    private static readonly IReadOnlyDictionary<Type, Type[]> AdditionalTypes = new Dictionary<Type, Type[]>
    {
        { typeof(GameServerDefinition), [typeof(GameServerConfiguration)] },
        { typeof(GameServerEndpoint), [typeof(GameClientDefinition)] },
        { typeof(ConnectServerDefinition), [typeof(GameClientDefinition)] },
        { typeof(DuelArea), [typeof(GameMapDefinition)] },
    };

    /// <summary>
    /// The factories of the contexts with a compiled model, per edit type.
    /// </summary>
    /// <remarks>
    /// These are the edit types which are used by the servers, e.g. by the game logic, outside of the admin panel.
    /// For them, building the model at runtime would delay their first use by several hundred milliseconds each.
    /// See CompiledModels/Readme.md about how to add another one.
    /// </remarks>
    private static readonly IReadOnlyDictionary<Type, Func<TypedContext>> ContextsWithCompiledModel = new Dictionary<Type, Func<TypedContext>>
    {
        { typeof(DataModel.Entities.CastleSiegeData), () => new CastleSiegeDataTypedContext() },
        { typeof(DataModel.Entities.CastleSiegeGuildRegistration), () => new CastleSiegeGuildRegistrationTypedContext() },
        { typeof(DataModel.Entities.CastleSiegePendingReward), () => new CastleSiegePendingRewardTypedContext() },
        { typeof(DataModel.Entities.GensAbuse), () => new GensAbuseTypedContext() },
        { typeof(DataModel.Entities.GensMember), () => new GensMemberTypedContext() },
        { typeof(DataModel.Statistics.MiniGameRankingEntry), () => new MiniGameRankingEntryTypedContext() },
        { typeof(PlugIns.PlugInConfiguration), () => new PlugInConfigurationTypedContext() },
        { typeof(SystemConfiguration), () => new SystemConfigurationTypedContext() },
    };

    private IEntityType? _rootType;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedContext"/> class.
    /// </summary>
    /// <param name="editType">Type of the edit.</param>
    public TypedContext(Type editType)
    {
        this.EditType = editType;

        this.SavingChanges += this.OnSavingChanges;
    }

    /// <inheritdoc/>
    public IEntityType RootType => this._rootType ??= this.Model.GetEntityTypes().First(t => t.ClrType == this.EditType || t.ClrType.BaseType == this.EditType);

    /// <summary>
    /// Gets the type which is edited with this context.
    /// </summary>
    public Type EditType { get; }

    private IReadOnlySet<Type> EditTypes => this.GetContextInfo().EditTypes;

    private IReadOnlySet<Type> BackReferenceTypes => this.GetContextInfo().BackReferenceTypes;

    private IReadOnlySet<Type> ReadOnlyTypes => this.GetContextInfo().ReadOnlyTypes;

    private string? GameConfigNavigationName => this.GetContextInfo().GameConfigNavigationName;

    /// <inheritdoc />
    public bool IsIncluded(Type clrType)
    {
        return this.EditTypes.Contains(clrType)
               || (clrType.BaseType is { } baseType && this.EditTypes.Contains(baseType));
    }

    /// <inheritdoc />
    public bool IsBackReference(Type type)
    {
        return this.BackReferenceTypes.Contains(type)
               || (type.BaseType is { } baseType && this.BackReferenceTypes.Contains(baseType));
    }

    /// <summary>
    /// Creates a new context for the specified edit type.
    /// </summary>
    /// <param name="editType">The edit type.</param>
    /// <returns>The new context, which uses a compiled model if one is available for the edit type.</returns>
    internal static TypedContext Create(Type editType)
    {
        return ContextsWithCompiledModel.TryGetValue(editType, out var factory)
            ? factory()
            : new TypedContext(editType);
    }

    /// <summary>
    /// Determines the information about the context of an edit type, based on the model of the <see cref="EntityDataContext"/>,
    /// which includes all entity types.
    /// </summary>
    /// <remarks>
    /// The information is determined independently of the model building, because a context with a compiled model
    /// doesn't build its model. The model of the <see cref="EntityDataContext"/> is the same as the one
    /// which is passed to <see cref="OnModelCreating"/> before the types are ignored.
    /// </remarks>
    /// <param name="completeModel">The model which includes all entity types.</param>
    /// <param name="editType">The edit type.</param>
    /// <returns>The information about the context.</returns>
    internal static ContextInfo DetermineContextInfo(IModel completeModel, Type editType)
    {
        var modelTypes = completeModel.GetEntityTypes().ToList();

        var editTypes = DetermineEditTypes(modelTypes, editType).DistinctBy(t => t.EntityType).ToList();
        var mainType = editTypes.FirstOrDefault().EntityType;

        var gameConfigType = modelTypes.FirstOrDefault(mt => mt.ClrType == typeof(EntityFramework.Model.GameConfiguration));
        var gameConfigNav = gameConfigType is null ? null : GetNavigationsInOrder(gameConfigType).FirstOrDefault(nav => nav.IsCollection && nav.TargetEntityType.ClrType == mainType);

        var additionalTypes = editTypes
            .Select(et => (et, entityType: modelTypes.FirstOrDefault(mt => mt.ClrType == et.EntityType)))
            .Where(t => t.entityType is not null)
            .SelectMany(t => DetermineAdditionalTypes(modelTypes.Select(m => m.ClrType), t.entityType!))
            .ToList();
        editTypes.AddRange(additionalTypes);

        var finalEditTypes = new HashSet<Type>();
        foreach (var type in editTypes)
        {
            var existingType = completeModel.FindEntityType(type.EntityType);
            if (existingType is not null)
            {
                finalEditTypes.Add(existingType.ClrType);
                if (existingType.ClrType.BaseType is { } baseType && baseType != typeof(object))
                {
                    finalEditTypes.Add(baseType);
                }
            }
        }

        if (gameConfigNav is not null)
        {
            finalEditTypes.Add(gameConfigNav.DeclaringEntityType.ClrType);
        }

        return new(
            finalEditTypes,
            editTypes.Where(t => t.IsBackReference).Select(t => t.EntityType).ToHashSet(),
            editTypes.Where(t => t.IsReadOnly).Select(t => t.EntityType).ToHashSet(),
            gameConfigNav?.Name);
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var editTypes = this.GetContextInfo().EditTypes;
        foreach (var type in modelBuilder.Model.GetEntityTypes().Where(t => !editTypes.Contains(t.ClrType)).ToList())
        {
            modelBuilder.Ignore(type.ClrType);
        }
    }

    private static IEnumerable<(Type EntityType, bool IsReadOnly, bool IsBackReference)> DetermineAdditionalTypes(IEnumerable<Type> modelTypes, IEntityType type)
    {
        if (!AdditionalTypes.TryGetValue(type.ClrType, out var additionalTypes)
            && !AdditionalTypes.TryGetValue(type.ClrType.BaseType!, out additionalTypes))
        {
            yield break;
        }

        foreach (var additionalType in additionalTypes)
        {
            var addEntityType = modelTypes.FirstOrDefault(met => met == additionalType)
                                ?? modelTypes.FirstOrDefault(met => met.BaseType == additionalType);

            if (addEntityType is not null)
            {
                yield return (addEntityType, true, false);
            }
        }
    }

    private static IEnumerable<(Type EntityType, bool IsReadOnly, bool IsBackReference)> DetermineNavigationTypes(IEntityType parentType)
    {
        var navigations = GetNavigationsInOrder(parentType)
            .Where(nav => nav.PropertyInfo is { });
        foreach (var navigation in navigations)
        {
            var type = navigation.TargetEntityType;

            if (navigation.IsMemberOfAggregate() || navigation.Name.StartsWith("Joined"))
            {
                yield return (type.ClrType, false, false);
            }
            else if (navigation.Inverse?.IsMemberOfAggregate() is true)
            {
                yield return (type.ClrType, false, true);
                if (type.ClrType.BaseType is { } baseType && baseType != typeof(object))
                {
                    yield return (baseType, false, true);
                }

                continue;
            }
            else
            {
                // We include the type, but don't go any deeper
                yield return (type.ClrType, true, false);
                continue;
            }

            foreach (var navEditType in DetermineNavigationTypes(type))
            {
                yield return navEditType;
            }
        }
    }

    /// <summary>
    /// Gets the navigations of the entity type, ordered by their name.
    /// </summary>
    /// <remarks>
    /// The edit types are de-duplicated by keeping the first occurrence, so the order matters. A model which is built
    /// at runtime orders the navigations ordinally by name, a compiled model doesn't. So we order them explicitly.
    /// </remarks>
    private static IEnumerable<INavigation> GetNavigationsInOrder(IEntityType entityType)
    {
        return entityType.GetNavigations().OrderBy(nav => nav.Name, StringComparer.Ordinal);
    }

    private static IEnumerable<(Type EntityType, bool IsReadOnly, bool IsBackReference)> DetermineEditTypes(IList<IEntityType> modelTypes, Type editType)
    {
        var mainType = modelTypes.FirstOrDefault(met => met.ClrType == editType)
                       ?? modelTypes.FirstOrDefault(met => met.ClrType.BaseType == editType);
        if (mainType is null)
        {
            yield break;
        }

        yield return (mainType.ClrType, false, false);

        foreach (var navType in DetermineNavigationTypes(mainType))
        {
            yield return navType;
        }
    }

    private ContextInfo GetContextInfo()
    {
        return ContextInfoPerEditType.GetOrAdd(this.EditType, static editType => DetermineContextInfo(CompleteModel, editType));
    }

    /// <summary>
    /// Called when the changes are about to get saved.
    /// We use this event to prevent saving of read-only types and to add the edited types to the current game configuration.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="SavingChangesEventArgs"/> instance containing the event data.</param>
    private void OnSavingChanges(object? sender, SavingChangesEventArgs e)
    {
        var gameConfigNavigation = this.Model.FindEntityType(typeof(EntityFramework.Model.GameConfiguration))
            ?.GetNavigations()
            .FirstOrDefault(nav => nav.Name == this.GameConfigNavigationName);

        var readOnlyTypes = this.ReadOnlyTypes;

        var fkProperty = gameConfigNavigation?.ForeignKey?.Properties.FirstOrDefault();

        foreach (var entry in this.ChangeTracker.Entries().ToList())
        {
            // Prevent accidental saves of read-only reference types.
            if ((entry.State == EntityState.Added || entry.State == EntityState.Modified)
                && readOnlyTypes.Contains(entry.Entity.GetType()))
            {
                entry.State = EntityState.Unchanged;
            }

            if (entry.State == EntityState.Added
                && fkProperty is not null
                && this.CurrentGameConfiguration is { } currentGameConfiguration
                && entry.Entity.GetType().IsAssignableTo(this.EditType))
            {
                // Set the FK value directly so EF writes the correct GameConfigurationId
                // without needing to Attach currentGameConfiguration (which would traverse
                // its entire graph and conflict with already-tracked read-only instances).
                var fkPropertyEntry = entry.Properties.FirstOrDefault(p => p.Metadata == fkProperty);
                if (fkPropertyEntry is not null)
                {
                    fkPropertyEntry.CurrentValue = currentGameConfiguration.GetId();
                }
            }
        }
    }

    /// <summary>
    /// Information about the context of an edit type.
    /// </summary>
    /// <param name="EditTypes">The types which are included in the model of the context.</param>
    /// <param name="BackReferenceTypes">The types which are referenced back by a member of the aggregate.</param>
    /// <param name="ReadOnlyTypes">The types which are referenced, but not saved.</param>
    /// <param name="GameConfigNavigationName">The name of the navigation of the game configuration to the edit type.</param>
    internal record ContextInfo(IReadOnlySet<Type> EditTypes, IReadOnlySet<Type> BackReferenceTypes, IReadOnlySet<Type> ReadOnlyTypes, string? GameConfigNavigationName);
}
