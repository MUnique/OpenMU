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

        /// <summary>
        /// Initializes a new instance of the <see cref="NpcInitialization" /> class.
        /// </summary>
        /// <param name="context">The persistence context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public NpcInitialization(IContext context, GameConfiguration gameConfiguration)
        {
            this.context = context;
            this.gameConfiguration = gameConfiguration;
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
                def.MerchantStore = this.CreatePotionGirl();
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 231;
                def.Designation = "Thompson the Merchant";
                def.NpcWindow = NpcWindow.Merchant;
                def.MerchantStore = this.CreatePotionGirl();
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
                def.MerchantStore = this.CreatePotionGirl();
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
                def.MerchantStore = this.CreatePotionGirl();
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
                def.MerchantStore = this.CreateWanderingMerchant();
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
                def.MerchantStore = this.CreatePotionGirl();
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 377;
                def.Designation = "Angela the Supplier";
                def.NpcWindow = NpcWindow.Merchant;
                def.MerchantStore = this.CreatePotionGirl();
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
                def.MerchantStore = this.CreatePotionGirl();
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 416;
                def.Designation = "Rhea";
                def.NpcWindow = NpcWindow.Merchant;
                def.MerchantStore = this.CreatePotionGirl();
                this.gameConfiguration.Monsters.Add(def);
            }

            {
                var def = this.context.CreateNew<MonsterDefinition>();
                def.Number = 417;
                def.Designation = "Marce";
                def.MerchantStore = this.CreatePotionGirl();
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
                def.MerchantStore = this.CreatePotionGirl();
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

            itemList.Add(ItemBuilder.PotionAtSlot(0).Potion(0).Make());
            itemList.Add(ItemBuilder.PotionAtSlot(8).Potion(0).StackSize(3).Make());
            itemList.Add(ItemBuilder.PotionAtSlot(16).Potion(0).PotionLevel(1).Make());
            itemList.Add(ItemBuilder.PotionAtSlot(24).Potion(0).StackSize(3).PotionLevel(1).Make());

            itemList.Add(ItemBuilder.PotionAtSlot(1).Potion(1).Make());
            itemList.Add(ItemBuilder.PotionAtSlot(9).Potion(1).StackSize(3).Make());
            itemList.Add(ItemBuilder.PotionAtSlot(17).Potion(1).PotionLevel(1).Make());
            itemList.Add(ItemBuilder.PotionAtSlot(25).Potion(1).StackSize(3).PotionLevel(1).Make());

            itemList.Add(ItemBuilder.PotionAtSlot(2).Potion(2).Make());
            itemList.Add(ItemBuilder.PotionAtSlot(10).Potion(2).StackSize(3).Make());
            itemList.Add(ItemBuilder.PotionAtSlot(18).Potion(2).PotionLevel(1).Make());
            itemList.Add(ItemBuilder.PotionAtSlot(26).Potion(2).StackSize(3).PotionLevel(1).Make());

            itemList.Add(ItemBuilder.PotionAtSlot(3).Potion(3).Make());
            itemList.Add(ItemBuilder.PotionAtSlot(11).Potion(3).StackSize(3).Make());
            itemList.Add(ItemBuilder.PotionAtSlot(19).Potion(3).PotionLevel(1).Make());
            itemList.Add(ItemBuilder.PotionAtSlot(27).Potion(3).StackSize(3).PotionLevel(1).Make());

            itemList.Add(ItemBuilder.PotionAtSlot(4).Potion(4).Make());
            itemList.Add(ItemBuilder.PotionAtSlot(12).Potion(4).StackSize(3).Make());
            itemList.Add(ItemBuilder.PotionAtSlot(20).Potion(4).PotionLevel(1).Make());
            itemList.Add(ItemBuilder.PotionAtSlot(28).Potion(4).StackSize(3).PotionLevel(1).Make());

            itemList.Add(ItemBuilder.PotionAtSlot(5).Potion(5).Make());
            itemList.Add(ItemBuilder.PotionAtSlot(13).Potion(5).StackSize(3).Make());
            itemList.Add(ItemBuilder.PotionAtSlot(21).Potion(5).PotionLevel(1).Make());
            itemList.Add(ItemBuilder.PotionAtSlot(29).Potion(5).StackSize(3).PotionLevel(1).Make());

            itemList.Add(ItemBuilder.PotionAtSlot(6).Potion(6).Make());
            itemList.Add(ItemBuilder.PotionAtSlot(14).Potion(6).StackSize(3).Make());
            itemList.Add(ItemBuilder.PotionAtSlot(22).Potion(6).PotionLevel(1).Make());
            itemList.Add(ItemBuilder.PotionAtSlot(30).Potion(6).StackSize(3).PotionLevel(1).Make());

            itemList.Add(ItemBuilder.PotionAtSlot(7).Potion(9).Make());

            // TODO: insert Town Portal item.
            // TODO: insert Ammo (Bolts and Arrows).
            // TODO: insert Armor of Guardsman.
            return this.CreateMerchantStore(itemList);
        }

        private ItemStorage CreateWanderingMerchant()
        {
            List<Item> itemList = new List<Item>();

            // Leather set +0+4+L
            itemList.Add(ItemBuilder.SetItemAtSlot(0).SetNumber(5).Helm().OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(16).SetNumber(5).Armor().OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(40).SetNumber(5).Pants().OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(56).SetNumber(5).Boots().OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(72).SetNumber(5).Gloves().OptionLevel(1).WithLuck().Make());

            // Bronze set +2+4+L
            itemList.Add(ItemBuilder.SetItemAtSlot(2).SetNumber(0).Helm().Level(2).OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(18).SetNumber(0).Armor().Level(2).OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(34).SetNumber(0).Pants().Level(2).OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(50).SetNumber(0).Boots().Level(2).OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(66).SetNumber(0).Gloves().Level(2).OptionLevel(1).WithLuck().Make());

            // Scale set +3+4+L
            itemList.Add(ItemBuilder.SetItemAtSlot(4).SetNumber(6).Helm().Level(3).OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(20).SetNumber(6).Armor().Level(3).OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(36).SetNumber(6).Pants().Level(3).OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(52).SetNumber(6).Boots().Level(3).OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(68).SetNumber(6).Gloves().Level(3).OptionLevel(1).WithLuck().Make());

            // Brass set +3+4+L
            itemList.Add(ItemBuilder.SetItemAtSlot(6).SetNumber(8).Helm().Level(3).OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(22).SetNumber(8).Armor().Level(3).OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(38).SetNumber(8).Pants().Level(3).OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(54).SetNumber(8).Boots().Level(3).OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(70).SetNumber(8).Gloves().Level(3).OptionLevel(1).WithLuck().Make());

            // Brass set +3+4+L
            itemList.Add(ItemBuilder.SetItemAtSlot(88).SetNumber(9).Helm().Level(3).OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(104).SetNumber(9).Armor().Level(3).OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(82).SetNumber(9).Pants().Level(3).OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(98).SetNumber(9).Boots().Level(3).OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(84).SetNumber(9).Gloves().Level(3).OptionLevel(1).WithLuck().Make());

            return this.CreateMerchantStore(itemList);
        }

        private ItemStorage CreateHanzoTheBlacksmith()
        {
            List<Item> itemList = new List<Item>();

            itemList.Add(ItemBuilder.ShieldAtSlot(0).Shield(0).OptionLevel(1).WithLuck().Make());                       // Small Shield     +0+5+L
            itemList.Add(ItemBuilder.ShieldAtSlot(2).Shield(4).Level(1).OptionLevel(1).WithLuck().WithSkill().Make());  // Buckler          +1+5+L+S
            itemList.Add(ItemBuilder.ShieldAtSlot(4).Shield(1).Level(2).OptionLevel(1).WithLuck().Make());              // Horn Shield      +2+5+L
            itemList.Add(ItemBuilder.ShieldAtSlot(6).Shield(2).Level(3).OptionLevel(1).WithLuck().Make());              // Kite Shield      +3+5+L

            itemList.Add(ItemBuilder.ShieldAtSlot(16).Shield(6).Level(3).OptionLevel(1).WithLuck().WithSkill().Make());     // Skull Shield     +3+5+L+S
            itemList.Add(ItemBuilder.ShieldAtSlot(18).Shield(10).Level(3).OptionLevel(1).WithLuck().WithSkill().Make());    // Big Round Shield +3+5+L+S
            itemList.Add(ItemBuilder.ShieldAtSlot(20).Shield(9).Level(3).OptionLevel(1).WithLuck().WithSkill().Make());     // Plate Shield     +3+5+L+S
            itemList.Add(ItemBuilder.ShieldAtSlot(22).Shield(7).Level(3).OptionLevel(1).WithLuck().WithSkill().Make());     // Spiked Shield    +3+5+L+S

            itemList.Add(ItemBuilder.ShieldAtSlot(32).Shield(5).Level(3).OptionLevel(1).WithLuck().WithSkill().Make());     // Dragon Slayer    +3+5+L+S
            itemList.Add(ItemBuilder.ShieldAtSlot(34).Shield(8).Level(3).OptionLevel(1).WithLuck().WithSkill().Make());     // Tower Shield     +3+5+L+S
            itemList.Add(ItemBuilder.ShieldAtSlot(36).Shield(11).Level(3).OptionLevel(1).WithLuck().WithSkill().Make());    // Serpent Shield   +3+5+L+S
            itemList.Add(ItemBuilder.ShieldAtSlot(38).Shield(12).Level(3).OptionLevel(1).WithLuck().WithSkill().Make());    // Bronze Shield    +3+5+L+S

            itemList.Add(ItemBuilder.WeaponAtSlot(48).Sword(1).OptionLevel(1).WithLuck().Make());                   // Short Sword          +0+4+L
            itemList.Add(ItemBuilder.WeaponAtSlot(49).Axe(1).Level(1).OptionLevel(1).WithLuck().Make());            // Hand Axe             +1+4+L
            itemList.Add(ItemBuilder.WeaponAtSlot(50).MaceOrBlunt(0).Level(2).OptionLevel(1).WithLuck().Make());    // Mace                 +2+4+L
            itemList.Add(ItemBuilder.WeaponAtSlot(51).Sword(2).Level(2).OptionLevel(1).WithLuck().Make());          // Rapier               +2+4+L
            itemList.Add(ItemBuilder.WeaponAtSlot(52).Axe(2).Level(2).OptionLevel(1).WithLuck().Make());            // Double Axe           +2+4+L
            itemList.Add(ItemBuilder.WeaponAtSlot(53).Sword(4).Level(3).OptionLevel(1).WithLuck().Make());          // Sword of Assassin    +3+4+L
            itemList.Add(ItemBuilder.WeaponAtSlot(54).MaceOrBlunt(1).Level(3).OptionLevel(1).WithLuck().Make());    // Morning Star         +3+4+L
            itemList.Add(ItemBuilder.WeaponAtSlot(55).Axe(3).Level(3).OptionLevel(1).WithLuck().Make());            // Tomahawk     +3+4+L

            itemList.Add(ItemBuilder.WeaponAtSlot(72).Sword(0).Level(2).OptionLevel(1).WithLuck().Make());              // Kris             +2+4+L
            itemList.Add(ItemBuilder.WeaponAtSlot(73).Sword(6).Level(3).OptionLevel(1).WithLuck().WithSkill().Make());  // Gladius          +3+4+L+S
            itemList.Add(ItemBuilder.WeaponAtSlot(74).Sword(7).Level(3).OptionLevel(1).WithLuck().WithSkill().Make());  // Falchion         +3+4+L+S
            itemList.Add(ItemBuilder.WeaponAtSlot(75).Sword(8).Level(3).OptionLevel(1).WithLuck().Make());              // Serpent Sword    +3+4+L
            itemList.Add(ItemBuilder.WeaponAtSlot(76).Sword(5).Level(3).OptionLevel(1).WithLuck().WithSkill().Make());  // Blade            +3+4+L+S

            return this.CreateMerchantStore(itemList);
        }

        private ItemStorage CreatePasiTheMageStore()
        {
            List<Item> itemList = new List<Item>();

            itemList.Add(ItemBuilder.ScrollAtSlot(0).Scroll(3).Make());     // Scroll of Fire Ball
            itemList.Add(ItemBuilder.ScrollAtSlot(1).Scroll(10).Make());    // Scroll of Power Wave
            itemList.Add(ItemBuilder.ScrollAtSlot(2).Scroll(2).Make());     // Scroll of Lighting
            itemList.Add(ItemBuilder.ScrollAtSlot(3).Scroll(1).Make());     // Scroll of Meteorite
            itemList.Add(ItemBuilder.ScrollAtSlot(4).Scroll(5).Make());     // Scroll of Teleport
            itemList.Add(ItemBuilder.ScrollAtSlot(5).Scroll(6).Make());     // Scroll of Ice
            itemList.Add(ItemBuilder.ScrollAtSlot(6).Scroll(0).Make());     // Scroll of Poison

            itemList.Add(ItemBuilder.OrbAtSlot(7).Orb(13).Make());          // Orb of Implate
            itemList.Add(ItemBuilder.OrbAtSlot(15).Orb(7).Make());          // Orb of Twisting Slash

            // Pad set +0+4+L
            itemList.Add(ItemBuilder.SetItemAtSlot(16).SetNumber(2).Helm().OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(32).SetNumber(2).Armor().OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(48).SetNumber(2).Pants().OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(64).SetNumber(2).Boots().OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(80).SetNumber(2).Gloves().OptionLevel(1).WithLuck().Make());

            // Bone set +2+4+L
            itemList.Add(ItemBuilder.SetItemAtSlot(18).SetNumber(4).Helm().Level(2).OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(34).SetNumber(4).Armor().Level(2).OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(50).SetNumber(4).Pants().Level(2).OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(66).SetNumber(4).Boots().Level(2).OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(82).SetNumber(4).Gloves().Level(2).OptionLevel(1).WithLuck().Make());

            // Sphinx set +3+4+L
            itemList.Add(ItemBuilder.SetItemAtSlot(20).SetNumber(7).Helm().Level(3).OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(36).SetNumber(7).Armor().Level(3).OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(60).SetNumber(7).Pants().Level(3).OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(76).SetNumber(7).Boots().Level(3).OptionLevel(1).WithLuck().Make());
            itemList.Add(ItemBuilder.SetItemAtSlot(92).SetNumber(7).Gloves().Level(3).OptionLevel(1).WithLuck().Make());

            itemList.Add(ItemBuilder.WeaponAtSlot(22).StaffOrBook(0).OptionLevel(1).WithLuck().Make());             // Skull Staff      +0+4+L
            itemList.Add(ItemBuilder.WeaponAtSlot(46).StaffOrBook(1).Level(2).OptionLevel(1).WithLuck().Make());    // Angelic Staff    +2+4+L
            itemList.Add(ItemBuilder.WeaponAtSlot(70).StaffOrBook(2).Level(3).OptionLevel(1).WithLuck().Make());    // Serpent Staff    +3+4+L
            itemList.Add(ItemBuilder.WeaponAtSlot(94).StaffOrBook(3).Level(3).OptionLevel(1).WithLuck().Make());    // Thunder Staff    +3+4+L

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
    }
}
