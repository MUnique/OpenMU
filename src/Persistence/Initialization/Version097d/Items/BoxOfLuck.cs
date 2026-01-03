// <copyright file="BoxOfLuck.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version097d.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// Initializer for box of luck items for 0.97d.
/// </summary>
internal class BoxOfLuck : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BoxOfLuck"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public BoxOfLuck(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        this.CreateBoxOfLuck();
    }

    private void CreateBoxOfLuck()
    {
        var box = this.CreateBox("Box of Luck", 14, 11);
        box.MaximumItemLevel = 11;

        var level0 = this.CreateBaseDropGroup(box, 0, "Box of Luck", 6, 6, SpecialItemType.RandomItem);
        this.AddBaseItems(level0);
        this.AddMoneyDropFallback(box, 10000, level0);

        for (byte level = 1; level <= 7; level++)
        {
            var group = this.CreateBaseDropGroup(box, level, $"Box of Luck +{level}", 6, 9, SpecialItemType.RandomItem);
            this.AddBaseItems(group);
            this.AddMoneyDropFallback(box, 10000, group, group.Description);
        }

        for (byte level = 8; level <= 11; level++)
        {
            var group = this.CreateBaseDropGroup(box, level, $"Box of Kundun +{level - 7}", 0, 0, SpecialItemType.Excellent);
            this.AddBaseItems(group);
            this.AddMoneyDropFallback(box, 20000, group, group.Description);
        }
    }

    private ItemDropItemGroup CreateBaseDropGroup(ItemDefinition box, byte level, string name, byte minLevel, byte maxLevel, SpecialItemType itemType)
    {
        var group = this.Context.CreateNew<ItemDropItemGroup>();
        group.ItemType = itemType;
        group.SourceItemLevel = level;
        group.Chance = 1.0;
        group.MinimumLevel = minLevel;
        group.MaximumLevel = maxLevel;
        group.Description = name;
        box.DropItems.Add(group);
        return group;
    }

    private void AddBaseItems(ItemDropItemGroup dropGroup)
    {
        this.AddDropItem(dropGroup, 0, 3); // Katana
        this.AddDropItem(dropGroup, 0, 5); // Blade
        this.AddDropItem(dropGroup, 0, 9); // Sword of Salamander
        this.AddDropItem(dropGroup, 0, 10); // Light Saber
        this.AddDropItem(dropGroup, 0, 13); // Double Blade
        this.AddDropItem(dropGroup, 4, 4); // Tiger Bow
        this.AddDropItem(dropGroup, 4, 5); // Silver Bow
        this.AddDropItem(dropGroup, 4, 9); // Golden Crossbow
        this.AddDropItem(dropGroup, 4, 11); // Light Crossbow
        this.AddDropItem(dropGroup, 4, 12); // Serpent Crossbow
        this.AddDropItem(dropGroup, 5, 0); // Skull Staff
        this.AddDropItem(dropGroup, 5, 2); // Serpent Staff
        this.AddDropItem(dropGroup, 5, 3); // Lightning Staff
        this.AddDropItem(dropGroup, 5, 4); // Gorgon Staff
        this.AddDropItem(dropGroup, 12, 15); // Jewel of Chaos
        this.AddDropItem(dropGroup, 14, 13); // Jewel of Bless
        this.AddDropItem(dropGroup, 14, 14); // Jewel of Soul
        this.AddArmorSet(dropGroup, 0); // Bronze Set
        this.AddArmorSet(dropGroup, 2); // Pad Set
        this.AddArmorSet(dropGroup, 4); // Bone Set
        this.AddArmorSet(dropGroup, 5); // Leather Set
        this.AddArmorSet(dropGroup, 6); // Scale Set
        this.AddArmorSet(dropGroup, 7); // Sphinx Set
        this.AddArmorSet(dropGroup, 8); // Brass Set
        this.AddArmorSet(dropGroup, 10); // Vine Set
        this.AddArmorSet(dropGroup, 11); // Silk Set
        this.AddArmorSet(dropGroup, 12); // Wind Set
    }

    private ItemDefinition CreateBox(string name, byte group, byte number, byte width = 1, byte height = 1, byte maximumItemLevel = 0)
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        this.GameConfiguration.Items.Add(item);
        item.Group = group;
        item.Number = number;
        item.Name = name;
        item.Width = width;
        item.Height = height;
        item.Durability = 1;
        item.DropsFromMonsters = false;
        item.MaximumItemLevel = maximumItemLevel;
        item.SetGuid(item.Group, item.Number);
        return item;
    }

    private void AddMoneyDropFallback(ItemDefinition item, int moneyAmount, ItemDropItemGroup baseGroup, string itemName = "")
    {
        var zenDrop = this.Context.CreateNew<ItemDropItemGroup>();
        zenDrop.ItemType = SpecialItemType.Money;
        zenDrop.MoneyAmount = moneyAmount;
        zenDrop.SourceItemLevel = baseGroup.SourceItemLevel;
        zenDrop.Chance = 1.0;
        zenDrop.Description = string.IsNullOrWhiteSpace(itemName) ? $"{baseGroup.Description} - Money" : $"{itemName} - Money";
        item.DropItems.Add(zenDrop);
    }

    private void AddArmorSet(ItemDropItemGroup dropGroup, short number)
    {
        this.TryAddDropItem(dropGroup, (byte)ItemGroups.Helm, number);
        this.TryAddDropItem(dropGroup, (byte)ItemGroups.Armor, number);
        this.TryAddDropItem(dropGroup, (byte)ItemGroups.Pants, number);
        this.TryAddDropItem(dropGroup, (byte)ItemGroups.Gloves, number);
        this.TryAddDropItem(dropGroup, (byte)ItemGroups.Boots, number);
    }

    private void AddDropItem(ItemDropItemGroup dropGroup, byte group, short number)
    {
        var item = this.GameConfiguration.Items.First(i => i.Group == group && i.Number == number);
        dropGroup.PossibleItems.Add(item);
    }

    private void TryAddDropItem(ItemDropItemGroup dropGroup, byte group, short number)
    {
        if (this.GameConfiguration.Items.FirstOrDefault(i => i.Group == group && i.Number == number) is { } item)
        {
            dropGroup.PossibleItems.Add(item);
        }
    }
}
