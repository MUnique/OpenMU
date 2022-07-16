// <copyright file="AttackAreaWhenPressedTrapIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC;

using MUnique.OpenMU.GameLogic.Views.World;
using Nito.AsyncEx;

/// <summary>
/// An AI which attacks all targets in an area, when it's triggered by one target.
/// </summary>
public class AttackAreaWhenPressedTrapIntelligence : TrapIntelligenceBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AttackAreaWhenPressedTrapIntelligence"/> class.
    /// </summary>
    /// <param name="map">The map.</param>
    public AttackAreaWhenPressedTrapIntelligence(GameMap map)
        : base(map)
    {
    }

    /// <inheritdoc />
    protected override async ValueTask TickAsync()
    {
        if (this.Trap.Observers.Count == 0)
        {
            return;
        }

        var isTrapActivated = this.PossibleTargets.Any(player => player.Position == this.Trap.Position);
        if (!isTrapActivated)
        {
            return;
        }

        var targetsInRange = this.PossibleTargets
            .Where(target => this.Trap.IsInRange(target.Position, this.Trap.Definition.AttackRange))
            .Where(target => !this.Map.Terrain.SafezoneMap[target.Position.X, target.Position.Y]);

        if (this.Trap.Definition.AttackSkill is { } attackSkill)
        {
            await this.Trap.ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(p => p.ShowSkillAnimationAsync(this.Trap, targetsInRange.FirstOrDefault(), attackSkill, true), true).ConfigureAwait(false);
        }

        await targetsInRange.Select(this.Trap.AttackAsync).WhenAll().ConfigureAwait(false);
    }
}