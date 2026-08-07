// <copyright file="CastleSiegeSwitchIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Intelligence;

using System.Diagnostics;
using System.Threading;
using MUnique.OpenMU.GameLogic.CastleSiege.NPC;

/// <summary>
/// Tracks the player standing at a Castle Siege Crown switch.
/// </summary>
public sealed class CastleSiegeSwitchIntelligence : CastleSiegeNpcIntelligenceBase, IDisposable
{
    private static readonly TimeSpan TrackingInterval = TimeSpan.FromSeconds(1);
    private readonly CastleSiegeContext _context;
    private Timer? _timer;
    private int _isTicking;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeSwitchIntelligence"/> class.
    /// </summary>
    /// <param name="context">The Castle Siege context.</param>
    public CastleSiegeSwitchIntelligence(CastleSiegeContext context)
    {
        this._context = context;
    }

    /// <inheritdoc />
    public override void Start()
    {
        this._timer ??= new Timer(
            static state => _ = ((CastleSiegeSwitchIntelligence)state!).SafeTickAsync(),
            this,
            TrackingInterval,
            TrackingInterval);
    }

    /// <inheritdoc />
    public override void Pause()
    {
        this._timer?.Dispose();
        this._timer = null;
    }

    /// <summary>
    /// Executes one player-tracking tick.
    /// </summary>
    /// <returns>A task that represents the asynchronous tracking operation.</returns>
    public ValueTask TickAsync()
    {
        if (this.Npc is not CastleSiegeSwitch siegeSwitch)
        {
            return ValueTask.CompletedTask;
        }

        var candidate = this._context.CurrentState == CastleSiegeState.Start
            ? siegeSwitch.CurrentMap.GetAttackablesInRange(siegeSwitch.Position, 1)
                .OfType<Player>()
                .Where(player => player.IsAlive
                                 && player.CurrentMap == siegeSwitch.CurrentMap)
                .MinBy(player => player.Id)
            : null;
        siegeSwitch.Occupant = candidate;
        this._context.SwitchUsers[siegeSwitch.SwitchIndex] = candidate;
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.Pause();
    }

    private async Task SafeTickAsync()
    {
        if (Interlocked.Exchange(ref this._isTicking, 1) != 0)
        {
            return;
        }

        try
        {
            await this.TickAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Debug.Fail(ex.Message, ex.StackTrace);
        }
        finally
        {
            Volatile.Write(ref this._isTicking, 0);
        }
    }
}
