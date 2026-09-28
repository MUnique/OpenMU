// <copyright file="CrywolfContractChange.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Crywolf;

/// <summary>
/// The change of the contract of an altar of the crywolf event.
/// </summary>
public enum CrywolfContractChange
{
    /// <summary>
    /// The contract didn't change.
    /// </summary>
    None,

    /// <summary>
    /// The contract got valid.
    /// </summary>
    Validated,

    /// <summary>
    /// The contract has been cancelled.
    /// </summary>
    Cancelled,
}
