// <copyright file="CastleSiegeDefenseBuyHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege.Actions;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles requests to re-purchase destroyed Castle Siege defense structures.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeDefenseBuyHandlerPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeDefenseBuyHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("1FCBF012-63CA-4B76-971F-F5848D2281CF")]
[BelongsToGroup(CastleSiegeGroupHandlerPlugIn.GroupKey)]
internal sealed class CastleSiegeDefenseBuyHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc />
    public bool IsEncryptionExpected => false;

    /// <inheritdoc />
    public byte Key => CastleSiegeDefenseBuyRequest.SubCode;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < CastleSiegeDefenseBuyRequest.Length)
        {
            return;
        }

        var request = new CastleSiegeDefenseBuyRequest(packet);
        await CastleSiegeNpcBuyAction.BuyAsync(
                player,
                CastleSiegeHandlerContext.Get(player),
                request.NpcNumber,
                request.NpcIndex)
            .ConfigureAwait(false);
    }
}
