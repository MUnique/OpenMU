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

        /// <inheritdoc/>
        public void ObjectGotKilled(IAttackable killed, IAttackable killer)
        {
            this.connection.Send(new byte[] { 0xC1, 9, 0x17, killed.Id.GetHighByte(), killed.Id.GetLowByte(), 0, 0, killer.Id.GetHighByte(), killer.Id.GetLowByte() });
            if (this.player.Id == killed.Id)
            {
                this.player.PlayerView.ShowMessage(string.Format("You got killed by {0}", killer.Id), MessageType.BlueNormal);
            }
        }

        /// <inheritdoc/>
        public void ObjectMoved(ILocateable obj, MoveType type)
        {
            if (type == MoveType.Instant)
            {
                this.connection.Send(new byte[] { 0xC1, 0x08, (byte)PacketType.Teleport, obj.Id.GetHighByte(), obj.Id.GetLowByte(), obj.X, obj.Y, 0 });
            }
            else
            {
                IList<Direction> steps = null;
                var supportWalk = obj as ISupportWalk;
                if (supportWalk != null)
                {
                    steps = supportWalk.NextDirections.Select(d => d.Direction).ToList();
                }

                var stepsSize = (steps?.Count / 2) + 1 ?? 1;
                byte[] walkPacket = new byte[7 + stepsSize];
                walkPacket.SetValues<byte>(0xC1, (byte)walkPacket.Length, (byte)PacketType.Walk, obj.Id.GetHighByte(), obj.Id.GetLowByte(), supportWalk?.WalkTarget.X ?? obj.X, supportWalk?.WalkTarget.Y ?? obj.Y);
                if (steps != null)
                {
                    for (int step = 0; step < steps.Count; step += 2)
                    {
                        var index = 7 + (step / 2);
                        var firstStep = steps[step];
                        var secondStep = steps.Count > step + 2 ? steps[step + 2] : Direction.Undefined;
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
            byte[] data = new byte[5 + (this.itemSerializer.NeededSpace * itemCount)];
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
            if (targetObj == null)
            {
                this.connection.Send(new byte[] { 0xC1, 0x07, 0x18, animatingObj.Id.GetLowByte(), animatingObj.Id.GetHighByte(), direction, animation });
            }
            else
            {
                this.connection.Send(new byte[] { 0xC1, 0x09, 0x18, animatingObj.Id.GetLowByte(), animatingObj.Id.GetHighByte(), direction, animation, targetObj.Id.GetLowByte(), targetObj.Id.GetHighByte() });
            }
        }

        /// <inheritdoc/>
        public void MapChange()
        {
            var map = this.player.SelectedCharacter.CurrentMap.Number;
            this.connection.Send(new byte[] { 0xC3, 0x09, 0x1C, 0x0F, 1, NumberConversionExtensions.ToUnsigned(map).GetLowByte(), NumberConversionExtensions.ToUnsigned(map).GetHighByte(), this.player.SelectedCharacter.PositionX, this.player.SelectedCharacter.PositionY });
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
                packet[i] = m.Id.GetHighByte();
                packet[i + 1] = m.Id.GetLowByte();
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
                packetList.Add((byte)((newPlayer.Id >> 8) & 0xFF));
                packetList.Add((byte)(newPlayer.Id & 0xFF));
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

                if (newPlayer.SelectedCharacter.GuildMemberInfo != null)
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

            if (newObjects == null && !newObjects.Any())
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
            foreach (var mob in newObjectList)
            {
                var monsterOffset = i * NpcDataSize;
                ////Mob Id:
                packet[5 + monsterOffset] = (byte)((mob.Id >> 8) & 0xFF);
                packet[6 + monsterOffset] = (byte)(mob.Id & 0xFF);

                ////Mob Type:
                var npcStats = mob.Definition;
                if (npcStats != null)
                {
                    packet[7 + monsterOffset] = (byte)((npcStats.Number >> 8) & 0xFF);
                    packet[8 + monsterOffset] = (byte)(npcStats.Number & 0xFF);
                }

                ////Coords:
                packet[9 + monsterOffset] = mob.X;
                packet[10 + monsterOffset] = mob.Y;
                var supportWalk = mob as ISupportWalk;
                if (supportWalk?.IsWalking ?? false)
                {
                    packet[11 + monsterOffset] = supportWalk.WalkTarget.X;
                    packet[12 + monsterOffset] = supportWalk.WalkTarget.Y;
                }
                else
                {
                    packet[11 + monsterOffset] = mob.X;
                    packet[12 + monsterOffset] = mob.Y;
                }

                packet[13 + monsterOffset] = (byte)((int)mob.Rotation << 4);
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
            this.connection.Send(new byte[]
            {
                0xC3, 9, 0x19,
                                     NumberConversionExtensions.ToUnsigned(skill.SkillID).GetHighByte(), NumberConversionExtensions.ToUnsigned(skill.SkillID).GetLowByte(),
                                     attackingPlayer.Id.GetHighByte(), attackingPlayer.Id.GetLowByte(),
                                     target.Id.GetHighByte(), target.Id.GetLowByte()
            });
        }

        /// <inheritdoc/>
        public void ShowAreaSkillAnimation(Player playerWhichPerformsSkill, Skill skill, byte x, byte y, byte rotation)
        {
            // C3 0A 1E 00 09 23 47 3D 62 3A
            this.connection.Send(new byte[]
            {
                0xC3, 0x0A, 0x1E, NumberConversionExtensions.ToUnsigned(skill.SkillID).GetLowByte(), NumberConversionExtensions.ToUnsigned(skill.SkillID).GetHighByte(),
                playerWhichPerformsSkill.Id.GetLowByte(), playerWhichPerformsSkill.Id.GetHighByte(), x, x, rotation
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
