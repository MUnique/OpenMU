// <copyright file="AddRandomExperienceConfigAttributesPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// This update adds the random experience config attributes to the database
/// (RandomExperienceMinMultiplier, RandomExperienceMaxMultiplier)
/// and assigns their default values to global base attributes.
/// </summary>
public abstract class AddRandomExperienceConfigAttributesPlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plugin name.
    /// </summary>
    internal const string PlugInName = "Add Random Experience Configuration Attributes";

    /// <summary>
    /// The plugin description.
    /// </summary>
    internal const string PlugInDescription = "Adds new random experience Configuration attributes to the game configuration.";

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
            Stats.RandomExperienceMinMultiplier,
            Stats.RandomExperienceMaxMultiplier,
        };

        foreach (var attr in attributesToAdd)
        {
            if (!gameConfiguration.Attributes.Any(a => a.Id == attr.Id))
            {
                var newAttr = context.CreateNew<AttributeDefinition>(attr.Id, attr.Designation, attr.Description);
                gameConfiguration.Attributes.Add(newAttr);
            }
        }

        var randMin = gameConfiguration.Attributes.First(a => a.Id == Stats.RandomExperienceMinMultiplier.Id);
        var randMax = gameConfiguration.Attributes.First(a => a.Id == Stats.RandomExperienceMaxMultiplier.Id);

        if (!gameConfiguration.GlobalBaseAttributeValues.Any(a => a.Definition?.Id == randMin.Id))
        {
            gameConfiguration.GlobalBaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(0.8f, randMin));
        }

        if (!gameConfiguration.GlobalBaseAttributeValues.Any(a => a.Definition?.Id == randMax.Id))
        {
            gameConfiguration.GlobalBaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(1.2f, randMax));
        }
    }
}
