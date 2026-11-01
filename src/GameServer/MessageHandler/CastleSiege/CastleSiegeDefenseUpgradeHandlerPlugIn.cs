// <copyright file="CastleSiegeDefenseUpgradeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege.Actions;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles requests to upgrade Castle Siege defense structures.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeDefenseUpgradeHandlerPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeDefenseUpgradeHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("6522CF58-6103-47D7-9ECF-B326EF2C9D88")]
[BelongsToGroup(CastleSiegeGroupHandlerPlugIn.GroupKey)]
internal sealed class CastleSiegeDefenseUpgradeHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private const short SeniorNumber = 223;

    /// <inheritdoc />
    public bool IsEncryptionExpected => false;

    /// <inheritdoc />
    public byte Key => CastleSiegeDefenseUpgradeRequest.SubCode;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        // Defense structures are managed in the Senior's window, like the taxes and the tribute.
        if (packet.Length < CastleSiegeDefenseUpgradeRequest.Length
            || player.OpenedNpc?.Definition.Number != SeniorNumber)
        {
            return;
        }

        var request = new CastleSiegeDefenseUpgradeRequest(packet);
        var type = request.NpcUpgradeType <= byte.MaxValue
                   && Enum.IsDefined((CastleSiegeUpgradeType)(byte)request.NpcUpgradeType)
            ? (CastleSiegeUpgradeType)(byte)request.NpcUpgradeType
            : (CastleSiegeUpgradeType)byte.MaxValue;
        var requestedLevel = request.NpcUpgradeValue <= byte.MaxValue
            ? (byte)request.NpcUpgradeValue
            : byte.MaxValue;
        await CastleSiegeNpcUpgradeAction.UpgradeAsync(
                player,
                CastleSiegeHandlerContext.Get(player),
                request.NpcNumber,
                request.NpcIndex,
                type,
                requestedLevel)
            .ConfigureAwait(false);
    }
}
