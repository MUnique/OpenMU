// <copyright file="ICrywolfEventViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Crywolf;

using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// The view plugin interface of the crywolf event.
/// </summary>
public interface ICrywolfEventViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the state of the event and the occupation state of the fortress.
    /// It must only be sent to players on the crywolf map, because the client loads the terrain of the occupation for its current map.
    /// </summary>
    /// <param name="occupation">The occupation state.</param>
    /// <param name="state">The state of the event.</param>
    ValueTask ShowStateAsync(CrywolfOccupationState occupation, CrywolfState state);

    /// <summary>
    /// Shows the shield of the statue and the states of the altars.
    /// </summary>
    /// <param name="statueShieldPercentage">The shield of the statue in percent.</param>
    /// <param name="altarStates">The states of the altars, like <see cref="CrywolfAltar.GetClientState"/>.</param>
    ValueTask ShowStatueAndAltarsAsync(int statueShieldPercentage, IReadOnlyList<byte> altarStates);

    /// <summary>
    /// Shows the result of the request to contract an altar.
    /// </summary>
    /// <param name="success">If set to <c>true</c>, the contract has been accepted.</param>
    /// <param name="altarIndex">The index of the altar (0 to 4).</param>
    /// <param name="altarState">The state of the altar, like <see cref="CrywolfAltar.GetClientState"/>.</param>
    ValueTask ShowContractResultAsync(bool success, int altarIndex, byte altarState);

    /// <summary>
    /// Shows the remaining time of the battle.
    /// </summary>
    /// <param name="remainingTime">The remaining time.</param>
    ValueTask ShowRemainingTimeAsync(TimeSpan remainingTime);

    /// <summary>
    /// Shows the health of Balgass and the number of the Dark Elves.
    /// </summary>
    /// <param name="balgassHealthPercentage">The health of Balgass in percent, or -1 if Balgass isn't alive.</param>
    /// <param name="darkElfCount">The number of alive Dark Elves.</param>
    ValueTask ShowBossMonsterInfoAsync(int balgassHealthPercentage, int darkElfCount);

    /// <summary>
    /// Shows the rank and the rewarded experience of the player.
    /// </summary>
    /// <param name="rank">The rank, from 0 (D) to 4 (S).</param>
    /// <param name="experience">The rewarded experience.</param>
    ValueTask ShowPersonalRankAsync(int rank, int experience);

    /// <summary>
    /// Shows the heroes with the highest scores.
    /// </summary>
    /// <param name="heroes">The heroes, ordered by their place.</param>
    ValueTask ShowHeroListAsync(IReadOnlyList<CrywolfHero> heroes);

    /// <summary>
    /// Shows the additional success rate of the chaos machine crafting.
    /// </summary>
    /// <param name="rate">The additional success rate in percent.</param>
    ValueTask ShowChaosRateBenefitAsync(byte rate);

    /// <summary>
    /// Shows the arrow of a ballista, which hits the target point.
    /// </summary>
    /// <param name="ballista">The ballista.</param>
    /// <param name="target">The target point.</param>
    ValueTask ShowBallistaAttackAsync(NonPlayerCharacter ballista, Point target);

    /// <summary>
    /// Shows or hides an effect of the event at an object, e.g. the state of an altar.
    /// </summary>
    /// <param name="npc">The object.</param>
    /// <param name="effect">The effect.</param>
    /// <param name="isActive">If set to <c>true</c>, the effect is shown; otherwise, it's hidden.</param>
    ValueTask ShowEffectAsync(NonPlayerCharacter npc, CrywolfEffect effect, bool isActive);
}
