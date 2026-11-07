// <copyright file="GoblinBoxItems.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// Initializer for the keys and boxes of the goblin combination: a Gold Key opens a Sealed Golden Box,
/// and a Silver Key a Sealed Silver Box, at the chaos goblin. The resulting Golden Box and Silver Box
/// are opened by dropping them, like other boxes, and give an item or some money.
/// </summary>
/// <remarks>
/// The combinations and their success rate of 100 percent are the ones of the client (mix.bmd, mix id 47 for both).
/// The client only says that the opened boxes may turn into a rare item; their contents and chances are
/// server data, which isn't known. The contents defined here are a sensible default: jewels, excellent
/// items, or money, with better contents in the Golden Box.
/// </remarks>
internal class GoblinBoxItems : InitializerBase
{
    /// <summary>
    /// The item group of the keys and boxes.
    /// </summary>
    internal const byte ItemGroup = 14;

    /// <summary>
    /// The number of the Silver Key.
    /// </summary>
    internal const short SilverKeyNumber = 112;

    /// <summary>
    /// The number of the Gold Key.
    /// </summary>
    internal const short GoldKeyNumber = 113;

    /// <summary>
    /// The number of the Sealed Golden Box.
    /// </summary>
    internal const short SealedGoldenBoxNumber = 121;

    /// <summary>
    /// The number of the Sealed Silver Box.
    /// </summary>
    internal const short SealedSilverBoxNumber = 122;

    /// <summary>
    /// The number of the Golden Box.
    /// </summary>
    internal const short GoldenBoxNumber = 123;

    /// <summary>
    /// The number of the Silver Box.
    /// </summary>
    internal const short SilverBoxNumber = 124;

    /// <summary>
    /// The mix id which the client sends for both combinations (the goblin combination of mix.bmd).
    /// </summary>
    internal const byte CraftingNumber = 47;

    /// <summary>
    /// The item numbers of all keys and boxes.
    /// </summary>
    internal static readonly short[] ItemNumbers = [SilverKeyNumber, GoldKeyNumber, SealedGoldenBoxNumber, SealedSilverBoxNumber, GoldenBoxNumber, SilverBoxNumber];

