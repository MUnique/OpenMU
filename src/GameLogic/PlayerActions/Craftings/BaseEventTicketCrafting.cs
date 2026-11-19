// <copyright file="BaseEventTicketCrafting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

using System.Diagnostics.CodeAnalysis;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Crywolf;
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

    /// <summary>
    /// Gets the maximum success rate, which also limits the additional success rate of a defended crywolf fortress.
    /// </summary>
    protected virtual byte MaximumSuccessRate => 100;

    /// <summary>
    /// Gets the type of the mini game whose ticket the crafting creates. Its definitions tell the levels of the event;
    /// without a type, the ingredients of every level are combined.
    /// </summary>
    /// <remarks>
    /// It's virtual, not abstract, so that craftings of plug-in assemblies which derive from this class keep working.
    /// </remarks>
    protected virtual MiniGameType? EventType => null;

    /// <inheritdoc />
    public override CraftingResult? TryGetRequiredItems(Player player, out IList<CraftingRequiredItemLink> items, out byte successRateByItems)
    {
        successRateByItems = 0;
        items = new List<CraftingRequiredItemLink>(3);

        var chaos = player.TemporaryStorage!.Items.FirstOrDefault(item => HasName(item, "Jewel of Chaos"));
        if (chaos is null)
        {
            return this.IncorrectMixItemsResult;
        }

        if (!this.TryFindIngredients(player, out var item1, out var item2, out var resultDefinition, out var error))
        {
            return error;
        }

        // The result, e.g. a 2x2 ticket, may be bigger than the ingredients: other items in the chaos machine are
        // refused when they leave it no place after the ingredients are removed, as it would end up in no storage.
        // The ingredients are right, so it's answered like the other craftings which get too many items.
        if (!player.TemporaryStorage.FitsAfterRemoval(resultDefinition, [item1, item2, chaos]))
        {
            return CraftingResult.TooManyItems;
        }

        items.Add(new CraftingRequiredItemLink(
            item1.GetAsEnumerable(),
            new TransientItemCraftingRequiredItem
            {
                PossibleItems = { item1.Definition! },
                MaximumAmount = 1,
                MinimumAmount = 1,
            }));
        items.Add(new CraftingRequiredItemLink(
            item2.GetAsEnumerable(),
            new TransientItemCraftingRequiredItem
            {
                PossibleItems = { item2.Definition! },
                MaximumAmount = 1,
                MinimumAmount = 1,
            }));
        items.Add(new CraftingRequiredItemLink(
            chaos.GetAsEnumerable(),
            new TransientItemCraftingRequiredItem
            {
                PossibleItems = { chaos.Definition! },
                MaximumAmount = 1,
                MinimumAmount = 1,
            }));

        var crywolfBenefit = CrywolfPlugIn.GetContext(player.GameContext)?.ChaosRateBenefit ?? 0;
        successRateByItems = (byte)Math.Min(this.GetSuccessRate(item1.Level) + crywolfBenefit, this.MaximumSuccessRate);
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
    protected override async ValueTask<List<Item>> CreateOrModifyResultItemsAsync(IList<CraftingRequiredItemLink> requiredItems, Player player, byte socketSlot, byte successRate)
    {
        var item = player.PersistenceContext.CreateNew<Item>();
        item.Level = this.GetEventLevel(requiredItems);
        item.Definition = this.GetResultItemDefinition(player, item.Level)
                          ?? throw new InvalidOperationException($"The item definition of the result item '{this._resultItemName}' doesn't exist.");
        item.Durability = 1;
        if (player.TemporaryStorage is { } temporaryStorage)
        {
            await temporaryStorage!.AddItemAsync(item).ConfigureAwait(false);
        }

        return new List<Item> { item };
    }

    /// <summary>
    /// Gets the item definition of the ticket which the crafting creates.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="eventLevel">The event level, which is the item level of the ticket.</param>
    /// <returns>The item definition with the name of the result item, or <c>null</c> when it doesn't exist.</returns>
    protected virtual ItemDefinition? GetResultItemDefinition(Player player, byte eventLevel)
    {
        return player.GameContext.Configuration.Items.FirstOrDefault(definition => HasName(definition, this._resultItemName));
    }

    /// <summary>
    /// Determines whether the item has the specified name in the neutral language. It's compared without creating a
    /// string for each localized name, as the items are checked on every preview of the chaos machine.
    /// </summary>
    private static bool HasName(Item item, string name)
    {
        return HasName(item.Definition, name);
    }

    /// <summary>
    /// Determines whether the item definition has the specified name in the neutral language. It's compared without
    /// creating a string for each localized name, as the definitions are looked up on every preview of the chaos machine.
    /// </summary>
    private static bool HasName(ItemDefinition? definition, string name)
    {
        return definition is not null && definition.Name.ValueInNeutralLanguageAsSpan.Equals(name, StringComparison.Ordinal);
    }

    /// <summary>
    /// Finds the two ingredients of the same level, which is the event level, and the ticket which they give. Other
    /// items may be in the chaos machine besides them, also more ingredients of another level, so the first pair of the
    /// same level which gives a ticket is taken, not the first item of each name. A pair which gives no ticket is
    /// skipped; when no pair gives one, the reason of the first one is the error.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="item1">The first ingredient.</param>
    /// <param name="item2">The second ingredient.</param>
    /// <param name="resultDefinition">The item definition of the ticket.</param>
    /// <param name="error">The reason why no pair gives a ticket; <c>null</c>, if a pair gives one.</param>
    /// <returns><c>True</c>, if a pair gives a ticket; Otherwise, <c>false</c>.</returns>
    private bool TryFindIngredients(
        Player player,
        [NotNullWhen(true)] out Item? item1,
        [NotNullWhen(true)] out Item? item2,
        [NotNullWhen(true)] out ItemDefinition? resultDefinition,
        [NotNullWhen(false)] out CraftingResult? error)
    {
        var storage = player.TemporaryStorage!;
        CraftingResult? firstError = null;
        foreach (var first in storage.Items.Where(item => HasName(item, this._requiredEventItemName1)))
        {
            // The level and the ticket depend only on the first ingredient, so they're checked once for all its
            // partners, as this runs on every preview of the chaos machine. The partners are collected in one pass
            // through the items.
            var partners = storage.Items.Where(item => HasName(item, this._requiredEventItemName2) && item.Level == first.Level).ToList();
            if (partners.Count == 0)
            {
                continue;
            }

            // Ingredients of a level which the event doesn't have, e.g. +0 ones of a game master, would give a
            // ticket which no event accepts, at the price of an unknown level.
            if (!this.IsLevelOfTheEvent(player, first.Level))
            {
                firstError ??= CraftingResult.InvalidItemLevel;
                continue;
            }

            // The ticket itself is no ingredient, also when its item definition got the name of one: the
            // combination would take it and give it back, and lose it when it fails. Without a ticket, there's
            // nothing to combine.
            var result = this.GetResultItemDefinition(player, first.Level);
            if (result is not null && !result.Equals(first.Definition)
                && partners.Find(partner => !result.Equals(partner.Definition)) is { } second)
            {
                item1 = first;
                item2 = second;
                resultDefinition = result;
                error = null;
                return true;
            }

            firstError ??= this.IncorrectMixItemsResult;
        }

        item1 = null;
        item2 = null;
        resultDefinition = null;
        error = firstError ?? this.IncorrectMixItemsResult;
        return false;
    }

    /// <summary>
    /// Determines whether a mini game definition of the event has the level, i.e. takes a ticket of this item level.
    /// Without an <see cref="EventType"/>, every level is one of the event.
    /// </summary>
    private bool IsLevelOfTheEvent(Player player, byte level)
    {
        return this.EventType is not { } type
               || player.GameContext.Configuration.MiniGameDefinitions.Any(definition => definition.Type == type && definition.TicketItemLevel == level);
    }

    private byte GetEventLevel(IList<CraftingRequiredItemLink> requiredItems)
    {
        var item = requiredItems.First(ri => ri.Items.Any(i => HasName(i, this._requiredEventItemName1)));
        return item.Items.First().Level;
    }
}