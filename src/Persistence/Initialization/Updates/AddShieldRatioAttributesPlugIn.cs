// <copyright file="AddShieldRatioAttributesPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds shield ratio attributes to initialized Season 6 data.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.AddShieldRatioAttributesPlugIn_Name), Description = nameof(PlugInResources.AddShieldRatioAttributesPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("7F6A597B-9639-4319-97FD-1966E08836EB")]
public class AddShieldRatioAttributesPlugIn : UpdatePlugInBase
{
    /// <inheritdoc />
    public override string Name => PlugInResources.AddShieldRatioAttributesPlugIn_Name;

    /// <inheritdoc />
    public override string Description => PlugInResources.AddShieldRatioAttributesPlugIn_Description;

    /// <inheritdoc />
    public override string DataInitializationKey => DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 10, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        this.AddStatIfNotExists(context, gameConfiguration, Stats.ShieldRatio);
        this.AddStatIfNotExists(context, gameConfiguration, Stats.OverShieldBurstDamageRatio);

        foreach (var characterClass in gameConfiguration.CharacterClasses)
        {
            if (!characterClass.AttributeCombinations.Any(relationship => relationship.TargetAttribute?.Id == Stats.ShieldRatio.Id))
            {
                characterClass.AttributeCombinations.Add(
                    CharacterClassHelper.CreateAttributeRelationship(context, gameConfiguration, Stats.ShieldRatio, 1, Stats.ShieldRateIncrease));
            }

            if (!characterClass.BaseAttributeValues.Any(attribute => attribute.Definition?.Id == Stats.ShieldRatio.Id))
            {
                characterClass.BaseAttributeValues.Add(
                    CharacterClassHelper.CreateConstValueAttribute(context, gameConfiguration, 0.9f, Stats.ShieldRatio));
            }

            if (!characterClass.BaseAttributeValues.Any(attribute => attribute.Definition?.Id == Stats.OverShieldBurstDamageRatio.Id))
            {
                characterClass.BaseAttributeValues.Add(
                    CharacterClassHelper.CreateConstValueAttribute(context, gameConfiguration, 0.2f, Stats.OverShieldBurstDamageRatio));
            }
        }

        return ValueTask.CompletedTask;
    }
}
