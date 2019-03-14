﻿// <copyright file="WorldView.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Pathfinding;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the world view which is forwarding everything to the game client which specific data packets.
    /// </summary>
    [PlugIn("World View PlugIn", "View Plugin to send world update messages to the player by sending network packets")]
    [Guid("043B3145-7237-4809-B6EF-997675975664")]
    public class WorldView : IWorldView
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldView"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public WorldView(RemotePlayer player)
        {
            this.player = player;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the directions provided by <see cref="ISupportWalk.GetDirections"/> should be send when an object moved.
        /// This is usually not required, because the game client calculates a proper path anyway and doesn't use the suggested path.
        /// </summary>
        public bool SendWalkDirections { get; set; }

        private IConnection Connection => this.player.Connection;

        /// <inheritdoc/>
        public void ObjectGotKilled(IAttackable killed, IAttackable killer)
        {
            var killedId = killed.GetId(this.player);
            var killerId = killer.GetId(this.player);
            using (var writer = this.Connection.StartSafeWrite(0xC1, 9))
            {
                var packet = writer.Span;
                packet[2] = 0x17;
                packet[3] = killedId.GetHighByte();
                packet[4] = killedId.GetLowByte();
                packet[7] = killerId.GetHighByte();
                packet[8] = killerId.GetLowByte();
                writer.Commit();
            }

            if (this.player == killed && killer is Player killerPlayer)
            {
                this.player.ViewPlugIns.GetPlugIn<IPlayerView>()?.ShowMessage($"You got killed by {killerPlayer.Name}", MessageType.BlueNormal);
            }
        }

        /// <inheritdoc/>
        public void ObjectMoved(ILocateable obj, MoveType type)
        {
            var objectId = obj.GetId(this.player);
            if (type == MoveType.Instant)
            {
                using (var writer = this.Connection.StartSafeWrite(0xC1, 0x08))
                {
                    var packet = writer.Span;
                    packet[2] = (byte)PacketType.Teleport;
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
                using (var writer = this.Connection.StartSafeWrite(0xC1, 7 + stepsSize))
                {
                    var walkPacket = writer.Span;
                    walkPacket[2] = (byte)PacketType.Walk;
                    walkPacket[3] = objectId.GetHighByte();
                    walkPacket[4] = objectId.GetLowByte();
                    walkPacket[5] = point.X;
                    walkPacket[6] = point.Y;
                    walkPacket[7] = (byte)(stepsLength | rotation.ToPacketByte() << 4);
                    if (steps != null)
                    {
                        walkPacket[7] = (byte)(steps[0].ToPacketByte() << 4 | stepsSize);
                        for (int i = 0; i < stepsSize; i += 2)
                        {
                            var index = 8 + (i / 2);
                            var firstStep = steps[i].ToPacketByte();
                            var secondStep = stepsSize > i + 2 ? steps[i + 2].ToPacketByte() : 0;
                            walkPacket[index] = (byte)(firstStep << 4 | secondStep);
                        }
                    }

                    writer.Commit();
                }
            }
        }

        /// <inheritdoc/>
        public void ShowDroppedItems(IEnumerable<DroppedItem> droppedItems, bool freshDrops)
        {
            const int itemHeaderSize = 4; ////Id + Coordinates
            const byte freshDropFlag = 0x80;
            var itemSerializer = this.player.ItemSerializer;

            int itemCount = droppedItems.Count();
            using (var writer = this.Connection.StartSafeWrite(0xC2, 9 + (itemSerializer.NeededSpace * itemCount)))
            {
                var data = writer.Span;
                data[3] = 0x20;
                data[4] = (byte)itemCount;

                int i = 0;
                int startOffset = 5;
                foreach (var item in droppedItems)
                {
                    var startIndex = startOffset + ((itemSerializer.NeededSpace + itemHeaderSize) * i);
                    var itemBlock = data.Slice(startIndex, itemSerializer.NeededSpace + itemHeaderSize);
                    itemBlock[0] = item.Id.GetHighByte();
                    if (freshDrops)
                    {
                        data[0] |= freshDropFlag;
                    }

                    itemBlock[1] = item.Id.GetLowByte();
                    itemBlock[2] = item.Position.X;
                    itemBlock[3] = item.Position.Y;
                    itemSerializer.SerializeItem(data.Slice(startIndex + 4), item.Item);

                    i++;
                }

                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void DroppedItemsDisappeared(IEnumerable<ushort> disappearedItemIds)
        {
            ////C2 00 07 21 01 00 0C
            int count = disappearedItemIds.Count();
            using (var writer = this.Connection.StartSafeWrite(0xC2, 5 + (2 * count)))
            {
                var data = writer.Span;
                data[3] = 0x21;
                data[4] = (byte)count;
                int i = 0;
                foreach (var dropId in disappearedItemIds)
                {
                    data[5 + (i * 2)] = (byte)((dropId >> 8) & 0xFF);
                    data[6 + (i * 2)] = (byte)(dropId & 0xFF);
                    i++;
                }

                writer.Commit();
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This Packet is sent to the Server when an Object does an animation, including attacking other players.
        /// It will create the animation at the client side.
        /// </remarks>
        public void ShowAnimation(IIdentifiable animatingObj, byte animation, IIdentifiable targetObj, Direction direction)
        {
            var animatingId = animatingObj.GetId(this.player);
            if (targetObj == null)
            {
                using (var writer = this.Connection.StartSafeWrite(0xC1, 7))
                {
                    var packet = writer.Span;
                    packet[2] = 0x18;
                    packet[3] = animatingId.GetHighByte();
                    packet[4] = animatingId.GetLowByte();
                    packet[5] = direction.ToPacketByte();
                    packet[6] = animation;
                    writer.Commit();
                }
            }
            else
            {
                var targetId = targetObj.GetId(this.player);
                using (var writer = this.Connection.StartSafeWrite(0xC1, 9))
                {
                    var packet = writer.Span;
                    packet[2] = 0x18;
                    packet[3] = animatingId.GetHighByte();
                    packet[4] = animatingId.GetLowByte();
                    packet[5] = direction.ToPacketByte();
                    packet[6] = animation;
                    packet[7] = targetId.GetHighByte();
                    packet[8] = targetId.GetLowByte();
                    writer.Commit();
                }
            }
        }

        /// <inheritdoc/>
        public void MapChange()
        {
            var mapNumber = NumberConversionExtensions.ToUnsigned(this.player.SelectedCharacter.CurrentMap.Number);
            using (var writer = this.Connection.StartSafeWrite(0xC3, 0x0F))
            {
                var packet = writer.Span;
                packet[2] = 0x1C;
                packet[3] = 0x0F;
                packet[4] = 1;
                packet.Slice(5).SetShortSmallEndian(mapNumber);
                var position = this.player.IsWalking ? this.player.WalkTarget : this.player.Position;
                packet[7] = position.X;
                packet[8] = position.Y;
                packet[9] = this.player.Rotation.ToPacketByte();
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ObjectsOutOfScope(IEnumerable<IIdentifiable> objects)
        {
            var count = objects.Count();
            using (var writer = this.Connection.StartSafeWrite(0xC1, 4 + (count * 2)))
            {
                var packet = writer.Span;
                packet[2] = 20;
                packet[3] = (byte)count;
                int i = 4;
                foreach (var m in objects)
                {
                    var objectId = m.GetId(this.player);
                    packet[i] = objectId.GetHighByte();
                    packet[i + 1] = objectId.GetLowByte();
                    i += 2;
                }

                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void NewPlayersInScope(IEnumerable<Player> newPlayers)
        {
            if (newPlayers == null || !newPlayers.Any())
            {
                return;
            }

            var appearanceSerializer = this.player.AppearanceSerializer;
            var playerDataSize = appearanceSerializer.NeededSpace + 18;

            IList<Player> shopPlayers = null;
            IList<Player> guildPlayers = null;
            var newPlayerList = newPlayers.ToList();
            const int estimatedEffectsPerPlayer = 5;
            var estimatedSize = 5 + (newPlayerList.Count * (playerDataSize + estimatedEffectsPerPlayer)); // this should just be a rough number to optimize the capacity of the list
            using (var writer = this.Connection.StartSafeWrite(0xC2, estimatedSize))
            {
                var packet = writer.Span;
                packet[3] = 0x12;
                packet[4] = (byte)newPlayerList.Count;
                var actualSize = 5;

                foreach (var newPlayer in newPlayerList)
                {
                    var playerId = newPlayer.GetId(this.player);
                    var playerBlock = packet.Slice(actualSize);
                    playerBlock[0] = playerId.GetHighByte();
                    playerBlock[1] = playerId.GetLowByte();
                    playerBlock[2] = newPlayer.Position.X;
                    playerBlock[3] = newPlayer.Position.Y;
                    playerBlock.Slice(4, 21);
                    appearanceSerializer.WriteAppearanceData(playerBlock.Slice(4, appearanceSerializer.NeededSpace), newPlayer.AppearanceData, true); // 4 ... 21
                    playerBlock.Slice(22, 10).WriteString(newPlayer.SelectedCharacter.Name, Encoding.UTF8); // 22 ... 31
                    if (newPlayer.IsWalking)
                    {
                        playerBlock[32] = newPlayer.WalkTarget.X;
                        playerBlock[33] = newPlayer.WalkTarget.Y;
                    }
                    else
                    {
                        playerBlock[32] = newPlayer.Position.X;
                        playerBlock[33] = newPlayer.Position.Y;
                    }

                    playerBlock[34] = (byte)((newPlayer.Rotation.ToPacketByte() * 0x10) + newPlayer.SelectedCharacter.State);
                    var activeEffects = newPlayer.MagicEffectList.GetVisibleEffects();
                    var effectCount = 0;
                    for (int e = activeEffects.Count - 1; e >= 0; e--)
                    {
                        playerBlock[36 + e] = (byte)activeEffects[e].Id;
                        effectCount++;
                    }

                    playerBlock[35] = (byte)effectCount;
                    actualSize += playerDataSize + effectCount;

                    if (newPlayer.ShopStorage.StoreOpen)
                    {
                        (shopPlayers ?? (shopPlayers = new List<Player>())).Add(newPlayer);
                    }

                    if (newPlayer.GuildStatus != null)
                    {
                        (guildPlayers ?? (guildPlayers = new List<Player>())).Add(newPlayer);
                    }
                }

                packet.Slice(0, actualSize).SetPacketSize();
                writer.Commit(actualSize);
            }

            if (shopPlayers != null)
            {
                this.player.ViewPlugIns.GetPlugIn<IPlayerView>()?.ShowShopsOfPlayers(shopPlayers);
            }

            if (guildPlayers != null)
            {
                this.player.ViewPlugIns.GetPlugIn<IGuildView>()?.AssignPlayersToGuild(guildPlayers, true);
            }
        }

        /// <inheritdoc/>
        public void NewNpcsInScope(IEnumerable<NonPlayerCharacter> newObjects)
        {
            const int NpcDataSize = 10;

            if (newObjects == null || !newObjects.Any())
            {
                return;
            }

            var newObjectList = newObjects.ToList();
            using (var writer = this.Connection.StartSafeWrite(0xC2, (newObjectList.Count * NpcDataSize) + 5))
            {
                var packet = writer.Span;
                packet[3] = 0x13; ////Packet Id
                packet[4] = (byte)newObjectList.Count;
                int i = 0;
                foreach (var npc in newObjectList)
                {
                    var npcBlock = packet.Slice(5 + (i * NpcDataSize));
                    ////Npc Id:
                    npcBlock[0] = npc.Id.GetHighByte();
                    npcBlock[1] = npc.Id.GetLowByte();

                    ////Npc Type:
                    var npcStats = npc.Definition;
                    if (npcStats != null)
                    {
                        npcBlock[2] = (byte)((npcStats.Number >> 8) & 0xFF);
                        npcBlock[3] = (byte)(npcStats.Number & 0xFF);
                    }

                    ////Coords:
                    npcBlock[4] = npc.Position.X;
                    npcBlock[5] = npc.Position.Y;
                    var supportWalk = npc as ISupportWalk;
                    if (supportWalk?.IsWalking ?? false)
                    {
                        npcBlock[6] = supportWalk.WalkTarget.X;
                        npcBlock[7] = supportWalk.WalkTarget.Y;
                    }
                    else
                    {
                        npcBlock[6] = npc.Position.X;
                        npcBlock[7] = npc.Position.Y;
                    }

                    npcBlock[8] = (byte)(npc.Rotation.ToPacketByte() << 4);
                    ////9 = offset byte for magic effects - currently we don't show them for NPCs
                    i++;
                }

                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void UpdateRotation()
        {
            //// TODO: Implement Rotation, packet: { 0xc1, 0x04, 0x0F, 0x12 }
        }

        /// <inheritdoc/>
        public void ShowSkillAnimation(Player attackingPlayer, IAttackable target, Skill skill)
        {
            var playerId = attackingPlayer.GetId(this.player);
            var targetId = target.GetId(this.player);
            var skillId = NumberConversionExtensions.ToUnsigned(skill.Number);
            using (var writer = this.Connection.StartSafeWrite(0xC3, 0x09))
            {
                var packet = writer.Span;
                packet[2] = 0x19;
                packet[3] = skillId.GetHighByte();
                packet[4] = skillId.GetLowByte();
                packet[5] = playerId.GetHighByte();
                packet[6] = playerId.GetLowByte();
                packet[7] = targetId.GetHighByte();
                packet[8] = targetId.GetLowByte();
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ShowAreaSkillAnimation(Player playerWhichPerformsSkill, Skill skill, Point point, byte rotation)
        {
            var skillId = NumberConversionExtensions.ToUnsigned(skill.Number);
            var playerId = playerWhichPerformsSkill.GetId(this.player);
            using (var writer = this.Connection.StartSafeWrite(0xC3, 0x0A))
            {
                // Example: C3 0A 1E 00 09 23 47 3D 62 3A
                var packet = writer.Span;
                packet[2] = 0x1E;
                packet[3] = skillId.GetHighByte();
                packet[4] = skillId.GetLowByte();
                packet[5] = playerId.GetHighByte();
                packet[6] = playerId.GetLowByte();
                packet[7] = point.X;
                packet[8] = point.Y;
                packet[9] = rotation;
                writer.Commit();
            }
        }
    }
}
