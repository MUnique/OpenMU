// <copyright file="SkillsInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;

// ReSharper disable StringLiteralTypo
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// Initialization logic for <see cref="Skill"/>s.
/// </summary>
internal class SkillsInitializer : SkillsInitializerBase
{
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
        { SkillNumber.InfinityArrowStr, MagicEffectNumber.InfiniteArrow },
        { SkillNumber.FireSlash, MagicEffectNumber.DefenseReduction },
        { SkillNumber.IgnoreDefense, MagicEffectNumber.IgnoreDefense },
        { SkillNumber.IncreaseHealth, MagicEffectNumber.IncreaseHealth },
        { SkillNumber.IncreaseBlock, MagicEffectNumber.IncreaseBlock },
        { SkillNumber.ExpansionofWizardry, MagicEffectNumber.WizEnhance },
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
        this.CreateSkill(SkillNumber.Poison, "Poison", CharacterClasses.AllMagicians, DamageType.Wizardry, 12, 6, manaConsumption: 42, energyRequirement: 140, elementalModifier: ElementalType.Poison);
        this.CreateSkill(SkillNumber.Meteorite, "Meteorite", CharacterClasses.AllMagicians | CharacterClasses.AllSummoners, DamageType.Wizardry, 21, 6, manaConsumption: 12, energyRequirement: 104, elementalModifier: ElementalType.Earth);
        this.CreateSkill(SkillNumber.Lightning, "Lightning", CharacterClasses.AllMagicians, DamageType.Wizardry, 17, 6, manaConsumption: 15, energyRequirement: 72, elementalModifier: ElementalType.Lightning);
        this.CreateSkill(SkillNumber.FireBall, "Fire Ball", CharacterClasses.AllMagicians | CharacterClasses.AllSummoners, DamageType.Wizardry, 8, 6, manaConsumption: 3, energyRequirement: 40, elementalModifier: ElementalType.Fire);
        this.CreateSkill(SkillNumber.Flame, "Flame", CharacterClasses.AllMagicians, DamageType.Wizardry, 25, 6, manaConsumption: 50, energyRequirement: 160, elementalModifier: ElementalType.Fire, skillType: SkillType.AreaSkillExplicitHits);
        this.CreateSkill(SkillNumber.Teleport, "Teleport", CharacterClasses.SoulMasterAndGrandMaster, DamageType.Wizardry, distance: 6, manaConsumption: 30, energyRequirement: 88, skillType: SkillType.Other);
        this.CreateSkill(SkillNumber.Ice, "Ice", CharacterClasses.AllMagicians | CharacterClasses.AllSummoners, DamageType.Wizardry, 10, 6, manaConsumption: 38, energyRequirement: 120, elementalModifier: ElementalType.Ice);
        this.CreateSkill(SkillNumber.Twister, "Twister", CharacterClasses.AllMagicians, DamageType.Wizardry, 35, 6, manaConsumption: 60, energyRequirement: 180, elementalModifier: ElementalType.Wind, skillType: SkillType.AreaSkillExplicitHits);
        this.CreateSkill(SkillNumber.EvilSpirit, "Evil Spirit", CharacterClasses.AllMagicians, DamageType.Wizardry, 45, 6, manaConsumption: 90, energyRequirement: 220, skillType: SkillType.AreaSkillExplicitHits);
        this.CreateSkill(SkillNumber.Hellfire, "Hellfire", CharacterClasses.AllMagicians, DamageType.Wizardry, 120, manaConsumption: 160, energyRequirement: 260, elementalModifier: ElementalType.Fire, skillType: SkillType.AreaSkillExplicitHits);
        this.CreateSkill(SkillNumber.PowerWave, "Power Wave", CharacterClasses.AllMagicians | CharacterClasses.AllSummoners, DamageType.Wizardry, 14, 6, manaConsumption: 5, energyRequirement: 56);
        this.CreateSkill(SkillNumber.AquaBeam, "Aqua Beam", CharacterClasses.AllMagicians, DamageType.Wizardry, 80, 6, manaConsumption: 140, energyRequirement: 345, elementalModifier: ElementalType.Water, skillType: SkillType.AreaSkillExplicitHits);
        this.CreateSkill(SkillNumber.Cometfall, "Cometfall", CharacterClasses.AllMagicians, DamageType.Wizardry, 70, 3, manaConsumption: 150, energyRequirement: 436, elementalModifier: ElementalType.Lightning, skillType: SkillType.AreaSkillExplicitHits);
        this.CreateSkill(SkillNumber.Inferno, "Inferno", CharacterClasses.AllMagicians, DamageType.Wizardry, 100, manaConsumption: 200, energyRequirement: 578, elementalModifier: ElementalType.Fire, skillType: SkillType.AreaSkillExplicitHits);
        this.CreateSkill(SkillNumber.TeleportAlly, "Teleport Ally", CharacterClasses.SoulMasterAndGrandMaster, distance: 6, abilityConsumption: 25, manaConsumption: 90, energyRequirement: 644, skillType: SkillType.Other);
        this.CreateSkill(SkillNumber.SoulBarrier, "Soul Barrier", CharacterClasses.SoulMasterAndGrandMaster, distance: 6, abilityConsumption: 22, manaConsumption: 70, energyRequirement: 408, skillType: SkillType.Buff, implicitTargetRange: 0, targetRestriction: SkillTargetRestriction.Party);
        this.CreateSkill(SkillNumber.EnergyBall, "Energy Ball", CharacterClasses.AllMagicians, DamageType.Wizardry, 3, 6, manaConsumption: 1);
        this.CreateSkill(SkillNumber.Defense, "Defense", CharacterClasses.AllKnightsLordsAndMGs, manaConsumption: 30, skillType: SkillType.Buff, implicitTargetRange: 0, targetRestriction: SkillTargetRestriction.Self);
        this.CreateSkill(SkillNumber.FallingSlash, "Falling Slash", CharacterClasses.AllKnightsLordsAndMGs | CharacterClasses.AllFighters, DamageType.Physical, distance: 3, manaConsumption: 9, movesToTarget: true, movesTarget: true);
        this.CreateSkill(SkillNumber.Lunge, "Lunge", CharacterClasses.AllKnightsLordsAndMGs, DamageType.Physical, distance: 2, manaConsumption: 9, movesToTarget: true, movesTarget: true);
        this.CreateSkill(SkillNumber.Uppercut, "Uppercut", CharacterClasses.AllKnightsLordsAndMGs, DamageType.Physical, distance: 2, manaConsumption: 8, movesToTarget: true, movesTarget: true);
        this.CreateSkill(SkillNumber.Cyclone, "Cyclone", CharacterClasses.AllKnightsLordsAndMGs, DamageType.Physical, distance: 2, manaConsumption: 9, movesToTarget: true, movesTarget: true);
        this.CreateSkill(SkillNumber.Slash, "Slash", CharacterClasses.AllKnightsLordsAndMGs, DamageType.Physical, distance: 2, manaConsumption: 10, movesToTarget: true, movesTarget: true);
        this.CreateSkill(SkillNumber.TripleShot, "Triple Shot", CharacterClasses.AllElfs, DamageType.Physical, distance: 6, manaConsumption: 5, skillType: SkillType.AreaSkillExplicitHits);
        this.CreateSkill(SkillNumber.Heal, "Heal", CharacterClasses.AllElfs, distance: 6, manaConsumption: 20, energyRequirement: 52, skillType: SkillType.Regeneration, targetRestriction: SkillTargetRestriction.Player);
        this.CreateSkill(SkillNumber.GreaterDefense, "Greater Defense", CharacterClasses.AllElfs, distance: 6, manaConsumption: 30, energyRequirement: 72, skillType: SkillType.Buff, targetRestriction: SkillTargetRestriction.Player);
        this.CreateSkill(SkillNumber.GreaterDamage, "Greater Damage", CharacterClasses.AllElfs, distance: 6, manaConsumption: 40, energyRequirement: 92, skillType: SkillType.Buff, targetRestriction: SkillTargetRestriction.Player);
        this.CreateSkill(SkillNumber.SummonGoblin, "Summon Goblin", CharacterClasses.AllElfs, manaConsumption: 40, energyRequirement: 90, skillType: SkillType.SummonMonster);
        this.CreateSkill(SkillNumber.SummonStoneGolem, "Summon Stone Golem", CharacterClasses.AllElfs, manaConsumption: 70, energyRequirement: 170, skillType: SkillType.SummonMonster);
        this.CreateSkill(SkillNumber.SummonAssassin, "Summon Assassin", CharacterClasses.AllElfs, manaConsumption: 110, energyRequirement: 190, skillType: SkillType.SummonMonster);
        this.CreateSkill(SkillNumber.SummonEliteYeti, "Summon Elite Yeti", CharacterClasses.AllElfs, manaConsumption: 160, energyRequirement: 230, skillType: SkillType.SummonMonster);
        this.CreateSkill(SkillNumber.SummonDarkKnight, "Summon Dark Knight", CharacterClasses.AllElfs, manaConsumption: 200, energyRequirement: 250, skillType: SkillType.SummonMonster);
        this.CreateSkill(SkillNumber.SummonBali, "Summon Bali", CharacterClasses.AllElfs, manaConsumption: 250, energyRequirement: 260, skillType: SkillType.SummonMonster);
        this.CreateSkill(SkillNumber.SummonSoldier, "Summon Soldier", CharacterClasses.AllElfs, manaConsumption: 350, energyRequirement: 280, skillType: SkillType.SummonMonster);
        this.CreateSkill(SkillNumber.Decay, "Decay", CharacterClasses.SoulMasterAndGrandMaster, DamageType.Wizardry, 95, 6, 7, 110, energyRequirement: 953, elementalModifier: ElementalType.Poison, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.IceStorm, "Ice Storm", CharacterClasses.SoulMasterAndGrandMaster, DamageType.Wizardry, 80, 6, 5, 100, energyRequirement: 849, elementalModifier: ElementalType.Ice, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.Nova, "Nova", CharacterClasses.SoulMasterAndGrandMaster, DamageType.Wizardry, distance: 6, manaConsumption: 180 / 12 /* mana per stage */, levelRequirement: 100, energyRequirement: 1052, elementalModifier: ElementalType.Fire, skillType: SkillType.Nova);
        this.CreateSkill(SkillNumber.NovaStart, "Nova (Start)", CharacterClasses.SoulMasterAndGrandMaster, DamageType.None, abilityConsumption: 45, levelRequirement: 100, energyRequirement: 1052, skillType: SkillType.Other);
        this.CreateSkill(SkillNumber.TwistingSlash, "Twisting Slash", CharacterClasses.AllKnights | CharacterClasses.AllMGs, DamageType.Physical, distance: 2, abilityConsumption: 10, manaConsumption: 10, elementalModifier: ElementalType.Wind, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.RagefulBlow, "Rageful Blow", CharacterClasses.BladeKnightAndBladeMaster, DamageType.Physical, 60, 3, 20, 25, 170, elementalModifier: ElementalType.Earth, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.DeathStab, "Death Stab", CharacterClasses.BladeKnightAndBladeMaster, DamageType.Physical, 70, 2, 12, 15, 160, elementalModifier: ElementalType.Wind, skillType: SkillType.DirectHit, skillTarget: SkillTarget.ExplicitWithImplicitInRange, implicitTargetRange: 1);
        this.CreateSkill(SkillNumber.CrescentMoonSlash, "Crescent Moon Slash", CharacterClasses.AllKnights, DamageType.Physical, 90, 4, 15, 22, movesToTarget: true, movesTarget: true);
        this.CreateSkill(SkillNumber.Lance, "Lance", CharacterClasses.SoulMasterAndGrandMaster | CharacterClasses.AllSummoners, DamageType.All, 90, 6, 10, 150);
        this.CreateSkill(SkillNumber.Starfall, "Starfall", CharacterClasses.AllElfs, DamageType.Physical, 120, 8, 15, 20);
        this.CreateSkill(SkillNumber.Impale, "Impale", CharacterClasses.AllKnights | CharacterClasses.AllMGs, DamageType.Physical, 15, 3, manaConsumption: 8, levelRequirement: 28);
        this.CreateSkill(SkillNumber.SwellLife, "Swell Life", CharacterClasses.AllKnights, abilityConsumption: 24, manaConsumption: 22, levelRequirement: 120, skillType: SkillType.Buff, skillTarget: SkillTarget.ImplicitParty);
        this.CreateSkill(SkillNumber.FireBreath, "Fire Breath", CharacterClasses.AllKnights, DamageType.Physical, 30, 3, manaConsumption: 9, levelRequirement: 110);
        this.CreateSkill(SkillNumber.FlameofEvil, "Flame of Evil (Monster)", damage: 120, manaConsumption: 160, levelRequirement: 60, energyRequirement: 100);
        this.CreateSkill(SkillNumber.IceArrow, "Ice Arrow", CharacterClasses.MuseElfAndHighElf, DamageType.Physical, 105, 8, 12, 10, elementalModifier: ElementalType.Ice);
        this.CreateSkill(SkillNumber.Penetration, "Penetration", CharacterClasses.AllElfs, DamageType.Physical, 70, 6, 9, 7, 130, elementalModifier: ElementalType.Wind, skillType: SkillType.AreaSkillExplicitHits);
        this.CreateSkill(SkillNumber.FireSlash, "Fire Slash", CharacterClasses.AllMGs, DamageType.Physical, 80, 2, 20, 15, elementalModifier: ElementalType.Fire, skillType: SkillType.AreaSkillExplicitHits);
        this.CreateSkill(SkillNumber.PowerSlash, "Power Slash", CharacterClasses.AllMGs, DamageType.Physical, distance: 5, manaConsumption: 15, energyRequirement: 100, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.SpiralSlash, "Spiral Slash", CharacterClasses.AllMGs, DamageType.Physical, 75, 5, 15, 20);
        this.CreateSkill(SkillNumber.Force, "Force", CharacterClasses.AllLords, DamageType.Physical, 10, 4, manaConsumption: 10);
        this.CreateSkill(SkillNumber.FireBurst, "Fire Burst", CharacterClasses.AllLords, DamageType.Physical, 100, 6, manaConsumption: 25, energyRequirement: 79, skillType: SkillType.DirectHit, skillTarget: SkillTarget.ExplicitWithImplicitInRange, implicitTargetRange: 1);
        this.CreateSkill(SkillNumber.Earthshake, "Earthshake", CharacterClasses.AllLords, DamageType.Physical, 150, 10, 50, elementalModifier: ElementalType.Lightning, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.Summon, "Summon", CharacterClasses.AllLords, abilityConsumption: 30, manaConsumption: 70, energyRequirement: 153, leadershipRequirement: 400, skillType: SkillType.Other);
        this.CreateSkill(SkillNumber.IncreaseCriticalDamage, "Increase Critical Damage", CharacterClasses.AllLords, abilityConsumption: 50, manaConsumption: 50, energyRequirement: 102, leadershipRequirement: 300, skillType: SkillType.Buff, skillTarget: SkillTarget.ImplicitParty);
        this.CreateSkill(SkillNumber.ElectricSpike, "Electric Spike", CharacterClasses.AllLords, DamageType.Physical, 250, 10, 100, energyRequirement: 126, leadershipRequirement: 340, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.ForceWave, "Force Wave", CharacterClasses.AllLords, DamageType.Physical, 50, 4, manaConsumption: 10, skillType: SkillType.DirectHit, skillTarget: SkillTarget.ExplicitWithImplicitInRange);
        this.CreateSkill(SkillNumber.Stun, "Stun", CharacterClasses.All, distance: 2, abilityConsumption: 50, manaConsumption: 70, skillType: SkillType.AreaSkillAutomaticHits, cooldownMinutes: 4);
        this.CreateSkill(SkillNumber.CancelStun, "Cancel Stun", CharacterClasses.All, abilityConsumption: 30, manaConsumption: 25, skillType: SkillType.Other, cooldownMinutes: 2);
        this.CreateSkill(SkillNumber.SwellMana, "Swell Mana", CharacterClasses.All, abilityConsumption: 30, manaConsumption: 35, cooldownMinutes: 4);
        this.CreateSkill(SkillNumber.Invisibility, "Invisibility", CharacterClasses.All, abilityConsumption: 60, manaConsumption: 80, cooldownMinutes: 5);
        this.CreateSkill(SkillNumber.CancelInvisibility, "Cancel Invisibility", CharacterClasses.All, abilityConsumption: 30, manaConsumption: 40, cooldownMinutes: 2);
        this.CreateSkill(SkillNumber.AbolishMagic, "Abolish Magic", CharacterClasses.AllLords, abilityConsumption: 70, manaConsumption: 90, cooldownMinutes: 8);
        this.CreateSkill(SkillNumber.ManaRays, "Mana Rays", CharacterClasses.AllMGs, DamageType.Wizardry, 85, 6, 7, 130);
        this.CreateSkill(SkillNumber.FireBlast, "Fire Blast", CharacterClasses.AllLords, DamageType.Physical, 150, 6, 10, 30);
        this.CreateSkill(SkillNumber.PlasmaStorm, "Plasma Storm", CharacterClasses.AllMastersAndSecondClass, DamageType.Fenrir, damage: 60, distance: 6, abilityConsumption: 20, manaConsumption: 50, levelRequirement: 110, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.InfinityArrow, "Infinity Arrow", CharacterClasses.MuseElfAndHighElf, distance: 6, abilityConsumption: 10, manaConsumption: 50, levelRequirement: 220, skillType: SkillType.Buff, targetRestriction: SkillTargetRestriction.Self);
        this.CreateSkill(SkillNumber.FireScream, "Fire Scream", CharacterClasses.AllLords, DamageType.Physical, 130, 6, 10, 45, energyRequirement: 70, leadershipRequirement: 150, skillType: SkillType.AreaSkillExplicitHits);
        this.CreateSkill(SkillNumber.Explosion79, "Explosion", CharacterClasses.AllLords, DamageType.Physical, distance: 2);
        this.CreateSkill(SkillNumber.SummonMonster, "Summon Monster", manaConsumption: 40, energyRequirement: 90);
        this.CreateSkill(SkillNumber.MagicAttackImmunity, "Magic Attack Immunity", manaConsumption: 40, energyRequirement: 90);
        this.CreateSkill(SkillNumber.PhysicalAttackImmunity, "Physical Attack Immunity", manaConsumption: 40, energyRequirement: 90);
        this.CreateSkill(SkillNumber.PotionofBless, "Potion of Bless", manaConsumption: 40, energyRequirement: 90);
        this.CreateSkill(SkillNumber.PotionofSoul, "Potion of Soul", manaConsumption: 40, energyRequirement: 90);
        this.CreateSkill(SkillNumber.SpellofProtection, "Spell of Protection", CharacterClasses.All, manaConsumption: 30, elementalModifier: ElementalType.Ice, cooldownMinutes: 5);
        this.CreateSkill(SkillNumber.SpellofRestriction, "Spell of Restriction", CharacterClasses.All, distance: 3, manaConsumption: 30, elementalModifier: ElementalType.Ice, cooldownMinutes: 5);
        this.CreateSkill(SkillNumber.SpellofPursuit, "Spell of Pursuit", CharacterClasses.All, manaConsumption: 30, elementalModifier: ElementalType.Ice, cooldownMinutes: 10);
        this.CreateSkill(SkillNumber.ShieldBurn, "Shield-Burn", CharacterClasses.All, distance: 3, manaConsumption: 30, elementalModifier: ElementalType.Ice, cooldownMinutes: 5);
        this.CreateSkill(SkillNumber.DrainLife, "Drain Life", CharacterClasses.AllSummoners, DamageType.Curse, 35, 6, manaConsumption: 50, energyRequirement: 150, skillType: SkillType.AreaSkillExplicitTarget);
        this.CreateSkill(SkillNumber.ChainLightning, "Chain Lightning", CharacterClasses.AllSummoners, DamageType.Curse, 70, 6, manaConsumption: 85, energyRequirement: 245, skillType: SkillType.AreaSkillExplicitTarget, skillTarget: SkillTarget.Explicit);
        this.CreateSkill(SkillNumber.DamageReflection, "Damage Reflection", CharacterClasses.AllSummoners, distance: 5, abilityConsumption: 10, manaConsumption: 40, energyRequirement: 375);
        this.CreateSkill(SkillNumber.Berserker, "Berserker", CharacterClasses.AllSummoners, DamageType.Curse, distance: 5, abilityConsumption: 50, manaConsumption: 100, energyRequirement: 620);
        this.CreateSkill(SkillNumber.Sleep, "Sleep", CharacterClasses.AllSummoners, distance: 6, abilityConsumption: 3, manaConsumption: 20, energyRequirement: 180, skillType: SkillType.Buff);
        this.CreateSkill(SkillNumber.Weakness, "Weakness", CharacterClasses.AllSummoners, distance: 6, abilityConsumption: 15, manaConsumption: 50, energyRequirement: 663);
        this.CreateSkill(SkillNumber.Innovation, "Innovation", CharacterClasses.AllSummoners, distance: 6, abilityConsumption: 15, manaConsumption: 70, energyRequirement: 912);
        this.CreateSkill(SkillNumber.Explosion223, "Explosion", CharacterClasses.AllSummoners, DamageType.Curse, 40, 6, 5, 90, energyRequirement: 100, elementalModifier: ElementalType.Fire);
        this.CreateSkill(SkillNumber.Requiem, "Requiem", CharacterClasses.AllSummoners, DamageType.Curse, 65, 6, 10, 110, energyRequirement: 99, elementalModifier: ElementalType.Wind);
        this.CreateSkill(SkillNumber.Pollution, "Pollution", CharacterClasses.AllSummoners, DamageType.Curse, 80, 6, 15, 120, energyRequirement: 115, elementalModifier: ElementalType.Lightning);
        this.CreateSkill(SkillNumber.LightningShock, "Lightning Shock", CharacterClasses.AllSummoners, DamageType.Curse, 95, 6, 7, 115, energyRequirement: 823, elementalModifier: ElementalType.Lightning);
        this.CreateSkill(SkillNumber.StrikeofDestruction, "Strike of Destruction", CharacterClasses.BladeKnightAndBladeMaster, DamageType.Physical, 110, 5, 24, 30, 100, elementalModifier: ElementalType.Ice, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.ExpansionofWizardry, "Expansion of Wizardry", CharacterClasses.SoulMasterAndGrandMaster, distance: 6, abilityConsumption: 50, manaConsumption: 200, levelRequirement: 220, energyRequirement: 118, skillType: SkillType.Buff, targetRestriction: SkillTargetRestriction.Player, skillTarget: SkillTarget.ImplicitPlayer);
        this.CreateSkill(SkillNumber.Recovery, "Recovery", CharacterClasses.MuseElfAndHighElf, distance: 6, abilityConsumption: 10, manaConsumption: 40, levelRequirement: 100, energyRequirement: 37, skillType: SkillType.Regeneration, targetRestriction: SkillTargetRestriction.Player);
        this.CreateSkill(SkillNumber.MultiShot, "Multi-Shot", CharacterClasses.MuseElfAndHighElf, DamageType.Physical, 40, 6, 7, 10, 100, skillType: SkillType.AreaSkillExplicitHits);
        this.CreateSkill(SkillNumber.FlameStrike, "Flame Strike", CharacterClasses.AllMGs, DamageType.Physical, 140, 3, 25, 20, 100, elementalModifier: ElementalType.Fire, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.GiganticStorm, "Gigantic Storm", CharacterClasses.AllMGs, DamageType.Wizardry, 110, 6, 10, 120, 220, 118, elementalModifier: ElementalType.Wind, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.ChaoticDiseier, "Chaotic Diseier", CharacterClasses.AllLords, DamageType.Physical, 190, 6, 15, 50, 100, 16, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.DoppelgangerSelfExplosion, "Doppelganger Self Explosion", CharacterClasses.AllMGs, DamageType.Wizardry, 140, 3, 25, 20, 100, elementalModifier: ElementalType.Fire);
        this.CreateSkill(SkillNumber.KillingBlow, "Killing Blow", CharacterClasses.AllFighters, DamageType.Physical, distance: 2, manaConsumption: 9, elementalModifier: ElementalType.Earth);
        this.CreateSkill(SkillNumber.BeastUppercut, "Beast Uppercut", CharacterClasses.AllFighters, DamageType.Physical, distance: 2, manaConsumption: 9, elementalModifier: ElementalType.Fire);
        this.CreateSkill(SkillNumber.ChainDrive, "Chain Drive", CharacterClasses.AllFighters, DamageType.Physical, distance: 4, abilityConsumption: 20, manaConsumption: 15, levelRequirement: 150, elementalModifier: ElementalType.Ice);
        this.CreateSkill(SkillNumber.DarkSide, "Dark Side", CharacterClasses.AllFighters, DamageType.Physical, distance: 4, manaConsumption: 70, levelRequirement: 180, elementalModifier: ElementalType.Wind);
        this.CreateSkill(SkillNumber.DragonRoar, "Dragon Roar", CharacterClasses.AllFighters, DamageType.Physical, distance: 3, abilityConsumption: 30, manaConsumption: 50, levelRequirement: 150, elementalModifier: ElementalType.Earth, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.DragonSlasher, "Dragon Slasher", CharacterClasses.AllFighters, DamageType.Physical, distance: 4, abilityConsumption: 100, manaConsumption: 100, levelRequirement: 200, elementalModifier: ElementalType.Wind);
        this.CreateSkill(SkillNumber.IgnoreDefense, "Ignore Defense", CharacterClasses.AllFighters, DamageType.Physical, distance: 3, abilityConsumption: 10, manaConsumption: 50, levelRequirement: 120, energyRequirement: 404, skillType: SkillType.Buff, skillTarget: SkillTarget.ImplicitPlayer);
        this.CreateSkill(SkillNumber.IncreaseHealth, "Increase Health", CharacterClasses.AllFighters, DamageType.Physical, distance: 7, abilityConsumption: 10, manaConsumption: 50, levelRequirement: 80, energyRequirement: 132, skillType: SkillType.Buff, skillTarget: SkillTarget.ImplicitParty);
        this.CreateSkill(SkillNumber.IncreaseBlock, "Increase Block", CharacterClasses.AllFighters, DamageType.Physical, distance: 7, abilityConsumption: 10, manaConsumption: 50, levelRequirement: 50, energyRequirement: 80, skillType: SkillType.Buff, skillTarget: SkillTarget.ImplicitParty);
        this.CreateSkill(SkillNumber.Charge, "Charge", CharacterClasses.AllFighters, DamageType.Physical, 90, 4, 15, 20);
        this.CreateSkill(SkillNumber.PhoenixShot, "Phoenix Shot", CharacterClasses.AllFighters, DamageType.Physical, distance: 4, manaConsumption: 30, elementalModifier: ElementalType.Earth, skillType: SkillType.AreaSkillExplicitTarget);

