// <copyright file="CharacterMoveHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Pathfinding;

    /// <summary>
    /// Packet handler for move packets.
    /// </summary>
    internal class CharacterMoveHandler : IPacketHandler
    {
        /// <inheritdoc/>
        public void HandlePacket(Player player, byte[] packet)
        {
            if (packet.Length > 4)
            {
                var moveType = this.GetMoveType(packet[2]);
                var x = packet[3];
                var y = packet[4];
                player.NextDirections.Clear();
                if (moveType == MoveType.Walk)
                {
                    var targetPoint = new Point(x, y);
                    Point currentTarget = targetPoint;

                    for (int i = packet.Length - 1; i > 4; i--)
                    {
                        Point previousTarget;
                        var steps = packet[i];
                        var first = (Direction)((steps & 0xF0) >> 4);
                        var second = (Direction)(steps & 0x0F);
                        if (second != Direction.Undefined)
                        {
                            previousTarget = currentTarget.CalculateTargetPoint(second.Negate());
                            player.NextDirections.Push(new WalkingStep { Direction = second, To = currentTarget, From = previousTarget });
                            currentTarget = previousTarget;
                        }

                        if (first != Direction.Undefined)
                        {
                            previousTarget = currentTarget.CalculateTargetPoint(first.Negate());
                            player.NextDirections.Push(new WalkingStep { Direction = first, To = currentTarget, From = previousTarget });
                            currentTarget = previousTarget;
                        }
                    }

                    player.WalkTarget = targetPoint;
                }

                player.Move(x, y, moveType);
            }
        }

        private MoveType GetMoveType(byte value)
        {
            switch (value)
            {
                case (byte)PacketType.Teleport: return MoveType.Instant;
                case (byte)PacketType.Walk: return MoveType.Walk;
            }

            return (MoveType)value;
        }
    }
}
