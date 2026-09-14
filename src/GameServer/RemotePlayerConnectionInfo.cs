// <copyright file="RemotePlayerConnectionInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer;

using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Analyzer;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// The <see cref="ICapturedConnectionInfo"/> of a <see cref="RemotePlayer"/>.
/// </summary>
internal sealed class RemotePlayerConnectionInfo : ICapturedConnectionInfo
{
    private readonly RemotePlayer _player;

    private readonly IConnection _connection;

    private readonly NetworkObservationHandler? _observationHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemotePlayerConnectionInfo"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="connection">The connection of the player.</param>
    /// <param name="serverId">The identifier of the game server.</param>
    /// <param name="serverDescription">The description of the game server.</param>
    /// <param name="observationHandler">The handler which archives the traffic of the observed
    /// accounts, if the observation is configured.</param>
    public RemotePlayerConnectionInfo(
        RemotePlayer player,
        IConnection connection,
        int serverId,
        string serverDescription,
        NetworkObservationHandler? observationHandler = null)
    {
        this._player = player;
        this._connection = connection;
        this.ServerId = serverId;
        this.ServerDescription = serverDescription;
        this._observationHandler = observationHandler;
    }

    /// <inheritdoc />
    public Guid Id => this._connection.Id;

    /// <inheritdoc />
    public ServerType ServerType => ServerType.GameServer;

    /// <inheritdoc />
    public int ServerId { get; }

    /// <inheritdoc />
    public string ServerDescription { get; }

    /// <inheritdoc />
    public string? AccountName => this._player.Account?.LoginName;

    /// <inheritdoc />
    public string? CharacterName => this._player.SelectedCharacter?.Name;

    /// <inheritdoc />
    public string? RemoteEndPoint => this._connection.EndPoint?.ToString();

    /// <inheritdoc />
    public ClientVersion ClientVersion => this._player.ClientVersion;

    /// <inheritdoc />
    public PacketDefinitionSet DefinitionSet => PacketDefinitionSet.GameServer;

    /// <inheritdoc />
    public bool IsConnected => this._connection.Connected;

    /// <inheritdoc />
    public string DisplayName => this.CharacterName ?? this.AccountName ?? this.RemoteEndPoint ?? this.Id.ToString();

    /// <inheritdoc />
    public bool IsObserved => this._player.Account?.IsNetworkObservationActive is true;

    /// <inheritdoc />
    public void AddCaptureSink(IPacketCaptureSink sink) => this._connection.AddCaptureSink(sink);

    /// <inheritdoc />
    public void RemoveCaptureSink(IPacketCaptureSink sink) => this._connection.RemoveCaptureSink(sink);

    /// <inheritdoc />
    public async ValueTask<bool> SetObservationAsync(bool isActive)
    {
        if (this._player.Account is not { } account)
        {
            return false;
        }

        if (account.IsNetworkObservationActive != isActive)
        {
            account.IsNetworkObservationActive = isActive;

            // The flag belongs to the account, so it survives the session - the player owns
            // the account object and its context, so it's saved through the player.
            await this._player.SaveProgressAsync().ConfigureAwait(false);
        }

        if (this._observationHandler is { } observationHandler)
        {
            await observationHandler.ApplyObservationAsync(this._player, isActive).ConfigureAwait(false);
        }

        return true;
    }

    /// <inheritdoc />
    public ValueTask DisconnectAsync() => this._player.DisconnectAsync();
}
