// <copyright file="GuildRelationshipChangeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Guild;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;
using ViewGuildRelationshipType = MUnique.OpenMU.GameLogic.Views.Guild.GuildRelationshipType;
using ViewGuildRelationshipRequestType = MUnique.OpenMU.GameLogic.Views.Guild.GuildRelationshipRequestType;

/// <summary>
/// Handler for guild relationship change request packets (C1 E5).
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.GuildRelationshipChangeHandlerPlugIn_Name), Description = nameof(PlugInResources.GuildRelationshipChangeHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("F1C3E1D2-7A4B-4E5C-9F6D-2A8B3C4D5E6F")]
internal class GuildRelationshipChangeHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly GuildRelationshipChangeAction _action = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => GuildRelationshipChangeRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        GuildRelationshipChangeRequest request = packet;

        var targetPlayer = player.GetObject(request.TargetPlayerId);
        if (player == targetPlayer && request.RelationshipType == GuildRelationshipType.Alliance && request.RequestType == GuildRequestType.Leave)
        {
            await this._action.RequestLeaveAllianceAsync(player).ConfigureAwait(false);
        }
        else
        {
            await this._action.RequestAsync(
                player,
                request.TargetPlayerId,
                (ViewGuildRelationshipType)(byte)request.RelationshipType,
                (ViewGuildRelationshipRequestType)(byte)request.RequestType).ConfigureAwait(false);
        }
    }
}
