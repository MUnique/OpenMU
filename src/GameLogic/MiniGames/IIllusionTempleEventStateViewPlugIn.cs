// <copyright file="IIllusionTempleEventStateViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// The state of an illusion temple event, as far as the game client is concerned.
/// </summary>
public enum IllusionTempleEventStatus
{
    /// <summary>
    /// The player entered the event and waits for it to start. In contrast to the other states, this
    /// one is only told to the entering player, not to all participants.
    /// </summary>
    WaitingRoom = 0,

    /// <summary>
    /// The preparation started: the players have been moved into the arena and assigned to their teams.
    /// The client opens the event interface with the score board, the timer and the mini map.
    /// </summary>
    Preparation = 1,

    /// <summary>
    /// The battle started: the statues are up and the barriers of the arena are removed, so that the
    /// players can reach the cursed statue. The barrier areas are hardcoded at client side, so this is
    /// the only way for the server to open them.
    /// </summary>
    BattleStarted = 2,

    /// <summary>
    /// The battle ended - the client closes the event interface.
    /// </summary>
    Ended = 3,
}

/// <summary>
/// Interface of a view whose implementation informs about the state of an illusion temple event.
/// </summary>
public interface IIllusionTempleEventStateViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Changes the state of the illusion temple event at the client.
    /// </summary>
    /// <param name="templeNumber">The number of the temple, from 1 to 6.</param>
    /// <param name="state">The new state of the event.</param>
    ValueTask ChangeEventStateAsync(byte templeNumber, IllusionTempleEventStatus state);
}
