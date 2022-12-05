// <copyright file="InvasionEventState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

/// <summary>
/// Describe the state of invasion.
/// </summary>
public enum InvasionEventState : byte
{
    /// <summary>
    /// Not started.
    /// </summary>
    NotStarted,

    /// <summary>
    /// Event prepared.
    /// </summary>
    Prepared,

    /// <summary>
    /// Event started.
    /// </summary>
    Started,
}
