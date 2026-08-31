// <copyright file="ICastleSiegeMachineUseResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// A view which shows a Castle Siege warfare-machine shot.
/// </summary>
public interface ICastleSiegeMachineUseResultPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the fired machine and its selected impact point.
    /// </summary>
    /// <param name="machineId">The machine object identifier.</param>
    /// <param name="machineType">The machine type.</param>
    /// <param name="target">The impact point.</param>
    ValueTask ShowMachineUseResultAsync(ushort machineId, CastleSiegeMachineType machineType, Point target);
}
