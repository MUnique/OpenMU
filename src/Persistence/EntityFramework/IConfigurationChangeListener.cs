// <copyright file="IConfigurationChangeListener.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.EntityFrameworkCore.Metadata;

/// <summary>
/// Interface for an object which listens to configuration changes.
/// </summary>
public interface IConfigurationChangeListener
{
    /// <summary>
    /// A configuration has changed.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="configuration">The changed configuration.</param>
    /// <param name="parent">The parent object, if available.</param>
    ValueTask ConfigurationChangedAsync(Type type, Guid id, object configuration, object? parent);

    /// <summary>
    /// A configuration has been added.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="configuration">The added configuration.</param>
    /// <param name="parent">The parent object, if available.</param>
    /// <param name="parentCollectionNavigation">The parent collection navigation, if available.</param>
    ValueTask ConfigurationAddedAsync(Type type, Guid id, object configuration, object? parent, INavigationBase? parentCollectionNavigation);

    /// <summary>
    /// A configuration has been removed.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="id">The identifier of the removed configuration object.</param>
    /// <param name="parent">The parent, if available.</param>
    /// <param name="parentCollectionNavigation">The parent collection navigation, if available.</param>
    ValueTask ConfigurationRemovedAsync(Type type, Guid id, object? parent, INavigationBase? parentCollectionNavigation);
}