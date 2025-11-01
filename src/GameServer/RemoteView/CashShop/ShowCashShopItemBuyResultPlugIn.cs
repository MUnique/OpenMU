// <copyright file="ShowCashShopItemBuyResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowCashShopItemBuyResultPlugIn"/>.
/// </summary>
[PlugIn(nameof(ShowCashShopItemBuyResultPlugIn), "The default implementation of the IShowCashShopItemBuyResultPlugIn.")]
[Guid("C3D4E5F6-A7B8-9C0D-1E2F-3A4B5C6D7E8F")]
public class ShowCashShopItemBuyResultPlugIn : IShowCashShopItemBuyResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowCashShopItemBuyResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowCashShopItemBuyResultPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowCashShopItemBuyResultAsync(CashShopBuyResult result, int productId)
    {
        if (this._player.Connection is not { Connected: true } connection)
        {
            return;
        }

        await connection.SendCashShopItemBuyResponseAsync((byte)result, (uint)productId).ConfigureAwait(false);
    }
}
