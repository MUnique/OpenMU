// <copyright file="CastleSiegeHuntZoneAccessType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Defines the access mode which the Land of Trials dialog shows to a player.
/// </summary>
public enum CastleSiegeHuntZoneAccessType : byte
{
    /// <summary>
    /// The dialog could not be opened.
    /// </summary>
    Failed = 0,

    /// <summary>
    /// The player enters as a paying guest.
    /// </summary>
    Guest = 1,

    /// <summary>
    /// The player belongs to the castle owner's guild or alliance.
    /// </summary>
    OwnerAllianceMember = 2,

    /// <summary>
    /// The player is the castle owner guild master.
    /// </summary>
    OwnerGuildMaster = 3,
}
