// <copyright file="ModelExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MUnique.OpenMU.DataModel.Composition;

/// <summary>
/// Extensions for <see cref="INavigation"/>s.
/// </summary>
public static class ModelExtensions
{
    /// <summary>
    /// Determines whether the specified navigation is a member of the aggregate.
    /// </summary>
    /// <param name="navigation">The navigation.</param>
    /// <returns>
    ///   <c>true</c> if the specified navigation is a member of the aggregate; otherwise, <c>false</c>.
    /// </returns>
    internal static bool IsMemberOfAggregate(this IReadOnlyNavigationBase navigation)
    {
        var propertyInfo = navigation.PropertyInfo;
        if (propertyInfo?.Name.StartsWith("Raw") ?? false)
        {
            propertyInfo = propertyInfo.DeclaringType?.GetProperty(propertyInfo.Name.Substring(3), BindingFlags.Instance | BindingFlags.Public);
        }

        return propertyInfo?.GetCustomAttribute<MemberOfAggregateAttribute>() is { };
    }

    /// <summary>
    /// Gets the value of a navigation property of an entity.
    /// </summary>
    /// <param name="navigation">The navigation.</param>
    /// <param name="entity">The entity.</param>
    /// <returns>The value of the navigation property.</returns>
    internal static object? GetClrValue(this IReadOnlyNavigation navigation, object entity)
    {
        return navigation.GetClrValue<object>(entity);
    }

    /// <summary>
    /// Gets the value of a navigation property of an entity.
    /// </summary>
    /// <param name="navigation">The navigation.</param>
    /// <param name="entity">The entity.</param>
    /// <returns>The value of the navigation property.</returns>
    /// <typeparam name="T">The expected type of the result.</typeparam>
    internal static T? GetClrValue<T>(this IReadOnlyNavigation navigation, object entity)
        where T : class
    {
        return navigation.PropertyInfo?.GetMethod?.Invoke(entity, []) as T;
    }

    /// <summary>
    /// Gets the value of a navigation property of an entity.
    /// </summary>
    /// <param name="navigation">The navigation.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="value">The value which should be set at the navigation property.</param>
    /// <returns>The success of the operation. Fails, when there is no available setter.</returns>
    internal static bool TrySetClrValue(this IReadOnlyNavigation navigation, object entity, object? value)
    {
        if (navigation.PropertyInfo?.SetMethod is { } setMethod)
        {
            setMethod.Invoke(entity, new[] { value });
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the column name of the declared primary key.
    /// When you have a 1:n-relationship, it's the primary key of the '1', where <paramref name="entityType"/> is the 'n'.
    /// Only works for single-column primary keys.
    /// </summary>
    /// <param name="entityType">The entity type of 'n' in a 1:n-relationship.</param>
    /// <returns>The column name of the declared primary key.</returns>
    internal static string GetDeclaredPrimaryKeyColumnName(this IEntityType entityType)
    {
#pragma warning disable EF1001 // Internal EF Core API usage.
        if (entityType.FindDeclaredPrimaryKey() is not { } primaryKey)
#pragma warning restore EF1001 // Internal EF Core API usage.
        {
            throw new ArgumentException($"Entity type {entityType} has no declared primary key.");
        }

        return primaryKey.GetColumnName();
    }

    /// <summary>
    /// Gets the column name of the primary key.
    /// Only works for single-column primary keys.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <returns>The column name of the primary key.</returns>
    internal static string GetPrimaryKeyColumnName(this IEntityType entityType)
    {
        if (entityType.FindPrimaryKey() is not { } primaryKey)
        {
            throw new ArgumentException($"Entity type {entityType} has no primary key.");
        }

        return primaryKey.GetColumnName();
    }

    /// <summary>
    /// Gets the column name for a foreign key.
    /// Only works for single-column foreign key.
    /// </summary>
    /// <param name="foreignKey">The foreign key.</param>
    /// <returns>The column name for a foreign key.</returns>
    internal static string GetColumnName(this IReadOnlyForeignKey foreignKey)
    {
        if (foreignKey.Properties.Count > 1)
        {
            throw new ArgumentException("Can't return a column name for a composite foreign key.", nameof(foreignKey));
        }

        if (foreignKey.Properties.SingleOrDefault() is not { } keyProperty)
        {
            throw new ArgumentException($"Key {foreignKey} has no properties", nameof(foreignKey));
        }

        return keyProperty.GetColumnName();
    }

    /// <summary>
    /// Gets the column name for a key.
    /// Only works for single-column key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The column name for a key.</returns>
    internal static string GetColumnName(this IReadOnlyKey key)
    {
        if (key.Properties.Count > 1)
        {
            throw new ArgumentException("Can't return a column name for a composite primary key.", nameof(key));
        }

        if (key.Properties.SingleOrDefault() is not { } keyProperty)
        {
            throw new ArgumentException($"Key {key} has no properties", nameof(key));
        }

        return keyProperty.GetColumnName();
    }

    /// <summary>
    /// Gets the column name for a property.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns>The column name for a property.</returns>
    internal static string GetColumnName(this IReadOnlyProperty property)
    {
        if (StoreObjectIdentifier.Create(property.DeclaringEntityType, StoreObjectType.Table) is not { } keyIdentifier)
        {
            throw new InvalidOperationException($"Couldn't create a {nameof(StoreObjectIdentifier)} for declaring entity type {property.DeclaringEntityType} of property {property}.");
        }

        if (property.GetDefaultColumnName(keyIdentifier) is not { } columnName)
        {
            throw new InvalidOperationException($"Found no column name for {keyIdentifier}.");
        }

        return columnName;
    }
}