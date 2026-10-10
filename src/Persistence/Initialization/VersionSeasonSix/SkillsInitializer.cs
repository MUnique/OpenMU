// <copyright file="SkillsInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;

// ReSharper disable StringLiteralTypo
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// Initialization logic for <see cref="Skill"/>s.
/// </summary>
internal class SkillsInitializer : SkillsInitializerBase
{
#pragma warning disable SA1600 // formula constants are documented by their name and inline skill number comment
    internal const string Formula1204 = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 6)))) * 10"; // 17
    internal const string Formula61408 = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12)) * 85 * 6"; // 12
    internal const string Formula51173 = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12)) * 85 * 5"; // 13
    internal const string Formula181 = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 6)))) * 1.5"; // 7
    internal const string FormulaRecoveryIncrease181 = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 6)))) * 1.5 * 0.01"; // 7
    internal const string Formula120 = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12))"; // 1    // about 1.2 to 9.0
    internal const string Formula120Value = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12)) * 0.01"; // 1    // about 0.012 to 0.09
    internal const string FormulaRecoveryIncrease120 = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12)) / 100"; // 1
    internal const string FormulaIncreaseMultiplicator120 = "(101 + (((((((level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12)) / 100"; // 1
    internal const string Formula6020 = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 6)))) * 50"; // 16
    internal const string Formula6020Value = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 6)))) * 50 / 100"; // 16
    internal const string Formula502 = "(0.8 + (((((((level - 30) ^ 3) + 25000) / 499) / 6)))) * 5";
    internal const string Formula632 = "(0.85 + (((((((level - 30) ^ 3) + 25000) / 499) / 6)))) * 6"; // 3
    internal const string Formula883 = "(0.9 + (((((((level - 30) ^ 3) + 25000) / 499) / 6)))) * 8"; // 4
    internal const string Formula10235 = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12)) * 85"; // 9
    internal const string Formula81877 = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12)) * 85 * 8"; // 14
    internal const string Formula1154 = "(0.95 + (((((((level - 30) ^ 3) + 25000) / 499) / 6)))) * 10"; // 5
    internal const string Formula803 = "(0.8 + (((((((level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12)) * 8"; // 10
    internal const string Formula1 = "1 * level";
    internal const string Formula1WhenComplete = "if(level < 10; 0; 1)";
    internal const string Formula722 = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 6)))) * 6"; // 18 // 7.22 to 54.09
    internal const string Formula722Value = "((1 + (((((((level - 30) ^ 3) + 25000) / 499) / 6)))) * 6) * 0.01"; // 18   // 0.0722 to 0.5409
    internal const string Formula4319 = "52 / (1 + (((((((level - 30) ^ 3) + 25000) / 499) / 6))))"; // 6
    internal const string Formula914 = "11 / (1 + (((((((level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12))"; // 11
    internal const string Formula3822 = "40 / (1 + (((((((level - 30) ^ 3) + 25000) / 499) / 6)))) + 5"; // 20
    internal const string Formula25587 = "(1 + ( ( ( ( ( ( (level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12)) * 85 * 2.5"; // 29
    internal const string Formula30704 = "(1 + ( ( ( ( ( ((level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12)) * 85 * 3"; // 33
    internal const string Formula3371 = "(1 + ( ( ( ( ( ( (level - 30) ^ 3) + 25000) / 499) / 6) ) ) ) * 28"; // 35
    internal const string Formula20469 = "(1 + ( ( ( ( ( ( (level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12) ) * 85 * 2"; // 30
    internal const string Formula1806 = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 6)))) * 15"; // 15
    internal const string Formula32751 = "(1 + ( ( ( ( ( ( (level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12)) * 85 * 3.2"; // 31
    internal const string Formula5418 = "(1 + ( ( ( ( ( ( (level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12)) * 45"; // 34
