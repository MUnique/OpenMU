// <copyright file="ShowTradeRequestAnswerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Trade;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Trade;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowTradeRequestAnswerPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("ShowTradeRequestAnswerPlugIn", "The default implementation of the IShowTradeRequestAnswerPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("243cbc67-7af3-48e2-9a56-d6e49c86b816")]
public class ShowTradeRequestAnswerPlugIn : IShowTradeRequestAnswerPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowTradeRequestAnswerPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowTradeRequestAnswerPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowTradeRequestAnswerAsync(bool tradeAccepted)
    {
        await this._player.Connection.SendTradeRequestAnswerAsync(
            tradeAccepted,
            this._player.TradingPartner?.Name ?? string.Empty,
            (ushort)(tradeAccepted ? this._player.TradingPartner?.Level ?? 0 : 0),
            tradeAccepted ? this._player.TradingPartner?.GuildStatus?.GuildId ?? 0 : 0).ConfigureAwait(false);
    }
}