// <copyright file="CrywolfEventDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Crywolf;

/// <summary>
/// Describes the run of the crywolf event.
/// </summary>
/// <remarks>
/// It's configured at the <see cref="CrywolfPlugIn"/>, so it can be adapted in the admin panel.
/// The monsters of the event are defined by the monster spawns of the crywolf map with the trigger
/// <see cref="SpawnTrigger.OnceAtWaveStart"/> and the wave numbers of the <see cref="MonsterGroups"/>
/// and <see cref="BalgassWaveNumber"/>. The default values are the ones of the original game.
/// </remarks>
public class CrywolfEventDefinition
{
    /// <summary>
    /// Gets or sets the number of the crywolf map.
    /// </summary>
    public short MapNumber { get; set; } = 34;

    /// <summary>
    /// Gets or sets the days of the week, on which the event starts.
    /// </summary>
    public IList<DayOfWeek> StartDays { get; set; } = new List<DayOfWeek> { DayOfWeek.Wednesday, DayOfWeek.Saturday };

    /// <summary>
    /// Gets or sets the times of the day (in the time zone of the server), at which the event starts with its first notification.
    /// </summary>
    public IList<TimeOnly> StartTimes { get; set; } = new List<TimeOnly> { new(20, 30) };

    /// <summary>
    /// Gets or sets the duration of the first notification of the upcoming attack.
    /// </summary>
    public TimeSpan Notify1Duration { get; set; } = TimeSpan.FromMinutes(10);

