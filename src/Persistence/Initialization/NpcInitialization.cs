// <copyright file="NpcInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Persistence.Initialization.Items;

    /// <summary>
    /// The initialization of all NPCs, which are no monsters.
    /// </summary>
    internal class NpcInitialization
    {
        private readonly IContext context;

        private readonly GameConfiguration gameConfiguration;

        private ItemHelper itemHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="NpcInitialization" /> class.
        /// </summary>
        /// <param name="context">The persistence context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public NpcInitialization(IContext context, GameConfiguration gameConfiguration)
        {
            this.context = context;
            this.gameConfiguration = gameConfiguration;
            this.itemHelper = new ItemHelper(this.context, this.gameConfiguration);
        }

        /// <summary>
        /// Creates all NPCs.
        /// </summary>
        /// <remarks>
        /// Extracted from Monsters.txt by Regex: (?m)^(\d+)\t1\t"(.*?)".*?$
        /// Replace by: yield return new MonsterDefinition() { Number = $1, Designation="$2" };
        /// yield return new (\w*) { Number = (\d+), Designation = (".*?").*?(, NpcWindow = (.*) ){0,1}};
        /// Replace by: <![CDATA[ {\n    var def = this.context.CreateNew<$1>();\n    def.Number = $2;\n    def.Designation = $3;\n    def.NpcWindow = $5;\n    this.gameConfiguration.Monsters.Add(def);\n}\n ]]>
        /// </remarks>
        internal void CreateNpcs()
        {
            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 226;
                def.Designation = "Pet Trainer";
                def.NpcWindow = NpcWindow.PetTrainer;
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 229;
                def.Designation = "Marlon";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 230;
                def.Designation = "Alex";
                def.NpcWindow = NpcWindow.Merchant;
                def.MerchantStore = this.CreateMerchantStoreWithPotions();
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 231;
                def.Designation = "Thompson the Merchant";
                def.NpcWindow = NpcWindow.Merchant;
                def.MerchantStore = this.CreateMerchantStoreWithPotions();
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 232;
                def.Designation = "Archangel";
                def.NpcWindow = NpcWindow.BloodCastle;
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 233;
                def.Designation = "Messenger of Arch.";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 235;
                def.Designation = "Sevina the Priestess";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 236;
                def.Designation = "Golden Archer";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 237;
                def.Designation = "Charon";
                def.NpcWindow = NpcWindow.DevilSquare;
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 238;
                def.Designation = "Chaos Goblin";
                def.NpcWindow = NpcWindow.ChaosMachine;
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 240;
                def.Designation = "Baz The Vault Keeper";
                def.NpcWindow = NpcWindow.VaultStorage;
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 241;
                def.Designation = "Guild Master";
                def.NpcWindow = NpcWindow.GuildMaster;
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 242;
                def.Designation = "Elf Lala";
                def.NpcWindow = NpcWindow.Merchant;
                def.MerchantStore = this.CreateMerchantStoreWithPotions();
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 243;
                def.Designation = "Eo the Craftsman";
                def.NpcWindow = NpcWindow.Merchant;
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 244;
                def.Designation = "Lumen The Barmaid";
                def.NpcWindow = NpcWindow.Merchant;
                def.MerchantStore = this.CreateMerchantStoreWithPotions();
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 245;
                def.Designation = "Izabel The Wizard";
                def.NpcWindow = NpcWindow.Merchant;
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 246;
                def.Designation = "Zienna The Weapons Merchant";
                def.NpcWindow = NpcWindow.Merchant;
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 247;
                def.Designation = "Guard";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 248;
                def.Designation = "Wandering Merchant";
                def.NpcWindow = NpcWindow.Merchant;
                def.MerchantStore = this.CreateMerchantStoreWithPotions();
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 249;
                def.Designation = "Guard";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 250;
                def.Designation = "Wandering Merchant";
                def.NpcWindow = NpcWindow.Merchant;
                def.MerchantStore = this.CreateWanderingMerchant();
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 251;
                def.Designation = "Hanzo The Blacksmith";
                def.NpcWindow = NpcWindow.Merchant;
                def.MerchantStore = this.CreateHanzoTheBlacksmith();
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 253;
                def.Designation = "Potion Girl";
                def.NpcWindow = NpcWindow.Merchant;
                def.MerchantStore = this.CreatePotionGirl();
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 254;
                def.Designation = "Pasi The Mage";
                def.NpcWindow = NpcWindow.Merchant;
                def.MerchantStore = this.CreatePasiTheMageStore();
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 255;
                def.Designation = "Lumen The Barmaid";
                def.NpcWindow = NpcWindow.Merchant;
                def.MerchantStore = this.CreatePotionGirl();
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 256;
                def.Designation = "Lahap";
                def.NpcWindow = NpcWindow.Lahap;
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 257;
                def.Designation = "Elf Soldier";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 259;
                def.Designation = "Oracle Layla";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 375;
                def.Designation = "Chaos Card Master";
                def.NpcWindow = NpcWindow.ChaosCardCombination;
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 376;
                def.Designation = "Pamela the Supplier";
                def.NpcWindow = NpcWindow.Merchant;
                def.MerchantStore = this.CreateMerchantStoreWithPotions();
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 377;
                def.Designation = "Angela the Supplier";
                def.NpcWindow = NpcWindow.Merchant;
                def.MerchantStore = this.CreateMerchantStoreWithPotions();
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 378;
                def.Designation = "GameMaster";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 379;
                def.Designation = "Fireworks Girl";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 371;
                def.Designation = "Leo The Helper";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 372;
                def.Designation = "Elite Skill Soldier";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 380;
                def.Designation = "Stone Statue";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 381;
                def.Designation = "MU Allies General";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 382;
                def.Designation = "Illusion Elder";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 383;
                def.Designation = "Alliance Item Storage";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 384;
                def.Designation = "Illusion Item Storage";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 385;
                def.Designation = "Mirage";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 239;
                def.Designation = "Arena Guard";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 215;
                def.Designation = "Shield";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 216;
                def.Designation = "Crown";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 217;
                def.Designation = "Crown Switch1";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 218;
                def.Designation = "Crown Switch2";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 219;
                def.Designation = "Castle Gate Switch";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 220;
                def.Designation = "Guard";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 221;
                def.Designation = "Slingshot Attack";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 222;
                def.Designation = "Slingshot Defense";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 223;
                def.Designation = "Senior";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 224;
                def.Designation = "Guardsman";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 277;
                def.Designation = "Castle Gate1";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 278;
                def.Designation = "Life Stone";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 283;
                def.Designation = "Guardian Statue";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 285;
                def.Designation = "Guardian";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 286;
                def.Designation = "Battle Guard1";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 287;
                def.Designation = "Battle Guard2";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 288;
                def.Designation = "Canon Tower";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 367;
                def.Designation = "Gateway Machine";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 368;
                def.Designation = "Elpis";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 369;
                def.Designation = "Osbourne";
                def.NpcWindow = NpcWindow.RefineStoneMaking;
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 370;
                def.Designation = "Jerridon";
                def.NpcWindow = NpcWindow.RemoveJohOption;
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 404;
                def.Designation = "MU Allies";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 405;
                def.Designation = "Illusion Sorcerer";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 406;
                def.Designation = "Priest Devin";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 407;
                def.Designation = "Werewolf Quarrel";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 408;
                def.Designation = "Gatekeeper";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 415;
                def.Designation = "Silvia";
                def.NpcWindow = NpcWindow.Merchant;
                def.MerchantStore = this.CreateMerchantStoreWithPotions();
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 416;
                def.Designation = "Rhea";
                def.NpcWindow = NpcWindow.Merchant;
                def.MerchantStore = this.CreateMerchantStoreWithPotions();
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 417;
                def.Designation = "Marce";
                def.MerchantStore = this.CreateMerchantStoreWithPotions();
                def.NpcWindow = NpcWindow.Merchant;
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 450;
                def.Designation = "Cherry Blossom Spirit";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 451;
                def.Designation = "Cherry Blossom Tree";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 452;
                def.Designation = "Seed Master";
                def.NpcWindow = NpcWindow.SeedMaster;
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 453;
                def.Designation = "Seed Researcher";
                def.NpcWindow = NpcWindow.SeedResearcher;
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 467;
                def.Designation = "Snowman";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 468;
                def.Designation = "Little Santa Yellow";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 469;
                def.Designation = "Little Santa Green";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 470;
                def.Designation = "Little Santa Red";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 471;
                def.Designation = "Little Santa Blue";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 472;
                def.Designation = "Little Santa White";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 473;
                def.Designation = "Little Santa Black";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 474;
                def.Designation = "Little Santa Orange";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 475;
                def.Designation = "Little Santa Pink";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 476;
                def.Designation = "Cursed Santa";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 477;
                def.Designation = "Transformed Snowman";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 478;
                def.Designation = "Delgado - Lucky Coins";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 479;
                def.Designation = "Gatekeeper Titus";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 492;
                def.Designation = "Moss The Merchant";
                def.NpcWindow = NpcWindow.Merchant;
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 522;
                def.Designation = "Adviser Jerinteu";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 540;
                def.Designation = "Lugard";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 541;
                def.Designation = "Compensation Box";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 542;
                def.Designation = "Golden Compensation Box";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 543;
                def.Designation = "Gens Duprian";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 544;
                def.Designation = "Gens Vanert";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 566;
                def.Designation = "Mercenary Guild Felicia";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 152;
                def.Designation = "Gate to Kalima 1 of {0}";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 153;
                def.Designation = "Gate to Kalima 2 of {0}";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 154;
                def.Designation = "Gate to Kalima 3 of {0}";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 155;
                def.Designation = "Gate to Kalima 4 of {0}";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 156;
                def.Designation = "Gate to Kalima 5 of {0}";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 157;
                def.Designation = "Gate to Kalima 6 of {0}";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 158;
                def.Designation = "Gate to Kalima 7 of {0}";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 131;
                def.Designation = "Castle Gate";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 132;
                def.Designation = "Statue of Saint";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 133;
                def.Designation = "Statue of Saint";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 134;
                def.Designation = "Statue of Saint";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 200;
                def.Designation = "Soccerball";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 579;
                def.Designation = "David";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 577;
                def.Designation = "Leina the General Goods Merchant";
                def.MerchantStore = this.CreateMerchantStoreWithPotions();
                def.NpcWindow = NpcWindow.Merchant;
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 578;
                def.Designation = "Weapons Merchant Bolo";
                def.NpcWindow = NpcWindow.Merchant;
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 545;
                def.Designation = "Christine the General Goods Merchant";
                def.NpcWindow = NpcWindow.Merchant;
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 546;
                def.Designation = "Jeweler Raul";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 547;
                def.Designation = "Market Union Member Julia";
                def.NpcWindow = NpcWindow.Merchant;
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 568;
                def.Designation = "Wandering Merchant Zyro";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 658;
                def.Designation = "Cursed Statue";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 659;
                def.Designation = "Captured Stone Statue (1)";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 660;
                def.Designation = "Captured Stone Statue (2)";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 661;
                def.Designation = "Captured Stone Statue (3)";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 662;
                def.Designation = "Captured Stone Statue (4)";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 663;
                def.Designation = "Captured Stone Statue (5)";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 664;
                def.Designation = "Captured Stone Statue (6)";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 665;
                def.Designation = "Captured Stone Statue (7)";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 666;
                def.Designation = "Captured Stone Statue (8)";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 667;
                def.Designation = "Captured Stone Statue (9)";
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 668;
                def.Designation = "Captured Stone Statue (10)";
                this.gameConfiguration.Monsters.Add(def);
            }
        }

        private ItemStorage CreatePotionGirl()
        {
            List<Item> itemList = new List<Item>();

            itemList.Add(this.itemHelper.CreatePotion(0, 0, 1, 0));     // Apple +0 x1
            itemList.Add(this.itemHelper.CreatePotion(8, 0, 3, 0));     // Apple +0 x3
            itemList.Add(this.itemHelper.CreatePotion(16, 0, 1, 1));    // Apple +1 x1
            itemList.Add(this.itemHelper.CreatePotion(24, 0, 3, 1));    // Apple +1 x3

            itemList.Add(this.itemHelper.CreatePotion(1, 1, 1, 0));     // Small Healing Potion +0 x1
            itemList.Add(this.itemHelper.CreatePotion(9, 1, 3, 0));     // Small Healing Potion +0 x3
            itemList.Add(this.itemHelper.CreatePotion(17, 1, 1, 1));    // Small Healing Potion +1 x1
            itemList.Add(this.itemHelper.CreatePotion(25, 1, 3, 1));    // Small Healing Potion +1 x3

            itemList.Add(this.itemHelper.CreatePotion(2, 2, 1, 0));     // Medium Healing Potion +0 x1
            itemList.Add(this.itemHelper.CreatePotion(10, 2, 3, 0));    // Medium Healing Potion +0 x3
            itemList.Add(this.itemHelper.CreatePotion(18, 2, 1, 1));    // Medium Healing Potion +1 x1
            itemList.Add(this.itemHelper.CreatePotion(26, 2, 3, 1));    // Medium Healing Potion +1 x3

            itemList.Add(this.itemHelper.CreatePotion(3, 3, 1, 0));     // Large Healing Potion +0 x1
            itemList.Add(this.itemHelper.CreatePotion(11, 3, 3, 0));    // Large Healing Potion +0 x3
            itemList.Add(this.itemHelper.CreatePotion(19, 3, 1, 1));    // Large Healing Potion +1 x1
            itemList.Add(this.itemHelper.CreatePotion(27, 3, 3, 1));    // Large Healing Potion +1 x3

            itemList.Add(this.itemHelper.CreatePotion(4, 4, 1, 0));     // Small Mana Potion +0 x1
            itemList.Add(this.itemHelper.CreatePotion(12, 4, 3, 0));    // Small Mana Potion +0 x3
            itemList.Add(this.itemHelper.CreatePotion(20, 4, 1, 1));    // Small Mana Potion +1 x1
            itemList.Add(this.itemHelper.CreatePotion(28, 4, 3, 1));    // Small Mana Potion +1 x3

            itemList.Add(this.itemHelper.CreatePotion(5, 5, 1, 0));     // Medium Mana Potion +0 x1
            itemList.Add(this.itemHelper.CreatePotion(13, 5, 3, 0));    // Medium Mana Potion +0 x3
            itemList.Add(this.itemHelper.CreatePotion(21, 5, 1, 1));    // Medium Mana Potion +1 x1
            itemList.Add(this.itemHelper.CreatePotion(29, 5, 3, 1));    // Medium Mana Potion +1 x3

            itemList.Add(this.itemHelper.CreatePotion(6, 6, 1, 0));     // Large Mana Potion +0 x1
            itemList.Add(this.itemHelper.CreatePotion(14, 6, 3, 0));    // Large Mana Potion +0 x3
            itemList.Add(this.itemHelper.CreatePotion(22, 6, 1, 1));    // Large Mana Potion +1 x1
            itemList.Add(this.itemHelper.CreatePotion(30, 6, 3, 1));    // Large Mana Potion +1 x3

            itemList.Add(this.itemHelper.CreatePotion(7, 9, 1, 0));     // Ale +0 x1
            // TODO: insert Town Portal item.
            // TODO: insert Ammo (Bolts and Arrows).
            // TODO: insert Armor of Guardsman.
            return this.CreateMerchantStore(itemList);
        }

        private ItemStorage CreateWanderingMerchant()
        {
            List<Item> itemList = new List<Item>();

            itemList.Add(this.itemHelper.CreateSetItem(0, 5, 7, null, 0, 1, true));     // Leather Helm     +0+4+L
            itemList.Add(this.itemHelper.CreateSetItem(16, 5, 8, null, 0, 1, true));    // Leather Armor    +0+4+L
            itemList.Add(this.itemHelper.CreateSetItem(40, 5, 9, null, 0, 1, true));    // Leather Pants    +0+4+L
            itemList.Add(this.itemHelper.CreateSetItem(56, 5, 11, null, 0, 1, true));   // Leather Boots    +0+4+L
            itemList.Add(this.itemHelper.CreateSetItem(72, 5, 10, null, 0, 1, true));   // Leather Gloves   +0+4+L

            itemList.Add(this.itemHelper.CreateSetItem(2, 0, 7, null, 2, 1, true));     // Bronze Helm    +2+4+L
            itemList.Add(this.itemHelper.CreateSetItem(18, 0, 8, null, 2, 1, true));    // Bronze Armor   +2+4+L
            itemList.Add(this.itemHelper.CreateSetItem(34, 0, 9, null, 2, 1, true));    // Bronze Pants   +2+4+L
            itemList.Add(this.itemHelper.CreateSetItem(50, 0, 11, null, 2, 1, true));   // Bronze Boots   +2+4+L
            itemList.Add(this.itemHelper.CreateSetItem(66, 0, 10, null, 2, 1, true));   // Bronze Gloves  +2+4+L

            itemList.Add(this.itemHelper.CreateSetItem(4, 6, 7, null, 3, 1, true));     // Scale Helm      +3+4+L
            itemList.Add(this.itemHelper.CreateSetItem(20, 6, 8, null, 3, 1, true));    // Scale Armor     +3+4+L
            itemList.Add(this.itemHelper.CreateSetItem(36, 6, 9, null, 3, 1, true));    // Scale Pants     +3+4+L
            itemList.Add(this.itemHelper.CreateSetItem(52, 6, 11, null, 3, 1, true));   // Scale Boots     +3+4+L
            itemList.Add(this.itemHelper.CreateSetItem(68, 6, 10, null, 3, 1, true));   // Scale Gloves    +3+4+L

            itemList.Add(this.itemHelper.CreateSetItem(6, 8, 7, null, 3, 1, true));     // Brass Helm      +3+4+L
            itemList.Add(this.itemHelper.CreateSetItem(22, 8, 8, null, 3, 1, true));    // Brass Armor     +3+4+L
            itemList.Add(this.itemHelper.CreateSetItem(38, 8, 9, null, 3, 1, true));    // Brass Pants     +3+4+L
            itemList.Add(this.itemHelper.CreateSetItem(54, 8, 11, null, 3, 1, true));   // Brass Boots     +3+4+L
            itemList.Add(this.itemHelper.CreateSetItem(70, 8, 10, null, 3, 1, true));   // Brass Gloves    +3+4+L

            itemList.Add(this.itemHelper.CreateSetItem(88, 9, 7, null, 3, 1, true));    // Plate Helm      +3+4+L
            itemList.Add(this.itemHelper.CreateSetItem(104, 9, 8, null, 3, 1, true));   // Plate Armor     +3+4+L
            itemList.Add(this.itemHelper.CreateSetItem(82, 9, 9, null, 3, 1, true));    // Plate Pants     +3+4+L
            itemList.Add(this.itemHelper.CreateSetItem(98, 9, 11, null, 3, 1, true));   // Plate Boots     +3+4+L
            itemList.Add(this.itemHelper.CreateSetItem(84, 9, 10, null, 3, 1, true));   // Plate Gloves    +3+4+L

            return this.CreateMerchantStore(itemList);
        }

        private ItemStorage CreateHanzoTheBlacksmith()
        {
            List<Item> itemList = new List<Item>();

            itemList.Add(this.itemHelper.CreateShield(0, 0, false, null, 0, 1, true));  // Small Shield     +0+4+L
            itemList.Add(this.itemHelper.CreateShield(2, 4, true, null, 1, 1, true));   // Buckler          +1+4+L+S
            itemList.Add(this.itemHelper.CreateShield(4, 1, false, null, 2, 1, true));  // Horn Shield      +2+4+L
            itemList.Add(this.itemHelper.CreateShield(6, 2, false, null, 3, 1, true));  // Kite Shield      +3+4+L

            itemList.Add(this.itemHelper.CreateShield(16, 6, true, null, 3, 1, true));  // Skull Shield     +3+4+L+S
            itemList.Add(this.itemHelper.CreateShield(18, 10, true, null, 3, 1, true)); // Big Round Shield +3+4+L+S
            itemList.Add(this.itemHelper.CreateShield(20, 9, true, null, 3, 1, true));  // Plate Shield     +3+4+L+S
            itemList.Add(this.itemHelper.CreateShield(22, 7, true, null, 3, 1, true));  // Spiked Shield    +3+4+L+S

            itemList.Add(this.itemHelper.CreateShield(32, 5, true, null, 3, 1, true));  // Dragon Slayer    +3+4+L+S
            itemList.Add(this.itemHelper.CreateShield(34, 8, true, null, 3, 1, true));  // Tower Shield     +3+4+L+S
            itemList.Add(this.itemHelper.CreateShield(36, 11, true, null, 3, 1, true)); // Serpent Shield   +3+4+L+S
            itemList.Add(this.itemHelper.CreateShield(38, 12, true, null, 3, 1, true)); // Bronze Shield    +3+4+L+S

            itemList.Add(this.itemHelper.CreateWeapon(48, 0, 1, 0, 1, true, false, null));  // Short Sword  +0+4+L
            itemList.Add(this.itemHelper.CreateWeapon(49, 1, 1, 1, 1, true, false, null));  // Hand Axe     +1+4+L
            itemList.Add(this.itemHelper.CreateWeapon(50, 2, 1, 2, 1, true, false, null));  // Mace         +2+4+L
            itemList.Add(this.itemHelper.CreateWeapon(51, 0, 2, 2, 1, true, false, null));  // Rapier       +2+4+L
            itemList.Add(this.itemHelper.CreateWeapon(52, 1, 2, 2, 1, true, false, null));  // Double Axe   +2+4+L
            itemList.Add(this.itemHelper.CreateWeapon(53, 0, 4, 3, 1, true, true, null));   // Assassin     +3+4+L
            itemList.Add(this.itemHelper.CreateWeapon(54, 2, 1, 3, 1, true, true, null));   // Morning Star +3+4+L
            itemList.Add(this.itemHelper.CreateWeapon(55, 1, 3, 3, 1, true, true, null));   // Tomahawk     +3+4+L

            itemList.Add(this.itemHelper.CreateWeapon(72, 0, 0, 2, 1, true, false, null));  // Kris             +2+4+L
            itemList.Add(this.itemHelper.CreateWeapon(73, 0, 6, 3, 1, true, true, null));   // Gladius          +3+4+L+S
            itemList.Add(this.itemHelper.CreateWeapon(73, 0, 7, 3, 1, true, true, null));   // Falchion         +3+4+L+S
            itemList.Add(this.itemHelper.CreateWeapon(74, 0, 8, 3, 1, true, false, null));  // Serpent Sword    +3+4+L
            itemList.Add(this.itemHelper.CreateWeapon(75, 0, 5, 3, 1, true, true, null));   // Blade            +3+4+L+S

            return this.CreateMerchantStore(itemList);
        }

        private ItemStorage CreatePasiTheMageStore()
        {
            List<Item> itemList = new List<Item>();

            itemList.Add(this.itemHelper.CreateScroll(0, 3));   // Scroll of Fire Ball
            itemList.Add(this.itemHelper.CreateScroll(1, 10));  // Scroll of Power Wave
            itemList.Add(this.itemHelper.CreateScroll(2, 2));   // Scroll of Lighting
            itemList.Add(this.itemHelper.CreateScroll(3, 1));   // Scroll of Meteorite
            itemList.Add(this.itemHelper.CreateScroll(4, 5));   // Scroll of Teleport
            itemList.Add(this.itemHelper.CreateScroll(5, 6));   // Scroll of Ice
            itemList.Add(this.itemHelper.CreateScroll(6, 0));   // Scroll of Poison

            itemList.Add(this.itemHelper.CreateOrb(7, 13));     // Orb of Implate
            itemList.Add(this.itemHelper.CreateOrb(15, 7));     // Orb of Twisting Slash

            itemList.Add(this.itemHelper.CreateSetItem(16, 2, 7, null, 0, 1, true));    // Pad Helm     +0+4+L
            itemList.Add(this.itemHelper.CreateSetItem(32, 2, 8, null, 0, 1, true));    // Pad Armor    +0+4+L
            itemList.Add(this.itemHelper.CreateSetItem(48, 2, 9, null, 0, 1, true));    // Pad Pants    +0+4+L
            itemList.Add(this.itemHelper.CreateSetItem(64, 2, 11, null, 0, 1, true));   // Pad Boots    +0+4+L
            itemList.Add(this.itemHelper.CreateSetItem(80, 2, 10, null, 0, 1, true));   // Pad Gloves   +0+4+L

            itemList.Add(this.itemHelper.CreateSetItem(18, 4, 7, null, 2, 1, true));    // Bone Helm    +2+4+L
            itemList.Add(this.itemHelper.CreateSetItem(34, 4, 8, null, 2, 1, true));    // Bone Armor   +2+4+L
            itemList.Add(this.itemHelper.CreateSetItem(50, 4, 9, null, 2, 1, true));    // Bone Pants   +2+4+L
            itemList.Add(this.itemHelper.CreateSetItem(66, 4, 11, null, 2, 1, true));   // Bone Boots   +2+4+L
            itemList.Add(this.itemHelper.CreateSetItem(82, 4, 10, null, 2, 1, true));   // Bone Gloves  +2+4+L

            itemList.Add(this.itemHelper.CreateSetItem(20, 7, 7, null, 3, 1, true));    // Sphinx Helm      +3+4+L
            itemList.Add(this.itemHelper.CreateSetItem(36, 7, 8, null, 3, 1, true));    // Sphinx Armor     +3+4+L
            itemList.Add(this.itemHelper.CreateSetItem(60, 7, 9, null, 3, 1, true));    // Sphinx Pants     +3+4+L
            itemList.Add(this.itemHelper.CreateSetItem(76, 7, 11, null, 3, 1, true));   // Sphinx Boots     +3+4+L
            itemList.Add(this.itemHelper.CreateSetItem(92, 7, 10, null, 3, 1, true));   // Sphinx Gloves    +3+4+L

            itemList.Add(this.itemHelper.CreateWeapon(22, 5, 0, 0, 1, true, false, null));  // Skull Staff      +0+4+L
            itemList.Add(this.itemHelper.CreateWeapon(46, 5, 1, 2, 1, true, false, null));  // Angelic Staff    +2+4+L
            itemList.Add(this.itemHelper.CreateWeapon(70, 5, 2, 3, 1, true, false, null));  // Serpent Staff    +3+4+L
            itemList.Add(this.itemHelper.CreateWeapon(94, 5, 3, 3, 1, true, false, null));  // Thunder Staff    +3+4+L

            return this.CreateMerchantStore(itemList);
        }

        private ItemStorage CreateMerchantStore(List<Item> itemList)
        {
            var merchantStore = this.context.CreateNew<ItemStorage>();

            foreach (var item in itemList)
            {
                merchantStore.Items.Add(item);
            }

            return merchantStore;
        }

        private ItemStorage CreateMerchantStoreWithPotions()
        {
            var allPotions = this.gameConfiguration.Items.Where(item => item.Group == 14).ToList();
            var merchantStore = this.context.CreateNew<ItemStorage>();

            var apple = this.context.CreateNew<Item>();
            apple.Definition = allPotions.First(def => def.Number == 0);
            merchantStore.Items.Add(apple);

            var smallHealingPotion = this.context.CreateNew<Item>();
            smallHealingPotion.Definition = allPotions.First(def => def.Number == 1);
            smallHealingPotion.ItemSlot = 1;
            merchantStore.Items.Add(smallHealingPotion);

            var mediumHealingPotion = this.context.CreateNew<Item>();
            mediumHealingPotion.Definition = allPotions.First(def => def.Number == 2);
            mediumHealingPotion.ItemSlot = 2;
            merchantStore.Items.Add(mediumHealingPotion);

            var largeHealingPotion = this.context.CreateNew<Item>();
            largeHealingPotion.Definition = allPotions.First(def => def.Number == 3);
            largeHealingPotion.ItemSlot = 3;
            merchantStore.Items.Add(largeHealingPotion);

            var smallManaPotion = this.context.CreateNew<Item>();
            smallManaPotion.Definition = allPotions.First(def => def.Number == 4);
            smallManaPotion.ItemSlot = 4;
            merchantStore.Items.Add(smallManaPotion);

            var mediumManaPotion = this.context.CreateNew<Item>();
            mediumManaPotion.Definition = allPotions.First(def => def.Number == 5);
            mediumManaPotion.ItemSlot = 5;
            merchantStore.Items.Add(mediumManaPotion);

            var largeManaPotion = this.context.CreateNew<Item>();
            largeManaPotion.Definition = allPotions.First(def => def.Number == 6);
            largeManaPotion.ItemSlot = 6;
            merchantStore.Items.Add(largeManaPotion);

            var ale = this.context.CreateNew<Item>();
            ale.Definition = allPotions.First(def => def.Number == 9);
            ale.ItemSlot = 7;
            merchantStore.Items.Add(ale);

            var applePlus1 = this.context.CreateNew<Item>();
            applePlus1.Definition = allPotions.First(def => def.Number == 0);
            applePlus1.ItemSlot = 8;
            applePlus1.Level = 1;
            merchantStore.Items.Add(applePlus1);

            var smallHealingPotionPlus1 = this.context.CreateNew<Item>();
            smallHealingPotionPlus1.Definition = allPotions.First(def => def.Number == 1);
            smallHealingPotionPlus1.ItemSlot = 9;
            smallHealingPotionPlus1.Level = 1;
            merchantStore.Items.Add(smallHealingPotionPlus1);

            var mediumHealingPotionPlus1 = this.context.CreateNew<Item>();
            mediumHealingPotionPlus1.Definition = allPotions.First(def => def.Number == 2);
            mediumHealingPotionPlus1.ItemSlot = 10;
            mediumHealingPotionPlus1.Level = 1;
            merchantStore.Items.Add(mediumHealingPotionPlus1);

            var largeHealingPotionPlus1 = this.context.CreateNew<Item>();
            largeHealingPotionPlus1.Definition = allPotions.First(def => def.Number == 3);
            largeHealingPotionPlus1.ItemSlot = 11;
            largeHealingPotionPlus1.Level = 1;
            merchantStore.Items.Add(largeHealingPotionPlus1);

            var smallManaPotionPlus1 = this.context.CreateNew<Item>();
            smallManaPotionPlus1.Definition = allPotions.First(def => def.Number == 4);
            smallManaPotionPlus1.ItemSlot = 12;
            smallManaPotionPlus1.Level = 1;
            merchantStore.Items.Add(smallManaPotionPlus1);

            var mediumManaPotionPlus1 = this.context.CreateNew<Item>();
            mediumManaPotionPlus1.Definition = allPotions.First(def => def.Number == 5);
            mediumManaPotionPlus1.ItemSlot = 13;
            mediumManaPotionPlus1.Level = 1;
            merchantStore.Items.Add(mediumManaPotionPlus1);

            var largeManaPotionPlus1 = this.context.CreateNew<Item>();
            largeManaPotionPlus1.Definition = allPotions.First(def => def.Number == 6);
            largeManaPotionPlus1.ItemSlot = 14;
            largeManaPotionPlus1.Level = 1;
            merchantStore.Items.Add(largeManaPotionPlus1);

            return merchantStore;
        }
    }
}
