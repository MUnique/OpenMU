// <copyright file="RaklionEventDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Raklion;

/// <summary>
/// Describes the run of the raklion event.
/// </summary>
/// <remarks>
/// It's configured at the <see cref="RaklionPlugIn"/>, so it can be adapted in the admin panel.
/// The monsters of the event are defined by the monster spawns of the hatchery map with the
/// trigger <see cref="SpawnTrigger.OnceAtWaveStart"/> and the wave numbers of this definition.
/// </remarks>
public class RaklionEventDefinition
{
    /// <summary>
    /// Gets or sets the number of the map of raklion.
    /// </summary>
    public short RaklionMapNumber { get; set; } = 57;

    /// <summary>
    /// Gets or sets the number of the map of the hatchery, where Selupan appears.
    /// </summary>
    public short HatcheryMapNumber { get; set; } = 58;

    /// <summary>
    /// Gets or sets the wave number of the monster spawns of the spider eggs.
    /// </summary>
    public byte SpiderEggWaveNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the wave number of the monster spawn of Selupan.
    /// </summary>
    public byte SelupanWaveNumber { get; set; } = 2;

    /// <summary>
    /// Gets or sets the wave number of the monster spawns which are summoned by Selupan.
    /// </summary>
    public byte SummonWaveNumber { get; set; } = 3;

    /// <summary>
    /// Gets or sets the number of remaining spider eggs, at which the players are notified.
    /// </summary>
    public int SpiderEggNotifyCount { get; set; } = 5;

    /// <summary>
    /// Gets or sets the delay between the destruction of the last spider egg and the appearance of Selupan.
    /// </summary>
    public TimeSpan SelupanAppearanceDelay { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Gets or sets the time after the appearance of Selupan, until the hatchery gets closed.
    /// </summary>
    public TimeSpan HatcheryCloseDelay { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets the time after the end of the battle, until the hatchery gets opened again and the spider eggs appear.
    /// </summary>
    public TimeSpan HatcheryOpenDelay { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets the delay between the skills of Selupan.
    /// </summary>
    public TimeSpan SelupanSkillDelay { get; set; } = TimeSpan.FromMilliseconds(1500);

    /// <summary>
    /// Gets or sets the thresholds of the health of Selupan in percent, at which the next pattern starts.
    /// The first value is the threshold of the second pattern, and so on.
    /// </summary>
    public IList<int> PatternHealthThresholds { get; set; } = new List<int> { 80, 60, 50, 40, 20, 10 };

    /// <summary>
    /// Gets or sets the berserk levels of each pattern. The first value is the level of the first pattern, and so on.
    /// </summary>
    public IList<int> PatternBerserkLevels { get; set; } = new List<int> { 0, 1, 2, 2, 3, 4, 4 };

    /// <summary>
    /// Gets or sets the additional damage of Selupan per berserk level.
    /// </summary>
    public int BerserkDamagePerLevel { get; set; } = 300;

    /// <summary>
    /// Gets or sets the first pattern, in which Selupan freezes its target.
    /// </summary>
    public int FreezeStartPattern { get; set; } = 2;

    /// <summary>
    /// Gets or sets the first pattern, in which Selupan heals itself.
    /// </summary>
    public int HealStartPattern { get; set; } = 4;

    /// <summary>
    /// Gets or sets the first pattern, in which Selupan summons monsters.
    /// </summary>
    public int SummonStartPattern { get; set; } = 5;

    /// <summary>
    /// Gets or sets the first pattern, in which Selupan gets invincible for a short time.
    /// </summary>
    public int InvincibilityStartPattern { get; set; } = 7;

    /// <summary>
    /// Gets or sets the percentage of the current health which Selupan heals.
    /// </summary>
    public int HealPercentage { get; set; } = 5;

    /// <summary>
    /// Gets or sets the minimum distance of a teleport of Selupan.
    /// </summary>
    public int TeleportMinimumDistance { get; set; } = 3;

    /// <summary>
    /// Gets or sets the maximum distance of a teleport of Selupan.
    /// </summary>
    public int TeleportMaximumDistance { get; set; } = 6;

    /// <summary>
    /// Gets or sets the duration of the freeze of a player.
    /// </summary>
    public TimeSpan FreezeDuration { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Gets or sets the duration of the invincibility of Selupan.
    /// </summary>
    public TimeSpan InvincibilityDuration { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Gets or sets the radius around the target, in which the area skills of Selupan hit players.
    /// </summary>
    public int AreaSkillRadius { get; set; } = 6;

    /// <summary>
    /// Gets the pattern (1 to 7) of Selupan by its remaining health.
    /// </summary>
    /// <param name="healthPercentage">The remaining health in percent.</param>
    /// <returns>The pattern.</returns>
    public int GetPattern(double healthPercentage)
    {
        var pattern = 1;
        foreach (var threshold in this.PatternHealthThresholds)
        {
            if (healthPercentage > threshold)
            {
                break;
            }

            pattern++;
        }

        return pattern;
    }

    /// <summary>
    /// Gets the berserk level of the pattern.
    /// </summary>
    /// <param name="pattern">The pattern (1 to 7).</param>
    /// <returns>The berserk level.</returns>
    public int GetBerserkLevel(int pattern)
    {
        if (this.PatternBerserkLevels.Count == 0)
        {
            return 0;
        }

        return this.PatternBerserkLevels[Math.Clamp(pattern, 1, this.PatternBerserkLevels.Count) - 1];
    }

    /// <summary>
    /// Gets the skills which Selupan may use in the pattern, except the <see cref="SelupanSkill.Fall"/>.
    /// </summary>
    /// <param name="pattern">The pattern (1 to 7).</param>
    /// <returns>The skills.</returns>
    public IReadOnlyList<SelupanSkill> GetSkills(int pattern)
    {
        var skills = new List<SelupanSkill> { SelupanSkill.Poison, SelupanSkill.IceStorm, SelupanSkill.IceStrike, SelupanSkill.Teleport };
        if (pattern >= this.FreezeStartPattern)
        {
            skills.Add(SelupanSkill.Freeze);
        }

        if (pattern >= this.HealStartPattern)
        {
            skills.Add(SelupanSkill.Heal);
        }

        if (pattern >= this.SummonStartPattern)
        {
            skills.Add(SelupanSkill.Summon);
        }

        if (pattern >= this.InvincibilityStartPattern)
        {
            skills.Add(SelupanSkill.Invincibility);
        }

        return skills;
    }
}
