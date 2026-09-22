// <copyright file="CastleSiegeGuildListHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles requests for the selected Castle Siege guild list.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeGuildListHandlerPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeGuildListHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("34BBD48A-1992-4F24-8A9E-6742D18E8C24")]
internal class CastleSiegeGuildListHandlerPlugIn : IPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => CastleOwnerListRequest.Code;

    /// <inheritdoc/>
    public ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        var context = CastleSiegeHandlerContext.Get(player);
        var isAvailable = context is not null && !context.FinalGuildList.IsEmpty;
        var guilds = isAvailable
            ? CastleSiegeGuildSelector.OrderFinalGuilds(context!.FinalGuildList.Values).ToList()
            : [];
        return player.InvokeViewPlugInAsync<ICastleSiegeGuildListPlugIn>(
            plugIn => plugIn.ShowGuildListAsync(isAvailable ? (byte)1 : (byte)2, guilds));
    }
}
