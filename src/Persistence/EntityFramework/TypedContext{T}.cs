// <copyright file="TypedContext{T}.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Composition;

/// <summary>
/// A context which is used to show and edit instances of <typeparamref name="T" />.
/// This context does not track and save data of other types, except direct object references which are marked with an <see cref="MemberOfAggregateAttribute" />.
/// </summary>
/// <typeparam name="T">The type which should be edited.</typeparam>
internal class TypedContext<T> : EntityDataContext, ITypedContext
{
    // ReSharper disable once StaticMemberInGenericType That's okay. We don't need the behavior, but introducing a base class is just too much boilerplate.
    private static readonly IReadOnlyDictionary<Type, Type[]> AdditionalTypes = new Dictionary<Type, Type[]>
    {
        { typeof(GameServerDefinition), new[] { typeof(GameServerConfiguration) } },
        { typeof(GameServerEndpoint), new[] { typeof(GameClientDefinition) } },
        { typeof(ConnectServerDefinition), new[] { typeof(GameClientDefinition) } },
    };

    // ReSharper disable once StaticMemberInGenericType That's okay, we want this behavior (each type context with it's own set)
    private static ISet<Type> EditTypes { get; } = new HashSet<Type>();

    // ReSharper disable once StaticMemberInGenericType That's okay, we want this behavior (each type context with it's own set)
    private static ISet<Type> BackReferenceTypes { get; } = new HashSet<Type>();

    private IEntityType? _rootType;

    /// <inheritdoc/>
    public IEntityType RootType => this._rootType ??= this.Model.GetEntityTypes().First(t => t.ClrType.BaseType == typeof(T));

    /// <inheritdoc />
    public bool IsIncluded(Type clrType)
    {
        return EditTypes.Contains(clrType)
               || (clrType.BaseType is { } baseType && EditTypes.Contains(baseType));
    }

    /// <inheritdoc />
    public bool IsBackReference(Type type)
    {
        return BackReferenceTypes.Contains(type)
               || (type.BaseType is { } baseType && BackReferenceTypes.Contains(baseType));
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var modelTypes = modelBuilder.Model.GetEntityTypes().ToList();

        var editTypes = DetermineEditTypes(modelTypes).ToList();
        var additionalTypes = editTypes.SelectMany(t => DetermineAdditionalTypes(modelTypes, t)).ToList();
        editTypes.AddRange(additionalTypes);
        foreach (var type in editTypes)
        {
            EditTypes.Add(type.ClrType);
            if (type.ClrType.BaseType is { } baseType && baseType != typeof(object))
            {
                EditTypes.Add(baseType);
            }

            modelBuilder.Entity(type.ClrType);
        }

        foreach (var type in modelTypes.Except(editTypes))
        {
            modelBuilder.Ignore(type.ClrType);
        }
    }

    private static IEnumerable<IMutableEntityType> DetermineEditTypes(IList<IMutableEntityType> modelTypes)
    {
        var mainType = modelTypes.FirstOrDefault(met => met.ClrType == typeof(T))
                       ?? modelTypes.FirstOrDefault(met => met.ClrType.BaseType == typeof(T));
        if (mainType is null)
        {
            yield break;
        }

        yield return mainType;

        foreach (var navType in DetermineNavigationTypes(mainType))
        {
            yield return navType;
        }
    }

    private static IEnumerable<IMutableEntityType> DetermineAdditionalTypes(IList<IMutableEntityType> modelTypes, IMutableEntityType type)
    {
        if (!AdditionalTypes.TryGetValue(type.ClrType, out var additionalTypes)
            && !AdditionalTypes.TryGetValue(type.ClrType.BaseType!, out additionalTypes))
        {
            yield break;
        }

        foreach (var additionalType in additionalTypes)
        {
            var addEntityType = modelTypes.FirstOrDefault(met => met.ClrType == additionalType)
                                ?? modelTypes.FirstOrDefault(met => met.ClrType.BaseType == additionalType);
            if (addEntityType is not null)
            {
                yield return addEntityType;
            }
        }
    }

    private static IEnumerable<IMutableEntityType> DetermineNavigationTypes(IMutableEntityType parentType)
    {
        var navigations = parentType
            .GetNavigations()
            .Where(nav => nav.PropertyInfo is { });
        foreach (var navigation in navigations)
        {
            var type = navigation.TargetEntityType;

            if (navigation.IsMemberOfAggregate() || navigation.Name.StartsWith("Joined"))
            {
                yield return type;
            }
            else if (navigation.Inverse?.IsMemberOfAggregate() is true)
            {
                BackReferenceTypes.Add(type.ClrType);
                if (type.ClrType.BaseType is { } baseType && baseType != typeof(object))
                {
                    BackReferenceTypes.Add(baseType);
                }

                yield return type;
                continue;
            }
            else
            {
                continue;
            }

            foreach (var navEditType in DetermineNavigationTypes(type))
            {
                yield return navEditType;
            }
        }
    }
}