        // Generic monster skills:
        this.CreateSkill(SkillNumber.MonsterSkill, "Generic Monster Skill", distance: 5, skillType: SkillType.Other);

        // Master skills:
        // Common:
        this.CreateSkill(SkillNumber.DurabilityReduction1, "Durability Reduction (1)", CharacterClasses.AllMastersExceptFistMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.PvPDefenceRateInc, "PvP Defence Rate Inc", CharacterClasses.AllMastersExceptFistMaster, damage: 12, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.MaximumSDincrease, "Maximum SD increase", CharacterClasses.AllMastersExceptFistMaster, damage: 13, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.AutomaticManaRecInc, "Automatic Mana Rec Inc", CharacterClasses.AllMastersExceptFistMaster, damage: 7, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.PoisonResistanceInc, "Poison Resistance Inc", CharacterClasses.AllMastersExceptFistMaster, damage: 1, elementalModifier: ElementalType.Poison, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DurabilityReduction2, "Durability Reduction (2)", CharacterClasses.AllMastersExceptFistMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.SdRecoverySpeedInc, "SD Recovery Speed Inc", CharacterClasses.AllMastersExceptFistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.AutomaticHpRecInc, "Automatic HP Rec Inc", CharacterClasses.AllMastersExceptFistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.LightningResistanceInc, "Lightning Resistance Inc", CharacterClasses.AllMastersExceptFistMaster, damage: 1, elementalModifier: ElementalType.Lightning, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DefenseIncrease, "Defense Increase", CharacterClasses.AllMastersExceptFistMaster, damage: 16, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.AutomaticAgRecInc, "Automatic AG Rec Inc", CharacterClasses.AllMastersExceptFistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IceResistanceIncrease, "Ice Resistance Increase", CharacterClasses.AllMastersExceptFistMaster, damage: 1, elementalModifier: ElementalType.Ice, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DurabilityReduction3, "Durability Reduction (3)", CharacterClasses.AllMastersExceptFistMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DefenseSuccessRateInc, "Defense Success Rate Inc", CharacterClasses.AllMastersExceptFistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.MaximumLifeIncrease, "Maximum Life Increase", CharacterClasses.AllMastersExceptFistMaster, damage: 9, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.ManaReduction, "Mana Reduction", CharacterClasses.AllMastersExceptFistMaster, damage: 18, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.MonsterAttackSdInc, "Monster Attack SD Inc", CharacterClasses.AllMastersExceptFistMaster, damage: 11, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.MonsterAttackLifeInc, "Monster Attack Life Inc", CharacterClasses.AllMastersExceptFistMaster, damage: 6, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.SwellLifeProficiency, "Swell Life Proficiency", CharacterClasses.BladeMaster, damage: 7, abilityConsumption: 28, manaConsumption: 26, levelRequirement: 120);
        this.CreateSkill(SkillNumber.MinimumAttackPowerInc, "Minimum Attack Power Inc", CharacterClasses.BladeMaster | CharacterClasses.DuelMaster | CharacterClasses.LordEmperor, DamageType.Physical, 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.MonsterAttackManaInc, "Monster Attack Mana Inc", CharacterClasses.AllMastersExceptFistMaster, damage: 6, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.PvPAttackRate, "PvP Attack Rate", CharacterClasses.AllMastersExceptFistMaster, damage: 14, skillType: SkillType.PassiveBoost);

