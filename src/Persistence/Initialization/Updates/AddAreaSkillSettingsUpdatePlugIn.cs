// <copyright file="AddAreaSkillSettingsUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This adds the items required to enter the kalima map.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("D01DA745-BF72-40C4-BD90-D2D637AEDF99")]
public class AddAreaSkillSettingsUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add Area Skill Settings";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Adds the new area skill settings for skills like evil spirit, etc. to make them work properly again.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddAreaSkillSettings;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2024, 10, 25, 19, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.Flame, false, default, default, default, true, TimeSpan.Zero, TimeSpan.FromMilliseconds(500), 0, 2, default, 0.5f, targetAreaDiameter: 2, useTargetAreaFilter: true);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.Twister, true, 1.5f, 1.5f, 4f, true, TimeSpan.FromMilliseconds(300), TimeSpan.FromMilliseconds(1000), 0, 2, default, 0.7f);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.EvilSpirit, false, default, default, default, true, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(1000), 0, 2, default, 0.7f, newRange: 7);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.AquaBeam, true, 1.5f, 1.5f, 8f);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.Cometfall, false, default, default, default, targetAreaDiameter: 2, useTargetAreaFilter: true);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.TripleShot, true, 1f, 4.5f, 7f, true, TimeSpan.FromMilliseconds(50), maximumHitsPerTarget: 3, maximumHitsPerAttack: 3);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.IceStorm, false, default, default, default, true, TimeSpan.Zero, TimeSpan.FromMilliseconds(200), targetAreaDiameter: 3, useTargetAreaFilter: true);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.Penetration, true, 1.1f, 1.2f, 8f, useDeferredHits: true, delayPerOneDistance: TimeSpan.FromMilliseconds(50));
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.FireSlash, true, 1.5f, 2, 2);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.PowerSlash, true, 1.0f, 6.0f, 6.0f);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.ElectricSpike, true, 1.5f, 1.5f, 12f, useDeferredHits: true, delayPerOneDistance: TimeSpan.FromMilliseconds(10));
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.ForceWave, true, 1f, 1f, 4f);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.Stun, true, 1.5f, 1.5f, 3f);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.FireScream, true, 2f, 3f, 6f);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.MultiShot, true, 1f, 6f, 7f);
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

            skill.AreaSkillSettings = context.CreateNew<AreaSkillSettings>();
            var id = skill.AreaSkillSettings.GetId();
            skill.AreaSkillSettings.AssignValuesOf(areaSkillSettings, gameConfiguration);
            skill.AreaSkillSettings.SetGuid(id);
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
        short? newRange = null)
    {
        var skill = gameConfiguration.Skills.First(s => s.Number == (short)skillNumber);
        var areaSkillSettings = context.CreateNew<AreaSkillSettings>();
        skill.AreaSkillSettings = areaSkillSettings;
        skill.SkillType = SkillType.AreaSkillAutomaticHits;

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