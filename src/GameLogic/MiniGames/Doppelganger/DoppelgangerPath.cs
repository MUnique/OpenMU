// <copyright file="DoppelgangerPath.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;

/// <summary>
/// The path of the monsters towards the magic circle on one doppelganger event map.
/// </summary>
public class DoppelgangerPath
{
    /// <summary>
    /// Gets or sets the number of the map.
    /// </summary>
    public short MapNumber { get; set; }

    /// <summary>
    /// Gets or sets the areas of the path, from the start of the monsters (index 0)
    /// to the magic circle (last index).
    /// </summary>
    public IList<DoppelgangerPathArea> Areas { get; set; } = new List<DoppelgangerPathArea>();
}
