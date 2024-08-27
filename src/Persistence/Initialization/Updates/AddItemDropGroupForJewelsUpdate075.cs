// <copyright file="AddItemDropGroupForJewels075Update.cs" company="MUnique">
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
public class AddItemDropGroupForJewels075Update : UpdatePlugInBase
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
        this.CreateDropItemGroupForJewels(context, gameConfiguration);

        this.AddJewelToItemDrop(gameConfiguration, "Jewel of Bless");
        this.AddJewelToItemDrop(gameConfiguration, "Jewel of Soul");
        this.AddJewelToItemDrop(gameConfiguration, "Jewel of Chaos");
    }

    /// <summary>
    /// Add jewel to specific drop item group
    /// </summary>
    /// <param name="gameConfiguration">Game configuration context to update.</param>
    /// <param name="jewelName">Jewel name to add into specific drop item group.</param>
    protected void AddJewelToItemDrop(GameConfiguration gameConfiguration, string jewelName)
    {
        var jewelsItemDrop = this.GetJewelsDropItemGroup(gameConfiguration);

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

    private DropItemGroup? GetJewelsDropItemGroup(GameConfiguration gameConfiguration)
    {
        var id = GuidHelper.CreateGuid<DropItemGroup>(4);
        var jewelsItemDrop = gameConfiguration.DropItemGroups.FirstOrDefault(x => x.GetId() == id);
        return jewelsItemDrop;
    }

    private void CreateDropItemGroupForJewels(IContext context, GameConfiguration gameConfiguration)
    {
        var jewelsItemDrop = this.GetJewelsDropItemGroup(gameConfiguration);

        if (jewelsItemDrop != null)
        {
            return;
        }

        var jewelsDropItemGroup = context.CreateNew<DropItemGroup>();
        jewelsDropItemGroup.SetGuid(4);
        jewelsDropItemGroup.Chance = 0.001;
        jewelsDropItemGroup.ItemType = SpecialItemType.RandomItem;
        jewelsDropItemGroup.Description = "The jewels drop item group (0.1 % drop chance)";
        gameConfiguration.DropItemGroups.Add(jewelsDropItemGroup);
    }
}