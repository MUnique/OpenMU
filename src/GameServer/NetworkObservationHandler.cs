// <copyright file="NetworkObservationHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer;

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Analyzer.Archive;

/// <summary>
/// Archives the traffic of the players whose account is observed.
/// </summary>
/// <remarks>
/// It's part of the game server and not of the admin panel, so that the traffic of an observed
/// account is archived in a distributed deployment as well. The archive starts when the player
/// is logged in - the few packets before that (version check, login request) are not part of
/// it, because capturing them would mean to capture every connection unconditionally.
/// </remarks>
internal sealed class NetworkObservationHandler
{
    private readonly IPacketArchive _archive;

    private readonly int _serverId;

    private readonly string _serverDescription;

    private readonly ILogger<NetworkObservationHandler> _logger;

    private readonly ConcurrentDictionary<Player, ObservedSession> _sessions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkObservationHandler"/> class.
    /// </summary>
    /// <param name="archive">The archive in which the sessions are written.</param>
    /// <param name="serverId">The identifier of the game server.</param>
    /// <param name="serverDescription">The description of the game server.</param>
    /// <param name="logger">The logger.</param>
    public NetworkObservationHandler(IPacketArchive archive, int serverId, string serverDescription, ILogger<NetworkObservationHandler> logger)
    {
        this._archive = archive;
        this._serverId = serverId;
        this._serverDescription = serverDescription;
        this._logger = logger;
    }

    /// <summary>
    /// Starts to watch the specified player, so that its traffic is archived when it logs in
    /// with an observed account.
    /// </summary>
    /// <param name="player">The player which just connected.</param>
    public void Watch(Player player)
    {
        if (player is not RemotePlayer)
        {
            // Without a connection there is no traffic - an offline player has none.
            return;
        }

        player.PlayerLoggedIn += this.OnPlayerLoggedInAsync;
        player.PlayerEnteredWorld += this.OnPlayerEnteredWorldAsync;
        player.PlayerDisconnected += this.OnPlayerDisconnectedAsync;
    }

    private async ValueTask OnPlayerLoggedInAsync(Player player)
    {
        try
        {
            if (player.Account is not { IsNetworkObservationActive: true } account
                || player is not RemotePlayer { Connection: { } connection } remotePlayer
                || this._sessions.ContainsKey(player))
            {
                return;
            }

            var metadata = new ArchivedSessionMetadata
            {
                AccountName = account.LoginName,
                ServerType = ServerType.GameServer,
                ServerId = this._serverId,
                ServerDescription = this._serverDescription,
                RemoteEndPoint = connection.EndPoint?.ToString(),
                ClientVersion = remotePlayer.ClientVersion,
                StartTimestamp = DateTime.UtcNow,
            };

            if (await this._archive.StartSessionAsync(metadata).ConfigureAwait(false) is not { } writer)
            {
                return;
            }

            if (!this._sessions.TryAdd(player, new ObservedSession(writer, connection)))
            {
                await writer.DisposeAsync().ConfigureAwait(false);
                return;
            }

            connection.AddCaptureSink(writer);
        }
        catch (Exception ex)
        {
            // The observation must never break the session of the player.
            this._logger.LogWarning(ex, "Could not start to archive the traffic of {Player}.", player);
        }
    }

    private async ValueTask OnPlayerEnteredWorldAsync(Player player)
    {
        if (this._sessions.TryGetValue(player, out var session)
            && player.SelectedCharacter?.Name is { } characterName)
        {
            await session.Writer.AddCharacterNameAsync(characterName).ConfigureAwait(false);
        }
    }

    private async ValueTask OnPlayerDisconnectedAsync(Player player)
    {
        player.PlayerLoggedIn -= this.OnPlayerLoggedInAsync;
        player.PlayerEnteredWorld -= this.OnPlayerEnteredWorldAsync;
        player.PlayerDisconnected -= this.OnPlayerDisconnectedAsync;

        if (!this._sessions.TryRemove(player, out var session))
        {
            return;
        }

        try
        {
            // The connection is taken from the session: the player may not have it anymore
            // when it's already tearing down.
            session.Connection.RemoveCaptureSink(session.Writer);
            await session.Writer.DisposeAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogWarning(ex, "Could not finish the archived session of {Player}.", player);
        }
    }

    private sealed record ObservedSession(ArchivedSessionWriter Writer, IConnection Connection);
}
