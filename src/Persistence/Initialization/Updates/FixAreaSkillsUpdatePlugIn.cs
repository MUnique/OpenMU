// <copyright file="FixAreaSkillsUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes area skills' range, effect radius, and delay issues.
/// Hellfire, Decay, and Ice Storm had missing area skill settings causing incorrect hit radius.
/// Ice Storm had an incorrect 200ms delay between hits.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("B9938E4D-8F63-48DF-AE45-6739D1E2A8C7")]
public class FixAreaSkillsUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Area Skills";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Fixes Hellfire, Decay, and Ice Storm skills' range, effect radius, and delay.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixAreaSkills;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 04, 09, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
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
}
