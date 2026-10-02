// <copyright file="ImperialGuardianMonsterIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.ImperialGuardian;

using System.Threading;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;

/// <summary>
/// The intelligence of the monsters of the imperial guardian event.
/// Additionally to the basic behavior, the monsters use their skills, and the bosses can get into rage.
/// </summary>
public sealed class ImperialGuardianMonsterIntelligence : BasicMonsterIntelligence
{
    private readonly ImperialGuardianEventDefinition _definition;
    private IList<ImperialGuardianMonsterSkill>? _skills;
    private int _isInRage;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImperialGuardianMonsterIntelligence"/> class.
    /// </summary>
    /// <param name="definition">The definition of the event.</param>
    public ImperialGuardianMonsterIntelligence(ImperialGuardianEventDefinition definition)
    {
        this._definition = definition;
    }

    /// <summary>
    /// Gets a value indicating whether the monster is in rage.
    /// </summary>
    public bool IsInRage => Volatile.Read(ref this._isInRage) != 0;

    /// <inheritdoc />
    /// <remarks>
    /// When the monster uses one of its skills, it doesn't attack normally in the same tick.
    /// </remarks>
    protected override async ValueTask<bool> CanAttackAsync()
    {
        var monster = this.Monster;
        await this.TryGetIntoRageAsync(monster).ConfigureAwait(false);

        this._skills ??= this._definition.MonsterSkills.Where(skill => skill.MonsterNumber == monster.Definition.Number).ToList();
        if (this._skills.Count == 0
            || this.CurrentTarget is not { } target
            || !target.IsActive()
            || !target.IsInRange(monster.Position, monster.Definition.AttackRange)
            || Rand.NextInt(0, 100) >= this._definition.SkillChance)
        {
            return true;
        }

        var skill = this._skills[Rand.NextInt(0, this._skills.Count)];
        await this.UseSkillAsync(monster, target, skill).ConfigureAwait(false);
        return false;
    }

    private static ValueTask ShowSkillAsync(Monster monster, IAttackable target, short skillNumber)
    {
        return monster.ForEachWorldObserverAsync<IImperialGuardianViewPlugIn>(p => p.ShowMonsterSkillAsync(monster, target, skillNumber), true);
    }

    private async ValueTask TryGetIntoRageAsync(Monster monster)
    {
        if (this._definition.RageHealthPercentage <= 0
            || this.IsInRage
            || !this._definition.RageMonsterNumbers.Contains(monster.Definition.Number))
        {
            return;
        }

        var maximumHealth = monster.Attributes[Stats.MaximumHealth];
        if (maximumHealth <= 0 || monster.Health * 100.0 / maximumHealth > this._definition.RageHealthPercentage
            || Interlocked.Exchange(ref this._isInRage, 1) != 0)
        {
            return;
        }

        foreach (var attribute in new[] { Stats.MinimumPhysBaseDmg, Stats.MaximumPhysBaseDmg, Stats.DefenseBase })
        {
            monster.Attributes.AddElement(new SimpleElement(this._definition.RageBonus, AggregateType.AddRaw), attribute);
        }

        // The client shows the rage as the animation of a skill, which targets the monster itself.
        await ShowSkillAsync(monster, monster, this._definition.RageSkillNumber).ConfigureAwait(false);
    }

    private async ValueTask UseSkillAsync(Monster monster, IAttackable target, ImperialGuardianMonsterSkill skill)
    {
        await ShowSkillAsync(monster, target, skill.SkillNumber).ConfigureAwait(false);

        IEnumerable<IAttackable> targets = [target];
        if (skill.Radius > 0)
        {
            var center = skill.IsAroundMonster ? monster.Position : target.Position;
            targets = monster.CurrentMap.GetAttackablesInRange(center, skill.Radius)
                .OfType<Player>()
                .Where(player => player.IsActive() && !player.IsAtSafezone())
                .ToList();
        }

        foreach (var hitTarget in targets)
        {
            await hitTarget.AttackByAsync(monster, null, false).ConfigureAwait(false);
            if (skill.StunChance > 0
                && hitTarget is Player { IsAlive: true } player
                && Rand.NextInt(0, 100) < skill.StunChance)
            {
                await player.ApplyStunEffectAsync(skill.StunDuration).ConfigureAwait(false);
            }
        }
    }
}
