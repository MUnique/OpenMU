// <copyright file="LuckyItemTicketExchangeCrafting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// Crafting which exchanges a lucky item ticket for a lucky item which fits the character class of the player.
/// </summary>
/// <remarks>
/// Each required item of the settings stands for one ticket and has its own <see cref="ItemCraftingRequiredItem.Reference"/>.
/// The result items with the same reference are the items for which the ticket can be exchanged. Of these, the player
/// gets the one which the character class of the player can wear. When more than one fits, the one for the fewest
/// character classes is taken, e.g. the set which is made for the class, and then the one with the lowest item number.
/// Exactly one ticket is accepted per exchange, and only when the item fits into the inventory.
/// </remarks>
public class LuckyItemTicketExchangeCrafting : SimpleItemCraftingHandler
{
    private readonly SimpleCraftingSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="LuckyItemTicketExchangeCrafting"/> class.
    /// </summary>
    /// <param name="settings">The settings.</param>
    public LuckyItemTicketExchangeCrafting(SimpleCraftingSettings settings)
        : base(settings)
    {
        this._settings = settings;
    }

    /// <inheritdoc />
    public override CraftingResult? TryGetRequiredItems(Player player, out IList<CraftingRequiredItemLink> items, out byte successRate)
    {
        if (base.TryGetRequiredItems(player, out items, out successRate) is { } error)
        {
            return error;
        }

        if (items.Sum(link => link.Items.Count()) != 1)
        {
            return CraftingResult.IncorrectMixItems;
        }

        if (this.GetResultItemDefinition(player, items) is not { } definition)
        {
            // The ticket would be consumed without giving an item, e.g. a helm ticket for a magic gladiator.
            return CraftingResult.IncorrectMixItems;
        }

        if (player.Inventory?.CheckInvSpace(new TemporaryItem { Definition = definition }) is null)
        {
            // The armor is bigger than the ticket. If it doesn't fit into the inventory, it would be lost
            // when the player leaves before making room for it.
            return CraftingResult.TooManyItems;
        }

        return default;
    }

    /// <inheritdoc />
    protected override async ValueTask<List<Item>> CreateOrModifyResultItemsAsync(IList<CraftingRequiredItemLink> requiredItems, Player player, byte socketSlot, byte successRate)
    {
        if (this.GetResultItemDefinition(player, requiredItems) is not { } definition)
        {
            return [];
        }

        var item = player.PersistenceContext.CreateNew<Item>();
        item.Definition = definition;
        item.Durability = item.GetMaximumDurabilityOfOnePiece();
        await player.TemporaryStorage!.AddItemAsync(item).ConfigureAwait(false);
        return [item];
    }

    private ItemDefinition? GetResultItemDefinition(Player player, IList<CraftingRequiredItemLink> requiredItems)
    {
        if (player.SelectedCharacter?.CharacterClass is not { } characterClass
            || requiredItems.FirstOrDefault(link => link.Items.Any()) is not { } ticketLink)
        {
            return null;
        }

        return this._settings.ResultItems
            .Where(result => result.Reference == ticketLink.ItemRequirement.Reference)
            .Select(result => result.ItemDefinition)
            .OfType<ItemDefinition>()
            .Where(definition => definition.QualifiedCharacters.Any(c => c.Number == characterClass.Number))
            .OrderBy(definition => definition.QualifiedCharacters.Count)
            .ThenBy(definition => definition.Number)
            .FirstOrDefault();
    }
}
