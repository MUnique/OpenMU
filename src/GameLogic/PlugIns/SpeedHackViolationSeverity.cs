// <copyright file="SpeedHackViolationSeverity.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Describes how a speed hack check verdict should be acted upon.
/// </summary>
public enum SpeedHackViolationSeverity
{
    /// <summary>
    /// No violation. The action may be performed.
    /// </summary>
    None,

    /// <summary>
    /// A soft violation, recorded through the normal (debounced) policy.
    /// Walk consumers only record it; attack consumers only drop the attack.
    /// </summary>
    Soft,

    /// <summary>
    /// A hard violation from sustained cheating. When assigned to a check event,
    /// consumers suppress the checked action. The violation is recorded through
    /// the full policy without debounce leniency.
    /// </summary>
    Reject,
}
