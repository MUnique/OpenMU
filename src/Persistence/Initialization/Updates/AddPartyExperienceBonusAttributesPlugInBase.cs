// <copyright file="AddPartyExperienceBonusAttributesPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// This update adds the attributes which define the experience bonus of general parties and set parties,
/// and assigns their default values to the global base attributes.
/// </summary>
/// <remarks>
/// Existing values are not changed, so that a server owner can adjust the bonus and still apply this update.
/// </remarks>
public abstract class AddPartyExperienceBonusAttributesPlugInBase : UpdatePlugInBase
{
    /// <inheritdoc />
    public override string Name => Properties.PlugInResources.AddPartyExperienceBonusAttributesPlugInBase_Name;

    /// <inheritdoc />
    public override string Description => Properties.PlugInResources.AddPartyExperienceBonusAttributesPlugInBase_Description;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 23, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var (stat, defaultValue) in PartyExperienceBonusDefaults.Values)
        {
            var definition = gameConfiguration.Attributes.FirstOrDefault(a => a.Id == stat.Id);
            if (definition is null)
            {
                definition = context.CreateNew<AttributeDefinition>(stat.Id, stat.Designation, stat.Description);
                gameConfiguration.Attributes.Add(definition);
            }

            if (!gameConfiguration.GlobalBaseAttributeValues.Any(a => a.Definition?.Id == definition.Id))
            {
                gameConfiguration.GlobalBaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(defaultValue, definition));
            }
        }

        return ValueTask.CompletedTask;
    }
}
