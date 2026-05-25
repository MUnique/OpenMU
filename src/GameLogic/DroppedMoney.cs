// <copyright file="DroppedMoney.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Diagnostics;
using System.Threading;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Pathfinding;
using Nito.AsyncEx;

/// <summary>
/// Money which got dropped on the ground of a map.
/// </summary>
public sealed class DroppedMoney : AsyncDisposable, ILocateable
{
    /// <summary>
    /// Gets the pickup lock. Used to synchronize pick up requests from the players.
    /// </summary>
    private readonly AsyncLock _pickupLock;

    private Timer? _removeTimer;

    private bool _availableToPick = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="DroppedMoney" /> class.
    /// </summary>
    /// <param name="amount">The amount.</param>
    /// <param name="position">The position where the item was dropped on the map.</param>
    /// <param name="map">The map.</param>
    public DroppedMoney(uint amount, Point position, GameMap map)
    {
        this.Amount = amount;
        this._pickupLock = new();
        this.Position = position;
        this.CurrentMap = map;
        this._removeTimer = new Timer(this.OnTimerTimeout, null, (int)map.ItemDropDuration.TotalMilliseconds, Timeout.Infinite);
    }

    /// <summary>
    /// Gets the money item.
    /// </summary>
    public uint Amount { get; }

    /// <inheritdoc />
    public Point Position { get; set; }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public ushort Id { get; set; }

    /// <inheritdoc/>
    public GameMap CurrentMap { get; }

    /// <summary>
    /// Tries to pick the money by the specified player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>
    /// The success.
    /// </returns>
    /// <remarks>
    /// Can be overwritten, for example for quest items which dropped only for a specific player.
    /// </remarks>
    public async ValueTask<bool> TryPickUpByAsync(Player player)
    {
        player.Logger.LogDebug("Player {0} tries to pick up {1}", player, this);

        using (await this._pickupLock.LockAsync())
        {
            if (!this._availableToPick)
            {
                player.Logger.LogDebug("Picked up by another player in the mean time, Player {0}, Money {1}", player, this);
                return false;
            }

            this._availableToPick = false;
        }

        if (player.Party is { } party)
        {
            var partyMembers = party.PartyList
                .OfType<Player>()
                .Where(p => p.CurrentMap == player.CurrentMap && !p.IsAtSafezone() && p.Attributes is { })
                .ToList();

            if (partyMembers.Count > 0)
            {
                var share = (int)(this.Amount / partyMembers.Count);
                foreach (var member in partyMembers)
                {
                    member.TryAddMoney((int)(share * member.Attributes![Stats.MoneyAmountRate]));
                }
            }
        }
        else
        {
            var clampMoneyOnPickup = player.GameContext?.Configuration?.ClampMoneyOnPickup ?? false;
            if (clampMoneyOnPickup)
            {
                var maxMoney = player.GameContext?.Configuration?.MaximumInventoryMoney ?? int.MaxValue;
                var currentMoney = player.Money;
                var amountToAdd = (int)Math.Min(this.Amount, (uint)Math.Max(0, maxMoney - currentMoney));

                if (amountToAdd <= 0)
                {
                    player.Logger.LogDebug("Player is at maximum money limit, Player {0}, Money {1}", player, this);
                    return false;
                }

                if (!player.TryAddMoney(amountToAdd))
                {
                    player.Logger.LogDebug("Money could not be added to the inventory, Player {0}, Money {1}", player, this);
                    return false;
                }
            }
            else
            {
                if (!player.TryAddMoney((int)this.Amount))
                {
                    player.Logger.LogDebug("Money could not be added to the inventory, Player {0}, Money {1}", player, this);
                    return false;
                }
            }
        }

        await this.DisposeAsync().ConfigureAwait(false);

        return true;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"Money: {this.Amount} at {this.CurrentMap.Definition.Name} ({this.Position})";
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        if (this._removeTimer is { } timer)
        {
            try
            {
                this._removeTimer = null;
                await timer.DisposeAsync().ConfigureAwait(false);
                await this.CurrentMap.RemoveAsync(this).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.Fail(e.Message, e.StackTrace);
            }
        }

        await base.DisposeAsyncCore().ConfigureAwait(false);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    private async void OnTimerTimeout(object? state)
    {
        try
        {
            await this.DisposeAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Debug.Fail(ex.Message, ex.StackTrace);
        }
    }
}