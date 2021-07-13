// <copyright file="ObjectMovedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.Pathfinding;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IObjectMovedPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ObjectMovedPlugIn", "The default implementation of the IObjectMovedPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("29ee689f-636c-47e7-a930-b60ce8e8993c")]
    [MinimumClient(0,90, ClientLanguage.Invariant)]
    public class ObjectMovedPlugIn : IObjectMovedPlugIn
    {
        private const short TeleportTargetNumber = 0x0F;

        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectMovedPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ObjectMovedPlugIn(RemotePlayer player) => this.player = player;

        /// <summary>
        /// Gets or sets a value indicating whether the directions provided by <see cref="ISupportWalk.GetDirections"/> should be send when an object moved.
        /// This is usually not required, because the game client calculates a proper path anyway and doesn't use the suggested path.
        /// </summary>
        public bool SendWalkDirections { get; set; }

        /// <inheritdoc/>
        public void ObjectMoved(ILocateable obj, MoveType type)
        {
            var objectId = obj.GetId(this.player);
            switch (type)
            {
                case MoveType.Instant:
                    this.player.Connection?.SendObjectMoved(this.GetInstantMoveCode(), objectId, obj.Position.X, obj.Position.Y);
                    break;

                case MoveType.Teleport when obj is Player movedPlayer && movedPlayer != this.player:
                    this.player.ViewPlugIns.GetPlugIn<INewPlayersInScopePlugIn>()?.NewPlayersInScope(movedPlayer.GetAsEnumerable(), false);
                    this.player.ViewPlugIns.GetPlugIn<IShowSkillAnimationPlugIn>()?.ShowSkillAnimation(movedPlayer, movedPlayer, TeleportTargetNumber, true);
                    break;

                case MoveType.Teleport when obj is NonPlayerCharacter movedNpc:
                    this.player.ViewPlugIns.GetPlugIn<INewNpcsInScopePlugIn>()?.NewNpcsInScope(movedNpc.GetAsEnumerable(), false);
                    if (movedNpc is Monster monster)
                    {
                        this.player.ViewPlugIns.GetPlugIn<IShowSkillAnimationPlugIn>()?.ShowSkillAnimation(monster, monster, TeleportTargetNumber, true);
                    }

                    break;

                case MoveType.Teleport:
                    // no other types available
                    break;

                case MoveType.Walk:
                    this.ObjectWalked(obj);
                    break;
            }
        }

        /// <summary>
        /// Sends the network message.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="objectId">The object identifier.</param>
        /// <param name="targetPoint">The target point.</param>
        /// <param name="steps">The steps.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="stepsLength">Length of the steps.</param>
        protected virtual void SendMessage(IConnection connection, ushort objectId, Point targetPoint, Span<Direction> steps, Direction rotation, int stepsLength)
        {
            var stepsSize = steps == default ? 1 : (steps.Length / 2) + 2;
            using var writer = connection.StartSafeWrite(
                Network.Packets.ServerToClient.ObjectWalked.HeaderType,
                Network.Packets.ServerToClient.ObjectWalked.GetRequiredSize(stepsSize));
            var walkPacket = new ObjectWalked(writer.Span)
            {
                HeaderCode = this.GetWalkCode(),
                ObjectId = objectId,
                TargetX = targetPoint.X,
                TargetY = targetPoint.Y,
                TargetRotation = rotation.ToPacketByte(),
                StepCount = (byte)stepsLength,
            };

            this.SetStepData(walkPacket, steps, stepsSize);

            writer.Commit();
        }

        private void ObjectWalked(ILocateable obj)
        {
            var connection = this.player.Connection;
            if (connection is null)
            {
                return;
            }

            var objectId = obj.GetId(this.player);
            Span<Direction> steps = this.SendWalkDirections ? stackalloc Direction[16] : default;
            var stepsLength = 0;
            Point targetPoint;
            var rotation = Direction.Undefined;
            if (obj is IRotatable rotatable)
            {
                rotation = rotatable.Rotation;
            }

            if (obj is ISupportWalk supportWalk)
            {
                if (this.SendWalkDirections)
                {
                    stepsLength = supportWalk.GetDirections(steps);
                    if (stepsLength > 0)
                    {
                        // The last one is the rotation
                        rotation = steps[stepsLength - 1];
                        steps = steps.Slice(0, stepsLength - 1);
                        stepsLength--;
                    }
                }

                targetPoint = supportWalk.WalkTarget;
            }
            else
            {
                targetPoint = obj.Position;
            }

            this.SendMessage(connection, objectId, targetPoint, steps, rotation, stepsLength);
        }

        private void SetStepData(ObjectWalked walkPacket, Span<Direction> steps, int stepsSize)
        {
            if (steps == default || walkPacket.StepCount == 0)
            {
                return;
            }

            walkPacket.StepData[0] = (byte)(steps[0].ToPacketByte() << 4 | stepsSize);
            for (int i = 0; i < stepsSize; i += 2)
            {
                var index = 1 + (i / 2);
                var firstStep = steps[i].ToPacketByte();
                var secondStep = steps.Length > i + 1 ? steps[i + 1].ToPacketByte() : 0;
                walkPacket.StepData[index] = (byte)(firstStep << 4 | secondStep);
            }
        }

        private byte GetInstantMoveCode()
        {
            if (this.player.ClientVersion.Season == 0 && this.player.ClientVersion.Episode <= 80)
            {
                return 0x11;
            }

            switch (this.player.ClientVersion.Language)
            {
                case ClientLanguage.Japanese: return 0xDC;
                case ClientLanguage.English:
                case ClientLanguage.Vietnamese:
                    return 0x15;
                case ClientLanguage.Filipino: return 0xD6;
                case ClientLanguage.Chinese:
                case ClientLanguage.Korean: return 0xD7;
                case ClientLanguage.Thai: return 0xD9;
                default:
                    return (byte)PacketType.Teleport;
            }
        }

        private byte GetWalkCode()
        {
            if (this.player.ClientVersion.Season == 0 && this.player.ClientVersion.Episode <= 80)
            {
                return 0x10;
            }

            switch (this.player.ClientVersion.Language)
            {
                case ClientLanguage.English: return 0xD4;
                case ClientLanguage.Japanese: return 0x1D;
                case ClientLanguage.Chinese:
                case ClientLanguage.Vietnamese:
                    return 0xD9;
                case ClientLanguage.Filipino: return 0xDD;
                case ClientLanguage.Korean: return 0xD3;
                case ClientLanguage.Thai: return 0xD7;
                default:
                    return (byte)PacketType.Walk;
            }
        }
    }
}