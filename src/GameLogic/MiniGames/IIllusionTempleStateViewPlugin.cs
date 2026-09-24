// <copyright file="IIllusionTempleStateViewPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// Interface of a view whose implementation informs about the state of a running illusion temple event.
/// </summary>
/// <remarks>
/// In contrast to the other mini games, this update is not the same for all participants: it tells the
/// receiver which team he belongs to, and where his own team mates currently are - so it has to be
/// built per player.
/// </remarks>
public interface IIllusionTempleStateViewPlugin : IViewPlugIn
{
    /// <summary>
    /// Updates the state of the illusion temple event.
    /// </summary>
    /// <param name="remainingTime">The remaining time of the event.</param>
    /// <param name="alliedForcesPoints">The points which the allied forces scored so far.</param>
    /// <param name="illusionForcesPoints">The points which the illusion forces scored so far.</param>
    /// <param name="ownTeam">The team of the player who receives the update.</param>
    /// <param name="teamMembers">The team mates of the receiving player, with their current position.</param>
    /// <param name="relicCarrier">
    /// The player who currently carries the holy relic, with his current position - or <c>null</c> if
    /// nobody currently carries it.
    /// </param>
    ValueTask UpdateStateAsync(
        TimeSpan remainingTime,
        byte alliedForcesPoints,
        byte illusionForcesPoints,
        IllusionTempleTeam ownTeam,
        IReadOnlyCollection<(ushort PlayerId, byte MapNumber, byte PositionX, byte PositionY)> teamMembers,
        (ushort PlayerId, byte PositionX, byte PositionY)? relicCarrier);
}
