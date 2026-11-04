// <copyright file="CastleSiegeDefenseRepairHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege.Actions;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles requests to repair Castle Siege defense structures.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeDefenseRepairHandlerPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeDefenseRepairHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("2FDCA492-97D6-4972-B094-D94A71871717")]
[BelongsToGroup(CastleSiegeGroupHandlerPlugIn.GroupKey)]
internal sealed class CastleSiegeDefenseRepairHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private const short SeniorNumber = 223;

    /// <inheritdoc />
    public bool IsEncryptionExpected => false;

    /// <inheritdoc />
    public byte Key => CastleSiegeDefenseRepairRequest.SubCode;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        // Defense structures are managed in the Senior's window, like the taxes and the tribute.
        if (packet.Length < CastleSiegeDefenseRepairRequest.Length
            || player.OpenedNpc?.Definition.Number != SeniorNumber)
        {
            return;
        }

        var request = new CastleSiegeDefenseRepairRequest(packet);
        await CastleSiegeNpcRepairAction.RepairAsync(
                player,
                CastleSiegeHandlerContext.Get(player),
                request.NpcNumber,
                request.NpcIndex)
            .ConfigureAwait(false);
    }
}
