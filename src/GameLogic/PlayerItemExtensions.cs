// <copyright file="PlayerItemExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Extensions for the items of a <see cref="Player"/>.
/// </summary>
public static class PlayerItemExtensions
{
    /// <summary>
    /// Determines whether the player complies with the requirements of the specified item.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="item">The item.</param>
    /// <returns><c>True</c>, if the player complies with the requirements of the specified item; Otherwise, <c>false</c>.</returns>
    public static bool CompliesRequirements(this Player player, Item item)
    {
        item.ThrowNotInitializedProperty(item.Definition is null, nameof(item.Definition));

        foreach (var requirement in item.Definition.Requirements.Select(item.GetRequirement))
        {
            if (player.Attributes![requirement.Attr] < requirement.Value)
            {
                return false;
            }
        }

        return item.Definition.QualifiedCharacters.Contains(player.SelectedCharacter!.CharacterClass!);
    }

    /// <summary>
    /// Destroys an item of the <see cref="Player.Inventory"/>.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="item">The item.</param>
    public static async ValueTask DestroyInventoryItemAsync(this Player player, Item item)
    {
        await player.Inventory!.RemoveItemAsync(item).ConfigureAwait(false);
        await player.PersistenceContext.DeleteAsync(item).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IItemRemovedPlugIn>(p => p.RemoveItemAsync(item.ItemSlot)).ConfigureAwait(false);
        player.GameContext.PlugInManager.GetPlugInPoint<IItemDestroyedPlugIn>()?.ItemDestroyed(item);
    }

    /// <summary>
    /// Logs the items of the vault of the account which have no item definition.
    /// </summary>
    /// <param name="player">The player.</param>
    internal static void LogInvalidVaultItems(this Player player)
    {
        var invalidItems = player.Account?.Vault?.Items.Where(i => i.Definition is null);
        if (invalidItems is null)
        {
            return;
        }

        foreach (var item in invalidItems)
        {
            player.Logger.LogWarning("Account {name} has item without definition in vault, Slot: {slot}, ID: {id}", player.Account?.LoginName, item.ItemSlot, item.GetId());
        }
    }

    /// <summary>
    /// Logs the items of the inventory of the selected character which have no item definition.
    /// </summary>
    /// <param name="player">The player.</param>
    internal static void LogInvalidInventoryItems(this Player player)
    {
        var invalidItems = player.SelectedCharacter?.Inventory?.Items.Where(i => i.Definition is null);
        if (invalidItems is null)
        {
            return;
        }

        foreach (var item in invalidItems)
        {
            player.Logger.LogWarning("Character {name} has item without definition in inventory, Slot: {slot}, ID: {id}", player.SelectedCharacter?.Name, item.ItemSlot, item.GetId());
        }
    }
}
