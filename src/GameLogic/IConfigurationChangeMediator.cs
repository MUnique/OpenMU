// <copyright file="IConfigurationChangeMediator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// A mediator which notifies about configuration changes for registered instances.
/// </summary>
public interface IConfigurationChangeMediator
{
    /// <summary>
    /// Registers for changes of a configuration object in addition with an actual game logic object.
    /// </summary>
    /// <typeparam name="TConfig">The type of the configuration.</typeparam>
    /// <typeparam name="T">The type of the game logic object which might be modified by the changes.</typeparam>
    /// <param name="config">The configuration object in which the caller is interested in.</param>
    /// <param name="obj">The game logic object which will be provided in the <paramref name="onChange"/> and <paramref name="onDelete"/> callbacks.</param>
    /// <param name="onChange">The on-change callback.</param>
    /// <param name="onDelete">The on-delete callback.</param>
    /// <returns>An <see cref="IDisposable"/> which is used to dispose the registration.</returns>
    IDisposable RegisterObject<TConfig, T>(TConfig config, T obj, Func<Action, TConfig, T, ValueTask>? onChange = null, Func<TConfig, T, ValueTask>? onDelete = null)
        where T : class
        where TConfig : class;

    /// <summary>
    /// Registers for created configuration objects of a specific type.
    /// </summary>
    /// <typeparam name="TConfig">The type of the configuration.</typeparam>
    /// <typeparam name="T">The type of the game logic object which might be modified by the changes.</typeparam>
    /// <param name="obj">The game logic object which will be provided in the <paramref name="onNewConfig"/> callback.</param>
    /// <param name="onNewConfig">The on-new-configuration callback.</param>
    /// <returns>An <see cref="IDisposable"/> which is used to dispose the registration.</returns>
    IDisposable RegisterForNew<TConfig, T>(T obj, Func<TConfig, T, ValueTask> onNewConfig);
}

/// <summary>
/// The listener interface for a mediator which notifies about configuration changes for registered instances.
/// </summary>
public interface IConfigurationChangeMediatorListener
{
    /// <summary>
    /// Handles the changed configuration.
    /// </summary>
    /// <param name="type">The type of the changed configuration object.</param>
    /// <param name="id">The identifier of the changed configuration object.</param>
    /// <param name="configuration">The changed configuration object.</param>
    ValueTask HandleConfigurationChangedAsync(Type type, Guid id, object configuration);

    /// <summary>
    /// Handles the added configuration.
    /// </summary>
    /// <param name="type">The type of the added configuration object.</param>
    /// <param name="id">The identifier of the added configuration object.</param>
    /// <param name="configuration">The added configuration object.</param>
    ValueTask HandleConfigurationAddedAsync(Type type, Guid id, object configuration);

    /// <summary>
    /// Handles the removed configuration.
    /// </summary>
    /// <param name="type">The type of the removed configuration object.</param>
    /// <param name="id">The identifier of the removed configuration object.</param>
    ValueTask HandleConfigurationRemovedAsync(Type type, Guid id);
}