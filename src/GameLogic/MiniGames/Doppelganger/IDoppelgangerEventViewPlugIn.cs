// <copyright file="IDoppelgangerEventViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// View plugin interface for the doppelganger event.
/// </summary>
/// <remarks>
/// Positions are indexes on the path of the monsters, between 0 (start)
/// and <see cref="MaximumPathPosition"/> (magic circle).
/// </remarks>
public interface IDoppelgangerEventViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Gets the maximum position index on the path, which is the position of the magic circle.
    /// </summary>
    static int MaximumPathPosition => 22;

    /// <summary>
    /// Shows the state of the event.
    /// When the event starts (<see cref="DoppelgangerState.Playing"/>), the client
    /// shows the event frame and a message box with the failure conditions.
    /// </summary>
    /// <param name="state">The state.</param>
    ValueTask ShowStateAsync(DoppelgangerState state);

    /// <summary>
    /// Shows the position of the most advanced monster on the path.
    /// </summary>
    /// <param name="position">The position index.</param>
    ValueTask ShowMonsterPositionAsync(int position);

    /// <summary>
    /// Shows the ice walker on the path.
    /// </summary>
    /// <param name="position">The position index.</param>
    ValueTask ShowIceWalkerAsync(int position);

    /// <summary>
    /// Hides the ice walker, because it was killed or disappeared.
    /// </summary>
    ValueTask HideIceWalkerAsync();

    /// <summary>
    /// Shows the countdown of the last 30 seconds in which the ice walkers have to be killed.
    /// </summary>
    ValueTask ShowIceWalkerCountdownAsync();

    /// <summary>
    /// Shows the remaining time and the positions of the players on the path.
    /// The client doesn't count down the time by itself, so this should be sent periodically.
    /// Players which are not included are no longer shown.
    /// </summary>
    /// <param name="remainingTime">The remaining time.</param>
    /// <param name="playerPositions">The players with their position index.</param>
    ValueTask ShowPlayInfoAsync(TimeSpan remainingTime, IReadOnlyCollection<(Player Player, int Position)> playerPositions);

    /// <summary>
    /// Shows the number of monsters which reached the magic circle.
    /// </summary>
    /// <param name="goalCount">The number of monsters which reached the magic circle.</param>
    /// <param name="maximumGoalCount">The number of monsters which may reach the magic circle until the event fails.</param>
    ValueTask ShowMonsterGoalAsync(int goalCount, int maximumGoalCount);

    /// <summary>
    /// Shows the result of the event.
    /// </summary>
    /// <param name="result">The result.</param>
    ValueTask ShowResultAsync(DoppelgangerResult result);
}
