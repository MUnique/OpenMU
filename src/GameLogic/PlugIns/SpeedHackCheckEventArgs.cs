// <copyright file="SpeedHackCheckEventArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Event arguments for speed hack cheat checks.
/// </summary>
public class SpeedHackCheckEventArgs
{
    /// <summary>
    /// Gets or sets how a detected violation should be acted upon.
    /// <see cref="SpeedHackViolationSeverity.None"/> means no violation was detected.
    /// </summary>
    public SpeedHackViolationSeverity Severity { get; set; } = SpeedHackViolationSeverity.None;

    /// <summary>
    /// Gets a value indicating whether any violation was detected, regardless of its severity.
    /// </summary>
    /// <remarks>
    /// A convenience reader over <see cref="Severity"/> for callers which cannot act on
    /// specific severities. It holds no state of its own, so it cannot diverge from it.
    /// </remarks>
    public bool IsCheatDetected => this.Severity is not SpeedHackViolationSeverity.None;
}
