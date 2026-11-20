// <copyright file="UpdateWingAndPetHitTollsPlugInSeasonSix.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Properties;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Updates wing, pet, and skill hit toll configuration for Season Six.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.UpdateWingAndPetHitTollsPlugIn_Name), Description = nameof(PlugInResources.UpdateWingAndPetHitTollsPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("D7D5EE55-51DA-4D38-9B0D-50EB2E121C37")]
public class UpdateWingAndPetHitTollsPlugInSeasonSix : UpdateWingAndPetHitTollsPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    protected override void UpdateSkillTolls(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var (skillNumber, toll) in SkillTolls)
        {
            var skill = gameConfiguration.Skills.FirstOrDefault(existing => existing.Number == skillNumber);
            if (skill is null)
            {
                continue;
            }

            SetSkillToll(context, gameConfiguration, skill.AttributeRelationships, toll, Stats.IsImpEquipped);
            SetSkillToll(context, gameConfiguration, skill.AttributeRelationships, toll, Stats.IsWingEquipped);
        }
    }

    private static void SetSkillToll(IContext context, GameConfiguration gameConfiguration, ICollection<AttributeRelationship> relationships, float toll, AttributeDefinition inputAttribute)
    {
        SetAttributeRelationship(context, relationships, gameConfiguration, Stats.SkillHealthLossAfterHit, toll, inputAttribute);
    }
}