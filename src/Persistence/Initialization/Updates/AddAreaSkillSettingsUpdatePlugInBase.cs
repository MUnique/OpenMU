// <copyright file="AddAreaSkillSettingsUpdatePlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// This update adds the area skill settings for skills like evil spirit, so that they work properly again.
/// </summary>
/// <remarks>
/// v2: Hellfire, Decay and Ice Storm range, radius and delay fixes.
/// v3: Triple Shot projectile count.
/// v4: guaranteed first hit for Flame, Twister and Evil Spirit; Twister frustum matched to its range.
/// v5: Force Wave stays DirectHit while keeping its frustum settings.
/// v6: Triple/Multi Shot 100 ms spacing, Multi Shot 5 arrows, mastery settings propagated.
/// Re-applies seeded values on every run, overwriting owner tweaks to the same fields; missing skills are skipped.
/// </remarks>
public abstract class AddAreaSkillSettingsUpdatePlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add Area Skill Settings";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Adds the new area skill settings for skills like evil spirit, etc. to make them work properly again. Re-applies the seeded values; Force Wave stays a targeted skill.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override int Version => 6;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2024, 10, 25, 19, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    public override DateTime UpdatedAt => new(2026, 10, 16, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.Flame, false, default, default, default, true, TimeSpan.Zero, TimeSpan.FromMilliseconds(500), 1, 2, default, 0.5f, targetAreaDiameter: 2, useTargetAreaFilter: true);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.Twister, true, 1.5f, 1.5f, 6f, true, TimeSpan.FromMilliseconds(300), TimeSpan.FromMilliseconds(1000), 1, 2, default, 0.7f);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.EvilSpirit, false, default, default, default, true, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(1000), 1, 2, default, 0.7f, newRange: 7);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.AquaBeam, true, 1.5f, 1.5f, 8f);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.Cometfall, false, default, default, default, targetAreaDiameter: 2, useTargetAreaFilter: true);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.TripleShot, true, 1f, 4.5f, 7f, true, TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100), maximumHitsPerTarget: 3, maximumHitsPerAttack: 3);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.IceStorm, false, default, default, default, true, TimeSpan.Zero, TimeSpan.FromMilliseconds(200), targetAreaDiameter: 3, useTargetAreaFilter: true);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.Penetration, true, 1.1f, 1.2f, 8f, useDeferredHits: true, delayPerOneDistance: TimeSpan.FromMilliseconds(50));
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.FireSlash, true, 1.5f, 2, 2);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.PowerSlash, true, 1.0f, 6.0f, 6.0f);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.ElectricSpike, true, 1.5f, 1.5f, 12f, useDeferredHits: true, delayPerOneDistance: TimeSpan.FromMilliseconds(10));
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.ForceWave, true, 1f, 1f, 4f, skillType: SkillType.DirectHit);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.Stun, true, 1.5f, 1.5f, 3f);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.FireScream, true, 2f, 3f, 6f);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.MultiShot, true, 1f, 6f, 7f, true, TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100));
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.FlameStrike, true, 5f, 2f, 4f);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.ChaoticDiseier, true, 1.5f, 1.5f, 6f);

        // Fix master skills as well:
        foreach (var skill in gameConfiguration.Skills.OrderBy(s => s.Number))
        {
            var replacedSkill = skill.MasterDefinition?.ReplacedSkill;
            if (replacedSkill?.AreaSkillSettings is not { } areaSkillSettings)
            {
                continue;
            }

            CopyAreaSkillSettings(context, gameConfiguration, areaSkillSettings, skill);
        }

        this.FixHellfireDecayAndIceStorm(context, gameConfiguration);
        EnsureProjectileCount(gameConfiguration, SkillNumber.TripleShot, 3);
        EnsureProjectileCount(gameConfiguration, SkillNumber.MultiShot, 5);
    }

    /// <summary>
    /// Copies the area skill settings onto the specified skill, preserving the target row identity.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="source">The settings to copy.</param>
    /// <param name="target">The skill receiving the copy.</param>
    private static void CopyAreaSkillSettings(IContext context, GameConfiguration gameConfiguration, AreaSkillSettings source, Skill target)
    {
        var targetSettings = target.AreaSkillSettings ?? context.CreateNew<AreaSkillSettings>();
        var id = targetSettings.GetId();
        targetSettings.AssignValuesOf(source, gameConfiguration);
        targetSettings.SetGuid(id);
        target.AreaSkillSettings = targetSettings;
    }

    /// <summary>
    /// Sets the projectile count of the specified skill, so that arrow directions are handled properly.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="skillNumber">The skill.</param>
    /// <param name="projectileCount">The projectile count.</param>
    private static void EnsureProjectileCount(GameConfiguration gameConfiguration, SkillNumber skillNumber, int projectileCount)
    {
        var skill = gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)skillNumber);
        if (skill?.AreaSkillSettings is { } settings)
        {
            settings.ProjectileCount = projectileCount;
        }
    }

    /// <summary>
    /// Fixes Hellfire, Decay and Ice Storm skills' range, effect radius and delay.
    /// Hellfire, Decay and their strengthen versions had missing area skill settings,
    /// causing an incorrect hit radius. Ice Storm had an incorrect 200ms delay between hits.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    private void FixHellfireDecayAndIceStorm(IContext context, GameConfiguration gameConfiguration)
    {
        // Fix Hellfire Strengthener
        var hellfireStrengthener = gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.HellfireStrengthener);
        if (hellfireStrengthener != null)
        {
            hellfireStrengthener.Range = 4;
            hellfireStrengthener.AttackDamage = 3;

            if (hellfireStrengthener.AreaSkillSettings == null)
            {
                var areaSkillSettings = context.CreateNew<AreaSkillSettings>();
                hellfireStrengthener.AreaSkillSettings = areaSkillSettings;
                areaSkillSettings.EffectRange = 2;
            }
        }

        // Fix Decay Strengthener
        var decayStrengthener = gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.DecayStrengthener);
        if (decayStrengthener != null)
        {
            decayStrengthener.Range = 6;

            if (decayStrengthener.AreaSkillSettings == null)
            {
                var areaSkillSettings = context.CreateNew<AreaSkillSettings>();
                decayStrengthener.AreaSkillSettings = areaSkillSettings;
                areaSkillSettings.EffectRange = 2;
            }
        }

        // Fix base Decay skill
        var decay = gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Decay);
        if (decay != null)
        {
            if (decay.AreaSkillSettings == null)
            {
                var areaSkillSettings = context.CreateNew<AreaSkillSettings>();
                decay.AreaSkillSettings = areaSkillSettings;
                areaSkillSettings.EffectRange = 2;
            }
        }

        // Fix Ice Storm - remove incorrect 200ms delay
        var iceStorm = gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.IceStorm);
        if (iceStorm?.AreaSkillSettings != null)
        {
            iceStorm.AreaSkillSettings.DelayBetweenHits = TimeSpan.Zero;
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
        int maximumHitsPerAttack = default,
        float hitChancePerDistanceMultiplier = 1.0f,
        bool useTargetAreaFilter = false,
        float targetAreaDiameter = default,
        short? newRange = null,
        SkillType skillType = SkillType.AreaSkillAutomaticHits)
    {
        // Older game versions don't contain every skill; those are skipped.
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)skillNumber) is not { } skill)
        {
            return;
        }

        var areaSkillSettings = skill.AreaSkillSettings ?? context.CreateNew<AreaSkillSettings>();
        skill.AreaSkillSettings = areaSkillSettings;
        skill.SkillType = skillType;

        if (newRange.HasValue)
        {
            skill.Range = newRange.Value;
        }

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
        areaSkillSettings.MaximumNumberOfHitsPerAttack = maximumHitsPerAttack;
        areaSkillSettings.HitChancePerDistanceMultiplier = hitChancePerDistanceMultiplier;
    }
}
