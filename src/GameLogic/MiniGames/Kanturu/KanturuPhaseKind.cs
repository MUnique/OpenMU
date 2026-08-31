// <copyright file="KanturuPhaseKind.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

/// <summary>
/// The kind of a <see cref="KanturuPhaseDefinition"/>, which decides how the
/// <see cref="KanturuContext"/> executes it.
/// </summary>
public enum KanturuPhaseKind
{
    /// <summary>
    /// A monster wave which is finished when the configured number of monsters has been
    /// killed, or when the configured duration elapsed.
    /// </summary>
    MonsterWave,

    /// <summary>
    /// The transition from the Maya battlefield into the Nightmare zone: it plays the
    /// client cinematic and then moves all players to the configured entry point.
    /// </summary>
    Transition,

    /// <summary>
    /// The Nightmare boss fight, which additionally monitors the boss' health to teleport
    /// it between the configured positions.
    /// </summary>
    Nightmare,
}
