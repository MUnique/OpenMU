// <copyright file="AdminReadModel.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.Threading;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Adapts the panel's existing search cache and dashboard data for read operations.</summary>
public sealed class AdminReadModel(ConfigurationSearchIndexCache search, OnlinePlayerHistory history, DataUpdateService updates) : IAdminReadModel
{
    /// <inheritdoc />
    public IReadOnlyList<PlayerCountSample> PlayerHistory => history.GetSamples().Select(s => new PlayerCountSample(s.Timestamp, s.PlayerCount)).ToArray();

    /// <inheritdoc />
    public async ValueTask<IReadOnlyList<ConfigurationSearchHit>> SearchConfigurationAsync(string text, CancellationToken cancellationToken)
    {
        await search.EnsureLoadedAsync(cancellationToken).ConfigureAwait(false);
        var terms = text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        return search.Entries.Where(e => !e.Url.StartsWith("/plugins", StringComparison.OrdinalIgnoreCase)
                && terms.All(term => e.NormalizedHaystack.Contains(term, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(e => e.Caption, StringComparer.OrdinalIgnoreCase).ThenBy(e => e.Url, StringComparer.Ordinal)
            .Select(e => new ConfigurationSearchHit(e.Caption, e.Path, e.Url)).ToArray();
    }

    /// <inheritdoc />
    public async ValueTask<int> GetPendingUpdatesAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return (await updates.DetermineAvailableUpdatesAsync().ConfigureAwait(false)).Count;
    }
}
