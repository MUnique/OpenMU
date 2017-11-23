// <copyright file="BasePacketHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using MUnique.OpenMU.GameLogic;

    /// <summary>
    /// A base packet handler which knows its game context.
    /// </summary>
    internal abstract class BasePacketHandler : IPacketHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasePacketHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public BasePacketHandler(IGameContext gameContext)
        {
            this.GameContext = gameContext;
        }

        /// <summary>
        /// Gets the game context.
        /// </summary>
        protected IGameContext GameContext
        {
            get;
        }

        /// <inheritdoc/>
        public abstract void HandlePacket(Player player, byte[] packet);
    }
}
