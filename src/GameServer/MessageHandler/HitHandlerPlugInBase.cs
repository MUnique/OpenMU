// <copyright file="HitHandlerPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.Network.Packets.ClientToServer;

    /// <summary>
    /// Handler for hit packets.
    /// </summary>
    internal abstract class HitHandlerPlugInBase : IPacketHandlerPlugIn
    {
        private readonly HitAction hitAction = new ();

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

            using var loggerScope = player.Logger.BeginScope(this.GetType());
            HitRequest message = packet;
            var currentMap = player.CurrentMap;
            if (currentMap is null)
            {
                player.Logger.LogWarning($"Current player map not set. Possible hacker action. Character name: {player.Name}");
                return;
            }

            if (currentMap.GetObject(message.TargetId) is not IAttackable target)
            {
                player.Logger.LogWarning($"Object {message.TargetId} of current player map not found alive. Possible hacker action. Character name: {player.Name}");
            }
            else
            {
                this.hitAction.Hit(player, target, message.AttackAnimation, message.LookingDirection.ParseAsDirection());
            }
        }
    }
}