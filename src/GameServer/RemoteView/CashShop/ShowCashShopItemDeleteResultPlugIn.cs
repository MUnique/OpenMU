// <copyright file="ShowCashShopItemDeleteResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowCashShopItemDeleteResultPlugIn"/>.
/// </summary>
[PlugIn(nameof(ShowCashShopItemDeleteResultPlugIn), "The default implementation of the IShowCashShopItemDeleteResultPlugIn.")]
[Guid("F6A7B8C9-D0E1-2F3A-4B5C-6D7E8F9A0B1C")]
public class ShowCashShopItemDeleteResultPlugIn : IShowCashShopItemDeleteResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowCashShopItemDeleteResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowCashShopItemDeleteResultPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowCashShopItemDeleteResultAsync(bool success, byte itemSlot)
    {
        if (this._player.Connection is not { Connected: true } connection)
        {
            return;
        }

        await connection.SendCashShopItemDeleteResponseAsync((byte)(success ? 1 : 0)).ConfigureAwait(false);
    }
}
