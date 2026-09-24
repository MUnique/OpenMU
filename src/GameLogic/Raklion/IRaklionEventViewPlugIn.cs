// <copyright file="IRaklionEventViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Raklion;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// View plugin interface for the raklion event.
/// </summary>
/// <remarks>
/// The client shows the portal to the hatchery, the effects and the music depending on the state.
/// It doesn't show a HUD or the result of the battle.
/// </remarks>
public interface IRaklionEventViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the state of the event, e.g. as response to a request.
    /// </summary>
    /// <param name="state">The state.</param>
    /// <param name="selupanState">The state of Selupan.</param>
    /// <param name="canEnter">If set to <c>true</c>, the hatchery can be entered.</param>
    /// <param name="remainingTime">The remaining time of the current state.</param>
    ValueTask ShowStateInfoAsync(RaklionState state, SelupanState selupanState, bool canEnter, TimeSpan remainingTime);

    /// <summary>
    /// Shows the current state of the event to a player who entered one of the raklion maps.
    /// </summary>
    /// <param name="state">The state.</param>
    ValueTask ShowCurrentStateAsync(RaklionState state);

    /// <summary>
    /// Shows a changed state of the event.
    /// </summary>
    /// <param name="state">The state.</param>
    ValueTask ShowStateChangeAsync(RaklionState state);

    /// <summary>
    /// Shows a changed state of Selupan.
    /// </summary>
    /// <param name="selupanState">The state of Selupan.</param>
    ValueTask ShowSelupanStateAsync(SelupanState selupanState);

    /// <summary>
    /// Shows the result of the battle against Selupan.
    /// </summary>
    /// <param name="success">If set to <c>true</c>, Selupan was killed.</param>
    ValueTask ShowBattleResultAsync(bool success);

    /// <summary>
    /// Shows the animation of a skill of Selupan.
    /// </summary>
    /// <param name="selupan">Selupan.</param>
    /// <param name="target">The target of the skill.</param>
    /// <param name="skill">The skill.</param>
    ValueTask ShowSelupanSkillAsync(IAttacker selupan, IAttackable? target, SelupanSkill skill);
}
