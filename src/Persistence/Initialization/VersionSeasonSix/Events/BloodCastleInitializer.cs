// <copyright file="BloodCastleInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

/// <summary>
/// The initializer for the blood castle event.
/// </summary>
internal class BloodCastleInitializer : InitializerBase
{
    private const short CastleGateNumber = 131;
    private const short StatueOfSaintNumber = 132;

    private const short RequiredKillsBeforeBridgePerPlayer = 40;
    private const short RequiredKillsAfterGatePerPlayer = 2;
    private static readonly Point StatusOfSaintSpawnPoint = new (14, 95);

    /// <summary>
    /// The score penalty which gets applied when the event wasn't won by any participating player.
    /// </summary>
    private const int ScorePenaltyAtLoss = -300;

    /// <summary>
    /// Gets the rewards based on game level and rank, in case the event was won (even by another player).
    /// </summary>
    private static readonly List<(int GameLevel, (int Gate, int Statue, int Success) ExperiencePerGoal, int ExperiencePerRemainingSecond, (int Winners, int Losers) Money)> RewardTable = new()
    {
        (1, (20000, 20000, 5000), 160, (20000, 10000)),
        (2, (50000, 50000, 10000), 180, (50000, 25000)),
        (3, (80000, 80000, 15000), 200, (100000, 50000)),
        (4, (90000, 90000, 20000), 220, (150000, 80000)),
        (5, (100000, 100000, 25000), 240, (200000, 100000)),
        (6, (110000, 110000, 30000), 260, (250000, 120000)),
        (7, (120000, 120000, 35000), 280, (250000, 120000)),
        (8, (130000, 130000, 40000), 300, (250000, 120000)),
    };

    private static readonly Dictionary<int, List<(MiniGameSuccessFlags Success, int Score)>> ScoreTableWithWinner = new ()
    {
        {
            1, new ()
            {
                (MiniGameSuccessFlags.Alive, 600),
                (MiniGameSuccessFlags.Dead, 300),
                (MiniGameSuccessFlags.Winner, 400),
                (MiniGameSuccessFlags.WinningParty | MiniGameSuccessFlags.Alive, 200),
                (MiniGameSuccessFlags.WinningParty | MiniGameSuccessFlags.Dead, 100),
            }
        },
        {
            2, new ()
            {
                (MiniGameSuccessFlags.Alive, 600),
                (MiniGameSuccessFlags.Dead, 300),
                (MiniGameSuccessFlags.Winner, 400),
                (MiniGameSuccessFlags.WinningParty | MiniGameSuccessFlags.Alive, 200),
                (MiniGameSuccessFlags.WinningParty | MiniGameSuccessFlags.Dead, 100),
            }
        },
        {
            3, new ()
            {
                (MiniGameSuccessFlags.Alive, 600),
                (MiniGameSuccessFlags.Dead, 300),
                (MiniGameSuccessFlags.Winner, 405),
                (MiniGameSuccessFlags.WinningParty | MiniGameSuccessFlags.Alive, 200),
                (MiniGameSuccessFlags.WinningParty | MiniGameSuccessFlags.Dead, 100),
            }
        },
        {
            4, new ()
            {
                (MiniGameSuccessFlags.Alive, 600),
                (MiniGameSuccessFlags.Dead, 300),
                (MiniGameSuccessFlags.Winner, 405),
                (MiniGameSuccessFlags.WinningParty | MiniGameSuccessFlags.Alive, 200),
                (MiniGameSuccessFlags.WinningParty | MiniGameSuccessFlags.Dead, 100),
            }
        },
        {
            5, new ()
            {
                (MiniGameSuccessFlags.Alive, 600),
                (MiniGameSuccessFlags.Dead, 300),
                (MiniGameSuccessFlags.Winner, 405),
                (MiniGameSuccessFlags.WinningParty | MiniGameSuccessFlags.Alive, 200),
                (MiniGameSuccessFlags.WinningParty | MiniGameSuccessFlags.Dead, 100),
            }
        },
        {
            6, new ()
            {
                (MiniGameSuccessFlags.Alive, 600),
                (MiniGameSuccessFlags.Dead, 300),
                (MiniGameSuccessFlags.Winner, 405),
                (MiniGameSuccessFlags.WinningParty | MiniGameSuccessFlags.Alive, 200),
                (MiniGameSuccessFlags.WinningParty | MiniGameSuccessFlags.Dead, 100),
            }
        },
        {
            7, new ()
            {
                (MiniGameSuccessFlags.Alive, 600),
                (MiniGameSuccessFlags.Dead, 300),
                (MiniGameSuccessFlags.Winner, 405),
                (MiniGameSuccessFlags.WinningParty | MiniGameSuccessFlags.Alive, 200),
                (MiniGameSuccessFlags.WinningParty | MiniGameSuccessFlags.Dead, 100),
            }
        },
        {
            8, new ()
            {
                (MiniGameSuccessFlags.Alive, 600),
                (MiniGameSuccessFlags.Dead, 300),
                (MiniGameSuccessFlags.Winner, 405),
                (MiniGameSuccessFlags.WinningParty | MiniGameSuccessFlags.Alive, 200),
                (MiniGameSuccessFlags.WinningParty | MiniGameSuccessFlags.Dead, 100),
            }
        },
    };

