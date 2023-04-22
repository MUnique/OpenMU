// <copyright file="ChaosCastleStatus.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

/// <summary>
/// The status of a blood castle event.
/// </summary>
public enum ChaosCastleStatus
{
    /// <summary>
    /// The event has just started and is running.
    /// </summary>
    Started,

    /// <summary>
    /// The event is running, no terrain shrinking applied yet.
    /// </summary>
    Running,

    /// <summary>
    /// The event is running, first terrain shrinking applied.
    /// </summary>
    RunningShrinkingStageOne,

    /// <summary>
    /// The event is running, second terrain shrinking applied.
    /// </summary>
    RunningShrinkingStageTwo,

    /// <summary>
    /// The event is running, third (final) terrain shrinking applied.
    /// </summary>
    RunningShrinkingStageThree,

    /// <summary>
    /// The event has ended.
    /// </summary>
    Ended,
}