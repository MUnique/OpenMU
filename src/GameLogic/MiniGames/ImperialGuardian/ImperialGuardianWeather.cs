// <copyright file="ImperialGuardianWeather.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.ImperialGuardian;

/// <summary>
/// The weather of the map of the imperial guardian event, which is chosen randomly for each game.
/// </summary>
public enum ImperialGuardianWeather
{
    /// <summary>
    /// The sun shines.
    /// </summary>
    Sun,

    /// <summary>
    /// It rains.
    /// </summary>
    Rain,

    /// <summary>
    /// There is fog.
    /// </summary>
    Fog,

    /// <summary>
    /// There is a storm.
    /// </summary>
    Storm,
}