        // Blade Master:
        this.CreateSkill(SkillNumber.AttackSuccRateInc, "Attack Succ Rate Inc", CharacterClasses.AllMastersExceptFistMaster, damage: 13, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.CycloneStrengthener, "Cyclone Strengthener", CharacterClasses.BladeMaster, DamageType.Physical, 22, 2, manaConsumption: 9);
        this.CreateSkill(SkillNumber.SlashStrengthener, "Slash Strengthener", CharacterClasses.BladeMaster, DamageType.Physical, 3, 2, manaConsumption: 10);
        this.CreateSkill(SkillNumber.FallingSlashStreng, "Falling Slash Streng", CharacterClasses.BladeMaster, DamageType.Physical, 3, 3, manaConsumption: 9);
        this.CreateSkill(SkillNumber.LungeStrengthener, "Lunge Strengthener", CharacterClasses.BladeMaster, DamageType.Physical, 3, 2, manaConsumption: 9);
        this.CreateSkill(SkillNumber.TwistingSlashStreng, "Twisting Slash Streng", CharacterClasses.BladeMaster, DamageType.Physical, 3, 2, 10, 10);
        this.CreateSkill(SkillNumber.RagefulBlowStreng, "Rageful Blow Streng", CharacterClasses.BladeMaster, DamageType.Physical, 22, 3, 22, 25, 170);
        this.CreateSkill(SkillNumber.TwistingSlashMastery, "Twisting Slash Mastery", CharacterClasses.BladeMaster, DamageType.Physical, 1, 2, 20, 22);
        this.CreateSkill(SkillNumber.RagefulBlowMastery, "Rageful Blow Mastery", CharacterClasses.BladeMaster, DamageType.Physical, 1, 3, 30, 50, 170, elementalModifier: ElementalType.Earth);
        this.CreateSkill(SkillNumber.WeaponMasteryBladeMaster, "Weapon Mastery", CharacterClasses.BladeMaster, damage: 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DeathStabStrengthener, "Death Stab Strengthener", CharacterClasses.BladeMaster, DamageType.Physical, 22, 2, 13, 15, 160, elementalModifier: ElementalType.Wind);
        this.CreateSkill(SkillNumber.StrikeofDestrStr, "Strike of Destr Str", CharacterClasses.BladeMaster, DamageType.Physical, 22, 5, 24, 30, 100, elementalModifier: ElementalType.Ice);
        this.CreateSkill(SkillNumber.MaximumManaIncrease, "Maximum Mana Increase", CharacterClasses.AllMastersExceptFistMaster, damage: 9, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.TwoHandedSwordStrengthener, "Two-handed Sword Stren", CharacterClasses.BladeMaster | CharacterClasses.DuelMaster, DamageType.Physical, 4, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.OneHandedSwordStrengthener, "One-handed Sword Stren", CharacterClasses.BladeMaster | CharacterClasses.DuelMaster, DamageType.Physical, 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.MaceStrengthener, "Mace Strengthener", CharacterClasses.BladeMaster, DamageType.Physical, 3, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.SpearStrengthener, "Spear Strengthener", CharacterClasses.BladeMaster, DamageType.Physical, 3, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.TwoHandedSwordMaster, "Two-handed Sword Mast", CharacterClasses.BladeMaster | CharacterClasses.DuelMaster, DamageType.Physical, 5, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.OneHandedSwordMaster, "One-handed Sword Mast", CharacterClasses.BladeMaster | CharacterClasses.DuelMaster, DamageType.Physical, 23, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.MaceMastery, "Mace Mastery", CharacterClasses.BladeMaster, DamageType.Physical, 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.SpearMastery, "Spear Mastery", CharacterClasses.BladeMaster, DamageType.Physical, 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.SwellLifeStrengt, "Swell Life Strengt", CharacterClasses.BladeMaster, damage: 7, abilityConsumption: 26, manaConsumption: 24, levelRequirement: 120);

