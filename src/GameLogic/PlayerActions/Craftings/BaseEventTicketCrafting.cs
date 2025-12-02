// <copyright file="BaseEventTicketCrafting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// Base class for a crafting of Event Tickets.
/// </summary>
public abstract class BaseEventTicketCrafting : BaseItemCraftingHandler
{
    private readonly string _requiredEventItemName1;

    private readonly string _requiredEventItemName2;

    private readonly string _resultItemName;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEventTicketCrafting" /> class.
    /// </summary>
    /// <param name="resultItemName">Name of the result item.</param>
    /// <param name="requiredEventItemName1">The name of the first required item.</param>
    /// <param name="requiredEventItemName2">The name of the second required item.</param>
    protected BaseEventTicketCrafting(string resultItemName, string requiredEventItemName1, string requiredEventItemName2)
    {
        this._resultItemName = resultItemName;
        this._requiredEventItemName1 = requiredEventItemName1;
        this._requiredEventItemName2 = requiredEventItemName2;
    }

    /// <summary>
    /// Gets the <see cref="CraftingResult"/> for a incorrect mix items result.
    /// </summary>
    protected virtual CraftingResult IncorrectMixItemsResult => CraftingResult.IncorrectMixItems;

    /// <inheritdoc />
    public override CraftingResult? TryGetRequiredItems(Player player, out IList<CraftingRequiredItemLink> itemLinks, out byte successRate)
    {
        successRate = 0;
        itemLinks = new List<CraftingRequiredItemLink>(3);

        var item1 = player.TemporaryStorage!.Items.FirstOrDefault(item => item.Definition?.Name == this._requiredEventItemName1);
        var item2 = player.TemporaryStorage.Items.FirstOrDefault(item => item.Definition?.Name == this._requiredEventItemName2);
        var chaos = player.TemporaryStorage.Items.FirstOrDefault(item => item.Definition?.Name == "Jewel of Chaos");
        if (item1 is null || item2 is null || item1.Level != item2.Level || chaos is null)
        {
            return this.IncorrectMixItemsResult;
        }

        itemLinks.Add(new CraftingRequiredItemLink(
            item1.GetAsEnumerable(),
            new TransientItemCraftingRequiredItem
            {
                PossibleItems = { item1.Definition! },
                MaximumAmount = 1,
                MinimumAmount = 1,
            }));
        itemLinks.Add(new CraftingRequiredItemLink(
            item2.GetAsEnumerable(),
            new TransientItemCraftingRequiredItem
            {
                PossibleItems = { item2.Definition! },
                MaximumAmount = 1,
                MinimumAmount = 1,
            }));
        itemLinks.Add(new CraftingRequiredItemLink(
            chaos.GetAsEnumerable(),
            new TransientItemCraftingRequiredItem
            {
                PossibleItems = { chaos.Definition! },
                MaximumAmount = 1,
                MinimumAmount = 1,
            }));

        successRate = this.GetSuccessRate(item1.Level);
        return default;
    }

    /// <inheritdoc />
    protected sealed override int GetPrice(byte successRate, IList<CraftingRequiredItemLink> requiredItems)
    {
        return this.GetPrice(this.GetEventLevel(requiredItems));
    }

    /// <summary>
    /// Gets the price of the crafting for the specified event level.
    /// </summary>
    /// <param name="eventLevel">The event level.</param>
    /// <returns>The price of the crafting for the specified event level.</returns>
    protected abstract int GetPrice(int eventLevel);

    /// <summary>
    /// Gets the success rate of the crafting for the specified event level.
    /// </summary>
    /// <param name="eventLevel">The event level.</param>
    /// <returns>The success rate of the crafting for the specified event level.</returns>
    protected abstract byte GetSuccessRate(int eventLevel);

    /// <inheritdoc />
    protected override async ValueTask<List<Item>> CreateOrModifyResultItemsAsync(IList<CraftingRequiredItemLink> requiredItems, Player player, byte socketIndex, byte successRate)
    {
        var item = player.PersistenceContext.CreateNew<Item>();
        item.Definition = player.GameContext.Configuration.Items.First(i => i.Name == this._resultItemName);
        item.Level = this.GetEventLevel(requiredItems);
        item.Durability = 1;
        if (player.TemporaryStorage is { } temporaryStorage)
        {
            await temporaryStorage!.AddItemAsync(item).ConfigureAwait(false);
        }

        return new List<Item> { item };
    }

    private byte GetEventLevel(IList<CraftingRequiredItemLink> requiredItems)
    {
        var item = requiredItems.First(ri => ri.Items.Any(i => i.Definition?.Name == this._requiredEventItemName1));
        return item.Items.First().Level;
    }
}