// <copyright file="BloodCastleInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The initializer for the blood castle event.
/// </summary>
internal class BloodCastleInitializer : InitializerBase
{
    /// <summary>
    /// The <see cref="MiniGameSpawnWave.WaveNumber"/> for the first wave.
    /// </summary>
    internal const int FirstWaveNumber = 1;

    /// <summary>
    /// The <see cref="MiniGameSpawnWave.WaveNumber"/> for the second wave.
    /// </summary>
    internal const int SecondWaveNumber = 2;

    /// <summary>
    /// The <see cref="MiniGameSpawnWave.WaveNumber"/> for the third wave.
    /// </summary>
    internal const int ThirdWaveNumber = 3;

    /// <summary>
    /// The <see cref="MiniGameSpawnWave.WaveNumber"/> for the boss wave.
    /// </summary>
    internal const int BossWaveNumber = 10;

    /// <summary>
    /// Gets the rewards based on game level and rank.
    /// </summary>
    private static readonly List<(int GameLevel, int Rank, int Experience, int Money)> RewardTable = new ()
    {
        (1, 1, 6000, 30000),
        (1, 2, 4000, 25000),
        (1, 3, 2000, 20000),
        (1, 4, 1000, 15000),

        (2, 1, 8000, 40000),
        (2, 2, 6000, 35000),
        (2, 3, 4000, 30000),
        (2, 4, 2000, 25000),

        (3, 1, 10000, 50000),
        (3, 2, 8000, 45000),
        (3, 3, 6000, 40000),
        (3, 4, 4000, 35000),

        (4, 1, 20000, 60000),
        (4, 2, 10000, 55000),
        (4, 3, 8000, 50000),
        (4, 4, 6000, 45000),

        (5, 1, 22000, 70000),
        (5, 2, 20000, 65000),
        (5, 3, 10000, 60000),
        (5, 4, 8000, 55000),

        (6, 1, 24000, 80000),
        (6, 2, 22000, 75000),
        (6, 3, 20000, 70000),
        (6, 4, 10000, 65000),

        (7, 1, 26000, 150000),
        (7, 2, 24000, 140000),
        (7, 3, 22000, 120000),
        (7, 4, 20000, 90000),

        (8, 1, 26000, 150000),
        (8, 2, 24000, 140000),
        (8, 3, 22000, 120000),
        (8, 4, 20000, 90000),
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="BloodCastleInitializer" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public BloodCastleInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        var bloodCastle1 = this.CreateBloodCastleDefinition(1, 11);
        bloodCastle1.MinimumCharacterLevel = 15;
        bloodCastle1.MaximumCharacterLevel = 80;
        bloodCastle1.MinimumSpecialCharacterLevel = 10;
        bloodCastle1.MaximumSpecialCharacterLevel = 60;

        var bloodCastle2 = this.CreateBloodCastleDefinition(2, 12);
        bloodCastle2.MinimumCharacterLevel = 81;
        bloodCastle2.MaximumCharacterLevel = 130;
        bloodCastle2.MinimumSpecialCharacterLevel = 61;
        bloodCastle2.MaximumSpecialCharacterLevel = 110;

        var bloodCastle3 = this.CreateBloodCastleDefinition(3, 13);
        bloodCastle3.MinimumCharacterLevel = 131;
        bloodCastle3.MaximumCharacterLevel = 180;
        bloodCastle3.MinimumSpecialCharacterLevel = 111;
        bloodCastle3.MaximumSpecialCharacterLevel = 160;

        var bloodCastle4 = this.CreateBloodCastleDefinition(4, 14);
        bloodCastle4.MinimumCharacterLevel = 181;
        bloodCastle4.MaximumCharacterLevel = 230;
        bloodCastle4.MinimumSpecialCharacterLevel = 161;
        bloodCastle4.MaximumSpecialCharacterLevel = 210;

        var bloodCastle5 = this.CreateBloodCastleDefinition(5, 15);
        bloodCastle5.MinimumCharacterLevel = 231;
        bloodCastle5.MaximumCharacterLevel = 280;
        bloodCastle5.MinimumSpecialCharacterLevel = 211;
        bloodCastle5.MaximumSpecialCharacterLevel = 260;

        var bloodCastle6 = this.CreateBloodCastleDefinition(6, 16);
        bloodCastle6.MinimumCharacterLevel = 281;
        bloodCastle6.MaximumCharacterLevel = 330;
        bloodCastle6.MinimumSpecialCharacterLevel = 261;
        bloodCastle6.MaximumSpecialCharacterLevel = 310;

        var bloodCastle7 = this.CreateBloodCastleDefinition(7, 17);
        bloodCastle7.MinimumCharacterLevel = 331;
        bloodCastle7.MaximumCharacterLevel = 400;
        bloodCastle7.MinimumSpecialCharacterLevel = 311;
        bloodCastle7.MaximumSpecialCharacterLevel = 400;

        var bloodCastle8 = this.CreateBloodCastleDefinition(8, 52);
        bloodCastle8.RequiresMasterClass = true;
        bloodCastle8.MinimumCharacterLevel = 331;
        bloodCastle8.MaximumCharacterLevel = 400;
        bloodCastle8.MinimumSpecialCharacterLevel = 0;
        bloodCastle8.MaximumSpecialCharacterLevel = 400;
    }

