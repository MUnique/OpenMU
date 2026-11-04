// <copyright file="IBloodCastleStateViewPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// The status of a blood castle event.
/// </summary>
public enum BloodCastleStatus
{
    /// <summary>
    /// The event has just started and is running.
    /// </summary>
    Started,

    /// <summary>
    /// The event is running, but the gate is not destroyed.
    /// </summary>
    GateNotDestroyed,

    /// <summary>
    /// The event is running and the gate is destroyed.
    /// </summary>
    GateDestroyed,

    /// <summary>
    /// The event has ended.
    /// </summary>
    Ended,
}

/// <summary>
/// Interface of a view whose implementation informs about the status of a blood castle event.
/// </summary>
public interface IBloodCastleStateViewPlugin : IViewPlugIn
{
    /// <summary>
    /// Update the state of the blood castle event.
    /// </summary>
    /// <param name="status">The status of the blood castle event.</param>
    /// <param name="remainingTime">The remaining time of the blood castle event.</param>
    /// <param name="maxMonster">Maximum number of monsters to kill.</param>
    /// <param name="curMonster">Current number of monsters killed.</param>
    /// <param name="questItemOwner">The player which picked up the quest item.</param>
    /// <param name="questItem">The quest item which was dropped by the statue.</param>
    ValueTask UpdateStateAsync(BloodCastleStatus status, TimeSpan remainingTime, int maxMonster, int curMonster, IIdentifiable? questItemOwner, Item? questItem);
}