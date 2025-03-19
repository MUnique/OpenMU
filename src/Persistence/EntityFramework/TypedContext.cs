// <copyright file="TypedContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MUnique.OpenMU.DataModel.Composition;
using MUnique.OpenMU.DataModel.Configuration;

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
    public IEntityType RootType => this._rootType ??= this.Model.GetEntityTypes().First(t => t.ClrType.BaseType == this.EditType);

    /// <summary>
    /// Gets the type which is edited with this context.
    /// </summary>
    public Type EditType { get; }

    private IReadOnlySet<Type> EditTypes => ContextInfoPerEditType[this.EditType].EditTypes;

    private IReadOnlySet<Type> BackReferenceTypes => ContextInfoPerEditType[this.EditType].BackReferenceTypes;

    private IReadOnlySet<Type> ReadOnlyTypes => ContextInfoPerEditType[this.EditType].ReadOnlyTypes;

    private string? GameConfigNavigationName => ContextInfoPerEditType[this.EditType].GameConfigNavigationName;

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

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var modelTypes = modelBuilder.Model.GetEntityTypes().ToList();

        var editTypes = this.DetermineEditTypes(modelTypes).DistinctBy(t => t.EntityType).ToList();
        var mainType = editTypes.FirstOrDefault().EntityType;

        var gameConfigType = modelTypes.FirstOrDefault(mt => mt.ClrType == typeof(EntityFramework.Model.GameConfiguration));
        var gameConfigNav = gameConfigType?.GetNavigations().FirstOrDefault(nav => nav.IsCollection && nav.TargetEntityType.ClrType == mainType);

        var additionalTypes = editTypes
            .SelectMany(et => DetermineAdditionalTypes(
                modelTypes.Select(t => t.ClrType),
                modelTypes.First(mt => mt.ClrType == et.EntityType)))
            .ToList();
        editTypes.AddRange(additionalTypes);
        var finalEditTypes = new HashSet<Type>();
        foreach (var type in editTypes)
        {
            finalEditTypes.Add(type.EntityType);
            if (type.EntityType.BaseType is { } baseType && baseType != typeof(object))
            {
                finalEditTypes.Add(baseType);
            }

            modelBuilder.Entity(type.EntityType);
        }

        if (gameConfigNav is not null)
        {
            finalEditTypes.Add(gameConfigNav.DeclaringEntityType.ClrType);
        }

        foreach (var type in modelTypes.Where(t => !finalEditTypes.Contains(t.ClrType)))
        {
            modelBuilder.Ignore(type.ClrType);
        }

        ContextInfoPerEditType.TryAdd(
            this.EditType,
            new(
                finalEditTypes,
                editTypes.Where(t => t.IsBackReference).Select(t => t.EntityType).ToHashSet(),
                editTypes.Where(t => t.IsReadOnly).Select(t => t.EntityType).ToHashSet(),
                gameConfigNav?.Name));
    }

    private static IEnumerable<(Type EntityType, bool IsReadOnly, bool IsBackReference)> DetermineAdditionalTypes(IEnumerable<Type> modelTypes, IMutableEntityType type)
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

    private static IEnumerable<(Type EntityType, bool IsReadOnly, bool IsBackReference)> DetermineNavigationTypes(IMutableEntityType parentType)
    {
        var navigations = parentType
            .GetNavigations()
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

    private IEnumerable<(Type EntityType, bool IsReadOnly, bool IsBackReference)> DetermineEditTypes(IList<IMutableEntityType> modelTypes)
    {
        var mainType = modelTypes.FirstOrDefault(met => met.ClrType == this.EditType)
                       ?? modelTypes.FirstOrDefault(met => met.ClrType.BaseType == this.EditType);
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

    /// <summary>
    /// Called when the changes are about to get saved.
    /// We use this event to prevent saving of read-only types and to add the edited types to the current game configuration.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="SavingChangesEventArgs"/> instance containing the event data.</param>
    private void OnSavingChanges(object? sender, SavingChangesEventArgs e)
    {
        var readOnlyTypes = this.ReadOnlyTypes;
        var gameConfigNavigation = this.Model.FindEntityType(typeof(EntityFramework.Model.GameConfiguration))?.GetNavigations().FirstOrDefault(nav => nav.Name == this.GameConfigNavigationName);
        this.ChangeTracker.DetectChanges();
        foreach (var entry in this.ChangeTracker.Entries().ToList())
        {
            if ((entry.State == EntityState.Added || entry.State == EntityState.Modified)
                && readOnlyTypes.Contains(entry.Entity.GetType()))
            {
                entry.State = EntityState.Unchanged;
            }

            if (entry.State == EntityState.Added
                && this.CurrentGameConfiguration is { } currentGameConfiguration
                && gameConfigNavigation is { }
                && entry.Entity.GetType().IsAssignableTo(this.EditType))
            {
                this.Attach(currentGameConfiguration);
                gameConfigNavigation.GetCollectionAccessor()?.Add(currentGameConfiguration, entry.Entity, false);
            }
        }
    }

    private record ContextInfo(IReadOnlySet<Type> EditTypes, IReadOnlySet<Type> BackReferenceTypes, IReadOnlySet<Type> ReadOnlyTypes, string? GameConfigNavigationName);
}