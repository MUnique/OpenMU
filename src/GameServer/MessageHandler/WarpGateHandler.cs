// <copyright file="WarpGateHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System.Linq;
    using log4net;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handler for warp gate packets.
    /// This one is called when a player has entered a gate area, and sends a gate enter request.
    /// </summary>
    internal class WarpGateHandler : BasePacketHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WarpGateHandler));

        private readonly WarpGateAction warpAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="WarpGateHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public WarpGateHandler(IGameContext gameContext)
            : base(gameContext)
        {
            this.warpAction = new WarpGateAction();
        }

        /// <inheritdoc/>
        public override void HandlePacket(Player player, byte[] packet)
        {
            if (packet.Length < 8)
            {
                return;
            }

            ushort gateNr = NumberConversionExtensions.MakeWord(packet[4], packet[3]);
            EnterGate gate = player.SelectedCharacter.CurrentMap.Gates.FirstOrDefault(g => NumberConversionExtensions.ToUnsigned(g.Number) == gateNr);
            if (gate == null)
            {
                Logger.WarnFormat("Gate {0} not found in current map {1}", gateNr,  player.SelectedCharacter.CurrentMap);
                return;
            }

            this.warpAction.EnterGate(player, gate);
        }
    }
}
