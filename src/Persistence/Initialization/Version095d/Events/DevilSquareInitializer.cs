// <copyright file="DevilSquareInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d.Events;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The initializer for the devil square event.
/// </summary>
internal class DevilSquareInitializer : InitializerBase
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
    /// The client-side map number of the map of devil square 1 to 4.
    /// </summary>
    private const short DevilSquare1To4Number = 9;

    /// <summary>
    /// The client-side map number of the map of devil square 5 to 7.
    /// </summary>
    private const short DevilSquare5To7Number = 32;

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
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="DevilSquareInitializer" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public DevilSquareInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        var devilSquare1 = this.CreateDevilSquareDefinition(1);
        devilSquare1.MinimumCharacterLevel = 15;
        devilSquare1.MaximumCharacterLevel = 130;
        devilSquare1.MinimumSpecialCharacterLevel = 10;
        devilSquare1.MaximumSpecialCharacterLevel = 110;

        var devilSquare2 = this.CreateDevilSquareDefinition(2);
        devilSquare2.MinimumCharacterLevel = 131;
        devilSquare2.MaximumCharacterLevel = 180;
        devilSquare2.MinimumSpecialCharacterLevel = 111;
        devilSquare2.MaximumSpecialCharacterLevel = 160;

        var devilSquare3 = this.CreateDevilSquareDefinition(3);
        devilSquare3.MinimumCharacterLevel = 181;
        devilSquare3.MaximumCharacterLevel = 230;
        devilSquare3.MinimumSpecialCharacterLevel = 161;
        devilSquare3.MaximumSpecialCharacterLevel = 210;

        var devilSquare4 = this.CreateDevilSquareDefinition(4);
        devilSquare4.MinimumCharacterLevel = 231;
        devilSquare4.MaximumCharacterLevel = 280;
        devilSquare4.MinimumSpecialCharacterLevel = 211;
        devilSquare4.MaximumSpecialCharacterLevel = 260;
    }

    /// <summary>
    /// Creates a new <see cref="MiniGameDefinition"/> for a devil square event.
    /// </summary>
    /// <param name="level">THe level of the event.</param>
    /// <returns>The created <see cref="MiniGameDefinition"/>.</returns>
    protected MiniGameDefinition CreateDevilSquareDefinition(byte level)
    {
        var devilSquare = this.Context.CreateNew<MiniGameDefinition>();
        this.GameConfiguration.MiniGameDefinitions.Add(devilSquare);
        devilSquare.SetGuid((short)MiniGameType.DevilSquare, level);
        devilSquare.Name = $"Devil Square {level}";
        devilSquare.Description = $"Event definition for devil square event, level {level}.";
        devilSquare.EnterDuration = TimeSpan.FromMinutes(1);
        devilSquare.GameDuration = TimeSpan.FromMinutes(20);
        devilSquare.ExitDuration = TimeSpan.FromMinutes(3);
        devilSquare.MaximumPlayerCount = 10;
        var mapNumber = level < 5 ? DevilSquare1To4Number : DevilSquare5To7Number;
        devilSquare.Entrance = this.GameConfiguration.Maps.First(m => m.Number == mapNumber && m.Discriminator == level).ExitGates.Single();
        devilSquare.Type = MiniGameType.DevilSquare;
        devilSquare.TicketItem = this.GameConfiguration.Items.Single(item => item.Group == 14 && item.Number == 19);
        devilSquare.TicketItemLevel = level;
        devilSquare.GameLevel = level;
        devilSquare.MapCreationPolicy = MiniGameMapCreationPolicy.Shared;
        devilSquare.SaveRankingStatistics = true;
        devilSquare.AllowParty = true;

        this.CreateRewards(level, devilSquare);
        this.CreateWaves(devilSquare);

        return devilSquare;
    }

    private void CreateRewards(byte level, MiniGameDefinition devilSquare)
    {
        for (int rank = 1; rank <= 4; rank++)
        {
            var rewardTableEntry = RewardTable.First(tuple => tuple.Rank == rank && tuple.GameLevel == level);

            var expReward = this.Context.CreateNew<MiniGameReward>();
            expReward.RewardType = MiniGameRewardType.Experience;
            expReward.Rank = rank;
            expReward.RewardAmount = rewardTableEntry.Experience;
            devilSquare.Rewards.Add(expReward);

            var moneyReward = this.Context.CreateNew<MiniGameReward>();
            moneyReward.RewardType = MiniGameRewardType.Money;
            moneyReward.Rank = rank;
            moneyReward.RewardAmount = rewardTableEntry.Money;
            devilSquare.Rewards.Add(moneyReward);
        }
    }

    private void CreateWaves(MiniGameDefinition devilSquare)
    {
        var firstWave = this.Context.CreateNew<MiniGameSpawnWave>();
        firstWave.WaveNumber = FirstWaveNumber;
        firstWave.Description = $"The first wave of devil square {devilSquare.GameLevel}";
        firstWave.StartTime = TimeSpan.Zero;
        firstWave.EndTime = TimeSpan.FromMinutes(7);
        devilSquare.SpawnWaves.Add(firstWave);

        var secondWave = this.Context.CreateNew<MiniGameSpawnWave>();
        secondWave.WaveNumber = SecondWaveNumber;
        secondWave.Description = $"The second wave of devil square {devilSquare.GameLevel}";
        secondWave.Message = "Lets continue with some stronger enemies ...";
        secondWave.StartTime = TimeSpan.FromMinutes(5);
        secondWave.EndTime = TimeSpan.FromMinutes(14);
        devilSquare.SpawnWaves.Add(secondWave);

        var thirdWave = this.Context.CreateNew<MiniGameSpawnWave>();
        thirdWave.WaveNumber = ThirdWaveNumber;
        thirdWave.Description = $"The third wave of devil square {devilSquare.GameLevel}";
        thirdWave.Message = "Still alive? I have more for you.";
        thirdWave.StartTime = TimeSpan.FromMinutes(12);
        thirdWave.EndTime = TimeSpan.FromMinutes(20);
        devilSquare.SpawnWaves.Add(thirdWave);

        var bossWave = this.Context.CreateNew<MiniGameSpawnWave>();
        bossWave.WaveNumber = BossWaveNumber;
        bossWave.Description = $"The boss wave of devil square {devilSquare.GameLevel}";
        bossWave.Message = "Beware of the bosses! You have 5 minutes left.";
        bossWave.StartTime = TimeSpan.FromMinutes(15);
        bossWave.EndTime = TimeSpan.FromMinutes(20);
        devilSquare.SpawnWaves.Add(bossWave);
    }
}