    /// <summary>
    /// Initializes a new instance of the <see cref="GoblinBoxItems"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public GoblinBoxItems(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        this.CreateItems();
    }

    /// <summary>
    /// Creates the keys and boxes which don't exist yet, with the contents of the opened boxes.
    /// The rules which block trading, dropping and selling them are applied by the <see cref="ItemRules"/>.
    /// </summary>
    /// <returns>The created items.</returns>
    internal IReadOnlyList<ItemDefinition> CreateItems()
    {
        var created = new List<ItemDefinition>();
        this.TryCreateItem(SilverKeyNumber, LocalizedString.FromResource(() => ItemNames.SilverKey), created);
        this.TryCreateItem(GoldKeyNumber, LocalizedString.FromResource(() => ItemNames.GoldKey), created);
        this.TryCreateItem(SealedGoldenBoxNumber, LocalizedString.FromResource(() => ItemNames.SealedGoldenBox), created);
        this.TryCreateItem(SealedSilverBoxNumber, LocalizedString.FromResource(() => ItemNames.SealedSilverBox), created);
        if (this.TryCreateItem(GoldenBoxNumber, LocalizedString.FromResource(() => ItemNames.GoldenBox), created) is { } goldenBox)
        {
            this.AddGoldenBoxContents(goldenBox);
        }

        if (this.TryCreateItem(SilverBoxNumber, LocalizedString.FromResource(() => ItemNames.SilverBox), created) is { } silverBox)
        {
            this.AddSilverBoxContents(silverBox);
        }

        return created;
    }

    /// <summary>
    /// Adds the goblin combinations of the keys and sealed boxes to the chaos goblin, if they don't exist yet.
    /// Both have the same number, because the client sends the same mix id for them; the server tells them apart by the items.
    /// </summary>
    internal void AddCraftings()
    {
        if (this.GameConfiguration.Monsters.FirstOrDefault(m => m.NpcWindow == NpcWindow.ChaosMachine) is not { } chaosGoblin)
        {
            return;
        }

        this.TryAddCrafting(chaosGoblin, LocalizedString.FromResource(() => CraftingNames.GoldenBoxCombination), GoldKeyNumber, SealedGoldenBoxNumber, GoldenBoxNumber);
        this.TryAddCrafting(chaosGoblin, LocalizedString.FromResource(() => CraftingNames.SilverBoxCombination), SilverKeyNumber, SealedSilverBoxNumber, SilverBoxNumber);
    }

    private void TryAddCrafting(MonsterDefinition chaosGoblin, LocalizedString name, short keyNumber, short sealedBoxNumber, short resultNumber)
    {
        var key = this.FindItem(keyNumber);
        var sealedBox = this.FindItem(sealedBoxNumber);
        var result = this.FindItem(resultNumber);
        if (key is null || sealedBox is null || result is null
            || chaosGoblin.ItemCraftings.Any(c => c.Number == CraftingNumber && c.SimpleCraftingSettings?.ResultItems.Any(r => r.ItemDefinition == result) is true))
        {
            return;
        }

        var crafting = this.Context.CreateNew<ItemCrafting>();
        crafting.Name = name;
        crafting.Number = CraftingNumber;

        // Both craftings have the same number, so they need ids which tell them apart, e.g. to link their captions.
        crafting.SetGuid(CraftingNumber, resultNumber);
        var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
        crafting.SimpleCraftingSettings = craftingSettings;
        craftingSettings.SuccessPercent = 100;

        foreach (var requiredItem in new[] { key, sealedBox })
        {
            var required = this.Context.CreateNew<ItemCraftingRequiredItem>();
            required.MinimumAmount = 1;
            required.MaximumAmount = 1;
            required.SuccessResult = MixResult.Disappear;
            required.FailResult = MixResult.Disappear;
            required.PossibleItems.Add(requiredItem);
            craftingSettings.RequiredItems.Add(required);
        }

        var resultItem = this.Context.CreateNew<ItemCraftingResultItem>();
        resultItem.ItemDefinition = result;
        resultItem.Durability = 1;
        craftingSettings.ResultItems.Add(resultItem);

        chaosGoblin.ItemCraftings.Add(crafting);
    }

    private ItemDefinition? TryCreateItem(short number, LocalizedString name, List<ItemDefinition> created)
    {
        if (this.FindItem(number) is not null)
        {
            return null;
        }

        var item = this.Context.CreateNew<ItemDefinition>();
        item.Group = ItemGroup;
        item.Number = number;
        item.Name = name;
        item.Width = 1;
        item.Height = 1;
        item.Durability = 1;
        item.DropsFromMonsters = false;
        item.SetGuid(item.Group, item.Number);
        this.GameConfiguration.Items.Add(item);
        created.Add(item);
        return item;
    }

    private ItemDefinition? FindItem(short number)
    {
        return this.GameConfiguration.Items.FirstOrDefault(item => item.Group == ItemGroup && item.Number == number);
    }

    private void AddGoldenBoxContents(ItemDefinition box)
    {
        var excellent = this.CreateDropGroup(box, SpecialItemType.Excellent, 0.15, LocalizedString.FromResource(() => ItemDropDescriptions.GoldenBoxExcellentItems));
        this.TryAddDropItem(excellent, 0, 16); // Sword of Destruction
        this.TryAddDropItem(excellent, 2, 9); // Master Scepter
        this.TryAddDropItem(excellent, 4, 13); // Bluewing Crossbow
        this.TryAddDropItem(excellent, 5, 7); // Chaos Lightning Staff
        this.TryAddDropItem(excellent, 5, 23); // Book of Lagle
        this.TryAddDropItem(excellent, 0, 33); // Holy Storm Claw
        this.AddArmorSet(excellent, 16); // Black Dragon
        this.AddArmorSet(excellent, 18); // Grand Soul
        this.AddArmorSet(excellent, 19); // Divine
        this.AddArmorSet(excellent, 15); // Storm Crow
        this.AddArmorSet(excellent, 26); // Adamantine
        this.AddArmorSet(excellent, 41); // Ancient
        this.AddArmorSet(excellent, 60); // Storm Jahad

        var jewels = this.CreateDropGroup(box, SpecialItemType.RandomItem, 0.55, LocalizedString.FromResource(() => ItemDropDescriptions.GoldenBoxJewels));
        this.TryAddDropItem(jewels, 12, 15); // Jewel of Chaos
        this.TryAddDropItem(jewels, 14, 13); // Jewel of Bless
        this.TryAddDropItem(jewels, 14, 14); // Jewel of Soul
        this.TryAddDropItem(jewels, 14, 16); // Jewel of Life
        this.TryAddDropItem(jewels, 14, 22); // Jewel of Creation

        this.AddMoneyFallback(box, 1_000_000, LocalizedString.FromResource(() => ItemDropDescriptions.GoldenBoxMoney));
    }

    private void AddSilverBoxContents(ItemDefinition box)
    {
        var excellent = this.CreateDropGroup(box, SpecialItemType.Excellent, 0.05, LocalizedString.FromResource(() => ItemDropDescriptions.SilverBoxExcellentItems));
        this.TryAddDropItem(excellent, 0, 13); // Double Blade
        this.TryAddDropItem(excellent, 0, 15); // Giant Sword
        this.TryAddDropItem(excellent, 2, 8); // Battle Scepter
        this.TryAddDropItem(excellent, 4, 12); // Serpent Crossbow
        this.TryAddDropItem(excellent, 5, 4); // Gorgon Staff
        this.TryAddDropItem(excellent, 5, 21); // Book of Samut
        this.TryAddDropItem(excellent, 0, 32); // Sacred Glove
        this.AddArmorSet(excellent, 9); // Plate
        this.AddArmorSet(excellent, 3); // Legendary
        this.AddArmorSet(excellent, 13); // Spirit
        this.AddArmorSet(excellent, 25); // Light Plate
        this.AddArmorSet(excellent, 40); // Red Wing
        this.AddArmorSet(excellent, 59); // Sacred Fire

        var jewels = this.CreateDropGroup(box, SpecialItemType.RandomItem, 0.45, LocalizedString.FromResource(() => ItemDropDescriptions.SilverBoxJewels));
        this.TryAddDropItem(jewels, 12, 15); // Jewel of Chaos
        this.TryAddDropItem(jewels, 14, 13); // Jewel of Bless
        this.TryAddDropItem(jewels, 14, 14); // Jewel of Soul

        this.AddMoneyFallback(box, 500_000, LocalizedString.FromResource(() => ItemDropDescriptions.SilverBoxMoney));
    }

    private ItemDropItemGroup CreateDropGroup(ItemDefinition box, SpecialItemType itemType, double chance, LocalizedString description)
    {
        var dropGroup = this.Context.CreateNew<ItemDropItemGroup>();
        dropGroup.ItemType = itemType;
        dropGroup.Chance = chance;
        dropGroup.Description = description;
        dropGroup.DropEffect = ItemDropEffect.Fireworks;
        box.DropItems.Add(dropGroup);
        return dropGroup;
    }

    private void AddMoneyFallback(ItemDefinition box, int moneyAmount, LocalizedString description)
    {
        // The chances of the item groups are less than one, so the remaining chance gives money instead of nothing.
        var money = this.Context.CreateNew<ItemDropItemGroup>();
        money.ItemType = SpecialItemType.Money;
        money.MoneyAmount = moneyAmount;
        money.Chance = 1.0;
        money.Description = description;
        box.DropItems.Add(money);
    }

    private void AddArmorSet(ItemDropItemGroup dropGroup, short number)
    {
        this.TryAddDropItem(dropGroup, (byte)ItemGroups.Helm, number);
        this.TryAddDropItem(dropGroup, (byte)ItemGroups.Armor, number);
        this.TryAddDropItem(dropGroup, (byte)ItemGroups.Pants, number);
        this.TryAddDropItem(dropGroup, (byte)ItemGroups.Gloves, number);
        this.TryAddDropItem(dropGroup, (byte)ItemGroups.Boots, number);
    }

    private void TryAddDropItem(ItemDropItemGroup dropGroup, byte group, short number)
    {
        if (this.GameConfiguration.Items.FirstOrDefault(i => i.Group == group && i.Number == number) is { } item)
        {
            dropGroup.PossibleItems.Add(item);
        }
    }
}