        // Grand Master:
        this.CreateSkill(SkillNumber.FlameStrengthener, "Flame Strengthener", CharacterClasses.GrandMaster, DamageType.Wizardry, 3, 6, manaConsumption: 55, levelRequirement: 35, energyRequirement: 100, elementalModifier: ElementalType.Fire);
        this.CreateSkill(SkillNumber.LightningStrengthener, "Lightning Strengthener", CharacterClasses.GrandMaster, DamageType.Wizardry, 3, 6, manaConsumption: 20, levelRequirement: 13, energyRequirement: 100, elementalModifier: ElementalType.Lightning);
        this.CreateSkill(SkillNumber.ExpansionofWizStreng, "Expansion of Wiz Streng", CharacterClasses.GrandMaster, DamageType.Wizardry, 1, 6, 55, 220, 220, 118);
        this.CreateSkill(SkillNumber.InfernoStrengthener, "Inferno Strengthener", CharacterClasses.GrandMaster, DamageType.Wizardry, 22, manaConsumption: 220, levelRequirement: 88, energyRequirement: 200, elementalModifier: ElementalType.Fire);
        this.CreateSkill(SkillNumber.BlastStrengthener, "Blast Strengthener", CharacterClasses.GrandMaster, DamageType.Wizardry, 22, 3, manaConsumption: 165, levelRequirement: 80, energyRequirement: 150, elementalModifier: ElementalType.Lightning);
        this.CreateSkill(SkillNumber.ExpansionofWizMas, "Expansion of Wiz Mas", CharacterClasses.GrandMaster, DamageType.Wizardry, 1, 6, 55, 220, 220, 118);
        this.CreateSkill(SkillNumber.PoisonStrengthener, "Poison Strengthener", CharacterClasses.GrandMaster, DamageType.Wizardry, 3, 6, manaConsumption: 46, levelRequirement: 30, energyRequirement: 100, elementalModifier: ElementalType.Poison);
        this.CreateSkill(SkillNumber.EvilSpiritStreng, "Evil Spirit Streng", CharacterClasses.GrandMaster, DamageType.Wizardry, 22, 6, manaConsumption: 108, levelRequirement: 50, energyRequirement: 100);
        this.CreateSkill(SkillNumber.MagicMasteryGrandMaster, "Magic Mastery", CharacterClasses.GrandMaster, damage: 22, levelRequirement: 50, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DecayStrengthener, "Decay Strengthener", CharacterClasses.GrandMaster, DamageType.Wizardry, 22, 6, 10, 120, 96, 243, elementalModifier: ElementalType.Poison);
        this.CreateSkill(SkillNumber.HellfireStrengthener, "Hellfire Strengthener", CharacterClasses.GrandMaster, DamageType.Wizardry, 3, manaConsumption: 176, levelRequirement: 60, energyRequirement: 100, elementalModifier: ElementalType.Fire);
        this.CreateSkill(SkillNumber.IceStrengthener, "Ice Strengthener", CharacterClasses.GrandMaster, DamageType.Wizardry, 3, 6, manaConsumption: 42, levelRequirement: 25, energyRequirement: 100, elementalModifier: ElementalType.Ice);
        this.CreateSkill(SkillNumber.OneHandedStaffStrengthener, "One-handed Staff Stren", CharacterClasses.GrandMaster | CharacterClasses.DuelMaster, DamageType.Wizardry, 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.TwoHandedStaffStrengthener, "Two-handed Staff Stren", CharacterClasses.GrandMaster | CharacterClasses.DuelMaster, DamageType.Wizardry, 4, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.ShieldStrengthenerGrandMaster, "Shield Strengthener", CharacterClasses.GrandMaster, damage: 10, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.OneHandedStaffMaster, "One-handed Staff Mast", CharacterClasses.GrandMaster | CharacterClasses.DuelMaster, damage: 23, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.TwoHandedStaffMaster, "Two-handed Staff Mast", CharacterClasses.GrandMaster | CharacterClasses.DuelMaster, DamageType.Wizardry, 5, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.ShieldMasteryGrandMaster, "Shield Mastery", CharacterClasses.GrandMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.SoulBarrierStrength, "Soul Barrier Strength", CharacterClasses.GrandMaster, damage: 7, distance: 6, abilityConsumption: 24, manaConsumption: 77, levelRequirement: 77, energyRequirement: 126);
        this.CreateSkill(SkillNumber.SoulBarrierProficie, "Soul Barrier Proficie", CharacterClasses.GrandMaster, damage: 10, distance: 6, abilityConsumption: 26, manaConsumption: 84, levelRequirement: 77, energyRequirement: 126);
        this.CreateSkill(SkillNumber.MinimumWizardryInc, "Minimum Wizardry Inc", CharacterClasses.GrandMaster | CharacterClasses.DuelMaster, damage: 22, skillType: SkillType.PassiveBoost);

        // High Elf:
        this.CreateSkill(SkillNumber.HealStrengthener, "Heal Strengthener", CharacterClasses.HighElf, DamageType.Physical, 22, 6, manaConsumption: 22, levelRequirement: 8, energyRequirement: 100);
        this.CreateSkill(SkillNumber.TripleShotStrengthener, "Triple Shot Strengthener", CharacterClasses.HighElf, DamageType.Physical, 22, 6, manaConsumption: 5);
        this.CreateSkill(SkillNumber.SummonedMonsterStr1, "Summoned Monster Str (1)", CharacterClasses.HighElf, damage: 16, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.PenetrationStrengthener, "Penetration Strengthener", CharacterClasses.HighElf, DamageType.Physical, 22, 6, 11, 10, 130, elementalModifier: ElementalType.Wind);
        this.CreateSkill(SkillNumber.DefenseIncreaseStr, "Defense Increase Str", CharacterClasses.HighElf, damage: 22, distance: 6, manaConsumption: 33, levelRequirement: 13, energyRequirement: 100);
        this.CreateSkill(SkillNumber.TripleShotMastery, "Triple Shot Mastery", CharacterClasses.HighElf, DamageType.Physical, distance: 6, manaConsumption: 9);
        this.CreateSkill(SkillNumber.SummonedMonsterStr2, "Summoned Monster Str (2)", CharacterClasses.HighElf, damage: 16, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.AttackIncreaseStr, "Attack Increase Str", CharacterClasses.HighElf, damage: 22, distance: 6, manaConsumption: 44, levelRequirement: 18, energyRequirement: 100);
        this.CreateSkill(SkillNumber.WeaponMasteryHighElf, "Weapon Mastery", CharacterClasses.HighElf, damage: 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.AttackIncreaseMastery, "Attack Increase Mastery", CharacterClasses.HighElf, damage: 22, distance: 6, manaConsumption: 48, levelRequirement: 18, energyRequirement: 100);
        this.CreateSkill(SkillNumber.DefenseIncreaseMastery, "Defense Increase Mastery", CharacterClasses.HighElf, damage: 22, distance: 6, manaConsumption: 36, levelRequirement: 13, energyRequirement: 100);
        this.CreateSkill(SkillNumber.IceArrowStrengthener, "Ice Arrow Strengthener", CharacterClasses.HighElf, DamageType.Physical, 22, 8, 18, 15, elementalModifier: ElementalType.Ice);
        this.CreateSkill(SkillNumber.BowStrengthener, "Bow Strengthener", CharacterClasses.HighElf, damage: 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.CrossbowStrengthener, "Crossbow Strengthener", CharacterClasses.HighElf, damage: 3, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.ShieldStrengthenerHighElf, "Shield Strengthener", CharacterClasses.HighElf, damage: 10, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.BowMastery, "Bow Mastery", CharacterClasses.HighElf, damage: 23, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.CrossbowMastery, "Crossbow Mastery", CharacterClasses.HighElf, damage: 5, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.ShieldMasteryHighElf, "Shield Mastery", CharacterClasses.HighElf, damage: 15, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.InfinityArrowStr, "Infinity Arrow Str", CharacterClasses.HighElf, damage: 1, distance: 6, abilityConsumption: 11, manaConsumption: 55, levelRequirement: 220, skillType: SkillType.Buff, targetRestriction: SkillTargetRestriction.Self);
        this.CreateSkill(SkillNumber.MinimumAttPowerInc, "Minimum Att Power Inc", CharacterClasses.HighElf, DamageType.Physical, 22, skillType: SkillType.PassiveBoost);

        // Dimension Master (Summoner):
        this.CreateSkill(SkillNumber.FireTomeStrengthener, "Fire Tome Strengthener", CharacterClasses.DimensionMaster, DamageType.Curse, 3, elementalModifier: ElementalType.Fire, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.WindTomeStrengthener, "Wind Tome Strengthener", CharacterClasses.DimensionMaster, DamageType.Curse, 3, elementalModifier: ElementalType.Wind, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.LightningTomeStren, "Lightning Tome Stren", CharacterClasses.DimensionMaster, DamageType.Curse, 3, elementalModifier: ElementalType.Lightning, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.FireTomeMastery, "Fire Tome Mastery", CharacterClasses.DimensionMaster, DamageType.Curse, 7, elementalModifier: ElementalType.Fire, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.WindTomeMastery, "Wind Tome Mastery", CharacterClasses.DimensionMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.LightningTomeMastery, "Lightning Tome Mastery", CharacterClasses.DimensionMaster, DamageType.Curse, 7, elementalModifier: ElementalType.Lightning, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.SleepStrengthener, "Sleep Strengthener", CharacterClasses.DimensionMaster, damage: 1, distance: 6, abilityConsumption: 7, manaConsumption: 30, levelRequirement: 40, energyRequirement: 100);
        this.CreateSkill(SkillNumber.ChainLightningStr, "Chain Lightning Str", CharacterClasses.DimensionMaster, DamageType.Curse, 22, 6, manaConsumption: 103, levelRequirement: 75, energyRequirement: 75, skillTarget: SkillTarget.Explicit, skillType: SkillType.AreaSkillExplicitTarget);
        this.CreateSkill(SkillNumber.LightningShockStr, "Lightning Shock Str", CharacterClasses.DimensionMaster, DamageType.Curse, 22, 6, 10, 125, 93, 216, elementalModifier: ElementalType.Lightning);
        this.CreateSkill(SkillNumber.MagicMasterySummoner, "Magic Mastery", CharacterClasses.DimensionMaster, DamageType.Curse, 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DrainLifeStrengthener, "Drain Life Strengthener", CharacterClasses.DimensionMaster, DamageType.Curse, 22, 6, manaConsumption: 57, levelRequirement: 35, energyRequirement: 93);
        this.CreateSkill(SkillNumber.StickStrengthener, "Stick Strengthener", CharacterClasses.DimensionMaster, DamageType.Curse, 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.OtherWorldTomeStreng, "Other World Tome Streng", CharacterClasses.DimensionMaster, DamageType.Curse, 3, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.StickMastery, "Stick Mastery", CharacterClasses.DimensionMaster, DamageType.Curse, 5, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.OtherWorldTomeMastery, "Other World Tome Mastery", CharacterClasses.DimensionMaster, damage: 23, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.BerserkerStrengthener, "Berserker Strengthener", CharacterClasses.DimensionMaster, DamageType.Curse, 7, 5, 75, 150, 83, 181);
        this.CreateSkill(SkillNumber.BerserkerProficiency, "Berserker Proficiency", CharacterClasses.DimensionMaster, DamageType.Curse, 7, 5, 82, 165, 83, 181);
        this.CreateSkill(SkillNumber.MinimumWizCurseInc, "Minimum Wiz/Curse Inc", CharacterClasses.DimensionMaster, damage: 22, skillType: SkillType.PassiveBoost);

        // Duel Master (MG):
        this.CreateSkill(SkillNumber.CycloneStrengthenerDuelMaster, "Cyclone Strengthener", CharacterClasses.DuelMaster, DamageType.Physical, 22, 2, manaConsumption: 9);
        this.CreateSkill(SkillNumber.LightningStrengthenerDuelMaster, "Lightning Strengthener", CharacterClasses.DuelMaster, DamageType.Physical, 3, 6, manaConsumption: 20, levelRequirement: 13, energyRequirement: 100, elementalModifier: ElementalType.Lightning);
        this.CreateSkill(SkillNumber.TwistingSlashStrengthenerDuelMaster, "Twisting Slash Stren", CharacterClasses.DuelMaster, DamageType.Physical, 3, 2, 10, 10);
        this.CreateSkill(SkillNumber.PowerSlashStreng, "Power Slash Streng", CharacterClasses.DuelMaster, damage: 3, distance: 5, manaConsumption: 15, energyRequirement: 100);
        this.CreateSkill(SkillNumber.FlameStrengthenerDuelMaster, "Flame Strengthener", CharacterClasses.DuelMaster, DamageType.Physical, 3, 6, manaConsumption: 55, levelRequirement: 35, energyRequirement: 100, elementalModifier: ElementalType.Fire);
        this.CreateSkill(SkillNumber.BlastStrengthenerDuelMaster, "Blast Strengthener", CharacterClasses.DuelMaster, DamageType.Physical, 22, 3, manaConsumption: 165, levelRequirement: 80, energyRequirement: 150, elementalModifier: ElementalType.Lightning);
        this.CreateSkill(SkillNumber.WeaponMasteryDuelMaster, "Weapon Mastery", CharacterClasses.DuelMaster, damage: 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.InfernoStrengthenerDuelMaster, "Inferno Strengthener", CharacterClasses.DuelMaster, DamageType.Physical, 22, manaConsumption: 220, levelRequirement: 88, energyRequirement: 200, elementalModifier: ElementalType.Fire);
        this.CreateSkill(SkillNumber.EvilSpiritStrengthenerDuelMaster, "Evil Spirit Strengthen", CharacterClasses.DuelMaster, DamageType.Physical, 22, 6, manaConsumption: 108, levelRequirement: 50, energyRequirement: 100);
        this.CreateSkill(SkillNumber.MagicMasteryDuelMaster, "Magic Mastery", CharacterClasses.DuelMaster, damage: 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IceStrengthenerDuelMaster, "Ice Strengthener", CharacterClasses.DuelMaster, DamageType.Physical, 3, 6, manaConsumption: 42, levelRequirement: 25, energyRequirement: 100, elementalModifier: ElementalType.Ice);
        this.CreateSkill(SkillNumber.BloodAttackStrengthen, "Blood Attack Strengthen", CharacterClasses.DuelMaster, damage: 22, distance: 3, abilityConsumption: 22, manaConsumption: 15, elementalModifier: ElementalType.Poison);

