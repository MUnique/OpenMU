// <copyright file="FixSummonerCurseSkillsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds missing area skill settings for summoner curse (book) and lightning shock skills.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("A3B4C8DB-2F39-4C81-A2D9-5E4FA5B9E004")]
public class FixSummonerCurseSkillsPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Summoner Curse Skills";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds missing area skill settings for summoner curse (book) and lightning shock skills.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixSummonerCurseSkills;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 3, 30, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        // Add new attributes
        this.AddStatIfNotExists(context, gameConfiguration, Stats.BleedingDamageMultiplier);
        var bleedingDamageMultiplier = Stats.BleedingDamageMultiplier.GetPersistent(gameConfiguration);
        this.AddStatIfNotExists(context, gameConfiguration, Stats.IsBleeding);
        var isBleeding = Stats.IsBleeding.GetPersistent(gameConfiguration);
        this.AddStatIfNotExists(context, gameConfiguration, Stats.StunChance);
        var stunChance = Stats.StunChance.GetPersistent(gameConfiguration);
        this.AddStatIfNotExists(context, gameConfiguration, Stats.PollutionMoveTargetChance);
        var pollutionMoveTargetChance = Stats.PollutionMoveTargetChance.GetPersistent(gameConfiguration);

        // Add new base attribute to summoner classes
        var summonerClassNumbers = new[] { (int)CharacterClassNumber.Summoner, (int)CharacterClassNumber.BloodySummoner, (int)CharacterClassNumber.DimensionMaster };
        foreach (var characterClass in gameConfiguration.CharacterClasses.Where(c => summonerClassNumbers.Contains(c.Number)))
        {
            characterClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(0.6f, bleedingDamageMultiplier));
        }

        // Create new magic effects
        var explosionEffect = this.CreateMagicEffect(context, gameConfiguration, MagicEffectNumber.Explosion, "Explosion Effect", isBleeding, duration: 5);
        var requiemEffect = this.CreateMagicEffect(context, gameConfiguration, MagicEffectNumber.Requiem, "Requiem Effect", isBleeding, duration: 5);
        this.CreateMagicEffect(context, gameConfiguration, MagicEffectNumber.Stunned, "Stun Effect", Stats.IsStunned.GetPersistent(gameConfiguration));

        this.MapSkillToEffect(gameConfiguration, SkillNumber.Explosion223, explosionEffect);
        this.MapSkillToEffect(gameConfiguration, SkillNumber.Requiem, requiemEffect);

        // Set elemental modifiers
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Pollution) is { } pollution)
        {
            pollution.SkipElementalModifier = true;
            pollution.MagicEffectDef = this.CreateMagicEffect(context, gameConfiguration, MagicEffectNumber.Iced, "Iced", Stats.IsIced.GetPersistent(gameConfiguration), duration: 2);
        }

        foreach (var skillNumber in new[]
            {
                SkillNumber.ChainDrive, SkillNumber.ChainDriveStrengthener,
                SkillNumber.LightningShock, SkillNumber.LightningShockStr,
                SkillNumber.Earthshake, SkillNumber.EarthshakeStreng, SkillNumber.EarthshakeMastery,
                SkillNumber.Explosion223, SkillNumber.Requiem,
            })
        {
            if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)skillNumber) is { } skill)
            {
                skill.SkipElementalModifier = true;
            }
        }

        // Update AreaSkillSettings
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.Explosion223, false, 0, 0, 0, effectRange: 2);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.Requiem, false, 0, 0, 0, effectRange: 2);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.Pollution, false, 0, 0, 0, minimumHitsPerAttack: 4, maximumHitsPerAttack: 8, effectRange: 3);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.LightningShock, false, 0, 0, 0, minimumHitsPerAttack: 5, maximumHitsPerAttack: 12, useTargetAreaFilter: true, targetAreaDiameter: 14);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.LightningShockStr, false, 0, 0, 0, minimumHitsPerAttack: 5, maximumHitsPerAttack: 12, useTargetAreaFilter: true, targetAreaDiameter: 14);

        // Update master skills
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.FireTomeMastery)?.MasterDefinition is { } fireTomeMastery)
        {
            fireTomeMastery.TargetAttribute = bleedingDamageMultiplier;
            fireTomeMastery.Aggregation = AggregateType.AddRaw;
            fireTomeMastery.ValueFormula = $"{fireTomeMastery.ValueFormula} / 100";
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.WindTomeMastery)?.MasterDefinition is { } windTomeMastery)
        {
            windTomeMastery.TargetAttribute = stunChance;
            windTomeMastery.Aggregation = AggregateType.AddRaw;
            windTomeMastery.ValueFormula = $"{windTomeMastery.ValueFormula} / 100";
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.LightningTomeMastery)?.MasterDefinition is { } lightningTomeMastery)
        {
            lightningTomeMastery.TargetAttribute = pollutionMoveTargetChance;
            lightningTomeMastery.Aggregation = AggregateType.AddRaw;
            lightningTomeMastery.ValueFormula = $"{lightningTomeMastery.ValueFormula} / 100";
        }
    }

    private MagicEffectDefinition CreateMagicEffect(IContext context, GameConfiguration gameConfiguration, MagicEffectNumber effectNumber, string name, AttributeDefinition targetAttribute, int? duration = null)
    {
        var magicEffect = context.CreateNew<MagicEffectDefinition>();
        gameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (short)effectNumber;
        magicEffect.Name = name;
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;

        if (duration is not null)
        {
            magicEffect.Duration = context.CreateNew<PowerUpDefinitionValue>();
            magicEffect.Duration.ConstantValue.Value = duration.Value;
        }

        var powerUpDefinition = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(powerUpDefinition);
        powerUpDefinition.TargetAttribute = targetAttribute;
        powerUpDefinition.Boost = context.CreateNew<PowerUpDefinitionValue>();
        powerUpDefinition.Boost.ConstantValue.Value = 1;

        return magicEffect;
    }

    private void MapSkillToEffect(GameConfiguration gameConfiguration, SkillNumber skillNumber, MagicEffectDefinition magicEffect)
    {
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)skillNumber) is { } skill)
        {
            skill.MagicEffectDef = magicEffect;
        }
    }

    private void AddAreaSkillSettings(
        GameConfiguration gameConfiguration,
        IContext context,
        SkillNumber skillNumber,
        bool useFrustumFilter,
        float frustumStartWidth,
        float frustumEndWidth,
        float frustumDistance,
        bool useDeferredHits = false,
        TimeSpan delayPerOneDistance = default,
        TimeSpan delayBetweenHits = default,
        int minimumHitsPerTarget = 1,
        int maximumHitsPerTarget = 1,
        int minimumHitsPerAttack = default,
        int maximumHitsPerAttack = default,
        float hitChancePerDistanceMultiplier = 1.0f,
        bool useTargetAreaFilter = false,
        float targetAreaDiameter = default,
        int projectileCount = 1,
        int effectRange = default)
    {
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)skillNumber) is not { } skill)
        {
            return;
        }

        skill.SkillType = SkillType.AreaSkillAutomaticHits;
        var areaSkillSettings = context.CreateNew<AreaSkillSettings>();
        skill.AreaSkillSettings = areaSkillSettings;

        areaSkillSettings.UseFrustumFilter = useFrustumFilter;
        areaSkillSettings.FrustumStartWidth = frustumStartWidth;
        areaSkillSettings.FrustumEndWidth = frustumEndWidth;
        areaSkillSettings.FrustumDistance = frustumDistance;
        areaSkillSettings.UseTargetAreaFilter = useTargetAreaFilter;
        areaSkillSettings.TargetAreaDiameter = targetAreaDiameter;
        areaSkillSettings.UseDeferredHits = useDeferredHits;
        areaSkillSettings.DelayPerOneDistance = delayPerOneDistance;
        areaSkillSettings.DelayBetweenHits = delayBetweenHits;
        areaSkillSettings.MinimumNumberOfHitsPerTarget = minimumHitsPerTarget;
        areaSkillSettings.MaximumNumberOfHitsPerTarget = maximumHitsPerTarget;
        areaSkillSettings.MinimumNumberOfHitsPerAttack = minimumHitsPerAttack;
        areaSkillSettings.MaximumNumberOfHitsPerAttack = maximumHitsPerAttack;
        areaSkillSettings.HitChancePerDistanceMultiplier = hitChancePerDistanceMultiplier;
        areaSkillSettings.ProjectileCount = projectileCount;
        areaSkillSettings.EffectRange = effectRange;
    }
}
