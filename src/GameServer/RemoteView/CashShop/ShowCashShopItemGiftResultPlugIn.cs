// <copyright file="ShowCashShopItemGiftResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowCashShopItemGiftResultPlugIn"/>.
/// </summary>
[PlugIn(nameof(ShowCashShopItemGiftResultPlugIn), "The default implementation of the IShowCashShopItemGiftResultPlugIn.")]
[Guid("D4E5F6A7-B8C9-0D1E-2F3A-4B5C6D7E8F9A")]
public class ShowCashShopItemGiftResultPlugIn : IShowCashShopItemGiftResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowCashShopItemGiftResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowCashShopItemGiftResultPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowCashShopItemGiftResultAsync(CashShopGiftResult result, string receiverName)
    {
        if (this._player.Connection is not { Connected: true } connection)
        {
            return;
        }

        await connection.SendCashShopItemGiftResponseAsync((byte)result).ConfigureAwait(false);
    }
}
