// <copyright file="ShowCashShopItemRefundResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowCashShopItemRefundResultPlugIn"/>.
/// </summary>
[PlugIn(nameof(ShowCashShopItemRefundResultPlugIn), "The default implementation of the IShowCashShopItemRefundResultPlugIn.")]
[Guid("A1B2C3D4-E5F6-7890-1A2B-3C4D5E6F7890")]
public class ShowCashShopItemRefundResultPlugIn : IShowCashShopItemRefundResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowCashShopItemRefundResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowCashShopItemRefundResultPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowCashShopItemRefundResultAsync(CashShopRefundResult result)
    {
        if (this._player.Connection is not { Connected: true } connection)
        {
            return;
        }

        await connection.SendCashShopItemRefundResponseAsync((byte)result).ConfigureAwait(false);
    }
}
