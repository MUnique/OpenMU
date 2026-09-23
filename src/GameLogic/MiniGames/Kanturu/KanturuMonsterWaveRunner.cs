// <copyright file="KanturuMonsterWaveRunner.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

using System.Threading;

/// <summary>
/// Runs a monster wave phase: begin, announce, wait for kills, standby.
/// </summary>
internal sealed class KanturuMonsterWaveRunner : IKanturuPhaseRunner
{
    private readonly Func<KanturuPhaseDefinition, CancellationToken, Task> _beginAsync;
    private readonly Func<KanturuPhaseDefinition, Task> _announceAsync;
    private readonly Func<KanturuPhaseDefinition, CancellationToken, Task<bool>> _waitAsync;
    private readonly Func<KanturuPhaseDefinition, CancellationToken, Task> _standbyAsync;

    /// <summary>
    /// Initializes a new instance of the <see cref="KanturuMonsterWaveRunner"/> class.
    /// </summary>
    /// <param name="beginAsync">Starts a phase.</param>
    /// <param name="announceAsync">Announces a phase.</param>
    /// <param name="waitAsync">Waits for a phase to end; reports whether it completed.</param>
    /// <param name="standbyAsync">Runs the standby time after a phase.</param>
    public KanturuMonsterWaveRunner(
        Func<KanturuPhaseDefinition, CancellationToken, Task> beginAsync,
        Func<KanturuPhaseDefinition, Task> announceAsync,
        Func<KanturuPhaseDefinition, CancellationToken, Task<bool>> waitAsync,
        Func<KanturuPhaseDefinition, CancellationToken, Task> standbyAsync)
    {
        this._beginAsync = beginAsync;
        this._announceAsync = announceAsync;
        this._waitAsync = waitAsync;
        this._standbyAsync = standbyAsync;
    }

    /// <inheritdoc />
    public KanturuPhaseKind Kind => KanturuPhaseKind.MonsterWave;

    /// <inheritdoc />
    public async Task<bool> RunAsync(KanturuPhaseDefinition phase, CancellationToken cancellationToken)
    {
        await this._beginAsync(phase, cancellationToken).ConfigureAwait(false);
        await this._announceAsync(phase).ConfigureAwait(false);
        if (!await this._waitAsync(phase, cancellationToken).ConfigureAwait(false))
        {
            return false;
        }

        await this._standbyAsync(phase, cancellationToken).ConfigureAwait(false);
        return true;
    }
}
