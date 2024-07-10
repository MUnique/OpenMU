// <copyright file="DuelChannelJoinRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Duel;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Duel;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for duel channel join request packets (new duel system), which is sent by a spectator.
/// </summary>
[PlugIn(nameof(DuelChannelJoinRequestHandlerPlugIn), "Handler for duel channel join request packets (new duel system), which is sent by a spectator.")]
[Guid("6A4DB859-A06E-4AA9-9E85-AAC712265492")]
[MinimumClient(4, 0, ClientLanguage.Invariant)]
[BelongsToGroup(DuelGroupHandlerPlugIn.GroupKey)]
internal class DuelChannelJoinRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly DuelActions _action = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => DuelChannelJoinRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        DuelChannelJoinRequest request = packet;
        await this._action.HandleDuelChannelJoinRequestAsync(player, request.ChannelId).ConfigureAwait(false);
    }
}