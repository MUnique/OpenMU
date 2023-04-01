// <copyright file="PeriodicTaskState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// Describe the state of invasion.
/// </summary>
public enum PeriodicTaskState : byte
{
    /// <summary>
    /// Task not started.
    /// </summary>
    NotStarted,

    /// <summary>
    /// Task prepared.
    /// </summary>
    Prepared,

    /// <summary>
    /// Task started.
    /// </summary>
    Started,
}