#pragma warning restore SA1600

    private static readonly IDictionary<SkillNumber, MagicEffectNumber> EffectsOfSkills = new Dictionary<SkillNumber, MagicEffectNumber>
    {
        { SkillNumber.SwellLife, MagicEffectNumber.GreaterFortitude },
        { SkillNumber.IncreaseCriticalDamage, MagicEffectNumber.CriticalDamageIncrease },
        { SkillNumber.SoulBarrier, MagicEffectNumber.SoulBarrier },
        { SkillNumber.Defense, MagicEffectNumber.ShieldSkill },
        { SkillNumber.GreaterDefense, MagicEffectNumber.GreaterDefense },
        { SkillNumber.GreaterDamage, MagicEffectNumber.GreaterDamage },
        { SkillNumber.Heal, MagicEffectNumber.Heal },
        { SkillNumber.Recovery, MagicEffectNumber.ShieldRecover },
        { SkillNumber.InfinityArrow, MagicEffectNumber.InfiniteArrow },
        { SkillNumber.FireSlash, MagicEffectNumber.DefenseReduction },
        { SkillNumber.IgnoreDefense, MagicEffectNumber.IgnoreDefense },
        { SkillNumber.IncreaseHealth, MagicEffectNumber.IncreaseHealth },
        { SkillNumber.IncreaseBlock, MagicEffectNumber.IncreaseBlock },
        { SkillNumber.ExpansionofWizardry, MagicEffectNumber.WizEnhance },
        { SkillNumber.Berserker, MagicEffectNumber.Berserker },
        { SkillNumber.KillingBlow, MagicEffectNumber.Weakness },
        { SkillNumber.Sleep, MagicEffectNumber.Sleep },
        { SkillNumber.Weakness, MagicEffectNumber.WeaknessSummoner },
        { SkillNumber.Innovation, MagicEffectNumber.Innovation },
        { SkillNumber.DamageReflection, MagicEffectNumber.Reflection },
        { SkillNumber.BeastUppercut, MagicEffectNumber.DefenseReductionBeastUppercut },
        { SkillNumber.PhoenixShot, MagicEffectNumber.DecreaseBlock },
        { SkillNumber.Explosion223, MagicEffectNumber.Explosion },
        { SkillNumber.Requiem, MagicEffectNumber.Requiem },
        { SkillNumber.FireBurstMastery, MagicEffectNumber.Stunned },
        { SkillNumber.EarthshakeMastery, MagicEffectNumber.Stunned },
        { SkillNumber.CritDmgIncPowUp3, MagicEffectNumber.CriticalDamageIncreaseMastery },
        { SkillNumber.SwellLifeProficiency, MagicEffectNumber.GreaterFortitudeProficiency },
        { SkillNumber.ExpansionofWizStreng, MagicEffectNumber.WizEnhanceStrengthener },
        { SkillNumber.ExpansionofWizMas, MagicEffectNumber.WizEnhanceMastery },
        { SkillNumber.StaminaIncreaseStrengthener, MagicEffectNumber.IncreaseHealthStrengthener },
        { SkillNumber.DefSuccessRateIncPowUp, MagicEffectNumber.IncreaseBlockPowerUp },
        { SkillNumber.DefSuccessRateIncMastery, MagicEffectNumber.IncreaseBlockMastery },
    };

    private readonly IDictionary<byte, MasterSkillRoot> _masterSkillRoots;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkillsInitializer"/> class.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public SkillsInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
        this._masterSkillRoots = new SortedDictionary<byte, MasterSkillRoot>();
    }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <remarks>
    /// Regex: (?m)^\s*(\d+)\s+\"(.+?)\"\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(-*\d+)\s(-*\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s*$
    /// Replace by: this.CreateSkill($1, "$2", $3, $4, $5, $6, $7, $9, $10, $11, $12, $13, $15, $19, $20, $21, $22, $23, $24, $25, $26, $27, $28);.
    /// </remarks>
    public override void Initialize()
    {
        this.CreateSkill(SkillNumber.Poison, LocalizedString.FromResource(() => SkillNames.Poison), CharacterClasses.AllMagicians, DamageType.Wizardry, 12, 6, manaConsumption: 42, energyRequirement: 140, elementalModifier: ElementalType.Poison);
        this.CreateSkill(SkillNumber.Meteorite, LocalizedString.FromResource(() => SkillNames.Meteorite), CharacterClasses.AllMagicians | CharacterClasses.AllSummoners, DamageType.Wizardry, 21, 6, manaConsumption: 12, energyRequirement: 104, elementalModifier: ElementalType.Earth);
        this.CreateSkill(SkillNumber.Lightning, LocalizedString.FromResource(() => SkillNames.Lightning), CharacterClasses.AllMagicians, DamageType.Wizardry, 17, 6, manaConsumption: 15, energyRequirement: 72, elementalModifier: ElementalType.Lightning);
        this.CreateSkill(SkillNumber.FireBall, LocalizedString.FromResource(() => SkillNames.FireBall), CharacterClasses.AllMagicians | CharacterClasses.AllSummoners, DamageType.Wizardry, 8, 6, manaConsumption: 3, energyRequirement: 40, elementalModifier: ElementalType.Fire);
        this.CreateSkill(SkillNumber.Flame, LocalizedString.FromResource(() => SkillNames.Flame), CharacterClasses.AllMagicians, DamageType.Wizardry, 25, 6, manaConsumption: 50, energyRequirement: 160, elementalModifier: ElementalType.Fire, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.Flame, false, default, default, default, true, TimeSpan.Zero, TimeSpan.FromMilliseconds(500), 0, 2, default, default, 0.5f, targetAreaDiameter: 2, useTargetAreaFilter: true);
        this.CreateSkill(SkillNumber.Teleport, LocalizedString.FromResource(() => SkillNames.Teleport), CharacterClasses.SoulMasterAndGrandMaster, DamageType.Wizardry, distance: 6, manaConsumption: 30, energyRequirement: 88, skillType: SkillType.Other);
        this.CreateSkill(SkillNumber.Ice, LocalizedString.FromResource(() => SkillNames.Ice), CharacterClasses.AllMagicians | CharacterClasses.AllSummoners, DamageType.Wizardry, 10, 6, manaConsumption: 38, energyRequirement: 120, elementalModifier: ElementalType.Ice);
        this.CreateSkill(SkillNumber.Twister, LocalizedString.FromResource(() => SkillNames.Twister), CharacterClasses.AllMagicians, DamageType.Wizardry, 35, 6, manaConsumption: 60, energyRequirement: 180, elementalModifier: ElementalType.Wind, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.Twister, true, 1.5f, 1.5f, 4f, true, TimeSpan.FromMilliseconds(300), TimeSpan.FromMilliseconds(1000), 0, 2, default, default, 0.7f);
        this.CreateSkill(SkillNumber.EvilSpirit, LocalizedString.FromResource(() => SkillNames.EvilSpirit), CharacterClasses.AllMagicians, DamageType.Wizardry, 45, 7, manaConsumption: 90, energyRequirement: 220, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.EvilSpirit, false, default, default, default, true, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(1000), 0, 2, default, default, 0.7f);
        this.CreateSkill(SkillNumber.Hellfire, LocalizedString.FromResource(() => SkillNames.Hellfire), CharacterClasses.AllMagicians, DamageType.Wizardry, 120, 4, manaConsumption: 160, energyRequirement: 260, elementalModifier: ElementalType.Fire, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.Hellfire, false, 0, 0, 0, effectRange: 2);
        this.CreateSkill(SkillNumber.PowerWave, LocalizedString.FromResource(() => SkillNames.PowerWave), CharacterClasses.AllMagicians | CharacterClasses.AllSummoners, DamageType.Wizardry, 14, 6, manaConsumption: 5, energyRequirement: 56);
        this.CreateSkill(SkillNumber.AquaBeam, LocalizedString.FromResource(() => SkillNames.AquaBeam), CharacterClasses.AllMagicians, DamageType.Wizardry, 80, 6, manaConsumption: 140, energyRequirement: 345, elementalModifier: ElementalType.Water, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.AquaBeam, true, 1.5f, 1.5f, 8f);
        this.CreateSkill(SkillNumber.Cometfall, LocalizedString.FromResource(() => SkillNames.Cometfall), CharacterClasses.AllMagicians, DamageType.Wizardry, 70, 3, manaConsumption: 150, energyRequirement: 436, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.Cometfall, false, default, default, default, targetAreaDiameter: 2, useTargetAreaFilter: true);
        this.CreateSkill(SkillNumber.Inferno, LocalizedString.FromResource(() => SkillNames.Inferno), CharacterClasses.AllMagicians, DamageType.Wizardry, 100, 4, manaConsumption: 200, energyRequirement: 578, elementalModifier: ElementalType.Fire, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.TeleportAlly, LocalizedString.FromResource(() => SkillNames.TeleportAlly), CharacterClasses.SoulMasterAndGrandMaster, distance: 6, abilityConsumption: 25, manaConsumption: 90, energyRequirement: 644, skillType: SkillType.Other);
        this.CreateSkill(SkillNumber.SoulBarrier, LocalizedString.FromResource(() => SkillNames.SoulBarrier), CharacterClasses.SoulMasterAndGrandMaster, distance: 6, abilityConsumption: 22, manaConsumption: 70, energyRequirement: 408, skillType: SkillType.Buff, implicitTargetRange: 0, targetRestriction: SkillTargetRestriction.Party);
        this.CreateSkill(SkillNumber.EnergyBall, LocalizedString.FromResource(() => SkillNames.EnergyBall), CharacterClasses.AllMagicians, DamageType.Wizardry, 3, 6, manaConsumption: 1);
        this.CreateSkill(SkillNumber.Defense, LocalizedString.FromResource(() => SkillNames.Defense), CharacterClasses.AllKnightsLordsAndMGs, manaConsumption: 30, skillType: SkillType.Buff, implicitTargetRange: 0, targetRestriction: SkillTargetRestriction.Self);
        this.CreateSkill(SkillNumber.FallingSlash, LocalizedString.FromResource(() => SkillNames.FallingSlash), CharacterClasses.AllKnightsLordsAndMGs | CharacterClasses.AllFighters, DamageType.Physical, distance: 3, manaConsumption: 9, movesToTarget: true, movesTarget: true);
        this.CreateSkill(SkillNumber.Lunge, LocalizedString.FromResource(() => SkillNames.Lunge), CharacterClasses.AllKnightsLordsAndMGs, DamageType.Physical, distance: 2, manaConsumption: 9, movesToTarget: true, movesTarget: true);
        this.CreateSkill(SkillNumber.Uppercut, LocalizedString.FromResource(() => SkillNames.Uppercut), CharacterClasses.AllKnightsLordsAndMGs, DamageType.Physical, distance: 2, manaConsumption: 8, movesToTarget: true, movesTarget: true);
        this.CreateSkill(SkillNumber.Cyclone, LocalizedString.FromResource(() => SkillNames.Cyclone), CharacterClasses.AllKnightsLordsAndMGs, DamageType.Physical, distance: 2, manaConsumption: 9, movesToTarget: true, movesTarget: true);
        this.CreateSkill(SkillNumber.Slash, LocalizedString.FromResource(() => SkillNames.Slash), CharacterClasses.AllKnightsLordsAndMGs, DamageType.Physical, distance: 2, manaConsumption: 10, movesToTarget: true, movesTarget: true);
        this.CreateSkill(SkillNumber.TripleShot, LocalizedString.FromResource(() => SkillNames.TripleShot), CharacterClasses.AllElfs, DamageType.Physical, distance: 6, manaConsumption: 5, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.TripleShot, true, 1f, 4.5f, 7f, true, TimeSpan.FromMilliseconds(50), maximumHitsPerTarget: 3, maximumHitsPerAttack: 3, projectileCount: 3);
        this.CreateSkill(SkillNumber.Heal, LocalizedString.FromResource(() => SkillNames.Heal), CharacterClasses.AllElfs, distance: 6, manaConsumption: 20, energyRequirement: 52, skillType: SkillType.Regeneration, targetRestriction: SkillTargetRestriction.Player);
        this.CreateSkill(SkillNumber.GreaterDefense, LocalizedString.FromResource(() => SkillNames.GreaterDefense), CharacterClasses.AllElfs, distance: 6, manaConsumption: 30, energyRequirement: 72, skillType: SkillType.Buff, targetRestriction: SkillTargetRestriction.Player);
        this.CreateSkill(SkillNumber.GreaterDamage, LocalizedString.FromResource(() => SkillNames.GreaterDamage), CharacterClasses.AllElfs, distance: 6, manaConsumption: 40, energyRequirement: 92, skillType: SkillType.Buff, targetRestriction: SkillTargetRestriction.Player);
        this.CreateSkill(SkillNumber.SummonGoblin, LocalizedString.FromResource(() => SkillNames.SummonGoblin), CharacterClasses.AllElfs, manaConsumption: 40, energyRequirement: 90, skillType: SkillType.SummonMonster);
        this.CreateSkill(SkillNumber.SummonStoneGolem, LocalizedString.FromResource(() => SkillNames.SummonStoneGolem), CharacterClasses.AllElfs, manaConsumption: 70, energyRequirement: 170, skillType: SkillType.SummonMonster);
        this.CreateSkill(SkillNumber.SummonAssassin, LocalizedString.FromResource(() => SkillNames.SummonAssassin), CharacterClasses.AllElfs, manaConsumption: 110, energyRequirement: 190, skillType: SkillType.SummonMonster);
        this.CreateSkill(SkillNumber.SummonEliteYeti, LocalizedString.FromResource(() => SkillNames.SummonEliteYeti), CharacterClasses.AllElfs, manaConsumption: 160, energyRequirement: 230, skillType: SkillType.SummonMonster);
        this.CreateSkill(SkillNumber.SummonDarkKnight, LocalizedString.FromResource(() => SkillNames.SummonDarkKnight), CharacterClasses.AllElfs, manaConsumption: 200, energyRequirement: 250, skillType: SkillType.SummonMonster);
        this.CreateSkill(SkillNumber.SummonBali, LocalizedString.FromResource(() => SkillNames.SummonBali), CharacterClasses.AllElfs, manaConsumption: 250, energyRequirement: 260, skillType: SkillType.SummonMonster);
        this.CreateSkill(SkillNumber.SummonSoldier, LocalizedString.FromResource(() => SkillNames.SummonSoldier), CharacterClasses.AllElfs, manaConsumption: 350, energyRequirement: 280, skillType: SkillType.SummonMonster);
        this.CreateSkill(SkillNumber.Decay, LocalizedString.FromResource(() => SkillNames.Decay), CharacterClasses.SoulMasterAndGrandMaster, DamageType.Wizardry, 95, 6, 7, 110, energyRequirement: 953, elementalModifier: ElementalType.Poison, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.Decay, false, 0, 0, 0, effectRange: 2);
        this.CreateSkill(SkillNumber.IceStorm, LocalizedString.FromResource(() => SkillNames.IceStorm), CharacterClasses.SoulMasterAndGrandMaster, DamageType.Wizardry, 80, 6, 5, 100, energyRequirement: 849, elementalModifier: ElementalType.Ice, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.IceStorm, false, default, default, default, true, TimeSpan.Zero, TimeSpan.Zero, targetAreaDiameter: 3, useTargetAreaFilter: true);
        this.CreateSkill(SkillNumber.Nova, LocalizedString.FromResource(() => SkillNames.Nova), CharacterClasses.SoulMasterAndGrandMaster, DamageType.Wizardry, distance: 6, manaConsumption: 180 / 12 /* mana per stage */, levelRequirement: 100, energyRequirement: 1052, elementalModifier: ElementalType.Fire);
        this.CreateSkill(SkillNumber.NovaStart, LocalizedString.FromResource(() => SkillNames.NovaStart), CharacterClasses.SoulMasterAndGrandMaster, DamageType.None, abilityConsumption: 45, levelRequirement: 100, energyRequirement: 1052, skillType: SkillType.Other);
        this.CreateSkill(SkillNumber.TwistingSlash, LocalizedString.FromResource(() => SkillNames.TwistingSlash), CharacterClasses.AllKnights | CharacterClasses.AllMGs, DamageType.Physical, distance: 2, abilityConsumption: 10, manaConsumption: 10, elementalModifier: ElementalType.Wind, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.RagefulBlow, LocalizedString.FromResource(() => SkillNames.RagefulBlow), CharacterClasses.BladeKnightAndBladeMaster, DamageType.Physical, 60, 3, 20, 25, 170, elementalModifier: ElementalType.Earth, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.DeathStab, LocalizedString.FromResource(() => SkillNames.DeathStab), CharacterClasses.BladeKnightAndBladeMaster, DamageType.Physical, 70, 2, 12, 15, 160, elementalModifier: ElementalType.Wind, skillType: SkillType.DirectHit, skillTarget: SkillTarget.ExplicitWithImplicitInRange, implicitTargetRange: 1);
        this.CreateSkill(SkillNumber.CrescentMoonSlash, LocalizedString.FromResource(() => SkillNames.CrescentMoonSlash), CharacterClasses.AllKnights, DamageType.Physical, 90, 4, 15, 22, movesToTarget: true, movesTarget: true);
        this.CreateSkill(SkillNumber.Lance, LocalizedString.FromResource(() => SkillNames.Lance), CharacterClasses.SoulMasterAndGrandMaster | CharacterClasses.AllSummoners, DamageType.Wizardry, 90, 6, 10, 150);
        this.CreateSkill(SkillNumber.Starfall, LocalizedString.FromResource(() => SkillNames.Starfall), CharacterClasses.AllElfs, DamageType.Physical, 120, 8, 15, 20);
        this.CreateSkill(SkillNumber.Impale, LocalizedString.FromResource(() => SkillNames.Impale), CharacterClasses.AllKnights | CharacterClasses.AllMGs, DamageType.Physical, 15, 3, manaConsumption: 8, levelRequirement: 28);
        this.CreateSkill(SkillNumber.SwellLife, LocalizedString.FromResource(() => SkillNames.SwellLife), CharacterClasses.AllKnights, abilityConsumption: 24, manaConsumption: 22, levelRequirement: 120, skillType: SkillType.Buff, skillTarget: SkillTarget.ImplicitParty);
        this.CreateSkill(SkillNumber.FireBreath, LocalizedString.FromResource(() => SkillNames.FireBreath), CharacterClasses.AllKnights, DamageType.Physical, 30, 3, manaConsumption: 9, levelRequirement: 110);
        this.CreateSkill(SkillNumber.FlameofEvil, LocalizedString.FromResource(() => SkillNames.FlameOfEvilMonster), damage: 120, manaConsumption: 160, levelRequirement: 60, energyRequirement: 100);
        this.CreateSkill(SkillNumber.IceArrow, LocalizedString.FromResource(() => SkillNames.IceArrow), CharacterClasses.MuseElfAndHighElf, DamageType.Physical, 105, 8, 12, 10, elementalModifier: ElementalType.Ice);
        this.CreateSkill(SkillNumber.Penetration, LocalizedString.FromResource(() => SkillNames.Penetration), CharacterClasses.AllElfs, DamageType.Physical, 70, 6, 9, 7, 130, elementalModifier: ElementalType.Wind, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.Penetration, true, 1.1f, 1.2f, 8f, useDeferredHits: true, delayPerOneDistance: TimeSpan.FromMilliseconds(50));
        this.CreateSkill(SkillNumber.FireSlash, LocalizedString.FromResource(() => SkillNames.FireSlash), CharacterClasses.AllMGs, DamageType.Physical, 80, 2, 20, 15, elementalModifier: ElementalType.Fire, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.FireSlash, true, 1.5f, 2, 2);
        this.CreateSkill(SkillNumber.PowerSlash, LocalizedString.FromResource(() => SkillNames.PowerSlash), CharacterClasses.AllMGs, DamageType.Physical, distance: 5, manaConsumption: 15, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.PowerSlash, true, 1.0f, 6.0f, 6.0f);
        this.CreateSkill(SkillNumber.SpiralSlash, LocalizedString.FromResource(() => SkillNames.SpiralSlash), CharacterClasses.AllMGs, DamageType.Physical, 75, 5, 15, 20);
        this.CreateSkill(SkillNumber.Force, LocalizedString.FromResource(() => SkillNames.Force), CharacterClasses.AllLords, DamageType.Physical, 10, 4, manaConsumption: 10);
        this.CreateSkill(SkillNumber.FireBurst, LocalizedString.FromResource(() => SkillNames.FireBurst), CharacterClasses.AllLords, DamageType.Physical, 100, 6, manaConsumption: 25, energyRequirement: 79, skillTarget: SkillTarget.ExplicitWithImplicitInRange, implicitTargetRange: 1);
        this.CreateSkill(SkillNumber.Earthshake, LocalizedString.FromResource(() => SkillNames.Earthshake), CharacterClasses.AllLords, DamageType.Physical, 150, 10, 50, elementalModifier: ElementalType.Lightning, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.Earthshake, false, 0, 0, 0, useTargetAreaFilter: true, targetAreaDiameter: 10, minimumHitsPerAttack: 9, maximumHitsPerAttack: 15);
        this.CreateSkill(SkillNumber.Summon, LocalizedString.FromResource(() => SkillNames.Summon), CharacterClasses.AllLords, abilityConsumption: 30, manaConsumption: 70, energyRequirement: 153, leadershipRequirement: 400, skillType: SkillType.Other);
        this.CreateSkill(SkillNumber.IncreaseCriticalDamage, LocalizedString.FromResource(() => SkillNames.IncreaseCriticalDamage), CharacterClasses.AllLords, abilityConsumption: 50, manaConsumption: 50, energyRequirement: 102, leadershipRequirement: 300, skillType: SkillType.Buff, skillTarget: SkillTarget.ImplicitParty);
        this.CreateSkill(SkillNumber.ElectricSpike, LocalizedString.FromResource(() => SkillNames.ElectricSpike), CharacterClasses.AllLords, DamageType.Physical, 250, 10, 100, energyRequirement: 126, leadershipRequirement: 340, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.ElectricSpike, true, 1.5f, 1.5f, 12f, useDeferredHits: true, delayPerOneDistance: TimeSpan.FromMilliseconds(10));
        this.CreateSkill(SkillNumber.ForceWave, LocalizedString.FromResource(() => SkillNames.ForceWave), CharacterClasses.AllLords, DamageType.Physical, 50, 4, manaConsumption: 10, skillTarget: SkillTarget.ExplicitWithImplicitInRange);
        this.AddAreaSkillSettings(SkillNumber.ForceWave, true, 1f, 1f, 4f);
        this.CreateSkill(SkillNumber.Stun, LocalizedString.FromResource(() => SkillNames.Stun), CharacterClasses.All, distance: 2, abilityConsumption: 50, manaConsumption: 70, skillType: SkillType.AreaSkillAutomaticHits, cooldownMinutes: 4);
        this.AddAreaSkillSettings(SkillNumber.Stun, true, 1.5f, 1.5f, 3f);
        this.CreateSkill(SkillNumber.CancelStun, LocalizedString.FromResource(() => SkillNames.CancelStun), CharacterClasses.All, abilityConsumption: 30, manaConsumption: 25, skillType: SkillType.Other, cooldownMinutes: 2);
        this.CreateSkill(SkillNumber.SwellMana, LocalizedString.FromResource(() => SkillNames.SwellMana), CharacterClasses.All, abilityConsumption: 30, manaConsumption: 35, cooldownMinutes: 4);
        this.CreateSkill(SkillNumber.Invisibility, LocalizedString.FromResource(() => SkillNames.Invisibility), CharacterClasses.All, abilityConsumption: 60, manaConsumption: 80, cooldownMinutes: 5);
        this.CreateSkill(SkillNumber.CancelInvisibility, LocalizedString.FromResource(() => SkillNames.CancelInvisibility), CharacterClasses.All, abilityConsumption: 30, manaConsumption: 40, cooldownMinutes: 2);
        this.CreateSkill(SkillNumber.AbolishMagic, LocalizedString.FromResource(() => SkillNames.AbolishMagic), CharacterClasses.AllLords, abilityConsumption: 70, manaConsumption: 90, cooldownMinutes: 8);
        this.CreateSkill(SkillNumber.ManaRays, LocalizedString.FromResource(() => SkillNames.ManaRays), CharacterClasses.AllMGs, DamageType.Wizardry, 85, 6, 7, 130);
        this.CreateSkill(SkillNumber.FireBlast, LocalizedString.FromResource(() => SkillNames.FireBlast), CharacterClasses.AllLords, DamageType.Physical, 150, 6, 10, 30);
        this.CreateSkill(SkillNumber.PlasmaStorm, LocalizedString.FromResource(() => SkillNames.PlasmaStorm), CharacterClasses.AllMastersAndSecondClass, DamageType.Fenrir, damage: 60, distance: 6, abilityConsumption: 20, manaConsumption: 50, levelRequirement: 110, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.InfinityArrow, LocalizedString.FromResource(() => SkillNames.InfinityArrow), CharacterClasses.MuseElfAndHighElf, distance: 6, abilityConsumption: 10, manaConsumption: 50, levelRequirement: 220, skillType: SkillType.Buff, targetRestriction: SkillTargetRestriction.Self);
        this.CreateSkill(SkillNumber.FireScream, LocalizedString.FromResource(() => SkillNames.FireScream), CharacterClasses.AllLords, DamageType.Physical, 130, 6, 10, 45, energyRequirement: 70, leadershipRequirement: 150, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.FireScream, true, 2f, 3f, 6f);
        this.CreateSkill(SkillNumber.Explosion79, LocalizedString.FromResource(() => SkillNames.Explosion), CharacterClasses.None, DamageType.Physical, distance: 2);
        this.CreateSkill(SkillNumber.SummonMonster, LocalizedString.FromResource(() => SkillNames.SummonMonster), manaConsumption: 40, energyRequirement: 90);
        this.CreateSkill(SkillNumber.MagicAttackImmunity, LocalizedString.FromResource(() => SkillNames.MagicAttackImmunity), manaConsumption: 40, energyRequirement: 90);
        this.CreateSkill(SkillNumber.PhysicalAttackImmunity, LocalizedString.FromResource(() => SkillNames.PhysicalAttackImmunity), manaConsumption: 40, energyRequirement: 90);
        this.CreateSkill(SkillNumber.PotionofBless, LocalizedString.FromResource(() => SkillNames.PotionOfBless), manaConsumption: 40, energyRequirement: 90);
        this.CreateSkill(SkillNumber.PotionofSoul, LocalizedString.FromResource(() => SkillNames.PotionOfSoul), manaConsumption: 40, energyRequirement: 90);
        this.CreateSkill(SkillNumber.SpellofProtection, LocalizedString.FromResource(() => SkillNames.SpellOfProtection), CharacterClasses.All, manaConsumption: 30, elementalModifier: ElementalType.Ice, cooldownMinutes: 5);
        this.CreateSkill(SkillNumber.SpellofRestriction, LocalizedString.FromResource(() => SkillNames.SpellOfRestriction), CharacterClasses.All, distance: 3, manaConsumption: 30, elementalModifier: ElementalType.Ice, cooldownMinutes: 5);
        this.CreateSkill(SkillNumber.SpellofPursuit, LocalizedString.FromResource(() => SkillNames.SpellOfPursuit), CharacterClasses.All, manaConsumption: 30, elementalModifier: ElementalType.Ice, cooldownMinutes: 10);
        this.CreateSkill(SkillNumber.ShieldBurn, LocalizedString.FromResource(() => SkillNames.ShieldBurn), CharacterClasses.All, distance: 3, manaConsumption: 30, elementalModifier: ElementalType.Ice, cooldownMinutes: 5);
        this.CreateSkill(SkillNumber.DrainLife, LocalizedString.FromResource(() => SkillNames.DrainLife), CharacterClasses.AllSummoners, DamageType.Wizardry, 35, 6, manaConsumption: 50, energyRequirement: 150, skillType: SkillType.AreaSkillExplicitTarget);
        this.CreateSkill(SkillNumber.ChainLightning, LocalizedString.FromResource(() => SkillNames.ChainLightning), CharacterClasses.AllSummoners, DamageType.Wizardry, 70, 6, manaConsumption: 85, energyRequirement: 245, skillType: SkillType.AreaSkillExplicitTarget, skillTarget: SkillTarget.Explicit);
        this.CreateSkill(SkillNumber.DamageReflection, LocalizedString.FromResource(() => SkillNames.DamageReflection), CharacterClasses.AllSummoners, distance: 5, abilityConsumption: 10, manaConsumption: 40, energyRequirement: 375, skillType: SkillType.Buff);
        this.CreateSkill(SkillNumber.Berserker, LocalizedString.FromResource(() => SkillNames.Berserker), CharacterClasses.AllSummoners, distance: 5, abilityConsumption: 50, manaConsumption: 100, energyRequirement: 620, skillType: SkillType.Buff, targetRestriction: SkillTargetRestriction.Self);
        this.CreateSkill(SkillNumber.Sleep, LocalizedString.FromResource(() => SkillNames.Sleep), CharacterClasses.AllSummoners, distance: 6, abilityConsumption: 3, manaConsumption: 20, energyRequirement: 180, skillType: SkillType.Buff);
        this.CreateSkill(SkillNumber.Weakness, LocalizedString.FromResource(() => SkillNames.Weakness), CharacterClasses.AllSummoners, distance: 6, abilityConsumption: 15, manaConsumption: 50, energyRequirement: 663, skillType: SkillType.Buff);
        this.AddAreaSkillSettings(SkillNumber.Weakness, false, 0, 0, 0, maximumHitsPerAttack: 5, useTargetAreaFilter: true, targetAreaDiameter: 10);
        this.CreateSkill(SkillNumber.Innovation, LocalizedString.FromResource(() => SkillNames.Innovation), CharacterClasses.AllSummoners, distance: 6, abilityConsumption: 15, manaConsumption: 70, energyRequirement: 912, skillType: SkillType.Buff);
        this.AddAreaSkillSettings(SkillNumber.Innovation, false, 0, 0, 0, maximumHitsPerAttack: 5, useTargetAreaFilter: true, targetAreaDiameter: 10);
        this.CreateSkill(SkillNumber.Explosion223, LocalizedString.FromResource(() => SkillNames.Explosion), CharacterClasses.AllSummoners, DamageType.Curse, 40, 6, 5, 90, energyRequirement: 100, elementalModifier: ElementalType.Fire, skillType: SkillType.AreaSkillAutomaticHits); // Book of Samut's skill
        this.AddAreaSkillSettings(SkillNumber.Explosion223, false, 0, 0, 0, effectRange: 2);
        this.CreateSkill(SkillNumber.Requiem, LocalizedString.FromResource(() => SkillNames.Requiem), CharacterClasses.AllSummoners, DamageType.Curse, 65, 6, 10, 110, energyRequirement: 99, elementalModifier: ElementalType.Wind, skillType: SkillType.AreaSkillAutomaticHits); // Book of Neil's skill
        this.AddAreaSkillSettings(SkillNumber.Requiem, false, 0, 0, 0, effectRange: 2);
        this.CreateSkill(SkillNumber.Pollution, LocalizedString.FromResource(() => SkillNames.Pollution), CharacterClasses.AllSummoners, DamageType.Curse, 80, 6, 15, 120, energyRequirement: 115, elementalModifier: ElementalType.Lightning, skillType: SkillType.AreaSkillAutomaticHits); // Book of Lagle's skill
        this.AddAreaSkillSettings(SkillNumber.Pollution, false, 0, 0, 0, minimumHitsPerAttack: 4, maximumHitsPerAttack: 8, effectRange: 3);
        this.CreateSkill(SkillNumber.LightningShock, LocalizedString.FromResource(() => SkillNames.LightningShock), CharacterClasses.AllSummoners, DamageType.Wizardry, 95, 6, 7, 115, energyRequirement: 823, elementalModifier: ElementalType.Lightning, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.LightningShock, false, 0, 0, 0, minimumHitsPerAttack: 5, maximumHitsPerAttack: 12, useTargetAreaFilter: true, targetAreaDiameter: 14);
        this.CreateSkill(SkillNumber.StrikeofDestruction, LocalizedString.FromResource(() => SkillNames.StrikeOfDestruction), CharacterClasses.BladeKnightAndBladeMaster, DamageType.Physical, 110, 5, 24, 30, 100, elementalModifier: ElementalType.Ice, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.ExpansionofWizardry, LocalizedString.FromResource(() => SkillNames.ExpansionOfWizardry), CharacterClasses.SoulMasterAndGrandMaster, distance: 6, abilityConsumption: 50, manaConsumption: 200, levelRequirement: 220, energyRequirement: 118, skillType: SkillType.Buff, targetRestriction: SkillTargetRestriction.Player, skillTarget: SkillTarget.ImplicitPlayer);
        this.CreateSkill(SkillNumber.Recovery, LocalizedString.FromResource(() => SkillNames.Recovery), CharacterClasses.MuseElfAndHighElf, distance: 6, abilityConsumption: 10, manaConsumption: 40, levelRequirement: 100, energyRequirement: 37, skillType: SkillType.Regeneration, targetRestriction: SkillTargetRestriction.Player);
        this.CreateSkill(SkillNumber.MultiShot, LocalizedString.FromResource(() => SkillNames.MultiShot), CharacterClasses.MuseElfAndHighElf, DamageType.Physical, 40, 6, 7, 10, 100, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.MultiShot, true, 1f, 6f, 7f);
        this.CreateSkill(SkillNumber.FlameStrike, LocalizedString.FromResource(() => SkillNames.FlameStrike), CharacterClasses.AllMGs, DamageType.Physical, 140, 3, 25, 20, 100, elementalModifier: ElementalType.Fire, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.FlameStrike, true, 5f, 2f, 4f);
        this.CreateSkill(SkillNumber.GiganticStorm, LocalizedString.FromResource(() => SkillNames.GiganticStorm), CharacterClasses.AllMGs, DamageType.Wizardry, 110, 6, 10, 120, 220, 118, elementalModifier: ElementalType.Wind, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.ChaoticDiseier, LocalizedString.FromResource(() => SkillNames.ChaoticDiseier), CharacterClasses.AllLords, DamageType.Physical, 190, 6, 15, 50, 100, 16, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.ChaoticDiseier, true, 1.5f, 1.5f, 6f, minimumHitsPerAttack: 7);
        this.CreateSkill(SkillNumber.DoppelgangerSelfExplosion, LocalizedString.FromResource(() => SkillNames.DoppelgangerSelfExplosion), CharacterClasses.None, DamageType.Wizardry, 140, 3, 25, 20, 100, elementalModifier: ElementalType.Fire);
        this.CreateSkill(SkillNumber.KillingBlow, LocalizedString.FromResource(() => SkillNames.KillingBlow), CharacterClasses.AllFighters, DamageType.Physical, distance: 2, manaConsumption: 9, elementalModifier: ElementalType.Earth, hitsPerAttack: 4); // 1 packet => 1*4 hits
        this.CreateSkill(SkillNumber.BeastUppercut, LocalizedString.FromResource(() => SkillNames.BeastUppercut), CharacterClasses.AllFighters, DamageType.Physical, distance: 2, manaConsumption: 9, elementalModifier: ElementalType.Fire, hitsPerAttack: 2);  // 2 packets => 2*2 hits
        this.CreateSkill(SkillNumber.ChainDrive, LocalizedString.FromResource(() => SkillNames.ChainDrive), CharacterClasses.AllFighters, DamageType.Physical, distance: 4, abilityConsumption: 20, manaConsumption: 15, levelRequirement: 150, elementalModifier: ElementalType.Ice, hitsPerAttack: 4); // 2 packets => 2*4 hits
        this.CreateSkill(SkillNumber.DarkSide, LocalizedString.FromResource(() => SkillNames.DarkSide), CharacterClasses.AllFighters, DamageType.Physical, distance: 4, manaConsumption: 70, levelRequirement: 180, elementalModifier: ElementalType.Wind);
        this.CreateSkill(SkillNumber.DragonRoar, LocalizedString.FromResource(() => SkillNames.DragonRoar), CharacterClasses.AllFighters, DamageType.Physical, distance: 3, abilityConsumption: 30, manaConsumption: 50, levelRequirement: 150, elementalModifier: ElementalType.Earth, skillType: SkillType.AreaSkillExplicitTarget, hitsPerAttack: 4); // 1 packet => 1*4 hits
        this.CreateSkill(SkillNumber.DragonSlasher, LocalizedString.FromResource(() => SkillNames.DragonSlasher), CharacterClasses.AllFighters, DamageType.Physical, distance: 4, abilityConsumption: 100, manaConsumption: 100, levelRequirement: 200, elementalModifier: ElementalType.Wind);
        this.CreateSkill(SkillNumber.IgnoreDefense, LocalizedString.FromResource(() => SkillNames.IgnoreDefense), CharacterClasses.AllFighters, DamageType.Physical, distance: 3, abilityConsumption: 10, manaConsumption: 50, levelRequirement: 120, energyRequirement: 404, skillType: SkillType.Buff, skillTarget: SkillTarget.ImplicitPlayer);
        this.CreateSkill(SkillNumber.IncreaseHealth, LocalizedString.FromResource(() => SkillNames.IncreaseHealth), CharacterClasses.AllFighters, DamageType.Physical, distance: 7, abilityConsumption: 10, manaConsumption: 50, levelRequirement: 80, energyRequirement: 132, skillType: SkillType.Buff, skillTarget: SkillTarget.ImplicitParty);
        this.CreateSkill(SkillNumber.IncreaseBlock, LocalizedString.FromResource(() => SkillNames.IncreaseBlock), CharacterClasses.AllFighters, DamageType.Physical, distance: 7, abilityConsumption: 10, manaConsumption: 50, levelRequirement: 50, energyRequirement: 80, skillType: SkillType.Buff, skillTarget: SkillTarget.ImplicitParty);
        this.CreateSkill(SkillNumber.Charge, LocalizedString.FromResource(() => SkillNames.Charge), CharacterClasses.AllFighters, DamageType.Physical, 90, 4, 15, 20);
        this.CreateSkill(SkillNumber.PhoenixShot, LocalizedString.FromResource(() => SkillNames.PhoenixShot), CharacterClasses.AllFighters, DamageType.Physical, distance: 4, manaConsumption: 30, elementalModifier: ElementalType.Earth, skillType: SkillType.AreaSkillExplicitTarget, hitsPerAttack: 4); // 1 packet => 1*4 hits

        // Generic monster skills:
        this.CreateSkill(SkillNumber.MonsterSkill, LocalizedString.FromResource(() => SkillNames.GenericMonsterSkill), distance: 5, skillType: SkillType.Other);

        // Skills of Selupan, the boss of the raklion event:
        this.CreateSkill(SkillNumber.SelupanPoison, LocalizedString.FromResource(() => SkillNames.SelupanPoison), damageType: DamageType.Physical, distance: 10, skillType: SkillType.AreaSkillExplicitTarget);
        this.CreateSkill(SkillNumber.SelupanIceStorm, LocalizedString.FromResource(() => SkillNames.SelupanIceStorm), damageType: DamageType.Physical, distance: 10, skillType: SkillType.AreaSkillExplicitTarget);
        this.CreateSkill(SkillNumber.SelupanIceStrike, LocalizedString.FromResource(() => SkillNames.SelupanIceStrike), damageType: DamageType.Physical, distance: 10, skillType: SkillType.AreaSkillExplicitTarget);
        this.CreateSkill(SkillNumber.SelupanFall, LocalizedString.FromResource(() => SkillNames.SelupanFall), damageType: DamageType.Physical, distance: 10, skillType: SkillType.AreaSkillExplicitTarget);

        // Master skills:
        // Common:
        this.CreateSkill(SkillNumber.DurabilityReduction1, LocalizedString.FromResource(() => SkillNames.DurabilityReduction1), CharacterClasses.AllMastersExceptFistMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.PvPDefenceRateInc, LocalizedString.FromResource(() => SkillNames.PvPDefenceRateInc), CharacterClasses.AllMastersExceptFistMaster, damage: 12, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.MaximumSDincrease, LocalizedString.FromResource(() => SkillNames.MaximumSDIncrease), CharacterClasses.AllMastersExceptFistMaster, damage: 13, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.AutomaticManaRecInc, LocalizedString.FromResource(() => SkillNames.AutomaticManaRecInc), CharacterClasses.AllMastersExceptFistMaster, damage: 7, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.PoisonResistanceInc, LocalizedString.FromResource(() => SkillNames.PoisonResistanceInc), CharacterClasses.AllMastersExceptFistMaster, damage: 1, elementalModifier: ElementalType.Poison, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DurabilityReduction2, LocalizedString.FromResource(() => SkillNames.DurabilityReduction2), CharacterClasses.AllMastersExceptFistMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.SdRecoverySpeedInc, LocalizedString.FromResource(() => SkillNames.SDRecoverySpeedInc), CharacterClasses.AllMastersExceptFistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.AutomaticHpRecInc, LocalizedString.FromResource(() => SkillNames.AutomaticHPRecInc), CharacterClasses.AllMastersExceptFistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.LightningResistanceInc, LocalizedString.FromResource(() => SkillNames.LightningResistanceInc), CharacterClasses.AllMastersExceptFistMaster, damage: 1, elementalModifier: ElementalType.Lightning, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DefenseIncrease, LocalizedString.FromResource(() => SkillNames.DefenseIncrease), CharacterClasses.AllMastersExceptFistMaster, damage: 16, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.AutomaticAgRecInc, LocalizedString.FromResource(() => SkillNames.AutomaticAGRecInc), CharacterClasses.AllMastersExceptFistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IceResistanceIncrease, LocalizedString.FromResource(() => SkillNames.IceResistanceIncrease), CharacterClasses.AllMastersExceptFistMaster, damage: 1, elementalModifier: ElementalType.Ice, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DurabilityReduction3, LocalizedString.FromResource(() => SkillNames.DurabilityReduction3Name), CharacterClasses.AllMastersExceptFistMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DefenseSuccessRateInc, LocalizedString.FromResource(() => SkillNames.DefenseSuccessRateInc), CharacterClasses.AllMastersExceptFistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.MaximumLifeIncrease, LocalizedString.FromResource(() => SkillNames.MaximumLifeIncrease), CharacterClasses.AllMastersExceptFistMaster, damage: 9, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.ManaReduction, LocalizedString.FromResource(() => SkillNames.ManaReduction), CharacterClasses.AllMastersExceptFistMaster, damage: 18, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.MonsterAttackSdInc, LocalizedString.FromResource(() => SkillNames.MonsterAttackSDInc), CharacterClasses.AllMastersExceptFistMaster, damage: 11, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.MonsterAttackLifeInc, LocalizedString.FromResource(() => SkillNames.MonsterAttackLifeInc), CharacterClasses.AllMastersExceptFistMaster, damage: 6, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.SwellLifeProficiency, LocalizedString.FromResource(() => SkillNames.SwellLifeProficiency), CharacterClasses.BladeMaster, damage: 7, abilityConsumption: 28, manaConsumption: 26, levelRequirement: 120);
        this.CreateSkill(SkillNumber.MinimumAttackPowerInc, LocalizedString.FromResource(() => SkillNames.MinimumAttackPowerInc), CharacterClasses.BladeMaster | CharacterClasses.DuelMaster | CharacterClasses.LordEmperor, DamageType.Physical, 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.MonsterAttackManaInc, LocalizedString.FromResource(() => SkillNames.MonsterAttackManaInc), CharacterClasses.AllMastersExceptFistMaster, damage: 6, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.PvPAttackRate, LocalizedString.FromResource(() => SkillNames.PvPAttackRate), CharacterClasses.AllMastersExceptFistMaster, damage: 14, skillType: SkillType.PassiveBoost);

        // Blade Master:
        this.CreateSkill(SkillNumber.AttackSuccRateInc, LocalizedString.FromResource(() => SkillNames.AttackSuccRateInc), CharacterClasses.AllMastersExceptFistMaster, damage: 13, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.CycloneStrengthener, LocalizedString.FromResource(() => SkillNames.CycloneStrengthener), CharacterClasses.BladeMaster, DamageType.Physical, 22, 2, manaConsumption: 9);
        this.CreateSkill(SkillNumber.SlashStrengthener, LocalizedString.FromResource(() => SkillNames.SlashStrengthener), CharacterClasses.BladeMaster, DamageType.Physical, 3, 2, manaConsumption: 10);
        this.CreateSkill(SkillNumber.FallingSlashStreng, LocalizedString.FromResource(() => SkillNames.FallingSlashStreng), CharacterClasses.BladeMaster, DamageType.Physical, 3, 3, manaConsumption: 9);
        this.CreateSkill(SkillNumber.LungeStrengthener, LocalizedString.FromResource(() => SkillNames.LungeStrengthener), CharacterClasses.BladeMaster, DamageType.Physical, 3, 2, manaConsumption: 9);
        this.CreateSkill(SkillNumber.TwistingSlashStreng, LocalizedString.FromResource(() => SkillNames.TwistingSlashStreng), CharacterClasses.BladeMaster, DamageType.Physical, 3, 2, 10, 10);
        this.CreateSkill(SkillNumber.RagefulBlowStreng, LocalizedString.FromResource(() => SkillNames.RagefulBlowStreng), CharacterClasses.BladeMaster, DamageType.Physical, 22, 3, 22, 25, 170);
        this.CreateSkill(SkillNumber.TwistingSlashMastery, LocalizedString.FromResource(() => SkillNames.TwistingSlashMastery), CharacterClasses.BladeMaster, DamageType.Physical, 1, 2, 20, 22);
        this.CreateSkill(SkillNumber.RagefulBlowMastery, LocalizedString.FromResource(() => SkillNames.RagefulBlowMastery), CharacterClasses.BladeMaster, DamageType.Physical, 1, 3, 30, 50, 170, elementalModifier: ElementalType.Earth);
        this.CreateSkill(SkillNumber.WeaponMasteryBladeMaster, LocalizedString.FromResource(() => SkillNames.WeaponMastery), CharacterClasses.BladeMaster, damage: 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DeathStabStrengthener, LocalizedString.FromResource(() => SkillNames.DeathStabStrengthener), CharacterClasses.BladeMaster, DamageType.Physical, 22, 2, 13, 15, 160, elementalModifier: ElementalType.Wind);
        this.CreateSkill(SkillNumber.StrikeofDestrStr, LocalizedString.FromResource(() => SkillNames.StrikeOfDestrStr), CharacterClasses.BladeMaster, DamageType.Physical, 22, 5, 24, 30, 100, elementalModifier: ElementalType.Ice);
        this.CreateSkill(SkillNumber.MaximumManaIncrease, LocalizedString.FromResource(() => SkillNames.MaximumManaIncrease), CharacterClasses.AllMastersExceptFistMaster, damage: 9, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.TwoHandedSwordStrengthener, LocalizedString.FromResource(() => SkillNames.TwoHandedSwordStren), CharacterClasses.BladeMaster | CharacterClasses.DuelMaster, DamageType.Physical, 4, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.OneHandedSwordStrengthener, LocalizedString.FromResource(() => SkillNames.OneHandedSwordStren), CharacterClasses.BladeMaster | CharacterClasses.DuelMaster, DamageType.Physical, 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.MaceStrengthener, LocalizedString.FromResource(() => SkillNames.MaceStrengthener), CharacterClasses.BladeMaster, DamageType.Physical, 3, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.SpearStrengthener, LocalizedString.FromResource(() => SkillNames.SpearStrengthener), CharacterClasses.BladeMaster, DamageType.Physical, 3, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.TwoHandedSwordMaster, LocalizedString.FromResource(() => SkillNames.TwoHandedSwordMast), CharacterClasses.BladeMaster | CharacterClasses.DuelMaster, DamageType.Physical, 5, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.OneHandedSwordMaster, LocalizedString.FromResource(() => SkillNames.OneHandedSwordMast), CharacterClasses.BladeMaster | CharacterClasses.DuelMaster, DamageType.Physical, 23, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.MaceMastery, LocalizedString.FromResource(() => SkillNames.MaceMastery), CharacterClasses.BladeMaster, DamageType.Physical, 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.SpearMastery, LocalizedString.FromResource(() => SkillNames.SpearMastery), CharacterClasses.BladeMaster, DamageType.Physical, 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.SwellLifeStrengt, LocalizedString.FromResource(() => SkillNames.SwellLifeStrengt), CharacterClasses.BladeMaster, damage: 7, abilityConsumption: 26, manaConsumption: 24, levelRequirement: 120);

        // Grand Master:
        this.CreateSkill(SkillNumber.FlameStrengthener, LocalizedString.FromResource(() => SkillNames.FlameStrengthener), CharacterClasses.GrandMaster, DamageType.Wizardry, 3, 6, manaConsumption: 55, levelRequirement: 35, energyRequirement: 100, elementalModifier: ElementalType.Fire);
        this.CreateSkill(SkillNumber.LightningStrengthener, LocalizedString.FromResource(() => SkillNames.LightningStrengthener), CharacterClasses.GrandMaster, DamageType.Wizardry, 3, 6, manaConsumption: 20, levelRequirement: 13, energyRequirement: 100, elementalModifier: ElementalType.Lightning);
        this.CreateSkill(SkillNumber.ExpansionofWizStreng, LocalizedString.FromResource(() => SkillNames.ExpansionOfWizStreng), CharacterClasses.GrandMaster, DamageType.Wizardry, 1, 6, 55, 220, 220, 118);
        this.CreateSkill(SkillNumber.InfernoStrengthener, LocalizedString.FromResource(() => SkillNames.InfernoStrengthener), CharacterClasses.GrandMaster, DamageType.Wizardry, 22, manaConsumption: 220, levelRequirement: 88, energyRequirement: 200, elementalModifier: ElementalType.Fire);
        this.CreateSkill(SkillNumber.BlastStrengthener, LocalizedString.FromResource(() => SkillNames.BlastStrengthener), CharacterClasses.GrandMaster, DamageType.Wizardry, 22, 3, manaConsumption: 165, levelRequirement: 80, energyRequirement: 150, elementalModifier: ElementalType.Lightning);
        this.CreateSkill(SkillNumber.ExpansionofWizMas, LocalizedString.FromResource(() => SkillNames.ExpansionOfWizMas), CharacterClasses.GrandMaster, DamageType.Wizardry, 1, 6, 55, 220, 220, 118);
        this.CreateSkill(SkillNumber.PoisonStrengthener, LocalizedString.FromResource(() => SkillNames.PoisonStrengthener), CharacterClasses.GrandMaster, DamageType.Wizardry, 3, 6, manaConsumption: 46, levelRequirement: 30, energyRequirement: 100, elementalModifier: ElementalType.Poison);
        this.CreateSkill(SkillNumber.EvilSpiritStreng, LocalizedString.FromResource(() => SkillNames.EvilSpiritStreng), CharacterClasses.GrandMaster, DamageType.Wizardry, 22, 6, manaConsumption: 108, levelRequirement: 50, energyRequirement: 100);
        this.CreateSkill(SkillNumber.MagicMasteryGrandMaster, LocalizedString.FromResource(() => SkillNames.MagicMastery), CharacterClasses.GrandMaster, damage: 22, levelRequirement: 50, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DecayStrengthener, LocalizedString.FromResource(() => SkillNames.DecayStrengthener), CharacterClasses.GrandMaster, DamageType.Wizardry, 22, 6, 10, 120, 96, 243, elementalModifier: ElementalType.Poison, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.DecayStrengthener, false, 0, 0, 0, effectRange: 2);
        this.CreateSkill(SkillNumber.HellfireStrengthener, LocalizedString.FromResource(() => SkillNames.HellfireStrengthener), CharacterClasses.GrandMaster, DamageType.Wizardry, 3, 4, manaConsumption: 176, levelRequirement: 60, energyRequirement: 100, elementalModifier: ElementalType.Fire, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.HellfireStrengthener, false, 0, 0, 0, effectRange: 2);
        this.CreateSkill(SkillNumber.IceStrengthener, LocalizedString.FromResource(() => SkillNames.IceStrengthener), CharacterClasses.GrandMaster, DamageType.Wizardry, 3, 6, manaConsumption: 42, levelRequirement: 25, energyRequirement: 100, elementalModifier: ElementalType.Ice);
        this.CreateSkill(SkillNumber.OneHandedStaffStrengthener, LocalizedString.FromResource(() => SkillNames.OneHandedStaffStren), CharacterClasses.GrandMaster | CharacterClasses.DuelMaster, DamageType.Wizardry, 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.TwoHandedStaffStrengthener, LocalizedString.FromResource(() => SkillNames.TwoHandedStaffStren), CharacterClasses.GrandMaster | CharacterClasses.DuelMaster, DamageType.Wizardry, 4, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.ShieldStrengthenerGrandMaster, LocalizedString.FromResource(() => SkillNames.ShieldStrengthener), CharacterClasses.GrandMaster, damage: 10, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.OneHandedStaffMaster, LocalizedString.FromResource(() => SkillNames.OneHandedStaffMast), CharacterClasses.GrandMaster | CharacterClasses.DuelMaster, damage: 23, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.TwoHandedStaffMaster, LocalizedString.FromResource(() => SkillNames.TwoHandedStaffMast), CharacterClasses.GrandMaster | CharacterClasses.DuelMaster, DamageType.Wizardry, 5, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.ShieldMasteryGrandMaster, LocalizedString.FromResource(() => SkillNames.ShieldMastery), CharacterClasses.GrandMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.SoulBarrierStrength, LocalizedString.FromResource(() => SkillNames.SoulBarrierStrength), CharacterClasses.GrandMaster, damage: 7, distance: 6, abilityConsumption: 24, manaConsumption: 77, levelRequirement: 77, energyRequirement: 126);
        this.CreateSkill(SkillNumber.SoulBarrierProficie, LocalizedString.FromResource(() => SkillNames.SoulBarrierProficie), CharacterClasses.GrandMaster, damage: 10, distance: 6, abilityConsumption: 26, manaConsumption: 84, levelRequirement: 77, energyRequirement: 126);
        this.CreateSkill(SkillNumber.MinimumWizardryInc, LocalizedString.FromResource(() => SkillNames.MinimumWizardryInc), CharacterClasses.GrandMaster | CharacterClasses.DuelMaster, damage: 22, skillType: SkillType.PassiveBoost);

        // High Elf:
        this.CreateSkill(SkillNumber.HealStrengthener, LocalizedString.FromResource(() => SkillNames.HealStrengthener), CharacterClasses.HighElf, DamageType.Physical, 22, 6, manaConsumption: 22, levelRequirement: 8, energyRequirement: 100);
        this.CreateSkill(SkillNumber.TripleShotStrengthener, LocalizedString.FromResource(() => SkillNames.TripleShotStrengthener), CharacterClasses.HighElf, DamageType.Physical, 22, 6, manaConsumption: 5);
        this.CreateSkill(SkillNumber.SummonedMonsterStr1, LocalizedString.FromResource(() => SkillNames.SummonedMonsterStr1), CharacterClasses.HighElf, damage: 16, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.PenetrationStrengthener, LocalizedString.FromResource(() => SkillNames.PenetrationStrengthener), CharacterClasses.HighElf, DamageType.Physical, 22, 6, 11, 10, 130, elementalModifier: ElementalType.Wind);
        this.CreateSkill(SkillNumber.DefenseIncreaseStr, LocalizedString.FromResource(() => SkillNames.DefenseIncreaseStr), CharacterClasses.HighElf, damage: 22, distance: 6, manaConsumption: 33, levelRequirement: 13, energyRequirement: 100);
        this.CreateSkill(SkillNumber.TripleShotMastery, LocalizedString.FromResource(() => SkillNames.TripleShotMastery), CharacterClasses.HighElf, DamageType.Physical, distance: 6, manaConsumption: 9);
        this.CreateSkill(SkillNumber.SummonedMonsterStr2, LocalizedString.FromResource(() => SkillNames.SummonedMonsterStr2), CharacterClasses.HighElf, damage: 16, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.AttackIncreaseStr, LocalizedString.FromResource(() => SkillNames.AttackIncreaseStr), CharacterClasses.HighElf, damage: 22, distance: 6, manaConsumption: 44, levelRequirement: 18, energyRequirement: 100);
        this.CreateSkill(SkillNumber.WeaponMasteryHighElf, LocalizedString.FromResource(() => SkillNames.WeaponMastery), CharacterClasses.HighElf, damage: 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.AttackIncreaseMastery, LocalizedString.FromResource(() => SkillNames.AttackIncreaseMastery), CharacterClasses.HighElf, damage: 22, distance: 6, manaConsumption: 48, levelRequirement: 18, energyRequirement: 100);
        this.CreateSkill(SkillNumber.DefenseIncreaseMastery, LocalizedString.FromResource(() => SkillNames.DefenseIncreaseMastery), CharacterClasses.HighElf, damage: 22, distance: 6, manaConsumption: 36, levelRequirement: 13, energyRequirement: 100);
        this.CreateSkill(SkillNumber.IceArrowStrengthener, LocalizedString.FromResource(() => SkillNames.IceArrowStrengthener), CharacterClasses.HighElf, DamageType.Physical, 22, 8, 18, 15, elementalModifier: ElementalType.Ice);
        this.CreateSkill(SkillNumber.BowStrengthener, LocalizedString.FromResource(() => SkillNames.BowStrengthener), CharacterClasses.HighElf, damage: 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.CrossbowStrengthener, LocalizedString.FromResource(() => SkillNames.CrossbowStrengthener), CharacterClasses.HighElf, damage: 3, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.ShieldStrengthenerHighElf, LocalizedString.FromResource(() => SkillNames.ShieldStrengthener), CharacterClasses.HighElf, damage: 10, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.BowMastery, LocalizedString.FromResource(() => SkillNames.BowMastery), CharacterClasses.HighElf, damage: 23, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.CrossbowMastery, LocalizedString.FromResource(() => SkillNames.CrossbowMastery), CharacterClasses.HighElf, damage: 5, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.ShieldMasteryHighElf, LocalizedString.FromResource(() => SkillNames.ShieldMastery), CharacterClasses.HighElf, damage: 15, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.InfinityArrowStr, LocalizedString.FromResource(() => SkillNames.InfinityArrowStr), CharacterClasses.HighElf, damage: 1, distance: 6, abilityConsumption: 11, manaConsumption: 55, levelRequirement: 220, skillType: SkillType.Buff, targetRestriction: SkillTargetRestriction.Self);
        this.CreateSkill(SkillNumber.MinimumAttPowerInc, LocalizedString.FromResource(() => SkillNames.MinimumAttPowerInc), CharacterClasses.HighElf, DamageType.Physical, 22, skillType: SkillType.PassiveBoost);

        // Dimension Master (Summoner):
        this.CreateSkill(SkillNumber.FireTomeStrengthener, LocalizedString.FromResource(() => SkillNames.FireTomeStrengthener), CharacterClasses.DimensionMaster, DamageType.Curse, 3, elementalModifier: ElementalType.Fire, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.WindTomeStrengthener, LocalizedString.FromResource(() => SkillNames.WindTomeStrengthener), CharacterClasses.DimensionMaster, DamageType.Curse, 3, elementalModifier: ElementalType.Wind, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.LightningTomeStren, LocalizedString.FromResource(() => SkillNames.LightningTomeStren), CharacterClasses.DimensionMaster, DamageType.Curse, 3, elementalModifier: ElementalType.Lightning, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.FireTomeMastery, LocalizedString.FromResource(() => SkillNames.FireTomeMastery), CharacterClasses.DimensionMaster, DamageType.Curse, 7, elementalModifier: ElementalType.Fire, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.WindTomeMastery, LocalizedString.FromResource(() => SkillNames.WindTomeMastery), CharacterClasses.DimensionMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.LightningTomeMastery, LocalizedString.FromResource(() => SkillNames.LightningTomeMastery), CharacterClasses.DimensionMaster, DamageType.Curse, 7, elementalModifier: ElementalType.Lightning, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.SleepStrengthener, LocalizedString.FromResource(() => SkillNames.SleepStrengthener), CharacterClasses.DimensionMaster, damage: 1, distance: 6, abilityConsumption: 7, manaConsumption: 30, levelRequirement: 40, energyRequirement: 100);
        this.CreateSkill(SkillNumber.ChainLightningStr, LocalizedString.FromResource(() => SkillNames.ChainLightningStr), CharacterClasses.DimensionMaster, DamageType.Wizardry, 22, 6, manaConsumption: 103, levelRequirement: 75, energyRequirement: 75, skillTarget: SkillTarget.Explicit, skillType: SkillType.AreaSkillExplicitTarget);
        this.CreateSkill(SkillNumber.LightningShockStr, LocalizedString.FromResource(() => SkillNames.LightningShockStr), CharacterClasses.DimensionMaster, DamageType.Wizardry, 22, 6, 10, 125, 93, 216, elementalModifier: ElementalType.Lightning);
        this.CreateSkill(SkillNumber.MagicMasterySummoner, LocalizedString.FromResource(() => SkillNames.MagicMastery), CharacterClasses.DimensionMaster, DamageType.Curse, 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DrainLifeStrengthener, LocalizedString.FromResource(() => SkillNames.DrainLifeStrengthener), CharacterClasses.DimensionMaster, DamageType.Wizardry, 22, 6, manaConsumption: 57, levelRequirement: 35, energyRequirement: 93);
        this.CreateSkill(SkillNumber.StickStrengthener, LocalizedString.FromResource(() => SkillNames.StickStrengthener), CharacterClasses.DimensionMaster, DamageType.Curse, 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.OtherWorldTomeStreng, LocalizedString.FromResource(() => SkillNames.OtherWorldTomeStreng), CharacterClasses.DimensionMaster, DamageType.Curse, 3, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.StickMastery, LocalizedString.FromResource(() => SkillNames.StickMastery), CharacterClasses.DimensionMaster, DamageType.Curse, 5, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.OtherWorldTomeMastery, LocalizedString.FromResource(() => SkillNames.OtherWorldTomeMastery), CharacterClasses.DimensionMaster, damage: 23, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.BerserkerStrengthener, LocalizedString.FromResource(() => SkillNames.BerserkerStrengthener), CharacterClasses.DimensionMaster, DamageType.Curse, 7, 5, 75, 150, 83, 181);
        this.CreateSkill(SkillNumber.BerserkerProficiency, LocalizedString.FromResource(() => SkillNames.BerserkerProficiency), CharacterClasses.DimensionMaster, DamageType.Curse, 7, 5, 82, 165, 83, 181);
        this.CreateSkill(SkillNumber.MinimumWizCurseInc, LocalizedString.FromResource(() => SkillNames.MinimumWizCurseInc), CharacterClasses.DimensionMaster, damage: 22, skillType: SkillType.PassiveBoost);

        // Duel Master (MG):
        this.CreateSkill(SkillNumber.CycloneStrengthenerDuelMaster, LocalizedString.FromResource(() => SkillNames.CycloneStrengthener), CharacterClasses.DuelMaster, DamageType.Physical, 22, 2, manaConsumption: 9);
        this.CreateSkill(SkillNumber.LightningStrengthenerDuelMaster, LocalizedString.FromResource(() => SkillNames.LightningStrengthener), CharacterClasses.DuelMaster, DamageType.Physical, 3, 6, manaConsumption: 20, levelRequirement: 13, energyRequirement: 100, elementalModifier: ElementalType.Lightning);
        this.CreateSkill(SkillNumber.TwistingSlashStrengthenerDuelMaster, LocalizedString.FromResource(() => SkillNames.TwistingSlashStren), CharacterClasses.DuelMaster, DamageType.Physical, 3, 2, 10, 10);
        this.CreateSkill(SkillNumber.PowerSlashStreng, LocalizedString.FromResource(() => SkillNames.PowerSlashStreng), CharacterClasses.DuelMaster, damage: 3, distance: 5, manaConsumption: 15, energyRequirement: 100);
        this.CreateSkill(SkillNumber.FlameStrengthenerDuelMaster, LocalizedString.FromResource(() => SkillNames.FlameStrengthener), CharacterClasses.DuelMaster, DamageType.Physical, 3, 6, manaConsumption: 55, levelRequirement: 35, energyRequirement: 100, elementalModifier: ElementalType.Fire);
        this.CreateSkill(SkillNumber.BlastStrengthenerDuelMaster, LocalizedString.FromResource(() => SkillNames.BlastStrengthener), CharacterClasses.DuelMaster, DamageType.Physical, 22, 3, manaConsumption: 165, levelRequirement: 80, energyRequirement: 150, elementalModifier: ElementalType.Lightning);
        this.CreateSkill(SkillNumber.WeaponMasteryDuelMaster, LocalizedString.FromResource(() => SkillNames.WeaponMastery), CharacterClasses.DuelMaster, damage: 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.InfernoStrengthenerDuelMaster, LocalizedString.FromResource(() => SkillNames.InfernoStrengthener), CharacterClasses.DuelMaster, DamageType.Physical, 22, manaConsumption: 220, levelRequirement: 88, energyRequirement: 200, elementalModifier: ElementalType.Fire);
        this.CreateSkill(SkillNumber.EvilSpiritStrengthenerDuelMaster, LocalizedString.FromResource(() => SkillNames.EvilSpiritStrengthen), CharacterClasses.DuelMaster, DamageType.Physical, 22, 6, manaConsumption: 108, levelRequirement: 50, energyRequirement: 100);
        this.CreateSkill(SkillNumber.MagicMasteryDuelMaster, LocalizedString.FromResource(() => SkillNames.MagicMastery), CharacterClasses.DuelMaster, damage: 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IceStrengthenerDuelMaster, LocalizedString.FromResource(() => SkillNames.IceStrengthener), CharacterClasses.DuelMaster, DamageType.Physical, 3, 6, manaConsumption: 42, levelRequirement: 25, energyRequirement: 100, elementalModifier: ElementalType.Ice);
        this.CreateSkill(SkillNumber.BloodAttackStrengthen, LocalizedString.FromResource(() => SkillNames.BloodAttackStrengthen), CharacterClasses.DuelMaster, damage: 22, distance: 3, abilityConsumption: 22, manaConsumption: 15, elementalModifier: ElementalType.Poison);

        // Lord Emperor (DL):
        this.CreateSkill(SkillNumber.FireBurstStreng, LocalizedString.FromResource(() => SkillNames.FireBurstStreng), CharacterClasses.LordEmperor, DamageType.Physical, 22, 6, manaConsumption: 25, levelRequirement: 74, energyRequirement: 20);
        this.CreateSkill(SkillNumber.ForceWaveStreng, LocalizedString.FromResource(() => SkillNames.ForceWaveStreng), CharacterClasses.LordEmperor, DamageType.Physical, 3, 4, manaConsumption: 15);
        this.CreateSkill(SkillNumber.DarkHorseStreng1, LocalizedString.FromResource(() => SkillNames.DarkHorseStreng1), CharacterClasses.LordEmperor, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.CriticalDmgIncPowUp, LocalizedString.FromResource(() => SkillNames.CriticalDMGIncPowUp), CharacterClasses.LordEmperor, damage: 3, abilityConsumption: 75, manaConsumption: 75, levelRequirement: 82, energyRequirement: 25, leadershipRequirement: 300);
        this.CreateSkill(SkillNumber.EarthshakeStreng, LocalizedString.FromResource(() => SkillNames.EarthshakeStreng), CharacterClasses.LordEmperor, DamageType.Physical, 22, 10, 75, elementalModifier: ElementalType.Lightning, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.WeaponMasteryLordEmperor, LocalizedString.FromResource(() => SkillNames.WeaponMastery), CharacterClasses.LordEmperor, damage: 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.FireBurstMastery, LocalizedString.FromResource(() => SkillNames.FireBurstMastery), CharacterClasses.LordEmperor, DamageType.Physical, 1, 6, manaConsumption: 27, levelRequirement: 74, energyRequirement: 20);
        this.CreateSkill(SkillNumber.CritDmgIncPowUp2, LocalizedString.FromResource(() => SkillNames.CritDMGIncPowUp2), CharacterClasses.LordEmperor, damage: 10, abilityConsumption: 82, manaConsumption: 82, levelRequirement: 82, energyRequirement: 25, leadershipRequirement: 300);
        this.CreateSkill(SkillNumber.EarthshakeMastery, LocalizedString.FromResource(() => SkillNames.EarthshakeMastery), CharacterClasses.LordEmperor, DamageType.Physical, 1, 10, 75, elementalModifier: ElementalType.Lightning, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.CritDmgIncPowUp3, LocalizedString.FromResource(() => SkillNames.CritDMGIncPowUp3), CharacterClasses.LordEmperor, damage: 7, abilityConsumption: 100, manaConsumption: 100, levelRequirement: 82, energyRequirement: 25, leadershipRequirement: 300);
        this.CreateSkill(SkillNumber.FireScreamStren, LocalizedString.FromResource(() => SkillNames.FireScreamStren), CharacterClasses.LordEmperor, DamageType.Physical, 22, 6, 11, 45, 102, 32, 70);
        this.CreateSkill(SkillNumber.DarkSpiritStr, LocalizedString.FromResource(() => SkillNames.DarkSpiritStr), CharacterClasses.LordEmperor, damage: 3, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.ScepterStrengthener, LocalizedString.FromResource(() => SkillNames.ScepterStrengthener), CharacterClasses.LordEmperor, DamageType.Physical, 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.ShieldStrengthenerLordEmperor, LocalizedString.FromResource(() => SkillNames.ShieldStrengthener), CharacterClasses.LordEmperor, damage: 10, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.UseScepterPetStr, LocalizedString.FromResource(() => SkillNames.UseScepterPetStr), CharacterClasses.LordEmperor, damage: 3, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DarkSpiritStr2, LocalizedString.FromResource(() => SkillNames.DarkSpiritStr2), CharacterClasses.LordEmperor, damage: 7, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.ScepterMastery, LocalizedString.FromResource(() => SkillNames.ScepterMastery), CharacterClasses.LordEmperor, damage: 5, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.ShieldMastery, LocalizedString.FromResource(() => SkillNames.ShieldMastery), CharacterClasses.LordEmperor, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.CommandAttackInc, LocalizedString.FromResource(() => SkillNames.CommandAttackInc), CharacterClasses.LordEmperor, damage: 20, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DarkSpiritStr3, LocalizedString.FromResource(() => SkillNames.DarkSpiritStr3), CharacterClasses.LordEmperor, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.PetDurabilityStr, LocalizedString.FromResource(() => SkillNames.PetDurabilityStr), CharacterClasses.LordEmperor, damage: 17, skillType: SkillType.PassiveBoost);

        // Fist Master (Rage Fighter):
        this.CreateSkill(SkillNumber.KillingBlowStrengthener, LocalizedString.FromResource(() => SkillNames.KillingBlowStrengthener), CharacterClasses.FistMaster, DamageType.Physical, 22, 2, manaConsumption: 10, elementalModifier: ElementalType.Earth, hitsPerAttack: 4);
        this.CreateSkill(SkillNumber.BeastUppercutStrengthener, LocalizedString.FromResource(() => SkillNames.BeastUppercutStrengthener), CharacterClasses.FistMaster, DamageType.Physical, 22, 2, manaConsumption: 10, elementalModifier: ElementalType.Fire, hitsPerAttack: 2);
        this.CreateSkill(SkillNumber.KillingBlowMastery, LocalizedString.FromResource(() => SkillNames.KillingBlowMastery), CharacterClasses.FistMaster, DamageType.Physical, 1, 2, manaConsumption: 10, elementalModifier: ElementalType.Earth, hitsPerAttack: 4);
        this.CreateSkill(SkillNumber.BeastUppercutMastery, LocalizedString.FromResource(() => SkillNames.BeastUppercutMastery), CharacterClasses.FistMaster, DamageType.Physical, 1, 2, manaConsumption: 10, elementalModifier: ElementalType.Fire, hitsPerAttack: 2);
        this.CreateSkill(SkillNumber.WeaponMasteryFistMaster, LocalizedString.FromResource(() => SkillNames.WeaponMastery), CharacterClasses.FistMaster, damage: 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.ChainDriveStrengthener, LocalizedString.FromResource(() => SkillNames.ChainDriveStrengthener), CharacterClasses.FistMaster, DamageType.Physical, 22, 4, 22, 22, 150, elementalModifier: ElementalType.Ice, hitsPerAttack: 4);
        this.CreateSkill(SkillNumber.DarkSideStrengthener, LocalizedString.FromResource(() => SkillNames.DarkSideStrengthener), CharacterClasses.FistMaster, DamageType.Physical, 22, 4, manaConsumption: 84, levelRequirement: 180, elementalModifier: ElementalType.Wind);
        this.CreateSkill(SkillNumber.DragonRoarStrengthener, LocalizedString.FromResource(() => SkillNames.DragonRoarStrengthener), CharacterClasses.FistMaster, DamageType.Physical, 22, 3, 33, 60, 150, elementalModifier: ElementalType.Earth, skillType: SkillType.AreaSkillExplicitTarget, hitsPerAttack: 4);
        this.CreateSkill(SkillNumber.EquippedWeaponStrengthener, LocalizedString.FromResource(() => SkillNames.EquippedWeaponStrengthener), CharacterClasses.FistMaster, damage: 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DefSuccessRateIncPowUp, LocalizedString.FromResource(() => SkillNames.DefSuccessRateIncPowUp), CharacterClasses.FistMaster, DamageType.Physical, 22, 7, 11, 55, 50, 30);
        this.CreateSkill(SkillNumber.EquippedWeaponMastery, LocalizedString.FromResource(() => SkillNames.EquippedWeaponMastery), CharacterClasses.FistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DefSuccessRateIncMastery, LocalizedString.FromResource(() => SkillNames.DefSuccessRateIncMastery), CharacterClasses.FistMaster, DamageType.Physical, 22, 7, 12, 60, 50, 30);
        this.CreateSkill(SkillNumber.StaminaIncreaseStrengthener, LocalizedString.FromResource(() => SkillNames.StaminaIncreaseStrengthener), CharacterClasses.FistMaster, DamageType.Physical, 5, 7, 11, 55, 80, 35);
        this.CreateSkill(SkillNumber.DurabilityReduction1FistMaster, LocalizedString.FromResource(() => SkillNames.DurabilityReduction1), CharacterClasses.FistMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreasePvPDefenseRate, LocalizedString.FromResource(() => SkillNames.IncreasePvPDefenseRate), CharacterClasses.FistMaster, damage: 29, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseMaximumSd, LocalizedString.FromResource(() => SkillNames.IncreaseMaximumSD), CharacterClasses.FistMaster, damage: 33, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseManaRecoveryRate, LocalizedString.FromResource(() => SkillNames.IncreaseManaRecoveryRate), CharacterClasses.FistMaster, damage: 7, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreasePoisonResistance, LocalizedString.FromResource(() => SkillNames.IncreasePoisonResistance), CharacterClasses.FistMaster, damage: 1, elementalModifier: ElementalType.Poison, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DurabilityReduction2FistMaster, LocalizedString.FromResource(() => SkillNames.DurabilityReduction2), CharacterClasses.FistMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseSdRecoveryRate, LocalizedString.FromResource(() => SkillNames.IncreaseSDRecoveryRate), CharacterClasses.FistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseHpRecoveryRate, LocalizedString.FromResource(() => SkillNames.IncreaseHPRecoveryRate), CharacterClasses.FistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseLightningResistance, LocalizedString.FromResource(() => SkillNames.IncreaseLightningResistance), CharacterClasses.FistMaster, damage: 1, elementalModifier: ElementalType.Lightning, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreasesDefense, LocalizedString.FromResource(() => SkillNames.IncreasesDefense), CharacterClasses.FistMaster, damage: 35, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreasesAgRecoveryRate, LocalizedString.FromResource(() => SkillNames.IncreasesAGRecoveryRate), CharacterClasses.FistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseIceResistance, LocalizedString.FromResource(() => SkillNames.IncreaseIceResistance), CharacterClasses.FistMaster, damage: 1, elementalModifier: ElementalType.Ice, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DurabilityReduction3FistMaster, LocalizedString.FromResource(() => SkillNames.DurabilityReduction3), CharacterClasses.FistMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseDefenseSuccessRate, LocalizedString.FromResource(() => SkillNames.IncreaseDefenseSuccessRate), CharacterClasses.FistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseAttackSuccessRate, LocalizedString.FromResource(() => SkillNames.IncreaseAttackSuccessRate), CharacterClasses.FistMaster, damage: 30, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseMaximumHp, LocalizedString.FromResource(() => SkillNames.IncreaseMaximumHP), CharacterClasses.FistMaster, damage: 34, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseMaximumMana, LocalizedString.FromResource(() => SkillNames.IncreaseMaximumMana), CharacterClasses.FistMaster, damage: 34, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreasePvPAttackRate, LocalizedString.FromResource(() => SkillNames.IncreasePvPAttackRate), CharacterClasses.FistMaster, damage: 31, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DecreaseMana, LocalizedString.FromResource(() => SkillNames.DecreaseMana), CharacterClasses.FistMaster, damage: 18, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.RecoverSDfromMonsterKills, LocalizedString.FromResource(() => SkillNames.RecoverSDFromMonsterKills), CharacterClasses.FistMaster, damage: 11, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.RecoverHPfromMonsterKills, LocalizedString.FromResource(() => SkillNames.RecoverHPFromMonsterKills), CharacterClasses.FistMaster, damage: 6, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseMinimumAttackPower, LocalizedString.FromResource(() => SkillNames.IncreaseMinimumAttackPower), CharacterClasses.FistMaster, damage: 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.RecoverManaMonsterKills, LocalizedString.FromResource(() => SkillNames.RecoverManaMonsterKills), CharacterClasses.FistMaster, damage: 6, skillType: SkillType.PassiveBoost);

        this.InitializeEffects();
        this.MapSkillsToEffects();
        this.InitializeMasterSkillData();
        MasterSkillPassivePowerUps.AddMissing(this.Context, this.GameConfiguration);
        this.CreateSpecialSummonMonsters();
        this.CreateSkillCombos();
        this.InitializeSkillAttributes();
    }

    // ReSharper disable once UnusedMember.Local
    private void InitializeNextSeasonMasterSkills()
    {
        // Common:
        this.CreateSkill(SkillNumber.CastInvincibility, "Cast Invincibility", CharacterClasses.AllMastersExceptFistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.ArmorSetBonusInc, "Armor Set Bonus Inc", CharacterClasses.AllMastersExceptFistMaster, damage: 3, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.Vengeance, "Vengeance", CharacterClasses.AllMastersExceptFistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.EnergyIncrease, "Energy Increase", CharacterClasses.AllMastersExceptFistMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.StaminaIncrease, "Stamina Increase", CharacterClasses.AllMastersExceptFistMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.AgilityIncrease, "Agility Increase", CharacterClasses.AllMastersExceptFistMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.StrengthIncrease, "Strength Increase", CharacterClasses.AllMastersExceptFistMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.SwellLifeMastery, "Swell Life Mastery", CharacterClasses.BladeMaster, damage: 7, abilityConsumption: 30, manaConsumption: 28, levelRequirement: 120);
        this.CreateSkill(SkillNumber.MaximumAttackPowerInc, "Maximum Attack Power Inc", CharacterClasses.BladeMaster | CharacterClasses.DuelMaster | CharacterClasses.LordEmperor, DamageType.Physical, 3, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.Inccritdamagerate, "Inc crit damage rate", CharacterClasses.AllMastersExceptFistMaster, damage: 7, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.RestoresallMana, "Restores all Mana", CharacterClasses.AllMastersExceptFistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.RestoresallHp, "Restores all HP", CharacterClasses.AllMastersExceptFistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.Incexcdamagerate, "Inc exc damage rate", CharacterClasses.AllMastersExceptFistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.Incdoubledamagerate, "Inc double damage rate", CharacterClasses.AllMastersExceptFistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncchanceofignoreDef, "Inc chance of ignore Def", CharacterClasses.AllMastersExceptFistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.RestoresallSd, "Restores all SD", CharacterClasses.AllMastersExceptFistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.Inctripledamagerate, "Inc triple damage rate", CharacterClasses.AllMastersExceptFistMaster, damage: 1, skillType: SkillType.PassiveBoost);

        // Blade Master:
        this.CreateSkill(SkillNumber.WingofStormAbsPowUp, "Wing of Storm Abs PowUp", CharacterClasses.BladeMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.WingofStormDefPowUp, "Wing of Storm Def PowUp", CharacterClasses.BladeMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IronDefense, "Iron Defense", CharacterClasses.AllMasters, damage: 1);
        this.CreateSkill(SkillNumber.WingofStormAttPowUp, "Wing of Storm Att PowUp", CharacterClasses.BladeMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DeathStabProficiency, "Death Stab Proficiency", CharacterClasses.BladeMaster, DamageType.Physical, 7, 2, 26, 30, 160, elementalModifier: ElementalType.Wind);
        this.CreateSkill(SkillNumber.StrikeofDestrProf, "Strike of Destr Prof", CharacterClasses.BladeMaster, DamageType.Physical, 7, 5, 24, 30, 100, elementalModifier: ElementalType.Ice);
        this.CreateSkill(SkillNumber.MaximumAgIncrease, "Maximum AG Increase", CharacterClasses.AllMastersExceptFistMaster, damage: 8, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DeathStabMastery, "Death Stab Mastery", CharacterClasses.BladeMaster, DamageType.Physical, 7, 2, 26, 30, 160, elementalModifier: ElementalType.Wind);
        this.CreateSkill(SkillNumber.StrikeofDestrMast, "Strike of Destr Mast", CharacterClasses.BladeMaster, DamageType.Physical, 1, 5, 24, 30, 100, elementalModifier: ElementalType.Ice);
        this.CreateSkill(SkillNumber.BloodStorm, "Blood Storm", CharacterClasses.BladeMaster | CharacterClasses.DuelMaster, DamageType.Physical, 25, 3, 29, 87);
        this.CreateSkill(SkillNumber.ComboStrengthener, "Combo Strengthener", CharacterClasses.BladeMaster, DamageType.Physical, 7, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.BloodStormStrengthener, "Blood Storm Strengthener", CharacterClasses.BladeMaster | CharacterClasses.DuelMaster, DamageType.Physical, 22, 3, 29, 87);

        // Grand Master:
        this.CreateSkill(SkillNumber.EternalWingsAbsPowUp, "Eternal Wings Abs PowUp", CharacterClasses.GrandMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.EternalWingsDefPowUp, "Eternal Wings Def PowUp", CharacterClasses.GrandMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.EternalWingsAttPowUp, "Eternal Wings Att PowUp", CharacterClasses.GrandMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.MeteorStrengthener, "Meteor Strengthener", CharacterClasses.GrandMaster, DamageType.Wizardry, 4, 6, manaConsumption: 13, levelRequirement: 21, energyRequirement: 100, elementalModifier: ElementalType.Earth);
        this.CreateSkill(SkillNumber.IceStormStrengthener, "Ice Storm Strengthener", CharacterClasses.GrandMaster, DamageType.Wizardry, 22, 6, 5, 110, 93, 223, elementalModifier: ElementalType.Ice);
        this.CreateSkill(SkillNumber.NovaStrengthener, "Nova Strengthener", CharacterClasses.GrandMaster, DamageType.Wizardry, 22, 6, 49, 198, 100, 258, elementalModifier: ElementalType.Fire);
        this.CreateSkill(SkillNumber.IceStormMastery, "Ice Storm Mastery", CharacterClasses.GrandMaster, DamageType.Wizardry, 1, 6, 5, 110, 93, 223, elementalModifier: ElementalType.Ice);
        this.CreateSkill(SkillNumber.MeteorMastery, "Meteor Mastery", CharacterClasses.GrandMaster, DamageType.Wizardry, 1, 6, manaConsumption: 14, levelRequirement: 21, energyRequirement: 100, elementalModifier: ElementalType.Earth);
        this.CreateSkill(SkillNumber.NovaCastStrengthener, "Nova Cast Strengthener", CharacterClasses.GrandMaster, DamageType.Wizardry, 22, 6, 49, 198, 100, 258, elementalModifier: ElementalType.Fire);
        this.CreateSkill(SkillNumber.SoulBarrierMastery, "Soul Barrier Mastery", CharacterClasses.GrandMaster, damage: 7, distance: 6, abilityConsumption: 28, manaConsumption: 92, levelRequirement: 77, energyRequirement: 126);
        this.CreateSkill(SkillNumber.MaximumWizardryInc, "Maximum Wizardry Inc", CharacterClasses.GrandMaster | CharacterClasses.DuelMaster, damage: 3, skillType: SkillType.PassiveBoost);

        // High Elf:
        this.CreateSkill(SkillNumber.IllusionWingsAbsPowUp, "Illusion Wings Abs PowUp", CharacterClasses.HighElf, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IllusionWingsDefPowUp, "Illusion Wings Def PowUp", CharacterClasses.HighElf, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.MultiShotStreng, "Multi-Shot Streng", CharacterClasses.HighElf, DamageType.Physical, 22, 6, 7, 11, 100, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.IllusionWingsAttPowUp, "Illusion Wings Att PowUp", CharacterClasses.HighElf, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.Cure, "Cure", CharacterClasses.HighElf, distance: 6, abilityConsumption: 10, manaConsumption: 72);
        this.CreateSkill(SkillNumber.PartyHealing, "Party Healing", CharacterClasses.HighElf, distance: 6, abilityConsumption: 12, manaConsumption: 66, energyRequirement: 100);
        this.CreateSkill(SkillNumber.PoisonArrow, "Poison Arrow", CharacterClasses.HighElf, DamageType.Physical, 27, 6, 27, 22, elementalModifier: ElementalType.Poison);
        this.CreateSkill(SkillNumber.SummonedMonsterStr3, "Summoned Monster Str (3)", CharacterClasses.HighElf, damage: 16, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.PartyHealingStr, "Party Healing Str", CharacterClasses.HighElf, damage: 22, distance: 6, abilityConsumption: 13, manaConsumption: 72, energyRequirement: 100);
        this.CreateSkill(SkillNumber.Bless, "Bless", CharacterClasses.HighElf, distance: 6, abilityConsumption: 18, manaConsumption: 108, energyRequirement: 100);
        this.CreateSkill(SkillNumber.MultiShotMastery, "Multi-Shot Mastery", CharacterClasses.HighElf, DamageType.Physical, 1, 6, 8, 12, 100, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.SummonSatyros, "Summon Satyros", CharacterClasses.HighElf, abilityConsumption: 52, manaConsumption: 525, energyRequirement: 280);
        this.CreateSkill(SkillNumber.BlessStrengthener, "Bless Strengthener", CharacterClasses.HighElf, damage: 10, distance: 6, abilityConsumption: 20, manaConsumption: 118, energyRequirement: 100);
        this.CreateSkill(SkillNumber.PoisonArrowStr, "Poison Arrow Str", CharacterClasses.HighElf, DamageType.Physical, 22, 6, 29, 24, elementalModifier: ElementalType.Poison);
        this.CreateSkill(SkillNumber.MaximumAttPowerInc, "Maximum Att Power Inc", CharacterClasses.HighElf, DamageType.Physical, 3, skillType: SkillType.PassiveBoost);

        // Dimension Master (Summoner):
        this.CreateSkill(SkillNumber.DimensionWingsAbsPowUp, "DimensionWings Abs PowUp", CharacterClasses.DimensionMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DimensionWingsDefPowUp, "DimensionWings Def PowUp", CharacterClasses.DimensionMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DimensionWingsAttPowUp, "DimensionWings Att PowUp", CharacterClasses.DimensionMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.WeaknessStrengthener, "Weakness Strengthener", CharacterClasses.DimensionMaster, DamageType.Curse, 3, 6, 17, 55, 93, 173);
        this.CreateSkill(SkillNumber.InnovationStrengthener, "Innovation Strengthener", CharacterClasses.DimensionMaster, DamageType.Curse, 3, 6, 17, 77, 111, 201);
        this.CreateSkill(SkillNumber.Blind, "Blind", CharacterClasses.DimensionMaster, DamageType.Curse, distance: 3, abilityConsumption: 25, manaConsumption: 115, energyRequirement: 201);
        this.CreateSkill(SkillNumber.DrainLifeMastery, "Drain Life Mastery", CharacterClasses.DimensionMaster, DamageType.Curse, 17, 6, manaConsumption: 62, levelRequirement: 35, energyRequirement: 93);
        this.CreateSkill(SkillNumber.BlindStrengthener, "Blind Strengthener", CharacterClasses.DimensionMaster, DamageType.Curse, 1, 3, 27, 126, energyRequirement: 201);
        this.CreateSkill(SkillNumber.BerserkerMastery, "Berserker Mastery", CharacterClasses.DimensionMaster, DamageType.Curse, 10, 5, 90, 181, 83, 181);
        this.CreateSkill(SkillNumber.MaximumWizCurseInc, "Maximum Wiz/Curse Inc", CharacterClasses.DimensionMaster, damage: 3, skillType: SkillType.PassiveBoost);

        // Duel Master (MG):
        this.CreateSkill(SkillNumber.WingofRuinAbsPowUp, "Wing of Ruin Abs PowUp", CharacterClasses.DuelMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.WingofRuinDefPowUp, "Wing of Ruin Def PowUp", CharacterClasses.DuelMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.WingofRuinAttPowUp, "Wing of Ruin Att PowUp", CharacterClasses.DuelMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IceMasteryDuelMaster, "Ice Mastery", CharacterClasses.DuelMaster, DamageType.Physical, 1, 6, manaConsumption: 46, levelRequirement: 25, energyRequirement: 100, elementalModifier: ElementalType.Ice);
        this.CreateSkill(SkillNumber.FlameStrikeStrengthen, "Flame Strike Strengthen", CharacterClasses.DuelMaster, DamageType.Physical, 22, 3, 37, 30, elementalModifier: ElementalType.Fire);
        this.CreateSkill(SkillNumber.FireSlashMastery, "Fire Slash Mastery", CharacterClasses.DuelMaster, damage: 7, distance: 3, abilityConsumption: 24, manaConsumption: 17, elementalModifier: ElementalType.Poison);
        this.CreateSkill(SkillNumber.FlameStrikeMastery, "Flame Strike Mastery", CharacterClasses.DuelMaster, DamageType.Physical, 7, 3, 40, 33, elementalModifier: ElementalType.Fire);
        this.CreateSkill(SkillNumber.EarthPrison, "Earth Prison", CharacterClasses.GrandMaster | CharacterClasses.DuelMaster, DamageType.Physical, 26, 3, 15, 180, energyRequirement: 127, elementalModifier: ElementalType.Earth);
        this.CreateSkill(SkillNumber.GiganticStormStr, "Gigantic Storm Str", CharacterClasses.DuelMaster, DamageType.Physical, 22, 6, 11, 132, 220, 118, elementalModifier: ElementalType.Wind);
        this.CreateSkill(SkillNumber.EarthPrisonStr, "Earth Prison Str", CharacterClasses.GrandMaster | CharacterClasses.DuelMaster, DamageType.Physical, 22, 3, 17, 198, energyRequirement: 127, elementalModifier: ElementalType.Earth);

        // Lord Emperor (DL):
        this.CreateSkill(SkillNumber.EmperorCapeAbsPowUp, "Emperor Cape Abs PowUp", CharacterClasses.LordEmperor, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.EmperorCapeDefPowUp, "Emperor Cape Def PowUp", CharacterClasses.LordEmperor, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.AddsCommandStat, "Adds Command Stat", CharacterClasses.LordEmperor, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.EmperorCapeAttPowUp, "Emperor Cape Att PowUp", CharacterClasses.LordEmperor, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.ElectricSparkStreng, "Electric Spark Streng", CharacterClasses.LordEmperor, DamageType.Physical, 3, 10, 150, levelRequirement: 92, energyRequirement: 29, leadershipRequirement: 340, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.FireScreamMastery, "Fire Scream Mastery", CharacterClasses.LordEmperor, DamageType.Physical, 5, 6, 12, 49, 102, 32, 70);
        this.CreateSkill(SkillNumber.IronDefenseLordEmperor, "Iron Defense", CharacterClasses.LordEmperor, damage: 28, abilityConsumption: 29, manaConsumption: 64);
        this.CreateSkill(SkillNumber.CriticalDamageIncM, "Critical Damage Inc M", CharacterClasses.LordEmperor, damage: 1, abilityConsumption: 110, manaConsumption: 110, levelRequirement: 82, energyRequirement: 25, leadershipRequirement: 300);
        this.CreateSkill(SkillNumber.ChaoticDiseierStr, "Chaotic Diseier Str", CharacterClasses.LordEmperor, DamageType.Physical, 22, 6, 22, 75, 100, 16, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.IronDefenseStr, "Iron Defense Str", CharacterClasses.LordEmperor, damage: 3, abilityConsumption: 31, manaConsumption: 70);
        this.CreateSkill(SkillNumber.DarkSpiritStr4, "Dark Spirit Str (4)", CharacterClasses.LordEmperor, damage: 23, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DarkSpiritStr5, "Dark Spirit Str (5)", CharacterClasses.LordEmperor, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.SpiritLord, "Spirit Lord", CharacterClasses.LordEmperor, damage: 1, skillType: SkillType.PassiveBoost);

        // Fist Master (Rage Fighter):
        this.CreateSkill(SkillNumber.CastInvincibilityFistMaster, "Cast Invincibility", CharacterClasses.FistMaster, damage: 38, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseMaximumAg, "Increase Maximum AG", CharacterClasses.FistMaster, damage: 37, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseMaximumAttackPower, "Increase Maximum Attack Power", CharacterClasses.FistMaster, damage: 3, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreasesCritDamageChance, "Increases Crit Damage Chance", CharacterClasses.FistMaster, damage: 38, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.RecoverManaFully, "Recover Mana Fully", CharacterClasses.FistMaster, damage: 38, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.RecoversHpFully, "Recovers HP Fully", CharacterClasses.FistMaster, damage: 38, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseExcDamageChance, "Increase Exc Damage Chance", CharacterClasses.FistMaster, damage: 38, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseDoubleDamageChance, "Increase Double Damage Chance", CharacterClasses.FistMaster, damage: 38, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseIgnoreDefChance, "Increase Ignore Def Chance", CharacterClasses.FistMaster, damage: 38, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.RecoversSdFully, "Recovers SD Fully", CharacterClasses.FistMaster, damage: 38, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseTripleDamageChance, "Increase Triple Damage Chance", CharacterClasses.FistMaster, damage: 38, skillType: SkillType.PassiveBoost);
    }

    private void CreateSkillCombos()
    {
        var bladeKnightCombo = this.Context.CreateNew<SkillComboDefinition>();
        var bladeKnight = this.GameConfiguration.CharacterClasses.First(c => c.Number == (byte)CharacterClassNumber.BladeKnight);
        bladeKnight.ComboDefinition = bladeKnightCombo;

        bladeKnightCombo.Name = LocalizedString.FromResource(() => SkillComboNames.BladeKnightCombo);

        this.AddComboStep(SkillNumber.Slash, 1, bladeKnightCombo);
        this.AddComboStep(SkillNumber.Cyclone, 1, bladeKnightCombo);
        this.AddComboStep(SkillNumber.Lunge, 1, bladeKnightCombo);
        this.AddComboStep(SkillNumber.FallingSlash, 1, bladeKnightCombo);
        this.AddComboStep(SkillNumber.Uppercut, 1, bladeKnightCombo);

        this.AddComboStep(SkillNumber.TwistingSlash, 2, bladeKnightCombo);
        this.AddComboStep(SkillNumber.RagefulBlow, 2, bladeKnightCombo);
        this.AddComboStep(SkillNumber.DeathStab, 2, bladeKnightCombo);
        this.AddComboStep(SkillNumber.StrikeofDestruction, 2, bladeKnightCombo);

        this.AddComboStep(SkillNumber.TwistingSlash, 3, bladeKnightCombo, true);
        this.AddComboStep(SkillNumber.RagefulBlow, 3, bladeKnightCombo, true);
        this.AddComboStep(SkillNumber.DeathStab, 3, bladeKnightCombo, true);
    }

    private void AddComboStep(SkillNumber skillNumber, int order, SkillComboDefinition comboDefinition, bool isFinal = false)
    {
        var skill = this.GameConfiguration.Skills.First(s => s.Number == (short)skillNumber);
        var step = this.Context.CreateNew<SkillComboStep>();
        comboDefinition.Steps.Add(step);
        comboDefinition.MaximumCompletionTime = TimeSpan.FromSeconds(3);
        step.Skill = skill;
        step.Order = order;
        step.IsFinalStep = isFinal;
    }

    private void InitializeSkillAttributes()
    {
        // Base damage
        this.AddAttributeRelationship(SkillNumber.Nova, Stats.SkillBaseDamageBonus, 1.0f / 2, Stats.TotalStrength);
        this.AddAttributeRelationship(SkillNumber.Nova, Stats.SkillBaseDamageBonus, 1, Stats.NovaStageDamage);

        this.AddAttributeRelationship(SkillNumber.Earthshake, Stats.SkillBaseDamageBonus, 1.0f / 10, Stats.TotalStrength);
        this.AddAttributeRelationship(SkillNumber.Earthshake, Stats.SkillBaseDamageBonus, 1.0f / 5, Stats.TotalLeadership);
        this.AddAttributeRelationship(SkillNumber.Earthshake, Stats.SkillBaseDamageBonus, 10, Stats.HorseLevel);

        this.AddAttributeRelationship(SkillNumber.ElectricSpike, Stats.SkillBaseDamageBonus, 50, Stats.NearbyPartyMemberCount);
        this.AddAttributeRelationship(SkillNumber.ElectricSpike, Stats.SkillBaseDamageBonus, 1.0f / 10, Stats.TotalLeadership);

        this.AddAttributeRelationship(SkillNumber.ChaoticDiseier, Stats.SkillBaseDamageBonus, 1.0f / 30, Stats.TotalStrength);
        this.AddAttributeRelationship(SkillNumber.ChaoticDiseier, Stats.SkillBaseDamageBonus, 1.0f / 55, Stats.TotalEnergy);

        SkillNumber[] lordSkills = [SkillNumber.Force, SkillNumber.FireBlast, SkillNumber.FireBurst, SkillNumber.ForceWave, SkillNumber.FireScream];
        foreach (var lordSkillNumber in lordSkills)
        {
            this.AddAttributeRelationship(lordSkillNumber, Stats.SkillBaseDamageBonus, 1.0f / 25, Stats.TotalStrength);
            this.AddAttributeRelationship(lordSkillNumber, Stats.SkillBaseDamageBonus, 1.0f / 50, Stats.TotalEnergy);
        }

        this.AddAttributeRelationship(SkillNumber.MultiShot, Stats.SkillBaseMultiplier, 0.8f, Stats.SkillMultiplier);

        // Final damage
        // Originally, starting weapon skills for DL (FallingSlash, Lunge, Uppercut, Cyclone) and RF (FallingSlash) have a constant multiplier of 2.
        // In OpenMU they might be able to do more damage with such skills, if their Stats.SkillMultiplier increases above 2.
        // Since this only applies to early game weapons, this is acceptable.
        this.AddAttributeRelationship(SkillNumber.FallingSlash, Stats.SkillFinalMultiplier, 2.0f, Stats.SkillMultiplier, InputOperator.Maximum); // For RF

        this.AddAttributeRelationship(SkillNumber.IceArrow, Stats.SkillFinalMultiplier, 2.0f, Stats.SkillMultiplier);
        this.AddAttributeRelationship(SkillNumber.Penetration, Stats.SkillFinalMultiplier, 2.0f, Stats.SkillMultiplier);
        this.AddAttributeRelationship(SkillNumber.Starfall, Stats.SkillFinalMultiplier, 2.0f, Stats.SkillMultiplier);

        // The attack skills of Selupan hit harder than its regular attack. The values are the damage
        // multipliers of the original server; Maximum keeps them absolute, regardless of the
        // Stats.SkillMultiplier of the monster.
        this.AddAttributeRelationship(SkillNumber.SelupanPoison, Stats.SkillFinalMultiplier, 2.0f, Stats.SkillMultiplier, InputOperator.Maximum);
        this.AddAttributeRelationship(SkillNumber.SelupanIceStorm, Stats.SkillFinalMultiplier, 2.2f, Stats.SkillMultiplier, InputOperator.Maximum);
        this.AddAttributeRelationship(SkillNumber.SelupanIceStrike, Stats.SkillFinalMultiplier, 2.3f, Stats.SkillMultiplier, InputOperator.Maximum);
        this.AddAttributeRelationship(SkillNumber.SelupanFall, Stats.SkillFinalMultiplier, 2.5f, Stats.SkillMultiplier, InputOperator.Maximum);

        this.AddAttributeRelationship(SkillNumber.Explosion223, Stats.SkillFinalDamageBonus, 1.0f, Stats.ExplosionBonusDmg);
        this.AddAttributeRelationship(SkillNumber.Requiem, Stats.SkillFinalDamageBonus, 1.0f, Stats.RequiemBonusDmg);
        this.AddAttributeRelationship(SkillNumber.Pollution, Stats.SkillFinalDamageBonus, 1.0f, Stats.PollutionBonusDmg);

        this.AddAttributeRelationship(SkillNumber.PlasmaStorm, Stats.SkillFinalMultiplier, 0.002f, Stats.TotalLevel);
        this.AddAttributeRelationship(SkillNumber.PlasmaStorm, Stats.SkillFinalMultiplier, -0.6f, Stats.MaximumHealth, InputOperator.Minimum); // 0.002 * 300(min lvl)
        this.AddAttributeRelationship(SkillNumber.PlasmaStorm, Stats.SkillFinalMultiplier, 2.0f, Stats.MaximumHealth, InputOperator.Minimum);

        this.AddAttributeRelationship(SkillNumber.ChaoticDiseier, Stats.SkillFinalMultiplier, 0.8f, Stats.SkillMultiplier);

        this.AddAttributeRelationship(SkillNumber.KillingBlow, Stats.SkillFinalMultiplier, 1.0f, Stats.VitalitySkillMultiplier);
        this.AddAttributeRelationship(SkillNumber.BeastUppercut, Stats.SkillFinalMultiplier, 1.0f, Stats.VitalitySkillMultiplier);
        this.AddAttributeRelationship(SkillNumber.ChainDrive, Stats.SkillFinalMultiplier, 1.0f, Stats.VitalitySkillMultiplier);
        this.AddAttributeRelationship(SkillNumber.Charge, Stats.SkillFinalMultiplier, 1.0f, Stats.VitalitySkillMultiplier);
        this.AddAttributeRelationship(SkillNumber.PhoenixShot, Stats.SkillFinalMultiplier, 1.0f, Stats.VitalitySkillMultiplier);

        this.AddAttributeRelationship(SkillNumber.DarkSide, Stats.SkillFinalMultiplier, 0.5f, Stats.SkillMultiplier, InputOperator.Add);
        this.AddAttributeRelationship(SkillNumber.DarkSide, Stats.SkillFinalMultiplier, 1.0f / 800, Stats.TotalAgility);

        this.AddAttributeRelationship(SkillNumber.DragonRoar, Stats.SkillFinalMultiplier, 1.0f, Stats.SkillMultiplier);
        this.AddAttributeRelationship(SkillNumber.DragonSlasher, Stats.SkillFinalMultiplier, 1.0f, Stats.SkillMultiplier);
        this.AddAttributeRelationship(SkillNumber.DragonSlasher, Stats.SkillFinalMultiplierPve, 3.0f, Stats.SkillMultiplier);

        // Other (mirror relationships)
        this.AddAttributeRelationship(SkillNumber.TripleShot, Stats.SkillExtraManaCost, 1, Stats.SkillExtraManaCost);
        this.AddAttributeRelationship(SkillNumber.IceArrow, Stats.SkillExtraManaCost, 1, Stats.SkillExtraManaCost);
        this.AddAttributeRelationship(SkillNumber.Penetration, Stats.SkillExtraManaCost, 1, Stats.SkillExtraManaCost);
        this.AddAttributeRelationship(SkillNumber.Starfall, Stats.SkillExtraManaCost, 1, Stats.SkillExtraManaCost);
        this.AddAttributeRelationship(SkillNumber.MultiShot, Stats.SkillExtraManaCost, 1, Stats.SkillExtraManaCost);
        this.AddAttributeRelationship(SkillNumber.RagefulBlowMastery, Stats.RagefulBlowMasteryDurabilityDecChance, 1, Stats.RagefulBlowMasteryDurabilityDecChance);
        this.AddAttributeRelationship(SkillNumber.SleepStrengthener, Stats.SleepStrBonusChance, 1, Stats.SleepStrBonusChance);
    }

    private void AddAttributeRelationship(SkillNumber skillNumber, AttributeDefinition targetAttribute, float multiplier, AttributeDefinition sourceAttribute, InputOperator inputOperator = InputOperator.Multiply, AggregateType aggregateType = AggregateType.AddRaw)
    {
        var skill = this.GameConfiguration.Skills.First(s => s.Number == (int)skillNumber);
        var relationship = CharacterClassHelper.CreateAttributeRelationship(this.Context, this.GameConfiguration, targetAttribute, multiplier, sourceAttribute, inputOperator, aggregateType);

        skill.AttributeRelationships.Add(relationship);
    }

    private void InitializeEffects()
    {
        new SoulBarrierEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new LifeSwellEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new CriticalDamageIncreaseEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new DefenseEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new GreaterDamageEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new GreaterDefenseEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new HealEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new ShieldRecoverEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new InfiniteArrowEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new DefenseReductionEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new InvisibleEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new IgnoreDefenseEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new IncreaseHealthEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new IncreaseBlockEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new WizardryEnhanceEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new AlcoholEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new SoulPotionEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new BlessPotionEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new BerserkerEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new WeaknessEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new SleepEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new WeaknessSummonerEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new InnovationEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new ReflectionEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new DefenseReductionBeastUppercutEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new DecreaseBlockEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new ExplosionEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new RequiemEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new StunEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new CriticalDamageIncreaseMasteryEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new LifeSwellProficiencyEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new WizardryEnhanceStrengthenerEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new WizardryEnhanceMasteryEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new IncreaseHealthStrengthenerEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new IncreaseBlockPowerUpEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new IncreaseBlockMasteryEffectInitializer(this.Context, this.GameConfiguration).Initialize();
    }

    private void MapSkillsToEffects()
    {
        foreach (var effectOfSkill in EffectsOfSkills)
        {
            var skill = this.GameConfiguration.Skills.First(s => s.Number == (short)effectOfSkill.Key);
            var effect = this.GameConfiguration.MagicEffects.First(e => e.Number == (short)effectOfSkill.Value);
            skill.MagicEffectDef = effect;

            // After the mapping, we override the internal effect number to the client's
            switch (effect.Number)
            {
                case (short)MagicEffectNumber.WeaknessSummoner:
                    effect.Number = (short)MagicEffectNumber.Weakness;
                    break;
                case (short)MagicEffectNumber.DefenseReductionBeastUppercut:
                    effect.Number = (short)MagicEffectNumber.DefenseReduction;
                    break;
                default:
                    // no change needed
                    break;
            }
        }
    }

    /// <summary>
    /// Initializes the master skill data.
    /// </summary>
    private void InitializeMasterSkillData()
    {
        // Roots:
        var leftRoot = this.Context.CreateNew<MasterSkillRoot>();
        leftRoot.Name = LocalizedString.FromResource(() => MasterSkillRootNames.LeftCommonSkills);
        this._masterSkillRoots.Add(1, leftRoot);
        this.GameConfiguration.MasterSkillRoots.Add(leftRoot);
        var middleRoot = this.Context.CreateNew<MasterSkillRoot>();
        middleRoot.Name = LocalizedString.FromResource(() => MasterSkillRootNames.MiddleRoot);
        this._masterSkillRoots.Add(2, middleRoot);
        this.GameConfiguration.MasterSkillRoots.Add(middleRoot);
        var rightRoot = this.Context.CreateNew<MasterSkillRoot>();
        rightRoot.Name = LocalizedString.FromResource(() => MasterSkillRootNames.RightRoot);
        this._masterSkillRoots.Add(3, rightRoot);
        this.GameConfiguration.MasterSkillRoots.Add(rightRoot);

        // Universal
        this.AddPassiveMasterSkillDefinition(SkillNumber.DurabilityReduction1, Stats.WeaponAndArmorDurationIncrease, AggregateType.AddRaw, $"{Formula1204} / 100", 1, 1);
        this.AddPassiveMasterSkillDefinition(SkillNumber.PvPDefenceRateInc, Stats.DefenseRatePvp, AggregateType.AddRaw, Formula61408, 1, 1);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MaximumSDincrease, Stats.MaximumShield, AggregateType.AddRaw, Formula51173, 2, 1);
        this.AddPassiveMasterSkillDefinition(SkillNumber.AutomaticManaRecInc, Stats.ManaRecoveryMultiplier, AggregateType.AddRaw, FormulaRecoveryIncrease181, Formula181, 2, 1, SkillNumber.Undefined, SkillNumber.Undefined, 20);
        this.AddPassiveMasterSkillDefinition(SkillNumber.PoisonResistanceInc, Stats.PoisonResistance, AggregateType.AddRaw, Formula120, Formula120, 2, 1);
        this.AddPassiveMasterSkillDefinition(SkillNumber.DurabilityReduction2, Stats.JewelryAndWingsDurationIncrease, AggregateType.AddRaw, $"{Formula1204} / 100", 3, 1, SkillNumber.DurabilityReduction1);
        this.AddPassiveMasterSkillDefinition(SkillNumber.SdRecoverySpeedInc, Stats.ShieldRecoveryMultiplier, AggregateType.Multiplicate, $"1 + {FormulaRecoveryIncrease120}", Formula120, 3, 1, SkillNumber.MaximumSDincrease, SkillNumber.Undefined, 20);
        this.AddPassiveMasterSkillDefinition(SkillNumber.AutomaticHpRecInc, Stats.HealthRecoveryMultiplier, AggregateType.AddRaw, FormulaRecoveryIncrease120, Formula120, 3, 1, SkillNumber.AutomaticManaRecInc, SkillNumber.Undefined, 20);
        this.AddPassiveMasterSkillDefinition(SkillNumber.LightningResistanceInc, Stats.LightningResistance, AggregateType.AddRaw, Formula120, Formula120, 2, 1, requiredSkill1: SkillNumber.PoisonResistanceInc);
        this.AddPassiveMasterSkillDefinition(SkillNumber.DefenseIncrease, Stats.DefenseBase, AggregateType.AddFinal, Formula6020, 4, 1);
        this.AddPassiveMasterSkillDefinition(SkillNumber.AutomaticAgRecInc, Stats.AbilityRecoveryMultiplier, AggregateType.AddRaw, FormulaRecoveryIncrease120, Formula120, 4, 1, SkillNumber.AutomaticHpRecInc, SkillNumber.Undefined, 20);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IceResistanceIncrease, Stats.IceResistance, AggregateType.AddRaw, Formula120, Formula120, 2, 1, requiredSkill1: SkillNumber.LightningResistanceInc);
        this.AddPassiveMasterSkillDefinition(SkillNumber.DurabilityReduction3, Stats.PetDurationIncrease, AggregateType.AddRaw, Formula1204, 5, 1, SkillNumber.DurabilityReduction2);
        this.AddPassiveMasterSkillDefinition(SkillNumber.DefenseSuccessRateInc, Stats.DefenseRatePvm, AggregateType.Multiplicate, $"1 + {Formula120} / 100", 5, 1, SkillNumber.DefenseIncrease);

        // DK
        this.AddPassiveMasterSkillDefinition(SkillNumber.AttackSuccRateInc, Stats.AttackRatePvm, AggregateType.AddRaw, Formula51173, 1, 2);
        this.AddMasterSkillDefinition(SkillNumber.CycloneStrengthener, SkillNumber.Cyclone, SkillNumber.Undefined, 2, 2, SkillNumber.Cyclone, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.SlashStrengthener, SkillNumber.Slash, SkillNumber.Undefined, 2, 2, SkillNumber.Slash, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.FallingSlashStreng, SkillNumber.FallingSlash, SkillNumber.Undefined, 2, 2, SkillNumber.FallingSlash, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.LungeStrengthener, SkillNumber.Lunge, SkillNumber.Undefined, 2, 2, SkillNumber.Lunge, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.TwistingSlashStreng, SkillNumber.TwistingSlash, SkillNumber.Undefined, 2, 3, SkillNumber.TwistingSlash, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.RagefulBlowStreng, SkillNumber.RagefulBlow, SkillNumber.Undefined, 2, 3, SkillNumber.RagefulBlow, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.TwistingSlashMastery, SkillNumber.TwistingSlashStreng, SkillNumber.Undefined, 2, 4, SkillNumber.TwistingSlashStreng, 20, $"{Formula120} / 100", Formula120, Stats.MasteryMoveTargetChance, AggregateType.AddRaw);
        this.AddMasterSkillDefinition(SkillNumber.RagefulBlowMastery, SkillNumber.RagefulBlowStreng, SkillNumber.Undefined, 2, 4, SkillNumber.RagefulBlowStreng, 20, $"{Formula120} / 100", Formula120, Stats.RagefulBlowMasteryDurabilityDecChance, AggregateType.AddRaw);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MaximumLifeIncrease, Stats.MaximumHealth, AggregateType.AddRaw, Formula10235, 4, 2);
        this.AddPassiveMasterSkillDefinition(SkillNumber.WeaponMasteryBladeMaster, Stats.MasterSkillPhysBonusDmg, AggregateType.AddRaw, Formula502, 4, 2);
        this.AddMasterSkillDefinition(SkillNumber.DeathStabStrengthener, SkillNumber.DeathStab, SkillNumber.Undefined, 2, 5, SkillNumber.DeathStab, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.StrikeofDestrStr, SkillNumber.StrikeofDestruction, SkillNumber.Undefined, 2, 5, SkillNumber.StrikeofDestruction, 20, Formula502);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MaximumManaIncrease, Stats.MaximumMana, AggregateType.AddRaw, Formula10235, 5, 2, SkillNumber.MaximumLifeIncrease);
        this.AddPassiveMasterSkillDefinition(SkillNumber.PvPAttackRate, Stats.AttackRatePvp, AggregateType.AddRaw, Formula81877, 1, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.TwoHandedSwordStrengthener, Stats.TwoHandedSwordStrBonusDamage, AggregateType.AddRaw, Formula883, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.OneHandedSwordStrengthener, Stats.OneHandedSwordBonusDamage, AggregateType.AddRaw, Formula502, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MaceStrengthener, Stats.MaceBonusDamage, AggregateType.AddRaw, Formula632, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.SpearStrengthener, Stats.SpearBonusDamage, AggregateType.AddRaw, Formula632, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.TwoHandedSwordMaster, Stats.TwoHandedSwordMasteryBonusDamage, AggregateType.AddRaw, Formula1154, 3, 3, SkillNumber.TwoHandedSwordStrengthener);
        this.AddPassiveMasterSkillDefinition(SkillNumber.OneHandedSwordMaster, Stats.WeaponMasteryAttackSpeed, AggregateType.AddRaw, Formula1, 3, 3, SkillNumber.OneHandedSwordStrengthener, SkillNumber.Undefined, 10);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MaceMastery, Stats.MasteryStunChance, AggregateType.AddRaw, $"{Formula120} / 100", 3, 3, SkillNumber.MaceStrengthener);
        this.AddPassiveMasterSkillDefinition(SkillNumber.SpearMastery, Stats.SpearMasteryDoubleDamageChance, AggregateType.AddRaw, $"{Formula120} / 100", 3, 3, SkillNumber.SpearStrengthener);
        this.AddMasterSkillDefinition(SkillNumber.SwellLifeStrengt, SkillNumber.SwellLife, SkillNumber.Undefined, 3, 4, SkillNumber.SwellLife, 20, $"{Formula181} / 100", Formula181, Stats.SwellLifeHealthIncrease, AggregateType.AddRaw);
        this.AddPassiveMasterSkillDefinition(SkillNumber.ManaReduction, Stats.ManaUsageReduction, AggregateType.AddRaw, Formula722Value, Formula722, 4, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MonsterAttackSdInc, Stats.ShieldAfterMonsterKillMultiplier, AggregateType.AddFinal, Formula914, 4, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MonsterAttackLifeInc, Stats.HealthAfterMonsterKillMultiplier, AggregateType.AddFinal, Formula4319, 4, 3);
        this.AddMasterSkillDefinition(SkillNumber.SwellLifeProficiency, SkillNumber.SwellLifeStrengt, SkillNumber.Undefined, 3, 5, SkillNumber.SwellLifeStrengt, 20, $"{Formula181} / 100", Formula181, Stats.SwellLifeManaIncrease, AggregateType.AddRaw);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MinimumAttackPowerInc, Stats.MinimumPhysBaseDmg, AggregateType.AddRaw, Formula502, 5, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MonsterAttackManaInc, Stats.ManaAfterMonsterKillMultiplier, AggregateType.AddFinal, Formula4319, 5, 3, SkillNumber.MonsterAttackLifeInc);

        // DW
        this.AddMasterSkillDefinition(SkillNumber.FlameStrengthener, SkillNumber.Flame, SkillNumber.Undefined, 2, 2, SkillNumber.Flame, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.LightningStrengthener, SkillNumber.Lightning, SkillNumber.Undefined, 2, 2, SkillNumber.Lightning, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.ExpansionofWizStreng, SkillNumber.ExpansionofWizardry, SkillNumber.Undefined, 2, 2, SkillNumber.ExpansionofWizardry, 20, Formula120Value, Formula120, Stats.MaximumWizBaseDmg, AggregateType.Multiplicate);
        this.AddMasterSkillDefinition(SkillNumber.InfernoStrengthener, SkillNumber.Inferno, SkillNumber.FlameStrengthener, 2, 3, SkillNumber.Inferno, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.BlastStrengthener, SkillNumber.Cometfall, SkillNumber.LightningStrengthener, 2, 3, SkillNumber.Cometfall, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.ExpansionofWizMas, SkillNumber.ExpansionofWizStreng, SkillNumber.Undefined, 2, 3, SkillNumber.ExpansionofWizStreng, 20, Formula120Value, Formula120, Stats.CriticalDamageChance, AggregateType.AddRaw);
        this.AddMasterSkillDefinition(SkillNumber.PoisonStrengthener, SkillNumber.Poison, SkillNumber.Undefined, 2, 3, SkillNumber.Poison, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.EvilSpiritStreng, SkillNumber.EvilSpirit, SkillNumber.Undefined, 2, 4, SkillNumber.EvilSpirit, 20, Formula502);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MagicMasteryGrandMaster, Stats.WizardryBaseDmg, AggregateType.AddRaw, Formula502, 4, 2, SkillNumber.EvilSpiritStreng);
        this.AddMasterSkillDefinition(SkillNumber.DecayStrengthener, SkillNumber.Decay, SkillNumber.PoisonStrengthener, 2, 4, SkillNumber.Decay, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.HellfireStrengthener, SkillNumber.Hellfire, SkillNumber.Undefined, 2, 5, SkillNumber.Hellfire, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.IceStrengthener, SkillNumber.Ice, SkillNumber.Undefined, 2, 5, SkillNumber.Ice, 20, Formula632);
        this.AddPassiveMasterSkillDefinition(SkillNumber.OneHandedStaffStrengthener, Stats.OneHandedStaffBonusBaseDamage, AggregateType.AddRaw, Formula502, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.TwoHandedStaffStrengthener, Stats.TwoHandedStaffBonusBaseDamage, AggregateType.AddRaw, Formula883, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.ShieldStrengthenerGrandMaster, Stats.BonusDefenseWithShield, AggregateType.AddRaw, Formula803, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.OneHandedStaffMaster, Stats.WeaponMasteryAttackSpeed, AggregateType.AddRaw, Formula1, 3, 3, SkillNumber.OneHandedStaffStrengthener, SkillNumber.Undefined, 10);
        this.AddPassiveMasterSkillDefinition(SkillNumber.TwoHandedStaffMaster, Stats.TwoHandedStaffMasteryBonusDamage, AggregateType.AddRaw, Formula1154, 3, 3, SkillNumber.TwoHandedStaffStrengthener);
        this.AddPassiveMasterSkillDefinition(SkillNumber.ShieldMasteryGrandMaster, Stats.BonusDefenseRateWithShield, AggregateType.AddRaw, Formula1204, 3, 3, SkillNumber.ShieldStrengthenerGrandMaster);
        this.AddMasterSkillDefinition(SkillNumber.SoulBarrierStrength, SkillNumber.SoulBarrier, SkillNumber.Undefined, 3, 4, SkillNumber.SoulBarrier, 20, $"{Formula181} / 100", Formula181, Stats.SoulBarrierReceiveDecrement, AggregateType.AddRaw);
        this.AddMasterSkillDefinition(SkillNumber.SoulBarrierProficie, SkillNumber.SoulBarrierStrength, SkillNumber.Undefined, 3, 5, SkillNumber.SoulBarrierStrength, 20, Formula803, true);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MinimumWizardryInc, Stats.MinimumWizBaseDmg, AggregateType.AddRaw, Formula502, 5, 3);

        // ELF
        this.AddMasterSkillDefinition(SkillNumber.HealStrengthener, SkillNumber.Heal, SkillNumber.Undefined, 2, 2, SkillNumber.Heal, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.TripleShotStrengthener, SkillNumber.TripleShot, SkillNumber.Undefined, 2, 2, SkillNumber.TripleShot, 20, Formula502);
        this.AddPassiveMasterSkillDefinition(SkillNumber.SummonedMonsterStr1, Stats.SummonedMonsterHealthIncrease, AggregateType.AddRaw, Formula6020Value, 2, 2, SkillNumber.SummonGoblin);
        this.AddMasterSkillDefinition(SkillNumber.PenetrationStrengthener, SkillNumber.Penetration, SkillNumber.Undefined, 2, 3, SkillNumber.Penetration, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.DefenseIncreaseStr, SkillNumber.GreaterDefense, SkillNumber.Undefined, 2, 3, SkillNumber.GreaterDefense, 20, $"{Formula502} / 100", Formula502, Stats.GreaterDefenseBonus, AggregateType.Multiplicate);
        this.AddMasterSkillDefinition(SkillNumber.TripleShotMastery, SkillNumber.TripleShotStrengthener, SkillNumber.Undefined, 2, 3, SkillNumber.TripleShotStrengthener, 10, Formula1WhenComplete, Formula1WhenComplete, Stats.ExtraProjectiles, AggregateType.AddRaw);
        this.AddPassiveMasterSkillDefinition(SkillNumber.SummonedMonsterStr2, Stats.SummonedMonsterDefenseIncrease, AggregateType.AddRaw, Formula6020Value, 3, 2, SkillNumber.SummonGoblin);
        this.AddMasterSkillDefinition(SkillNumber.AttackIncreaseStr, SkillNumber.GreaterDamage, SkillNumber.Undefined, 2, 4, SkillNumber.GreaterDamage, 20, $"{Formula502} / 100", Formula502, Stats.GreaterDamageBonus, AggregateType.Multiplicate);
        this.AddPassiveMasterSkillDefinition(SkillNumber.WeaponMasteryHighElf, Stats.MasterSkillPhysBonusDmg, AggregateType.AddRaw, Formula502, 4, 2);
        this.AddMasterSkillDefinition(SkillNumber.AttackIncreaseMastery, SkillNumber.AttackIncreaseStr, SkillNumber.Undefined, 2, 5, SkillNumber.AttackIncreaseStr, 20, $"{Formula502} / 100", Formula502, Stats.GreaterDamageBonus, AggregateType.Multiplicate, true);
        this.AddMasterSkillDefinition(SkillNumber.DefenseIncreaseMastery, SkillNumber.DefenseIncreaseStr, SkillNumber.Undefined, 2, 5, SkillNumber.DefenseIncreaseStr, 20, $"{Formula502} / 100", Formula502, Stats.GreaterDefenseBonus, AggregateType.Multiplicate, true);
        this.AddMasterSkillDefinition(SkillNumber.IceArrowStrengthener, SkillNumber.IceArrow, SkillNumber.Undefined, 2, 5, SkillNumber.IceArrow, 20, Formula502);
        this.AddPassiveMasterSkillDefinition(SkillNumber.BowStrengthener, Stats.BowStrBonusDamage, AggregateType.AddRaw, Formula502, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.CrossbowStrengthener, Stats.CrossBowStrBonusDamage, AggregateType.AddRaw, Formula632, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.ShieldStrengthenerHighElf, Stats.BonusDefenseWithShield, AggregateType.AddRaw, Formula803, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.BowMastery, Stats.WeaponMasteryAttackSpeed, AggregateType.AddRaw, Formula1, 3, 3, SkillNumber.BowStrengthener, SkillNumber.Undefined, 10);
        this.AddPassiveMasterSkillDefinition(SkillNumber.CrossbowMastery, Stats.CrossBowMasteryBonusDamage, AggregateType.AddRaw, Formula1154, 3, 3, SkillNumber.CrossbowStrengthener);
        this.AddPassiveMasterSkillDefinition(SkillNumber.ShieldMasteryHighElf, Stats.BonusDefenseRateWithShield, AggregateType.AddRaw, Formula1806, 3, 3, SkillNumber.ShieldStrengthenerHighElf);
        this.AddMasterSkillDefinition(SkillNumber.InfinityArrowStr, SkillNumber.InfinityArrow, SkillNumber.Undefined, 3, 5, SkillNumber.InfinityArrow, 20, $"{Formula120} / 100", Formula120, Stats.AttackDamageIncrease, AggregateType.Multiplicate);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MinimumAttPowerInc, Stats.MinimumPhysBaseDmg, AggregateType.AddRaw, Formula502, 5, 3);

        // SUM
        this.AddPassiveMasterSkillDefinition(SkillNumber.FireTomeStrengthener, Stats.ExplosionBonusDmg, AggregateType.AddRaw, Formula632, 2, 2);
        this.AddPassiveMasterSkillDefinition(SkillNumber.WindTomeStrengthener, Stats.RequiemBonusDmg, AggregateType.AddRaw, Formula632, 2, 2);
        this.AddPassiveMasterSkillDefinition(SkillNumber.LightningTomeStren, Stats.PollutionBonusDmg, AggregateType.AddRaw, Formula632, 2, 2);
        this.AddPassiveMasterSkillDefinition(SkillNumber.FireTomeMastery, Stats.BleedingDamageMultiplier, AggregateType.AddRaw, $"{Formula181} / 100", 3, 2, SkillNumber.FireTomeStrengthener);
        this.AddPassiveMasterSkillDefinition(SkillNumber.WindTomeMastery, Stats.MasteryStunChance, AggregateType.AddRaw, $"{Formula120} / 100", 3, 2, SkillNumber.WindTomeStrengthener);
        this.AddPassiveMasterSkillDefinition(SkillNumber.LightningTomeMastery, Stats.MasteryMoveTargetChance, AggregateType.AddRaw, $"{Formula181} / 100", 3, 2, SkillNumber.LightningTomeStren);
        this.AddMasterSkillDefinition(SkillNumber.SleepStrengthener, SkillNumber.Sleep, SkillNumber.Undefined, 2, 3, SkillNumber.Sleep, 20, Formula120Value, Formula120, Stats.SleepStrBonusChance, AggregateType.AddRaw);
        this.AddMasterSkillDefinition(SkillNumber.ChainLightningStr, SkillNumber.ChainLightning, SkillNumber.Undefined, 2, 4, SkillNumber.ChainLightning, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.LightningShockStr, SkillNumber.LightningShock, SkillNumber.Undefined, 2, 4, SkillNumber.LightningShock, 20, Formula502);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MagicMasterySummoner, Stats.WizardryAndCurseBaseDmgBonus, AggregateType.AddRaw, Formula502, 5, 2);
        this.AddMasterSkillDefinition(SkillNumber.DrainLifeStrengthener, SkillNumber.DrainLife, SkillNumber.Undefined, 2, 5, SkillNumber.DrainLife, 20, Formula502, Formula502, Stats.DrainLifeStrBonusHealing, AggregateType.AddRaw);
        this.AddPassiveMasterSkillDefinition(SkillNumber.StickStrengthener, Stats.StickBonusBaseDamage, AggregateType.AddRaw, Formula502, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.OtherWorldTomeStreng, Stats.BookBonusBaseDamage, AggregateType.AddRaw, Formula632, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.StickMastery, Stats.StickMasteryBonusDamage, AggregateType.AddRaw, Formula1154, 3, 3, SkillNumber.StickStrengthener);
        this.AddPassiveMasterSkillDefinition(SkillNumber.OtherWorldTomeMastery, Stats.WeaponMasteryAttackSpeed, AggregateType.AddRaw, Formula1, 3, 3, SkillNumber.OtherWorldTomeStreng, SkillNumber.Undefined, 10);
        this.AddMasterSkillDefinition(SkillNumber.BerserkerStrengthener, SkillNumber.Berserker, SkillNumber.Undefined, 3, 4, SkillNumber.Berserker, 20, $"{Formula181} / 100", Formula181, Stats.BerserkerCurseMultiplier, AggregateType.AddRaw);
        this.AddMasterSkillDefinition(SkillNumber.BerserkerProficiency, SkillNumber.BerserkerStrengthener, SkillNumber.Undefined, 3, 5, SkillNumber.BerserkerStrengthener, 20, $"{Formula181} / 100", Formula181, Stats.BerserkerProficiencyMultiplier, AggregateType.AddRaw);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MinimumWizCurseInc, Stats.MinWizardryAndCurseDmgBonus, AggregateType.AddRaw, Formula502, 5, 3);

        // MG
        this.AddMasterSkillDefinition(SkillNumber.CycloneStrengthenerDuelMaster, SkillNumber.Cyclone, SkillNumber.Undefined, 2, 2, SkillNumber.Cyclone, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.LightningStrengthenerDuelMaster, SkillNumber.Lightning, SkillNumber.Undefined, 2, 2, SkillNumber.Lightning, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.TwistingSlashStrengthenerDuelMaster, SkillNumber.TwistingSlash, SkillNumber.Undefined, 2, 2, SkillNumber.TwistingSlash, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.PowerSlashStreng, SkillNumber.PowerSlash, SkillNumber.Undefined, 2, 2, SkillNumber.PowerSlash, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.FlameStrengthenerDuelMaster, SkillNumber.Flame, SkillNumber.Undefined, 2, 3, SkillNumber.Flame, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.BlastStrengthenerDuelMaster, SkillNumber.Cometfall, SkillNumber.LightningStrengthenerDuelMaster, 2, 3, SkillNumber.Cometfall, 20, Formula502);
        this.AddPassiveMasterSkillDefinition(SkillNumber.WeaponMasteryDuelMaster, Stats.MasterSkillPhysBonusDmg, AggregateType.AddRaw, Formula502, 3, 2, SkillNumber.TwistingSlashStrengthenerDuelMaster, SkillNumber.PowerSlashStreng);
        this.AddMasterSkillDefinition(SkillNumber.InfernoStrengthenerDuelMaster, SkillNumber.Inferno, SkillNumber.FlameStrengthenerDuelMaster, 2, 4, SkillNumber.Inferno, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.EvilSpiritStrengthenerDuelMaster, SkillNumber.EvilSpirit, SkillNumber.Undefined, 2, 4, SkillNumber.EvilSpirit, 20, Formula502);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MagicMasteryDuelMaster, Stats.WizardryBaseDmg, AggregateType.AddRaw, Formula502, 4, 2, SkillNumber.EvilSpiritStrengthenerDuelMaster);
        this.AddMasterSkillDefinition(SkillNumber.IceStrengthenerDuelMaster, SkillNumber.Ice, SkillNumber.Undefined, 2, 5, SkillNumber.Ice, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.BloodAttackStrengthen, SkillNumber.FireSlash, SkillNumber.Undefined, 2, 5, SkillNumber.FireSlash, 20, Formula502);

        // DL
        this.AddMasterSkillDefinition(SkillNumber.FireBurstStreng, SkillNumber.FireBurst, SkillNumber.Undefined, 2, 2, SkillNumber.FireBurst, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.ForceWaveStreng, SkillNumber.Force, SkillNumber.Undefined, 2, 2, SkillNumber.ForceWave, 20, Formula632);
        this.AddPassiveMasterSkillDefinition(SkillNumber.DarkHorseStreng1, Stats.BonusDefenseWithHorse, AggregateType.AddRaw, Formula1204, 2, 2);
        this.AddMasterSkillDefinition(SkillNumber.CriticalDmgIncPowUp, SkillNumber.IncreaseCriticalDamage, SkillNumber.Undefined, 2, 3, SkillNumber.IncreaseCriticalDamage, 20, Formula632, Formula632, Stats.CriticalDamageBonus, AggregateType.AddRaw);
        this.AddMasterSkillDefinition(SkillNumber.EarthshakeStreng, SkillNumber.Earthshake, SkillNumber.DarkHorseStreng1, 2, 3, SkillNumber.Earthshake, 20, Formula502);
        this.AddPassiveMasterSkillDefinition(SkillNumber.WeaponMasteryLordEmperor, Stats.MasterSkillPhysBonusDmg, AggregateType.AddRaw, Formula502, 3, 2);
        this.AddMasterSkillDefinition(SkillNumber.FireBurstMastery, SkillNumber.FireBurstStreng, SkillNumber.Undefined, 2, 4, SkillNumber.FireBurstStreng, 20, $"{Formula120} / 100", Formula120, Stats.MasteryStunChance, AggregateType.AddRaw);
        this.AddMasterSkillDefinition(SkillNumber.CritDmgIncPowUp2, SkillNumber.CriticalDmgIncPowUp, SkillNumber.Undefined, 2, 4, SkillNumber.CriticalDmgIncPowUp, 20, Formula803, true);
        this.AddMasterSkillDefinition(SkillNumber.EarthshakeMastery, SkillNumber.EarthshakeStreng, SkillNumber.Undefined, 2, 4, SkillNumber.EarthshakeStreng, 20, $"{Formula120} / 100", Formula120, Stats.MasteryStunChance, AggregateType.AddRaw);
        this.AddMasterSkillDefinition(SkillNumber.CritDmgIncPowUp3, SkillNumber.CritDmgIncPowUp2, SkillNumber.Undefined, 2, 5, SkillNumber.CritDmgIncPowUp2, 20, $"{Formula181} / 100", Formula181, Stats.CriticalDamageChance, AggregateType.AddRaw);
        this.AddMasterSkillDefinition(SkillNumber.FireScreamStren, SkillNumber.FireScream, SkillNumber.Undefined, 2, 5, SkillNumber.FireScream, 20, Formula502);
        this.AddPassiveMasterSkillDefinition(SkillNumber.DarkSpiritStr, Stats.RavenBonusDamage, AggregateType.AddRaw, Formula632, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.ScepterStrengthener, Stats.ScepterStrBonusDamage, AggregateType.AddRaw, Formula502, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.ShieldStrengthenerLordEmperor, Stats.BonusDefenseWithShield, AggregateType.AddRaw, Formula803, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.UseScepterPetStr, Stats.ScepterPetBonusDamage, AggregateType.AddRaw, Formula632, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.DarkSpiritStr2, Stats.RavenCriticalDamageChance, AggregateType.AddRaw, $"{Formula181} / 100", Formula181, 3, 3, SkillNumber.DarkSpiritStr);
        this.AddPassiveMasterSkillDefinition(SkillNumber.ScepterMastery, Stats.ScepterMasteryBonusDamage, AggregateType.AddRaw, Formula1154, 3, 3, SkillNumber.ScepterStrengthener);
        this.AddPassiveMasterSkillDefinition(SkillNumber.ShieldMastery, Stats.BonusDefenseRateWithShield, AggregateType.AddRaw, Formula1204, 3, 3, SkillNumber.ShieldStrengthenerLordEmperor);
        this.AddPassiveMasterSkillDefinition(SkillNumber.CommandAttackInc, Stats.BonusDamageWithScepterCmdDiv, AggregateType.AddRaw, $"1 / ({Formula3822})", Formula3822, 3, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.DarkSpiritStr3, Stats.RavenExcDamageChance, AggregateType.AddRaw, $"{Formula120} / 100", Formula120, 5, 3, SkillNumber.DarkSpiritStr2);
        this.AddPassiveMasterSkillDefinition(SkillNumber.PetDurabilityStr, Stats.TrainablePetDurationIncrease, AggregateType.AddRaw, Formula1204, 5, 3);

        // RF
        this.AddPassiveMasterSkillDefinition(SkillNumber.DurabilityReduction1FistMaster, Stats.WeaponAndArmorDurationIncrease, AggregateType.AddRaw, $"{Formula1204} / 100", 1, 1);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreasePvPDefenseRate, Stats.DefenseRatePvp, AggregateType.AddRaw, Formula25587, 1, 1);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseMaximumSd, Stats.MaximumShield, AggregateType.AddRaw, Formula30704, 2, 1);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseManaRecoveryRate, Stats.ManaRecoveryMultiplier, AggregateType.AddRaw, FormulaRecoveryIncrease181, Formula181, 2, 1, SkillNumber.Undefined, SkillNumber.Undefined, 20);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreasePoisonResistance, Stats.PoisonResistance, AggregateType.AddRaw, Formula120Value, Formula120, 2, 1);
        this.AddPassiveMasterSkillDefinition(SkillNumber.DurabilityReduction2FistMaster, Stats.JewelryAndWingsDurationIncrease, AggregateType.AddRaw, $"{Formula1204} / 100", 3, 1, SkillNumber.DurabilityReduction1FistMaster);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseSdRecoveryRate, Stats.ShieldRecoveryMultiplier, AggregateType.Multiplicate, $"1 + {FormulaRecoveryIncrease120}", Formula120, 3, 1, SkillNumber.IncreaseMaximumSd, SkillNumber.Undefined, 20);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseHpRecoveryRate, Stats.HealthRecoveryMultiplier, AggregateType.AddRaw, FormulaRecoveryIncrease120, Formula120, 3, 1, SkillNumber.IncreaseManaRecoveryRate, SkillNumber.Undefined, 20);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseLightningResistance, Stats.LightningResistance, AggregateType.AddRaw, Formula120Value, Formula120, 3, 1, requiredSkill1: SkillNumber.IncreasePoisonResistance);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreasesDefense, Stats.DefenseBase, AggregateType.AddFinal, Formula3371, 4, 1);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreasesAgRecoveryRate, Stats.AbilityRecoveryMultiplier, AggregateType.AddRaw, FormulaRecoveryIncrease120, Formula120, 4, 1, SkillNumber.IncreaseHpRecoveryRate, SkillNumber.Undefined, 20);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseIceResistance, Stats.IceResistance, AggregateType.AddRaw, Formula120Value, Formula120, 4, 1, requiredSkill1: SkillNumber.IncreaseLightningResistance);
        this.AddPassiveMasterSkillDefinition(SkillNumber.DurabilityReduction3FistMaster, Stats.PetDurationIncrease, AggregateType.AddRaw, Formula1204, 5, 1, SkillNumber.DurabilityReduction2FistMaster);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseDefenseSuccessRate, Stats.DefenseRatePvm, AggregateType.Multiplicate, FormulaIncreaseMultiplicator120, Formula120, 5, 1, SkillNumber.IncreasesDefense, SkillNumber.Undefined, 20);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseAttackSuccessRate, Stats.AttackRatePvm, AggregateType.AddRaw, Formula20469, 1, 2);
        this.AddMasterSkillDefinition(SkillNumber.KillingBlowStrengthener, SkillNumber.Undefined, SkillNumber.Undefined, 2, 2, SkillNumber.KillingBlow, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.BeastUppercutStrengthener, SkillNumber.Undefined, SkillNumber.Undefined, 2, 2, SkillNumber.BeastUppercut, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.KillingBlowMastery, SkillNumber.KillingBlowStrengthener, SkillNumber.Undefined, 2, 3, SkillNumber.KillingBlowStrengthener, 20, Formula120Value, Formula120, Stats.WeaknessPhysDmgDecrement, AggregateType.AddRaw);
        this.AddMasterSkillDefinition(SkillNumber.BeastUppercutMastery, SkillNumber.BeastUppercutStrengthener, SkillNumber.Undefined, 2, 3, SkillNumber.BeastUppercutStrengthener, 20, $"-1 * {Formula120Value}", Formula120, Stats.DefenseDecrement, AggregateType.Multiplicate);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseMaximumHp, Stats.MaximumHealth, AggregateType.AddRaw, Formula5418, 4, 2);
        this.AddPassiveMasterSkillDefinition(SkillNumber.WeaponMasteryFistMaster, Stats.MasterSkillPhysBonusDmg, AggregateType.AddRaw, Formula502, 4, 2);
        this.AddMasterSkillDefinition(SkillNumber.ChainDriveStrengthener, SkillNumber.ChainDrive, SkillNumber.Undefined, 2, 5, SkillNumber.ChainDrive, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.DarkSideStrengthener, SkillNumber.DarkSide, SkillNumber.Undefined, 2, 5, SkillNumber.DarkSide, 20, Formula502);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseMaximumMana, Stats.MaximumMana, AggregateType.AddRaw, Formula5418, 5, 2, SkillNumber.IncreaseMaximumHp);
        this.AddMasterSkillDefinition(SkillNumber.DragonRoarStrengthener, SkillNumber.DragonRoar, SkillNumber.Undefined, 2, 5, SkillNumber.DragonRoar, 20, Formula502);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreasePvPAttackRate, Stats.AttackRatePvp, AggregateType.AddRaw, Formula32751, 1, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.EquippedWeaponStrengthener, Stats.GloveWeaponBonusDamage, AggregateType.AddRaw, Formula502, 2, 3);
        this.AddMasterSkillDefinition(SkillNumber.DefSuccessRateIncPowUp, SkillNumber.IncreaseBlock, SkillNumber.Undefined, 3, 2, SkillNumber.IncreaseBlock, 20, $"{Formula502} / 100", Formula502, Stats.IncreaseBlockBonus, AggregateType.AddRaw);
        this.AddPassiveMasterSkillDefinition(SkillNumber.EquippedWeaponMastery, Stats.GloveWeaponMasteryDoubleDamageChance, AggregateType.AddRaw, Formula120Value, Formula120, 3, 3, SkillNumber.EquippedWeaponStrengthener);
        this.AddMasterSkillDefinition(SkillNumber.DefSuccessRateIncMastery, SkillNumber.DefSuccessRateIncPowUp, SkillNumber.Undefined, 3, 3, SkillNumber.DefSuccessRateIncPowUp, 20, Formula502, Formula502, Stats.DefenseFinal, AggregateType.AddFinal);
        this.AddMasterSkillDefinition(SkillNumber.StaminaIncreaseStrengthener, SkillNumber.IncreaseHealth, SkillNumber.Undefined, 3, 4, SkillNumber.IncreaseHealth, 20, Formula1154, Formula1154, Stats.TotalVitality, AggregateType.AddFinal);
        this.AddPassiveMasterSkillDefinition(SkillNumber.DecreaseMana, Stats.ManaUsageReduction, AggregateType.AddRaw, Formula722Value, Formula722, 4, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.RecoverSDfromMonsterKills, Stats.ShieldAfterMonsterKillMultiplier, AggregateType.AddFinal, Formula914, 4, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.RecoverHPfromMonsterKills, Stats.HealthAfterMonsterKillMultiplier, AggregateType.AddFinal, Formula4319, 4, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseMinimumAttackPower, Stats.MinimumPhysBaseDmg, AggregateType.AddRaw, Formula502, 5, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.RecoverManaMonsterKills, Stats.ManaAfterMonsterKillMultiplier, AggregateType.AddFinal, Formula4319, 5, 3, SkillNumber.RecoverHPfromMonsterKills);
    }

    private void AddPassiveMasterSkillDefinition(SkillNumber skillNumber, AttributeDefinition targetAttribute, AggregateType aggregateType, string valueFormula, string displayValueFormula, byte rank, byte root, SkillNumber requiredSkill1 = SkillNumber.Undefined, SkillNumber requiredSkill2 = SkillNumber.Undefined, byte maximumLevel = 20)
    {
        this.AddMasterSkillDefinition(skillNumber, requiredSkill1, requiredSkill2, root, rank, SkillNumber.Undefined, maximumLevel, valueFormula, displayValueFormula, targetAttribute, aggregateType);
    }

    private void AddPassiveMasterSkillDefinition(SkillNumber skillNumber, AttributeDefinition targetAttribute, AggregateType aggregateType, string valueFormula, byte rank, byte root, SkillNumber requiredSkill1 = SkillNumber.Undefined, SkillNumber requiredSkill2 = SkillNumber.Undefined, byte maximumLevel = 20)
    {
        this.AddMasterSkillDefinition(skillNumber, requiredSkill1, requiredSkill2, root, rank, SkillNumber.Undefined, maximumLevel, valueFormula, valueFormula, targetAttribute, aggregateType);
    }

    private void AddMasterSkillDefinition(SkillNumber skillNumber, SkillNumber requiredSkill1, SkillNumber requiredSkill2, byte root, byte rank, SkillNumber regularSkill, byte maximumLevel, string valueFormula, bool extendsDuration = false)
    {
        this.AddMasterSkillDefinition(skillNumber, requiredSkill1, requiredSkill2, root, rank, regularSkill, maximumLevel, valueFormula, valueFormula, null, AggregateType.AddRaw, extendsDuration);
    }

    private void AddMasterSkillDefinition(SkillNumber skillNumber, SkillNumber requiredSkill1, SkillNumber requiredSkill2, byte root, byte rank, SkillNumber regularSkill, byte maximumLevel, string valueFormula, string displayValueFormula, AttributeDefinition? targetAttribute, AggregateType aggregateType, bool extendsDuration = false)
    {
        var skill = this.GameConfiguration.Skills.First(s => s.Number == (short)skillNumber);
        skill.MasterDefinition = this.Context.CreateNew<MasterSkillDefinition>();
        skill.MasterDefinition.Rank = rank;
        skill.MasterDefinition.Root = this._masterSkillRoots[root];
        skill.MasterDefinition.ValueFormula = valueFormula;
        skill.MasterDefinition.DisplayValueFormula = displayValueFormula;
        skill.MasterDefinition.MaximumLevel = maximumLevel;
        skill.MasterDefinition.TargetAttribute = targetAttribute?.GetPersistent(this.GameConfiguration);
        skill.MasterDefinition.Aggregation = aggregateType;
        skill.MasterDefinition.ReplacedSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)regularSkill);
        skill.MasterDefinition.ExtendsDuration = extendsDuration;
        if (requiredSkill1 != SkillNumber.Undefined)
        {
            skill.MasterDefinition.RequiredMasterSkills.Add(this.GameConfiguration.Skills.First(s => s.Number == (short)requiredSkill1));
        }

        if (requiredSkill2 != SkillNumber.Undefined)
        {
            skill.MasterDefinition.RequiredMasterSkills.Add(this.GameConfiguration.Skills.First(s => s.Number == (short)requiredSkill2));
        }

        if (maximumLevel == 10 && valueFormula == Formula1WhenComplete)
        {
            skill.MasterDefinition.MinimumLevel = maximumLevel;
        }
        else
        {
            skill.MasterDefinition.MinimumLevel = 1;
        }

        var replacedSkill = skill.MasterDefinition.ReplacedSkill;
        if (replacedSkill != null)
        {
            // Because we don't want to duplicate code from the replaced skills to the master skills, we just assign some values from the replaced skill.
            // These describe the skill behavior.
            skill.AttackDamage = replacedSkill.AttackDamage;
            skill.DamageType = replacedSkill.DamageType;
            skill.ElementalModifierTarget = replacedSkill.ElementalModifierTarget;
            skill.SkipElementalModifier = replacedSkill.SkipElementalModifier;
            skill.ImplicitTargetRange = replacedSkill.ImplicitTargetRange;
            skill.MovesTarget = replacedSkill.MovesTarget;
            skill.MovesToTarget = replacedSkill.MovesToTarget;
            skill.SkillType = replacedSkill.SkillType;
            skill.Target = replacedSkill.Target;
            skill.TargetRestriction = replacedSkill.TargetRestriction;
            skill.MagicEffectDef ??= replacedSkill.MagicEffectDef;

            if (replacedSkill.AreaSkillSettings is { } areaSkillSettings)
            {
                skill.AreaSkillSettings = this.Context.CreateNew<AreaSkillSettings>();
                var id = skill.AreaSkillSettings.GetId();
                skill.AreaSkillSettings.AssignValuesOf(areaSkillSettings, this.GameConfiguration);
                skill.AreaSkillSettings.SetGuid(id);
            }
        }
    }

    private void CreateSpecialSummonMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 150;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.Bali);
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(100 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 52 },
                { Stats.MaximumHealth, 5000 },
                { Stats.MinimumPhysBaseDmg, 165 },
                { Stats.MaximumPhysBaseDmg, 170 },
                { Stats.DefenseBase, 100 },
                { Stats.AttackRatePvm, 260 },
                { Stats.DefenseRatePvm, 75 },
                { Stats.PoisonResistance, 6f / 255 },
                { Stats.IceResistance, 6f / 255 },
                { Stats.WaterResistance, 6f / 255 },
                { Stats.FireResistance, 6f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 151;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.Soldier);
            monster.MoveRange = 3;
            monster.AttackRange = 4;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(100 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 58 },
                { Stats.MaximumHealth, 4000 },
                { Stats.MinimumPhysBaseDmg, 175 },
                { Stats.MaximumPhysBaseDmg, 180 },
                { Stats.DefenseBase, 110 },
                { Stats.AttackRatePvm, 290 },
                { Stats.DefenseRatePvm, 86 },
                { Stats.PoisonResistance, 6f / 255 },
                { Stats.IceResistance, 6f / 255 },
                { Stats.WaterResistance, 6f / 255 },
                { Stats.FireResistance, 6f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }
    }
}
