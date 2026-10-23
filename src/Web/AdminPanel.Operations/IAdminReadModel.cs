// <copyright file="IAdminReadModel.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using System.Threading;

/// <summary>Supplies existing dashboard and search data without depending on a UI framework.</summary>
public interface IAdminReadModel
{
    /// <summary>Gets the sampled online player history of the last 24 hours.</summary>
    IReadOnlyList<PlayerCountSample> PlayerHistory { get; }

    /// <summary>Gets the configuration search entries matching the supplied text.</summary>
    /// <param name="text">The case-insensitive search text.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching entries as scalar fields.</returns>
    ValueTask<IReadOnlyList<ConfigurationSearchHit>> SearchConfigurationAsync(string text, CancellationToken cancellationToken);

    /// <summary>Gets the number of pending configuration updates.</summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of pending updates.</returns>
    ValueTask<int> GetPendingUpdatesAsync(CancellationToken cancellationToken);
}