    /// <summary>
    /// A set of monster ids which should count as kill after the gate has been destroyed.
    /// These are all for "Spirit Sorcerer" monsters, for the different levels of blood castle.
    /// </summary>
    private static readonly short[] SpiritSorcererPerCastleLevel = { -1, 89, 95, 112, 118, 124, 130, 143, 433 };

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
        bloodCastle.SetGuid((short)MiniGameType.BloodCastle, level);
        this.GameConfiguration.MiniGameDefinitions.Add(bloodCastle);
        bloodCastle.Name = $"Blood Castle {level}";
        bloodCastle.Description = $"Event definition for blood castle event, level {level}.";
        bloodCastle.EnterDuration = TimeSpan.FromMinutes(1);
        bloodCastle.GameDuration = TimeSpan.FromMinutes(20);
        bloodCastle.ExitDuration = TimeSpan.FromMinutes(1);
        bloodCastle.MaximumPlayerCount = 10;
        bloodCastle.Entrance = this.GameConfiguration.Maps.First(m => m.Number == mapNumber).ExitGates.Single();
        bloodCastle.Type = MiniGameType.BloodCastle;
        bloodCastle.TicketItem = this.GameConfiguration.Items.Single(item => item.Group == 13 && item.Number == 18);
        bloodCastle.TicketItemLevel = level;
        bloodCastle.GameLevel = level;
        bloodCastle.MapCreationPolicy = MiniGameMapCreationPolicy.Shared;
        bloodCastle.SaveRankingStatistics = true;
        bloodCastle.AllowParty = true;

        this.CreateRewards(level, bloodCastle);
        this.CreateEvents(level, bloodCastle);