        // Lord Emperor (DL):
        this.CreateSkill(SkillNumber.FireBurstStreng, "Fire Burst Streng", CharacterClasses.LordEmperor, DamageType.Physical, 22, 6, manaConsumption: 25, levelRequirement: 74, energyRequirement: 20);
        this.CreateSkill(SkillNumber.ForceWaveStreng, "Force Wave Streng", CharacterClasses.LordEmperor, DamageType.Physical, 3, 4, manaConsumption: 15);
        this.CreateSkill(SkillNumber.DarkHorseStreng1, "Dark Horse Streng (1)", CharacterClasses.LordEmperor, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.CriticalDmgIncPowUp, "Critical DMG Inc PowUp", CharacterClasses.LordEmperor, damage: 3, abilityConsumption: 75, manaConsumption: 75, levelRequirement: 82, energyRequirement: 25, leadershipRequirement: 300);
        this.CreateSkill(SkillNumber.EarthshakeStreng, "Earthshake Streng", CharacterClasses.LordEmperor, DamageType.Physical, 22, 10, 75, elementalModifier: ElementalType.Lightning);
        this.CreateSkill(SkillNumber.WeaponMasteryLordEmperor, "Weapon Mastery", CharacterClasses.LordEmperor, damage: 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.FireBurstMastery, "Fire Burst Mastery", CharacterClasses.LordEmperor, DamageType.Physical, 1, 6, manaConsumption: 27, levelRequirement: 74, energyRequirement: 20);
        this.CreateSkill(SkillNumber.CritDmgIncPowUp2, "Crit DMG Inc PowUp (2)", CharacterClasses.LordEmperor, damage: 10, abilityConsumption: 82, manaConsumption: 82, levelRequirement: 82, energyRequirement: 25, leadershipRequirement: 300);
        this.CreateSkill(SkillNumber.EarthshakeMastery, "Earthshake Mastery", CharacterClasses.LordEmperor, DamageType.Physical, 1, 10, 75, elementalModifier: ElementalType.Lightning);
        this.CreateSkill(SkillNumber.CritDmgIncPowUp3, "Crit DMG Inc PowUp (3)", CharacterClasses.LordEmperor, damage: 7, abilityConsumption: 100, manaConsumption: 100, levelRequirement: 82, energyRequirement: 25, leadershipRequirement: 300);
        this.CreateSkill(SkillNumber.FireScreamStren, "Fire Scream Stren", CharacterClasses.LordEmperor, DamageType.Physical, 22, 6, 11, 45, 102, 32, 70);
        this.CreateSkill(SkillNumber.DarkSpiritStr, "Dark Spirit Str", CharacterClasses.LordEmperor, damage: 3, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.ScepterStrengthener, "Scepter Strengthener", CharacterClasses.LordEmperor, DamageType.Physical, 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.ShieldStrengthenerLordEmperor, "Shield Strengthener", CharacterClasses.LordEmperor, damage: 10, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.UseScepterPetStr, "Use Scepter : Pet Str", CharacterClasses.LordEmperor, damage: 3, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DarkSpiritStr2, "Dark Spirit Str (2)", CharacterClasses.LordEmperor, damage: 7, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.ScepterMastery, "Scepter Mastery", CharacterClasses.LordEmperor, damage: 5, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.ShieldMastery, "Shield Mastery", CharacterClasses.LordEmperor, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.CommandAttackInc, "Command Attack Inc", CharacterClasses.LordEmperor, damage: 20, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DarkSpiritStr3, "Dark Spirit Str (3)", CharacterClasses.LordEmperor, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.PetDurabilityStr, "Pet Durability Str", CharacterClasses.LordEmperor, damage: 17, skillType: SkillType.PassiveBoost);

        // Fist Master (Rage Fighter):
        this.CreateSkill(SkillNumber.KillingBlowStrengthener, "Killing Blow Strengthener", CharacterClasses.FistMaster, DamageType.Physical, 22, 2, manaConsumption: 10, elementalModifier: ElementalType.Earth);
        this.CreateSkill(SkillNumber.BeastUppercutStrengthener, "Beast Uppercut Strengthener", CharacterClasses.FistMaster, DamageType.Physical, 22, 2, manaConsumption: 10, elementalModifier: ElementalType.Fire);
        this.CreateSkill(SkillNumber.KillingBlowMastery, "Killing Blow Mastery", CharacterClasses.FistMaster, DamageType.Physical, 1, 2, manaConsumption: 10, elementalModifier: ElementalType.Earth);
        this.CreateSkill(SkillNumber.BeastUppercutMastery, "Beast Uppercut Mastery", CharacterClasses.FistMaster, DamageType.Physical, 1, 2, manaConsumption: 10, elementalModifier: ElementalType.Fire);
        this.CreateSkill(SkillNumber.WeaponMasteryFistMaster, "Weapon Mastery", CharacterClasses.FistMaster, damage: 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.ChainDriveStrengthener, "Chain Drive Strengthener", CharacterClasses.FistMaster, DamageType.Physical, 22, 4, 22, 22, 150, elementalModifier: ElementalType.Ice);
        this.CreateSkill(SkillNumber.DarkSideStrengthener, "Dark Side Strengthener", CharacterClasses.FistMaster, DamageType.Physical, 22, 4, manaConsumption: 84, levelRequirement: 180, elementalModifier: ElementalType.Wind);
        this.CreateSkill(SkillNumber.DragonRoarStrengthener, "Dragon Roar Strengthener", CharacterClasses.FistMaster, DamageType.Physical, 22, 3, 33, 60, 150, elementalModifier: ElementalType.Earth);
        this.CreateSkill(SkillNumber.EquippedWeaponStrengthener, "Equipped Weapon Strengthener", CharacterClasses.FistMaster, damage: 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DefSuccessRateIncPowUp, "Def SuccessRate IncPowUp", CharacterClasses.FistMaster, DamageType.Physical, 22, 7, 11, 55, 50, 30);
        this.CreateSkill(SkillNumber.EquippedWeaponMastery, "Equipped Weapon Mastery", CharacterClasses.FistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DefSuccessRateIncMastery, "DefSuccessRate IncMastery", CharacterClasses.FistMaster, DamageType.Physical, 22, 7, 12, 60, 50, 30);
        this.CreateSkill(SkillNumber.StaminaIncreaseStrengthener, "Stamina Increase Strengthener", CharacterClasses.FistMaster, DamageType.Physical, 5, 7, 11, 55, 80, 35);
        this.CreateSkill(SkillNumber.DurabilityReduction1FistMaster, "Durability Reduction (1)", CharacterClasses.FistMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreasePvPDefenseRate, "Increase PvP Defense Rate", CharacterClasses.FistMaster, damage: 29, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseMaximumSd, "Increase Maximum SD", CharacterClasses.FistMaster, damage: 33, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseManaRecoveryRate, "Increase Mana Recovery Rate", CharacterClasses.FistMaster, damage: 7, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreasePoisonResistance, "Increase Poison Resistance", CharacterClasses.FistMaster, damage: 1, elementalModifier: ElementalType.Poison, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DurabilityReduction2FistMaster, "Durability Reduction (2)", CharacterClasses.FistMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseSdRecoveryRate, "Increase SD Recovery Rate", CharacterClasses.FistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseHpRecoveryRate, "Increase HP Recovery Rate", CharacterClasses.FistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseLightningResistance, "Increase Lightning Resistance", CharacterClasses.FistMaster, damage: 1, elementalModifier: ElementalType.Lightning, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreasesDefense, "Increases Defense", CharacterClasses.FistMaster, damage: 35, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreasesAgRecoveryRate, "Increases AG Recovery Rate", CharacterClasses.FistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseIceResistance, "Increase Ice Resistance", CharacterClasses.FistMaster, damage: 1, elementalModifier: ElementalType.Ice, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DurabilityReduction3FistMaster, "Durability Reduction(3)", CharacterClasses.FistMaster, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseDefenseSuccessRate, "Increase Defense Success Rate", CharacterClasses.FistMaster, damage: 1, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseAttackSuccessRate, "Increase Attack Success Rate", CharacterClasses.FistMaster, damage: 30, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseMaximumHp, "Increase Maximum HP", CharacterClasses.FistMaster, damage: 34, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseMaximumMana, "Increase Maximum Mana", CharacterClasses.FistMaster, damage: 34, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreasePvPAttackRate, "Increase PvP Attack Rate", CharacterClasses.FistMaster, damage: 31, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.DecreaseMana, "Decrease Mana", CharacterClasses.FistMaster, damage: 18, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.RecoverSDfromMonsterKills, "Recover SD from Monster Kills", CharacterClasses.FistMaster, damage: 11, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.RecoverHPfromMonsterKills, "Recover HP from Monster Kills", CharacterClasses.FistMaster, damage: 6, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.IncreaseMinimumAttackPower, "Increase Minimum Attack Power", CharacterClasses.FistMaster, damage: 22, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.RecoverManaMonsterKills, "Recover Mana Monster Kills", CharacterClasses.FistMaster, damage: 6, skillType: SkillType.PassiveBoost);

        this.InitializeEffects();
        this.MapSkillsToEffects();
        this.InitializeMasterSkillData();
        this.CreateSpecialSummonMonsters();
        this.CreateSkillCombos();
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
        this.CreateSkill(SkillNumber.MultiShotStreng, "Multi-Shot Streng", CharacterClasses.HighElf, DamageType.Physical, 22, 6, 7, 11, 100, skillType: SkillType.AreaSkillExplicitHits);
        this.CreateSkill(SkillNumber.IllusionWingsAttPowUp, "Illusion Wings Att PowUp", CharacterClasses.HighElf, damage: 17, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.Cure, "Cure", CharacterClasses.HighElf, distance: 6, abilityConsumption: 10, manaConsumption: 72);
        this.CreateSkill(SkillNumber.PartyHealing, "Party Healing", CharacterClasses.HighElf, distance: 6, abilityConsumption: 12, manaConsumption: 66, energyRequirement: 100);
        this.CreateSkill(SkillNumber.PoisonArrow, "Poison Arrow", CharacterClasses.HighElf, DamageType.Physical, 27, 6, 27, 22, elementalModifier: ElementalType.Poison);
        this.CreateSkill(SkillNumber.SummonedMonsterStr3, "Summoned Monster Str (3)", CharacterClasses.HighElf, damage: 16, skillType: SkillType.PassiveBoost);
        this.CreateSkill(SkillNumber.PartyHealingStr, "Party Healing Str", CharacterClasses.HighElf, damage: 22, distance: 6, abilityConsumption: 13, manaConsumption: 72, energyRequirement: 100);
        this.CreateSkill(SkillNumber.Bless, "Bless", CharacterClasses.HighElf, distance: 6, abilityConsumption: 18, manaConsumption: 108, energyRequirement: 100);
        this.CreateSkill(SkillNumber.MultiShotMastery, "Multi-Shot Mastery", CharacterClasses.HighElf, DamageType.Physical, 1, 6, 8, 12, 100, skillType: SkillType.AreaSkillExplicitHits);
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
        this.CreateSkill(SkillNumber.ElectricSparkStreng, "Electric Spark Streng", CharacterClasses.LordEmperor, DamageType.Physical, 3, 10, 150, levelRequirement: 92, energyRequirement: 29, leadershipRequirement: 340);
        this.CreateSkill(SkillNumber.FireScreamMastery, "Fire Scream Mastery", CharacterClasses.LordEmperor, DamageType.Physical, 5, 6, 12, 49, 102, 32, 70);
        this.CreateSkill(SkillNumber.IronDefenseLordEmperor, "Iron Defense", CharacterClasses.LordEmperor, damage: 28, abilityConsumption: 29, manaConsumption: 64);
        this.CreateSkill(SkillNumber.CriticalDamageIncM, "Critical Damage Inc M", CharacterClasses.LordEmperor, damage: 1, abilityConsumption: 110, manaConsumption: 110, levelRequirement: 82, energyRequirement: 25, leadershipRequirement: 300);
        this.CreateSkill(SkillNumber.ChaoticDiseierStr, "Chaotic Diseier Str", CharacterClasses.LordEmperor, DamageType.Physical, 22, 6, 22, 75, 100, 16);
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

