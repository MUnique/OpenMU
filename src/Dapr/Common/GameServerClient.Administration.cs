// <copyright file="GameServerClient.Administration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

using System.Text.Json.Nodes;
using System.Threading;
using MUnique.OpenMU.Interfaces;

/// <summary>The confirmed live administration part of the remote game server client.</summary>
public sealed partial class GameServerClient : IGameServerAdministration
{
    /// <summary>Gets the unique Dapr app identifier used for confirmed live administration.</summary>
    public string AdministrationAppId => $"game-server-{this.Id}";

    /// <inheritdoc />
    public ValueTask<IReadOnlyList<LivePlayerInfo>> GetPlayerSnapshotAsync(CancellationToken cancellationToken)
        => this.InvokeAdministrationAsync<IReadOnlyList<LivePlayerInfo>>(nameof(this.GetPlayerSnapshotAsync), new(), cancellationToken);

    /// <inheritdoc />
    public ValueTask<LivePlayerPage> GetPlayersAsync(ushort? mapNumber, uint? guildId, int offset, int limit, CancellationToken cancellationToken)
        => this.InvokeAdministrationAsync<LivePlayerPage>(nameof(this.GetPlayersAsync), new() { MapNumber = mapNumber, GuildId = guildId, Offset = offset, Limit = limit }, cancellationToken);

    /// <inheritdoc />
    public async ValueTask<LivePlayerInfo?> GetPlayerAsync(string characterName, CancellationToken cancellationToken)
        => (await this.InvokeAdministrationAsync<LivePlayerResponse>(nameof(this.GetPlayerAsync), new() { CharacterName = characterName }, cancellationToken).ConfigureAwait(false)).Player;

    /// <inheritdoc />
    public ValueTask<GameServerAdminResult> TeleportAsync(string characterName, Guid characterId, ushort mapNumber, byte x, byte y, CancellationToken cancellationToken)
        => this.InvokeAdministrationAsync<GameServerAdminResult>(nameof(this.TeleportAsync), new() { CharacterName = characterName, CharacterId = characterId, MapNumber = mapNumber, X = x, Y = y }, cancellationToken);

    /// <inheritdoc />
    public ValueTask<GameServerAdminResult> GiveItemAsync(string characterName, Guid characterId, ItemSpecification item, CancellationToken cancellationToken)
        => this.InvokeAdministrationAsync<GameServerAdminResult>(nameof(this.GiveItemAsync), new() { CharacterName = characterName, CharacterId = characterId, Item = item }, cancellationToken);

    /// <inheritdoc />
    public ValueTask<GameServerAdminResult> SetLevelAsync(string characterName, Guid characterId, int level, bool master, CancellationToken cancellationToken)
        => this.InvokeAdministrationAsync<GameServerAdminResult>(nameof(this.SetLevelAsync), new() { CharacterName = characterName, CharacterId = characterId, Level = level, Master = master }, cancellationToken);

    /// <inheritdoc />
    public ValueTask<IReadOnlyList<LiveEventInfo>> GetEventsAsync(CancellationToken cancellationToken)
        => this.InvokeAdministrationAsync<IReadOnlyList<LiveEventInfo>>(nameof(this.GetEventsAsync), new(), cancellationToken);

    /// <inheritdoc />
    public ValueTask<GameServerAdminResult> StartEventAsync(string eventType, bool restart, LiveEventInfo expectedEvent, CancellationToken cancellationToken)
        => this.InvokeAdministrationAsync<GameServerAdminResult>(nameof(this.StartEventAsync), new() { EventType = eventType, Restart = restart, ExpectedEvent = expectedEvent }, cancellationToken);

    /// <inheritdoc />
    public ValueTask<IReadOnlyList<AdminPluginOperationInfo>> GetAdminOperationsAsync(CancellationToken cancellationToken)
        => this.InvokeAdministrationAsync<IReadOnlyList<AdminPluginOperationInfo>>(nameof(this.GetAdminOperationsAsync), new(), cancellationToken);

    /// <inheritdoc />
    public ValueTask<JsonObject> ReadAdminOperationAsync(Guid plugInId, string name, string arguments, CancellationToken cancellationToken)
        => this.InvokeAdministrationAsync<JsonObject>(nameof(this.ReadAdminOperationAsync), new() { PlugInId = plugInId, Operation = name, Arguments = arguments }, cancellationToken);

    /// <inheritdoc />
    public ValueTask<JsonObject> PreviewAdminOperationAsync(Guid plugInId, string name, string arguments, CancellationToken cancellationToken)
        => this.InvokeAdministrationAsync<JsonObject>(nameof(this.PreviewAdminOperationAsync), new() { PlugInId = plugInId, Operation = name, Arguments = arguments }, cancellationToken);

    /// <inheritdoc />
    public ValueTask<JsonObject> ExecuteAdminOperationAsync(Guid plugInId, string name, string arguments, JsonObject expectedPreview, CancellationToken cancellationToken)
        => this.InvokeAdministrationAsync<JsonObject>(nameof(this.ExecuteAdminOperationAsync), new() { PlugInId = plugInId, Operation = name, Arguments = arguments, ExpectedPreview = expectedPreview }, cancellationToken);

    private async ValueTask<T> InvokeAdministrationAsync<T>(string method, GameServerAdministrationRequest request, CancellationToken cancellationToken)
    {
        request.ServerId = this.Id;
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(30));
        return await this.DaprClient.InvokeMethodAsync<GameServerAdministrationRequest, T>(this.AdministrationAppId, $"administration/{method}", request, timeout.Token).ConfigureAwait(false);
    }
}
