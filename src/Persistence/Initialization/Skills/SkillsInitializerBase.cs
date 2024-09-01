// <copyright file="SkillsInitializerBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;

/// <summary>
/// Base class for a skills initializer.
/// </summary>
internal abstract class SkillsInitializerBase : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SkillsInitializerBase"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    protected SkillsInitializerBase(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Creates the skill.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <param name="name">The name of the skill.</param>
    /// <param name="characterClasses">The character classes.</param>
    /// <param name="damageType">Type of the damage.</param>
    /// <param name="damage">The damage.</param>
    /// <param name="distance">The distance.</param>
    /// <param name="abilityConsumption">The ability consumption.</param>
    /// <param name="manaConsumption">The mana consumption.</param>
    /// <param name="levelRequirement">The level requirement.</param>
    /// <param name="energyRequirement">The energy requirement.</param>
    /// <param name="leadershipRequirement">The leadership requirement.</param>
    /// <param name="elementalModifier">The elemental modifier.</param>
    /// <param name="skillType">Type of the skill.</param>
    /// <param name="skillTarget">The skill target.</param>
    /// <param name="implicitTargetRange">The implicit target range.</param>
    /// <param name="targetRestriction">The target restriction.</param>
    /// <param name="movesToTarget">If set to <c>true</c>, the skill moves the player to the target.</param>
    /// <param name="movesTarget">If set to <c>true</c>, it moves target randomly.</param>
    /// <param name="cooldownMinutes">The cooldown minutes.</param>
    protected void CreateSkill(
        SkillNumber number,
        string name,
        CharacterClasses characterClasses = CharacterClasses.None,
        DamageType damageType = DamageType.None,
        int damage = 0,
        short distance = 0,
        int abilityConsumption = 0,
        int manaConsumption = 0,
        int levelRequirement = 0,
        int energyRequirement = 0,
        int leadershipRequirement = 0,
        ElementalType elementalModifier = ElementalType.Undefined,
        SkillType skillType = SkillType.DirectHit,
        SkillTarget skillTarget = SkillTarget.Explicit,
        short implicitTargetRange = 0,
        SkillTargetRestriction targetRestriction = SkillTargetRestriction.Undefined,
        bool movesToTarget = false,
        bool movesTarget = false,
        int cooldownMinutes = 0)
    {
        var skill = this.Context.CreateNew<Skill>();
        this.GameConfiguration.Skills.Add(skill);
        skill.Number = (short)number;
        skill.Name = name;
        skill.MovesToTarget = movesToTarget;
        skill.MovesTarget = movesTarget;
        skill.AttackDamage = damage;

        this.CreateSkillRequirementIfNeeded(skill, Stats.Level, levelRequirement);
        this.CreateSkillRequirementIfNeeded(skill, Stats.TotalLeadership, leadershipRequirement);
        this.CreateSkillRequirementIfNeeded(skill, Stats.TotalEnergy, energyRequirement);
        this.CreateSkillConsumeRequirementIfNeeded(skill, Stats.CurrentMana, manaConsumption);
        this.CreateSkillConsumeRequirementIfNeeded(skill, Stats.CurrentAbility, abilityConsumption);

        skill.Range = distance;
        skill.DamageType = damageType;
        skill.SkillType = skillType;

        skill.ImplicitTargetRange = implicitTargetRange;
        skill.Target = skillTarget;
        skill.TargetRestriction = targetRestriction;
        var classes = this.GameConfiguration.DetermineCharacterClasses(characterClasses);
        foreach (var characterClass in classes)
        {
            skill.QualifiedCharacters.Add(characterClass);
        }

        if (elementalModifier != ElementalType.Undefined)
        {
            this.ApplyElementalModifier(elementalModifier, skill);
        }

        skill.SetGuid(skill.Number);
    }

    private void ApplyElementalModifier(ElementalType elementalModifier, Skill skill)
    {
        if ((SkillNumber)skill.Number is SkillNumber.IceArrow or SkillNumber.IceArrowStrengthener)
        {
            skill.ElementalModifierTarget = Stats.IceResistance.GetPersistent(this.GameConfiguration);
            skill.MagicEffectDef = this.CreateEffect(ElementalType.Ice, MagicEffectNumber.Freeze, Stats.IsFrozen, 5);
            return;
        }

        if ((SkillNumber)skill.Number is SkillNumber.Sleep)
        {
            skill.MagicEffectDef = this.CreateEffect(ElementalType.Undefined, MagicEffectNumber.Sleep, Stats.IsAsleep, 5);
            return;
        }

        switch (elementalModifier)
        {
            case ElementalType.Ice:
                skill.ElementalModifierTarget = Stats.IceResistance.GetPersistent(this.GameConfiguration);
                skill.MagicEffectDef = this.CreateEffect(ElementalType.Ice, MagicEffectNumber.Iced, Stats.IsIced, 10);
                break;
            case ElementalType.Poison:
                skill.ElementalModifierTarget = Stats.PoisonResistance.GetPersistent(this.GameConfiguration);

                // Poison Skill applies damage 7 times, while decay three times. We assume that we apply each damage
                // every 3 seconds. We leave one or two extra seconds, so that the damage is applied for sure.
                var durationInSeconds = skill.Number == (short)SkillNumber.Poison ? 20 : 10;
                skill.MagicEffectDef = this.CreateEffect(ElementalType.Poison, MagicEffectNumber.Poisoned, Stats.IsPoisoned, durationInSeconds);
                break;
            case ElementalType.Lightning:
                skill.ElementalModifierTarget = Stats.LightningResistance.GetPersistent(this.GameConfiguration);
                break;
            case ElementalType.Fire:
                skill.ElementalModifierTarget = Stats.FireResistance.GetPersistent(this.GameConfiguration);
                break;
            case ElementalType.Earth:
                skill.ElementalModifierTarget = Stats.EarthResistance.GetPersistent(this.GameConfiguration);
                break;
            case ElementalType.Wind:
                skill.ElementalModifierTarget = Stats.WindResistance.GetPersistent(this.GameConfiguration);
                break;
            case ElementalType.Water:
                skill.ElementalModifierTarget = Stats.WaterResistance.GetPersistent(this.GameConfiguration);
                break;
            default:
                // None
                break;
        }
    }

    private MagicEffectDefinition CreateEffect(ElementalType type, MagicEffectNumber effectNumber, AttributeDefinition targetAttribute, float durationInSeconds)
    {
        if (this.GameConfiguration.MagicEffects.FirstOrDefault(
                e => e.Number == (short)effectNumber
                     && e.SubType == (byte)(0xFF - type)
                     && Equals(e.Duration?.ConstantValue.Value, durationInSeconds)
                     && e.PowerUpDefinitions.FirstOrDefault()?.TargetAttribute == targetAttribute) is { } existingEffect)
        {
            return existingEffect;
        }

        var effect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(effect);
        effect.Name = Enum.GetName(effectNumber) ?? string.Empty;
        effect.InformObservers = true;
        effect.Number = (short)effectNumber;
        effect.StopByDeath = true;
        effect.SubType = (byte)(0xFF - type);
        effect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        effect.Duration.ConstantValue.Value = durationInSeconds;
        var powerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        effect.PowerUpDefinitions.Add(powerUpDefinition);
        powerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        powerUpDefinition.Boost.ConstantValue.Value = 1;
        powerUpDefinition.TargetAttribute = targetAttribute.GetPersistent(this.GameConfiguration);
        return effect;
    }

    private void CreateSkillConsumeRequirementIfNeeded(Skill skill, AttributeDefinition attribute, int requiredValue)
    {
        if (requiredValue == 0)
        {
            return;
        }

        var requirement = this.CreateRequirement(attribute, requiredValue);
        skill.ConsumeRequirements.Add(requirement);
    }

    private void CreateSkillRequirementIfNeeded(Skill skill, AttributeDefinition attribute, int requiredValue)
    {
        if (requiredValue == 0)
        {
            return;
        }

        var requirement = this.CreateRequirement(attribute, requiredValue);
        skill.Requirements.Add(requirement);
    }
}