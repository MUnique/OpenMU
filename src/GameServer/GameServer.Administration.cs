// <copyright file="GameServer.Administration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer;

using System.Text.Json.Nodes;
using System.Threading;
using MUnique.OpenMU.GameLogic.Administration;
using MUnique.OpenMU.Interfaces;

/// <summary>The live administration channel of a game server.</summary>
public sealed partial class GameServer : IGameServerAdministration
{
    private GameServerAdministration Administration => new(this._gameContext);

    /// <inheritdoc />
    public ValueTask<IReadOnlyList<LivePlayerInfo>> GetPlayerSnapshotAsync(CancellationToken cancellationToken)
        => this.Administration.GetPlayerSnapshotAsync(cancellationToken);

    /// <inheritdoc />
    public ValueTask<LivePlayerPage> GetPlayersAsync(ushort? mapNumber, uint? guildId, int offset, int limit, CancellationToken cancellationToken)
        => this.Administration.GetPlayersAsync(mapNumber, guildId, offset, limit, cancellationToken);

    /// <inheritdoc />
    public ValueTask<LivePlayerInfo?> GetPlayerAsync(string characterName, CancellationToken cancellationToken)
        => this.Administration.GetPlayerAsync(characterName, cancellationToken);

    /// <inheritdoc />
    public ValueTask<GameServerAdminResult> TeleportAsync(string characterName, Guid characterId, ushort mapNumber, byte x, byte y, CancellationToken cancellationToken)
        => this.Administration.TeleportAsync(characterName, characterId, mapNumber, x, y, cancellationToken);

    /// <inheritdoc />
    public ValueTask<GameServerAdminResult> GiveItemAsync(string characterName, Guid characterId, ItemSpecification item, CancellationToken cancellationToken)
        => this.Administration.GiveItemAsync(characterName, characterId, item, cancellationToken);

    /// <inheritdoc />
    public ValueTask<GameServerAdminResult> SetLevelAsync(string characterName, Guid characterId, int level, bool master, CancellationToken cancellationToken)
        => this.Administration.SetLevelAsync(characterName, characterId, level, master, cancellationToken);

    /// <inheritdoc />
    public ValueTask<IReadOnlyList<LiveEventInfo>> GetEventsAsync(CancellationToken cancellationToken)
        => this.Administration.GetEventsAsync(cancellationToken);

    /// <inheritdoc />
    public ValueTask<GameServerAdminResult> StartEventAsync(string eventType, bool restart, LiveEventInfo expectedEvent, CancellationToken cancellationToken)
        => this.Administration.StartEventAsync(eventType, restart, expectedEvent, cancellationToken);

    /// <inheritdoc />
    public ValueTask<IReadOnlyList<AdminPluginOperationInfo>> GetAdminOperationsAsync(CancellationToken cancellationToken)
        => this.Administration.GetAdminOperationsAsync(cancellationToken);

    /// <inheritdoc />
    public ValueTask<JsonObject> ReadAdminOperationAsync(Guid plugInId, string name, string arguments, CancellationToken cancellationToken)
        => this.Administration.ReadAdminOperationAsync(plugInId, name, arguments, cancellationToken);

    /// <inheritdoc />
    public ValueTask<JsonObject> PreviewAdminOperationAsync(Guid plugInId, string name, string arguments, CancellationToken cancellationToken)
        => this.Administration.PreviewAdminOperationAsync(plugInId, name, arguments, cancellationToken);

    /// <inheritdoc />
    public ValueTask<JsonObject> ExecuteAdminOperationAsync(Guid plugInId, string name, string arguments, JsonObject expectedPreview, CancellationToken cancellationToken)
        => this.Administration.ExecuteAdminOperationAsync(plugInId, name, arguments, expectedPreview, cancellationToken);
}
