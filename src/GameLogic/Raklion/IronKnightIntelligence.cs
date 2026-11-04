// <copyright file="IronKnightIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Raklion;

using MUnique.OpenMU.GameLogic.NPC;

/// <summary>
/// The intelligence of the iron knight of raklion.
/// Additionally to the basic behavior, it attacks its target with its stab by the
/// chance of <see cref="RaklionEventDefinition.IronKnightStabChance"/>.
/// </summary>
/// <remarks>
/// The client shows the stab of an iron knight only, when it's sent as a monster skill.
/// The stab hits with the same damage as a normal attack.
/// </remarks>
public sealed class IronKnightIntelligence : BasicMonsterIntelligence
{
    private static readonly RaklionEventDefinition DefaultDefinition = new();

    /// <summary>
    /// Attacks the target with the stab, by the chance of <see cref="RaklionEventDefinition.IronKnightStabChance"/>.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c>, if the iron knight attacked the target with its stab.</returns>
    internal async ValueTask<bool> TryStabAsync(IAttackable? target)
    {
        var monster = this.Monster;
        if (target is not Player player
            || !player.IsActive()
            || !player.IsInRange(monster.Position, monster.Definition.AttackRange)
            || monster.IsAtSafezone()
            || !monster.HasLineOfSightTo(player)
            || Rand.NextInt(0, 100) >= GetDefinition(player.GameContext).IronKnightStabChance)
        {
            return false;
        }

        await monster.ForEachWorldObserverAsync<IRaklionEventViewPlugIn>(p => p.ShowIronKnightStabAsync(monster, player), true).ConfigureAwait(false);
        await player.AttackByAsync(monster, null, false).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    /// <remarks>
    /// When the monster uses its stab, it doesn't attack normally in the same tick.
    /// </remarks>
    protected override async ValueTask<bool> CanAttackAsync()
    {
        return !await this.TryStabAsync(this.CurrentTarget).ConfigureAwait(false);
    }

    private static RaklionEventDefinition GetDefinition(IGameContext gameContext)
    {
        return gameContext.FeaturePlugIns.GetPlugIn<RaklionPlugIn>()?.Configuration ?? DefaultDefinition;
    }
}
