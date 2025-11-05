// <copyright file="GuildRelationshipChangeRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Guild;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for guild relationship change request packets (alliance or hostility).
/// </summary>
[PlugIn(nameof(GuildRelationshipChangeRequestHandlerPlugIn), "Handler for guild relationship change request packets.")]
[Guid("8F2A5D19-3C4E-4F1B-9A7D-E8B1C5F6D2A3")]
internal class GuildRelationshipChangeRequestHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly AllianceRequestAction _allianceRequestAction = new();
    private readonly HostilityRequestAction _hostilityRequestAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => GuildRelationshipChangeRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        GuildRelationshipChangeRequest request = packet;

        if (request.RelationshipType == GuildRelationshipType.Alliance && request.RequestType == GuildRequestType.Join)
        {
            // Get the target player by ID to find their guild name
            var targetPlayer = player.CurrentMap?.GetObject(request.TargetPlayerId) as Player;
            if (targetPlayer?.GuildStatus is { } guildStatus
                && targetPlayer.GameContext is IGameServerContext serverContext
                && await serverContext.GuildServer.GetGuildAsync(guildStatus.GuildId).ConfigureAwait(false) is { Name: not null } targetGuild)
            {
                await this._allianceRequestAction.RequestAllianceAsync(player, targetGuild.Name).ConfigureAwait(false);
            }
        }
        else if (request.RelationshipType == GuildRelationshipType.Hostility && request.RequestType == GuildRequestType.Join)
        {
            // Get the target player by ID to find their guild name
            var targetPlayer = player.CurrentMap?.GetObject(request.TargetPlayerId) as Player;
            if (targetPlayer?.GuildStatus is { } guildStatus
                && targetPlayer.GameContext is IGameServerContext serverContext
                && await serverContext.GuildServer.GetGuildAsync(guildStatus.GuildId).ConfigureAwait(false) is { Name: not null } targetGuild)
            {
                await this._hostilityRequestAction.RequestHostilityAsync(player, targetGuild.Name).ConfigureAwait(false);
            }
        }
    }
}
