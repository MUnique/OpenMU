// <copyright file="HitHandlerPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using log4net;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handler for hit packets.
    /// </summary>
    internal abstract class HitHandlerPlugInBase : IPacketHandlerPlugIn
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(HitHandlerPlugIn));

        private readonly HitAction hitAction = new HitAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public abstract byte Key { get; }

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            if (packet.Length < 7)
            {
                return;
            }

            ushort id = NumberConversionExtensions.MakeWord(packet[4], packet[3]);
            var attackAnimation = packet[5];
            var lookingDirection = packet[6].ParseAsDirection();
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