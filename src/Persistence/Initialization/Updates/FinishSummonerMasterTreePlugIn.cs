// <copyright file="FinishSummonerMasterTreePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update completes the summoner master tree and fixes some of its skill values.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("4A1F9B2C-3D6E-4F70-A8B9-1C2D3E4F5A6B")]
public class FinishSummonerMasterTreePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Finish Summoner Master Tree PlugIn";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update completes the summoner master tree and fixes some of its skill values.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FinishSummonerMasterTree;

    /// <inheritdoc />
    public override string DataInitializationKey => DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 7, 27, 20, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        // Create new Stats
        var sleepStrBonusChance = context.CreateNew<AttributeDefinition>(Stats.SleepStrBonusChance.Id, Stats.SleepStrBonusChance.Designation, Stats.SleepStrBonusChance.Description);
        gameConfiguration.Attributes.Add(sleepStrBonusChance);
        var drainLifeStrBonusHealing = context.CreateNew<AttributeDefinition>(Stats.DrainLifeStrBonusHealing.Id, Stats.DrainLifeStrBonusHealing.Designation, Stats.DrainLifeStrBonusHealing.Description);
        gameConfiguration.Attributes.Add(drainLifeStrBonusHealing);

        // Update Berserker effect
        var berserkerEffect = gameConfiguration.MagicEffects.First(e => e.Number == (short)MagicEffectNumber.Berserker);
        berserkerEffect.Duration?.MaximumValue = 180;

        if (berserkerEffect.PowerUpDefinitions.FirstOrDefault(pud => pud.TargetAttribute == Stats.BerserkerManaMultiplier) is PowerUpDefinition berserkerManaPowerUp)
        {
            berserkerManaPowerUp.Boost?.MaximumValue = 1f; // 100% increase
        }

        // Update master skills
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.SleepStrengthener) is { } sleepStrengthener)
        {
            sleepStrengthener.AttributeRelationships.Add(context.CreateNew<AttributeRelationship>(
                sleepStrBonusChance,
                1,
                sleepStrBonusChance,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.AddRaw));

            if (sleepStrengthener.MasterDefinition is { } masterDefinition)
            {
                masterDefinition.TargetAttribute = sleepStrBonusChance;
                masterDefinition.ValueFormula = SkillsInitializer.Formula120Value;
                masterDefinition.Aggregation = AggregateType.AddRaw;
            }
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.DrainLifeStrengthener)?.MasterDefinition is { } drainLifeStrengthener)
        {
            drainLifeStrengthener.TargetAttribute = drainLifeStrBonusHealing;
            drainLifeStrengthener.Aggregation = AggregateType.AddRaw;
        }
    }
}
