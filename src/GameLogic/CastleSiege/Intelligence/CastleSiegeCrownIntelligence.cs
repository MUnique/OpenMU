// <copyright file="CastleSiegeCrownIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Intelligence;

using System.Diagnostics;
using System.Threading;
using MUnique.OpenMU.GameLogic.CastleSiege.NPC;

/// <summary>
/// Tracks the player operating the Castle Siege Crown.
/// </summary>
public sealed class CastleSiegeCrownIntelligence : CastleSiegeNpcIntelligenceBase, IDisposable
{
    private static readonly TimeSpan TrackingInterval = TimeSpan.FromSeconds(1);
    private readonly CastleSiegeContext _context;
    private Timer? _timer;
    private int _isTicking;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeCrownIntelligence"/> class.
    /// </summary>
    /// <param name="context">The Castle Siege context.</param>
    public CastleSiegeCrownIntelligence(CastleSiegeContext context)
    {
        this._context = context;
    }

    /// <inheritdoc />
    public override void Start()
    {
        this._timer ??= new Timer(
            static state => _ = ((CastleSiegeCrownIntelligence)state!).SafeTickAsync(),
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
        if (this.Npc is not CastleSiegeCrown crown)
        {
            return ValueTask.CompletedTask;
        }

        if (this._context.CurrentState != CastleSiegeState.Start)
        {
            this._context.CrownUser = null;
            crown.State = CastleSiegeCrownState.Locked;
            return ValueTask.CompletedTask;
        }

        var candidate = crown.CurrentMap.GetAttackablesInRange(crown.Position, 1)
            .OfType<Player>()
            .Where(player => player.IsAlive
                             && player.CurrentMap == crown.CurrentMap)
            .MinBy(player => player.Id);
        this._context.CrownUser = candidate;
        crown.State = this._context.IsCrownAvailable
            ? CastleSiegeCrownState.Idle
            : CastleSiegeCrownState.Locked;
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
