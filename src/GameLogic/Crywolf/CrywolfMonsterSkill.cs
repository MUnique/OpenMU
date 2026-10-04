// <copyright file="CrywolfMonsterSkill.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Crywolf;

/// <summary>
/// A skill of a monster of the army of Balgass, like the monster skill units of the original game.
/// </summary>
/// <remarks>
/// Each effect is rolled separately for each hit player.
/// </remarks>
public class CrywolfMonsterSkill
{
    /// <summary>
    /// Gets or sets the number of the monster.
    /// </summary>
    public short MonsterNumber { get; set; }

    /// <summary>
    /// Gets or sets the number of the skill, which the client uses to show its animation.
    /// </summary>
    public short SkillNumber { get; set; }

    /// <summary>
    /// Gets or sets the radius around the monster, in which the skill hits the players. When it's 0, it only hits the target.
    /// </summary>
    public int Radius { get; set; }

    /// <summary>
    /// Gets or sets the chance in percent to push the hit players away from the monster.
    /// </summary>
    public int PushChance { get; set; }

    /// <summary>
    /// Gets or sets the distance, by which the hit players are pushed away.
    /// </summary>
    public int PushDistance { get; set; } = 3;

    /// <summary>
    /// Gets or sets the chance in percent to stun the hit players.
    /// </summary>
    public int StunChance { get; set; }

    /// <summary>
    /// Gets or sets the duration of the stun.
    /// </summary>
    public TimeSpan StunDuration { get; set; } = TimeSpan.FromSeconds(3);

    /// <summary>
    /// Gets or sets the number of the magic effect, which is removed from the hit players, e.g. the greater defense of the elves.
    /// </summary>
    public short RemovedMagicEffectNumber { get; set; }

    /// <summary>
    /// Gets or sets the chance in percent to remove the <see cref="RemovedMagicEffectNumber"/>.
    /// </summary>
    public int RemoveEffectChance { get; set; }

    /// <summary>
    /// Gets or sets the chance in percent to decrease the mana of the hit players by <see cref="DecreasePercentage"/>.
    /// </summary>
    public int ManaDecreaseChance { get; set; }

    /// <summary>
    /// Gets or sets the chance in percent to decrease the ability (AG) of the hit players by <see cref="DecreasePercentage"/>.
    /// </summary>
    public int AbilityDecreaseChance { get; set; }

    /// <summary>
    /// Gets or sets the percentage, by which the mana or ability is decreased.
    /// </summary>
    public int DecreasePercentage { get; set; } = 50;
}
