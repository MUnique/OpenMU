// <copyright file="AddExperienceConfigAttributesPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// This update adds the experience configuration attributes to the database
/// (ExperienceRandomMinMultiplier, ExperienceRandomMaxMultiplier,
/// ExperienceRatePerPartyMemberBonus, ExperienceRateBonusForSetParty)
/// and assigns their default values to global and class base attributes.
/// </summary>
public abstract class AddExperienceConfigAttributesPlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add Experience Config Attributes";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Adds new experience configuration attributes to the game configuration.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 04, 27, 20, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var attributesToAdd = new[]
        {
            Stats.ExperienceRandomMinMultiplier,
            Stats.ExperienceRandomMaxMultiplier,
            Stats.ExperienceRatePerPartyMemberBonus,
            Stats.ExperienceRateBonusForSetParty,
        };

        foreach (var attr in attributesToAdd)
        {
            if (!gameConfiguration.Attributes.Any(a => a.Id == attr.Id))
            {
                var newAttr = context.CreateNew<AttributeDefinition>(attr.Id, attr.Designation, attr.Description);
                gameConfiguration.Attributes.Add(newAttr);
            }
        }

        var randMin = gameConfiguration.Attributes.First(a => a.Id == Stats.ExperienceRandomMinMultiplier.Id);
        var randMax = gameConfiguration.Attributes.First(a => a.Id == Stats.ExperienceRandomMaxMultiplier.Id);
        var partyRate = gameConfiguration.Attributes.First(a => a.Id == Stats.ExperienceRatePerPartyMemberBonus.Id);
        var partySet = gameConfiguration.Attributes.First(a => a.Id == Stats.ExperienceRateBonusForSetParty.Id);

        if (!gameConfiguration.GlobalBaseAttributeValues.Any(a => a.Definition?.Id == randMin.Id))
        {
            gameConfiguration.GlobalBaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(0.8f, randMin));
        }

        if (!gameConfiguration.GlobalBaseAttributeValues.Any(a => a.Definition?.Id == randMax.Id))
        {
            gameConfiguration.GlobalBaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(1.2f, randMax));
        }

        foreach (var characterClass in gameConfiguration.CharacterClasses)
        {
            if (!characterClass.BaseAttributeValues.Any(a => a.Definition?.Id == partyRate.Id))
            {
                characterClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(0.01f, partyRate));
            }

            if (!characterClass.BaseAttributeValues.Any(a => a.Definition?.Id == partySet.Id))
            {
                characterClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(0.02f, partySet));
            }
        }
    }
}
