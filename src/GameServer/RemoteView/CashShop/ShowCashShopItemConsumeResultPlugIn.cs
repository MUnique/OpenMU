// <copyright file="ShowCashShopItemConsumeResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowCashShopItemConsumeResultPlugIn"/>.
/// </summary>
[PlugIn(nameof(ShowCashShopItemConsumeResultPlugIn), "The default implementation of the IShowCashShopItemConsumeResultPlugIn.")]
[Guid("A7B8C9D0-E1F2-3A4B-5C6D-7E8F9A0B1C2D")]
public class ShowCashShopItemConsumeResultPlugIn : IShowCashShopItemConsumeResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowCashShopItemConsumeResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowCashShopItemConsumeResultPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowCashShopItemConsumeResultAsync(bool success, byte itemSlot)
    {
        if (this._player.Connection is not { Connected: true } connection)
        {
            return;
        }

        await connection.SendCashShopItemConsumeResponseAsync((byte)(success ? 1 : 0)).ConfigureAwait(false);
    }
}
