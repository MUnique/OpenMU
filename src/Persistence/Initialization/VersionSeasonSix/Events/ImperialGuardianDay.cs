// <copyright file="ImperialGuardianDay.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;

/// <summary>
/// The day of the week of the imperial guardian event, as it's used by the event and the game client.
/// </summary>
internal enum ImperialGuardianDay : byte
{
    /// <summary>
    /// Monday.
    /// </summary>
    Monday = 1,

    /// <summary>
    /// Tuesday.
    /// </summary>
    Tuesday = 2,

    /// <summary>
    /// Wednesday.
    /// </summary>
    Wednesday = 3,

    /// <summary>
    /// Thursday.
    /// </summary>
    Thursday = 4,

    /// <summary>
    /// Friday.
    /// </summary>
    Friday = 5,

    /// <summary>
    /// Saturday.
    /// </summary>
    Saturday = 6,

    /// <summary>
    /// Sunday.
    /// </summary>
    Sunday = 7,
}
