// <copyright file="RenaDropSupport.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Shared logic for creating the Rena item and its <see cref="DropItemGroup"/>, and wiring it to the
/// Blood Castle and Devil Square 5-7 event maps. Used both by <see cref="Items.Misc"/> (fresh installs)
/// and <see cref="Updates.AddRenaItemUpdatePlugIn"/> (upgraded servers), so the two paths can't drift apart.
/// </summary>
internal static class RenaDropSupport
{
    /// <summary>
    /// The item group of Rena.
    /// </summary>
    public const byte Group = 14;

    /// <summary>
    /// The item number of Rena.
    /// </summary>
    public const short Number = 21;

    /// <summary>
    /// Gets the existing Rena <see cref="ItemDefinition"/>, or creates it if it doesn't exist yet.
    /// </summary>
    public static ItemDefinition GetOrCreateItem(IContext context, GameConfiguration gameConfiguration)
    {
        var itemDefinition = gameConfiguration.Items.FirstOrDefault(i => i.Group == Group && i.Number == Number);
        if (itemDefinition is not null)
        {
            return itemDefinition;
        }

        itemDefinition = context.CreateNew<ItemDefinition>();
        itemDefinition.Name = "Rena";
        itemDefinition.Number = Number;
        itemDefinition.Group = Group;
        itemDefinition.DropLevel = 10;
        itemDefinition.DropsFromMonsters = false;
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        gameConfiguration.Items.Add(itemDefinition);
        return itemDefinition;
    }

    /// <summary>
    /// Gets the existing <see cref="DropItemGroup"/> for Rena, or creates it if it doesn't exist yet.
    /// </summary>
    public static DropItemGroup GetOrCreateDropItemGroup(IContext context, GameConfiguration gameConfiguration, ItemDefinition itemDefinition)
    {
        var dropItemGroup = gameConfiguration.DropItemGroups.FirstOrDefault(g => g.PossibleItems.Contains(itemDefinition));
        if (dropItemGroup is not null)
        {
            return dropItemGroup;
        }

        dropItemGroup = context.CreateNew<DropItemGroup>();
        dropItemGroup.SetGuid(itemDefinition.Group, itemDefinition.Number);
        dropItemGroup.PossibleItems.Add(itemDefinition);
        dropItemGroup.Chance = 0.01; // 1 Percent
        dropItemGroup.Description = "The drop item group for Rena";
        dropItemGroup.MinimumMonsterLevel = 30;
        dropItemGroup.MaximumMonsterLevel = 255;
        gameConfiguration.DropItemGroups.Add(dropItemGroup);
        return dropItemGroup;
    }

    /// <summary>
    /// Wires the given <see cref="DropItemGroup"/> to the Blood Castle and Devil Square 5-7 event maps,
    /// skipping maps which already reference it.
    /// </summary>
    public static void WireToEventMaps(GameConfiguration gameConfiguration, DropItemGroup dropItemGroup)
    {
        var eventMaps = gameConfiguration.Maps
            .Where(m => m.Name.Value?.StartsWith("Blood Castle") is true || m.Name.Value is "Devil Square 5" or "Devil Square 6" or "Devil Square 7");
        foreach (var map in eventMaps)
        {
            if (!map.DropItemGroups.Contains(dropItemGroup))
            {
                map.DropItemGroups.Add(dropItemGroup);
            }
        }
    }
}
