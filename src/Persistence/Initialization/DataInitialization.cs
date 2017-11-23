// <copyright file="DataInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Attributes;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameServer;
    using MUnique.OpenMU.GameServer.MessageHandler;
    using MUnique.OpenMU.GameServer.MessageHandler.Friends;
    using MUnique.OpenMU.GameServer.MessageHandler.Guild;
    using MUnique.OpenMU.GameServer.MessageHandler.Items;
    using MUnique.OpenMU.GameServer.MessageHandler.Party;
    using MUnique.OpenMU.GameServer.MessageHandler.Trade;
    using MUnique.OpenMU.Persistence.Initialization.Items;
    using MUnique.OpenMU.Persistence.Initialization.Maps;

    /// <summary>
    /// Class to manage data initialization.
    /// </summary>
    public class DataInitialization
    {
        private readonly IRepositoryManager repositoryManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataInitialization"/> class.
        /// </summary>
        /// <param name="repositoryManager">The repository manager.</param>
        public DataInitialization(IRepositoryManager repositoryManager)
        {
            this.repositoryManager = repositoryManager;
        }

        /// <summary>
        /// Creates the initial data for a server.
        /// </summary>
        public void CreateInitialData()
        {
            GameConfiguration gameConfiguration;
            using (this.repositoryManager.UseTemporaryContext())
            {
                gameConfiguration = this.repositoryManager.CreateNew<GameConfiguration>();
            }

            using (var context = this.repositoryManager.UseTemporaryContext(gameConfiguration))
            {
                this.InitializeGameConfiguration(gameConfiguration);
                var gameServerConfiguration = this.CreateGameServerConfiguration(gameConfiguration.Maps);
                this.CreateGameServerDefinitions(gameConfiguration, gameServerConfiguration, 3);
                context.SaveChanges();

                var safezone = gameConfiguration.Maps.First(map => map.Number == 0).SpawnGates.First(gate => gate.X1 == 133 && gate.X2 == 151);
                foreach (var map in gameConfiguration.Maps)
                {
                    // set safezone to lorencia for now...
                    map.DeathSafezone = safezone;
                }

                this.CreateTestAccounts(10, gameConfiguration);

                context.SaveChanges();
            }
        }

        private void CreateTestAccounts(int count, GameConfiguration gameConfiguration)
        {
            for (int i = 0; i < count; i++)
            {
                this.CreateTestAccount(i, gameConfiguration);
            }
        }

        private void CreateTestAccount(int index, GameConfiguration gameConfiguration)
        {
            var loginName = "test" + index.ToString();

            var account = this.repositoryManager.CreateNew<Account>();
            account.LoginName = loginName;
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(loginName);
            account.Vault = this.repositoryManager.CreateNew<ItemStorage>();

            var character = this.repositoryManager.CreateNew<Character>();
            account.Characters.Add(character);
            character.CharacterClass = gameConfiguration.CharacterClasses.First(c => c.Number == (byte)CharacterClassNumber.DarkKnight);
            character.Name = loginName;
            character.CharacterSlot = 0;
            character.CreateDate = DateTime.Now;
            character.KeyConfiguration = new byte[30];
            foreach (
                var attribute in
                character.CharacterClass.StatAttributes.Select(
                    a => this.repositoryManager.CreateNew<StatAttribute>(a.Attribute, a.BaseValue)))
            {
                character.Attributes.Add(attribute);
            }

            character.CurrentMap = character.CharacterClass.HomeMap;
            character.PositionX = (byte)Rand.NextInt(character.CurrentMap.DeathSafezone.X1, character.CurrentMap.DeathSafezone.X2);
            character.PositionY = (byte)Rand.NextInt(character.CurrentMap.DeathSafezone.Y1, character.CurrentMap.DeathSafezone.Y2);
            character.Inventory = this.repositoryManager.CreateNew<ItemStorage>();
            var item = this.repositoryManager.CreateNew<Item>();
            item.Definition = gameConfiguration.Items.FirstOrDefault(def => def.Group == 1 && def.Number == 0); // small axe
            item.Durability = item.Definition?.Durability ?? 0;
            item.ItemSlot = 0;
            character.Inventory.Items.Add(item);
            character.Inventory.Money = 1000000;

            // TODO: Some potions and other stuff
        }

        private void CreateNpcs(GameConfiguration gameConfiguration)
        {
            var init = new NpcInitialization(this.repositoryManager, gameConfiguration);
            init.CreateNpcs();
        }

        private PacketHandlerConfiguration CreatePacketConfig<THandler>(PacketType packetType, bool needsEncryption = false)
            where THandler : IPacketHandler
        {
            var config = this.repositoryManager.CreateNew<PacketHandlerConfiguration>();
            config.PacketIdentifier = (byte)packetType;
            config.PacketHandlerClassName = typeof(THandler).AssemblyQualifiedName;
            config.NeedsToBeEncrypted = needsEncryption;
            return config;
        }

        /*
        private void CreateExcellentWeaponOptions()
        {
            var mana8 = new ItemOption
            {
                Number = 1,
                OptionType = ItemOptionTypes.Excellent,
                PowerUpDefinition = new PowerUpDefinition
                {
                    Boost = new PowerUpDefinitionValue
                    {
                        RelatedValues = new List<AttributeRelationship> { new AttributeRelationship { InputAttribute = Stats.MaximumMana, InputOperand = 1.0f / 8.0f, InputOperator = InputOperator.Multiply } }
                    },
                    TargetAttribute = Stats.ManaAfterMonsterKill
                }
            };

            var life8 = new ItemOption
            {
                Number = 2,
                OptionType = ItemOptionTypes.Excellent,
                PowerUpDefinition = new PowerUpDefinition
                {
                    Boost = new PowerUpDefinitionValue
                    {
                        RelatedValues = new[] { new AttributeRelationship { InputAttribute = Stats.MaximumHealth, InputOperand = 1.0f / 8.0f, InputOperator = InputOperator.Multiply } }
                    },
                    TargetAttribute = Stats.HealthAfterMonsterKill
                }
            };

            var speed = new ItemOption
            {
                Number = 3,
                OptionType = ItemOptionTypes.Excellent,
                PowerUpDefinition = new PowerUpDefinition
                {
                    Boost = new PowerUpDefinitionValue
                    {
                        ConstantValue = new SimpleElement { Value = 7, AggregateType = AggregateType.AddRaw }
                    },
                    TargetAttribute = Stats.AttackSpeed
                }
            };

            var dmg2percentPhys = new ItemOption
            {
                Number = 4,
                OptionType = ItemOptionTypes.Excellent,
                PowerUpDefinition = new PowerUpDefinition
                {
                    Boost = new PowerUpDefinitionValue
                    {
                        RelatedValues = new[] { new AttributeRelationship { InputAttribute = Stats.MaximumPhysBaseDmg, InputOperand = 1.02f, InputOperator = InputOperator.Multiply } }
                    },
                    TargetAttribute = Stats.MaximumPhysBaseDmg // hopefully we get no endless loop...
                }
            };

            var dmg2percentWiz = new ItemOption
            {
                Number = 4,
                OptionType = ItemOptionTypes.Excellent,
                PowerUpDefinition = new PowerUpDefinition
                {
                    Boost = new PowerUpDefinitionValue
                    {
                        RelatedValues = new[] { new AttributeRelationship { InputAttribute = Stats.MaximumWizBaseDmg, InputOperand = 1.02f, InputOperator = InputOperator.Multiply } }
                    },
                    TargetAttribute = Stats.MaximumWizBaseDmg
                }
            };

            var dmg2percentCurse = new ItemOption
            {
                Number = 4,
                OptionType = ItemOptionTypes.Excellent,
                PowerUpDefinition = new PowerUpDefinition
                {
                    Boost = new PowerUpDefinitionValue
                    {
                        RelatedValues = new[] { new AttributeRelationship { InputAttribute = Stats.MaximumCurseBaseDmg, InputOperand = 1.02f, InputOperator = InputOperator.Multiply } }
                    },
                    TargetAttribute = Stats.MaximumCurseBaseDmg
                }
            };

            var dmglvl20Phys = new ItemOption
            {
                Number = 5,
                OptionType = ItemOptionTypes.Excellent,
                PowerUpDefinition = new PowerUpDefinition
                {
                    Boost = new PowerUpDefinitionValue
                    {
                        RelatedValues = new[] { new AttributeRelationship { InputAttribute = Stats.Level, InputOperand = 1f / 20f, InputOperator = InputOperator.Multiply } }
                    },
                    TargetAttribute = Stats.MaximumPhysBaseDmg
                }
            };

            var dmglvl20Wiz = new ItemOption
            {
                Number = 5,
                OptionType = ItemOptionTypes.Excellent,
                PowerUpDefinition = new PowerUpDefinition
                {
                    Boost = new PowerUpDefinitionValue
                    {
                        RelatedValues = new[] { new AttributeRelationship { InputAttribute = Stats.Level, InputOperand = 1f / 20f, InputOperator = InputOperator.Multiply } }
                    },
                    TargetAttribute = Stats.MaximumWizBaseDmg
                }
            };

            var dmglvl20Curse = new ItemOption
            {
                Number = 5,
                OptionType = ItemOptionTypes.Excellent,
                PowerUpDefinition = new PowerUpDefinition
                {
                    Boost = new PowerUpDefinitionValue
                    {
                        RelatedValues = new[] { new AttributeRelationship { InputAttribute = Stats.Level, InputOperand = 1f / 20f, InputOperator = InputOperator.Multiply } }
                    },
                    TargetAttribute = Stats.MaximumCurseBaseDmg
                }
            };

            var excellentDmgChance = new ItemOption
            {
                Number = 6,
                OptionType = ItemOptionTypes.Excellent,
                PowerUpDefinition = new PowerUpDefinition
                {
                    Boost = new PowerUpDefinitionValue
                    {
                        ConstantValue = new SimpleElement { Value = 0.1f, AggregateType = AggregateType.AddRaw }
                    },
                    TargetAttribute = Stats.ExcellentDamageChance
                }
            };

            var repository = this.repositoryManager.GetRepository<ItemOption>();
            repository.Store(mana8);
            repository.Store(life8);
            repository.Store(speed);
            repository.Store(dmg2percentCurse);
            repository.Store(dmg2percentPhys);
            repository.Store(dmg2percentWiz);
            repository.Store(dmglvl20Curse);
            repository.Store(dmglvl20Phys);
            repository.Store(dmglvl20Wiz);
            repository.Store(excellentDmgChance);
        }*/

        private ItemOptionDefinition CreateLuckOptionDefinition(GameConfiguration gameConfiguration)
        {
            var definition = this.repositoryManager.CreateNew<ItemOptionDefinition>();

            definition.Name = "Luck";
            definition.AddChance = 0.25f;
            definition.AddsRandomly = true;
            definition.MaximumOptionsPerItem = 1;

            var itemOption = this.repositoryManager.CreateNew<IncreasableItemOption>();
            itemOption.OptionType = gameConfiguration.ItemOptionTypes.FirstOrDefault(o => o == ItemOptionTypes.Luck);
            itemOption.PowerUpDefinition = this.repositoryManager.CreateNew<PowerUpDefinition>();
            itemOption.PowerUpDefinition.TargetAttribute = gameConfiguration.Attributes.FirstOrDefault(a => a == Stats.CriticalDamageChance);
            itemOption.PowerUpDefinition.Boost = this.repositoryManager.CreateNew<PowerUpDefinitionValue>();
            itemOption.PowerUpDefinition.Boost.ConstantValue.Value = 0.05f;
            definition.PossibleOptions.Add(itemOption);

            return definition;
        }

        private ItemOptionDefinition CreateOptionDefinition(GameConfiguration gameConfiguration, AttributeDefinition attributeDefinition)
        {
            var definition = this.repositoryManager.CreateNew<ItemOptionDefinition>();

            definition.Name = "Option";
            definition.AddChance = 0.25f;
            definition.AddsRandomly = true;
            definition.MaximumOptionsPerItem = 4;

            var itemOption = this.repositoryManager.CreateNew<IncreasableItemOption>();
            itemOption.OptionType = gameConfiguration.ItemOptionTypes.FirstOrDefault(o => o == ItemOptionTypes.Option);
            itemOption.PowerUpDefinition = this.repositoryManager.CreateNew<PowerUpDefinition>();
            itemOption.PowerUpDefinition.TargetAttribute = gameConfiguration.Attributes.First(a => a == attributeDefinition);
            itemOption.PowerUpDefinition.Boost = this.repositoryManager.CreateNew<PowerUpDefinitionValue>();
            itemOption.PowerUpDefinition.Boost.ConstantValue.Value = 4;
            definition.PossibleOptions.Add(itemOption);

            return definition;
        }

        private void CreateItemOptionTypes(GameConfiguration gameConfiguration)
        {
            var optionTypes = typeof(ItemOptionTypes)
                .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(p => p.PropertyType == typeof(ItemOptionType))
                .Select(p => p.GetValue(typeof(ItemOptionType)))
                .OfType<ItemOptionType>()
                .ToList();

            foreach (var optionType in optionTypes)
            {
                var persistentOptionType = this.repositoryManager.CreateNew<ItemOptionType>();
                persistentOptionType.Description = optionType.Description;
                persistentOptionType.Id = optionType.Id;
                persistentOptionType.Name = optionType.Name;
                gameConfiguration.ItemOptionTypes.Add(persistentOptionType);
            }
        }

        /// <summary>
        /// Creates the stat attributes.
        /// </summary>
        /// <param name="gameConfiguration">The game configuration.</param>
        private void CreateStatAttributes(GameConfiguration gameConfiguration)
        {
            var attributes = typeof(Stats)
                .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(p => p.PropertyType == typeof(AttributeDefinition))
                .Select(p => p.GetValue(typeof(Stats)))
                .OfType<AttributeDefinition>()
                .ToList();

            foreach (var attribute in attributes)
            {
                var persistentAttribute = this.repositoryManager.CreateNew<AttributeDefinition>(attribute.Id, attribute.Designation, attribute.Description);
                gameConfiguration.Attributes.Add(persistentAttribute);
            }
        }

        private void CreateItemSlotTypes(GameConfiguration gameConfiguration)
        {
            var leftHand = this.repositoryManager.CreateNew<ItemSlotType>();
            leftHand.Description = "Left Hand";
            leftHand.ItemSlots.Add(0);
            gameConfiguration.ItemSlotTypes.Add(leftHand);

            var rightHand = this.repositoryManager.CreateNew<ItemSlotType>();
            rightHand.Description = "Right Hand";
            rightHand.ItemSlots.Add(1);
            gameConfiguration.ItemSlotTypes.Add(rightHand);

            var helm = this.repositoryManager.CreateNew<ItemSlotType>();
            helm.Description = "Helm";
            helm.ItemSlots.Add(2);
            gameConfiguration.ItemSlotTypes.Add(helm);

            var armor = this.repositoryManager.CreateNew<ItemSlotType>();
            armor.Description = "Armor";
            armor.ItemSlots.Add(3);
            gameConfiguration.ItemSlotTypes.Add(armor);

            var pants = this.repositoryManager.CreateNew<ItemSlotType>();
            pants.Description = "Pants";
            pants.ItemSlots.Add(4);
            gameConfiguration.ItemSlotTypes.Add(pants);

            var gloves = this.repositoryManager.CreateNew<ItemSlotType>();
            gloves.Description = "Gloves";
            gloves.ItemSlots.Add(5);
            gameConfiguration.ItemSlotTypes.Add(gloves);

            var boots = this.repositoryManager.CreateNew<ItemSlotType>();
            boots.Description = "Boots";
            boots.ItemSlots.Add(6);
            gameConfiguration.ItemSlotTypes.Add(boots);

            var wings = this.repositoryManager.CreateNew<ItemSlotType>();
            wings.Description = "Wings";
            wings.ItemSlots.Add(7);
            gameConfiguration.ItemSlotTypes.Add(wings);

            var pet = this.repositoryManager.CreateNew<ItemSlotType>();
            pet.Description = "Pet";
            pet.ItemSlots.Add(8);
            gameConfiguration.ItemSlotTypes.Add(pet);

            var pendant = this.repositoryManager.CreateNew<ItemSlotType>();
            pendant.Description = "Pendant";
            pendant.ItemSlots.Add(9);
            gameConfiguration.ItemSlotTypes.Add(pendant);

            var ring = this.repositoryManager.CreateNew<ItemSlotType>();
            ring.Description = "Ring";
            ring.ItemSlots.Add(10);
            ring.ItemSlots.Add(11);
            gameConfiguration.ItemSlotTypes.Add(ring);
        }

        private void CreateGameServerDefinitions(GameConfiguration gameConfiguration, GameServerConfiguration gameServerConfiguration, int numberOfServers)
        {
            for (int i = 0; i < numberOfServers; i++)
            {
                var server = this.repositoryManager.CreateNew<GameServerDefinition>();
                server.ServerID = (byte)i;
                server.Description = $"Server {i}";
                server.NetworkPort = 55901 + i;
                server.GameConfiguration = gameConfiguration;
                server.ServerConfiguration = gameServerConfiguration;
            }
        }

        private void CreateGameMapDefinitions(GameConfiguration gameConfiguration)
        {
            var lorencia = new Lorencia();
            gameConfiguration.Maps.Add(lorencia.Initialize(this.repositoryManager, gameConfiguration));

            string[] mapNames =
            {
                "Lorencia", "Dungeon", "Devias", "Noria", "Lost_Tower", "Exile", "Arena", "Atlans", "Tarkan", "Devil_Square (1-4)", "Icarus", // 10
                "Blood_Castle 1", "Blood_Castle 2", "Blood_Castle 3", "Blood_Castle 4", "Blood_Castle 5", "Blood_Castle 6", "Blood_Castle 7", "Chaos_Castle 1", "Chaos_Castle 2", "Chaos_Castle 3", // 20
                "Chaos_Castle 4", "Chaos_Castle 5", "Chaos_Castle 6", "Kalima 1", "Kalima 2", "Kalima 3", "Kalima 4", "Kalima 5", "Kalima 6", "Valley of Loren", // 30
                "Land_of_Trials", "Devil_Square (5-6)", "Aida", "Crywolf Fortress", "?", "Kalima 7", "Kanturu_I", "Kanturu_III", "Kanturu_Event", "Silent Map?", // 40
                "Barracks of Balgass", "Balgass Refuge", "?", "?", "Illusion_Temple 1", "Illusion_Temple 2", "Illusion_Temple 3", "Illusion_Temple 4", "Illusion_Temple 5", "Illusion_Temple 6", // 50
                "Elvenland", "Blood_Castle 8", "Chaos_Castle 7", "?", "?", "Swamp_Of Calmness", "LaCleon", "LaCleonBoss", "?", "?", // 60
                "?", "Santa Village", "Vulcanus", "Duel Arena", "Double Gear 1", "Double Gear 2", "Double Gear 3", "Double Gear 4", "Empire Fortress 1", // 69
                "Empire Fortress 2", "Empire Fortress 3", "Empire Fortress 4", "Empire Fortress 5", "?", "?", "?", "?", "?", "LorenMarket", // 79
                "Karutan1", "Karutan2"
            };

            var skipCount = 1;
            mapNames.Skip(skipCount).Select((mapName, i) =>
                {
                    var map = this.repositoryManager.CreateNew<GameMapDefinition>();

                    map.Name = mapNames[i + skipCount];
                    map.Number = (short)(i + skipCount);
                    map.ExpMultiplier = 1;
                    map.TerrainData = Terrains.ResourceManager.GetObject("Terrain" + (i + 1 + skipCount).ToString()) as byte[];
                    return map;
                })
                .Where(map => map.Number > 0 && map.Name != "?")
                .ForEach(map => gameConfiguration.Maps.Add(map));
        }

        private GameServerConfiguration CreateGameServerConfiguration(ICollection<GameMapDefinition> maps)
        {
            var gameServerConfiguration = this.repositoryManager.CreateNew<GameServerConfiguration>();
            gameServerConfiguration.MaximumNPCs = 20000;
            gameServerConfiguration.MaximumPlayers = 1000;

            var mainPacketHandlerConfig = this.repositoryManager.CreateNew<MainPacketHandlerConfiguration>();
            mainPacketHandlerConfig.ClientVersion = new byte[] { 0x31, 0x30, 0x34, 0x30, 0x34 };
            mainPacketHandlerConfig.ClientSerial = Encoding.UTF8.GetBytes("k1Pk2jcET48mxL3b");

            this.CreatePacketHandlerConfiguration().ToList().ForEach(mainPacketHandlerConfig.PacketHandlers.Add);
            gameServerConfiguration.SupportedPacketHandlers.Add(mainPacketHandlerConfig);

            // by default we add every map to a server configuration
            foreach (var map in maps)
            {
                gameServerConfiguration.Maps.Add(map);
            }

            return gameServerConfiguration;
        }

        private IEnumerable<PacketHandlerConfiguration> CreatePacketHandlerConfiguration()
        {
            yield return this.CreatePacketConfig<ChatMessageHandler>(PacketType.Speak);
            yield return this.CreatePacketConfig<ChatMessageHandler>(PacketType.Whisper);
            yield return this.CreatePacketConfig<LoginHandler>(PacketType.LoginLogoutGroup);
            yield return this.CreatePacketConfig<StoreHandler>(PacketType.PersonalShopGroup);
            yield return this.CreatePacketConfig<PickupItemHandler>(PacketType.PickupItem);
            yield return this.CreatePacketConfig<DropItemHandler>(PacketType.DropItem);
            yield return this.CreatePacketConfig<ItemMoveHandler>(PacketType.InventoryMove);
            yield return this.CreatePacketConfig<ConsumeItemHandler>(PacketType.ConsumeItem);
            yield return this.CreatePacketConfig<TalkNpcHandler>(PacketType.TalkNPC);
            yield return this.CreatePacketConfig<CloseNPCHandler>(PacketType.CloseNPC);
            yield return this.CreatePacketConfig<BuyNPCItemHandler>(PacketType.BuyNPCItem);
            yield return this.CreatePacketConfig<SellItemToNPCHandler>(PacketType.SellNPCItem);
            yield return this.CreatePacketConfig<WarpS54Handler>(PacketType.WarpCommand);
            yield return this.CreatePacketConfig<WarpGateHandler>(PacketType.WarpGate);
            yield return this.CreatePacketConfig<WarehouseCloseHandler>(PacketType.VaultClose);
            yield return this.CreatePacketConfig<JewelMixHandler>(PacketType.JewelMix);

            yield return this.CreatePacketConfig<PartyListRequestHandler>(PacketType.RequestPartyList);
            yield return this.CreatePacketConfig<PartyKickHandler>(PacketType.PartyKick);
            yield return this.CreatePacketConfig<PartyRequestHandler>(PacketType.PartyRequest);
            yield return this.CreatePacketConfig<PartyResponseHandler>(PacketType.PartyRequestAnswer);

            yield return this.CreatePacketConfig<CharacterMoveHandler>(PacketType.Walk);
            yield return this.CreatePacketConfig<CharacterMoveHandler>(PacketType.Teleport);
            yield return this.CreatePacketConfig<AnimationHandler>(PacketType.Animation);
            yield return this.CreatePacketConfig<CharacterGroupHandler>(PacketType.CharacterGroup);

            yield return this.CreatePacketConfig<HitHandler>(PacketType.Hit);
            yield return this.CreatePacketConfig<TargettedSkillHandler>(PacketType.SkillAttack);
            yield return this.CreatePacketConfig<AreaSkillAttackHandler>(PacketType.AreaSkill);
            yield return this.CreatePacketConfig<AreaSkillHitHandler>(PacketType.AreaSkillHit);

            yield return this.CreatePacketConfig<TradeCancelHandler>(PacketType.TradeCancel);
            yield return this.CreatePacketConfig<TradeButtonHandler>(PacketType.TradeButton);
            yield return this.CreatePacketConfig<TradeRequestHandler>(PacketType.TradeRequest);
            yield return this.CreatePacketConfig<TradeAcceptHandler>(PacketType.TradeAccept);
            yield return this.CreatePacketConfig<TradeMoneyHandler>(PacketType.TradeMoney);
            yield return this.CreatePacketConfig<LetterDeleteHandler>(PacketType.FriendMemoDelete);
            yield return this.CreatePacketConfig<LetterSendHandler>(PacketType.FriendMemoSend);
            yield return this.CreatePacketConfig<LetterReadRequestHandler>(PacketType.FriendMemoReadRequest);
            yield return this.CreatePacketConfig<GuildKickPlayerHandler>(PacketType.GuildKickPlayer);
            yield return this.CreatePacketConfig<GuildRequestHandler>(PacketType.GuildJoinRequest);
            yield return this.CreatePacketConfig<GuildRequestAnswerHandler>(PacketType.GuildJoinAnswer);
            yield return this.CreatePacketConfig<GuildListRequestHandler>(PacketType.RequestGuildList);
            yield return this.CreatePacketConfig<GuildCreateHandler>(PacketType.GuildMasterInfoSave);
            yield return this.CreatePacketConfig<GuildMasterAnswerHandler>(PacketType.GuildMasterAnswer);
            yield return this.CreatePacketConfig<GuildInfoRequestHandler>(PacketType.GuildInfoRequest);

            yield return this.CreatePacketConfig<ItemRepairHandler>(PacketType.ItemRepair);
            yield return this.CreatePacketConfig<ChaosMixHandler>(PacketType.ChaosMachineMix);
            yield return this.CreatePacketConfig<AddFriendHandler>(PacketType.FriendAdd);
            yield return this.CreatePacketConfig<DeleteFriendHandler>(PacketType.FriendDelete);
            yield return this.CreatePacketConfig<ChatRequestHandler>(PacketType.ChatRoomCreate);
            yield return this.CreatePacketConfig<FriendAddResponseHandler>(PacketType.FriendAddReponse);
            yield return this.CreatePacketConfig<ChangeOnlineStateHandler>(PacketType.FriendStateClient);
        }

        private void InitializeGameConfiguration(GameConfiguration gameConfiguration)
        {
            gameConfiguration.MaximumLevel = 400;
            gameConfiguration.InfoRange = 12;
            gameConfiguration.AreaSkillHitsPlayer = false;
            gameConfiguration.MaximumInventoryMoney = int.MaxValue;
            gameConfiguration.RecoveryInterval = 3000;
            gameConfiguration.MaximumLetters = 50;
            gameConfiguration.MaximumCharactersPerAccount = 5;
            gameConfiguration.CharacterNameRegex = "^[a-zA-Z0-9]{3,10}$";
            gameConfiguration.MaximumPasswordLength = 20;
            gameConfiguration.MaximumPartySize = 5;
            var moneyDropItemGroup = this.repositoryManager.CreateNew<DropItemGroup>();
            moneyDropItemGroup.Chance = 0.5;
            moneyDropItemGroup.ItemType = SpecialItemType.Money;
            gameConfiguration.BaseDropItemGroups.Add(moneyDropItemGroup);
            this.CreateStatAttributes(gameConfiguration);

            this.CreateNpcs(gameConfiguration);
            this.CreateGameMapDefinitions(gameConfiguration);
            this.CreateItemSlotTypes(gameConfiguration);
            this.CreateItemOptionTypes(gameConfiguration);
            gameConfiguration.ItemOptions.Add(this.CreateLuckOptionDefinition(gameConfiguration));
            gameConfiguration.ItemOptions.Add(this.CreateOptionDefinition(gameConfiguration, Stats.DefenseBase));
            gameConfiguration.ItemOptions.Add(this.CreateOptionDefinition(gameConfiguration, Stats.MaximumPhysBaseDmg));
            gameConfiguration.ItemOptions.Add(this.CreateOptionDefinition(gameConfiguration, Stats.MaximumWizBaseDmg));

            // TODO: Excellent Options
            new Gates().Initialize(this.repositoryManager, gameConfiguration);
            new CharacterClassInitialization(this.repositoryManager, gameConfiguration).CreateCharacterClasses();
            var setHelper = new SetItemHelper(this.repositoryManager, gameConfiguration);
            setHelper.CreateSets();
            var weaponHelper = new WeaponItemHelper(this.repositoryManager, gameConfiguration);
            weaponHelper.InitializeWeapons();
            //// TODO: ItemSetGroups
            //// TODO: MagicEffects
            //// TODO: MasterSkillRoots
        }
    }
}
