// <copyright file="ICastleSiegeMachineRegionNotifyPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// A view which shows the impact region of a Castle Siege warfare machine.
/// </summary>
public interface ICastleSiegeMachineRegionNotifyPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the machine impact region.
    /// </summary>
    /// <param name="machineType">The machine type.</param>
    /// <param name="target">The impact point.</param>
    ValueTask ShowMachineRegionAsync(CastleSiegeMachineType machineType, Point target);
}
