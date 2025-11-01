// <copyright file="ShowCashShopPointsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowCashShopPointsPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowCashShopPointsPlugIn), "The default implementation of the IShowCashShopPointsPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("A1B2C3D4-E5F6-7A8B-9C0D-1E2F3A4B5C6D")]
public class ShowCashShopPointsPlugIn : IShowCashShopPointsPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowCashShopPointsPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowCashShopPointsPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowCashShopPointsAsync()
    {
        if (this._player.Connection is not { Connected: true } connection)
        {
            return;
        }

        var wCoinC = (uint)(this._player.Account?.WCoinC ?? 0);
        var wCoinP = (uint)(this._player.Account?.WCoinP ?? 0);
        var goblinPoints = (uint)(this._player.Account?.GoblinPoints ?? 0);

        await connection.SendCashShopPointsResponseAsync(wCoinC, wCoinP, goblinPoints).ConfigureAwait(false);
    }
}
