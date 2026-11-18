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
/// <param name="accounts">The account operations, which the registration shares with the admin panel.</param>
/// <param name="executor">The authorization and audit service.</param>
public sealed class WebsiteOperations(IServerProvider servers, AccountWriteOperations accounts, AdminOperationExecutor executor)
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

    /// <summary>Registers a new account, which can log in right away.</summary>
    /// <param name="loginName">The login name of the new account.</param>
    /// <param name="password">The password of the new account; never recorded in the audit trail.</param>
    /// <param name="email">The optional e-mail address.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c>, if the account was created; <c>false</c>, if the login name is already taken.</returns>
    /// <remarks>
    /// The account is created exactly like by the admin panel, and the registration is audited with the name of the key.
    /// The website has to protect its form against abuse (a captcha, a limit per visitor): this server only sees the website.
    /// </remarks>
    [AdminOperation("WebManager", ReadOnly = false)]
    [NotExposedToAgents("Registration form of websites; agents use accounts_create.")]
    public async ValueTask<bool> RegisterAsync(string loginName, string password, string? email, CancellationToken cancellationToken)
    {
        var result = await executor.ExecuteAsync(
            AdminRoles.WebManager,
            WebsitePermissions.Register,
            new { loginName, email },
            _ => accounts.CreateAccountAsync(loginName, password, email, cancellationToken),
            cancellationToken).ConfigureAwait(false);
        return result["status"]?.GetValue<string>() == "created";
    }
}
