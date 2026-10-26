// <copyright file="GameServerAdministrationController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.Host;

using System.Text.Json.Nodes;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.Interfaces;

/// <summary>Internal request/response administration; never subscribe live writes to pub/sub.</summary>
/// <param name="server">The owning game server.</param>
/// <param name="deliveries">The bounded duplicate-delivery guard.</param>
[ApiController]
[Route("administration")]
[ServiceFilter(typeof(DaprAppTokenAuthorizationFilter))]
public sealed class GameServerAdministrationController(IGameServer server, AdministrationDeliveries deliveries) : ControllerBase
{
    /// <summary>Reads one scalar snapshot for a complete player-list query.</summary>
    /// <param name="request">The addressed server.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The live scalar snapshot.</returns>
    [HttpPost(nameof(IGameServerAdministration.GetPlayerSnapshotAsync))]
    public ValueTask<IReadOnlyList<LivePlayerInfo>> GetPlayerSnapshotAsync(GameServerAdministrationRequest request, CancellationToken cancellationToken)
        => this.Resolve(request).GetPlayerSnapshotAsync(cancellationToken);

    /// <summary>Reads live characters with server-side filters and paging.</summary>
    /// <param name="request">The request arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The bounded page.</returns>
    [HttpPost(nameof(IGameServerAdministration.GetPlayersAsync))]
    public ValueTask<LivePlayerPage> GetPlayersAsync(GameServerAdministrationRequest request, CancellationToken cancellationToken)
        => this.Resolve(request).GetPlayersAsync(request.MapNumber, request.GuildId, request.Offset, request.Limit, cancellationToken);

    /// <summary>Reads one exact live character.</summary>
    /// <param name="request">The request arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The live character, or null.</returns>
    [HttpPost(nameof(IGameServerAdministration.GetPlayerAsync))]
    public async ValueTask<LivePlayerResponse> GetPlayerAsync(GameServerAdministrationRequest request, CancellationToken cancellationToken)
        => new(await this.Resolve(request).GetPlayerAsync(request.CharacterName, cancellationToken).ConfigureAwait(false));

    /// <summary>Teleports a live character with duplicate-delivery protection.</summary>
    /// <param name="request">The request arguments.</param>
    /// <returns>The confirmed result.</returns>
    [HttpPost(nameof(IGameServerAdministration.TeleportAsync))]
    public ValueTask<GameServerAdminResult> TeleportAsync(GameServerAdministrationRequest request)
        => deliveries.RunAsync(nameof(this.TeleportAsync), request, () => this.Resolve(request).TeleportAsync(request.CharacterName, request.CharacterId, request.MapNumber ?? throw new ArgumentException("Map number is required."), request.X, request.Y, CancellationToken.None));

    /// <summary>Gives one item with duplicate-delivery protection.</summary>
    /// <param name="request">The request arguments.</param>
    /// <returns>The confirmed result.</returns>
    [HttpPost(nameof(IGameServerAdministration.GiveItemAsync))]
    public ValueTask<GameServerAdminResult> GiveItemAsync(GameServerAdministrationRequest request)
        => deliveries.RunAsync(nameof(this.GiveItemAsync), request, () => this.Resolve(request).GiveItemAsync(request.CharacterName, request.CharacterId, request.Item ?? throw new ArgumentException("Item is required."), CancellationToken.None));

    /// <summary>Sets a live level with duplicate-delivery protection.</summary>
    /// <param name="request">The request arguments.</param>
    /// <returns>The confirmed result.</returns>
    [HttpPost(nameof(IGameServerAdministration.SetLevelAsync))]
    public ValueTask<GameServerAdminResult> SetLevelAsync(GameServerAdministrationRequest request)
        => deliveries.RunAsync(nameof(this.SetLevelAsync), request, () => this.Resolve(request).SetLevelAsync(request.CharacterName, request.CharacterId, request.Level, request.Master, CancellationToken.None));

    /// <summary>Reads the live event schedule.</summary>
    /// <param name="request">The request arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The bounded schedule.</returns>
    [HttpPost(nameof(IGameServerAdministration.GetEventsAsync))]
    public ValueTask<IReadOnlyList<LiveEventInfo>> GetEventsAsync(GameServerAdministrationRequest request, CancellationToken cancellationToken)
        => this.Resolve(request).GetEventsAsync(cancellationToken);

    /// <summary>Requests an event start with duplicate-delivery protection.</summary>
    /// <param name="request">The request arguments.</param>
    /// <returns>The confirmed result.</returns>
    [HttpPost(nameof(IGameServerAdministration.StartEventAsync))]
    public ValueTask<GameServerAdminResult> StartEventAsync(GameServerAdministrationRequest request)
        => deliveries.RunAsync(nameof(this.StartEventAsync), request, () => this.Resolve(request).StartEventAsync(request.EventType, request.Restart, request.ExpectedEvent ?? throw new ArgumentException("An approved event preview is required."), CancellationToken.None));

    /// <summary>Lists operations contributed by active plugins.</summary>
    /// <param name="request">The request arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The bounded catalog.</returns>
    [HttpPost(nameof(IGameServerAdministration.GetAdminOperationsAsync))]
    public ValueTask<IReadOnlyList<AdminPluginOperationInfo>> GetAdminOperationsAsync(GameServerAdministrationRequest request, CancellationToken cancellationToken)
        => this.Resolve(request).GetAdminOperationsAsync(cancellationToken);

    /// <summary>Calls a read-only contributed operation.</summary>
    /// <param name="request">The request arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The safe result.</returns>
    [HttpPost(nameof(IGameServerAdministration.ReadAdminOperationAsync))]
    public ValueTask<JsonObject> ReadAdminOperationAsync(GameServerAdministrationRequest request, CancellationToken cancellationToken)
        => this.Resolve(request).ReadAdminOperationAsync(request.PlugInId, request.Operation, request.Arguments, cancellationToken);

    /// <summary>Previews a contributed write without side effects.</summary>
    /// <param name="request">The request arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The safe preview.</returns>
    [HttpPost(nameof(IGameServerAdministration.PreviewAdminOperationAsync))]
    public ValueTask<JsonObject> PreviewAdminOperationAsync(GameServerAdministrationRequest request, CancellationToken cancellationToken)
        => this.Resolve(request).PreviewAdminOperationAsync(request.PlugInId, request.Operation, request.Arguments, cancellationToken);

    /// <summary>Applies a confirmed contributed write once per delivery identifier.</summary>
    /// <param name="request">The request arguments.</param>
    /// <returns>The safe result.</returns>
    [HttpPost(nameof(IGameServerAdministration.ExecuteAdminOperationAsync))]
    public ValueTask<JsonObject> ExecuteAdminOperationAsync(GameServerAdministrationRequest request)
        => deliveries.RunAsync(nameof(this.ExecuteAdminOperationAsync), request, () => this.Resolve(request).ExecuteAdminOperationAsync(request.PlugInId, request.Operation, request.Arguments, request.ExpectedPreview ?? throw new ArgumentException("A preview is required."), CancellationToken.None));

    private IGameServerAdministration Resolve(GameServerAdministrationRequest request)
        => request.ServerId == server.Id && server is IGameServerAdministration administration
            ? administration : throw new ArgumentException("The request does not address this game server.");
}
