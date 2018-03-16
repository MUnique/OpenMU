// <copyright file="CharacterMoveHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System.Collections.Generic;
    using System.Linq;
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
                    // in a walk packet, x and y are the current coordinates and the steps are leading us to the target
                    var sourcePoint = new Point(x, y);
                    var steps = this.GetSteps(sourcePoint, this.GetDirections(packet));
                    Point target = sourcePoint;

                    // we need to reverse the steps, because we put it on a stack - where the top element is the next step.
                    foreach (var step in steps.Reverse())
                    {
                        if (player.NextDirections.Count == 0)
                        {
                            // the first direction (which will end up at the bottom of the stack) is our target
                            target = step.To;
                        }

                        player.NextDirections.Push(step);
                    }

                    player.WalkTarget = target;

                    player.Move(player.WalkTarget.X, player.WalkTarget.Y, MoveType.Walk);
                }
                else
                {
                    player.Move(x, y, MoveType.Instant);
                }
            }
        }

        private IEnumerable<WalkingStep> GetSteps(Point start, IEnumerable<Direction> directions)
        {
            Point currentTarget;

            Point previousTarget = start;
            yield return new WalkingStep { Direction = directions.First(), To = start };

            foreach (var direction in directions)
            {
                currentTarget = previousTarget.CalculateTargetPoint(direction);
                yield return new WalkingStep { Direction = direction, To = currentTarget, From = previousTarget };
                previousTarget = currentTarget;
            }
        }

        /// <summary>
        /// Gets the walking directions from the walk packet.
        /// </summary>
        /// <param name="packet">The walk packet.</param>
        /// <returns>The walking directions.</returns>
        /// <remarks>
        /// We return here the directions left-rotated; I don't know yet if that's an error in our Direction-enum
        /// or just the client uses another enumeration for it.
        /// </remarks>
        private IEnumerable<Direction> GetDirections(byte[] packet)
        {
            // the first 4 bits of the first path byte contains the number of steps
            var count = packet[5] & 0x0F;
            var firstDirection = (Direction)((packet[5] >> 4) & 0x0F);
            yield return firstDirection.RotateLeft();
            for (int i = 1; i < count; i++)
            {
                var index = 5 + (i / 2);
                var direction = (Direction)((packet[index] >> ((i % 2) == 0 ? 0 : 4)) & 0x0F);
                if (direction == Direction.Undefined)
                {
                    yield break;
                }

                yield return direction.RotateLeft();
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
