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
        /// An item consumption failed, no value is updated.
        /// </summary>
        Failed = 0xFD,

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
        /// This contains again all available skills. However, we need this to maintain the indexes. It can happen that the list contains holes after a skill got removed!
        /// </summary>
        private IList<Skill> skillList;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteView"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="player">The player.</param>
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
            this.GuildView = new GuildView(connection, player);
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

        private IList<Skill> SkillList => this.skillList ?? (this.skillList = new List<Skill>());

        /// <inheritdoc/>
        public void ShowCharacterList()
        {
            const int characterSize = 34;
            using (var writer = this.connection.StartSafeWrite(0xC1, (this.player.Account.Characters.Count * characterSize) + 8))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0;
                byte maxClass = 0;
                packet[4] = this.player.Account.UnlockedCharacterClasses?.Select(c => c.CreationAllowedFlag)
                                .Aggregate(maxClass, (current, flag) => (byte)(current | flag)) ?? 0;
                packet[5] = 0; // MoveCnt
                packet[6] = (byte)this.player.Account.Characters.Count;

                // packet[7] ??? new in season 6 - probably vault extension
                int i = 0;
                foreach (var character in this.player.Account.Characters.OrderBy(c => c.CharacterSlot))
                {
                    var characterBlock = packet.Slice((i * characterSize) + 8, characterSize);
                    characterBlock[0] = character.CharacterSlot;
                    characterBlock.Slice(1, 10).WriteString(character.Name, Encoding.UTF8);
                    characterBlock[11] = 1; // unknown
                    var level = (ushort)character.Attributes.First(s => s.Definition == Stats.Level).Value;
                    characterBlock[12] = level.GetLowByte();
                    characterBlock[13] = level.GetHighByte();
                    characterBlock[14] = (byte)character.State; // | 0x10 for item block?

                    this.appearanceSerializer.WriteAppearanceData(characterBlock.Slice(15), new CharacterAppearanceDataAdapter(character), false);

                    //// var guildStatusIndex = offset + 15 + 18;
                    //// TODO: characterBLock[guildStatusIndex] = this.GetGuildMemberStatusCode(character.GuildMemberInfo?.Status);

                    i++;
                }

                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ShowCharacterCreationFailed()
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 5))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x01;
                packet[4] = 0x00;
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ShowCreatedCharacter(Character character)
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 0x2A))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x01;
                packet[4] = 0x01; // success
                packet.Slice(5).WriteString(character.Name, Encoding.UTF8);
                packet[15] = character.CharacterSlot;
                packet[16] = 0x01;
                packet[17] = 0x00;
                packet[18] = (byte)(character.CharacterClass.Number << 3);
                packet[19] = this.GetPlayerStateCode(character.State);
                packet.Slice(20, 22).Fill(0xFF); // preview data?
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void AppearanceChanged(Player changedPlayer)
        {
            //// var appearanceData = changedPlayer.GetAppearanceData(this.appearanceSerializer);

            // todo: find the right packet structure and send it to the client.
        }

        /// <inheritdoc/>
        public void AddSkill(Skill skill)
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 0x0A))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x11;
                packet[4] = 0xFE;

                byte? skillIndex = null;
                for (byte i = 0; i < this.SkillList.Count; i++)
                {
                    if (this.SkillList[i] == null)
                    {
                        skillIndex = i;
                    }
                }

                if (skillIndex == null)
                {
                    this.SkillList.Add(skill);
                    skillIndex = (byte)(this.SkillList.Count - 1);
                }

                packet[6] = skillIndex.Value;
                var unsignedSkillId = ShortExtensions.ToUnsigned(skill.Number);
                packet[7] = unsignedSkillId.GetLowByte();
                packet[8] = unsignedSkillId.GetHighByte();
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void RemoveSkill(Skill skill)
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 0x0A))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x11;
                packet[4] = 0xFF;
                packet[6] = (byte)this.SkillList.IndexOf(skill);
                var unsignedSkillId = ShortExtensions.ToUnsigned(skill.Number);
                packet[7] = unsignedSkillId.GetLowByte();
                packet[8] = unsignedSkillId.GetHighByte();
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void DrinkAlcohol()
        {
            using (var writer = this.connection.StartSafeWrite(0xC3, 0x06))
            {
                var packet = writer.Span;
                packet[2] = 0x29;
                packet[3] = 0;
                packet[4] = 0x50;
                packet[5] = 0;
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ShowCharacterDeleteResponse(CharacterDeleteResult result)
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 0x05))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x02;
                packet[4] = (byte)result;
                writer.Commit();
            }
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
            using (var writer = this.connection.StartSafeWrite(0xC1, 0x08))
            {
                var packet = writer.Span;
                packet[2] = 0x27;
                packet[3] = (byte)UpdateType.Current;
                packet[4] = mana.GetHighByte();
                packet[5] = mana.GetLowByte();
                packet[6] = ag.GetHighByte();
                packet[7] = ag.GetLowByte();
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void UpdateCurrentHealth()
        {
            // C1 09 26 FE 00 C3 00 00 85
            var hp = (ushort)Math.Max(this.player.Attributes[Stats.CurrentHealth], 0f);
            var sd = (ushort)Math.Max(this.player.Attributes[Stats.CurrentShield], 0f);
            using (var writer = this.connection.StartSafeWrite(0xC1, 0x09))
            {
                var packet = writer.Span;
                packet[2] = 0x26;
                packet[3] = (byte)UpdateType.Current;
                packet[4] = hp.GetHighByte();
                packet[5] = hp.GetLowByte();
                packet[7] = sd.GetHighByte();
                packet[8] = sd.GetLowByte();
                writer.Commit();
            }
        }

        /// <inheritdoc />
        /// <remarks>The server sends the current health/shield to the client, with <see cref="UpdateType.Failed"/>.</remarks>
        public void RequestedItemConsumptionFailed()
        {
            var hp = (ushort)Math.Max(this.player.Attributes[Stats.CurrentHealth], 0f);
            var sd = (ushort)Math.Max(this.player.Attributes[Stats.CurrentShield], 0f);

            using (var writer = this.connection.StartSafeWrite(0xC1, 0x09))
            {
                var packet = writer.Span;
                packet[2] = 0x26;
                packet[3] = (byte)UpdateType.Failed;
                packet[4] = hp.GetHighByte();
                packet[5] = hp.GetLowByte();
                packet[7] = sd.GetHighByte();
                packet[8] = sd.GetLowByte();
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void UpdateMaximumMana()
        {
            var mana = (ushort)this.player.Attributes[Stats.MaximumMana];
            var ag = (ushort)this.player.Attributes[Stats.MaximumAbility];

            using (var writer = this.connection.StartSafeWrite(0xC1, 0x08))
            {
                var packet = writer.Span;
                packet[2] = 0x27;
                packet[3] = (byte)UpdateType.Maximum;
                packet[4] = mana.GetHighByte();
                packet[5] = mana.GetLowByte();
                packet[6] = ag.GetHighByte();
                packet[7] = ag.GetLowByte();
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void UpdateMaximumHealth()
        {
            var hp = (ushort)this.player.Attributes[Stats.MaximumHealth];
            var sd = (ushort)this.player.Attributes[Stats.MaximumShield];
            using (var writer = this.connection.StartSafeWrite(0xC1, 0x09))
            {
                var packet = writer.Span;
                packet[2] = 0x26;
                packet[3] = (byte)UpdateType.Maximum;
                packet[4] = hp.GetHighByte();
                packet[5] = hp.GetLowByte();
                packet[7] = sd.GetHighByte();
                packet[8] = sd.GetLowByte();
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void UpdateLevel()
        {
            var selectedCharacter = this.player.SelectedCharacter;
            if (selectedCharacter == null)
            {
                return;
            }

            var charStats = this.player.Attributes;
            var level = (ushort)charStats[Stats.Level];
            var levelUpPoints = (ushort)selectedCharacter.LevelUpPoints;
            var maximumHealth = (ushort)charStats[Stats.MaximumHealth];
            var maximumMana = (ushort)charStats[Stats.MaximumMana];
            var maximumShield = (ushort)charStats[Stats.MaximumShield];
            var maximumAbility = (ushort)charStats[Stats.MaximumAbility];
            var fruitPoints = (ushort)selectedCharacter.UsedFruitPoints;
            var maxFruitPoints = selectedCharacter.GetMaximumFruitPoints();
            var negativeFruitPoints = (ushort)selectedCharacter.UsedNegFruitPoints;

            using (var writer = this.connection.StartSafeWrite(0xC1, 0x18))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x05;
                packet.Slice(4).SetShortBigEndian(level);
                packet.Slice(6).SetShortBigEndian(levelUpPoints);
                packet.Slice(8).SetShortBigEndian(maximumHealth);
                packet.Slice(10).SetShortBigEndian(maximumMana);
                packet.Slice(12).SetShortBigEndian(maximumShield);
                packet.Slice(14).SetShortBigEndian(maximumAbility);
                packet.Slice(16).SetShortBigEndian(fruitPoints);
                packet.Slice(18).SetShortBigEndian(maxFruitPoints);
                packet.Slice(20).SetShortBigEndian(negativeFruitPoints);
                packet.Slice(22).SetShortBigEndian(maxFruitPoints);

                writer.Commit();
            }

            this.player.PlayerView.ShowMessage($"Congratulations, you are Level {this.player.Attributes[Stats.Level]} now.", MessageType.BlueNormal);
        }

        /// <inheritdoc/>
        public void AddExperience(int exp, IIdentifiable obj)
        {
            var remainingExperience = exp;
            ushort id = obj.GetId(this.player);
            while (remainingExperience > 0)
            {
                // We send multiple exp packets if the value is bigger than ushort.MaxValue, because that's all what the packet can carry.
                // On a normal exp server this should never be an issue, but with higher settings, it fixes the problem that the exp bar
                // shows less exp than the player actually gained.
                ushort sendExp = remainingExperience > ushort.MaxValue ? ushort.MaxValue : (ushort)remainingExperience;
                using (var writer = this.connection.StartSafeWrite(0xC3, 0x09))
                {
                    var packet = writer.Span;
                    packet[2] = 0x16;
                    packet.Slice(3).SetShortSmallEndian(id);
                    packet.Slice(5).SetShortSmallEndian(sendExp);
                    packet.Slice(7).SetShortSmallEndian((ushort)((obj as IAttackable)?.LastReceivedDamage ?? 0));
                    writer.Commit();
                }

                remainingExperience -= sendExp;
            }
        }

        /// <inheritdoc/>
        public void PlayerShopClosed(Player playerWithClosedShop)
        {
            var playerId = playerWithClosedShop.GetId(this.player);
            using (var writer = this.connection.StartSafeWrite(0xC1, 0x07))
            {
                var packet = writer.Span;
                packet[2] = 0x3F;
                packet[3] = 3;
                packet[4] = 1;
                packet.Slice(5).SetShortSmallEndian(playerId);
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void CloseVault()
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 0x03))
            {
                var packet = writer.Span;
                packet[2] = 0x82;
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ShowVault()
        {
            (this as IPlayerView).OpenNpcWindow(NpcWindow.VaultStorage);
            var itemsCount = this.player.Vault.ItemStorage.Items.Count;
            var spacePerItem = this.itemSerializer.NeededSpace + 1;
            using (var writer = this.connection.StartSafeWrite(0xC2, 6 + (itemsCount * spacePerItem)))
            {
                var packet = writer.Span;
                packet[3] = 0x31;
                packet[4] = 0;
                packet[5] = (byte)itemsCount;
                int i = 0;
                foreach (var item in this.player.Vault.Items)
                {
                    var itemBlock = packet.Slice(6 + (i * spacePerItem), spacePerItem);
                    itemBlock[0] = item.ItemSlot;
                    this.itemSerializer.SerializeItem(itemBlock.Slice(1), item);
                    i++;
                }

                writer.Commit();
            }

            using (var writer = this.connection.StartSafeWrite(0xC1, 0x0C))
            {
                uint zen = (uint)this.player.Account.Vault.Money;
                var zenPacket = writer.Span;
                zenPacket[2] = 0x81;
                zenPacket[3] = 0x01;
                zenPacket.Slice(4).SetIntegerBigEndian(zen);
                writer.Commit();
            }

            using (var writer = this.connection.StartSafeWrite(0xC1, 4))
            {
                // vault password protection info
                var packet = writer.Span;
                packet[2] = 0x83;
                packet[3] = 0; // if protected, it's 1
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void UpdateCharacterStats()
        {
            using (var writer = this.connection.StartSafeWrite(0xC3, 0x48))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x03;
                packet[4] = this.player.Position.X;
                packet[5] = this.player.Position.Y;
                var unsignedMapNumber = ShortExtensions.ToUnsigned(this.player.SelectedCharacter.CurrentMap.Number);
                packet[6] = unsignedMapNumber.GetLowByte();
                packet[7] = unsignedMapNumber.GetHighByte();
                packet.Slice(8).SetLongSmallEndian(this.player.SelectedCharacter.Experience);
                packet.Slice(16).SetLongSmallEndian(this.context.Configuration.ExperienceTable[(int)this.player.Attributes[Stats.Level] + 1]);
                packet.Slice(24).SetShortBigEndian((ushort)this.player.SelectedCharacter.LevelUpPoints);
                packet.Slice(26).SetShortBigEndian((ushort)this.player.Attributes[Stats.BaseStrength]);
                packet.Slice(28).SetShortBigEndian((ushort)this.player.Attributes[Stats.BaseAgility]);
                packet.Slice(30).SetShortBigEndian((ushort)this.player.Attributes[Stats.BaseVitality]);
                packet.Slice(32).SetShortBigEndian((ushort)this.player.Attributes[Stats.BaseEnergy]);
                packet.Slice(34).SetShortBigEndian((ushort)this.player.Attributes[Stats.CurrentHealth]);
                packet.Slice(36).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumHealth]);
                packet.Slice(38).SetShortBigEndian((ushort)this.player.Attributes[Stats.CurrentMana]);
                packet.Slice(40).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumMana]);
                packet.Slice(42).SetShortBigEndian((ushort)this.player.Attributes[Stats.CurrentShield]);
                packet.Slice(44).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumShield]);
                packet.Slice(46).SetShortBigEndian((ushort)this.player.Attributes[Stats.CurrentAbility]);
                packet.Slice(48).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumAbility]);

                //// 2 missing bytes here are padding
                packet.Slice(52).SetIntegerBigEndian((uint)this.player.Money);
                packet[56] = (byte)this.player.SelectedCharacter.PlayerKillCount;
                packet[57] = (byte)this.player.SelectedCharacter.State;
                packet.Slice(58).SetShortBigEndian((ushort)this.player.SelectedCharacter.UsedFruitPoints);
                packet.Slice(60).SetShortBigEndian(this.player.SelectedCharacter.GetMaximumFruitPoints());
                packet.Slice(62).SetShortBigEndian((ushort)this.player.Attributes[Stats.BaseLeadership]);
                packet.Slice(64).SetShortBigEndian((ushort)this.player.SelectedCharacter.UsedNegFruitPoints);
                packet.Slice(66).SetShortBigEndian(this.player.SelectedCharacter.GetMaximumFruitPoints());
                packet[68] = this.player.Account.IsVaultExtended ? (byte)1 : (byte)0;
                //// 3 additional bytes are padding

                writer.Commit();
            }

            if (this.player.SelectedCharacter.CharacterClass.IsMasterClass)
            {
                this.SendMasterStats();
            }

            this.SendKeyConfiguration();
        }

        /// <remarks>
        /// This Packet is sent to the Client when a Player or Monster got Hit and damaged.
        /// It includes which Player/Monster got hit by who, and the Damage Type.
        /// It is obvious that the mu online protocol only supports 16 bits for each damage value. To prevent bugs (own player health)
        /// and to make it somehow visible that the damage exceeds 65k, we send more than one packet, if the 16bits are not enough.
        /// </remarks>
        /// <inheritdoc/>
        public void ShowHit(IAttackable target, HitInfo hitInfo)
        {
            var targetId = target.GetId(this.player);
            var remainingHealthDamage = hitInfo.HealthDamage;
            var remainingShieldDamage = hitInfo.ShieldDamage;
            while (remainingHealthDamage > 0 || remainingShieldDamage > 0)
            {
                var healthDamage = (ushort)(remainingHealthDamage & 0xFFFF);
                var shieldDamage = (ushort)(remainingShieldDamage & 0xFFFF);
                using (var writer = this.connection.StartSafeWrite(0xC1, 0x0A))
                {
                    var packet = writer.Span;
                    packet[2] = (byte) PacketType.Hit;
                    packet.Slice(3).SetShortSmallEndian(targetId);
                    packet.Slice(5).SetShortSmallEndian(healthDamage);
                    packet[7] = this.GetDamageColor(hitInfo.Attributes);
                    packet.Slice(8).SetShortSmallEndian(shieldDamage);
                    writer.Commit();
                }

                remainingShieldDamage -= shieldDamage;
                remainingHealthDamage -= healthDamage;
            }
        }

        /// <inheritdoc/>
        public void ActivateMagicEffect(MagicEffect effect, IAttackable affectedObject)
        {
            this.SendMagicEffectStatus(effect, affectedObject, true, effect.Definition.SendDuration ? (uint)effect.Duration.TotalMilliseconds : 0);
        }

        /// <inheritdoc/>
        public void DeactivateMagicEffect(MagicEffect effect, IAttackable affectedObject)
        {
            this.SendMagicEffectStatus(effect, affectedObject, false, 0);
        }

        /// <inheritdoc/>
        public void UpdateSkillList()
        {
            this.SkillList.Clear();
            using (var writer = this.connection.StartSafeWrite(0xC1, 6 + (4 * this.player.SkillList.SkillCount)))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x11;
                packet[4] = this.player.SkillList.SkillCount;

                byte i = 0;
                foreach (var skillEntry in this.player.SkillList.Skills)
                {
                    int offset = i * 4;
                    packet[6 + offset] = i;
                    this.SkillList.Add(skillEntry.Skill);
                    var unsignedSkillId = ShortExtensions.ToUnsigned(skillEntry.Skill.Number);
                    packet[7 + offset] = unsignedSkillId.GetLowByte();
                    packet[8 + offset] = unsignedSkillId.GetHighByte();
                    packet[9 + offset] = (byte)skillEntry.Level;
                    i++;
                }

                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ShowMessage(string message, OpenMU.Interfaces.MessageType messageType)
        {
            const string messagePrefix = "000000000";
            string content = messagePrefix + message;
            using (var writer = this.connection.StartSafeWrite(0xC1, 5 + content.Length))
            {
                var packet = writer.Span;
                packet[2] = 13;
                packet[3] = (byte)messageType;
                packet.Slice(4).WriteString(content, Encoding.UTF8);
                writer.Commit();
            }
        }

        /// <inheritdoc />
        public void ShowMessageOfObject(string message, IIdentifiable sender)
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 6 + message.Length))
            {
                var packet = writer.Span;
                packet[2] = 0x01;
                packet.Slice(3).SetShortSmallEndian(sender.Id);
                packet.Slice(5).WriteString(message, Encoding.UTF8);
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Maybe cache the result, because a lot of players could request the same list. However, this isn't critical.
        /// </remarks>
        public void ShowShopItemList(Player requestedPlayer)
        {
            const int maxCharacterNameLength = 10;
            const int maxStoreNameLength = 36;
            const int itemPriceSize = 4;
            var sizePerItem = this.itemSerializer.NeededSpace + itemPriceSize + 4; // don't know yet, where the additional 4 bytes come from

            var playerId = requestedPlayer.GetId(this.player);
            var itemlist = requestedPlayer.ShopStorage.Items.ToList();
            var itemcount = itemlist.Count;
            using (var writer = this.connection.StartSafeWrite(0xC2, 11 + maxCharacterNameLength + maxStoreNameLength + (itemcount * sizePerItem)))
            {
                var packet = writer.Span;
                packet[3] = 0x3F;
                packet[4] = 0x05;
                packet[5] = 1;
                packet[6] = playerId.GetHighByte();
                packet[7] = playerId.GetLowByte();
                packet.Slice(8, maxCharacterNameLength).WriteString(this.player.SelectedCharacter.Name, Encoding.UTF8);
                var storeName = requestedPlayer.ShopStorage.StoreName;
                packet.Slice(18, maxStoreNameLength).WriteString(storeName, Encoding.UTF8);

                packet[8 + maxCharacterNameLength + maxStoreNameLength] = (byte)itemcount;
                int i = 0;

                foreach (var item in itemlist)
                {
                    var itemBlock = packet.Slice(9 + maxCharacterNameLength + maxStoreNameLength + (i * sizePerItem), sizePerItem);
                    itemBlock[0] = item.ItemSlot;
                    this.itemSerializer.SerializeItem(itemBlock.Slice(1), item);
                    itemBlock.Slice(1 + this.itemSerializer.NeededSpace).SetIntegerBigEndian((uint)(item.StorePrice ?? 0));
                }

                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void PlayerShopOpened(Player playerWithShop)
        {
            this.ShowShopsOfPlayers(new List<Player>(1) { playerWithShop });
            if (this.player == playerWithShop)
            {
                // Success of opening the own shop
                using (var writer = this.connection.StartSafeWrite(0xC1, 0x05))
                {
                    var packet = writer.Span;
                    packet[2] = 0x3F;
                    packet[3] = 0x02;
                    packet[4] = 0x01;
                    writer.Commit();
                }
            }
        }

        /// <inheritdoc />
        public void ShowShopsOfPlayers(ICollection<Player> playersWithShop)
        {
            const int sizePerShop = 38;
            const int headerSize = 6;
            var shopCount = playersWithShop.Count;
            using (var writer = this.connection.StartSafeWrite(0xC2, headerSize + (sizePerShop * shopCount)))
            {
                var packet = writer.Span;
                packet[3] = 0x3F;
                packet[5] = (byte)shopCount;
                int offset = headerSize;
                foreach (var shopPlayer in playersWithShop)
                {
                    var shopPlayerId = shopPlayer.GetId(this.player);
                    var shopBlock = packet.Slice(offset, sizePerShop);
                    shopBlock.SetShortBigEndian(shopPlayerId);
                    shopBlock.Slice(2).WriteString(shopPlayer.ShopStorage.StoreName, Encoding.UTF8);
                    offset += sizePerShop;
                }

                writer.Commit();
            }
        }

        /// <remarks>
        /// This packet is sent to the Client after a Logout Request, or by disconnection by the server.
        /// It will send the Client to the the Server Select, Character Select or Disconnects the User.
        /// </remarks>
        /// <inheritdoc />
        public void Logout(LogoutType logoutType)
        {
            using (var writer = this.connection.StartSafeWrite(0xC3, 0x05))
            {
                var packet = writer.Span;
                packet[2] = 0xF1;
                packet[3] = 0x02;
                packet[4] = (byte)logoutType;
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void CharacterFocused(Character character)
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 0x0F))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x15;
                packet.Slice(4).WriteString(character.Name, Encoding.UTF8);
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void StatIncreaseResult(AttributeDefinition attribute, bool success)
        {
            byte successAndStatType = (byte)attribute.GetStatType();
            if (success)
            {
                successAndStatType += 1 << 4;
            }

            using (var writer = this.connection.StartSafeWrite(0xC1, 0x0C))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x06;
                packet[4] = successAndStatType;
                if (success)
                {
                    if (attribute == Stats.BaseEnergy)
                    {
                        packet.Slice(6).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumMana]);
                    }
                    else if (attribute == Stats.BaseVitality)
                    {
                        packet.Slice(6).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumHealth]);
                    }
                    else
                    {
                        // no updated value required at index 6
                    }

                    packet.Slice(8).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumShield]);
                    packet.Slice(10).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumAbility]);
                }

                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void OpenNpcWindow(NpcWindow window)
        {
            using (var writer = this.connection.StartSafeWrite(0xC3, 0x0B))
            {
                var packet = writer.Span;
                packet[2] = 0x30;
                packet[3] = this.GetWindowIdOf(window);
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ShowMerchantStoreItemList(ICollection<Item> storeItems)
        {
            // C2 [len_high] [len_low] 31 [itemcount_high] [itemcount_low]
            // for each item: [slot number] [item data....]
            const int slotNumberSize = 1;
            const int headerSize = 6;
            int sizePerItem = this.itemSerializer.NeededSpace + slotNumberSize;
            using (var writer = this.connection.StartSafeWrite(0xC2, headerSize + (storeItems.Count * sizePerItem)))
            {
                var packet = writer.Span;
                packet[3] = 0x31;
                packet.Slice(4).SetShortSmallEndian((ushort)storeItems.Count);

                int i = 0;
                foreach (var item in storeItems)
                {
                    var offset = headerSize + (i * sizePerItem);
                    var itemBlock = packet.Slice(offset, sizePerItem);
                    itemBlock[0] = item.ItemSlot;
                    this.itemSerializer.SerializeItem(itemBlock.Slice(slotNumberSize), item);
                    i++;
                }

                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ShowLoginResult(LoginResult loginResult)
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 0x05))
            {
                var packet = writer.Span;
                packet[2] = 0xF1;
                packet[3] = 0x01;
                packet[4] = (byte)loginResult;
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ShowLoginWindow()
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 0x0C))
            {
                var message = writer.Span;
                message[2] = 0xF1;
                message[3] = 0x00;
                message[4] = 0x01; // Success
                message[5] = ViewExtensions.ConstantPlayerId.GetHighByte();
                message[6] = ViewExtensions.ConstantPlayerId.GetLowByte();
                this.lowestClientVersion.CopyTo(message.Slice(7, 5));
                writer.Commit();
            }
        }

        private void SendMasterStats()
        {
            var character = this.player.SelectedCharacter;
            using (var writer = this.connection.StartSafeWrite(0xC1, 0x20))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x50;
                var masterLevel = (ushort)this.player.Attributes[Stats.MasterLevel];
                packet.Slice(4).SetShortBigEndian(masterLevel);
                packet.Slice(6).SetLongSmallEndian(character.MasterExperience);
                packet.Slice(14).SetLongSmallEndian(this.context.Configuration.MasterExperienceTable[masterLevel + 1]);
                packet.Slice(22).SetShortBigEndian((ushort)character.MasterLevelUpPoints);
                packet.Slice(24).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumHealth]);
                packet.Slice(26).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumMana]);
                packet.Slice(28).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumShield]);
                packet.Slice(30).SetShortBigEndian((ushort)this.player.Attributes[Stats.MaximumAbility]);
                writer.Commit();
            }
        }

        private void SendKeyConfiguration()
        {
            var keyConfiguration = this.player.SelectedCharacter.KeyConfiguration;
            if (keyConfiguration == null || keyConfiguration.Length == 0)
            {
                return;
            }

            using (var writer = this.connection.StartSafeWrite(0xC1, 4 + keyConfiguration.Length))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x30;
                keyConfiguration.AsSpan().CopyTo(packet.Slice(4));
                writer.Commit();
            }
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
            else if (attributes.HasFlag(DamageAttributes.Reflected))
            {
                colorResult = DamageColor.DarkPink;
            }
            else
            {
                // no special color
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

        private void SendMagicEffectStatus(MagicEffect effect, IAttackable affectedPlayer, bool isActive, uint duration)
        {
            if (effect.Definition.Number <= 0)
            {
                return;
            }

            // TODO: Duration
            var playerId = affectedPlayer.GetId(this.player);
            using (var writer = this.connection.StartSafeWrite(0xC1, 0x07))
            {
                var packet = writer.Span;
                packet[2] = 0x07;
                packet[3] = isActive ? (byte)1 : (byte)0;
                packet.Slice(4).SetShortSmallEndian(playerId);
                packet[6] = (byte)effect.Id;
                writer.Commit();
            }
        }

        private byte GetGuildMemberStatusCode(GuildPosition? position)
        {
            switch (position)
            {
                case null:
                    return 0xFF;
                case GuildPosition.NormalMember:
                    return 0;
                case GuildPosition.GuildMaster:
                    return 0x80;
                case GuildPosition.BattleMaster:
                    return 0x20;
                default:
                    throw new ArgumentException("GuildPosition not mapped.");
            }
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
                default: return (byte)window;
            }
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

            /// <summary>
            /// Occurs when the appearance of the player changed.
            /// </summary>
            /// <remarks>This never happens in this implementation.</remarks>
            public event EventHandler AppearanceChanged;

            public CharacterClass CharacterClass => this.character?.CharacterClass;

            /// <inheritdoc />
            public CharacterPose Pose => CharacterPose.Standing;

            public bool FullAncientSetEquipped => this.character.HasFullAncientSetEquipped();

            public IEnumerable<ItemAppearance> EquippedItems
            {
                get
                {
                    if (this.character.Inventory != null)
                    {
                        return this.character.Inventory.Items
                            .Where(item => item.ItemSlot <= InventoryConstants.LastEquippableItemSlotIndex)
                            .Select(item => item.GetAppearance());
                    }

                    return Enumerable.Empty<ItemAppearance>();
                }
            }
        }
    }
}