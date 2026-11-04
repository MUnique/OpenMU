// <copyright file="IWalkStepPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin point which is called when a player performed a walk step on the game map.
/// </summary>
[Guid("E89F3D6D-5C1D-4B3C-8E5B-2C1D9A7F4A61")]
[PlugInPoint("Walk step", "Plugins which are called when a player performed a walk step on the game map.")]
public interface IWalkStepPlugIn
{
    /// <summary>
    /// Is called after a player performed a walk step. It reports the step which was actually
    /// executed by the server, not a step which the client requested.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="step">The executed step.</param>
    /// <param name="stepDelay">
    /// The delay which was used for pacing the step. It already includes the distance of the step,
    /// so a diagonal step has a longer delay than a straight one.
    /// </param>
    void WalkStepPerformed(Player player, WalkingStep step, TimeSpan stepDelay);
}
