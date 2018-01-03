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
        private GameConfiguration gameConfiguration;

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
            using (this.repositoryManager.UseTemporaryContext())
            {
                this.gameConfiguration = this.repositoryManager.CreateNew<GameConfiguration>();
            }

            using (var context = this.repositoryManager.UseTemporaryContext(this.gameConfiguration))
            {
                this.InitializeGameConfiguration();
                var gameServerConfiguration = this.CreateGameServerConfiguration(this.gameConfiguration.Maps);
                this.CreateGameServerDefinitions(gameServerConfiguration, 3);
                context.SaveChanges();

                var safezone = this.gameConfiguration.Maps.First(map => map.Number == 0).SpawnGates.First(gate => gate.X1 == 133 && gate.X2 == 151);
                foreach (var map in this.gameConfiguration.Maps)
                {
                    // set safezone to lorencia for now...
                    map.DeathSafezone = safezone;
                }

                this.CreateTestAccounts(10);

                context.SaveChanges();
            }
        }

        private void CreateTestAccounts(int count)
        {
            for (int i = 0; i < count; i++)
            {
                this.CreateTestAccount(i);
            }
        }

        private void CreateTestAccount(int index)
        {
            var loginName = "test" + index.ToString();

            var account = this.repositoryManager.CreateNew<Account>();
            account.LoginName = loginName;
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(loginName);
            account.Vault = this.repositoryManager.CreateNew<ItemStorage>();

            var character = this.repositoryManager.CreateNew<Character>();
            account.Characters.Add(character);
            character.CharacterClass = this.gameConfiguration.CharacterClasses.First(c => c.Number == (byte)CharacterClassNumber.DarkKnight);
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
            character.Attributes.First(a => a.Definition == Stats.Level).Value = (index * 10) + 1;
            character.Inventory = this.repositoryManager.CreateNew<ItemStorage>();
            character.Inventory.Money = 1000000;
            character.Inventory.Items.Add(this.CreateSmallAxe(0));
            character.Inventory.Items.Add(this.CreateJewelOfBless(12));
            character.Inventory.Items.Add(this.CreateJewelOfBless(13));
            character.Inventory.Items.Add(this.CreateJewelOfBless(14));
            character.Inventory.Items.Add(this.CreateJewelOfBless(15));
            character.Inventory.Items.Add(this.CreateJewelOfBless(16));
            character.Inventory.Items.Add(this.CreateJewelOfBless(17));
            character.Inventory.Items.Add(this.CreateJewelOfBless(18));
            character.Inventory.Items.Add(this.CreateJewelOfBless(19));
            character.Inventory.Items.Add(this.CreateJewelOfSoul(20));
            character.Inventory.Items.Add(this.CreateJewelOfSoul(21));
            character.Inventory.Items.Add(this.CreateJewelOfSoul(22));
            character.Inventory.Items.Add(this.CreateJewelOfSoul(23));
            character.Inventory.Items.Add(this.CreateJewelOfSoul(24));
            character.Inventory.Items.Add(this.CreateJewelOfSoul(25));
            character.Inventory.Items.Add(this.CreateJewelOfSoul(26));
            character.Inventory.Items.Add(this.CreateJewelOfSoul(27));
            character.Inventory.Items.Add(this.CreateJewelOfLife(28));
            character.Inventory.Items.Add(this.CreateJewelOfLife(29));
            character.Inventory.Items.Add(this.CreateJewelOfLife(30));
            character.Inventory.Items.Add(this.CreateJewelOfLife(31));
            character.Inventory.Items.Add(this.CreateJewelOfLife(32));
            character.Inventory.Items.Add(this.CreateJewelOfLife(33));
            character.Inventory.Items.Add(this.CreateJewelOfLife(34));
            character.Inventory.Items.Add(this.CreateJewelOfLife(35));
            character.Inventory.Items.Add(this.CreateHealthPotion(36, 0));
            character.Inventory.Items.Add(this.CreateHealthPotion(37, 1));
            character.Inventory.Items.Add(this.CreateHealthPotion(38, 2));
            character.Inventory.Items.Add(this.CreateHealthPotion(39, 3));
            character.Inventory.Items.Add(this.CreateManaPotion(40, 0));
            character.Inventory.Items.Add(this.CreateManaPotion(41, 1));
            character.Inventory.Items.Add(this.CreateManaPotion(42, 2));
            character.Inventory.Items.Add(this.CreateAlcohol(43));
            character.Inventory.Items.Add(this.CreateShieldPotion(44, 0));
            character.Inventory.Items.Add(this.CreateShieldPotion(45, 1));
            character.Inventory.Items.Add(this.CreateShieldPotion(46, 2));
        }

        private Item CreateAlcohol(byte itemSlot)
        {
            var potion = this.repositoryManager.CreateNew<Item>();
            potion.Definition = this.gameConfiguration.Items.FirstOrDefault(def => def.Group == 14 && def.Number == 9);
            potion.Durability = 1;
            potion.ItemSlot = itemSlot;
            return potion;
        }

        private Item CreateManaPotion(byte itemSlot, byte size)
        {
            return this.CreatePotion(itemSlot, (byte)(size + 4));
        }

        private Item CreateHealthPotion(byte itemSlot, byte size)
        {
            return this.CreatePotion(itemSlot, size);
        }

        private Item CreateShieldPotion(byte itemSlot, byte size)
        {
            return this.CreatePotion(itemSlot, (byte)(size + 35));
        }

        private Item CreatePotion(byte itemSlot, byte itemNumber)
        {
            var potion = this.repositoryManager.CreateNew<Item>();
            potion.Definition = this.gameConfiguration.Items.FirstOrDefault(def => def.Group == 14 && def.Number == itemNumber);
            potion.Durability = 3; // Stack of 3 Potions
            potion.ItemSlot = itemSlot;
            return potion;
        }

        private Item CreateJewelOfBless(byte itemSlot)
        {
            return this.CreateJewel(itemSlot, 13);
        }

        private Item CreateJewelOfSoul(byte itemSlot)
        {
            return this.CreateJewel(itemSlot, 14);
        }

        private Item CreateJewelOfLife(byte itemSlot)
        {
            return this.CreateJewel(itemSlot, 16);
        }

        private Item CreateJewel(byte itemSlot, byte itemNumber)
        {
            var jewel = this.repositoryManager.CreateNew<Item>();
            jewel.Definition = this.gameConfiguration.Items.FirstOrDefault(def => def.Group == 14 && def.Number == itemNumber);
            jewel.Durability = 1;
            jewel.ItemSlot = itemSlot;
            return jewel;
        }

        private Item CreateSmallAxe(byte itemSlot)
        {
            var smallAxe = this.repositoryManager.CreateNew<Item>();
            smallAxe.Definition = this.gameConfiguration.Items.FirstOrDefault(def => def.Group == 1 && def.Number == 0); // small axe
            smallAxe.Durability = smallAxe.Definition?.Durability ?? 0;
            smallAxe.ItemSlot = itemSlot;
            return smallAxe;
        }

        private void CreateNpcs()
        {
            var init = new NpcInitialization(this.repositoryManager, this.gameConfiguration);
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

        private ItemOptionDefinition CreateLuckOptionDefinition()
        {
            var definition = this.repositoryManager.CreateNew<ItemOptionDefinition>();

            definition.Name = "Luck";
            definition.AddChance = 0.25f;
            definition.AddsRandomly = true;
            definition.MaximumOptionsPerItem = 1;

            var itemOption = this.repositoryManager.CreateNew<IncreasableItemOption>();
            itemOption.OptionType = this.gameConfiguration.ItemOptionTypes.FirstOrDefault(o => o == ItemOptionTypes.Luck);
            itemOption.PowerUpDefinition = this.repositoryManager.CreateNew<PowerUpDefinition>();
            itemOption.PowerUpDefinition.TargetAttribute = this.gameConfiguration.Attributes.FirstOrDefault(a => a == Stats.CriticalDamageChance);
            itemOption.PowerUpDefinition.Boost = this.repositoryManager.CreateNew<PowerUpDefinitionValue>();
            itemOption.PowerUpDefinition.Boost.ConstantValue.Value = 0.05f;
            definition.PossibleOptions.Add(itemOption);

            return definition;
        }

        private ItemOptionDefinition CreateOptionDefinition(AttributeDefinition attributeDefinition)
        {
            var definition = this.repositoryManager.CreateNew<ItemOptionDefinition>();

            definition.Name = "Option";
            definition.AddChance = 0.25f;
            definition.AddsRandomly = true;
            definition.MaximumOptionsPerItem = 4;

            var itemOption = this.repositoryManager.CreateNew<IncreasableItemOption>();
            itemOption.OptionType = this.gameConfiguration.ItemOptionTypes.FirstOrDefault(o => o == ItemOptionTypes.Option);
            itemOption.PowerUpDefinition = this.repositoryManager.CreateNew<PowerUpDefinition>();
            itemOption.PowerUpDefinition.TargetAttribute = this.gameConfiguration.Attributes.First(a => a == attributeDefinition);
            itemOption.PowerUpDefinition.Boost = this.repositoryManager.CreateNew<PowerUpDefinitionValue>();
            itemOption.PowerUpDefinition.Boost.ConstantValue.Value = 4;
            definition.PossibleOptions.Add(itemOption);

            return definition;
        }

        private void CreateItemOptionTypes()
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
                this.gameConfiguration.ItemOptionTypes.Add(persistentOptionType);
            }
        }

        /// <summary>
        /// Creates the stat attributes.
        /// </summary>
        private void CreateStatAttributes()
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
                this.gameConfiguration.Attributes.Add(persistentAttribute);
            }
        }

        private void CreateItemSlotTypes()
        {
            var leftHand = this.repositoryManager.CreateNew<ItemSlotType>();
            leftHand.Description = "Left Hand";
            leftHand.ItemSlots.Add(0);
            this.gameConfiguration.ItemSlotTypes.Add(leftHand);

            var rightHand = this.repositoryManager.CreateNew<ItemSlotType>();
            rightHand.Description = "Right Hand";
            rightHand.ItemSlots.Add(1);
            this.gameConfiguration.ItemSlotTypes.Add(rightHand);

            var helm = this.repositoryManager.CreateNew<ItemSlotType>();
            helm.Description = "Helm";
            helm.ItemSlots.Add(2);
            this.gameConfiguration.ItemSlotTypes.Add(helm);

            var armor = this.repositoryManager.CreateNew<ItemSlotType>();
            armor.Description = "Armor";
            armor.ItemSlots.Add(3);
            this.gameConfiguration.ItemSlotTypes.Add(armor);

            var pants = this.repositoryManager.CreateNew<ItemSlotType>();
            pants.Description = "Pants";
            pants.ItemSlots.Add(4);
            this.gameConfiguration.ItemSlotTypes.Add(pants);

            var gloves = this.repositoryManager.CreateNew<ItemSlotType>();
            gloves.Description = "Gloves";
            gloves.ItemSlots.Add(5);
            this.gameConfiguration.ItemSlotTypes.Add(gloves);

            var boots = this.repositoryManager.CreateNew<ItemSlotType>();
            boots.Description = "Boots";
            boots.ItemSlots.Add(6);
            this.gameConfiguration.ItemSlotTypes.Add(boots);

            var wings = this.repositoryManager.CreateNew<ItemSlotType>();
            wings.Description = "Wings";
            wings.ItemSlots.Add(7);
            this.gameConfiguration.ItemSlotTypes.Add(wings);

            var pet = this.repositoryManager.CreateNew<ItemSlotType>();
            pet.Description = "Pet";
            pet.ItemSlots.Add(8);
            this.gameConfiguration.ItemSlotTypes.Add(pet);

            var pendant = this.repositoryManager.CreateNew<ItemSlotType>();
            pendant.Description = "Pendant";
            pendant.ItemSlots.Add(9);
            this.gameConfiguration.ItemSlotTypes.Add(pendant);

            var ring = this.repositoryManager.CreateNew<ItemSlotType>();
            ring.Description = "Ring";
            ring.ItemSlots.Add(10);
            ring.ItemSlots.Add(11);
            this.gameConfiguration.ItemSlotTypes.Add(ring);
        }

        private void CreateGameServerDefinitions(GameServerConfiguration gameServerConfiguration, int numberOfServers)
        {
            for (int i = 0; i < numberOfServers; i++)
            {
                var server = this.repositoryManager.CreateNew<GameServerDefinition>();
                server.ServerID = (byte)i;
                server.Description = $"Server {i}";
                server.NetworkPort = 55901 + i;
                server.GameConfiguration = this.gameConfiguration;
                server.ServerConfiguration = gameServerConfiguration;
            }
        }

        private void CreateGameMapDefinitions()
        {
            var lorencia = new Lorencia();
            this.gameConfiguration.Maps.Add(lorencia.Initialize(this.repositoryManager, this.gameConfiguration));

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
                .ForEach(map => this.gameConfiguration.Maps.Add(map));
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

        private void InitializeGameConfiguration()
        {
            this.gameConfiguration.MaximumLevel = 400;
            this.gameConfiguration.InfoRange = 12;
            this.gameConfiguration.AreaSkillHitsPlayer = false;
            this.gameConfiguration.MaximumInventoryMoney = int.MaxValue;
            this.gameConfiguration.RecoveryInterval = 3000;
            this.gameConfiguration.MaximumLetters = 50;
            this.gameConfiguration.MaximumCharactersPerAccount = 5;
            this.gameConfiguration.CharacterNameRegex = "^[a-zA-Z0-9]{3,10}$";
            this.gameConfiguration.MaximumPasswordLength = 20;
            this.gameConfiguration.MaximumPartySize = 5;
            var moneyDropItemGroup = this.repositoryManager.CreateNew<DropItemGroup>();
            moneyDropItemGroup.Chance = 0.5;
            moneyDropItemGroup.ItemType = SpecialItemType.Money;
            this.gameConfiguration.BaseDropItemGroups.Add(moneyDropItemGroup);
            this.CreateStatAttributes();

            this.CreateItemSlotTypes();
            this.CreateItemOptionTypes();
            this.gameConfiguration.ItemOptions.Add(this.CreateLuckOptionDefinition());
            this.gameConfiguration.ItemOptions.Add(this.CreateOptionDefinition(Stats.DefenseBase));
            this.gameConfiguration.ItemOptions.Add(this.CreateOptionDefinition(Stats.MaximumPhysBaseDmg));
            this.gameConfiguration.ItemOptions.Add(this.CreateOptionDefinition(Stats.MaximumWizBaseDmg));
            //// TODO: Excellent Options

            new CharacterClassInitialization(this.repositoryManager, this.gameConfiguration).CreateCharacterClasses();
            var setHelper = new SetItemHelper(this.repositoryManager, this.gameConfiguration);
            setHelper.CreateSets();
            var weaponHelper = new WeaponItemHelper(this.repositoryManager, this.gameConfiguration);
            weaponHelper.InitializeWeapons();
            new Potions(this.repositoryManager, this.gameConfiguration).Initialize();
            new Jewels(this.repositoryManager, this.gameConfiguration).Initialize();
            this.CreateNpcs();
            this.CreateGameMapDefinitions();
            this.AssignCharacterClassHomeMaps();
            new Gates().Initialize(this.repositoryManager, this.gameConfiguration);
            //// TODO: ItemSetGroups
            //// TODO: MagicEffects
            //// TODO: MasterSkillRoots
        }

        private void AssignCharacterClassHomeMaps()
        {
            foreach (var characterClass in this.gameConfiguration.CharacterClasses)
            {
                byte mapNumber;
                switch ((CharacterClassNumber)characterClass.Number)
                {
                    case CharacterClassNumber.FairyElf:
                    case CharacterClassNumber.HighElf:
                    case CharacterClassNumber.MuseElf:
                        mapNumber = 3; // Noria
                        break;
                    default:
                        mapNumber = Lorencia.Number;
                        break;
                }

                characterClass.HomeMap = this.gameConfiguration.Maps.First(map => map.Number == mapNumber);
            }
        }
    }
}
