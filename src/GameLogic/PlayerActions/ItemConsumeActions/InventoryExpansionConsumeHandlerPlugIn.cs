// Copyright (c) MUnique. Licensed under the MIT license.

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consumes an "Inventory Expansion" item and increases the characters inventory extensions by 1, up to the maximum.
/// </summary>
[Guid("8F03D1B8-4A11-4F2E-A9C3-4D6F3C0C7B8E")]
[PlugIn(nameof(InventoryExpansionConsumeHandlerPlugIn), "Expands the character inventory by one extension when the corresponding item is used.")]
public class InventoryExpansionConsumeHandlerPlugIn : BaseConsumeHandlerPlugIn
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.InventoryExpansion;

    /// <inheritdoc />
    public override async ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        if (!this.CheckPreconditions(player, item))
        {
            return false;
        }

        var current = player.SelectedCharacter!.InventoryExtensions;
        if (current >= InventoryConstants.MaximumNumberOfExtensions)
        {
            var message = player.GetLocalizedMessage(
                "InventoryExpansion_Message_MaxReached",
                "Your inventory is already fully expanded.");
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.BlueNormal)).ConfigureAwait(false);
            return false;
        }

        player.SelectedCharacter.InventoryExtensions = Math.Min(current + 1, InventoryConstants.MaximumNumberOfExtensions);

        // Inform client about changed inventory size via character stats packet.
        await player.InvokeViewPlugInAsync<IUpdateCharacterStatsPlugIn>(p => p.UpdateCharacterStatsAsync()).ConfigureAwait(false);

        // Consume the source item (reduce durability / delete).
        await this.ConsumeSourceItemAsync(player, item).ConfigureAwait(false);
        return true;
    }
}
