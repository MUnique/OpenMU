// <copyright file="RemoveJewelDropLevelGapPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// This update removes the existing drop level gap condition for jewels and similar items that should always drop.
/// </summary>
public abstract class RemoveJewelDropLevelGapPlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Remove Jewel Drop Level Gap";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update removes the existing drop level gap condition for jewels and similar items that should always drop.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 3, 5, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var jewelsDropGroupId = new Guid("00000200-0004-0000-0000-000000000000");
        var jewelsDropGroup = gameConfiguration.DropItemGroups.First(x => x.GetId() == jewelsDropGroupId);
        jewelsDropGroup.ItemType = SpecialItemType.Jewel;

        Dictionary<ItemGroups, List<int>> itemGroupToItemNumbers = new()
        {
            { ItemGroups.Misc1, [0, 1, 2] },    // angel, imp, uniria
            { ItemGroups.Misc2, [9, 10] },   // alcohol, town portal scroll
        };
        foreach (var entry in itemGroupToItemNumbers)
        {
            var items = gameConfiguration.Items.Where(i => i.Group == (int)entry.Key && entry.Value.Contains(i.Number));
            foreach (var item in items)
            {
                jewelsDropGroup.PossibleItems.Add(item);
            }
        }
    }
}