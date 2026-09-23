// <copyright file="KanturuTransitionRunner.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

using System.Threading;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Runs the transition into the Nightmare zone: cinematic, move, warp animation.
/// </summary>
/// <remarks>
/// The detail state triggers the full client cinematic; players are moved only after
/// that, so the movement isn't visible during the animation.
/// </remarks>
internal sealed class KanturuTransitionRunner : IKanturuPhaseRunner
{
    private readonly Func<KanturuState, byte, ValueTask> _showStateAsync;
    private readonly Func<Func<Player, Task>, ValueTask> _forEachPlayerAsync;
    private readonly Action _clearPhase;

    /// <summary>
    /// Initializes a new instance of the <see cref="KanturuTransitionRunner"/> class.
    /// </summary>
    /// <param name="showStateAsync">Broadcasts a state change to the clients.</param>
    /// <param name="forEachPlayerAsync">Executes an action for each player.</param>
    /// <param name="clearPhase">Clears the current phase of the kill tracker.</param>
    public KanturuTransitionRunner(
        Func<KanturuState, byte, ValueTask> showStateAsync,
        Func<Func<Player, Task>, ValueTask> forEachPlayerAsync,
        Action clearPhase)
    {
        this._showStateAsync = showStateAsync;
        this._forEachPlayerAsync = forEachPlayerAsync;
        this._clearPhase = clearPhase;
    }

    /// <inheritdoc />
    public KanturuPhaseKind Kind => KanturuPhaseKind.Transition;

    /// <inheritdoc />
    public async Task<bool> RunAsync(KanturuPhaseDefinition phase, CancellationToken cancellationToken)
    {
        var transition = phase.Transition ?? new KanturuTransitionDefinition();
        this._clearPhase();

        await this._showStateAsync(phase.State, phase.DetailState).ConfigureAwait(false);

        // The cinematic is never cancelled in the middle, so it uses no cancellation token.
        await Task.Delay(transition.CinematicDuration).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();

        var entryPoint = new Point(transition.EntryPointX, transition.EntryPointY);
        await this._forEachPlayerAsync(player => player.MoveAsync(entryPoint).AsTask()).ConfigureAwait(false);

        await Task.Delay(transition.WarpAnimationDelay).ConfigureAwait(false);
        await this._forEachPlayerAsync(player =>
            player.InvokeViewPlugInAsync<IMapChangePlugIn>(p =>
                p.MapChangeFailedAsync()).AsTask()).ConfigureAwait(false);
        return true;
    }
}
