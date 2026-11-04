// <copyright file="IKanturuPhaseRunner.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

using System.Threading;

/// <summary>
/// Runs one phase of the Kanturu event. Each phase kind has its own implementation,
/// which the game loop selects through this interface.
/// </summary>
internal interface IKanturuPhaseRunner
{
    /// <summary>
    /// Gets the kind of phase this runner executes.
    /// </summary>
    KanturuPhaseKind Kind { get; }

    /// <summary>
    /// Runs the phase.
    /// </summary>
    /// <param name="phase">The phase to run.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c> when the phase completed; <c>false</c> when its time limit expired.</returns>
    Task<bool> RunAsync(KanturuPhaseDefinition phase, CancellationToken cancellationToken);
}
