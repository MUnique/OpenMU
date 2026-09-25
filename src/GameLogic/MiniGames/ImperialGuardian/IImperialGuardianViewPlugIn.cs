// <copyright file="IImperialGuardianViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.ImperialGuardian;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// View plugin interface for the imperial guardian event.
/// </summary>
public interface IImperialGuardianViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the result of an enter request, or the zone which the player entered.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="day">The day of the week (1 = monday, ..., 7 = sunday).</param>
    /// <param name="zone">The zone, starting at 0.</param>
    /// <param name="weather">The weather of the map.</param>
    /// <param name="remainingTime">The remaining time.</param>
    ValueTask ShowEnterResultAsync(ImperialGuardianEnterResult result, byte day, int zone, ImperialGuardianWeather weather, TimeSpan remainingTime);

    /// <summary>
    /// Shows the timer of the current zone.
    /// </summary>
    /// <param name="type">The type of the timer.</param>
    /// <param name="remainingTime">The remaining time.</param>
    /// <param name="monsterCount">The number of remaining monsters.</param>
    ValueTask ShowTimerAsync(ImperialGuardianTimerType type, TimeSpan remainingTime, int monsterCount);

    /// <summary>
    /// Shows the result of a zone or of the event.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="experience">The rewarded experience.</param>
    ValueTask ShowResultAsync(ImperialGuardianResult result, int experience);
}
