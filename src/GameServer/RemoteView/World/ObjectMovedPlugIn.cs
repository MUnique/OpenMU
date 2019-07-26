// <copyright file="ObjectMovedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IObjectMovedPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ObjectMovedPlugIn", "The default implementation of the IObjectMovedPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("29ee689f-636c-47e7-a930-b60ce8e8993c")]
    public class ObjectMovedPlugIn : IObjectMovedPlugIn
    {
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
            if (type == MoveType.Instant)
            {
                using (var writer = this.player.Connection.StartSafeWrite(0xC1, 0x08))
                {
                    var packet = writer.Span;
                    packet[2] = this.GetInstantMoveCode();
                    packet[3] = objectId.GetHighByte();
                    packet[4] = objectId.GetLowByte();
                    packet[5] = obj.Position.X;
                    packet[6] = obj.Position.Y;
                    writer.Commit();
                }
            }
            else
            {
                Span<Direction> steps = this.SendWalkDirections ? stackalloc Direction[16] : null;
                var stepsLength = 0;
                var point = obj.Position;
                Direction rotation = Direction.Undefined;
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

                    if (obj is IRotatable rotatable)
                    {
                        rotation = rotatable.Rotation;
                    }

                    point = supportWalk.WalkTarget;
                }

                var stepsSize = steps == null ? 1 : (steps.Length / 2) + 2;
                using (var writer = this.player.Connection.StartSafeWrite(0xC1, 8 + stepsSize))
                {
                    var walkPacket = writer.Span;
                    walkPacket[2] = this.GetWalkCode();
                    walkPacket[3] = objectId.GetHighByte();
                    walkPacket[4] = objectId.GetLowByte();
                    walkPacket[5] = point.X;
                    walkPacket[6] = point.Y;
                    walkPacket[7] = (byte)(stepsLength | rotation.ToPacketByte() << 4);
                    if (steps != null && stepsLength > 0)
                    {
                        walkPacket[8] = (byte)(steps[0].ToPacketByte() << 4 | stepsSize);
                        for (int i = 0; i < stepsSize; i += 2)
                        {
                            var index = 9 + (i / 2);
                            var firstStep = steps[i].ToPacketByte();
                            var secondStep = steps.Length > i + 1 ? steps[i + 1].ToPacketByte() : 0;
                            walkPacket[index] = (byte)(firstStep << 4 | secondStep);
                        }
                    }

                    writer.Commit();
                }
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