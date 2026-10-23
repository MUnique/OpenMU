// <copyright file="TestReadModel.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using System.Threading;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Deterministic dashboard data for protocol integration tests.</summary>
internal sealed class TestReadModel : IAdminReadModel
{
    /// <inheritdoc />
    public IReadOnlyList<PlayerCountSample> PlayerHistory => [new(DateTime.UnixEpoch, 0)];

    /// <inheritdoc />
    public ValueTask<IReadOnlyList<ConfigurationSearchHit>> SearchConfigurationAsync(string text, CancellationToken cancellationToken)
        => ValueTask.FromResult<IReadOnlyList<ConfigurationSearchHit>>([new("Experience rate", "Game configuration > Experience rate", "/edit-config/test")]);

    /// <inheritdoc />
    public ValueTask<int> GetPendingUpdatesAsync(CancellationToken cancellationToken) => ValueTask.FromResult(0);
}
