// <copyright file="WebsiteOperations.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations.Website;

using System.Threading;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// The functions of the website API, which a website of the server uses with an API key
/// with the <see cref="AdminRoles.WebManager"/> role.
/// </summary>
/// <remarks>
/// Each function needs its <see cref="WebsitePermissions"/> entry in the allowlist of the key.
/// The results only contain what a website may show to its visitors, so a website never needs
/// a key which can read the admin views of accounts and characters.
/// </remarks>
/// <param name="servers">The server provider.</param>
/// <param name="executor">The authorization and audit service.</param>
public sealed class WebsiteOperations(IServerProvider servers, AdminOperationExecutor executor)
{
    /// <summary>Reads the public state of the game servers.</summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The game servers with their state and number of players.</returns>
    [AdminOperation("WebManager")]
    [NotExposedToAgents("Public projection for websites; agents use server_overview.")]
    public async ValueTask<WebsiteServerStatus> StatusAsync(CancellationToken cancellationToken)
    {
        await executor.RequireOperationAsync(AdminRoles.WebManager, WebsitePermissions.Status, cancellationToken).ConfigureAwait(false);
        var gameServers = servers.Servers
            .OfType<IGameServer>()
            .OrderBy(server => server.Id)
            .Take(AdminReadResult.MaximumRows)
            .Select(server => new WebsiteGameServerStatus(
                server.Id,
                server.Description,
                server.ServerState == ServerState.Started,
                server.CurrentConnections,
                server.MaximumConnections))
            .ToList();
        return new WebsiteServerStatus(gameServers.Sum(server => server.Players), gameServers);
    }
}
