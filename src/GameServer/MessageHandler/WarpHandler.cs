// <copyright file="WarpHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using System.Linq;
    using GameLogic.Interfaces;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handler for warp request packets.
    /// This one is called when a player uses the warp list.
    /// </summary>
    internal class WarpHandler : BasePacketHandler
    {
        private readonly WarpAction warpAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="WarpHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public WarpHandler(IGameContext gameContext)
            : base(gameContext)
        {
            this.warpAction = new WarpAction();
        }

        /// <inheritdoc/>
        public override void HandlePacket(Player player, Span<byte> packet)
        {
            if (packet[3] != 2) ////is always 2 i guess?
            {
                return;
            }

            ushort warpInfoIndex = NumberConversionExtensions.MakeWord(packet[8], packet[9]);
            var warpInfo = this.GameContext.Configuration.WarpList?.FirstOrDefault(info => info.Index == warpInfoIndex);
            if (warpInfo != null)
            {
                this.warpAction.WarpTo(player, warpInfo);
            }
            else
            {
                player.PlayerView.ShowMessage($"Unknown warp index {warpInfoIndex}", MessageType.BlueNormal);
            }
        }
    }
}
