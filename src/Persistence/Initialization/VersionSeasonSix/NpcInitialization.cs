// <copyright file="NpcInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Interfaces;
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
            def.Designation = LocalizedString.FromResource(() => MonsterNames.PetTrainer);
            def.NpcWindow = NpcWindow.PetTrainer;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 229;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.Marlon);
            def.NpcWindow = NpcWindow.LegacyQuest;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 230;
            def.Designation = LocalizedString.FromResource(() => MerchantNames.Alex);
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.MerchantStore = this.CreateAlexStore(230);
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 231;
            def.Designation = LocalizedString.FromResource(() => MerchantNames.ThompsonTheMerchant);
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.MerchantStore = this.CreatePotionGirlItemStorage(def.Number);
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 232;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.Archangel);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 233;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.MessengerOfArch);
            def.NpcWindow = NpcWindow.BloodCastle;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 256;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.Lahap);
            def.NpcWindow = NpcWindow.Lahap;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 257;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.ElfSoldier);
            def.NpcWindow = NpcWindow.NpcDialog;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        // Elf Soldier Buff
        {
            var buffEffect = this.Context.CreateNew<MagicEffectDefinition>();
            this.GameConfiguration.MagicEffects.Add(buffEffect);
            buffEffect.Number = (short)MagicEffectNumber.ElfSoldierBuff;
            buffEffect.Name = LocalizedString.FromResource(() => MagicEffectNames.ElfSoldierBuff);
            buffEffect.InformObservers = true;
            buffEffect.StopByDeath = true;

            // Duration: 60 minutes
            buffEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
            buffEffect.Duration.ConstantValue.Value = 3600;

            // Defense boost: 50 + (Level / 5)
            var defensePowerUp = this.Context.CreateNew<PowerUpDefinition>();
            defensePowerUp.TargetAttribute = Stats.DefenseFinal.GetPersistent(this.GameConfiguration);
            defensePowerUp.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
            defensePowerUp.Boost.ConstantValue.Value = 50;
            defensePowerUp.Boost.ConstantValue.AggregateType = AggregateType.AddFinal;
            var defensePerLevel = this.Context.CreateNew<AttributeRelationship>();
            defensePerLevel.InputAttribute = Stats.Level.GetPersistent(this.GameConfiguration);
            defensePerLevel.InputOperand = 1f / 5;
            defensePerLevel.InputOperator = InputOperator.Multiply;
            defensePowerUp.Boost.RelatedValues.Add(defensePerLevel);
            buffEffect.PowerUpDefinitions.Add(defensePowerUp);

            // Damage boost: 45 + (Level / 3)
            var damagePowerUp = this.Context.CreateNew<PowerUpDefinition>();
            damagePowerUp.TargetAttribute = Stats.GreaterDamageBonus.GetPersistent(this.GameConfiguration);
            damagePowerUp.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
            damagePowerUp.Boost.ConstantValue.Value = 45;
            damagePowerUp.Boost.ConstantValue.AggregateType = AggregateType.AddRaw;
            var damagePerLevel = this.Context.CreateNew<AttributeRelationship>();
            damagePerLevel.InputAttribute = Stats.Level.GetPersistent(this.GameConfiguration);
            damagePerLevel.InputOperand = 1f / 3;
            damagePerLevel.InputOperator = InputOperator.Multiply;
            damagePowerUp.Boost.RelatedValues.Add(damagePerLevel);
            buffEffect.PowerUpDefinitions.Add(damagePowerUp);

            var elfSoldier = this.GameConfiguration.Monsters.First(m => m.Number == 257);
            var buff = this.Context.CreateNew<Buff>();
            buff.MagicEffectDefinition = buffEffect;
            buff.MaximumLevel = 220;
            elfSoldier.Buffs.Add(buff);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 259;
            def.Designation = LocalizedString.FromResource(() => MerchantNames.OracleLayla);
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.MerchantStore = this.CreatePotionGirlItemStorage(def.Number);
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 375;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.ChaosCardMaster);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.NpcWindow = NpcWindow.ChaosCardCombination;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 376;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.PamelaTheSupplier);
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.MerchantStore = this.CreatePotionGirlItemStorage(def.Number);
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 377;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.AngelaTheSupplier);
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.MerchantStore = this.CreatePotionGirlItemStorage(def.Number);
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 378;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.GameMaster);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 379;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.FireworksGirl);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 371;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.LeoTheHelper);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 372;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.EliteSkillSoldier);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 380;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.StoneStatue);
            def.ObjectKind = NpcObjectKind.Statue;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 381;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.MUAlliesGeneral);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 382;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.IllusionElder);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 383;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.AllianceItemStorage);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 384;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.IllusionItemStorage);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 385;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.Mirage);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 215;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.Shield);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 216;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.Crown);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 217;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.CrownSwitch1);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 218;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.CrownSwitch2);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 219;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.CastleGateSwitch);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 220;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.Guard);
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
            def.Designation = LocalizedString.FromResource(() => MonsterNames.SlingshotAttack);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 222;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.SlingshotDefense);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 223;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.Senior);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.NpcWindow = NpcWindow.CastleSeniorNPC;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 224;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.Guardsman);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 277;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.CastleGate1);
            def.ObjectKind = NpcObjectKind.Gate;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 278;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.LifeStone);
            def.ObjectKind = NpcObjectKind.Statue;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 283;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.GuardianStatue);
            def.ObjectKind = NpcObjectKind.Statue;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 285;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.Guardian);
            def.ObjectKind = NpcObjectKind.Guard;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 286;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.BattleGuard1);
            def.ObjectKind = NpcObjectKind.Guard;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 287;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.BattleGuard2);
            def.ObjectKind = NpcObjectKind.Guard;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 288;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.CanonTower);
            def.ObjectKind = NpcObjectKind.Trap;
            def.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.MonsterSkill);
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 367;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.GatewayMachine);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 368;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.Elphis);
            def.NpcWindow = NpcWindow.ElphisRefinery;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 369;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.Osbourne);
            def.NpcWindow = NpcWindow.RefineStoneMaking;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 370;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.Jerridon);
            def.NpcWindow = NpcWindow.RemoveJohOption;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 404;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.MUAllies);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 405;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.IllusionSorcerer);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 406;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.PriestDevin);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.NpcWindow = NpcWindow.LegacyQuest;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 407;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.WerewolfQuarrel);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 408;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.Gatekeeper);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 415;
            def.Designation = LocalizedString.FromResource(() => MerchantNames.Silvia);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.NpcWindow = NpcWindow.Merchant;
            def.MerchantStore = this.CreatePotionGirlItemStorage(def.Number);
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 416;
            def.Designation = LocalizedString.FromResource(() => MerchantNames.Rhea);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.NpcWindow = NpcWindow.Merchant;
            def.MerchantStore = this.CreateRheaStore(def.Number);
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 417;
            def.Designation = LocalizedString.FromResource(() => MerchantNames.Marce);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.MerchantStore = this.CreateMarceStore(def.Number);
            def.NpcWindow = NpcWindow.Merchant;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 450;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.CherryBlossomSpirit);
            def.NpcWindow = NpcWindow.CherryBlossomBranchesAssembly;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 451;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.CherryBlossomTree);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 452;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.SeedMaster);
            def.NpcWindow = NpcWindow.SeedMaster;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 453;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.SeedResearcher);
            def.NpcWindow = NpcWindow.SeedResearcher;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 467;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.Snowman);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 468;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.LittleSantaYellow);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 469;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.LittleSantaGreen);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 470;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.LittleSantaRed);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 471;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.LittleSantaBlue);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 472;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.LittleSantaWhite);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 473;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.LittleSantaBlack);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 474;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.LittleSantaOrange);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 475;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.LittleSantaPink);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 476;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.CursedSanta);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 477;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.TransformedSnowman);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 478;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.DelgadoLuckyCoins);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 479;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.GatekeeperTitus);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.NpcWindow = NpcWindow.DoorkeeperTitusDuelWatch;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 492;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.MossTheMerchant);
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 522;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.AdviserJerinteu);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 540;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.Lugard);
            def.NpcWindow = NpcWindow.LugardDoppelgangerEntry;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 541;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.CompensationBox);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 542;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.GoldenCompensationBox);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 543;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.GensDuprian);
            def.NpcWindow = NpcWindow.NpcDialog;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 544;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.GensVanert);
            def.NpcWindow = NpcWindow.NpcDialog;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 566;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.MercenaryGuildFelicia);
            def.NpcWindow = NpcWindow.NpcDialog;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 152;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.GateToKalima1OfPlayer);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 153;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.GateToKalima2OfPlayer);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 154;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.GateToKalima3OfPlayer);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 155;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.GateToKalima4OfPlayer);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 156;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.GateToKalima5OfPlayer);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 157;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.GateToKalima6OfPlayer);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 158;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.GateToKalima7OfPlayer);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
            def.SetGuid(def.Number);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 131;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.CastleGate);
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
            def.Designation = LocalizedString.FromResource(() => MonsterNames.StatueOfSaint);
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
            questItemDrop.Description = LocalizedString.FromResource(() => DropGroupDescriptions.ArchangelWeaponBloodCastle);
            questItemDrop.Monster = def;
            questItemDrop.PossibleItems.Add(this.GameConfiguration.Items.First(item => item.IsArchangelQuestItem()));
            def.DropItemGroups.Add(questItemDrop);
            this.GameConfiguration.DropItemGroups.Add(questItemDrop);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 133;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.StatueOfSaint);
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
            def.Designation = LocalizedString.FromResource(() => MonsterNames.StatueOfSaint);
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
            def.Designation = LocalizedString.FromResource(() => MonsterNames.David);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 577;
            def.Designation = LocalizedString.FromResource(() => MerchantNames.LeinaTheGeneralGoodsMerchant);
            def.MerchantStore = this.CreatePotionGirlItemStorage(def.Number);
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 578;
            def.Designation = LocalizedString.FromResource(() => MerchantNames.WeaponsMerchantBolo);
            def.NpcWindow = NpcWindow.Merchant;
            def.MerchantStore = this.CreateBoloStore(def.Number);
            def.SetGuid(def.Number);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 545;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.ChristineTheGeneralGoodsMerchant);
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.MerchantStore = this.CreatePotionGirlItemStorage(def.Number);
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 546;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.JewelerRaul);
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 547;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.MarketUnionMemberJulia);
            def.NpcWindow = NpcWindow.JuliaWarpMarketServer;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 568;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.WanderingMerchantZyro);
            def.NpcWindow = NpcWindow.NpcDialog;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 658;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.CursedStatue);
            def.ObjectKind = NpcObjectKind.Statue;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 659;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.CapturedStoneStatue1);
            def.ObjectKind = NpcObjectKind.Statue;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 660;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.CapturedStoneStatue2);
            def.ObjectKind = NpcObjectKind.Statue;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 661;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.CapturedStoneStatue3);
            def.ObjectKind = NpcObjectKind.Statue;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 662;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.CapturedStoneStatue4);
            def.ObjectKind = NpcObjectKind.Statue;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 663;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.CapturedStoneStatue5);
            def.ObjectKind = NpcObjectKind.Statue;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 664;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.CapturedStoneStatue6);
            def.ObjectKind = NpcObjectKind.Statue;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 665;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.CapturedStoneStatue7);
            def.ObjectKind = NpcObjectKind.Statue;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 666;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.CapturedStoneStatue8);
            def.ObjectKind = NpcObjectKind.Statue;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 667;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.CapturedStoneStatue9);
            def.ObjectKind = NpcObjectKind.Statue;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 668;
            def.Designation = LocalizedString.FromResource(() => MonsterNames.CapturedStoneStatue10);
            def.ObjectKind = NpcObjectKind.Statue;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }
    }
}
