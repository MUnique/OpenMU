// <copyright file="WarpHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for warp request packets.
/// This one is called when a player uses the warp list.
/// </summary>
[PlugIn("WarpHandlerPlugIn", "Handler for warp request packets.")]
[Guid("3d261a26-4357-4367-b999-703ea936f4e9")]
internal class WarpHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly WarpAction _warpAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => WarpCommandRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        WarpCommandRequest request = packet;
        ushort warpInfoIndex = request.WarpInfoIndex;
        var warpInfo = player.GameContext.Configuration.WarpList?.FirstOrDefault(info => info.Index == warpInfoIndex);
        if (warpInfo != null)
        {
            await this._warpAction.WarpToAsync(player, warpInfo).ConfigureAwait(false);
        }
        else
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync($"Unknown warp index {warpInfoIndex}", MessageType.BlueNormal)).ConfigureAwait(false);
        }
    }
}