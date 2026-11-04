// <copyright file="WalkRequestValidationArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// The result of a walk request validation.
/// </summary>
public sealed class WalkRequestValidationArgs
{
    /// <summary>
    /// Gets or sets a value indicating whether the walk may be performed.
    /// </summary>
    public bool IsWalkAllowed { get; set; } = true;
}
