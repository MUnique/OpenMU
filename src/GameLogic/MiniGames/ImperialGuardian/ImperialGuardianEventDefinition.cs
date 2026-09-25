// <copyright file="ImperialGuardianEventDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.ImperialGuardian;

/// <summary>
/// Describes the run of the imperial guardian event.
/// </summary>
/// <remarks>
/// It's configured at the <see cref="ImperialGuardianFeaturePlugIn"/>, so it can be adapted in the admin panel.
/// The monsters of a zone are defined by the monster spawns of the event map with the trigger
/// <see cref="SpawnTrigger.OnceAtWaveStart"/>. Their wave number is the day of the week
/// (1 = monday, ..., 7 = sunday) multiplied by 10, plus the zone (starting at 0).
/// </remarks>
public class ImperialGuardianEventDefinition
{
    /// <summary>
    /// The day of the week of sunday, on which the event takes place on its own map.
    /// </summary>
    public const byte Sunday = 7;

    /// <summary>
    /// Gets or sets the time between the start of a zone and the appearance of its monsters,
    /// in which the players can gather. The first zone uses the enter duration of the mini game instead.
    /// </summary>
    public TimeSpan StandbyDuration { get; set; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Gets or sets the time in which the monsters of a zone have to be killed.
    /// </summary>
    public TimeSpan ZoneDuration { get; set; } = TimeSpan.FromMinutes(10);

    /// <summary>
    /// Gets or sets a value indicating whether the next zone starts as soon as the monsters of a zone are killed.
    /// Otherwise, it starts when the time of the zone is over, like in the original game.
    /// </summary>
    public bool StartNextZoneWhenCleared { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the event can only be entered as member of a party.
    /// </summary>
    public bool IsPartyRequired { get; set; }

    /// <summary>
    /// Gets or sets a fixed day of the week (1 = monday, ..., 7 = sunday) for the event, e.g. for testing.
    /// When it's 0, the current day of the week of the server is used.
    /// </summary>
    public byte FixedDay { get; set; }

    /// <summary>
    /// Gets or sets the numbers of the gates, which block the way until they're destroyed.
    /// </summary>
    public IList<short> BlockingGateNumbers { get; set; } = new List<short> { 524, 525, 527, 528 };

    /// <summary>
    /// Gets or sets the numbers of the gates and statues, which can be attacked as soon as the monsters of a zone appear.
    /// </summary>
    public IList<short> EarlyAttackableGateNumbers { get; set; } = new List<short> { 525, 528 };

    /// <summary>
    /// Gets or sets the numbers of the gates and statues, which can be attacked when all monsters of a zone are killed.
    /// </summary>
    public IList<short> LateAttackableGateNumbers { get; set; } = new List<short> { 524, 526, 527 };

    /// <summary>
    /// Gets or sets the numbers of the traps.
    /// </summary>
    public IList<short> TrapNumbers { get; set; } = new List<short> { 523 };

    /// <summary>
    /// Gets or sets the secromicon fragments, which are dropped by the bosses from monday to saturday.
    /// </summary>
    public IList<ImperialGuardianFragmentDrop> FragmentDrops { get; set; } = new List<ImperialGuardianFragmentDrop>
    {
        new() { MonsterNumber = 508, ItemNumber = 103 },
        new() { MonsterNumber = 509, ItemNumber = 104 },
        new() { MonsterNumber = 510, ItemNumber = 105 },
        new() { MonsterNumber = 511, ItemNumber = 106 },
        new() { MonsterNumber = 507, ItemNumber = 107 },
        new() { MonsterNumber = 506, ItemNumber = 108 },
    };

    /// <summary>
    /// Gets or sets the chances in percent to drop one, two, three, ... fragments.
    /// </summary>
    public IList<int> FragmentCountChances { get; set; } = new List<int> { 50, 29, 21 };

    /// <summary>
    /// Gets or sets the experience which the players get when the event has been completed,
    /// depending on their level (including the master level).
    /// </summary>
    public IList<ImperialGuardianExperienceReward> ExperienceRewards { get; set; } = new List<ImperialGuardianExperienceReward>
    {
        new() { MaximumPlayerLevel = int.MaxValue, Experience = 100_000 },
    };

    /// <summary>
    /// Gets or sets the multiplier of the experience reward on sunday.
    /// </summary>
    public int SundayExperienceMultiplier { get; set; } = 2;

    /// <summary>
    /// Gets or sets the scaling of the monsters, depending on the highest level (including the master level)
    /// of the players. When it's empty, the monsters aren't scaled.
    /// </summary>
    public IList<ImperialGuardianMonsterScaling> MonsterScalings { get; set; } = new List<ImperialGuardianMonsterScaling>();

    /// <summary>
    /// Gets the wave number of the monster spawns of a zone.
    /// </summary>
    /// <param name="day">The day of the week (1 = monday, ..., 7 = sunday).</param>
    /// <param name="zone">The zone, starting at 0.</param>
    /// <returns>The wave number.</returns>
    public static byte GetWaveNumber(byte day, int zone) => (byte)((day * 10) + zone);

    /// <summary>
    /// Gets the experience reward for a player.
    /// </summary>
    /// <param name="playerLevel">The level of the player, including the master level.</param>
    /// <param name="day">The day of the week.</param>
    /// <returns>The experience.</returns>
    public int GetExperienceReward(int playerLevel, byte day)
    {
        var experience = this.ExperienceRewards
            .OrderBy(reward => reward.MaximumPlayerLevel)
            .FirstOrDefault(reward => playerLevel <= reward.MaximumPlayerLevel)?.Experience ?? 0;
        return day == Sunday ? experience * this.SundayExperienceMultiplier : experience;
    }

    /// <summary>
    /// Gets the scaling of the monsters for the highest level of the players.
    /// </summary>
    /// <param name="playerLevel">The highest level of the players, including the master level.</param>
    /// <returns>The scaling, or <c>null</c> if the monsters aren't scaled.</returns>
    public ImperialGuardianMonsterScaling? GetMonsterScaling(int playerLevel)
    {
        return this.MonsterScalings
            .OrderBy(scaling => scaling.MaximumPlayerLevel)
            .FirstOrDefault(scaling => playerLevel <= scaling.MaximumPlayerLevel)
               ?? this.MonsterScalings.MaxBy(scaling => scaling.MaximumPlayerLevel);
    }

    /// <summary>
    /// Gets the number of fragments which a boss drops, based on the <see cref="FragmentCountChances"/>.
    /// </summary>
    /// <param name="randomPercent">A random number from 0 to 99.</param>
    /// <returns>The number of fragments.</returns>
    public int GetFragmentCount(int randomPercent)
    {
        var count = 0;
        foreach (var chance in this.FragmentCountChances)
        {
            count++;
            randomPercent -= chance;
            if (randomPercent < 0)
            {
                return count;
            }
        }

        return Math.Max(count, 1);
    }
}