        bladeKnightCombo.Name = "Blade Knight Combo";

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
    }

    private void MapSkillsToEffects()
    {
        foreach (var effectOfSkill in EffectsOfSkills)
        {
            var skill = this.GameConfiguration.Skills.First(s => s.Number == (short)effectOfSkill.Key);
            var effect = this.GameConfiguration.MagicEffects.First(e => e.Number == (short)effectOfSkill.Value);
            skill.MagicEffectDef = effect;
        }
    }

    /// <summary>
    /// Initializes the master skill data.
    /// </summary>
    private void InitializeMasterSkillData()
    {
        // Roots:
        var leftRoot = this.Context.CreateNew<MasterSkillRoot>();
        leftRoot.Name = "Left (Common Skills)";
        this._masterSkillRoots.Add(1, leftRoot);
        this.GameConfiguration.MasterSkillRoots.Add(leftRoot);
        var middleRoot = this.Context.CreateNew<MasterSkillRoot>();
        middleRoot.Name = "Middle Root";
        this._masterSkillRoots.Add(2, middleRoot);
        this.GameConfiguration.MasterSkillRoots.Add(middleRoot);
        var rightRoot = this.Context.CreateNew<MasterSkillRoot>();
        rightRoot.Name = "Right Root";
        this._masterSkillRoots.Add(3, rightRoot);
        this.GameConfiguration.MasterSkillRoots.Add(rightRoot);

        // Universal
        this.AddPassiveMasterSkillDefinition(SkillNumber.DurabilityReduction1, Stats.ItemDurationIncrease, AggregateType.Multiplicate, Formula1204, 1, 1);
        this.AddPassiveMasterSkillDefinition(SkillNumber.PvPDefenceRateInc, Stats.DefenseRatePvp, AggregateType.AddRaw, Formula61408, 1, 1);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MaximumSDincrease, Stats.MaximumShield, AggregateType.AddRaw, Formula51173, 2, 1);
        this.AddPassiveMasterSkillDefinition(SkillNumber.AutomaticManaRecInc, Stats.ManaRecoveryMultiplier, AggregateType.AddRaw, FormulaRecoveryIncrease181, Formula181, 2, 1, SkillNumber.Undefined, SkillNumber.Undefined, 20);
        this.AddPassiveMasterSkillDefinition(SkillNumber.PoisonResistanceInc, Stats.PoisonResistance, AggregateType.AddRaw, Formula120Value, Formula120, 2, 1);
        this.AddPassiveMasterSkillDefinition(SkillNumber.DurabilityReduction2, Stats.ItemDurationIncrease, AggregateType.Multiplicate, Formula1204, 3, 1, SkillNumber.DurabilityReduction1);
        this.AddMasterSkillDefinition(SkillNumber.SdRecoverySpeedInc, SkillNumber.MaximumSDincrease, SkillNumber.Undefined, 1, 3, SkillNumber.Undefined, 20, Formula120);
        this.AddPassiveMasterSkillDefinition(SkillNumber.AutomaticHpRecInc, Stats.HealthRecoveryMultiplier, AggregateType.AddRaw, FormulaRecoveryIncrease120, Formula120, 3, 1, SkillNumber.AutomaticManaRecInc, SkillNumber.Undefined, 20);
        this.AddPassiveMasterSkillDefinition(SkillNumber.LightningResistanceInc, Stats.LightningResistance, AggregateType.AddRaw, Formula120Value, Formula120, 2, 1, requiredSkill1: SkillNumber.PoisonResistanceInc);
        this.AddPassiveMasterSkillDefinition(SkillNumber.DefenseIncrease, Stats.DefenseBase, AggregateType.AddRaw, Formula6020, 4, 1);
        this.AddPassiveMasterSkillDefinition(SkillNumber.AutomaticAgRecInc, Stats.AbilityRecoveryMultiplier, AggregateType.AddRaw, FormulaRecoveryIncrease120, Formula120, 4, 1, SkillNumber.AutomaticHpRecInc, SkillNumber.Undefined, 20);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IceResistanceIncrease, Stats.IceResistance, AggregateType.AddRaw, Formula120Value, Formula120, 2, 1, requiredSkill1: SkillNumber.LightningResistanceInc);
        this.AddPassiveMasterSkillDefinition(SkillNumber.DurabilityReduction3, Stats.ItemDurationIncrease, AggregateType.Multiplicate, Formula1204, 5, 1, SkillNumber.DurabilityReduction2);
        this.AddPassiveMasterSkillDefinition(SkillNumber.DefenseSuccessRateInc, Stats.DefenseRatePvm, AggregateType.AddRaw, Formula120, 5, 1, SkillNumber.DefenseIncrease);

        // DK
        this.AddPassiveMasterSkillDefinition(SkillNumber.AttackSuccRateInc, Stats.AttackRatePvm, AggregateType.AddRaw, Formula51173, 1, 2);
        this.AddMasterSkillDefinition(SkillNumber.CycloneStrengthener, SkillNumber.Cyclone, SkillNumber.Undefined, 2, 2, SkillNumber.Cyclone, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.SlashStrengthener, SkillNumber.Slash, SkillNumber.Undefined, 2, 2, SkillNumber.Slash, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.FallingSlashStreng, SkillNumber.FallingSlash, SkillNumber.Undefined, 2, 2, SkillNumber.FallingSlash, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.LungeStrengthener, SkillNumber.Lunge, SkillNumber.Undefined, 2, 2, SkillNumber.Lunge, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.TwistingSlashStreng, SkillNumber.TwistingSlash, SkillNumber.Undefined, 2, 3, SkillNumber.TwistingSlash, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.RagefulBlowStreng, SkillNumber.RagefulBlow, SkillNumber.Undefined, 2, 3, SkillNumber.RagefulBlow, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.TwistingSlashMastery, SkillNumber.TwistingSlashStreng, SkillNumber.Undefined, 2, 4, SkillNumber.TwistingSlash, 20, Formula120);
        this.AddMasterSkillDefinition(SkillNumber.RagefulBlowMastery, SkillNumber.RagefulBlowStreng, SkillNumber.Undefined, 2, 4, SkillNumber.RagefulBlow, 20, Formula120);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MaximumLifeIncrease, Stats.MaximumHealth, AggregateType.AddRaw, Formula10235, 4, 2);
        this.AddPassiveMasterSkillDefinition(SkillNumber.WeaponMasteryBladeMaster, Stats.PhysicalBaseDmg, AggregateType.AddRaw, Formula502, 4, 2);
        this.AddMasterSkillDefinition(SkillNumber.DeathStabStrengthener, SkillNumber.DeathStab, SkillNumber.Undefined, 2, 5, SkillNumber.DeathStab, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.StrikeofDestrStr, SkillNumber.StrikeofDestruction, SkillNumber.Undefined, 2, 5, SkillNumber.StrikeofDestruction, 20, Formula502);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MaximumManaIncrease, Stats.MaximumMana, AggregateType.AddRaw, Formula10235, 5, 2, SkillNumber.MaximumLifeIncrease);
        this.AddPassiveMasterSkillDefinition(SkillNumber.PvPAttackRate, Stats.AttackRatePvp, AggregateType.AddRaw, Formula81877, 1, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.TwoHandedSwordStrengthener, Stats.TwoHandedSwordBonusBaseDamage, AggregateType.AddRaw, Formula883, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.OneHandedSwordStrengthener, Stats.OneHandedSwordBonusBaseDamage, AggregateType.AddRaw, Formula502, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MaceStrengthener, Stats.MaceBonusBaseDamage, AggregateType.AddRaw, Formula632, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.SpearStrengthener, Stats.SpearBonusBaseDamage, AggregateType.AddRaw, Formula632, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.TwoHandedSwordMaster, Stats.TwoHandedSwordBonusBaseDamage, AggregateType.AddRaw, Formula1154, 3, 3, SkillNumber.TwoHandedSwordStrengthener); // todo: this is only pvp damage
        this.AddPassiveMasterSkillDefinition(SkillNumber.OneHandedSwordMaster, Stats.AttackSpeed, AggregateType.AddRaw, Formula1, 3, 3, SkillNumber.OneHandedSwordStrengthener, SkillNumber.Undefined, 10);

        // todo: Probability of stunning the target for 2 seconds according to the assigned Skill Level while using a Mace.
        this.AddMasterSkillDefinition(SkillNumber.MaceMastery, SkillNumber.MaceStrengthener, SkillNumber.Undefined, 3, 3, SkillNumber.Undefined, 20, Formula120);

        // todo: Increases the probability of Double Damage while using a Spear according to the assigned Skill Level.
        this.AddMasterSkillDefinition(SkillNumber.SpearMastery, SkillNumber.SpearStrengthener, SkillNumber.Undefined, 3, 3, SkillNumber.Undefined, 20, Formula120);

        this.AddMasterSkillDefinition(SkillNumber.SwellLifeStrengt, SkillNumber.SwellLife, SkillNumber.Undefined, 3, 4, SkillNumber.SwellLife, 20, Formula181);
        this.AddPassiveMasterSkillDefinition(SkillNumber.ManaReduction, Stats.ManaUsageReduction, AggregateType.AddRaw, Formula722Value, Formula722, 4, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MonsterAttackSdInc, Stats.ShieldAfterMonsterKillMultiplier, AggregateType.AddFinal, Formula914, 4, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MonsterAttackLifeInc, Stats.HealthAfterMonsterKillMultiplier, AggregateType.AddFinal, Formula4319, 4, 3);
        this.AddMasterSkillDefinition(SkillNumber.SwellLifeProficiency, SkillNumber.SwellLifeStrengt, SkillNumber.Undefined, 3, 5, SkillNumber.SwellLife, 20, Formula181);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MinimumAttackPowerInc, Stats.MinimumPhysBaseDmg, AggregateType.AddRaw, Formula502, 5, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MonsterAttackManaInc, Stats.ManaAfterMonsterKillMultiplier, AggregateType.AddFinal, Formula4319, 5, 3, SkillNumber.MonsterAttackLifeInc);

        // DW
        this.AddMasterSkillDefinition(SkillNumber.FlameStrengthener, SkillNumber.Flame, SkillNumber.Undefined, 2, 2, SkillNumber.Flame, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.LightningStrengthener, SkillNumber.Lightning, SkillNumber.Undefined, 2, 2, SkillNumber.Lightning, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.ExpansionofWizStreng, SkillNumber.ExpansionofWizardry, SkillNumber.Undefined, 2, 2, SkillNumber.ExpansionofWizardry, 20, Formula120Value, Formula120, Stats.MaximumWizBaseDmg, AggregateType.Multiplicate);
        this.AddMasterSkillDefinition(SkillNumber.InfernoStrengthener, SkillNumber.Inferno, SkillNumber.FlameStrengthener, 2, 3, SkillNumber.Inferno, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.BlastStrengthener, SkillNumber.Cometfall, SkillNumber.LightningStrengthener, 2, 3, SkillNumber.Cometfall, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.ExpansionofWizMas, SkillNumber.ExpansionofWizStreng, SkillNumber.Undefined, 2, 3, SkillNumber.ExpansionofWizardry, 20, Formula120Value, Formula120, targetAttribute: Stats.CriticalDamageChance, AggregateType.Multiplicate);
        this.AddMasterSkillDefinition(SkillNumber.PoisonStrengthener, SkillNumber.Poison, SkillNumber.Undefined, 2, 3, SkillNumber.Poison, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.EvilSpiritStreng, SkillNumber.EvilSpirit, SkillNumber.Undefined, 2, 4, SkillNumber.EvilSpirit, 20, Formula502);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MagicMasteryGrandMaster, Stats.WizardryBaseDmg, AggregateType.AddRaw, Formula502, 4, 2, SkillNumber.EvilSpiritStreng);
        this.AddMasterSkillDefinition(SkillNumber.DecayStrengthener, SkillNumber.Decay, SkillNumber.PoisonStrengthener, 2, 4, SkillNumber.Decay, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.HellfireStrengthener, SkillNumber.Hellfire, SkillNumber.Undefined, 2, 5, SkillNumber.Hellfire, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.IceStrengthener, SkillNumber.Ice, SkillNumber.Undefined, 2, 5, SkillNumber.Ice, 20, Formula632);
        this.AddPassiveMasterSkillDefinition(SkillNumber.OneHandedStaffStrengthener, Stats.WizardryBaseDmg, AggregateType.AddRaw, Formula502, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.TwoHandedStaffStrengthener, Stats.WizardryBaseDmg, AggregateType.AddRaw, Formula883, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.ShieldStrengthenerGrandMaster, Stats.BonusDefenseWithShield, AggregateType.AddRaw, Formula803, 2, 3); // todo: check if this is correct
        this.AddPassiveMasterSkillDefinition(SkillNumber.OneHandedStaffMaster, Stats.AttackSpeed, AggregateType.AddRaw, Formula1, 3, 3, SkillNumber.OneHandedStaffStrengthener, SkillNumber.Undefined, 10);
        this.AddPassiveMasterSkillDefinition(SkillNumber.TwoHandedStaffMaster, Stats.TwoHandedStaffBonusBaseDamage, AggregateType.AddRaw, Formula1154, 3, 3, SkillNumber.TwoHandedStaffStrengthener); // todo: only pvp
        this.AddPassiveMasterSkillDefinition(SkillNumber.ShieldMasteryGrandMaster, Stats.BonusDefenseWithShield, AggregateType.AddRaw, Formula1204, 3, 3, SkillNumber.ShieldStrengthenerGrandMaster);
        this.AddMasterSkillDefinition(SkillNumber.SoulBarrierStrength, SkillNumber.SoulBarrier, SkillNumber.Undefined, 3, 4, SkillNumber.SoulBarrier, 20, Formula181);
        this.AddMasterSkillDefinition(SkillNumber.SoulBarrierProficie, SkillNumber.SoulBarrierStrength, SkillNumber.Undefined, 3, 5, SkillNumber.SoulBarrier, 20, Formula803);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MinimumWizardryInc, Stats.MinimumWizBaseDmg, AggregateType.AddRaw, Formula502, 5, 3);

        // ELF
        this.AddMasterSkillDefinition(SkillNumber.HealStrengthener, SkillNumber.Heal, SkillNumber.Undefined, 2, 2, SkillNumber.Heal, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.TripleShotStrengthener, SkillNumber.TripleShot, SkillNumber.Undefined, 2, 2, SkillNumber.TripleShot, 20, Formula502);
        this.AddPassiveMasterSkillDefinition(SkillNumber.SummonedMonsterStr1, Stats.SummonedMonsterHealthIncrease, AggregateType.AddRaw, Formula6020Value, 2, 2, SkillNumber.SummonGoblin);
        this.AddMasterSkillDefinition(SkillNumber.PenetrationStrengthener, SkillNumber.Penetration, SkillNumber.Undefined, 2, 3, SkillNumber.Penetration, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.DefenseIncreaseStr, SkillNumber.GreaterDefense, SkillNumber.Undefined, 2, 3, SkillNumber.GreaterDefense, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.TripleShotMastery, SkillNumber.TripleShotStrengthener, SkillNumber.Undefined, 2, 3, SkillNumber.TripleShot, 10, Formula1WhenComplete);
        this.AddPassiveMasterSkillDefinition(SkillNumber.SummonedMonsterStr2, Stats.SummonedMonsterDefenseIncrease, AggregateType.AddRaw, Formula6020, 2, 3, SkillNumber.SummonGoblin);
        this.AddMasterSkillDefinition(SkillNumber.AttackIncreaseStr, SkillNumber.GreaterDamage, SkillNumber.Undefined, 2, 4, SkillNumber.GreaterDamage, 20, Formula502);
        this.AddPassiveMasterSkillDefinition(SkillNumber.WeaponMasteryHighElf, Stats.PhysicalBaseDmg, AggregateType.AddRaw, Formula502, 4, 2);
        this.AddMasterSkillDefinition(SkillNumber.AttackIncreaseMastery, SkillNumber.AttackIncreaseStr, SkillNumber.Undefined, 2, 5, SkillNumber.GreaterDamage, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.DefenseIncreaseMastery, SkillNumber.DefenseIncreaseStr, SkillNumber.Undefined, 2, 5, SkillNumber.GreaterDefense, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.IceArrowStrengthener, SkillNumber.IceArrow, SkillNumber.Undefined, 2, 5, SkillNumber.IceArrow, 20, Formula502);
        this.AddPassiveMasterSkillDefinition(SkillNumber.BowStrengthener, Stats.BowBonusBaseDamage, AggregateType.AddRaw, Formula502, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.CrossbowStrengthener, Stats.CrossBowBonusBaseDamage, AggregateType.AddRaw, Formula632, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.ShieldStrengthenerHighElf, Stats.BonusDefenseWithShield, AggregateType.AddRaw, Formula803, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.BowMastery, Stats.AttackSpeed, AggregateType.AddRaw, Formula1, 3, 3, SkillNumber.BowStrengthener, SkillNumber.Undefined, 10);
        this.AddPassiveMasterSkillDefinition(SkillNumber.CrossbowMastery, Stats.CrossBowBonusBaseDamage, AggregateType.AddRaw, Formula1154, 3, 3, SkillNumber.CrossbowStrengthener); // todo only pvp
        this.AddPassiveMasterSkillDefinition(SkillNumber.ShieldMasteryHighElf, Stats.BonusDefenseWithShield, AggregateType.AddRaw, Formula1806, 3, 3, SkillNumber.ShieldStrengthenerHighElf);
        this.AddMasterSkillDefinition(SkillNumber.InfinityArrowStr, SkillNumber.InfinityArrow, SkillNumber.Undefined, 3, 5, SkillNumber.InfinityArrow, 20, $"{Formula120} / 100", Formula120, Stats.AttackDamageIncrease, AggregateType.AddRaw);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MinimumAttPowerInc, Stats.MinimumPhysBaseDmg, AggregateType.AddRaw, Formula502, 5, 3);

        // SUM
        this.AddMasterSkillDefinition(SkillNumber.FireTomeStrengthener, SkillNumber.Undefined, SkillNumber.Undefined, 2, 2, SkillNumber.Undefined, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.WindTomeStrengthener, SkillNumber.Undefined, SkillNumber.Undefined, 2, 2, SkillNumber.Undefined, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.LightningTomeStren, SkillNumber.Undefined, SkillNumber.Undefined, 2, 2, SkillNumber.Undefined, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.FireTomeMastery, SkillNumber.FireTomeStrengthener, SkillNumber.Undefined, 2, 3, SkillNumber.Undefined, 20, Formula181);
        this.AddMasterSkillDefinition(SkillNumber.WindTomeMastery, SkillNumber.WindTomeStrengthener, SkillNumber.Undefined, 2, 3, SkillNumber.Undefined, 20, Formula120);
        this.AddMasterSkillDefinition(SkillNumber.LightningTomeMastery, SkillNumber.LightningTomeStren, SkillNumber.Undefined, 2, 3, SkillNumber.Undefined, 20, Formula181);
        this.AddMasterSkillDefinition(SkillNumber.SleepStrengthener, SkillNumber.Sleep, SkillNumber.Undefined, 2, 3, SkillNumber.Sleep, 20, Formula120);
        this.AddMasterSkillDefinition(SkillNumber.ChainLightningStr, SkillNumber.ChainLightning, SkillNumber.Undefined, 2, 4, SkillNumber.ChainLightning, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.LightningShockStr, SkillNumber.LightningShock, SkillNumber.Undefined, 2, 4, SkillNumber.LightningShock, 20, Formula502);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MagicMasterySummoner, Stats.WizardryBaseDmg, AggregateType.AddRaw, Formula502, 5, 2); // todo? curse AND wiz bonus,
        this.AddMasterSkillDefinition(SkillNumber.DrainLifeStrengthener, SkillNumber.DrainLife, SkillNumber.Undefined, 2, 5, SkillNumber.DrainLife, 20, Formula502);
        this.AddPassiveMasterSkillDefinition(SkillNumber.StickStrengthener, Stats.StickBonusBaseDamage, AggregateType.AddRaw, Formula502, 2, 3);
        this.AddMasterSkillDefinition(SkillNumber.OtherWorldTomeStreng, SkillNumber.Undefined, SkillNumber.Undefined, 3, 2, SkillNumber.Undefined, 20, Formula632);
        this.AddPassiveMasterSkillDefinition(SkillNumber.StickMastery, Stats.StickBonusBaseDamage, AggregateType.AddRaw, Formula1154, 3, 3, SkillNumber.StickStrengthener); // todo: only PVP
        this.AddPassiveMasterSkillDefinition(SkillNumber.OtherWorldTomeMastery, Stats.AttackSpeed, AggregateType.AddRaw, Formula1, 3, 3, SkillNumber.OtherWorldTomeStreng, SkillNumber.Undefined, 10);
        this.AddMasterSkillDefinition(SkillNumber.BerserkerStrengthener, SkillNumber.Berserker, SkillNumber.Undefined, 3, 4, SkillNumber.Berserker, 20, Formula181);
        this.AddMasterSkillDefinition(SkillNumber.BerserkerProficiency, SkillNumber.BerserkerStrengthener, SkillNumber.Undefined, 3, 5, SkillNumber.Undefined, 20, Formula181);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MinimumWizCurseInc, Stats.MinimumCurseBaseDmg, AggregateType.AddRaw, Formula502, 5, 3);

        // MG
        this.AddMasterSkillDefinition(SkillNumber.CycloneStrengthenerDuelMaster, SkillNumber.Cyclone, SkillNumber.Undefined, 2, 2, SkillNumber.Cyclone, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.LightningStrengthenerDuelMaster, SkillNumber.Lightning, SkillNumber.Undefined, 2, 2, SkillNumber.Lightning, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.TwistingSlashStrengthenerDuelMaster, SkillNumber.TwistingSlash, SkillNumber.Undefined, 2, 2, SkillNumber.TwistingSlash, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.PowerSlashStreng, SkillNumber.PowerSlash, SkillNumber.Undefined, 2, 2, SkillNumber.PowerSlash, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.FlameStrengthenerDuelMaster, SkillNumber.Flame, SkillNumber.Undefined, 2, 3, SkillNumber.Flame, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.BlastStrengthenerDuelMaster, SkillNumber.Cometfall, SkillNumber.LightningStrengthenerDuelMaster, 2, 3, SkillNumber.Cometfall, 20, Formula502);
        this.AddPassiveMasterSkillDefinition(SkillNumber.WeaponMasteryDuelMaster, Stats.PhysicalBaseDmg, AggregateType.AddRaw, Formula502, 3, 2, SkillNumber.TwistingSlashStrengthenerDuelMaster, SkillNumber.PowerSlashStreng);
        this.AddMasterSkillDefinition(SkillNumber.InfernoStrengthenerDuelMaster, SkillNumber.Inferno, SkillNumber.FlameStrengthenerDuelMaster, 2, 4, SkillNumber.Inferno, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.EvilSpiritStrengthenerDuelMaster, SkillNumber.EvilSpirit, SkillNumber.Undefined, 2, 4, SkillNumber.EvilSpirit, 20, Formula502);
        this.AddPassiveMasterSkillDefinition(SkillNumber.MagicMasteryDuelMaster, Stats.WizardryBaseDmg, AggregateType.AddRaw, Formula502, 4, 2, SkillNumber.EvilSpiritStrengthenerDuelMaster);
        this.AddMasterSkillDefinition(SkillNumber.IceStrengthenerDuelMaster, SkillNumber.Ice, SkillNumber.Undefined, 2, 5, SkillNumber.Ice, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.BloodAttackStrengthen, SkillNumber.FireSlash, SkillNumber.Undefined, 2, 5, SkillNumber.FireSlash, 20, Formula502);

        // DL
        this.AddMasterSkillDefinition(SkillNumber.FireBurstStreng, SkillNumber.FireBurst, SkillNumber.Undefined, 2, 2, SkillNumber.FireBurst, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.ForceWaveStreng, SkillNumber.Force, SkillNumber.Undefined, 2, 2, SkillNumber.Force, 20, Formula632);
        this.AddPassiveMasterSkillDefinition(SkillNumber.DarkHorseStreng1, Stats.BonusDefenseWithHorse, AggregateType.AddRaw, Formula1204, 2, 2);
        this.AddMasterSkillDefinition(SkillNumber.CriticalDmgIncPowUp, SkillNumber.IncreaseCriticalDamage, SkillNumber.Undefined, 2, 3, SkillNumber.IncreaseCriticalDamage, 20, Formula632);
        this.AddMasterSkillDefinition(SkillNumber.EarthshakeStreng, SkillNumber.Earthshake, SkillNumber.DarkHorseStreng1, 2, 3, SkillNumber.Earthshake, 20, Formula502);
        this.AddPassiveMasterSkillDefinition(SkillNumber.WeaponMasteryLordEmperor, Stats.PhysicalBaseDmg, AggregateType.AddRaw, Formula502, 3, 2);
        this.AddMasterSkillDefinition(SkillNumber.FireBurstMastery, SkillNumber.FireBurstStreng, SkillNumber.Undefined, 2, 4, SkillNumber.FireBurst, 20, Formula120);
        this.AddMasterSkillDefinition(SkillNumber.CritDmgIncPowUp2, SkillNumber.CriticalDmgIncPowUp, SkillNumber.Undefined, 2, 4, SkillNumber.IncreaseCriticalDamage, 20, Formula803);
        this.AddMasterSkillDefinition(SkillNumber.EarthshakeMastery, SkillNumber.EarthshakeStreng, SkillNumber.Undefined, 2, 4, SkillNumber.Earthshake, 20, Formula120);
        this.AddMasterSkillDefinition(SkillNumber.CritDmgIncPowUp3, SkillNumber.CritDmgIncPowUp2, SkillNumber.Undefined, 2, 5, SkillNumber.IncreaseCriticalDamage, 20, Formula181);
        this.AddMasterSkillDefinition(SkillNumber.FireScreamStren, SkillNumber.FireScream, SkillNumber.Undefined, 2, 5, SkillNumber.FireScream, 20, Formula502);
        this.AddPassiveMasterSkillDefinition(SkillNumber.DarkSpiritStr, Stats.RavenBaseDamage, AggregateType.AddRaw, Formula632, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.ScepterStrengthener, Stats.ScepterBonusBaseDamage, AggregateType.AddRaw, Formula502, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.ShieldStrengthenerLordEmperor, Stats.BonusDefenseWithShield, AggregateType.AddRaw, Formula803, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.UseScepterPetStr, Stats.ScepterPetBonusBaseDamage, AggregateType.AddRaw, Formula632, 2, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.DarkSpiritStr2, Stats.RavenCriticalDamageChanceBonus, AggregateType.AddRaw, $"{Formula181} / 100", Formula181, 3, 3, SkillNumber.DarkSpiritStr);
        this.AddPassiveMasterSkillDefinition(SkillNumber.ScepterMastery, Stats.ScepterBonusBaseDamage, AggregateType.AddRaw, Formula1154, 3, 3, SkillNumber.ScepterStrengthener); // todo pvp
        this.AddPassiveMasterSkillDefinition(SkillNumber.ShieldMastery, Stats.BonusDefenseWithShield, AggregateType.AddRaw, Formula1204, 3, 3, SkillNumber.ShieldStrengthenerLordEmperor);
        this.AddPassiveMasterSkillDefinition(SkillNumber.CommandAttackInc, Stats.BonusDefenseWithScepterCmdDiv, AggregateType.AddRaw, $"1 / ({Formula3822})", Formula3822, 3, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.DarkSpiritStr3, Stats.RavenExcDamageChanceBonus, AggregateType.AddRaw, $"{Formula120} / 100", Formula120, 5, 3, SkillNumber.DarkSpiritStr2);
        this.AddPassiveMasterSkillDefinition(SkillNumber.PetDurabilityStr, Stats.PetDurationIncrease, AggregateType.Multiplicate, Formula1204, 5, 3);

        // RF
        this.AddPassiveMasterSkillDefinition(SkillNumber.DurabilityReduction1FistMaster, Stats.ItemDurationIncrease, AggregateType.Multiplicate, Formula1204, 1, 1);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreasePvPDefenseRate, Stats.DefenseRatePvp, AggregateType.AddRaw, Formula25587, 1, 1);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseMaximumSd, Stats.MaximumShield, AggregateType.AddRaw, Formula30704, 2, 1);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseManaRecoveryRate, Stats.ManaRecoveryMultiplier, AggregateType.AddRaw, FormulaRecoveryIncrease181, Formula181, 2, 1, SkillNumber.Undefined, SkillNumber.Undefined, 20);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreasePoisonResistance, Stats.PoisonResistance, AggregateType.AddRaw, Formula120Value, Formula120, 2, 1);
        this.AddPassiveMasterSkillDefinition(SkillNumber.DurabilityReduction2FistMaster, Stats.ItemDurationIncrease, AggregateType.Multiplicate, Formula1204, 3, 1, SkillNumber.DurabilityReduction1FistMaster);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseSdRecoveryRate, Stats.ShieldRecoveryMultiplier, AggregateType.AddRaw, FormulaRecoveryIncrease120, Formula120, 3, 1, SkillNumber.IncreaseMaximumSd, SkillNumber.Undefined, 20);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseHpRecoveryRate, Stats.HealthRecoveryMultiplier, AggregateType.AddRaw, FormulaRecoveryIncrease120, Formula120, 3, 1, SkillNumber.IncreaseManaRecoveryRate, SkillNumber.Undefined, 20);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseLightningResistance, Stats.LightningResistance, AggregateType.AddRaw, Formula120Value, Formula120, 3, 1, requiredSkill1: SkillNumber.IncreasePoisonResistance);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreasesDefense, Stats.DefenseBase, AggregateType.AddRaw, Formula3371, 4, 1);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreasesAgRecoveryRate, Stats.AbilityRecoveryMultiplier, AggregateType.AddRaw, FormulaRecoveryIncrease120, Formula120, 4, 1, SkillNumber.IncreaseHpRecoveryRate, SkillNumber.Undefined, 20);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseIceResistance, Stats.IceResistance, AggregateType.AddRaw, Formula120Value, Formula120, 4, 1, requiredSkill1: SkillNumber.IncreaseLightningResistance);
        this.AddPassiveMasterSkillDefinition(SkillNumber.DurabilityReduction3FistMaster, Stats.ItemDurationIncrease, AggregateType.Multiplicate, Formula1204, 5, 1, SkillNumber.DurabilityReduction2FistMaster);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseDefenseSuccessRate, Stats.DefenseRatePvm, AggregateType.Multiplicate, FormulaIncreaseMultiplicator120, Formula120, 5, 1, SkillNumber.IncreasesDefense, SkillNumber.Undefined, 20);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseAttackSuccessRate, Stats.AttackRatePvm, AggregateType.AddRaw, Formula20469, 1, 2);
        this.AddMasterSkillDefinition(SkillNumber.KillingBlowStrengthener, SkillNumber.Undefined, SkillNumber.Undefined, 2, 2, SkillNumber.KillingBlow, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.BeastUppercutStrengthener, SkillNumber.Undefined, SkillNumber.Undefined, 2, 2, SkillNumber.BeastUppercut, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.KillingBlowMastery, SkillNumber.KillingBlowStrengthener, SkillNumber.Undefined, 2, 3, SkillNumber.KillingBlow, 20, Formula120);
        this.AddMasterSkillDefinition(SkillNumber.BeastUppercutMastery, SkillNumber.BeastUppercutStrengthener, SkillNumber.Undefined, 2, 3, SkillNumber.BeastUppercut, 20, Formula120);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseMaximumHp, Stats.MaximumHealth, AggregateType.AddRaw, Formula5418, 4, 2);
        this.AddPassiveMasterSkillDefinition(SkillNumber.WeaponMasteryFistMaster, Stats.PhysicalBaseDmg, AggregateType.AddRaw, Formula502, 4, 2);
        this.AddMasterSkillDefinition(SkillNumber.ChainDriveStrengthener, SkillNumber.ChainDrive, SkillNumber.Undefined, 2, 5, SkillNumber.ChainDrive, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.DarkSideStrengthener, SkillNumber.DarkSide, SkillNumber.Undefined, 2, 5, SkillNumber.DarkSide, 20, Formula502);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseMaximumMana, Stats.MaximumMana, AggregateType.AddRaw, Formula5418, 5, 2, SkillNumber.IncreaseMaximumHp);
        this.AddMasterSkillDefinition(SkillNumber.DragonRoarStrengthener, SkillNumber.DragonRoar, SkillNumber.Undefined, 2, 5, SkillNumber.DragonRoar, 20, Formula502);
        this.AddPassiveMasterSkillDefinition(SkillNumber.IncreasePvPAttackRate, Stats.AttackRatePvp, AggregateType.AddRaw, Formula32751, 1, 3);
        this.AddPassiveMasterSkillDefinition(SkillNumber.EquippedWeaponStrengthener, Stats.GloveWeaponBonusBaseDamage, AggregateType.AddRaw, Formula502, 2, 3);
        this.AddMasterSkillDefinition(SkillNumber.DefSuccessRateIncPowUp, SkillNumber.IncreaseBlock, SkillNumber.Undefined, 3, 2, SkillNumber.IncreaseBlock, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.EquippedWeaponMastery, SkillNumber.EquippedWeaponStrengthener, SkillNumber.Undefined, 3, 3, SkillNumber.Undefined, 20, Formula120);
        this.AddMasterSkillDefinition(SkillNumber.DefSuccessRateIncMastery, SkillNumber.DefSuccessRateIncPowUp, SkillNumber.Undefined, 3, 3, SkillNumber.IncreaseBlock, 20, Formula502);
        this.AddMasterSkillDefinition(SkillNumber.StaminaIncreaseStrengthener, SkillNumber.IncreaseHealth, SkillNumber.Undefined, 3, 4, SkillNumber.IncreaseHealth, 20, Formula1154);
        this.AddMasterSkillDefinition(SkillNumber.DecreaseMana, SkillNumber.Undefined, SkillNumber.Undefined, 3, 4, SkillNumber.Undefined, 20, Formula722);
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

    private void AddMasterSkillDefinition(SkillNumber skillNumber, SkillNumber requiredSkill1, SkillNumber requiredSkill2, byte root, byte rank, SkillNumber regularSkill, byte maximumLevel, string valueFormula)
    {
        this.AddMasterSkillDefinition(skillNumber, requiredSkill1, requiredSkill2, root, rank, regularSkill, maximumLevel, valueFormula, valueFormula, null, AggregateType.AddRaw);
    }

    private void AddMasterSkillDefinition(SkillNumber skillNumber, SkillNumber requiredSkill1, SkillNumber requiredSkill2, byte root, byte rank, SkillNumber regularSkill, byte maximumLevel, string valueFormula, string displayValueFormula, AttributeDefinition? targetAttribute, AggregateType aggregateType)
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
            skill.ImplicitTargetRange = replacedSkill.ImplicitTargetRange;
            skill.MovesTarget = replacedSkill.MovesTarget;
            skill.MovesToTarget = replacedSkill.MovesToTarget;
            skill.SkillType = replacedSkill.SkillType;
            skill.Target = replacedSkill.Target;
            skill.TargetRestriction = replacedSkill.TargetRestriction;
            skill.MagicEffectDef = replacedSkill.MagicEffectDef;
        }
    }

    private void CreateSpecialSummonMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 150;
            monster.Designation = "Bali";
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
            monster.Designation = "Soldier";
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
