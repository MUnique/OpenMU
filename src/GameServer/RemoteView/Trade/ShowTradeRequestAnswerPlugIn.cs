// <copyright file="ShowTradeRequestAnswerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Trade
{
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.GameLogic.Views.Trade;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowTradeRequestAnswerPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowTradeRequestAnswerPlugIn", "The default implementation of the IShowTradeRequestAnswerPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("243cbc67-7af3-48e2-9a56-d6e49c86b816")]
    public class ShowTradeRequestAnswerPlugIn : IShowTradeRequestAnswerPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowTradeRequestAnswerPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowTradeRequestAnswerPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowTradeRequestAnswer(bool tradeAccepted)
        {
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 0x14))
            {
                var packet = writer.Span;
                packet[2] = 0x37;
                packet.Slice(4).WriteString(this.player.TradingPartner.Name, Encoding.UTF8);

                if (tradeAccepted)
                {
                    packet[3] = 1; // accepted

                    var tradePartnerLevel = (ushort)this.player.TradingPartner.Level;
                    packet[14] = tradePartnerLevel.GetHighByte();
                    packet[15] = tradePartnerLevel.GetLowByte();

                    var guildId = this.player.TradingPartner.GuildStatus?.GuildId ?? 0;
                    packet.Slice(16).SetIntegerBigEndian(guildId);
                }

                writer.Commit();
            }
        }
    }
}