// <copyright file="ConsumeItemHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items
{
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

    /// <summary>
    /// Handler for item consume packets.
    /// </summary>
    internal class ConsumeItemHandler : IPacketHandler
    {
        private readonly ItemConsumeAction consumeAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumeItemHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public ConsumeItemHandler(IGameContext gameContext)
        {
            this.consumeAction = new ItemConsumeAction(gameContext);
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, byte[] packet)
        {
            byte invPos = packet[3];
            byte invTarget = packet[4];
            //// byte itemUseType = packet[5]; ////Dont know for what its used yet

            this.consumeAction.HandleConsumeRequest(player, invPos, invTarget);
        }
    }
}
