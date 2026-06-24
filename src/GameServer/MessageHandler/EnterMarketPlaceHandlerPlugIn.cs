// <copyright file="EnterMarketPlaceHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameServer.MessageHandler.MuHelper;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for the <see cref="EnterMarketPlaceRequest"/>, sent by the 'Warp' button of the
/// Market Union Member Julia window. It warps the player between Lorencia and the Loren Market,
/// depending on which side the player is currently on.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("5e028dcf-a5af-40a0-b958-2deb38aae4bc")]
[BelongsToGroup(MuHelperGroupHandler.GroupKey)]
internal class EnterMarketPlaceHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private const string PlugInName = "Enter Market Place Handler";

    private const string PlugInDescription = "Handler which warps a player between Lorencia and the Loren Market when using the 'Warp' button of Market Union Member Julia.";

    /// <summary>
    /// The map number of the Loren Market.
    /// </summary>
    private const short LorenMarketMapNumber = 79;

    /// <summary>
    /// The map number of Lorencia, where the player is warped back to.
    /// </summary>
    private const short LorenciaMapNumber = 0;

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => EnterMarketPlaceRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < EnterMarketPlaceRequest.Length)
        {
            return;
        }

        // Only allow the warp through the actual Julia window, so a crafted packet can't be used
        // as a free teleport from anywhere.
        if (player.OpenedNpc?.Definition.NpcWindow != NpcWindow.JuliaWarpMarketServer)
        {
            return;
        }

        // Julia warps both ways: from the Loren Market back to Lorencia, and from anywhere else
        // (her counterpart in Lorencia) into the Loren Market.
        var targetMapNumber = player.CurrentMap?.Definition.Number == LorenMarketMapNumber
            ? LorenciaMapNumber
            : LorenMarketMapNumber;

        var targetMap = await player.GameContext.GetMapAsync((ushort)targetMapNumber).ConfigureAwait(false);
        if (targetMap?.SafeZoneSpawnGate is not { } targetGate)
        {
            return;
        }

        player.OpenedNpc = null;
        await player.WarpToAsync(targetGate).ConfigureAwait(false);
    }
}
