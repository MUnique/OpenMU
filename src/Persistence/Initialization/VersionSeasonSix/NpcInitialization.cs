// <copyright file="NpcInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// The initialization of all NPCs, which are no monsters.
/// </summary>
internal partial class NpcInitialization : Version095d.NpcInitialization
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NpcInitialization" /> class.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public NpcInitialization(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Creates all NPCs.
    /// </summary>
    /// <remarks>
    /// Extracted from Monsters.txt by Regex: (?m)^(\d+)\t1\t"(.*?)".*?$
    /// Replace by: yield return new MonsterDefinition() { Number = $1, Designation="$2" };
    /// yield return new (\w*) { Number = (\d+), Designation = (".*?").*?(, NpcWindow = (.*) ){0,1}};
    /// Replace by: <![CDATA[ {\n    var def = this.Context.CreateNew<$1>();\n    def.Number = $2;\n    def.Designation = $3;\n    def.NpcWindow = $5;\n    this.GameConfiguration.Monsters.Add(def);\n}\n ]]>.
    /// </remarks>
    public override void Initialize()
    {
        base.Initialize();
        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 226;
            def.Designation = "Pet Trainer";
            def.NpcWindow = NpcWindow.PetTrainer;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 229;
            def.Designation = "Marlon";
            def.NpcWindow = NpcWindow.LegacyQuest;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 230;
            def.Designation = "Alex";
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.MerchantStore = this.CreateAlexStore(230);
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 231;
            def.Designation = "Thompson the Merchant";
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.MerchantStore = this.CreatePotionGirlItemStorage(def.Number);
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 232;
            def.Designation = "Archangel";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 233;
            def.Designation = "Messenger of Arch.";
            def.NpcWindow = NpcWindow.BloodCastle;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 256;
            def.Designation = "Lahap";
            def.NpcWindow = NpcWindow.Lahap;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 257;
            def.Designation = "Elf Soldier";
            def.NpcWindow = NpcWindow.NpcDialog;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 259;
            def.Designation = "Oracle Layla";
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.MerchantStore = this.CreatePotionGirlItemStorage(def.Number);
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 375;
            def.Designation = "Chaos Card Master";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.NpcWindow = NpcWindow.ChaosCardCombination;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 376;
            def.Designation = "Pamela the Supplier";
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.MerchantStore = this.CreatePotionGirlItemStorage(def.Number);
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 377;
            def.Designation = "Angela the Supplier";
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.MerchantStore = this.CreatePotionGirlItemStorage(def.Number);
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 378;
            def.Designation = "GameMaster";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 379;
            def.Designation = "Fireworks Girl";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 371;
            def.Designation = "Leo The Helper";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 372;
            def.Designation = "Elite Skill Soldier";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 380;
            def.Designation = "Stone Statue";
            def.ObjectKind = NpcObjectKind.Statue;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 381;
            def.Designation = "MU Allies General";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 382;
            def.Designation = "Illusion Elder";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 383;
            def.Designation = "Alliance Item Storage";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 384;
            def.Designation = "Illusion Item Storage";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 385;
            def.Designation = "Mirage";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 215;
            def.Designation = "Shield";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 216;
            def.Designation = "Crown";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 217;
            def.Designation = "Crown Switch1";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 218;
            def.Designation = "Crown Switch2";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 219;
            def.Designation = "Castle Gate Switch";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 220;
            def.Designation = "Guard";
            def.ObjectKind = NpcObjectKind.Guard;
            def.MoveRange = 3;
            def.AttackRange = 2;
            def.ViewRange = 8;
            def.IntelligenceTypeName = typeof(GuardIntelligence).FullName;
            def.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            def.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            def.RespawnDelay = new TimeSpan(3 * TimeSpan.TicksPerSecond);
            def.NumberOfMaximumItemDrops = 0;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 2 },
                { Stats.MaximumHealth, 500 },
                { Stats.MinimumPhysBaseDmg, 15 },
                { Stats.MaximumPhysBaseDmg, 30 },
                { Stats.AttackRatePvm, 30 },
                { Stats.DefenseRatePvm, 20 },
                { Stats.DefenseBase, 70 },
            };
            def.AddAttributes(attributes, this.Context, this.GameConfiguration);
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 221;
            def.Designation = "Slingshot Attack";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 222;
            def.Designation = "Slingshot Defense";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 223;
            def.Designation = "Senior";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 224;
            def.Designation = "Guardsman";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 277;
            def.Designation = "Castle Gate1";
            def.ObjectKind = NpcObjectKind.Gate;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 278;
            def.Designation = "Life Stone";
            def.ObjectKind = NpcObjectKind.Statue;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 283;
            def.Designation = "Guardian Statue";
            def.ObjectKind = NpcObjectKind.Statue;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 285;
            def.Designation = "Guardian";
            def.ObjectKind = NpcObjectKind.Guard;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 286;
            def.Designation = "Battle Guard1";
            def.ObjectKind = NpcObjectKind.Guard;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 287;
            def.Designation = "Battle Guard2";
            def.ObjectKind = NpcObjectKind.Guard;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 288;
            def.Designation = "Canon Tower";
            def.ObjectKind = NpcObjectKind.Trap;
            def.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.MonsterSkill);
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 367;
            def.Designation = "Gateway Machine";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 368;
            def.Designation = "Elphis";
            def.NpcWindow = NpcWindow.ElphisRefinery;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 369;
            def.Designation = "Osbourne";
            def.NpcWindow = NpcWindow.RefineStoneMaking;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 370;
            def.Designation = "Jerridon";
            def.NpcWindow = NpcWindow.RemoveJohOption;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 404;
            def.Designation = "MU Allies";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 405;
            def.Designation = "Illusion Sorcerer";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 406;
            def.Designation = "Priest Devin";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.NpcWindow = NpcWindow.LegacyQuest;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 407;
            def.Designation = "Werewolf Quarrel";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 408;
            def.Designation = "Gatekeeper";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 415;
            def.Designation = "Silvia";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.NpcWindow = NpcWindow.Merchant;
            def.MerchantStore = this.CreatePotionGirlItemStorage(def.Number);
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 416;
            def.Designation = "Rhea";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.NpcWindow = NpcWindow.Merchant;
            def.MerchantStore = this.CreateRheaStore(def.Number);
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 417;
            def.Designation = "Marce";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.MerchantStore = this.CreateMarceStore(def.Number);
            def.NpcWindow = NpcWindow.Merchant;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 450;
            def.Designation = "Cherry Blossom Spirit";
            def.NpcWindow = NpcWindow.CherryBlossomBranchesAssembly;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 451;
            def.Designation = "Cherry Blossom Tree";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 452;
            def.Designation = "Seed Master";
            def.NpcWindow = NpcWindow.SeedMaster;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 453;
            def.Designation = "Seed Researcher";
            def.NpcWindow = NpcWindow.SeedResearcher;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 467;
            def.Designation = "Snowman";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 468;
            def.Designation = "Little Santa Yellow";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 469;
            def.Designation = "Little Santa Green";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 470;
            def.Designation = "Little Santa Red";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 471;
            def.Designation = "Little Santa Blue";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 472;
            def.Designation = "Little Santa White";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 473;
            def.Designation = "Little Santa Black";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 474;
            def.Designation = "Little Santa Orange";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 475;
            def.Designation = "Little Santa Pink";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 476;
            def.Designation = "Cursed Santa";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 477;
            def.Designation = "Transformed Snowman";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 478;
            def.Designation = "Delgado - Lucky Coins";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 479;
            def.Designation = "Gatekeeper Titus";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.NpcWindow = NpcWindow.DoorkeeperTitusDuelWatch;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 492;
            def.Designation = "Moss The Merchant";
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 522;
            def.Designation = "Adviser Jerinteu";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 540;
            def.Designation = "Lugard";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 541;
            def.Designation = "Compensation Box";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 542;
            def.Designation = "Golden Compensation Box";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 543;
            def.Designation = "Gens Duprian";
            def.NpcWindow = NpcWindow.NpcDialog;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 544;
            def.Designation = "Gens Vanert";
            def.NpcWindow = NpcWindow.NpcDialog;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 566;
            def.Designation = "Mercenary Guild Felicia";
            def.NpcWindow = NpcWindow.NpcDialog;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 152;
            def.Designation = "Gate to Kalima 1 of {0}";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 153;
            def.Designation = "Gate to Kalima 2 of {0}";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 154;
            def.Designation = "Gate to Kalima 3 of {0}";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 155;
            def.Designation = "Gate to Kalima 4 of {0}";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 156;
            def.Designation = "Gate to Kalima 5 of {0}";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 157;
            def.Designation = "Gate to Kalima 6 of {0}";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 158;
            def.Designation = "Gate to Kalima 7 of {0}";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 131;
            def.Designation = "Castle Gate";
            def.ObjectKind = NpcObjectKind.Destructible;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.MaximumHealth, 5000000 },
            };
            def.AddAttributes(attributes, this.Context, this.GameConfiguration);
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 132;
            def.Designation = "Statue of Saint";
            def.ObjectKind = NpcObjectKind.Destructible;
            def.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.MaximumHealth, 5000000 },
            };
            def.AddAttributes(attributes, this.Context, this.GameConfiguration);
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
            var questItemDrop = this.Context.CreateNew<DropItemGroup>();
            questItemDrop.SetGuid(132);
            questItemDrop.Chance = 1;
            questItemDrop.Description = "Archangel Weapon (Blood Castle)";
            questItemDrop.Monster = def;
            questItemDrop.PossibleItems.Add(this.GameConfiguration.Items.First(item => item.IsArchangelQuestItem()));
            def.DropItemGroups.Add(questItemDrop);
            this.GameConfiguration.DropItemGroups.Add(questItemDrop);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 133;
            def.Designation = "Statue of Saint";
            def.ObjectKind = NpcObjectKind.Destructible;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.MaximumHealth, 5000000 },
            };
            def.AddAttributes(attributes, this.Context, this.GameConfiguration);
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 134;
            def.Designation = "Statue of Saint";
            def.ObjectKind = NpcObjectKind.Destructible;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.MaximumHealth, 5000000 },
            };
            def.AddAttributes(attributes, this.Context, this.GameConfiguration);
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 579;
            def.Designation = "David";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 577;
            def.Designation = "Leina the General Goods Merchant";
            def.MerchantStore = this.CreatePotionGirlItemStorage(def.Number);
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 578;
            def.Designation = "Weapons Merchant Bolo";
            def.NpcWindow = NpcWindow.Merchant;
            def.MerchantStore = this.CreateBoloStore(def.Number);
            def.SetGuid(def.Number);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 545;
            def.Designation = "Christine the General Goods Merchant";
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 546;
            def.Designation = "Jeweler Raul";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 547;
            def.Designation = "Market Union Member Julia";
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 568;
            def.Designation = "Wandering Merchant Zyro";
            def.NpcWindow = NpcWindow.NpcDialog;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 658;
            def.Designation = "Cursed Statue";
            def.ObjectKind = NpcObjectKind.Statue;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 659;
            def.Designation = "Captured Stone Statue (1)";
            def.ObjectKind = NpcObjectKind.Statue;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 660;
            def.Designation = "Captured Stone Statue (2)";
            def.ObjectKind = NpcObjectKind.Statue;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 661;
            def.Designation = "Captured Stone Statue (3)";
            def.ObjectKind = NpcObjectKind.Statue;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 662;
            def.Designation = "Captured Stone Statue (4)";
            def.ObjectKind = NpcObjectKind.Statue;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 663;
            def.Designation = "Captured Stone Statue (5)";
            def.ObjectKind = NpcObjectKind.Statue;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 664;
            def.Designation = "Captured Stone Statue (6)";
            def.ObjectKind = NpcObjectKind.Statue;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 665;
            def.Designation = "Captured Stone Statue (7)";
            def.ObjectKind = NpcObjectKind.Statue;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 666;
            def.Designation = "Captured Stone Statue (8)";
            def.ObjectKind = NpcObjectKind.Statue;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 667;
            def.Designation = "Captured Stone Statue (9)";
            def.ObjectKind = NpcObjectKind.Statue;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 668;
            def.Designation = "Captured Stone Statue (10)";
            def.ObjectKind = NpcObjectKind.Statue;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }
    }
}