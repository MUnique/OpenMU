// <copyright file="IPeriodicTaskPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// An interface for a task which is executed periodically by the game context every second.
/// The task implementation itself can then decide if it wants to execute itself.
/// </summary>
[Guid("A890CEB1-6EA7-49A1-8099-6FF52D33DC64")]
[PlugInPoint("Periodic Tasks", "Is called in an periodic interval.")]
public interface IPeriodicTaskPlugIn
{
    /// <summary>
    /// Executes the task.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    ValueTask ExecuteTaskAsync(GameContext gameContext);

    /// <summary>
    /// Forces to start the task on the next start check.
    /// </summary>
    void ForceStart();
}