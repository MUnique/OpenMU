// <copyright file="WorldView.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// The default implementation of the world view which is forwarding everything to the game client which specific data packets.
    /// </summary>
    public class WorldView : IWorldView
    {
        private readonly IConnection connection;

        private readonly Player player;

        private readonly IItemSerializer itemSerializer;

        private readonly IAppearanceSerializer appearanceSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldView"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="player">The player.</param>
        /// <param name="itemSerializer">The item serializer.</param>
        /// <param name="appearanceSerializer">The appearance serializer.</param>
        public WorldView(IConnection connection, Player player, IItemSerializer itemSerializer, IAppearanceSerializer appearanceSerializer)
        {
            this.connection = connection;
            this.player = player;
            this.itemSerializer = itemSerializer;
            this.appearanceSerializer = appearanceSerializer;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ISupportWalk.NextDirections"/> should be send when an object moved.
        /// This is usually not required, because the game client calculates a proper path anyway and doesn't use the suggested path.
        /// </summary>
        public bool SendWalkDirections { get; set; }

        /// <inheritdoc/>
        public void ObjectGotKilled(IAttackable killed, IAttackable killer)
        {
            var killedId = killed.GetId(this.player);
            var killerId = killer.GetId(this.player);
            using (var writer = this.connection.StartSafeWrite(0xC1, 9))
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
                this.player.PlayerView.ShowMessage($"You got killed by {killerPlayer.Name}", MessageType.BlueNormal);
            }
        }

        /// <inheritdoc/>
        public void ObjectMoved(ILocateable obj, MoveType type)
        {
            var objectId = obj.GetId(this.player);
            if (type == MoveType.Instant)
            {
                using (var writer = this.connection.StartSafeWrite(0xC1, 0x08))
                {
                    var packet = writer.Span;
                    packet[2] = (byte)PacketType.Teleport;
                    packet[3] = objectId.GetHighByte();
                    packet[4] = objectId.GetLowByte();
                    packet[5] = obj.X;
                    packet[6] = obj.Y;
                    writer.Commit();
                }
            }
            else
            {
                IList<Direction> steps = null;
                var x = obj.X;
                var y = obj.Y;
                Direction rotation = Direction.Undefined;
                if (obj is ISupportWalk supportWalk)
                {
                    if (this.SendWalkDirections)
                    {
                        steps = supportWalk.NextDirections.Select(d => d.Direction).ToList();

                        // The last one is the rotation
                        rotation = steps.LastOrDefault();
                        steps.RemoveAt(steps.Count - 1);
                    }

                    if (obj is IRotateable rotateable)
                    {
                        rotation = rotateable.Rotation.RotateLeft();
                    }

                    x = supportWalk.WalkTarget.X;
                    y = supportWalk.WalkTarget.Y;
                }

                var stepsSize = (steps?.Count / 2) + 2 ?? 1;
                using (var writer = this.connection.StartSafeWrite(0xC1, 7 + stepsSize))
                {
                    var walkPacket = writer.Span;
                    walkPacket[2] = (byte)PacketType.Walk;
                    walkPacket[3] = objectId.GetHighByte();
                    walkPacket[4] = objectId.GetLowByte();
                    walkPacket[5] = x;
                    walkPacket[6] = y;
                    walkPacket[7] = (byte)((steps?.Count ?? 0) | ((byte)rotation) << 4);
                    if (steps != null)
                    {
                        walkPacket[7] = (byte)((int)steps.First() << 4 | (int)steps.Count);
                        for (int i = 0; i < steps.Count; i += 2)
                        {
                            var index = 8 + (i / 2);
                            var firstStep = steps[i];
                            var secondStep = steps.Count > i + 2 ? steps[i + 2] : Direction.Undefined;
                            walkPacket[index] = (byte)((int)firstStep << 4 | (int)secondStep);
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

            int itemCount = droppedItems.Count();
            using (var writer = this.connection.StartSafeWrite(0xC2, 9 + (this.itemSerializer.NeededSpace * itemCount)))
            {
                var data = writer.Span;
                data[3] = 0x20;
                data[4] = (byte)itemCount;

                int i = 0;
                int startOffset = 5;
                foreach (var item in droppedItems)
                {
                    var startIndex = startOffset + ((this.itemSerializer.NeededSpace + itemHeaderSize) * i);
                    var itemBlock = data.Slice(startIndex, this.itemSerializer.NeededSpace + itemHeaderSize);
                    itemBlock[0] = item.Id.GetHighByte();
                    if (freshDrops)
                    {
                        data[0] |= freshDropFlag;
                    }

                    itemBlock[1] = item.Id.GetLowByte();
                    itemBlock[2] = item.X;
                    itemBlock[3] = item.Y;
                    this.itemSerializer.SerializeItem(data.Slice(startIndex + 4), item.Item);

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
            using (var writer = this.connection.StartSafeWrite(0xC2, 5 + (2 * count)))
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
        public void ShowAnimation(IIdentifiable animatingObj, byte animation, IIdentifiable targetObj, byte direction)
        {
            var animatingId = animatingObj.GetId(this.player);
            if (targetObj == null)
            {
                using (var writer = this.connection.StartSafeWrite(0xC1, 7))
                {
                    var packet = writer.Span;
                    packet[2] = 0x18;
                    packet[3] = animatingId.GetHighByte();
                    packet[4] = animatingId.GetLowByte();
                    packet[5] = direction;
                    packet[6] = animation;
                    writer.Commit();
                }
            }
            else
            {
                var targetId = targetObj.GetId(this.player);
                using (var writer = this.connection.StartSafeWrite(0xC1, 9))
                {
                    var packet = writer.Span;
                    packet[2] = 0x18;
                    packet[3] = animatingId.GetHighByte();
                    packet[4] = animatingId.GetLowByte();
                    packet[5] = direction;
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
            using (var writer = this.connection.StartSafeWrite(0xC3, 0x09))
            {
                var packet = writer.Span;
                packet[2] = 0x1C;
                packet[3] = 0x0F;
                packet[4] = 1;
                packet[5] = mapNumber.GetHighByte();
                packet[6] = mapNumber.GetLowByte();
                packet[7] = this.player.SelectedCharacter.PositionX;
                packet[8] = this.player.SelectedCharacter.PositionY;
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ObjectsOutOfScope(IEnumerable<IIdentifiable> objects)
        {
            var count = objects.Count();
            using (var writer = this.connection.StartSafeWrite(0xC1, 4 + (count * 2)))
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

            const int playerDataSize = 36;

            IList<Player> shopPlayers = null;
            IList<Player> guildPlayers = null;
            var newPlayerList = newPlayers.ToList();
            const int estimatedEffectsPerPlayer = 5;
            var estimatedSize = 5 + (newPlayerList.Count * (playerDataSize + estimatedEffectsPerPlayer)); // this should just be a rough number to optimize the capacity of the list
            using (var writer = this.connection.StartSafeWrite(0xC2, estimatedSize))
            {
                var packet = writer.Span;
                packet[3] = 0x12;
                packet[4] = (byte)newPlayerList.Count;
                var actualSize = 5;

                // var nameArray = new byte[10];
                foreach (var newPlayer in newPlayerList)
                {
                    var playerId = newPlayer.GetId(this.player);
                    var playerBlock = packet.Slice(actualSize);
                    playerBlock[0] = playerId.GetHighByte();
                    playerBlock[1] = playerId.GetLowByte();
                    playerBlock[2] = newPlayer.X;
                    playerBlock[3] = newPlayer.Y;
                    playerBlock.Slice(4, 21);
                    newPlayer.GetAppearanceData(this.appearanceSerializer).AsSpan().CopyTo(playerBlock.Slice(4, 21)); // 4 ... 21
                    playerBlock.Slice(22, 10).WriteString(newPlayer.SelectedCharacter.Name, Encoding.UTF8); // 22 ... 31
                    if (newPlayer.IsWalking)
                    {
                        playerBlock[32] = newPlayer.WalkTarget.X;
                        playerBlock[33] = newPlayer.WalkTarget.Y;
                    }
                    else
                    {
                        playerBlock[32] = newPlayer.X;
                        playerBlock[33] = newPlayer.Y;
                    }

                    playerBlock[34] = (byte)(((int)newPlayer.Rotation * 0x10) + GetStateValue(newPlayer.SelectedCharacter.State));
                    var activeEffects = newPlayer.MagicEffectList.GetVisibleEffects();
                    var effectCount = 0;
                    for (int e = activeEffects.Count - 1; e >= 0; e--)
                    {
                        playerBlock[36 + e] = activeEffects[e].Id;
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
                this.player.PlayerView.ShowShopsOfPlayers(shopPlayers);
            }

            if (guildPlayers != null)
            {
                this.player.PlayerView.GuildView.AssignPlayersToGuild(guildPlayers, true);
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
            using (var writer = this.connection.StartSafeWrite(0xC2, (newObjectList.Count * NpcDataSize) + 5))
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
                    npcBlock[4] = npc.X;
                    npcBlock[5] = npc.Y;
                    var supportWalk = npc as ISupportWalk;
                    if (supportWalk?.IsWalking ?? false)
                    {
                        npcBlock[6] = supportWalk.WalkTarget.X;
                        npcBlock[7] = supportWalk.WalkTarget.Y;
                    }
                    else
                    {
                        npcBlock[6] = npc.X;
                        npcBlock[7] = npc.Y;
                    }

                    npcBlock[8] = (byte)((int)npc.Rotation << 4);
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
            var skillId = NumberConversionExtensions.ToUnsigned(skill.SkillID);
            using (var writer = this.connection.StartSafeWrite(0xC3, 0x09))
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
        public void ShowAreaSkillAnimation(Player playerWhichPerformsSkill, Skill skill, byte x, byte y, byte rotation)
        {
            var skillId = NumberConversionExtensions.ToUnsigned(skill.SkillID);
            var playerId = playerWhichPerformsSkill.GetId(this.player);
            using (var writer = this.connection.StartSafeWrite(0xC3, 0x0A))
            {
                // Example: C3 0A 1E 00 09 23 47 3D 62 3A
                var packet = writer.Span;
                packet[2] = 0x1E;
                packet[3] = skillId.GetHighByte();
                packet[4] = skillId.GetLowByte();
                packet[5] = playerId.GetHighByte();
                packet[6] = playerId.GetLowByte();
                packet[7] = x;
                packet[8] = y;
                packet[9] = rotation;
                writer.Commit();
            }
        }

        private static byte GetStateValue(HeroState state)
        {
            switch (state)
            {
                case HeroState.New:
                    return 0;
                case HeroState.LightHero:
                    return 1;
                case HeroState.MediumHero:
                    return 2;
                case HeroState.Hero:
                    return 3;
                case HeroState.PlayerKillWarning:
                    return 4;
                case HeroState.PlayerKiller1stStage:
                    return 5;
                case HeroState.PlayerKiller2ndStage:
                    return 6;
            }

            return 0;
        }
    }
}
