// <copyright file="CharacterMoveHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
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
        public void HandlePacket(Player player, Span<byte> packet)
        {
            if (packet.Length <= 4)
            {
                return;
            }

            var moveType = this.GetMoveType(packet[2]);

            var x = packet[3];
            var y = packet[4];
            if (moveType == MoveType.Walk)
            {
                this.Walk(player, packet, new Point(x, y));
            }
            else
            {
                    player.Move(new Point(x, y));
            }
        }

        private void Walk(Player player, Span<byte> packet, Point sourcePoint)
        {
            if (packet.Length > 6)
            {
                // in a walk packet, x and y are the current coordinates and the steps are leading us to the target
                var steps = this.GetSteps(sourcePoint, this.GetDirections(packet));
                Point target = this.GetTarget(steps, sourcePoint);

                player.WalkTo(target, steps);
            }
            else
            {
                player.Rotation = (Direction)(1 + (packet[5] >> 4) & 0x0F);
            }
        }

        private Point GetTarget(Span<WalkingStep> steps, Point source)
        {
            if (steps.Length > 0)
            {
                var step = steps[steps.Length - 1];
                return step.To;
            }

            return source;
        }

        private Span<WalkingStep> GetSteps(Point start, Span<Direction> directions)
        {
            var result = new WalkingStep[directions.Length];
            Point previousTarget = start;
            int i = 0;
            foreach (var direction in directions)
            {
                var currentTarget = previousTarget.CalculateTargetPoint(direction);
                result[i] = new WalkingStep { Direction = direction, To = currentTarget, From = previousTarget };
                i++;
                previousTarget = currentTarget;
            }

            return result;
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
        private Span<Direction> GetDirections(Span<byte> packet)
        {
            // the first 4 bits of the first path byte contains the number of steps
            var count = packet[5] & 0x0F;
            var result = new Direction[count];
            var firstDirection = (Direction)(1 + (packet[5] >> 4) & 0x0F);
            result[0] = firstDirection;

            int i;
            for (i = 1; i < count; i++)
            {
                var index = 5 + (i / 2);
                var direction = (Direction)(1 + (packet[index] >> ((i % 2) == 0 ? 0 : 4)) & 0x0F);
                if (direction == Direction.Undefined)
                {
                    break;
                }

                result[i] = direction;
            }

            return result.AsSpan(0, i);
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