        return bloodCastle;
    }

    private void CreateEvents(byte level, MiniGameDefinition bloodCastle)
    {
        var entranceToggleEvent = this.Context.CreateNew<MiniGameChangeEvent>();
        bloodCastle.ChangeEvents.Add(entranceToggleEvent);
        entranceToggleEvent.Index = 0;
        entranceToggleEvent.Description = "Entrance Toggle Event";
        var entranceToggleArea = this.Context.CreateNew<MiniGameTerrainChange>();
        entranceToggleEvent.TerrainChanges.Add(entranceToggleArea);
        entranceToggleArea.StartX = 13;
        entranceToggleArea.StartY = 15;
        entranceToggleArea.EndX = 15;
        entranceToggleArea.EndY = 23;
        entranceToggleArea.SetTerrainAttribute = false;
        entranceToggleArea.TerrainAttribute = TerrainAttributeType.Blocked;
        entranceToggleArea.IsClientUpdateRequired = true;

        // The next two areas are already unlocked because of the monster spawns.
        var behindGateArea = this.Context.CreateNew<MiniGameTerrainChange>();
        entranceToggleEvent.TerrainChanges.Add(behindGateArea);
        behindGateArea.StartX = 11;
        behindGateArea.StartY = 78;
        behindGateArea.EndX = 25;
        behindGateArea.EndY = 89;
        behindGateArea.SetTerrainAttribute = false;
        behindGateArea.TerrainAttribute = TerrainAttributeType.Blocked;
        behindGateArea.IsClientUpdateRequired = true;

        var altarArea = this.Context.CreateNew<MiniGameTerrainChange>();
        entranceToggleEvent.TerrainChanges.Add(altarArea);
        altarArea.StartX = 8;
        altarArea.StartY = 78;
        altarArea.EndX = 11;
        altarArea.EndY = 83;
        altarArea.SetTerrainAttribute = false;
        altarArea.TerrainAttribute = TerrainAttributeType.Blocked;
        altarArea.IsClientUpdateRequired = true;

        var bridgeToggleEvent = this.Context.CreateNew<MiniGameChangeEvent>();
        bloodCastle.ChangeEvents.Add(bridgeToggleEvent);
        bridgeToggleEvent.Index = 1;
        bridgeToggleEvent.Description = "Bridge Toggle Event";
        bridgeToggleEvent.Message = "Enough monster kills, now destroy the Castle Gate!";
        bridgeToggleEvent.Target = KillTarget.AnyMonster;
        bridgeToggleEvent.NumberOfKills = RequiredKillsBeforeBridgePerPlayer;
        bridgeToggleEvent.MultiplyKillsByPlayers = true;
        var bridgeToggleArea = this.Context.CreateNew<MiniGameTerrainChange>();
        bridgeToggleEvent.TerrainChanges.Add(bridgeToggleArea);
        bridgeToggleArea.StartX = 13;
        bridgeToggleArea.StartY = 70;
        bridgeToggleArea.EndX = 15;
        bridgeToggleArea.EndY = 75;
        bridgeToggleArea.SetTerrainAttribute = false;
        bridgeToggleArea.TerrainAttribute = TerrainAttributeType.NoGround;
        bridgeToggleArea.IsClientUpdateRequired = true;

        var gateToggleEvent = this.Context.CreateNew<MiniGameChangeEvent>();
        bloodCastle.ChangeEvents.Add(gateToggleEvent);
        gateToggleEvent.Index = 2;
        gateToggleEvent.Description = "Gate Toggle Event";
        gateToggleEvent.Target = KillTarget.Specific;
        gateToggleEvent.TargetDefinition = this.GameConfiguration.Monsters.First(m => m.Number == CastleGateNumber);
        gateToggleEvent.NumberOfKills = 1;
        gateToggleEvent.Message = "{0} has demolished the Castle Gate!";
        var gateToggleArea = this.Context.CreateNew<MiniGameTerrainChange>();
        gateToggleEvent.TerrainChanges.Add(gateToggleArea);
        gateToggleArea.StartX = 13;
        gateToggleArea.StartY = 76;
        gateToggleArea.EndX = 15;
        gateToggleArea.EndY = 79;
        gateToggleArea.SetTerrainAttribute = false;
        gateToggleArea.TerrainAttribute = TerrainAttributeType.Blocked;
        gateToggleArea.IsClientUpdateRequired = true;

        var spawnStatueEvent = this.Context.CreateNew<MiniGameChangeEvent>();
        bloodCastle.ChangeEvents.Add(spawnStatueEvent);
        spawnStatueEvent.Index = 3;
        spawnStatueEvent.Description = "Statue Spawn Event";
        spawnStatueEvent.Message = "Kundun minions have been subdued! Destroy the Crystal Statue!";
        spawnStatueEvent.Target = KillTarget.Specific;
        spawnStatueEvent.TargetDefinition = this.GameConfiguration.Monsters.First(m => m.Number == SpiritSorcererPerCastleLevel[level]);
        spawnStatueEvent.NumberOfKills = RequiredKillsAfterGatePerPlayer;
        spawnStatueEvent.MultiplyKillsByPlayers = true;

        spawnStatueEvent.SpawnArea = this.Context.CreateNew<MonsterSpawnArea>();
        spawnStatueEvent.SpawnArea.MonsterDefinition = this.GameConfiguration.Monsters.First(m => m.Number == StatueOfSaintNumber);
        spawnStatueEvent.SpawnArea.MaximumHealthOverride = BloodCastleBase.CrystalStatueHealthPerLevel[level];
        spawnStatueEvent.SpawnArea.Direction = Direction.SouthWest;
        spawnStatueEvent.SpawnArea.Quantity = 1;
        spawnStatueEvent.SpawnArea.X1 = StatusOfSaintSpawnPoint.X;
        spawnStatueEvent.SpawnArea.X2 = StatusOfSaintSpawnPoint.X;
        spawnStatueEvent.SpawnArea.Y1 = StatusOfSaintSpawnPoint.Y;
        spawnStatueEvent.SpawnArea.Y2 = StatusOfSaintSpawnPoint.Y;
        spawnStatueEvent.SpawnArea.SpawnTrigger = SpawnTrigger.OnceAtWaveStart;
    }

    private void CreateRewards(byte level, MiniGameDefinition bloodCastle)
    {
        var rewardDropItemGroup = this.Context.CreateNew<DropItemGroup>();
        rewardDropItemGroup.Description = $"Rewarded items for Blood Castle {level}";
        rewardDropItemGroup.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos"));
        this.GameConfiguration.DropItemGroups.Add(rewardDropItemGroup);

        var rewardTableEntry = RewardTable.First(tuple => tuple.GameLevel == level);

        var gateExpReward = this.Context.CreateNew<MiniGameReward>();
        gateExpReward.RewardType = MiniGameRewardType.Experience;
        gateExpReward.RewardAmount = rewardTableEntry.ExperiencePerGoal.Gate;
        gateExpReward.RequiredKill = this.GameConfiguration.Monsters.First(m => m.Number == 131);
        bloodCastle.Rewards.Add(gateExpReward);

        var statueExpReward = this.Context.CreateNew<MiniGameReward>();
        statueExpReward.RewardType = MiniGameRewardType.Experience;
        statueExpReward.RewardAmount = rewardTableEntry.ExperiencePerGoal.Statue;
        statueExpReward.RequiredKill = this.GameConfiguration.Monsters.First(m => m.Number == 132);
        bloodCastle.Rewards.Add(statueExpReward);

        var successExpReward = this.Context.CreateNew<MiniGameReward>();
        successExpReward.RewardType = MiniGameRewardType.Experience;
        successExpReward.RewardAmount = rewardTableEntry.ExperiencePerGoal.Success;
        successExpReward.RequiredSuccess = MiniGameSuccessFlags.WinnerOrInWinningParty;
        bloodCastle.Rewards.Add(successExpReward);

        var remainingSecondsExpReward = this.Context.CreateNew<MiniGameReward>();
        remainingSecondsExpReward.RewardType = MiniGameRewardType.ExperiencePerRemainingSeconds;
        remainingSecondsExpReward.RewardAmount = rewardTableEntry.ExperiencePerRemainingSecond;
        bloodCastle.Rewards.Add(remainingSecondsExpReward);

        var winnersMoneyReward = this.Context.CreateNew<MiniGameReward>();
        winnersMoneyReward.RewardType = MiniGameRewardType.Money;
        winnersMoneyReward.RewardAmount = rewardTableEntry.Money.Winners;
        winnersMoneyReward.RequiredSuccess = MiniGameSuccessFlags.WinnerOrInWinningParty;
        bloodCastle.Rewards.Add(winnersMoneyReward);

        var losersMoneyReward = this.Context.CreateNew<MiniGameReward>();
        losersMoneyReward.RewardType = MiniGameRewardType.Money;
        losersMoneyReward.RewardAmount = rewardTableEntry.Money.Losers;
        losersMoneyReward.RequiredSuccess = MiniGameSuccessFlags.Loser;
        bloodCastle.Rewards.Add(losersMoneyReward);

        var itemReward = this.Context.CreateNew<MiniGameReward>();
        itemReward.ItemReward = rewardDropItemGroup;
        itemReward.RewardAmount = 1;
        itemReward.RewardType = MiniGameRewardType.ItemDrop;
        itemReward.RequiredSuccess = MiniGameSuccessFlags.WinnerOrInWinningParty | MiniGameSuccessFlags.Alive;
        bloodCastle.Rewards.Add(itemReward);

        if (ScoreTableWithWinner.TryGetValue(level, out var scoreTableEntry))
        {
            foreach (var s in scoreTableEntry)
            {
                var scoreReward = this.Context.CreateNew<MiniGameReward>();
                scoreReward.RequiredSuccess = s.Success | MiniGameSuccessFlags.WinnerExists;
                scoreReward.RewardAmount = s.Score;
                scoreReward.RewardType = MiniGameRewardType.Score;
                bloodCastle.Rewards.Add(scoreReward);
            }
        }

        var scoreRewardLosing = this.Context.CreateNew<MiniGameReward>();
        scoreRewardLosing.RequiredSuccess = MiniGameSuccessFlags.WinnerNotExists;
        scoreRewardLosing.RewardAmount = ScorePenaltyAtLoss;
        scoreRewardLosing.RewardType = MiniGameRewardType.Score;
        bloodCastle.Rewards.Add(scoreRewardLosing);
    }
}