// <copyright file="FixDrainLifeSkillUpdate.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This adds the items required to enter the kalima map.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("A8827A3C-7F52-47CF-9EA5-562A9C06B986")]
public class FixSleepSkillUpdate : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Sleep Skill";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Updates the Sleep skill definition to correct it and implement the new stat.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixSleepSkillSeason6;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2024, 09, 01, 18, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var sleepAttribute = gameConfiguration.Attributes.FirstOrDefault(a => a.Id == Stats.IsAsleep.Id);

        if (sleepAttribute == null)
        {
            gameConfiguration.Attributes.Add(new AttributeSystem.AttributeDefinition
            {
                Id = Stats.IsAsleep.Id,
                Designation = Stats.IsAsleep.Designation,
                Description = Stats.IsAsleep.Description,
            });
        }

        var magicEffectSleep = gameConfiguration.MagicEffects.FirstOrDefault(e => e.Number == (short)MagicEffectNumber.Sleep);

        if (magicEffectSleep == null)
        {
            magicEffectSleep = context.CreateNew<MagicEffectDefinition>();
            gameConfiguration.MagicEffects.Add(magicEffectSleep);
            magicEffectSleep.Name = "Sleep";
            magicEffectSleep.InformObservers = true;
            magicEffectSleep.Number = (short)MagicEffectNumber.Sleep;
            magicEffectSleep.StopByDeath = true;
            magicEffectSleep.SubType = 255;
            magicEffectSleep.Duration = context.CreateNew<PowerUpDefinitionValue>();
            magicEffectSleep.Duration.ConstantValue.Value = 5;
            var powerUp = context.CreateNew<PowerUpDefinition>();
            magicEffectSleep.PowerUpDefinitions.Add(powerUp);
            var boost = context.CreateNew<PowerUpDefinitionValue>();
            powerUp.Boost = boost;
            boost.ConstantValue.Value = 1;
            powerUp.TargetAttribute = Stats.IsAsleep.GetPersistent(gameConfiguration);
        }

        var sleepSkill = gameConfiguration.Skills.First(x => x.Number == (short)SkillNumber.Sleep);
        sleepSkill.SkillType = SkillType.Buff;
        sleepSkill.MagicEffectDef = magicEffectSleep;
    }
}