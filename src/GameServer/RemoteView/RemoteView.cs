// <copyright file="RemoteView.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views;

    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Type of an attribute update.
    /// </summary>
    public enum UpdateType
    {
        /// <summary>
        /// The maximum value is updated.
        /// </summary>
        Maximum = 0xFE,

        /// <summary>
        /// The current value is updated.
        /// </summary>
        Current = 0xFF
    }

    /// <summary>
    /// The default implementation of the remote view which is forwarding everything to the game client which specific data packets.
    /// </summary>
    public class RemoteView : ChatView, IPlayerView
    {
        private readonly byte[] lowestClientVersion;

        private readonly IConnection connection;

        private readonly IGameServerContext context;

        private readonly Player player;

        private readonly IItemSerializer itemSerializer;

        private readonly IAppearanceSerializer appearanceSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteView"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="player">The playerWithClosedShop.</param>
        /// <param name="context">The context.</param>
        /// <param name="appearanceSerializer">The appearance serializer.</param>
        public RemoteView(IConnection connection, Player player, IGameServerContext context, IAppearanceSerializer appearanceSerializer)
            : base(connection)
        {
            this.context = context;
            this.itemSerializer = new ItemSerializer();
            this.connection = connection;
            this.player = player;
            this.PartyView = new PartyView(connection, player);
            this.MessengerView = new MessengerView(connection, player, context.FriendServer, appearanceSerializer);
            this.TradeView = new TradeView(connection, player, this.itemSerializer);
            this.GuildView = new GuildView(connection);
            this.WorldView = new WorldView(connection, player, this.itemSerializer, appearanceSerializer);
            this.InventoryView = new InventoryView(connection, player, this.itemSerializer);
            this.appearanceSerializer = appearanceSerializer;
            this.lowestClientVersion = this.GetLowestClientVersionOfConfiguration();
        }

        /// <summary>
        /// The color of the damage.
        /// </summary>
        public enum DamageColor : byte
        {
            /// <summary>
            /// The normal, red damage color.
            /// </summary>
            NormalRed = 0,

            /// <summary>
            /// The ignore defense, cyan damage color.
            /// </summary>
            IgnoreDefenseCyan = 1,

            /// <summary>
            /// The excellent, light green damage color.
            /// </summary>
            ExcellentLightGreen = 2,

            /// <summary>
            /// The critical, blue damage color.
            /// </summary>
            CriticalBlue = 3,

            /// <summary>
            /// The light pink damage color.
            /// </summary>
            LightPink = 4,

            /// <summary>
            /// The poison, dark green damage color.
            /// </summary>
            PoisonDarkGreen = 5,

            /// <summary>
            /// The dark pink damage color.
            /// </summary>
            DarkPink = 6,

            /// <summary>
            /// The white damage color.
            /// </summary>
            White = 7,
        }

        /// <summary>
        /// The special damage.
        /// </summary>
        public enum SpecialDamage : byte
        {
            /// <summary>
            /// The double damage.
            /// </summary>
            Double = 0x40,

            /// <summary>
            /// The triple damage.
            /// </summary>
            Triple = 0x80
        }

        /// <inheritdoc/>
        public IPartyView PartyView { get; }

        /// <inheritdoc/>
        public IMessengerView MessengerView { get; }

        /// <inheritdoc/>
        public ITradeView TradeView { get; }

        /// <inheritdoc/>
        public IGuildView GuildView { get; }

        /// <inheritdoc/>
        public IWorldView WorldView { get; }

        /// <inheritdoc/>
        public IInventoryView InventoryView { get; }

        /// <inheritdoc/>
        public void ShowCharacterList()
        {
            const int characterSize = 34;
            var packet = new byte[(this.player.Account.Characters.Count * characterSize) + 8];
            packet[0] = 0xC1;
            packet[1] = (byte)packet.Length;
            packet[2] = 0xF3;
            packet[3] = 0;
            byte maxClass = 0;
            packet[4] = this.player.Account.UnlockedCharacterClasses?.Select(c => c.CreationAllowedFlag).Aggregate(maxClass, (current, flag) => (byte)(current | flag)) ?? 0;
            packet[5] = 0; // MoveCnt
            packet[6] = (byte)this.player.Account.Characters.Count;

            // packet[7] ??? new in season 6 - probably vault extension
            int i = 0;
            foreach (var character in this.player.Account.Characters.OrderBy(c => c.CharacterSlot))
            {
                var offset = (i * characterSize) + 8;
                packet[offset + 0] = character.CharacterSlot;
                Encoding.UTF8.GetBytes(character.Name, 0, character.Name.Length, packet, offset + 1);
                packet[offset + 11] = 1; // unknown
                var level = (ushort)character.Attributes.First(s => s.Definition == Stats.Level).Value;
                packet[offset + 12] = level.GetLowByte();
                packet[offset + 13] = level.GetHighByte();
                packet[offset + 14] = (byte)character.State; // | 0x10 for item block?

                var preview = this.appearanceSerializer.GetAppearanceData(new CharacterAppearanceDataAdapter(character));
                Buffer.BlockCopy(preview, 0, packet, offset + 15, preview.Length);
                if (character.GuildMemberInfo != null)
                {
                    packet[offset + 15 + 18] = this.GetGuildMemberStatusCode(character.GuildMemberInfo.Status); ////not sure about the index yet...
                }

                i++;
            }

            this.connection.Send(packet);
        }

        /// <inheritdoc/>
        public void ShowCharacterCreationFailed()
        {
            this.connection.Send(new byte[] { 0xC1, 0x05, 0xF3, 0x01, 0x00 });
        }

        /// <inheritdoc/>
        public void ShowCreatedCharacter(Character character)
        {
            byte[] packet =
            {
                0xC1, 0x2A, 0xF3, 0x01,
                0x01, // success
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // name
                character.CharacterSlot,
                1, 0,
                (byte)(character.CharacterClass.Number << 3),
                this.GetPlayerStateCode(character.State),
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF // preview data?
            };

            Encoding.UTF8.GetBytes(character.Name, 0, character.Name.Length, packet, 5);
            this.connection.Send(packet);
        }

        /// <inheritdoc/>
        public void AppearanceChanged(Player changedPlayer)
        {
            //// var appearanceData = changedPlayer.GetAppearanceData(this.appearanceSerializer);

            // todo: find the right packet structure and send it to the client.
        }

        /// <inheritdoc/>
        public void AddSkill(Skill skill, int skillIndex)
        {
            byte[] packet = { 0xC1, 0x0A, 0xF3, 0x11, 0xFE, 0, 0, 0, 0, 0 };
            packet[6] = (byte)skillIndex;
            var unsignedSkillId = ShortExtensions.ToUnsigned(skill.SkillID);
            packet[7] = unsignedSkillId.GetLowByte();
            packet[8] = unsignedSkillId.GetHighByte();
            this.connection.Send(packet);
        }

        /// <inheritdoc/>
        public void DrinkAlcohol()
        {
            this.connection.Send(new byte[] { 0xC3, 6, 0x29, 0, 0x50, 0 });
        }

        /// <inheritdoc/>
        public void ShowCharacterDeleteResponse(CharacterDeleteResult result)
        {
            this.connection.Send(new byte[] { 0xC1, 0x05, 0xF3, 0x02, (byte)result });
        }

        /// <inheritdoc/>
        public void UpdateCurrentManaAndHp()
        {
            this.UpdateCurrentMana();
            this.UpdateCurrentHealth();
        }

        /// <inheritdoc/>
        public void UpdateCurrentMana()
        {
            // C1 08 27 FE 00 16 00 21
            var mana = (ushort)this.player.Attributes[Stats.CurrentMana];
            var ag = (ushort)this.player.Attributes[Stats.CurrentAbility];
            this.connection.Send(new byte[]
            {
                0xC1, 0x08, 0x27, (byte)UpdateType.Current,
                                mana.GetHighByte(), mana.GetLowByte(),
                                ag.GetHighByte(), ag.GetLowByte()
            });
        }

        /// <inheritdoc/>
        public void UpdateCurrentHealth()
        {
            // C1 09 26 FE 00 C3 00 00 85
            var hp = (ushort)Math.Max(this.player.Attributes[Stats.CurrentHealth], 0f);
            var sd = (ushort)Math.Max(this.player.Attributes[Stats.CurrentShield], 0f);
            this.connection.Send(new byte[]
            {
                0xC1, 0x09, 0x26, (byte)UpdateType.Current,
                                hp.GetHighByte(), hp.GetLowByte(),
                                0x00,
                                sd.GetHighByte(), sd.GetLowByte()
            });
        }

        /// <inheritdoc/>
        public void UpdateMaximumMana()
        {
            var mana = (ushort)this.player.Attributes[Stats.MaximumMana];
            var ag = (ushort)this.player.Attributes[Stats.MaximumAbility];
            this.connection.Send(new byte[]
            {
                0xC1, 0x08, 0x27, (byte)UpdateType.Maximum,
                                mana.GetHighByte(), mana.GetLowByte(),
                                ag.GetHighByte(), ag.GetLowByte()
            });
        }

        /// <inheritdoc/>
        public void UpdateMaximumHealth()
        {
            var hp = (ushort)this.player.Attributes[Stats.MaximumHealth];
            var sd = (ushort)this.player.Attributes[Stats.MaximumShield];
            this.connection.Send(new byte[]
            {
                0xC1, 0x09, 0x26, (byte)UpdateType.Maximum,
                                hp.GetHighByte(), hp.GetLowByte(),
                                0x00,
                                sd.GetHighByte(), sd.GetLowByte()
            });
        }

        /// <inheritdoc/>
        public void UpdateLevel()
        {
            var selectedCharacter = this.player.SelectedCharacter;
            if (selectedCharacter != null)
            {
                var charStats = this.player.Attributes;
                var level = (ushort)charStats[Stats.Level];
                var leveluppoints = (ushort)selectedCharacter.LevelUpPoints;
                var maximumHealth = (ushort)charStats[Stats.MaximumHealth];
                var maximumMana = (ushort)charStats[Stats.MaximumMana];
                var maximumShield = (ushort)charStats[Stats.MaximumShield];
                var maximumAbility = (ushort)charStats[Stats.MaximumAbility];
                var fruitPoints = (ushort)selectedCharacter.UsedFruitPoints; // TODO: is this right or is this the new MaxFruitPoints?

                var packet = new byte[]
                {
                    0xC1, 0x12, 0xF3, 5,
                    level.GetLowByte(), level.GetHighByte(),
                    leveluppoints.GetLowByte(), leveluppoints.GetHighByte(),
                    maximumHealth.GetLowByte(), maximumHealth.GetHighByte(),
                    maximumMana.GetLowByte(), maximumMana.GetHighByte(),
                    maximumShield.GetLowByte(), maximumShield.GetHighByte(),
                    maximumAbility.GetLowByte(), maximumAbility.GetHighByte(),
                    fruitPoints.GetLowByte(), fruitPoints.GetHighByte()
                };

                this.connection.Send(packet);
                this.player.PlayerView.ShowMessage($"Congratuations, you are Level {this.player.Attributes[Stats.Level]} now.", MessageType.BlueNormal);
            }
        }

        /// <inheritdoc/>
        public void AddExperience(int exp, IIdentifiable obj)
        {
            ushort id = obj?.Id ?? 0;
            while (exp > 0)
            {
                // We send multiple exp packets if the value is bigger than ushort.MaxValue, because that's all what the packet can carry.
                // On a normal exp server this should never be an issue, but with higher settings, it fixes the problem that the exp bar
                // shows less exp than the player actually gained.
                ushort sendExp;
                if (exp > ushort.MaxValue)
                {
                    sendExp = ushort.MaxValue;
                }
                else
                {
                    sendExp = (ushort)exp;
                }

                byte[] packet = { 0xC3, 0x09, 0x16, id.GetHighByte(), id.GetLowByte(), sendExp.GetHighByte(), sendExp.GetLowByte(), 0, 0 }; // last 2 bytes = last hit dmg
                this.connection.Send(packet);
                exp -= sendExp;
            }
        }

        /// <inheritdoc/>
        public void PlayerShopClosed(Player playerWithClosedShop)
        {
            this.connection.Send(new byte[] { 0xC1, 7, 0x3F, 3, 1, playerWithClosedShop.Id.GetHighByte(), playerWithClosedShop.Id.GetLowByte() });
        }

        /// <inheritdoc/>
        public void CloseVault()
        {
            this.connection.Send(new byte[] { 0xC1, 3, 0x82 });
        }

        /// <inheritdoc/>
        public void ShowVault()
        {
            (this as IPlayerView).OpenNpcWindow(NpcWindow.VaultStorage);
            var itemcount = this.player.Vault.ItemStorage.Items.Count;
            var spacePerItem = this.itemSerializer.NeededSpace + 1;
            var packet = new byte[6 + (itemcount * spacePerItem)];
            packet[0] = 0xC2;
            packet[1] = (byte)((packet.Length >> 8) & 0xFF);
            packet[2] = (byte)(packet.Length & 0xFF);
            packet[3] = 0x31;
            packet[4] = 0;
            packet[5] = (byte)itemcount;
            int i = 0;
            foreach (var item in this.player.Vault.Items)
            {
                packet[6 + (i * 13)] = item.ItemSlot;
                this.itemSerializer.SerializeItem(packet, 7 + (i * spacePerItem), item);
                i++;
            }

            this.connection.Send(packet); // item list
            uint zen = (uint)this.player.Account.Vault.Money;
            var zenpacket = new byte[0x0C];
            zenpacket[0] = 0xC1;
            zenpacket[1] = 0x0C;
            zenpacket[2] = 0x81;
            zenpacket[3] = 0x01;
            zenpacket[4] = (byte)(zen & 0xFF);
            zenpacket[5] = (byte)((zen >> 8) & 0xFF);
            zenpacket[6] = (byte)((zen >> 16) & 0xFF);
            zenpacket[7] = (byte)((zen >> 24) & 0xFF);

            this.connection.Send(zenpacket); // zen amount
            this.connection.Send(new byte[] { 0xC1, 4, 0x83, 0 }); // ??
        }

        /// <inheritdoc/>
        public void UpdateCharacterStats()
        {
            var packet = new byte[0x48];
            packet.SetValues<byte>(0xC3, (byte)packet.Length, 0xF3, 0x03);
            var i = 4;
            packet[i++] = this.player.X;
            packet[i++] = this.player.Y;
            var unsignedMapNumber = ShortExtensions.ToUnsigned(this.player.SelectedCharacter.CurrentMap.Number);
            packet[i++] = unsignedMapNumber.GetHighByte();
            packet[i++] = unsignedMapNumber.GetLowByte();
            packet.SetLongSmallEndian(this.player.SelectedCharacter.Experience, i++);
            i += 7;
            packet.SetLongSmallEndian(this.context.Configuration.ExperienceTable[(int)this.player.Attributes[Stats.Level] + 1], i++);
            i += 7;
            packet.SetShortBigEndian((ushort)this.player.SelectedCharacter.LevelUpPoints, i++);
            i++;
            packet.SetShortBigEndian((ushort)this.player.Attributes[Stats.BaseStrength], i++);
            i++;
            packet.SetShortBigEndian((ushort)this.player.Attributes[Stats.BaseAgility], i++);
            i++;
            packet.SetShortBigEndian((ushort)this.player.Attributes[Stats.BaseVitality], i++);
            i++;
            packet.SetShortBigEndian((ushort)this.player.Attributes[Stats.BaseEnergy], i++);
            i++;
            packet.SetShortBigEndian((ushort)this.player.Attributes[Stats.CurrentHealth], i++);
            i++;
            packet.SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumHealth], i++);
            i++;
            packet.SetShortBigEndian((ushort)this.player.Attributes[Stats.CurrentMana], i++);
            i++;
            packet.SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumMana], i++);
            i++;
            packet.SetShortBigEndian((ushort)this.player.Attributes[Stats.CurrentShield], i++);
            i++;
            packet.SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumShield], i++);
            i++;
            packet.SetShortBigEndian((ushort)this.player.Attributes[Stats.CurrentAbility], i++);
            i++;
            packet.SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumAbility], i++);
            i++;
            packet[i++] = 0;    // unknown
            packet[i++] = 0x12; // unknown
            packet.SetIntegerBigEndian((uint)this.player.Money, i++);
            i += 3;
            packet[i++] = (byte)this.player.SelectedCharacter.PlayerKillCount;
            packet[i++] = (byte)this.player.SelectedCharacter.State;
            packet.SetShortBigEndian((ushort)this.player.SelectedCharacter.UsedFruitPoints, i++);
            i++;
            packet.SetShortBigEndian(127, i++); // TODO: MaxFruits, calculate the right value
            i++;
            packet.SetShortBigEndian((ushort)this.player.Attributes[Stats.BaseLeadership], i++);
            i++;
            packet.SetShortBigEndian((ushort)this.player.SelectedCharacter.UsedNegFruitPoints, i++);
            i++;
            packet.SetShortBigEndian(127, i++); // TODO: MaxNegFruits, calculate the right value
            i++;
            packet[i++] = this.player.Account.IsVaultExtended ? (byte)1 : (byte)0;
            packet[i++] = 0x25; // unknown
            packet[i++] = 0xAB; // unknown
            packet[i] = 0x71; // unknown
            this.connection.Send(packet);
        }

        /// <remarks>
        /// This Packet is sent to the Client when a Player or Monster got Hit and damaged.
        /// It includes which Player/Monster got hit by who, and the Damage Type.
        /// TODO: It is obvious that the mu online protocol only supports 16 bits for each damage value. Maybe in the future we could send more than one packet, if the 16bits are not enough.
        /// </remarks>
        /// <inheritdoc/>
        public void ShowHit(IAttackable target, HitInfo hitInfo)
        {
            var healthDamage = (ushort)(hitInfo.DamageHP & 0xFFFF);
            var shieldDamage = (ushort)(hitInfo.DamageSD & 0xFFFF);
            this.connection.Send(new byte[]
            {
                0xC1, 0x0A, (byte)PacketType.Hit, target.Id.GetHighByte(), target.Id.GetLowByte(),
                                    healthDamage.GetHighByte(), healthDamage.GetLowByte(),
                                    this.GetDamageColor(hitInfo.Attributes),
                                    shieldDamage.GetLowByte(), shieldDamage.GetHighByte()
            });
        }

        /// <inheritdoc/>
        public void ActivateMagicEffect(MagicEffect effect, Player affectedPlayer)
        {
            this.SendMagicEffectStatus(effect, affectedPlayer, true, effect.Definition.SendDuration ? (uint)effect.Duration.TotalMilliseconds : 0);
        }

        /// <inheritdoc/>
        public void DeactivateMagicEffect(MagicEffect effect, Player affectedPlayer)
        {
            this.SendMagicEffectStatus(effect, affectedPlayer, false, 0);
        }

        /// <inheritdoc/>
        public void UpdateSkillList()
        {
            byte[] packet = new byte[6 + (4 * this.player.SkillList.SkillCount)];
            packet[0] = 0xC1;
            packet[1] = (byte)packet.Length;
            packet[2] = 0xF3;
            packet[3] = 0x11;
            packet[4] = this.player.SkillList.SkillCount;

            byte i = 0;
            foreach (var skillEntry in this.player.SkillList.Skills)
            {
                int offset = i * 4;
                packet[6 + offset] = i;
                var unsignedSkillId = ShortExtensions.ToUnsigned(skillEntry.Skill.SkillID);
                packet[7 + offset] = unsignedSkillId.GetLowByte();
                packet[8 + offset] = unsignedSkillId.GetHighByte();
                packet[9 + offset] = (byte)skillEntry.Level;
                i++;
            }

            this.connection.Send(packet);
        }

        /// <inheritdoc/>
        public void ShowMessage(string message, OpenMU.Interfaces.MessageType messageType)
        {
            const string messagePrefix = "000000000";
            string content = messagePrefix + message;
            byte[] packet = new byte[content.Length + 5];
            packet[0] = 0xc1;
            packet[1] = (byte)(content.Length + 5);
            packet[2] = 13;
            packet[3] = (byte)messageType;
            Encoding.UTF8.GetBytes(content, 0, content.Length, packet, 4);
            this.connection.Send(packet);
        }

        /// <inheritdoc/>
        public void ShowShopItemList(Player requestedPlayer)
        {
            const int maxCharacterNameLength = 10;
            const int maxStoreNameLength = 36;

            // TODO:  Maybe cache the result, because a lot of players could request the same list.
            var itemlist = requestedPlayer.ShopStorage.Items.ToList();
            var itemcount = itemlist.Count;
            var packet = new byte[itemcount];
            packet[0] = 0xC2;
            packet[3] = 1;
            packet[4] = requestedPlayer.Id.GetLowByte();
            packet[5] = requestedPlayer.Id.GetHighByte();

            Encoding.UTF8.GetBytes(this.player.SelectedCharacter.Name, 0, Math.Min(maxCharacterNameLength, this.player.SelectedCharacter.Name.Length), packet, 6);
            var storeName = requestedPlayer.ShopStorage.StoreName;
            Encoding.UTF8.GetBytes(storeName, 0, Math.Min(storeName.Length, maxStoreNameLength), packet, 16);
            packet[6 + maxCharacterNameLength + maxStoreNameLength] = (byte)itemcount;
            int i = 0;
            foreach (var item in itemlist)
            {
                var offset = 16 + 37 + (i * (8 + this.itemSerializer.NeededSpace)); // not sure about the last part...
                var slot = item.ItemSlot - InventoryConstants.FirstStoreItemSlotIndex;
                packet[offset + 0] = (byte)slot;
                this.itemSerializer.SerializeItem(packet, offset + 1, item);
                packet.SetIntegerSmallEndian(requestedPlayer.ShopStorage.StorePrices[slot], offset + 4 + this.itemSerializer.NeededSpace);
            }

            this.connection.Send(packet);
        }

        /// <inheritdoc/>
        public void PlayerShopOpened(Player playerWithShop)
        {
            this.ShowShopsOfPlayers(new List<Player>(1) { playerWithShop });
            if (this.player.Id == playerWithShop.Id)
            {
                this.connection.Send(new byte[] { 0xC1, 0x05, 0x3F, 0x02, 0x01 }); // Success of opening the own shop
            }
        }

        /// <inheritdoc />
        public void ShowShopsOfPlayers(ICollection<Player> playersWithShop)
        {
            var shopCount = playersWithShop.Count;
            var packet = new byte[6 + (38 * shopCount)];
            packet[0] = 0xC2;
            packet[1] = (byte)((packet.Length >> 8) & 0xFF);
            packet[2] = (byte)(packet.Length & 0xFF);
            packet[3] = 0x3F;
            packet[4] = (byte)shopCount;
            int offset = 5;
            foreach (var shopPlayer in playersWithShop)
            {
                packet[offset] = shopPlayer.Id.GetHighByte();
                packet[offset + 1] = shopPlayer.Id.GetLowByte();
                System.Text.Encoding.UTF8.GetBytes(shopPlayer.ShopStorage.StoreName, 0, shopPlayer.ShopStorage.StoreName.Length, packet, offset + 2);
                offset += 38;
            }

            this.connection.Send(packet);
        }

        /// <remarks>
        /// This packet is sent to the Client after a Logout Request, or by disconnection by the server.
        /// It will send the Client to the the Server Select, Character Select or Disconnects the User.
        /// </remarks>
        /// <inheritdoc />
        public void Logout(LogoutType logoutType)
        {
            this.connection.Send(new byte[] { 0xC3, 0x05, 0xF1, 0x02, (byte)logoutType });
        }

        /// <inheritdoc/>
        public void CharacterFocused(Character character)
        {
            var packet = new byte[0x0F];
            packet[0] = 0xC1;
            packet[1] = 0x0F;
            packet[2] = 0xF3;
            packet[3] = 0x15;
            Encoding.UTF8.GetBytes(character.Name, 0, character.Name.Length, packet, 4);
            this.connection.Send(packet);
        }

        /// <inheritdoc/>
        public void StatIncreaseResult(AttributeDefinition attribute, bool success)
        {
            byte successstat = (byte)attribute.GetStatType();
            if (success)
            {
                successstat += 1 << 4;
            }

            this.connection.Send(new byte[] { 0xC1, 0x05, 0xF3, 0x06, successstat });
        }

        /// <inheritdoc/>
        public void OpenNpcWindow(NpcWindow window)
        {
            this.connection.Send(new byte[] { 0xC3, 0x0B, 0x30, this.GetWindowIdOf(window), 0, 0, 0, 0, 0, 0, 0 });
        }

        /// <inheritdoc/>
        public void ShowMerchantStoreItemList(ICollection<Item> storeItems)
        {
            // C2 [len_high] [len_low] 31 [itemcount_high] [itemcount_low]
            // for each item: [slot number] [item data....]
            const int slotNumberSize = 1;
            const int headerSize = 6;
            var packet = new byte[headerSize + (storeItems.Count * (this.itemSerializer.NeededSpace + slotNumberSize))];
            packet[0] = 0xC2;
            packet[1] = ((ushort)packet.Length).GetHighByte();
            packet[2] = ((ushort)packet.Length).GetLowByte();
            packet[3] = 0x31;
            packet[4] = ((ushort)storeItems.Count).GetHighByte();
            packet[5] = ((ushort)storeItems.Count).GetLowByte();

            int i = 0;
            foreach (var item in storeItems)
            {
                var offset = 6 + (i * this.itemSerializer.NeededSpace);
                packet[offset] = item.ItemSlot;
                this.itemSerializer.SerializeItem(packet, offset + 1, item);
                i++;
            }

            this.connection.Send(packet);
        }

        /// <inheritdoc/>
        public void ShowLoginResult(LoginResult loginResult)
        {
            this.connection.Send(new byte[] { 0xC1, 0x05, 0xF1, 0x01, (byte)loginResult });
        }

        /// <inheritdoc/>
        public void ShowLoginWindow()
        {
            var message = new byte[0x0C];
            message[0] = 0xC1;
            message[1] = 0x0C;
            message[2] = 0xF1;
            message[3] = 0x00;
            message[4] = 0x01; // Success
            message[5] = this.player.Id.GetHighByte();
            message[6] = this.player.Id.GetLowByte();
            Buffer.BlockCopy(this.lowestClientVersion, 0, message, 7, 5);
            this.connection.Send(message);
        }

        private byte GetDamageColor(DamageAttributes attributes)
        {
            var colorResult = DamageColor.NormalRed;
            if (attributes.HasFlag(DamageAttributes.IgnoreDefense))
            {
                colorResult = DamageColor.IgnoreDefenseCyan;
            }
            else if (attributes.HasFlag(DamageAttributes.Excellent))
            {
                colorResult = DamageColor.ExcellentLightGreen;
            }
            else if (attributes.HasFlag(DamageAttributes.Critical))
            {
                colorResult = DamageColor.CriticalBlue;
            }

            byte result = (byte)colorResult;
            if (attributes.HasFlag(DamageAttributes.Double))
            {
                result |= (byte)SpecialDamage.Double;
            }

            if (attributes.HasFlag(DamageAttributes.Triple))
            {
                result |= (byte)SpecialDamage.Triple;
            }

            return result;
        }

        private void SendMagicEffectStatus(MagicEffect effect, Player affectedPlayer, bool isActive, uint duration)
        {
            this.connection.Send(new byte[] { 0xC1, 7, 7, isActive ? (byte)1 : (byte)0, affectedPlayer.Id.GetHighByte(), affectedPlayer.Id.GetLowByte(), effect.Id });

            // TODO: Duration
        }

        private byte GetGuildMemberStatusCode(GuildPosition position)
        {
            switch (position)
            {
                case GuildPosition.NormalMember:
                    return 0;
                case GuildPosition.GuildMaster:
                    return 0x80;
                case GuildPosition.BattleMaster:
                    return 0x20;
            }

            throw new ArgumentException("GuildPosition not mapped.");
        }

        private byte GetPlayerStateCode(HeroState state)
        {
            // TODO: check if this is right.
            return (byte)state;
        }

        private byte GetWindowIdOf(NpcWindow window)
        {
            switch (window)
            {
                case NpcWindow.Merchant: return 0;
                case NpcWindow.Merchant1: return 1;
                case NpcWindow.VaultStorage: return 2;
                case NpcWindow.ChaosMachine: return 3;
                case NpcWindow.DevilSquare: return 4;
                case NpcWindow.BloodCastle: return 6;
                case NpcWindow.PetTrainer: return 7;
                case NpcWindow.Lahap: return 9;
                case NpcWindow.CastleSeniorNPC: return 0x0C;
                case NpcWindow.ElphisRefinery: return 0x11;
                case NpcWindow.RefineStoneMaking: return 0x12;
                case NpcWindow.RemoveJohOption: return 0x13;
                case NpcWindow.IllusionTemple: return 0x14;
                case NpcWindow.ChaosCardCombination: return 0x15;
                case NpcWindow.CherryBlossomBranchesAssembly: return 0x16;
                case NpcWindow.SeedMaster: return 0x17;
                case NpcWindow.SeedResearcher: return 0x18;
                case NpcWindow.StatReInitializer: return 0x19;
                case NpcWindow.DelgadoLuckyCoinRegistration: return 0x20;
                case NpcWindow.DoorkeeperTitusDuelWatch: return 0x21;
                case NpcWindow.LugardDoppelgangerEntry: return 0x23;
                case NpcWindow.JerintGaionEvententry: return 0x24;
                case NpcWindow.JuliaWarpMarketServer: return 0x25;
                case NpcWindow.CombineLuckyItem: return 0x26;
                case NpcWindow.GuildMaster: throw new ArgumentException("guild master dialog is opened by another action.");
            }

            return (byte)window;
        }

        private byte[] GetLowestClientVersionOfConfiguration()
        {
            return this.context.ServerConfiguration.SupportedPacketHandlers.OrderBy(v => v.ClientVersion.MakeDwordSmallEndian(1)).First().ClientVersion;
        }

        private class CharacterAppearanceDataAdapter : IAppearanceData
        {
            private readonly Character character;

            public CharacterAppearanceDataAdapter(Character character)
            {
                this.character = character;
            }

            public CharacterClass CharacterClass => this.character?.CharacterClass;

            public IEnumerable<ItemAppearance> EquippedItems
            {
                get
                {
                    if (this.character.Inventory != null)
                    {
                        return this.character.Inventory.Items
                            .Where(item => item.ItemSlot <= InventoryConstants.LastEquippableItemSlotIndex)
                            .Select(item =>
                            {
                                return new ItemAppearance
                                {
                                    Index = item.Definition.Number,
                                    Group = item.Definition.Group,
                                    ItemSlot = item.ItemSlot,
                                    Level = item.Level,
                                    VisibleOptions = item.ItemOptions.Select(option => option.ItemOption.OptionType).ToArray()
                                };
                            });
                    }

                    return Enumerable.Empty<ItemAppearance>();
                }
            }
        }
    }
}