// <copyright file="RemoveSeedSphereCrafting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;

/// <summary>
/// Crafting to remove a mounted seed sphere from a socket item.
/// </summary>
public class RemoveSeedSphereCrafting : SimpleItemCraftingHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveSeedSphereCrafting"/> class.
    /// </summary>
    /// <param name="settings">The settings.</param>
    public RemoveSeedSphereCrafting(SimpleCraftingSettings settings)
        : base(settings)
    {
    }

    /// <summary>
    /// Gets the reference to the socket item which must be specified in the <see cref="ItemCraftingRequiredItem.Reference"/>.
    /// </summary>
    public static byte SocketItemReference { get; } = 0x88;

    /// <inheritdoc />
    protected override async ValueTask<List<Item>> CreateOrModifyResultItemsAsync(IList<CraftingRequiredItemLink> requiredItems, Player player, byte socketSlot, byte successRate)
    {
        var socketItem = requiredItems.Single(i => i.ItemRequirement.Reference == SocketItemReference).Items.Single();

        var socketOption = socketItem.ItemOptions
            .FirstOrDefault(optionLink => optionLink.ItemOption?.OptionType == ItemOptionTypes.SocketOption && optionLink.Index == socketSlot);
        if (socketOption is null)
        {
            throw new ArgumentException($"No seed sphere is mounted on the socket slot {socketSlot}.");
        }

        socketItem.ItemOptions.Remove(socketOption);
        await player.PersistenceContext.DeleteAsync(socketOption).ConfigureAwait(false);

        if (socketOption.Index < 3
            && socketItem.ItemOptions.FirstOrDefault(o => o.ItemOption?.OptionType == ItemOptionTypes.SocketBonusOption) is { } bonusOption)
        {
            socketItem.ItemOptions.Remove(bonusOption);
            await player.PersistenceContext.DeleteAsync(bonusOption).ConfigureAwait(false);
        }

        return new List<Item> { socketItem };
    }
}