    /// <summary>
    /// Creates a new <see cref="MiniGameDefinition"/> for a blood castle event.
    /// </summary>
    /// <param name="level">The level of the event.</param>
    /// <param name="mapNumber">The map number.</param>
    /// <returns>The created <see cref="MiniGameDefinition"/>.</returns>
    protected MiniGameDefinition CreateBloodCastleDefinition(byte level, short mapNumber)
    {
        var bloodCastle = this.Context.CreateNew<MiniGameDefinition>();
        this.GameConfiguration.MiniGameDefinitions.Add(bloodCastle);
        bloodCastle.Name = $"Blood Castle {level}";
        bloodCastle.Description = $"Event definition for blood castle event, level {level}.";
        bloodCastle.EnterDuration = TimeSpan.FromMinutes(1);
        bloodCastle.GameDuration = TimeSpan.FromMinutes(20);
        bloodCastle.ExitDuration = TimeSpan.FromMinutes(3);
        bloodCastle.MaximumPlayerCount = 5;
        bloodCastle.Entrance = this.GameConfiguration.Maps.First(m => m.Number == mapNumber).ExitGates.Single();
        bloodCastle.Type = MiniGameType.BloodCastle;
        bloodCastle.TicketItem = this.GameConfiguration.Items.Single(item => item.Group == 13 && item.Number == 18);
        bloodCastle.TicketItemLevel = level;
        bloodCastle.GameLevel = level;
        bloodCastle.MapCreationPolicy = MiniGameMapCreationPolicy.OnePerParty;
        bloodCastle.SaveRankingStatistics = true;

        this.CreateRewards(level, bloodCastle);
        this.CreateWaves(bloodCastle);

        return bloodCastle;
    }

    private void CreateRewards(byte level, MiniGameDefinition bloodCastle)
    {
        for (int rank = 1; rank <= 4; rank++)
        {
            var rewardTableEntry = RewardTable.First(tuple => tuple.Rank == rank && tuple.GameLevel == level);

            var expReward = this.Context.CreateNew<MiniGameReward>();
            expReward.RewardType = MiniGameRewardType.Experience;
            expReward.Rank = rank;
            expReward.RewardAmount = rewardTableEntry.Experience;
            bloodCastle.Rewards.Add(expReward);

            var moneyReward = this.Context.CreateNew<MiniGameReward>();
            moneyReward.RewardType = MiniGameRewardType.Money;
            moneyReward.Rank = rank;
            moneyReward.RewardAmount = rewardTableEntry.Money;
            bloodCastle.Rewards.Add(moneyReward);
        }
    }

    private void CreateWaves(MiniGameDefinition bloodCastle)
    {
        var firstWave = this.Context.CreateNew<MiniGameSpawnWave>();
        firstWave.WaveNumber = FirstWaveNumber;
        firstWave.Description = $"The first wave of blood castle {bloodCastle.GameLevel}";
        firstWave.StartTime = TimeSpan.Zero;
        firstWave.EndTime = TimeSpan.FromMinutes(7);
        bloodCastle.SpawnWaves.Add(firstWave);

        var secondWave = this.Context.CreateNew<MiniGameSpawnWave>();
        secondWave.WaveNumber = SecondWaveNumber;
        secondWave.Description = $"The second wave of blood castle {bloodCastle.GameLevel}";
        secondWave.Message = "Lets continue with some stronger enemies ...";
        secondWave.StartTime = TimeSpan.FromMinutes(5);
        secondWave.EndTime = TimeSpan.FromMinutes(14);
        bloodCastle.SpawnWaves.Add(secondWave);

        var thirdWave = this.Context.CreateNew<MiniGameSpawnWave>();
        thirdWave.WaveNumber = ThirdWaveNumber;
        thirdWave.Description = $"The third wave of blood castle {bloodCastle.GameLevel}";
        thirdWave.Message = "Still alive? I have more for you.";
        thirdWave.StartTime = TimeSpan.FromMinutes(12);
        thirdWave.EndTime = TimeSpan.FromMinutes(20);
        bloodCastle.SpawnWaves.Add(thirdWave);

        var bossWave = this.Context.CreateNew<MiniGameSpawnWave>();
        bossWave.WaveNumber = BossWaveNumber;
        bossWave.Description = $"The boss wave of blood castle {bloodCastle.GameLevel}";
        bossWave.Message = "Beware of the bosses! You have 5 minutes left.";
        bossWave.StartTime = TimeSpan.FromMinutes(15);
        bossWave.EndTime = TimeSpan.FromMinutes(20);
        bloodCastle.SpawnWaves.Add(bossWave);
    }
}