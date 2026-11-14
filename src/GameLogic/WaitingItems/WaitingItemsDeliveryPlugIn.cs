// <copyright file="WaitingItemsDeliveryPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.WaitingItems;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Puts the waiting items of the characters into their inventories, as soon as they fit there:
/// when the character entered the game or opened the vault, and then periodically.
/// </summary>
/// <remarks>
/// It checks periodically instead of reacting on each item action, because an action like moving
/// an item frees a place and takes it again; delivering in between would take the place away.
/// The delivery runs while holding the persistence lock of the player, so it doesn't run in the
/// middle of a packet handler, and the lock of the personal store, so it doesn't run in the middle of a purchase
/// in it. It only delivers while no dialog is open (e.g. trade, chaos machine).
/// When this plug-in is deactivated, the items keep waiting until it's activated again.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.WaitingItemsDeliveryPlugIn_Name), Description = nameof(PlugInResources.WaitingItemsDeliveryPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("112A4B4F-814B-415B-9A35-FB3C08C0AA87")]
public class WaitingItemsDeliveryPlugIn : IPeriodicTaskPlugIn, IPlayerStateChangedPlugIn, ISupportCustomConfiguration<WaitingItemsDeliveryPlugInConfiguration>, ISupportDefaultCustomConfiguration
{
    private DateTime _nextRunUtc = DateTime.UtcNow;

    /// <inheritdoc />
    public WaitingItemsDeliveryPlugInConfiguration? Configuration { get; set; }

    private WaitingItemsDeliveryPlugInConfiguration CurrentConfiguration => this.Configuration ??= new WaitingItemsDeliveryPlugInConfiguration();

    /// <summary>
    /// Gets the active instance of this plug-in of the game, if there is one.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The active instance, or <c>null</c> when it's deactivated.</returns>
    public static WaitingItemsDeliveryPlugIn? GetActive(IGameContext gameContext)
    {
        return gameContext.PlugInManager.GetActivePlugInsOf<IPlayerStateChangedPlugIn>().OfType<WaitingItemsDeliveryPlugIn>().FirstOrDefault();
    }

    /// <inheritdoc />
    public async ValueTask PlayerStateChangedAsync(Player player, State previousState, State currentState)
    {
        if (previousState != PlayerState.CharacterSelection
            || currentState != PlayerState.EnteredWorld
            || !player.HasDeliverableWaitingItems())
        {
            return;
        }

        await player.RunPersistenceExclusiveAsync(() => this.DeliverAndShowAsync(player)).ConfigureAwait(false);
    }

    /// <summary>
    /// Puts the waiting items of the player into the inventory, as far as they fit there, and tells the player
    /// which items still wait: when the character entered the game, and when items were put aside when the vault was opened.
    /// An error is logged instead of thrown, so that it doesn't stop the action which called it.
    /// </summary>
    /// <param name="player">The player.</param>
    public async ValueTask DeliverAndShowAsync(Player player)
    {
        try
        {
            // A purchase in the personal store changes the inventory of the seller while holding this lock,
            // and the vault can be opened while the store is open.
            using (await player.ShopStorage!.StoreLock.LockAsync().ConfigureAwait(false))
            {
                var commandIsActive = player.GameContext.PlugInManager.GetStrategy<IChatCommandPlugIn>(WaitingItemsChatCommandPlugIn.Command) is not null;
                await player.DeliverWaitingItemsAsync().ConfigureAwait(false);
                await player.ShowWaitingItemsAsync(this.CurrentConfiguration.ItemsNamed, pointToCommand: commandIsActive).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Error when delivering the waiting items of player {Player}.", player);
        }
    }

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        var now = DateTime.UtcNow;
        if (now < this._nextRunUtc)
        {
            return;
        }

        this._nextRunUtc = now + this.CurrentConfiguration.CheckInterval;
        foreach (var player in await gameContext.GetPlayersAsync().ConfigureAwait(false))
        {
            // Reading the count is safe without the lock; the items are only looked at while holding it,
            // because opening the vault or a delivery may change them at the same time.
            if (player.SelectedCharacter?.WaitingItems is not { Count: > 0 })
            {
                continue;
            }

            try
            {
                await player.RunPersistenceExclusiveAsync(async () =>
                {
                    if (player.PlayerState.CurrentState != PlayerState.EnteredWorld
                        || !player.HasDeliverableWaitingItems())
                    {
                        return;
                    }

                    // A purchase in the personal store changes the inventory of the seller while holding this lock,
                    // but not the persistence lock of the seller.
                    using (await player.ShopStorage!.StoreLock.LockAsync().ConfigureAwait(false))
                    {
                        await player.DeliverWaitingItemsAsync().ConfigureAwait(false);
                    }
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                player.Logger.LogError(ex, "Error when delivering the waiting items of player {Player}.", player);
            }
        }
    }

    /// <inheritdoc />
    public void ForceStart()
    {
        this._nextRunUtc = DateTime.UtcNow;
    }

    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        return new WaitingItemsDeliveryPlugInConfiguration();
    }
}
