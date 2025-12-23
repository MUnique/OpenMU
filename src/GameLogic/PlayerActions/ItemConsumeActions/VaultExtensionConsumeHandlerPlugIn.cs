// Copyright (c) MUnique. Licensed under the MIT license.

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consumes a "Vault Extension" item and enables the extended vault for the account.
/// </summary>
[Guid("6F5B09C7-2D0B-4C8E-8A11-3F6E6951E3E2")]
[PlugIn(nameof(VaultExtensionConsumeHandlerPlugIn), "Extends the vault when the corresponding item is used.")]
public class VaultExtensionConsumeHandlerPlugIn : BaseConsumeHandlerPlugIn
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.VaultExtension;

    /// <inheritdoc />
    public override async ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        if (!this.CheckPreconditions(player, item))
        {
            return false;
        }

        if (player.Account is null)
        {
            return false;
        }

        if (player.Account.IsVaultExtended)
        {
            var message = player.GetLocalizedMessage(
                "VaultExtension_Message_AlreadyExtended",
                "Your vault is already extended.");
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.BlueNormal)).ConfigureAwait(false);
            return false;
        }

        player.Account.IsVaultExtended = true;

        // Consume the source item (reduce durability / delete).
        await this.ConsumeSourceItemAsync(player, item).ConfigureAwait(false);
        return true;
    }
}
