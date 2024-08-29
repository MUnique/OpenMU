// <copyright file="AddItemDropGroupForJewelsUpdate075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update creates a specific item drop group for jewels with a default chance of 5%.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("DCF14924-BB19-4CA2-93EC-397A89AA3EB3")]
public class AddItemDropGroupForJewelsUpdate075 : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Create item drop group for jewels";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update creates a specific item drop group for jewels with a default chance of 5%.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddItemDropGroupForJewels075;

    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2024, 08, 26, 20, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        this.CreateDropItemGroupForJewels(context, gameConfiguration, 4, null, "The jewels drop item group (0.1 % drop chance)");

        this.AddJewelToItemDrop(gameConfiguration, 4, null, "Jewel of Bless");
        this.AddJewelToItemDrop(gameConfiguration, 4, null, "Jewel of Soul");
        this.AddJewelToItemDrop(gameConfiguration, 4, null, "Jewel of Chaos");
    }

    /// <summary>
    /// Add jewel to specific drop item group.
    /// </summary>
    /// <param name="gameConfiguration">Game configuration context to update.</param>
    /// <param name="dropId">Drop to which the jewel will be associated.</param>
    /// <param name="mapNumber">Optionally the map number to asociate drop group item.</param>
    /// <param name="jewelName">Jewel name to add into specific drop item group.</param>
    protected void AddJewelToItemDrop(GameConfiguration gameConfiguration, short dropId, short? mapNumber, string jewelName)
    {
        var jewelsItemDrop = GetJewelsDropItemGroup(gameConfiguration, dropId, mapNumber);

        if (jewelsItemDrop == null)
        {
            return;
        }

        var item = gameConfiguration.Items.FirstOrDefault(x => x.Name == jewelName);

        if (item == null)
        {
            return;
        }

        if (item.DropsFromMonsters == true)
        {
            item.DropsFromMonsters = false;
        }

        var jewelId = item.GetItemId();

        if (!jewelsItemDrop.PossibleItems.Any(x => x.GetItemId() == jewelId))
        {
            jewelsItemDrop.PossibleItems.Add(item);
        }
    }

    /// <summary>
    /// Create drop item group for jewels.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">Game configuration context to update.</param>
    /// <param name="dropId">New drop id.</param>
    /// <param name="mapNumber">Optionally the map number to asociate drop group item.</param>
    /// <param name="name">Description of drop,</param>
    /// <returns>Returns new drop item group.</returns>
    protected DropItemGroup CreateDropItemGroupForJewels(IContext context, GameConfiguration gameConfiguration, short dropId, short? mapNumber, string name)
    {
        var jewelsItemDrop = GetJewelsDropItemGroup(gameConfiguration, dropId, mapNumber);

        if (jewelsItemDrop != null)
        {
            return jewelsItemDrop;
        }

        var jewelsDropItemGroup = context.CreateNew<DropItemGroup>();

        if (mapNumber != null)
        {
            jewelsDropItemGroup.SetGuid(mapNumber.Value, dropId);
        }
        else
        {
            jewelsDropItemGroup.SetGuid(dropId);
        }

        jewelsDropItemGroup.Chance = 0.001;
        jewelsDropItemGroup.ItemType = SpecialItemType.RandomItem;
        jewelsDropItemGroup.Description = name;
        gameConfiguration.DropItemGroups.Add(jewelsDropItemGroup);

        if (mapNumber != null)
        {
            var map = gameConfiguration.Maps.First(x => x.Number == mapNumber);
            map.DropItemGroups.Add(jewelsDropItemGroup);
        }

        return jewelsDropItemGroup;
    }

    private static DropItemGroup? GetJewelsDropItemGroup(GameConfiguration gameConfiguration, short dropId, short? mapNumber)
    {
        var id = mapNumber != null
            ? GuidHelper.CreateGuid<DropItemGroup>(mapNumber ?? 0, dropId)
            : GuidHelper.CreateGuid<DropItemGroup>(dropId);

        var jewelsItemDrop = gameConfiguration.DropItemGroups.FirstOrDefault(x => x.GetId() == id);
        return jewelsItemDrop;
    }
}