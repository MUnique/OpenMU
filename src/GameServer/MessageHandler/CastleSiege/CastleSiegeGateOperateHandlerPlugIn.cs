// <copyright file="CastleSiegeGateOperateHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege.Actions;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles Castle Siege gate open and close requests.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeGateOperateHandlerPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeGateOperateHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("8B4ADE95-4BA9-4425-99AA-B2DBA03ACD88")]
[BelongsToGroup(CastleSiegeGroupHandlerPlugIn.GroupKey)]
internal sealed class CastleSiegeGateOperateHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc />
    public bool IsEncryptionExpected => false;

    /// <inheritdoc />
    public byte Key => ToggleCastleGateRequest.SubCode;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < ToggleCastleGateRequest.Length)
        {
            return;
        }

        var request = new ToggleCastleGateRequest(packet);
        await CastleSiegeGateOperateAction.OperateAsync(
                player,
                CastleSiegeHandlerContext.Get(player),
                request.GateId,
                request.IsOpen)
            .ConfigureAwait(false);
    }
}
