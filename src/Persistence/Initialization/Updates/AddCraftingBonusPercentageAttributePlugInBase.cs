// <copyright file="AddCraftingBonusPercentageAttributePlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// This update adds the <see cref="Stats.CraftingBonusPercentage"/> attribute, so that it can be
/// configured globally, per map, per account or per character.
/// </summary>
public abstract class AddCraftingBonusPercentageAttributePlugInBase : UpdatePlugInBase
{
    /// <inheritdoc />
    public override string Name => Properties.PlugInResources.AddCraftingBonusPercentageAttributePlugInBase_Name;

    /// <inheritdoc />
    public override string Description => Properties.PlugInResources.AddCraftingBonusPercentageAttributePlugInBase_Description;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 10, 24, 18, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Attributes.All(a => a.Id != Stats.CraftingBonusPercentage.Id))
        {
            var attribute = context.CreateNew<AttributeDefinition>(Stats.CraftingBonusPercentage.Id, Stats.CraftingBonusPercentage.Designation, Stats.CraftingBonusPercentage.Description);
            gameConfiguration.Attributes.Add(attribute);
        }

        return ValueTask.CompletedTask;
    }
}
