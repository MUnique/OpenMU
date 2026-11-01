// <copyright file="KanturuWaveGroup.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

/// <summary>
/// One wave of the Kanturu event. All phases of a wave share a single countdown
/// (see <see cref="KanturuPhaseDefinition.TimeLimitGroup"/>): the monsters and the
/// boss of the wave must be killed before it expires.
/// </summary>
public enum KanturuWaveGroup
{
    /// <summary>
    /// The first wave: monsters, then Maya's left hand.
    /// </summary>
    MayaLeftHand,

    /// <summary>
    /// The second wave: monsters, then Maya's right hand.
    /// </summary>
    MayaRightHand,

    /// <summary>
    /// The third wave: monsters, then both hands of Maya.
    /// </summary>
    MayaBothHands,

    /// <summary>
    /// The fourth wave: the guardians, then Nightmare.
    /// </summary>
    Nightmare,
}