    /// <summary>
    /// Gets or sets the duration of the second notification, in which the fortress prepares for the attack.
    /// </summary>
    public TimeSpan Notify2Duration { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets the duration, in which the elves can contract the altars before the battle starts.
    /// </summary>
    public TimeSpan ReadyDuration { get; set; } = TimeSpan.FromSeconds(150);

    /// <summary>
    /// Gets or sets the duration of the battle.
    /// </summary>
    public TimeSpan BattleDuration { get; set; } = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Gets or sets the duration, in which the result is shown.
    /// </summary>
    public TimeSpan EndDuration { get; set; } = TimeSpan.FromSeconds(15);

    /// <summary>
    /// Gets or sets the duration, after which the event is finished and the common monsters return.
    /// </summary>
    public TimeSpan EndCycleDuration { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Gets or sets the interval, in which the notifications are repeated.
    /// </summary>
    public TimeSpan NotificationInterval { get; set; } = TimeSpan.FromSeconds(70);

    /// <summary>
    /// Gets or sets the time after the start of the battle, at which the army starts to attack the statue.
    /// </summary>
    public TimeSpan StatueAttackDelay { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets the time after the start of the battle, at which Balgass appears.
    /// </summary>
    public TimeSpan BalgassAppearanceDelay { get; set; } = TimeSpan.FromMinutes(10);

    /// <summary>
    /// Gets or sets the number of the statue of the holy wolf.
    /// </summary>
    public short StatueNumber { get; set; } = 204;

    /// <summary>
    /// Gets or sets the numbers of the altars, in the order which is expected by the client.
    /// </summary>
    public IList<short> AltarNumbers { get; set; } = new List<short> { 205, 206, 207, 208, 209 };

    /// <summary>
    /// Gets or sets the numbers of the character classes, which can contract an altar.
    /// </summary>
    public IList<byte> ContractCharacterClassNumbers { get; set; } = new List<byte> { 8, 10, 11 };

    /// <summary>
    /// Gets or sets the minimum level of a character to contract an altar.
    /// </summary>
    public int MinimumContractLevel { get; set; } = 260;

    /// <summary>
    /// Gets or sets the time which an elf has to stay at the altar until the contract is valid.
    /// </summary>
    public TimeSpan ContractDelay { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Gets or sets the number of contracts per altar and event. A failed attempt counts as contract as well.
    /// </summary>
    public int ContractsPerAltar { get; set; } = 2;

    /// <summary>
    /// Gets or sets the time after a contract ended, in which the altar can't be contracted.
    /// </summary>
    public TimeSpan AltarCooldown { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Gets or sets the number of the Dark Elf, which leads a group of the army.
    /// </summary>
    public short DarkElfNumber { get; set; } = 340;

    /// <summary>
    /// Gets or sets the groups of the army of Balgass.
    /// </summary>
    public IList<CrywolfMonsterGroup> MonsterGroups { get; set; } = new List<CrywolfMonsterGroup>
    {
        new() { WaveNumber = 1, AdvanceGoalX = 128, AdvanceGoalY = 45, AttackGoalX = 126, AttackGoalY = 35 },
        new() { WaveNumber = 2, AdvanceGoalX = 109, AdvanceGoalY = 42, AttackGoalX = 125, AttackGoalY = 27 },
        new() { WaveNumber = 3, AdvanceGoalX = 123, AdvanceGoalY = 84, AttackGoalX = 121, AttackGoalY = 19 },
        new() { WaveNumber = 4, AdvanceGoalX = 119, AdvanceGoalY = 87, AttackGoalX = 119, AttackGoalY = 87 },
        new() { WaveNumber = 6, AdvanceGoalX = 105, AdvanceGoalY = 32, AttackGoalX = 120, AttackGoalY = 38 },
        new() { WaveNumber = 7, AdvanceGoalX = 89, AdvanceGoalY = 32, AttackGoalX = 115, AttackGoalY = 32 },
        new() { WaveNumber = 8, AdvanceGoalX = 83, AdvanceGoalY = 29, AttackGoalX = 109, AttackGoalY = 40 },
        new() { WaveNumber = 9, AdvanceGoalX = 65, AdvanceGoalY = 34, AttackGoalX = 65, AttackGoalY = 34 },
        new() { WaveNumber = 10, AdvanceGoalX = 136, AdvanceGoalY = 38, AttackGoalX = 117, AttackGoalY = 27 },
        new() { WaveNumber = 11, AdvanceGoalX = 134, AdvanceGoalY = 19, AttackGoalX = 117, AttackGoalY = 37 },
        new() { WaveNumber = 12, AdvanceGoalX = 177, AdvanceGoalY = 37, AttackGoalX = 131, AttackGoalY = 31 },
        new() { WaveNumber = 13, AdvanceGoalX = 179, AdvanceGoalY = 33, AttackGoalX = 179, AttackGoalY = 33 },
    };

    /// <summary>
    /// Gets or sets the ballistas of the army, which bombard the fortress.
    /// </summary>
    public IList<CrywolfBallista> Ballistas { get; set; } = new List<CrywolfBallista>
    {
        new() { X = 122, Y = 90, TargetX = 121, TargetY = 52 },
        new() { X = 116, Y = 90, TargetX = 120, TargetY = 35 },
        new() { X = 62, Y = 37, TargetX = 101, TargetY = 31 },
        new() { X = 62, Y = 31, TargetX = 117, TargetY = 31 },
        new() { X = 183, Y = 36, TargetX = 141, TargetY = 32 },
        new() { X = 183, Y = 30, TargetX = 125, TargetY = 31 },
    };

    /// <summary>
    /// Gets or sets the maximum random offset of the bombarded point of a ballista.
    /// </summary>
    public int BallistaTargetSpread { get; set; } = 4;

    /// <summary>
    /// Gets or sets the radius around the bombarded point, in which the players are hit.
    /// </summary>
    public int BallistaHitRadius { get; set; } = 6;

    /// <summary>
    /// Gets or sets the radius around the bombarded point, in which the players see the arrow of the ballista.
    /// </summary>
    public int BallistaEffectRadius { get; set; } = 10;

    /// <summary>
    /// Gets or sets the interval, in which the leader of a group revives one of its dead members.
    /// </summary>
    public TimeSpan MemberReviveInterval { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Gets or sets the distance to the leader, above which the members of a group follow it.
    /// </summary>
    public int FollowLeaderDistance { get; set; } = 6;

    /// <summary>
    /// Gets or sets the wave number of the monster spawn of Balgass.
    /// </summary>
    public byte BalgassWaveNumber { get; set; } = 5;

    /// <summary>
    /// Gets or sets the x coordinate, to which Balgass marches.
    /// </summary>
    public byte BalgassGoalX { get; set; } = 121;

    /// <summary>
    /// Gets or sets the y coordinate, to which Balgass marches.
    /// </summary>
    public byte BalgassGoalY { get; set; } = 36;

    /// <summary>
    /// Gets or sets the scores which the players get for killing the monsters of the army.
    /// </summary>
    public IList<CrywolfMonsterScore> MonsterScores { get; set; } = new List<CrywolfMonsterScore>
    {
        new() { MonsterNumber = 340, Score = 3000 },
        new() { MonsterNumber = 341, Score = 700 },
        new() { MonsterNumber = 344, Score = 600 },
        new() { MonsterNumber = 345, Score = 600 },
        new() { MonsterNumber = 348, Score = 1000 },
        new() { MonsterNumber = 349, Score = 7000 },
    };

    /// <summary>
    /// Gets or sets the score, which an elf gets for having contracted an altar until the end of the battle.
    /// </summary>
    public int ContractorScore { get; set; } = 6000;

    /// <summary>
    /// Gets or sets the minimum scores of the ranks D, C, B, A and S.
    /// </summary>
    public IList<int> RankScores { get; set; } = new List<int> { 0, 1001, 3001, 5001, 10001 };

    /// <summary>
    /// Gets or sets the experience of the ranks D, C, B, A and S.
    /// </summary>
    public IList<int> RankExperience { get; set; } = new List<int> { 0, 200_000, 800_000, 1_200_000, 1_800_000 };

    /// <summary>
    /// Gets or sets the percentage of the rank experience, which the players get when the fortress has been occupied.
    /// </summary>
    public int FailedExperiencePercentage { get; set; } = 10;

    /// <summary>
    /// Gets or sets the number of heroes, which are shown in the result and rewarded.
    /// </summary>
    public int HeroCount { get; set; } = 5;

    /// <summary>
    /// Gets or sets the group of the item, which is dropped for the contractors and heroes when the fortress has been defended.
    /// </summary>
    public byte RewardItemGroup { get; set; } = 14;

    /// <summary>
    /// Gets or sets the number of the item, which is dropped for the contractors and heroes when the fortress has been defended.
    /// </summary>
    public short RewardItemNumber { get; set; } = 13;

    /// <summary>
    /// Gets or sets the numbers of the NPCs of the crywolf map, which stay visible when the fortress is occupied.
    /// </summary>
    public IList<short> AlwaysVisibleNpcNumbers { get; set; } = new List<short> { 406, 407 };

    /// <summary>
    /// Determines whether the event starts at the specified local time of the server.
    /// </summary>
    /// <param name="localTime">The local time of the server.</param>
    /// <returns><c>true</c>, if the event starts at the specified time; otherwise, <c>false</c>.</returns>
    public bool IsStartTime(DateTime localTime)
    {
        var time = TimeOnly.FromDateTime(localTime);
        var earlier = time.Add(TimeSpan.FromSeconds(-5));
        return this.StartDays.Contains(localTime.DayOfWeek)
               && this.StartTimes.Any(startTime => startTime.IsBetween(earlier, time));
    }

    /// <summary>
    /// Gets the duration of the specified state.
    /// </summary>
    /// <param name="state">The state.</param>
    /// <returns>The duration of the state, or <see cref="TimeSpan.Zero"/> if the state has no duration.</returns>
    public TimeSpan GetDuration(CrywolfState state)
    {
        return state switch
        {
            CrywolfState.Notify1 => this.Notify1Duration,
            CrywolfState.Notify2 => this.Notify2Duration,
            CrywolfState.Ready => this.ReadyDuration,
            CrywolfState.Start => this.BattleDuration,
            CrywolfState.End => this.EndDuration,
            CrywolfState.EndCycle => this.EndCycleDuration,
            _ => TimeSpan.Zero,
        };
    }

    /// <summary>
    /// Gets the score which a player gets for killing a monster.
    /// </summary>
    /// <param name="monsterNumber">The number of the monster.</param>
    /// <returns>The score.</returns>
    public int GetScore(short monsterNumber)
    {
        return this.MonsterScores.FirstOrDefault(score => score.MonsterNumber == monsterNumber)?.Score ?? 0;
    }

    /// <summary>
    /// Gets the rank (0 = D to 4 = S) of a score.
    /// </summary>
    /// <param name="score">The score.</param>
    /// <returns>The rank.</returns>
    public int GetRank(int score)
    {
        var rank = 0;
        for (var i = 0; i < this.RankScores.Count; i++)
        {
            if (score >= this.RankScores[i])
            {
                rank = i;
            }
        }

        return Math.Min(rank, 4);
    }

    /// <summary>
    /// Gets the experience which a player gets for its rank.
    /// </summary>
    /// <param name="rank">The rank.</param>
    /// <param name="isDefended">If set to <c>true</c>, the fortress has been defended.</param>
    /// <returns>The experience.</returns>
    public int GetExperience(int rank, bool isDefended)
    {
        var experience = rank >= 0 && rank < this.RankExperience.Count ? this.RankExperience[rank] : 0;
        return isDefended ? experience : (int)((long)experience * this.FailedExperiencePercentage / 100);
    }

    /// <summary>
    /// Gets the index (0 to 4) of an altar.
    /// </summary>
    /// <param name="monsterNumber">The number of the altar.</param>
    /// <returns>The index of the altar, or -1 if it's no altar.</returns>
    public int GetAltarIndex(short monsterNumber)
    {
        return this.AltarNumbers.IndexOf(monsterNumber);
    }
}
