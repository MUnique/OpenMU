// <copyright file="ShowCashShopOpenStatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowCashShopOpenStatePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowCashShopOpenStatePlugIn), "The default implementation of the IShowCashShopOpenStatePlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("B2C3D4E5-F6A7-8B9C-0D1E-2F3A4B5C6D7E")]
public class ShowCashShopOpenStatePlugIn : IShowCashShopOpenStatePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowCashShopOpenStatePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowCashShopOpenStatePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowCashShopOpenStateAsync(bool isOpen)
    {
        if (this._player.Connection is not { Connected: true } connection)
        {
            return;
        }

        await connection.SendCashShopOpenStateResponseAsync(isOpen).ConfigureAwait(false);
    }
}
