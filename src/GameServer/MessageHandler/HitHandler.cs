// <copyright file="HitHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using log4net;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handler for hit packets.
    /// </summary>
    internal class HitHandler : BasePacketHandler, IPacketHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(HitHandler));
        private readonly HitAction hitAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="HitHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public HitHandler(IGameContext gameContext)
            : base(gameContext)
        {
            this.hitAction = new HitAction();
        }

        /// <inheritdoc/>
        public override void HandlePacket(Player player, Span<byte> packet)
        {
            if (packet.Length < 7)
            {
                return;
            }

            ushort id = NumberConversionExtensions.MakeWord(packet[4], packet[3]);
            var attackAnimation = packet[5];
            var lookingDirection = (Direction)(packet[6] + 1);
            var currentMap = player.CurrentMap;
            if (currentMap == null)
            {
                Log.Warn($"Current player map not set. Possible hacker action. Character name: {player.Name}");
                return;
            }

            var target = currentMap.GetObject(id) as IAttackable;
            if (target == null)
            {
                Log.Warn($"Object {id} of current player map not found. Possible hacker action. Character name: {player.Name}");
            }
            else
            {
                this.hitAction.Hit(player, target, attackAnimation, lookingDirection);
            }
        }
    }
}
