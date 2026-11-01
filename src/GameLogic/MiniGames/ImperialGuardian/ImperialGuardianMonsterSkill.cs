// <copyright file="ImperialGuardianMonsterSkill.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.ImperialGuardian;

/// <summary>
/// A skill of a monster of the imperial guardian event.
/// </summary>
public class ImperialGuardianMonsterSkill
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
    /// Gets or sets the radius, in which the skill hits the players. When it's 0, it only hits the target.
    /// </summary>
    public int Radius { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the area of the skill is around the monster.
    /// Otherwise, it's around the target.
    /// </summary>
    public bool IsAroundMonster { get; set; }

    /// <summary>
    /// Gets or sets the chance in percent to stun the hit players.
    /// </summary>
    public int StunChance { get; set; }

    /// <summary>
    /// Gets or sets the duration of the stun.
    /// </summary>
    public TimeSpan StunDuration { get; set; }

    /// <summary>
    /// Creates a skill, which only hits the target.
    /// </summary>
    /// <param name="monsterNumber">The number of the monster.</param>
    /// <param name="skillNumber">The number of the skill.</param>
    /// <param name="stunChance">The chance in percent to stun the target.</param>
    /// <param name="stunSeconds">The duration of the stun in seconds.</param>
    /// <returns>The skill.</returns>
    public static ImperialGuardianMonsterSkill Single(short monsterNumber, short skillNumber, int stunChance = 0, int stunSeconds = 0)
    {
        return new() { MonsterNumber = monsterNumber, SkillNumber = skillNumber, StunChance = stunChance, StunDuration = TimeSpan.FromSeconds(stunSeconds) };
    }

    /// <summary>
    /// Creates a skill, which hits the players in an area.
    /// </summary>
    /// <param name="monsterNumber">The number of the monster.</param>
    /// <param name="skillNumber">The number of the skill.</param>
    /// <param name="radius">The radius of the area.</param>
    /// <param name="isAroundMonster">If set to <c>true</c>, the area is around the monster; otherwise, around the target.</param>
    /// <param name="stunChance">The chance in percent to stun the hit players.</param>
    /// <param name="stunSeconds">The duration of the stun in seconds.</param>
    /// <returns>The skill.</returns>
    public static ImperialGuardianMonsterSkill Area(short monsterNumber, short skillNumber, int radius, bool isAroundMonster, int stunChance = 0, int stunSeconds = 0)
    {
        return new()
        {
            MonsterNumber = monsterNumber,
            SkillNumber = skillNumber,
            Radius = radius,
            IsAroundMonster = isAroundMonster,
            StunChance = stunChance,
            StunDuration = TimeSpan.FromSeconds(stunSeconds),
        };
    }
}
