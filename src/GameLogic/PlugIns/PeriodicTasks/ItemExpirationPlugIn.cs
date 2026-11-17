// <copyright file="ItemExpirationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles the time-limited items of the players when they expire (see <see cref="Item.ExpiresAt"/>):
/// the player is told, an equipped item loses its effect, and the client shows the item as expired.
/// </summary>
/// <remarks>
/// An expired item stays in the inventory. That it has no effect doesn't depend on this plugin: the power-ups
/// of an expired item aren't created at all. The plugin only applies the change to an item which was equipped
/// when it expired, and updates the client. One periodic check for all players is enough, because the periods
/// are days; a timer per player would have to be updated with every change of the inventory.
/// An item which expired while the player was offline is reported at the first check after the login.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.ItemExpirationPlugIn_Name), Description = nameof(PlugInResources.ItemExpirationPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("3F264FD8-4431-49D5-8A5E-E600467CF728")]
public class ItemExpirationPlugIn : IPeriodicTaskPlugIn, ISupportCustomConfiguration<ItemExpirationConfiguration>, ISupportDefaultCustomConfiguration
{
    private readonly ConditionalWeakTable<Player, HashSet<Item>> _handledItems = new();

    /// <summary>
    /// The next check per game context. One plugin instance serves all game servers of a process, so each of them
    /// needs its own; otherwise the game server which comes first would take the check of the others.
    /// </summary>
    private readonly ConditionalWeakTable<GameContext, StrongBox<DateTime>> _nextRunUtc = new();

    /// <inheritdoc />
    public ItemExpirationConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        var now = DateTime.UtcNow;
        var nextRunUtc = this._nextRunUtc.GetValue(gameContext, _ => new StrongBox<DateTime>(DateTime.MinValue));
        if (now < nextRunUtc.Value)
        {
            return;
        }

        var configuration = this.Configuration ??= new ItemExpirationConfiguration();
        nextRunUtc.Value = now + configuration.CheckInterval;
        foreach (var player in await gameContext.GetPlayersAsync().ConfigureAwait(false))
        {
            if (player.PlayerState.CurrentState != PlayerState.EnteredWorld || player.Inventory is not { } inventory)
            {
                continue;
            }

            var handledItems = this._handledItems.GetOrCreateValue(player);
            foreach (var item in inventory.Items.Where(item => item.IsExpired(now) && !handledItems.Contains(item)).ToList())
            {
                handledItems.Add(item);
                await HandleExpiredItemAsync(player, item).ConfigureAwait(false);
            }
        }
    }

    /// <inheritdoc />
    public void ForceStart()
    {
        this._nextRunUtc.Clear();
    }

    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        return new ItemExpirationConfiguration();
    }

    private static async ValueTask HandleExpiredItemAsync(Player player, Item item)
    {
        try
        {
            if (player.Inventory?.EquippedItems.Contains(item) == true
                && player.Inventory.AsInventoryStorage is { } inventoryStorage)
            {
                // The power-ups of the item are created again, which gives none for an expired item.
                await inventoryStorage.RaiseEquippedItemsChangedAsync(item, true).ConfigureAwait(false);
            }

            await player.InvokeViewPlugInAsync<IItemUpgradedPlugIn>(p => p.ItemUpgradedAsync(item)).ConfigureAwait(false);
            var itemName = item.Definition?.Name.GetTranslation(player.Culture) ?? item.ToString();
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.ItemHasExpired), itemName).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Error when handling the expired item {item} of player {player}.", item, player);
        }
    }
}
