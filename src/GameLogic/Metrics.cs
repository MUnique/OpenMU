// <copyright file="Metrics.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Provides information about the available metrics of this project.
/// </summary>
public static class Metrics
{
    /// <summary>
    /// Gets all available meters of this project.
    /// </summary>
    public static IEnumerable<string> Meters
    {
        get
        {
            yield return GameContext.MeterName;
            yield return BaseTradeAction.MeterName;
            yield return PathFinder.MeterName;
        }
    }
}