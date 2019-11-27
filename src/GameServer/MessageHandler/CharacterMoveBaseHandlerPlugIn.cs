// <copyright file="CharacterMoveBaseHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.Pathfinding;

    /// <summary>
    /// Abstract packet handler for move packets.
    /// </summary>
    internal abstract class CharacterMoveBaseHandlerPlugIn : IPacketHandlerPlugIn
    {
        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public abstract byte Key { get; }

        /// <summary>
        /// Gets the type of the move.
        /// </summary>
        /// <value>
        /// The type of the move.
        /// </value>
        public abstract MoveType MoveType { get; }

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            if (packet.Length <= 4)
            {
                return;
            }

            if (this.MoveType == MoveType.Walk)
            {
                WalkRequest request = packet;
                this.Walk(player, request, new Point(request.SourceX, request.SourceY));
            }
            else
            {
                // We don't move the player anymore by his request. This was usually requested after a player performed a skill.
                // However, it adds way for cheaters to move through the map.
                // So, we just allow it for developers when the debugger is attached.
                // When handling a skill which moves to the target, we'll handle the move on server-side, instead.
#if DEBUG
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    InstantMoveRequest moveRequest = packet;
                    player.Move(new Point(moveRequest.TargetX, moveRequest.TargetY));
                }
#endif
            }
        }

        private void Walk(Player player, WalkRequest request, Point sourcePoint)
        {
            if (request.Header.Length > 6)
            {
                // in a walk packet, x and y are the current coordinates and the steps are leading us to the target
                var steps = this.GetSteps(sourcePoint, this.GetDirections(request.Directions));
                Point target = this.GetTarget(steps, sourcePoint);

                player.WalkTo(target, steps);
            }
            else
            {
                var rotationValue = (byte)((request.Directions[0] >> 4) & 0x0F);
                player.Rotation = rotationValue.ParseAsDirection();
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
        /// <param name="binaryDirections">The walking directions as binary data.</param>
        /// <returns>The walking directions.</returns>
        /// <remarks>
        /// We return here the directions left-rotated; I don't know yet if that's an error in our Direction-enum
        /// or just the client uses another enumeration for it.
        /// </remarks>
        private Span<Direction> GetDirections(Span<byte> binaryDirections)
        {
            // the first 4 bits of the first path byte contains the number of steps
            var count = binaryDirections[0] & 0x0F;
            var result = new Direction[count];
            var firstDirectionValue = (byte)((binaryDirections[0] >> 4) & 0x0F);
            result[0] = firstDirectionValue.ParseAsDirection();

            int i;
            for (i = 1; i < count; i++)
            {
                var index = 0 + (i / 2);
                var directionValue = (byte)((binaryDirections[index] >> ((i % 2) == 0 ? 0 : 4)) & 0x0F);
                result[i] = directionValue.ParseAsDirection();
            }

            return result.AsSpan(0, i);
        }
    }
}
