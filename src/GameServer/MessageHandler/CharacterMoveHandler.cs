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
            player.NextDirections.Clear();
            if (moveType == MoveType.Walk)
            {
                this.Walk(player, packet, x, y);
            }
            else
            {
                player.Move(x, y, MoveType.Instant);
            }
        }

        private void Walk(Player player, Span<byte> packet, byte x, byte y)
        {
            if (packet.Length > 6)
            {
                // in a walk packet, x and y are the current coordinates and the steps are leading us to the target
                var sourcePoint = new Point(x, y);
                var steps = this.GetSteps(sourcePoint, this.GetDirections(packet));
                Point target = this.ApplySteps(player, steps, sourcePoint);

                player.WalkTarget = target;

                player.Move(player.WalkTarget.X, player.WalkTarget.Y, MoveType.Walk);
            }
            else
            {
                player.Rotation = (Direction)((packet[5] >> 4) & 0x0F);
            }
        }

        private Point ApplySteps(Player player, Span<WalkingStep> steps, Point target)
        {
            // we need to reverse the steps, because we put it on a stack - where the top element is the next step.
            for (int i = steps.Length - 1; i >= 0; i--)
            {
                var step = steps[i];
                if (player.NextDirections.Count == 0)
                {
                    // the first direction (which will end up at the bottom of the stack) is our target
                    target = step.To;
                }

                player.NextDirections.Push(step);
            }

            return target;
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
            var firstDirection = (Direction)((packet[5] >> 4) & 0x0F);
            result[0] = firstDirection.RotateLeft();

            int i;
            for (i = 1; i < count; i++)
            {
                var index = 5 + (i / 2);
                var direction = (Direction)((packet[index] >> ((i % 2) == 0 ? 0 : 4)) & 0x0F);
                if (direction == Direction.Undefined)
                {
                    break;
                }

                result[i] = direction.RotateLeft();
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
