// <copyright file="PlayerWriteOperations.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using System.Text.Json.Nodes;
using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>Routes player operations to the game server which owns the live account.</summary>
/// <param name="servers">The server registry.</param>
/// <param name="login">The login server.</param>
/// <param name="persistence">The persistence provider.</param>
/// <param name="configurations">The configuration source.</param>
/// <param name="executor">The authorization and audit service.</param>
public sealed class PlayerWriteOperations(IServerProvider servers, ILoginServer login, IPersistenceContextProvider persistence, IDataSource<GameConfiguration> configurations, AdminOperationExecutor executor)
{
    /// <summary>Disconnects an online account without changing its persisted state.</summary>
    /// <param name="loginName">The exact account login name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Whether a live account was disconnected.</returns>
    [AdminOperation(AdminRole.Operator, ReadOnly = false)]
    public ValueTask<JsonObject> DisconnectAsync(string loginName, CancellationToken cancellationToken)
        => executor.ExecuteAsync(
            AdminRoles.Operator,
            "players_disconnect",
            new { loginName },
            async _ =>
            {
                var server = await this.ResolveAsync(loginName).ConfigureAwait(false);
                var disconnected = server is not null && await server.DisconnectAccountAsync(loginName).ConfigureAwait(false);
                return AdminReadResult.From(new { loginName, disconnected });
            },
            cancellationToken);

    /// <summary>Bans and disconnects a live character through its owning game server.</summary>
    /// <param name="characterName">The exact live character name.</param>
    /// <param name="reason">The mandatory audit reason.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Whether the live character was banned.</returns>
    [AdminOperation(AdminRole.Operator, ReadOnly = false)]
    public ValueTask<JsonObject> BanAsync(string characterName, string reason, CancellationToken cancellationToken)
        => executor.ExecuteAsync(
            AdminRoles.Operator,
            "players_ban",
            new { characterName, reason },
            async _ =>
            {
                AccountWriteOperations.ValidateReason(reason);
                using var context = persistence.CreateNewPlayerContext(await configurations.GetOwnerAsync(cancellationToken: cancellationToken).ConfigureAwait(false));
                var account = await context.GetAccountByCharacterNameAsync(characterName, cancellationToken).ConfigureAwait(false)
                    ?? throw new ArgumentException("Character not found.", nameof(characterName));
                if (account.State is not (MUnique.OpenMU.DataModel.Entities.AccountState.Normal or MUnique.OpenMU.DataModel.Entities.AccountState.Banned or MUnique.OpenMU.DataModel.Entities.AccountState.TemporarilyBanned))
                {
                    await executor.RequireAsync(AdminRoles.Administrator, cancellationToken).ConfigureAwait(false);
                }

                var server = await this.ResolveAsync(account.LoginName).ConfigureAwait(false)
                    ?? throw new AdminOperationException("The character is offline. Use accounts_set_state instead.");
                if (!await server.BanPlayerAsync(characterName).ConfigureAwait(false))
                {
                    throw new AdminOperationException("The live character could not be banned. Refresh players_list and retry.");
                }

                return AdminReadResult.From(new { characterName, banned = true });
            },
            cancellationToken);

    private async ValueTask<IGameServer?> ResolveAsync(string loginName)
    {
        var snapshot = await login.GetSnapshotAsync().ConfigureAwait(false);
        var entry = snapshot.FirstOrDefault(e => string.Equals(e.Key, loginName, StringComparison.OrdinalIgnoreCase));
        if (entry.Key is null)
        {
            return null;
        }

        return servers.Servers.OfType<IGameServer>().SingleOrDefault(s => s.Id == entry.Value)
            ?? throw new AdminOperationException("The owning game server is unavailable; no player operation was executed.", AdminOperationErrorKind.Unavailable);
    }
}
