// <copyright file="GoblinPointsForPlayTimeConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CashShop;

/// <summary>
/// The configuration of the <see cref="GoblinPointsForPlayTimePlugIn"/>.
/// </summary>
public class GoblinPointsForPlayTimeConfiguration
{
    /// <summary>
    /// Gets or sets the play time after which a player gets the <see cref="Points"/>.
    /// </summary>
    public TimeSpan Interval { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// Gets or sets the number of Goblin Points which a player gets per <see cref="Interval"/>.
    /// </summary>
    public int Points { get; set; } = 10;

    /// <summary>
    /// Gets or sets the minimum level of the character.
    /// </summary>
    public int MinimumLevel { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether characters which level offline get the points, too.
    /// </summary>
    public bool IncludeOfflineLeveling { get; set; }
}
