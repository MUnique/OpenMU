// <copyright file="GensRewardItems.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Initializer for the jewellery cases, which the gens members get as monthly reward.
/// Like other boxes, they're opened by dropping them, and give one jewel or some money.
/// </summary>
internal class GensRewardItems : InitializerBase
{
    /// <summary>
    /// The item group of the jewellery cases.
    /// </summary>
    internal const byte ItemGroup = 14;

    /// <summary>
    /// The chance that a jewellery case gives a jewel instead of money.
    /// </summary>
    /// <remarks>
    /// The known item data lists the possible contents, but no chances.
    /// </remarks>
    private const double JewelChance = 0.5;

    /// <summary>
    /// Initializes a new instance of the <see cref="GensRewardItems"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public GensRewardItems(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Gets the jewellery cases with their number, name and the money which they give instead of a jewel.
    /// </summary>
    internal static IReadOnlyList<(short Number, string Name, int Money)> Cases { get; } =
    [
        (141, "Shining Jewellery Case", 90_000),
        (142, "Elegant Jewellery Case", 90_000),
        (143, "Steel Jewellery Case", 60_000),
        (144, "Old Jewellery Case", 60_000),
    ];

    /// <inheritdoc />
    public override void Initialize()
    {
        foreach (var (number, name, money) in Cases)
        {
            CreateCase(this.Context, this.GameConfiguration, number, name, money);
        }
    }

    /// <summary>
    /// Creates a jewellery case, if it doesn't exist yet.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="number">The item number.</param>
    /// <param name="name">The name.</param>
    /// <param name="money">The money which it gives instead of a jewel.</param>
    internal static void CreateCase(IContext context, GameConfiguration gameConfiguration, short number, string name, int money)
    {
        if (gameConfiguration.Items.Any(item => item.Group == ItemGroup && item.Number == number))
        {
            return;
        }

        var item = context.CreateNew<ItemDefinition>();
        item.Group = ItemGroup;
        item.Number = number;
        item.Name = name;
        item.Width = 1;
        item.Height = 1;
        item.Durability = 1;
        item.DropsFromMonsters = false;
        item.SetGuid(item.Group, item.Number);
        gameConfiguration.Items.Add(item);

        var jewels = context.CreateNew<ItemDropItemGroup>();
        jewels.ItemType = SpecialItemType.RandomItem;
        jewels.Chance = JewelChance;
        jewels.Description = $"{name} (Jewels)";
        AddPossibleItem(gameConfiguration, jewels, 12, 15); // Jewel of Chaos
        AddPossibleItem(gameConfiguration, jewels, 14, 13); // Jewel of Bless
        AddPossibleItem(gameConfiguration, jewels, 14, 14); // Jewel of Soul
        AddPossibleItem(gameConfiguration, jewels, 14, 16); // Jewel of Life
        AddPossibleItem(gameConfiguration, jewels, 14, 22); // Jewel of Creation
        AddPossibleItem(gameConfiguration, jewels, 14, 41); // Gemstone
        item.DropItems.Add(jewels);

        var moneyDrop = context.CreateNew<ItemDropItemGroup>();
        moneyDrop.ItemType = SpecialItemType.Money;
        moneyDrop.MoneyAmount = money;
        moneyDrop.Chance = 1.0;
        moneyDrop.Description = $"{name} (Money)";
        item.DropItems.Add(moneyDrop);
    }

    private static void AddPossibleItem(GameConfiguration gameConfiguration, ItemDropItemGroup dropGroup, byte group, short number)
    {
        if (gameConfiguration.Items.FirstOrDefault(item => item.Group == group && item.Number == number) is { } item)
        {
            dropGroup.PossibleItems.Add(item);
        }
    }
}
