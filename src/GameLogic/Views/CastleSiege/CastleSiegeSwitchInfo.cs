// <copyright file="CastleSiegeSwitchInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Describes the current occupant of a Castle Siege Crown switch.
/// </summary>
/// <param name="ObjectId">The switch object's network identifier.</param>
/// <param name="IsOccupied">Whether a player currently occupies the switch.</param>
/// <param name="JoinSide">The occupant's Castle Siege side.</param>
/// <param name="GuildName">The occupant's guild name.</param>
/// <param name="CharacterName">The occupant's character name.</param>
public sealed record CastleSiegeSwitchInfo(
    ushort ObjectId,
    bool IsOccupied,
    CastleSiegeJoinSide JoinSide,
    string GuildName,
    string CharacterName);
