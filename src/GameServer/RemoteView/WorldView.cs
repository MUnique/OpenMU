// <copyright file="WorldView.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
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
            this.connection.Send(new byte[] { 0xC1, 9, 0x17, killedId.GetHighByte(), killedId.GetLowByte(), 0, 0, killerId.GetHighByte(), killerId.GetLowByte() });
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
                this.connection.Send(new byte[] { 0xC1, 0x08, (byte)PacketType.Teleport, objectId.GetHighByte(), objectId.GetLowByte(), obj.X, obj.Y, 0 });
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
                byte[] walkPacket = new byte[7 + stepsSize];
                walkPacket.SetValues<byte>(0xC1, (byte)walkPacket.Length, (byte)PacketType.Walk, objectId.GetHighByte(), objectId.GetLowByte(), x, y, (byte)((steps?.Count ?? 0) | ((byte)rotation) << 4));
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

                this.connection.Send(walkPacket);
            }
        }

        /// <inheritdoc/>
        public void ShowDroppedItems(IEnumerable<DroppedItem> droppedItems, bool freshDrops)
        {
            const int itemHeaderSize = 4; ////Id + Coordinates
            const byte freshDropFlag = 0x80;

            int itemCount = droppedItems.Count();
            byte[] data = new byte[9 + (this.itemSerializer.NeededSpace * itemCount)];
            ushort length = (ushort)data.Length;
            data.SetValues<byte>(0xC2, length.GetHighByte(), length.GetLowByte(), 0x20, (byte)itemCount);
            int i = 0;
            int startOffset = 5;
            foreach (var item in droppedItems)
            {
                var startIndex = startOffset + ((this.itemSerializer.NeededSpace + itemHeaderSize) * i);

                data[startIndex] = item.Id.GetHighByte();
                if (freshDrops)
                {
                    data[startIndex] |= freshDropFlag;
                }

                data[startIndex + 1] = item.Id.GetLowByte();
                data[startIndex + 2] = item.X;
                data[startIndex + 3] = item.Y;
                this.itemSerializer.SerializeItem(data, startIndex + 4, item.Item);

                i++;
            }

            this.connection.Send(data);
        }

        /// <inheritdoc/>
        public void DroppedItemsDisappeared(IEnumerable<ushort> disappearedItemIds)
        {
            ////C2 00 07 21 01 00 0C
            int count = disappearedItemIds.Count();
            var data = new byte[5 + (2 * count)];
            data[0] = 0xC2;
            data[1] = (byte)((data.Length >> 8) & 0xFF);
            data[2] = (byte)(data.Length & 0xFF);
            data[3] = 0x21;
            data[4] = (byte)count;
            int i = 0;
            foreach (var dropId in disappearedItemIds)
            {
                data[5 + (i * 2)] = (byte)((dropId >> 8) & 0xFF);
                data[6 + (i * 2)] = (byte)(dropId & 0xFF);
                i++;
            }

            this.connection.Send(data);
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
                this.connection.Send(new byte[] { 0xC1, 0x07, 0x18, animatingId.GetHighByte(), animatingId.GetLowByte(), direction, animation });
            }
            else
            {
                var targetId = targetObj.GetId(this.player);
                this.connection.Send(new byte[] { 0xC1, 0x09, 0x18, animatingId.GetHighByte(), animatingId.GetLowByte(), direction, animation, targetId.GetHighByte(), targetId.GetLowByte() });
            }
        }

        /// <inheritdoc/>
        public void MapChange()
        {
            var map = this.player.SelectedCharacter.CurrentMap.Number;
            this.connection.Send(new byte[] { 0xC3, 0x09, 0x1C, 0x0F, 1, NumberConversionExtensions.ToUnsigned(map).GetHighByte(), NumberConversionExtensions.ToUnsigned(map).GetLowByte(), this.player.SelectedCharacter.PositionX, this.player.SelectedCharacter.PositionY });
        }

        /// <inheritdoc/>
        public void ObjectsOutOfScope(IEnumerable<IIdentifiable> objects)
        {
            var count = objects.Count();
            var packet = new byte[4 + (count * 2)];
            packet[0] = 0xC1;
            packet[1] = (byte)packet.Length;
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

            this.connection.Send(packet);
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
            var estimatedEffectsPerPlayer = 3;
            var estimatedSize = 5 + (newPlayerList.Count * (playerDataSize + estimatedEffectsPerPlayer)); // this should just be a rough number to optimize the capacity of the list
            var packetList = new List<byte>(estimatedSize) { 0xC2, 0, 0, 0x12, (byte)newPlayerList.Count };

            var nameArray = new byte[10];
            foreach (var newPlayer in newPlayerList)
            {
                var playerId = newPlayer.GetId(this.player);
                packetList.Add(playerId.GetHighByte());
                packetList.Add(playerId.GetLowByte());
                packetList.Add(newPlayer.X); // 7
                packetList.Add(newPlayer.Y); // 8
                packetList.AddRange(newPlayer.GetAppearanceData(this.appearanceSerializer)); // 9 ... 26
                Encoding.ASCII.GetBytes(newPlayer.SelectedCharacter.Name, 0, newPlayer.SelectedCharacter.Name.Length, nameArray, 0);
                packetList.AddRange(nameArray); // 27 ... 36

                if (newPlayer.IsWalking)
                {
                    packetList.Add(newPlayer.WalkTarget.X); // 37
                    packetList.Add(newPlayer.WalkTarget.Y); // 38
                }
                else
                {
                    packetList.Add(newPlayer.X); // 37
                    packetList.Add(newPlayer.Y); // 38
                }

                packetList.Add((byte)(((int)newPlayer.Rotation * 0x10) + GetStateValue(newPlayer.SelectedCharacter.State))); // 39

                var activeEffects = newPlayer.MagicEffectList.GetVisibleEffects();
                var effectCountIndex = packetList.Count;
                var effectCount = 0;
                packetList.Add(0); // 40
                for (int e = activeEffects.Count - 1; e >= 0; e--)
                {
                    packetList.Add(activeEffects[e].Id);
                    effectCount++;
                }

                packetList[effectCountIndex] = (byte)effectCount;

                if (newPlayer.ShopStorage.StoreOpen)
                {
                    (shopPlayers ?? (shopPlayers = new List<Player>())).Add(newPlayer);
                }

                if (newPlayer.GuildStatus != null)
                {
                    (guildPlayers ?? (guildPlayers = new List<Player>())).Add(newPlayer);
                }
            }

            packetList[1] = (byte)(packetList.Count >> 8 & 0xFF);
            packetList[2] = (byte)(packetList.Count & 0xFF);
            this.connection.Send(packetList.ToArray());

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
            var packet = new byte[(newObjectList.Count * NpcDataSize) + 5];
            packet[0] = 0xC2;
            packet[1] = (byte)(packet.Length >> 8 & 0xFF);
            packet[2] = (byte)(packet.Length & 0xFF);
            packet[3] = 0x13; ////Packet Id
            packet[4] = (byte)newObjectList.Count;
            int i = 0;
            foreach (var npc in newObjectList)
            {
                var monsterOffset = i * NpcDataSize;
                ////Mob Id:
                packet[5 + monsterOffset] = npc.Id.GetHighByte();
                packet[6 + monsterOffset] = npc.Id.GetLowByte();

                ////Mob Type:
                var npcStats = npc.Definition;
                if (npcStats != null)
                {
                    packet[7 + monsterOffset] = (byte)((npcStats.Number >> 8) & 0xFF);
                    packet[8 + monsterOffset] = (byte)(npcStats.Number & 0xFF);
                }

                ////Coords:
                packet[9 + monsterOffset] = npc.X;
                packet[10 + monsterOffset] = npc.Y;
                var supportWalk = npc as ISupportWalk;
                if (supportWalk?.IsWalking ?? false)
                {
                    packet[11 + monsterOffset] = supportWalk.WalkTarget.X;
                    packet[12 + monsterOffset] = supportWalk.WalkTarget.Y;
                }
                else
                {
                    packet[11 + monsterOffset] = npc.X;
                    packet[12 + monsterOffset] = npc.Y;
                }

                packet[13 + monsterOffset] = (byte)((int)npc.Rotation << 4);
                ////14 = offset byte for magic effects - currently we don't show them for NPCs
                i++;
            }

            this.connection.Send(packet);
        }

        /// <inheritdoc/>
        public void UpdateRotation()
        {
            //// TODO: Implement Rotation, packet: new byte[] { 0xc1, 0x04, 0x0F, 0x12 }
        }

        /// <inheritdoc/>
        public void ShowSkillAnimation(Player attackingPlayer, IAttackable target, Skill skill)
        {
            var playerId = attackingPlayer.GetId(this.player);
            var targetId = target.GetId(this.player);
            var skillId = NumberConversionExtensions.ToUnsigned(skill.SkillID);
            this.connection.Send(new byte[]
            {
                0xC3, 9, 0x19,
                skillId.GetHighByte(), skillId.GetLowByte(),
                playerId.GetHighByte(), playerId.GetLowByte(),
                targetId.GetHighByte(), targetId.GetLowByte()
            });
        }

        /// <inheritdoc/>
        public void ShowAreaSkillAnimation(Player playerWhichPerformsSkill, Skill skill, byte x, byte y, byte rotation)
        {
            var skillId = NumberConversionExtensions.ToUnsigned(skill.SkillID);
            var playerId = playerWhichPerformsSkill.GetId(this.player);

            // Example: C3 0A 1E 00 09 23 47 3D 62 3A
            this.connection.Send(new byte[]
            {
                0xC3, 0x0A, 0x1E, skillId.GetHighByte(), skillId.GetLowByte(),
                playerId.GetHighByte(), playerId.GetLowByte(), x, y, rotation
            });
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
