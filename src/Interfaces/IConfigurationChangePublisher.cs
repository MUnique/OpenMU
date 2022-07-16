// <copyright file="IConfigurationChangePublisher.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// Interface for an object which publishes configuration changes to other services.
/// </summary>
public interface IConfigurationChangePublisher
{
    /// <summary>
    /// Gets a publisher which doesn't publish at all.
    /// </summary>
    static IConfigurationChangePublisher None { get; } = new NoneConfigurationChangePublisher();

    /// <summary>
    /// A configuration has changed.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="configuration">The changed configuration.</param>
    Task ConfigurationChangedAsync(Type type, Guid id, object configuration);

    /// <summary>
    /// A configuration has been added.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="configuration">The added configuration.</param>
    Task ConfigurationAddedAsync(Type type, Guid id, object configuration);

    /// <summary>
    /// A configuration has been removed.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="id">The identifier.</param>
    Task ConfigurationRemovedAsync(Type type, Guid id);

    /// <summary>
    /// A publisher which doesn't publish changes at all.
    /// </summary>
    private class NoneConfigurationChangePublisher : IConfigurationChangePublisher
    {
        /// <inheritdoc />
        public Task ConfigurationChangedAsync(Type type, Guid id, object configuration)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task ConfigurationAddedAsync(Type type, Guid id, object configuration)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task ConfigurationRemovedAsync(Type type, Guid id)
        {
            return Task.CompletedTask;
        }
    }
}
