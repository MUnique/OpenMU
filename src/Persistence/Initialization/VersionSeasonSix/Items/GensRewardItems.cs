// <copyright file="GensRewardItems.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Properties;

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
    /// Gets the jewellery cases with their number, name, the descriptions of their drop groups and the money which they give instead of a jewel.
    /// </summary>
    internal static IReadOnlyList<(short Number, LocalizedString Name, LocalizedString JewelsDescription, LocalizedString MoneyDescription, int Money)> Cases { get; } =
    [
        (141, LocalizedString.FromResource(() => ItemNames.ShiningJewelleryCase), LocalizedString.FromResource(() => ItemDropDescriptions.ShiningJewelleryCaseJewels), LocalizedString.FromResource(() => ItemDropDescriptions.ShiningJewelleryCaseMoney), 90_000),
        (142, LocalizedString.FromResource(() => ItemNames.ElegantJewelleryCase), LocalizedString.FromResource(() => ItemDropDescriptions.ElegantJewelleryCaseJewels), LocalizedString.FromResource(() => ItemDropDescriptions.ElegantJewelleryCaseMoney), 90_000),
        (143, LocalizedString.FromResource(() => ItemNames.SteelJewelleryCase), LocalizedString.FromResource(() => ItemDropDescriptions.SteelJewelleryCaseJewels), LocalizedString.FromResource(() => ItemDropDescriptions.SteelJewelleryCaseMoney), 60_000),
        (144, LocalizedString.FromResource(() => ItemNames.OldJewelleryCase), LocalizedString.FromResource(() => ItemDropDescriptions.OldJewelleryCaseJewels), LocalizedString.FromResource(() => ItemDropDescriptions.OldJewelleryCaseMoney), 60_000),
    ];

    /// <inheritdoc />
    public override void Initialize()
    {
        foreach (var jewelleryCase in Cases)
        {
            CreateCase(this.Context, this.GameConfiguration, jewelleryCase);
        }
    }

    /// <summary>
    /// Creates a jewellery case, if it doesn't exist yet.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="jewelleryCase">The jewellery case of <see cref="Cases"/>.</param>
    internal static void CreateCase(IContext context, GameConfiguration gameConfiguration, (short Number, LocalizedString Name, LocalizedString JewelsDescription, LocalizedString MoneyDescription, int Money) jewelleryCase)
    {
        var (number, name, jewelsDescription, moneyDescription, money) = jewelleryCase;
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
        jewels.Description = jewelsDescription;
        AddPossibleItem(gameConfiguration, jewels, 12, 15); // Jewel of Chaos
        AddPossibleItem(gameConfiguration, jewels, 14, 13); // Jewel of Bless
        AddPossibleItem(gameConfiguration, jewels, 14, 14); // Jewel of Soul
        AddPossibleItem(gameConfiguration, jewels, 14, 16); // Jewel of Life
        AddPossibleItem(gameConfiguration, jewels, 14, 22); // Jewel of Creation
        AddPossibleItem(gameConfiguration, jewels, 14, 41); // Gemstone
        if (jewels.PossibleItems.Count > 0)
        {
            // Without any possible item, an opened case would give nothing instead of money.
            item.DropItems.Add(jewels);
        }

        var moneyDrop = context.CreateNew<ItemDropItemGroup>();
        moneyDrop.ItemType = SpecialItemType.Money;
        moneyDrop.MoneyAmount = money;
        moneyDrop.Chance = 1.0;
        moneyDrop.Description = moneyDescription;
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
