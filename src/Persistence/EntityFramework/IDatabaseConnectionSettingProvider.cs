// <copyright file="IDatabaseConnectionSettingProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Threading;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Interface for a provider of <see cref="ConnectionSetting"/>s for specified context types.
/// </summary>
public interface IDatabaseConnectionSettingProvider
{
    /// <summary>
    /// Gets the initialization task.
    /// </summary>
    Task? Initialization { get; }

    /// <summary>
    /// Initializes the settings provider.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task InitializeAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets the connection setting for the specified context type.
    /// </summary>
    /// <typeparam name="TContextType">The type of the context type.</typeparam>
    /// <returns>The connection settings.</returns>
    ConnectionSetting GetConnectionSetting<TContextType>()
        where TContextType : DbContext;

    /// <summary>
    /// Gets the connection setting for the specified context type.
    /// </summary>
    /// <param name="contextType">Type of the context.</param>
    /// <returns>The connection settings.</returns>
    ConnectionSetting GetConnectionSetting(Type contextType);
}