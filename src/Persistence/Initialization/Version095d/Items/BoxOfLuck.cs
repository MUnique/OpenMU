// <copyright file="BoxOfLuck.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// Initializer for box of luck items.
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

    /// <summary>
    /// Creates the Box of Luck and its higher level boxes.
    /// +0: Box of Luck
    /// +1: Star of the Sacred Birth
    /// +2: Firecracker
    /// +3: Heart of Love
    /// +4: Olive of Love
    /// +5: Silver Medal
    /// +6: Gold Medal
    /// +7: Box of Heaven
    /// +8: Box of Kundun+1
    /// +9: Box of Kundun+2
    /// +10: Box of Kundun+3
    /// +11: Box of Kundun+4
    /// </summary>
    /// <remarks>
    /// Regex for ExTeam Files:
    /// Search: ^(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+\/\/(.+)$
    /// Replace: this.AddDropItem(boxOfLuck, $1, $2); // $9
    /// And for another format:
    /// Search: ^(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+\/\/(.+)$
    /// Replace: this.AddDropItem(heart, $1, $2); // $7.
    /// </remarks>
    private void CreateBoxOfLuck()
    {
        var box = this.CreateBox("Box of Luck", 14, 11);
        box.MaximumItemLevel = 1;
        var boxOfLuck = this.Context.CreateNew<ItemDropItemGroup>();
        boxOfLuck.ItemType = SpecialItemType.RandomItem;
        boxOfLuck.SourceItemLevel = 0;
        boxOfLuck.Chance = 0.5;
        boxOfLuck.MinimumLevel = 6;
        boxOfLuck.MaximumLevel = 6;
        boxOfLuck.Description = "Box of Luck";
        box.DropItems.Add(boxOfLuck);
        this.AddDropItem(boxOfLuck, 0, 3); // Katana
        this.AddDropItem(boxOfLuck, 0, 5); // Blade
        this.AddDropItem(boxOfLuck, 0, 9); // Sword of Salamander
        this.AddDropItem(boxOfLuck, 0, 10); // Light Saber
        this.AddDropItem(boxOfLuck, 0, 13); // Double Blade
        this.AddDropItem(boxOfLuck, 4, 4); // Tiger Bow
        this.AddDropItem(boxOfLuck, 4, 5); // Silver Bow
        this.AddDropItem(boxOfLuck, 4, 9); // Golden Crossbow
        this.AddDropItem(boxOfLuck, 4, 11); // Light Crossbow
        this.AddDropItem(boxOfLuck, 4, 12); // Serpent Crossbow
        this.AddDropItem(boxOfLuck, 5, 0); // Skull Staff
        this.AddDropItem(boxOfLuck, 5, 2); // Serpent Staff
        this.AddDropItem(boxOfLuck, 5, 3); // Lightning Staff
        this.AddDropItem(boxOfLuck, 5, 4); // Gorgon Staff
        this.AddDropItem(boxOfLuck, 12, 15); // Jewel of Chaos
        this.AddDropItem(boxOfLuck, 14, 13); // Jewel of Bless
        this.AddDropItem(boxOfLuck, 14, 14); // Jewel of Soul
        this.AddArmorSet(boxOfLuck, 0); // Bronze Set
        this.AddArmorSet(boxOfLuck, 2); // Pad Set
        this.AddArmorSet(boxOfLuck, 4); // Bone Set
        this.AddArmorSet(boxOfLuck, 5); // Leather Set
        this.AddArmorSet(boxOfLuck, 6); // Scale Set
        this.AddArmorSet(boxOfLuck, 7); // Sphinx Set
        this.AddArmorSet(boxOfLuck, 8); // Brass Set
        this.AddArmorSet(boxOfLuck, 10); // Vine Set
        this.AddArmorSet(boxOfLuck, 11); // Silk Set
        this.AddArmorSet(boxOfLuck, 12); // Wind Set
        this.AddMoneyDropFallback(box, 10000, boxOfLuck);
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
