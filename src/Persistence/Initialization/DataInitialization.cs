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
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Attributes;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Resets;
    using MUnique.OpenMU.GameServer;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
    using MUnique.OpenMU.Persistence.Initialization.Items;
    using MUnique.OpenMU.Persistence.Initialization.Maps;
    using MUnique.OpenMU.Persistence.Initialization.Skills;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Class to manage data initialization.
    /// </summary>
    public class DataInitialization
    {
        private readonly IPersistenceContextProvider persistenceContextProvider;
        private readonly ILoggerFactory loggerFactory;
        private GameConfiguration gameConfiguration;
        private IContext context;
        private IList<Maps.IMapInitializer> mapInitializers;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataInitialization" /> class.
        /// </summary>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public DataInitialization(IPersistenceContextProvider persistenceContextProvider, ILoggerFactory loggerFactory)
        {
            this.persistenceContextProvider = persistenceContextProvider;
            this.loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Creates the initial data for a server.
        /// </summary>
        public void CreateInitialData()
        {
            BaseMapInitializer.ClearDefaultDropItemGroups();
            using (var temporaryContext = this.persistenceContextProvider.CreateNewContext())
            {
                this.gameConfiguration = temporaryContext.CreateNew<GameConfiguration>();
                temporaryContext.SaveChanges();
            }

            using (this.context = this.persistenceContextProvider.CreateNewContext(this.gameConfiguration))
            {
                this.CreateGameClientDefinitions();
                this.CreateChatServerDefinition();
                this.InitializeGameConfiguration();

                var gameServerConfiguration = this.CreateGameServerConfiguration(this.gameConfiguration.Maps);
                this.CreateGameServerDefinitions(gameServerConfiguration, 3);
                this.CreateConnectServerDefinitions();
                this.context.SaveChanges();
                this.SetSafezoneMaps();
                this.CreateTestAccounts();

                if (!AppDomain.CurrentDomain.GetAssemblies().Contains(typeof(GameServer).Assembly))
                {
                    // should never happen, but the access to the GameServer type is a trick to load the assembly into the current domain.
                }

                var plugInManager = new PlugInManager(null, this.loggerFactory, null);
                plugInManager.DiscoverAndRegisterPlugIns();
                plugInManager.KnownPlugInTypes.ForEach(plugInType =>
                {
                    var plugInConfiguration = this.context.CreateNew<PlugInConfiguration>();
                    plugInConfiguration.TypeId = plugInType.GUID;
                    plugInConfiguration.IsActive = true;
                    this.gameConfiguration.PlugInConfigurations.Add(plugInConfiguration);

                    // Resets are disabled by default.
                    if (plugInType == typeof(ResetFeaturePlugIn))
                    {
                        plugInConfiguration.IsActive = false;
                        plugInConfiguration.SetConfiguration(new ResetConfiguration());
                    }
                });

                this.context.SaveChanges();
            }
        }

        /// <summary>
        /// Calculates the needed experience for the specified character level.
        /// </summary>
        /// <param name="level">The character level.</param>
        /// <returns>The calculated needed experience.</returns>
        internal static long CalculateNeededExperience(long level)
        {
            if (level == 0)
            {
                return 0;
            }

            if (level < 256)
            {
                return 10 * (level + 8) * (level - 1) * (level - 1);
            }

            return (10 * (level + 8) * (level - 1) * (level - 1)) + (1000 * (level - 247) * (level - 256) * (level - 256));
        }

        private void CreateTestAccounts()
        {
            for (int i = 0; i < 10; i++)
            {
                var level = (i * 10) + 1;
                new TestAccounts.LowLevel(this.context, this.gameConfiguration, "test" + i, level).Initialize();
            }

            new TestAccounts.Level300(this.context, this.gameConfiguration).Initialize();
            new TestAccounts.Level400(this.context, this.gameConfiguration).Initialize();
            new TestAccounts.Ancient(this.context, this.gameConfiguration).Initialize();
            new TestAccounts.Socket(this.context, this.gameConfiguration).Initialize();
            new TestAccounts.Quest150(this.context, this.gameConfiguration).Initialize();
            new TestAccounts.Quest220(this.context, this.gameConfiguration).Initialize();
            new TestAccounts.Quest400(this.context, this.gameConfiguration).Initialize();
            new TestAccounts.GameMaster(this.context, this.gameConfiguration).Initialize();
        }

        private long CalcNeededMasterExp(long lvl)
        {
            // f(x) = 505 * x^3 + 35278500 * x + 228045 * x^2
            return (505 * lvl * lvl * lvl) + (35278500 * lvl) + (228045 * lvl * lvl);
        }

        private void CreateNpcs()
        {
            var init = new NpcInitialization(this.context, this.gameConfiguration);
            init.CreateNpcs();
        }

        private ItemOptionDefinition CreateLuckOptionDefinition()
        {
            var definition = this.context.CreateNew<ItemOptionDefinition>();

            definition.Name = "Luck";
            definition.AddChance = 0.25f;
            definition.AddsRandomly = true;
            definition.MaximumOptionsPerItem = 1;

            var itemOption = this.context.CreateNew<IncreasableItemOption>();
            itemOption.OptionType = this.gameConfiguration.ItemOptionTypes.FirstOrDefault(o => o == ItemOptionTypes.Luck);
            itemOption.PowerUpDefinition = this.context.CreateNew<PowerUpDefinition>();
            itemOption.PowerUpDefinition.TargetAttribute = this.gameConfiguration.Attributes.FirstOrDefault(a => a == Stats.CriticalDamageChance);
            itemOption.PowerUpDefinition.Boost = this.context.CreateNew<PowerUpDefinitionValue>();
            itemOption.PowerUpDefinition.Boost.ConstantValue.Value = 0.05f;
            definition.PossibleOptions.Add(itemOption);

            return definition;
        }

        private ItemOptionDefinition CreateOptionDefinition(AttributeDefinition attributeDefinition)
        {
            var definition = this.context.CreateNew<ItemOptionDefinition>();

            definition.Name = attributeDefinition.Designation + " Option";
            definition.AddChance = 0.25f;
            definition.AddsRandomly = true;
            definition.MaximumOptionsPerItem = 1;

            var itemOption = this.context.CreateNew<IncreasableItemOption>();
            itemOption.OptionType = this.gameConfiguration.ItemOptionTypes.FirstOrDefault(o => o == ItemOptionTypes.Option);
            itemOption.PowerUpDefinition = this.context.CreateNew<PowerUpDefinition>();
            itemOption.PowerUpDefinition.TargetAttribute = this.gameConfiguration.Attributes.First(a => a == attributeDefinition);
            itemOption.PowerUpDefinition.Boost = this.context.CreateNew<PowerUpDefinitionValue>();
            itemOption.PowerUpDefinition.Boost.ConstantValue.Value = 4;
            for (int level = 2; level <= 4; level++)
            {
                var levelDependentOption = this.context.CreateNew<ItemOptionOfLevel>();
                levelDependentOption.Level = level;
                var powerUpDefinition = this.context.CreateNew<PowerUpDefinition>();
                powerUpDefinition.TargetAttribute = itemOption.PowerUpDefinition.TargetAttribute;
                powerUpDefinition.Boost = this.context.CreateNew<PowerUpDefinitionValue>();
                powerUpDefinition.Boost.ConstantValue.Value = level * 4;
                levelDependentOption.PowerUpDefinition = powerUpDefinition;
                itemOption.LevelDependentOptions.Add(levelDependentOption);
            }

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
                var persistentOptionType = this.context.CreateNew<ItemOptionType>();
                persistentOptionType.Description = optionType.Description;
                persistentOptionType.Id = optionType.Id;
                persistentOptionType.Name = optionType.Name;
                persistentOptionType.IsVisible = optionType.IsVisible;
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
                var persistentAttribute = this.context.CreateNew<AttributeDefinition>(attribute.Id, attribute.Designation, attribute.Description);
                this.gameConfiguration.Attributes.Add(persistentAttribute);
            }
        }

        private void CreateItemSlotTypes()
        {
            var leftHand = this.context.CreateNew<ItemSlotType>();
            leftHand.Description = "Left Hand";
            leftHand.ItemSlots.Add(0);
            this.gameConfiguration.ItemSlotTypes.Add(leftHand);

            var rightHand = this.context.CreateNew<ItemSlotType>();
            rightHand.Description = "Right Hand";
            rightHand.ItemSlots.Add(1);
            this.gameConfiguration.ItemSlotTypes.Add(rightHand);

            var leftOrRightHand = this.context.CreateNew<ItemSlotType>();
            leftOrRightHand.Description = "Left or Right Hand";
            leftOrRightHand.ItemSlots.Add(0);
            leftOrRightHand.ItemSlots.Add(1);
            this.gameConfiguration.ItemSlotTypes.Add(leftOrRightHand);

            var helm = this.context.CreateNew<ItemSlotType>();
            helm.Description = "Helm";
            helm.ItemSlots.Add(2);
            this.gameConfiguration.ItemSlotTypes.Add(helm);

            var armor = this.context.CreateNew<ItemSlotType>();
            armor.Description = "Armor";
            armor.ItemSlots.Add(3);
            this.gameConfiguration.ItemSlotTypes.Add(armor);

            var pants = this.context.CreateNew<ItemSlotType>();
            pants.Description = "Pants";
            pants.ItemSlots.Add(4);
            this.gameConfiguration.ItemSlotTypes.Add(pants);

            var gloves = this.context.CreateNew<ItemSlotType>();
            gloves.Description = "Gloves";
            gloves.ItemSlots.Add(5);
            this.gameConfiguration.ItemSlotTypes.Add(gloves);

            var boots = this.context.CreateNew<ItemSlotType>();
            boots.Description = "Boots";
            boots.ItemSlots.Add(6);
            this.gameConfiguration.ItemSlotTypes.Add(boots);

            var wings = this.context.CreateNew<ItemSlotType>();
            wings.Description = "Wings";
            wings.ItemSlots.Add(7);
            this.gameConfiguration.ItemSlotTypes.Add(wings);

            var pet = this.context.CreateNew<ItemSlotType>();
            pet.Description = "Pet";
            pet.ItemSlots.Add(8);
            this.gameConfiguration.ItemSlotTypes.Add(pet);

            var pendant = this.context.CreateNew<ItemSlotType>();
            pendant.Description = "Pendant";
            pendant.ItemSlots.Add(9);
            this.gameConfiguration.ItemSlotTypes.Add(pendant);

            var ring = this.context.CreateNew<ItemSlotType>();
            ring.Description = "Ring";
            ring.ItemSlots.Add(10);
            ring.ItemSlots.Add(11);
            this.gameConfiguration.ItemSlotTypes.Add(ring);
        }

        private void CreateGameClientDefinitions()
        {
            var clientDefinition = this.context.CreateNew<GameClientDefinition>();
            clientDefinition.Season = 6;
            clientDefinition.Episode = 3;
            clientDefinition.Language = ClientLanguage.English;
            clientDefinition.Version = new byte[] { 0x31, 0x30, 0x34, 0x30, 0x34 };
            clientDefinition.Serial = Encoding.ASCII.GetBytes("k1Pk2jcET48mxL3b");
            clientDefinition.Description = "Season 6 Episode 3 GMO Client";

            var version075Definition = this.context.CreateNew<GameClientDefinition>();
            version075Definition.Season = 0;
            version075Definition.Episode = 75;
            version075Definition.Language = ClientLanguage.Invariant; // it doesn't fit into any available category - maybe it's so old that it didn't have differences in the protocol yet.
            version075Definition.Version = new byte[] { 0x30, 0x37, 0x35, 0x30, 0x30 }; // the last two bytes are not relevant as te 0.75 does only use the first 3 bytes.
            version075Definition.Serial = Encoding.ASCII.GetBytes("sudv(*40ds7lkN2n");
            version075Definition.Description = "Version 0.75 Client";
        }

        private void CreateConnectServerDefinitions()
        {
            var i = 0;
            foreach (var client in this.context.Get<GameClientDefinition>().ToList())
            {
                var connectServer = this.context.CreateNew<ConnectServerDefinition>();
                connectServer.ServerId = (byte)i;
                connectServer.Client = client;
                connectServer.ClientListenerPort = 44405 + i;
                connectServer.Description = $"Connect Server ({new ClientVersion(client.Season, client.Episode, client.Language)})";
                connectServer.DisconnectOnUnknownPacket = true;
                connectServer.MaximumReceiveSize = 6;
                connectServer.Timeout = new TimeSpan(0, 1, 0);
                connectServer.CurrentPatchVersion = new byte[] { 1, 3, 0x2B };
                connectServer.PatchAddress = "patch.muonline.webzen.com";
                connectServer.MaxConnectionsPerAddress = 30;
                connectServer.CheckMaxConnectionsPerAddress = true;
                connectServer.MaxConnections = 10000;
                connectServer.ListenerBacklog = 100;
                connectServer.MaxFtpRequests = 1;
                connectServer.MaxIpRequests = 5;
                connectServer.MaxServerListRequests = 20;
                i++;
            }
        }

        private void CreateGameServerDefinitions(GameServerConfiguration gameServerConfiguration, int numberOfServers)
        {
            for (int i = 0; i < numberOfServers; i++)
            {
                var server = this.context.CreateNew<GameServerDefinition>();
                server.ServerID = (byte)i;
                server.Description = $"Server {i}";
                server.ExperienceRate = 1.0f;
                server.GameConfiguration = this.gameConfiguration;
                server.ServerConfiguration = gameServerConfiguration;

                var j = 0;
                foreach (var client in this.context.Get<GameClientDefinition>().ToList())
                {
                    var endPoint = this.context.CreateNew<GameServerEndpoint>();
                    endPoint.Client = client;
                    endPoint.NetworkPort = 55901 + i + j;
                    server.Endpoints.Add(endPoint);
                    j += 20;
                }
            }
        }

        private void CreateChatServerDefinition()
        {
            var server = this.context.CreateNew<ChatServerDefinition>();
            server.ServerId = 0;
            server.Description = "Chat Server";

            var i = 0;
            foreach (var client in this.context.Get<GameClientDefinition>()
                .OrderByDescending(c => c.Season) // Season 6 should get the standard port
                .ToList())
            {
                var endPoint = this.context.CreateNew<ChatServerEndpoint>();
                endPoint.Client = client;
                endPoint.NetworkPort = 55980 + i++;
                server.Endpoints.Add(endPoint);
            }
        }

        private void CreateGameMapDefinitions()
        {
            var mapInitializerTypes = new[]
            {
                typeof(Lorencia),
                typeof(Dungeon),
                typeof(Devias),
                typeof(Noria),
                typeof(LostTower),
                typeof(Exile),
                typeof(Arena),
                typeof(Atlans),
                typeof(Tarkan),
                typeof(DevilSquare1To4),
                typeof(Icarus),
                typeof(Elvenland),
                typeof(Karutan1),
                typeof(Karutan2),
                typeof(Aida),
                typeof(Vulcanus),
                typeof(CrywolfFortress),
                typeof(LandOfTrials),
                typeof(LorenMarket),
                typeof(SantaVillage),
                typeof(SilentMap),
                typeof(ValleyOfLoren),
                typeof(BarracksOfBalgass),
                typeof(BalgassRefuge),
                typeof(Kalima1),
                typeof(Kalima2),
                typeof(Kalima3),
                typeof(Kalima4),
                typeof(Kalima5),
                typeof(Kalima6),
                typeof(Kalima7),
                typeof(KanturuRelics),
                typeof(KanturuRuins),
                typeof(KanturuEvent),
                typeof(Raklion),
                typeof(RaklionBoss),
                typeof(SwampOfCalmness),
                typeof(DuelArena),
                typeof(BloodCastle1),
                typeof(BloodCastle2),
                typeof(BloodCastle3),
                typeof(BloodCastle4),
                typeof(BloodCastle5),
                typeof(BloodCastle6),
                typeof(BloodCastle7),
                typeof(BloodCastle8),
                typeof(ChaosCastle1),
                typeof(ChaosCastle2),
                typeof(ChaosCastle3),
                typeof(ChaosCastle4),
                typeof(ChaosCastle5),
                typeof(ChaosCastle6),
                typeof(ChaosCastle7),
                typeof(IllusionTemple1),
                typeof(IllusionTemple2),
                typeof(IllusionTemple3),
                typeof(IllusionTemple4),
                typeof(IllusionTemple5),
                typeof(IllusionTemple6),
                typeof(DevilSquare5To7),
                typeof(Doppelgaenger1),
                typeof(Doppelgaenger2),
                typeof(Doppelgaenger3),
                typeof(Doppelgaenger4),
                typeof(FortressOfImperialGuardian1),
                typeof(FortressOfImperialGuardian2),
                typeof(FortressOfImperialGuardian3),
                typeof(FortressOfImperialGuardian4),
            };

            var parameters = new object[] { this.context, this.gameConfiguration };
            this.mapInitializers = mapInitializerTypes
                .Select(type => type.GetConstructors().First().Invoke(parameters) as Maps.IMapInitializer)
                .Where(i => i != null)
                .ToList();

            foreach (var mapInitializer in this.mapInitializers)
            {
                mapInitializer.Initialize();
            }
        }

        private void SetSafezoneMaps()
        {
            foreach (var mapInitializer in this.mapInitializers)
            {
                mapInitializer.SetSafezoneMap();
            }
        }

        private GameServerConfiguration CreateGameServerConfiguration(ICollection<GameMapDefinition> maps)
        {
            var gameServerConfiguration = this.context.CreateNew<GameServerConfiguration>();
            gameServerConfiguration.MaximumPlayers = 1000;

            // by default we add every map to a server configuration
            foreach (var map in maps)
            {
                gameServerConfiguration.Maps.Add(map);
            }

            return gameServerConfiguration;
        }

        private void InitializeGameConfiguration()
        {
            this.gameConfiguration.ExperienceRate = 1.0f;
            this.gameConfiguration.MaximumLevel = 400;
            this.gameConfiguration.InfoRange = 12;
            this.gameConfiguration.AreaSkillHitsPlayer = false;
            this.gameConfiguration.MaximumInventoryMoney = int.MaxValue;
            this.gameConfiguration.MaximumVaultMoney = int.MaxValue;
            this.gameConfiguration.RecoveryInterval = 3000;
            this.gameConfiguration.MaximumLetters = 50;
            this.gameConfiguration.LetterSendPrice = 1000;
            this.gameConfiguration.MaximumCharactersPerAccount = 5;
            this.gameConfiguration.CharacterNameRegex = "^[a-zA-Z0-9]{3,10}$";
            this.gameConfiguration.MaximumPasswordLength = 20;
            this.gameConfiguration.MaximumPartySize = 5;
            this.gameConfiguration.ShouldDropMoney = true;
            this.gameConfiguration.ExperienceTable =
                Enumerable.Range(0, this.gameConfiguration.MaximumLevel + 2)
                    .Select(level => CalculateNeededExperience(level))
                    .ToArray();
            this.gameConfiguration.MasterExperienceTable =
                Enumerable.Range(0, 201).Select(level => this.CalcNeededMasterExp(level)).ToArray();
            var moneyDropItemGroup = this.context.CreateNew<DropItemGroup>();
            moneyDropItemGroup.Chance = 0.5;
            moneyDropItemGroup.ItemType = SpecialItemType.Money;
            moneyDropItemGroup.Description = "The common money drop item group (50 % drop chance)";
            this.gameConfiguration.DropItemGroups.Add(moneyDropItemGroup);
            BaseMapInitializer.RegisterDefaultDropItemGroup(moneyDropItemGroup);

            var randomItemDropItemGroup = this.context.CreateNew<DropItemGroup>();
            randomItemDropItemGroup.Chance = 0.3;
            randomItemDropItemGroup.ItemType = SpecialItemType.RandomItem;
            randomItemDropItemGroup.Description = "The common drop item group for random items (30 % drop chance)";
            this.gameConfiguration.DropItemGroups.Add(randomItemDropItemGroup);
            BaseMapInitializer.RegisterDefaultDropItemGroup(randomItemDropItemGroup);

            var excellentItemDropItemGroup = this.context.CreateNew<DropItemGroup>();
            excellentItemDropItemGroup.Chance = 0.0001;
            excellentItemDropItemGroup.ItemType = SpecialItemType.Excellent;
            excellentItemDropItemGroup.Description = "The common drop item group for random excellent items (0.01 % drop chance)";
            this.gameConfiguration.DropItemGroups.Add(excellentItemDropItemGroup);
            BaseMapInitializer.RegisterDefaultDropItemGroup(excellentItemDropItemGroup);

            this.CreateStatAttributes();

            this.CreateItemSlotTypes();
            this.CreateItemOptionTypes();
            this.gameConfiguration.ItemOptions.Add(this.CreateLuckOptionDefinition());
            this.gameConfiguration.ItemOptions.Add(this.CreateOptionDefinition(Stats.DefenseBase));
            this.gameConfiguration.ItemOptions.Add(this.CreateOptionDefinition(Stats.MaximumPhysBaseDmg));
            this.gameConfiguration.ItemOptions.Add(this.CreateOptionDefinition(Stats.MaximumWizBaseDmg));
            this.gameConfiguration.ItemOptions.Add(this.CreateOptionDefinition(Stats.MaximumCurseBaseDmg));

            new CharacterClassInitialization(this.context, this.gameConfiguration).Initialize();
            new SkillsInitializer(this.context, this.gameConfiguration).Initialize();
            new Orbs(this.context, this.gameConfiguration).Initialize();
            new Scrolls(this.context, this.gameConfiguration).Initialize();
            new EventTicketItems(this.context, this.gameConfiguration).Initialize();
            new Wings(this.context, this.gameConfiguration).Initialize();
            new Pets(this.context, this.gameConfiguration).Initialize();
            new ExcellentOptions(this.context, this.gameConfiguration).Initialize();
            new HarmonyOptions(this.context, this.gameConfiguration).Initialize();
            new GuardianOptions(this.context, this.gameConfiguration).Initialize();
            new Armors(this.context, this.gameConfiguration).Initialize();
            new Weapons(this.context, this.gameConfiguration).Initialize();
            new Potions(this.context, this.gameConfiguration).Initialize();
            new Jewels(this.context, this.gameConfiguration).Initialize();
            new Misc(this.context, this.gameConfiguration).Initialize();
            new PackedJewels(this.context, this.gameConfiguration).Initialize();
            new Jewellery(this.context, this.gameConfiguration).Initialize();
            new AncientSets(this.context, this.gameConfiguration).Initialize();
            this.CreateJewelMixes();
            this.CreateNpcs();
            this.CreateGameMapDefinitions();
            this.AssignCharacterClassHomeMaps();
            new SocketSystem(this.context, this.gameConfiguration).Initialize();
            new ChaosMixes(this.context, this.gameConfiguration).Initialize();
            new Gates().Initialize(this.context, this.gameConfiguration);
            new Items.Quest(this.context, this.gameConfiguration).Initialize();
            new Quests(this.context, this.gameConfiguration).Initialize();
            //// TODO: ItemSetGroups
        }

        private void CreateJewelMixes()
        {
            this.CreateJewelMix(0, 13, 0xE, 30); // Bless
            this.CreateJewelMix(1, 14, 0xE, 31); // Soul
            this.CreateJewelMix(2, 16, 0xE, 136); // Jewel of Life
            this.CreateJewelMix(3, 22, 0xE, 137); // Jewel of Creation
            this.CreateJewelMix(4, 31, 0xE, 138); // Jewel of Guardian
            this.CreateJewelMix(5, 41, 0xE, 139); // Gemstone
            this.CreateJewelMix(6, 42, 0xE, 140); // Jewel of Harmony
            this.CreateJewelMix(7, 15, 0xC, 141); // Chaos
            this.CreateJewelMix(8, 43, 0xE, 142); // Lower Refine Stone
            this.CreateJewelMix(9, 44, 0xE, 143); // Higher Refine Stone
        }

        private void CreateJewelMix(byte mixNumber, int itemNumber, int itemGroup, int packedJewelId)
        {
            var singleJewel = this.gameConfiguration.Items.First(i => i.Group == itemGroup && i.Number == itemNumber);
            var packedJewel = this.gameConfiguration.Items.First(i => i.Group == 0x0C && i.Number == packedJewelId);
            var jewelMix = this.context.CreateNew<JewelMix>();
            jewelMix.Number = mixNumber;
            jewelMix.SingleJewel = singleJewel;
            jewelMix.MixedJewel = packedJewel;
            this.gameConfiguration.JewelMixes.Add(jewelMix);
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
                        mapNumber = Noria.Number;
                        break;
                    case CharacterClassNumber.BloodySummoner:
                    case CharacterClassNumber.Summoner:
                        mapNumber = Elvenland.Number;
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
