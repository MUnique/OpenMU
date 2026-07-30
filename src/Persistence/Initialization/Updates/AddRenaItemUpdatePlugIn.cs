// <copyright file="AddRenaItemUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the missing Rena item, which is required for the item registration feature.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("6A1B7C3D-2E5F-4A7B-8C9D-1E1F2A3B4C6E")]
public class AddRenaItemUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add Rena Item";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the missing Rena item, which is required for the item registration feature.";

    /// <summary>
    /// The item group of Rena.
    /// </summary>
    private const byte ItemGroup = 14;

    /// <summary>
    /// The item number of Rena.
    /// </summary>
    private const short ItemNumber = 21;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddRenaItem;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 07, 26, 18, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
#pragma warning disable CS1998
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
#pragma warning restore CS1998
    {
        var itemDefinition = gameConfiguration.Items.FirstOrDefault(item => item.Group == ItemGroup && item.Number == ItemNumber);
        if (itemDefinition is null)
        {
            itemDefinition = context.CreateNew<ItemDefinition>();
            itemDefinition.Name = "Rena";
            itemDefinition.Number = ItemNumber;
            itemDefinition.Group = ItemGroup;
            itemDefinition.DropLevel = 0;
            itemDefinition.DropsFromMonsters = false;
            itemDefinition.Durability = 1;
            itemDefinition.Width = 1;
            itemDefinition.Height = 1;
            itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
            gameConfiguration.Items.Add(itemDefinition);
        }

        var dropItemGroup = gameConfiguration.DropItemGroups.FirstOrDefault(group => group.PossibleItems.Contains(itemDefinition));
        if (dropItemGroup is null)
        {
            dropItemGroup = context.CreateNew<DropItemGroup>();
            dropItemGroup.SetGuid(ItemGroup, ItemNumber);
            dropItemGroup.PossibleItems.Add(itemDefinition);
            dropItemGroup.Chance = 0.01; // 1 Percent
            dropItemGroup.Description = "The drop item group for Rena";
            dropItemGroup.MinimumMonsterLevel = 30;
            dropItemGroup.MaximumMonsterLevel = 255;
            gameConfiguration.DropItemGroups.Add(dropItemGroup);
        }

        // Always (re-)wire, even if the item/group already existed - a previous, incomplete run
        // must not be able to skip this step.
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
