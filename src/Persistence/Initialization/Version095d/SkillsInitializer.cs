// <copyright file="SkillsInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d;

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
    private static readonly IDictionary<SkillNumber, MagicEffectNumber> EffectsOfSkills = new Dictionary<SkillNumber, MagicEffectNumber>
    {
        { SkillNumber.Defense, MagicEffectNumber.ShieldSkill },
        { SkillNumber.GreaterDefense, MagicEffectNumber.GreaterDefense },
        { SkillNumber.GreaterDamage, MagicEffectNumber.GreaterDamage },
        { SkillNumber.Heal, MagicEffectNumber.Heal },
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="SkillsInitializer"/> class.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public SkillsInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    public override void Initialize()
    {
        this.CreateSkill(SkillNumber.Poison, "Poison", CharacterClasses.DarkWizard | CharacterClasses.MagicGladiator, DamageType.Wizardry, 12, 6, manaConsumption: 42, energyRequirement: 140, elementalModifier: ElementalType.Poison);
        this.CreateSkill(SkillNumber.Meteorite, "Meteorite", CharacterClasses.DarkWizard | CharacterClasses.MagicGladiator, DamageType.Wizardry, 21, 6, manaConsumption: 12, energyRequirement: 104, elementalModifier: ElementalType.Earth);
        this.CreateSkill(SkillNumber.Lightning, "Lightning", CharacterClasses.DarkWizard | CharacterClasses.MagicGladiator, DamageType.Wizardry, 17, 6, manaConsumption: 15, energyRequirement: 72, elementalModifier: ElementalType.Lightning);
        this.CreateSkill(SkillNumber.FireBall, "Fire Ball", CharacterClasses.DarkWizard | CharacterClasses.MagicGladiator, DamageType.Wizardry, 8, 6, manaConsumption: 3, energyRequirement: 40, elementalModifier: ElementalType.Fire);
        this.CreateSkill(SkillNumber.Flame, "Flame", CharacterClasses.DarkWizard | CharacterClasses.MagicGladiator, DamageType.Wizardry, 25, 6, manaConsumption: 50, energyRequirement: 160, elementalModifier: ElementalType.Fire, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.Flame, false, default, default, default, true, TimeSpan.Zero, TimeSpan.FromMilliseconds(500), 0, 2, default, 0.5f, targetAreaDiameter: 2, useTargetAreaFilter: true);
        this.CreateSkill(SkillNumber.Teleport, "Teleport", CharacterClasses.DarkWizard, DamageType.Wizardry, distance: 6, manaConsumption: 30, energyRequirement: 88, skillType: SkillType.Other);
        this.CreateSkill(SkillNumber.Ice, "Ice", CharacterClasses.DarkWizard | CharacterClasses.MagicGladiator, DamageType.Wizardry, 10, 6, manaConsumption: 38, energyRequirement: 120, elementalModifier: ElementalType.Ice);
        this.CreateSkill(SkillNumber.Twister, "Twister", CharacterClasses.DarkWizard | CharacterClasses.MagicGladiator, DamageType.Wizardry, 35, 6, manaConsumption: 60, energyRequirement: 180, elementalModifier: ElementalType.Wind, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.Twister, true, 1.5f, 1.5f, 4f, true, TimeSpan.FromMilliseconds(300), TimeSpan.FromMilliseconds(1000), 0, 2, default, 0.7f);
        this.CreateSkill(SkillNumber.EvilSpirit, "Evil Spirit", CharacterClasses.DarkWizard | CharacterClasses.MagicGladiator, DamageType.Wizardry, 45, 6, manaConsumption: 90, energyRequirement: 220, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.EvilSpirit, false, default, default, default, true, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(1000), 0, 2, default, 0.7f);
        this.CreateSkill(SkillNumber.Hellfire, "Hellfire", CharacterClasses.DarkWizard | CharacterClasses.MagicGladiator, DamageType.Wizardry, 120, manaConsumption: 160, energyRequirement: 260, elementalModifier: ElementalType.Fire, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.PowerWave, "Power Wave", CharacterClasses.DarkWizard | CharacterClasses.MagicGladiator, DamageType.Wizardry, 14, 6, manaConsumption: 5, energyRequirement: 56);
        this.CreateSkill(SkillNumber.AquaBeam, "Aqua Beam", CharacterClasses.DarkWizard | CharacterClasses.MagicGladiator, DamageType.Wizardry, 80, 6, manaConsumption: 140, energyRequirement: 345, elementalModifier: ElementalType.Water, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.AquaBeam, true, 1.5f, 1.5f, 8f);
        this.CreateSkill(SkillNumber.Cometfall, "Cometfall", CharacterClasses.DarkWizard | CharacterClasses.MagicGladiator, DamageType.Wizardry, 70, 3, manaConsumption: 150, energyRequirement: 436, elementalModifier: ElementalType.Lightning);
        this.AddAreaSkillSettings(SkillNumber.Cometfall, false, default, default, default, targetAreaDiameter: 2, useTargetAreaFilter: true);
        this.CreateSkill(SkillNumber.Inferno, "Inferno", CharacterClasses.DarkWizard | CharacterClasses.MagicGladiator, DamageType.Wizardry, 100, manaConsumption: 200, energyRequirement: 578, elementalModifier: ElementalType.Fire, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.EnergyBall, "Energy Ball", CharacterClasses.DarkWizard | CharacterClasses.MagicGladiator, DamageType.Wizardry, 3, 6, manaConsumption: 1);
        this.CreateSkill(SkillNumber.Defense, "Defense", CharacterClasses.DarkKnight | CharacterClasses.MagicGladiator, manaConsumption: 30, skillType: SkillType.Buff, skillTarget: SkillTarget.Explicit, implicitTargetRange: 0, targetRestriction: SkillTargetRestriction.Self);
        this.CreateSkill(SkillNumber.FallingSlash, "Falling Slash", CharacterClasses.DarkKnight | CharacterClasses.MagicGladiator, DamageType.Physical, distance: 3, manaConsumption: 9, movesToTarget: true, movesTarget: true);
        this.CreateSkill(SkillNumber.Lunge, "Lunge", CharacterClasses.DarkKnight | CharacterClasses.MagicGladiator, DamageType.Physical, distance: 2, manaConsumption: 9, movesToTarget: true, movesTarget: true);
        this.CreateSkill(SkillNumber.Uppercut, "Uppercut", CharacterClasses.DarkKnight | CharacterClasses.MagicGladiator, DamageType.Physical, distance: 2, manaConsumption: 8, movesToTarget: true, movesTarget: true);
        this.CreateSkill(SkillNumber.Cyclone, "Cyclone", CharacterClasses.DarkKnight | CharacterClasses.MagicGladiator, DamageType.Physical, distance: 2, manaConsumption: 9, movesToTarget: true, movesTarget: true);
        this.CreateSkill(SkillNumber.Slash, "Slash", CharacterClasses.DarkKnight | CharacterClasses.MagicGladiator, DamageType.Physical, distance: 2, manaConsumption: 10, movesToTarget: true, movesTarget: true);
        this.CreateSkill(SkillNumber.TripleShot, "Triple Shot", CharacterClasses.FairyElf, DamageType.Physical, distance: 6, manaConsumption: 5, skillType: SkillType.AreaSkillAutomaticHits);
        this.AddAreaSkillSettings(SkillNumber.TripleShot, true, 1f, 4.5f, 7f, true, TimeSpan.FromMilliseconds(50), maximumHitsPerTarget: 3, maximumHitsPerAttack: 3);
        this.CreateSkill(SkillNumber.Heal, "Heal", CharacterClasses.FairyElf, distance: 6, manaConsumption: 20, energyRequirement: 52, skillType: SkillType.Regeneration, targetRestriction: SkillTargetRestriction.Player);
        this.CreateSkill(SkillNumber.GreaterDefense, "Greater Defense", CharacterClasses.FairyElf, distance: 6, manaConsumption: 30, energyRequirement: 72, skillType: SkillType.Buff, targetRestriction: SkillTargetRestriction.Player);
        this.CreateSkill(SkillNumber.GreaterDamage, "Greater Damage", CharacterClasses.FairyElf, distance: 6, manaConsumption: 40, energyRequirement: 92, skillType: SkillType.Buff, targetRestriction: SkillTargetRestriction.Player);
        this.CreateSkill(SkillNumber.SummonGoblin, "Summon Goblin", CharacterClasses.FairyElf, manaConsumption: 40, energyRequirement: 90, skillType: SkillType.SummonMonster);
        this.CreateSkill(SkillNumber.SummonStoneGolem, "Summon Stone Golem", CharacterClasses.FairyElf, manaConsumption: 70, energyRequirement: 170, skillType: SkillType.SummonMonster);
        this.CreateSkill(SkillNumber.SummonAssassin, "Summon Assassin", CharacterClasses.FairyElf, manaConsumption: 110, energyRequirement: 190, skillType: SkillType.SummonMonster);
        this.CreateSkill(SkillNumber.SummonEliteYeti, "Summon Elite Yeti", CharacterClasses.FairyElf, manaConsumption: 160, energyRequirement: 230, skillType: SkillType.SummonMonster);
        this.CreateSkill(SkillNumber.SummonDarkKnight, "Summon Dark Knight", CharacterClasses.FairyElf, manaConsumption: 200, energyRequirement: 250, skillType: SkillType.SummonMonster);
        this.CreateSkill(SkillNumber.SummonBali, "Summon Bali", CharacterClasses.FairyElf, manaConsumption: 250, energyRequirement: 260, skillType: SkillType.SummonMonster);
        this.CreateSkill(SkillNumber.TwistingSlash, "Twisting Slash", CharacterClasses.DarkKnight | CharacterClasses.MagicGladiator, DamageType.Physical, distance: 2, manaConsumption: 10, elementalModifier: ElementalType.Wind, skillType: SkillType.AreaSkillAutomaticHits);
        this.CreateSkill(SkillNumber.Impale, "Impale", CharacterClasses.DarkKnight | CharacterClasses.MagicGladiator, DamageType.Physical, 15, 3, manaConsumption: 8, levelRequirement: 28);
        this.CreateSkill(SkillNumber.FireBreath, "Fire Breath", CharacterClasses.DarkKnight | CharacterClasses.MagicGladiator, DamageType.Physical, 30, 3, manaConsumption: 9, levelRequirement: 110);
        this.CreateSkill(SkillNumber.FlameofEvil, "Flame of Evil (Monster)", damage: 120, manaConsumption: 160, levelRequirement: 60, energyRequirement: 100);

        this.InitializeEffects();
        this.MapSkillsToEffects();
        this.CreateSpecialSummonMonsters();
        this.FixMagicEffectNumbers();
    }

    private void FixMagicEffectNumbers()
    {
        foreach (var skill in this.GameConfiguration.Skills.Where(s => s.MagicEffectDef is { }))
        {
            var effect = skill.MagicEffectDef;
            if (effect?.Number >= 0)
            {
                effect.Number = skill.Number;
            }
        }
    }

    private void CreateSpecialSummonMonsters()
    {
        var bali = this.Context.CreateNew<MonsterDefinition>();
        this.GameConfiguration.Monsters.Add(bali);
        bali.Number = 150;
        bali.Designation = "Bali";
        bali.MoveRange = 3;
        bali.AttackRange = 1;
        bali.ViewRange = 7;
        bali.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
        bali.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
        bali.RespawnDelay = new TimeSpan(100 * TimeSpan.TicksPerSecond);
        bali.Attribute = 2;
        bali.NumberOfMaximumItemDrops = 1;
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

        bali.AddAttributes(attributes, this.Context, this.GameConfiguration);
    }

    private void InitializeEffects()
    {
        new DefenseEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new GreaterDamageEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new GreaterDefenseEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new HealEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        new AlcoholEffectInitializer(this.Context, this.GameConfiguration).Initialize();
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
}