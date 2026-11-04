// <copyright file="IWalkRequestValidationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin point which decides whether a walk request of a player may be performed.
/// </summary>
[Guid("90D91C0D-5E7D-4908-A9C6-D18A4C4EEA04")]
[PlugInPoint("Walk request validation", "Validates whether a walk request of a player may be performed.")]
public interface IWalkRequestValidationPlugIn
{
    /// <summary>
    /// Validates the walk request. Plugins can reject it by setting
    /// <see cref="WalkRequestValidationArgs.IsWalkAllowed"/> to <c>false</c>.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="steps">The steps requested by the player.</param>
    /// <param name="eventArgs">The validation result.</param>
    ValueTask ValidateWalkRequestAsync(Player player, Memory<WalkingStep> steps, WalkRequestValidationArgs eventArgs);
}
