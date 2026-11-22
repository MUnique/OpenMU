// <copyright file="LordsMixLimitPeriod.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The period of the limit of the Lord's Mix, see <see cref="CastleSiegeConfiguration.LordsMixLimit"/>.
/// </summary>
public enum LordsMixLimitPeriod
{
    /// <summary>
    /// The limit is per day; the count is reset at midnight of the server's local time.
    /// </summary>
    Day = 0,

    /// <summary>
    /// The limit is per siege cycle; the count is reset when a siege ends.
    /// </summary>
    SiegeCycle = 1,
}
