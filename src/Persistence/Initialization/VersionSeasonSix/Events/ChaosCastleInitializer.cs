// <copyright file="ChaosCastleInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

/// <summary>
/// The initializer for the chaos castle event.
/// </summary>
internal class ChaosCastleInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChaosCastleInitializer" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public ChaosCastleInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        var chaosCastle1 = this.CreateChaosCastleDefinition(1, ChaosCastle1.Number, 25000);
        chaosCastle1.MinimumCharacterLevel = 15;
        chaosCastle1.MaximumCharacterLevel = 49;
        chaosCastle1.MinimumSpecialCharacterLevel = 15;
        chaosCastle1.MaximumSpecialCharacterLevel = 29;

        var chaosCastle2 = this.CreateChaosCastleDefinition(2, ChaosCastle2.Number, 80000);
        chaosCastle2.MinimumCharacterLevel = 50;
        chaosCastle2.MaximumCharacterLevel = 119;
        chaosCastle2.MinimumSpecialCharacterLevel = 30;
        chaosCastle2.MaximumSpecialCharacterLevel = 99;

        var chaosCastle3 = this.CreateChaosCastleDefinition(3, ChaosCastle3.Number, 150000);
        chaosCastle3.MinimumCharacterLevel = 120;
        chaosCastle3.MaximumCharacterLevel = 179;
        chaosCastle3.MinimumSpecialCharacterLevel = 100;
        chaosCastle3.MaximumSpecialCharacterLevel = 159;

        var chaosCastle4 = this.CreateChaosCastleDefinition(4, ChaosCastle4.Number, 250000);
        chaosCastle4.MinimumCharacterLevel = 180;
        chaosCastle4.MaximumCharacterLevel = 239;
        chaosCastle4.MinimumSpecialCharacterLevel = 160;
        chaosCastle4.MaximumSpecialCharacterLevel = 219;

        var chaosCastle5 = this.CreateChaosCastleDefinition(5, ChaosCastle5.Number, 400000);
        chaosCastle5.MinimumCharacterLevel = 240;
        chaosCastle5.MaximumCharacterLevel = 299;
        chaosCastle5.MinimumSpecialCharacterLevel = 220;
        chaosCastle5.MaximumSpecialCharacterLevel = 279;

        var chaosCastle6 = this.CreateChaosCastleDefinition(6, ChaosCastle6.Number, 650000);
        chaosCastle6.MinimumCharacterLevel = 300;
        chaosCastle6.MaximumCharacterLevel = 400;
        chaosCastle6.MinimumSpecialCharacterLevel = 280;
        chaosCastle6.MaximumSpecialCharacterLevel = 400;

        var chaosCastle7 = this.CreateChaosCastleDefinition(7, ChaosCastle7.Number, 1000000);
        chaosCastle7.RequiresMasterClass = true;
        chaosCastle7.MinimumCharacterLevel = 400;
        chaosCastle7.MaximumCharacterLevel = 400;
        chaosCastle7.MinimumSpecialCharacterLevel = 400;
        chaosCastle7.MaximumSpecialCharacterLevel = 400;
    }

    /// <summary>
    /// Creates a new <see cref="MiniGameDefinition" /> for a blood castle event.
    /// </summary>
    /// <param name="level">The level of the event.</param>
    /// <param name="mapNumber">The map number.</param>
    /// <param name="entranceFee">The entrance fee.</param>
    /// <returns>
    /// The created <see cref="MiniGameDefinition" />.
    /// </returns>
    protected MiniGameDefinition CreateChaosCastleDefinition(byte level, short mapNumber, int entranceFee)
    {
        var chaosCastle = this.Context.CreateNew<MiniGameDefinition>();
        chaosCastle.SetGuid((short)MiniGameType.ChaosCastle, level);
        this.GameConfiguration.MiniGameDefinitions.Add(chaosCastle);
        chaosCastle.Name = $"Chaos Castle {level}";
        chaosCastle.Description = $"Event definition for chaos castle event, level {level}.";
        chaosCastle.EnterDuration = TimeSpan.FromMinutes(5);
        chaosCastle.GameDuration = TimeSpan.FromMinutes(10);
        chaosCastle.ExitDuration = TimeSpan.FromMinutes(1);
        chaosCastle.MaximumPlayerCount = 70;
        chaosCastle.Entrance = this.GameConfiguration.Maps.First(m => m.Number == mapNumber).ExitGates.First();
        chaosCastle.Type = MiniGameType.ChaosCastle;
        chaosCastle.TicketItem = this.GameConfiguration.Items.Single(item => item is { Group: 13, Number: 29 });
        chaosCastle.GameLevel = level;
        chaosCastle.MapCreationPolicy = MiniGameMapCreationPolicy.Shared;
        chaosCastle.SaveRankingStatistics = true;
        chaosCastle.EntranceFee = entranceFee;
        chaosCastle.AllowParty = false;

        this.CreateRewards(level, chaosCastle);
        this.CreateEvents(chaosCastle);

        return chaosCastle;
    }

    private void CreateEvents(MiniGameDefinition chaosCastle)
    {
        AddStage(0, 0);
        AddSafezoneToggle(0, 23, 75, 44, 108, false);

        AddStage(1, 60);
        AddStage(2, 10);
        AddStage(3, 10);

        AddTerrainBlocking(1, 0x17, 0x4B, 0x2C, 0x4C);
        AddTerrainBlocking(1, 0x2B, 0x4D, 0x2C, 0x6C);
        AddTerrainBlocking(1, 0x17, 0x6B, 0x2A, 0x6C);
        AddTerrainBlocking(1, 0x17, 0x4D, 0x18, 0x6A);
        AddTerrainBlocking(2, 0x19, 0x4D, 0x2A, 0x4E);
        AddTerrainBlocking(2, 0x29, 0x4F, 0x2A, 0x6A);
        AddTerrainBlocking(2, 0x19, 0x69, 0x28, 0x6A);
        AddTerrainBlocking(2, 0x19, 0x4F, 0x1A, 0x68);
        AddTerrainBlocking(3, 0x1B, 0x4F, 0x28, 0x50);
        AddTerrainBlocking(3, 0x27, 0x51, 0x28, 0x68);
        AddTerrainBlocking(3, 0x1B, 0x67, 0x26, 0x68);
        AddTerrainBlocking(3, 0x1B, 0x51, 0x1C, 0x66);

        void AddStage(int index, short kills)
        {
            var stageChangeEvent = this.Context.CreateNew<MiniGameChangeEvent>();
            chaosCastle.ChangeEvents.Add(stageChangeEvent);
            stageChangeEvent.Index = index;
            stageChangeEvent.Description = "Stage Terrain Change Event";
            stageChangeEvent.NumberOfKills = kills;
            stageChangeEvent.Target = KillTarget.AnyObject;
        }

        void AddTerrainBlocking(int index, byte startX, byte startY, byte endX, byte endY)
            => AddTerrainChange(index, startX, startY, endX, endY, true, TerrainAttributeType.NoGround);

        void AddSafezoneToggle(int index, byte startX, byte startY, byte endX, byte endY, bool makeSafe)
            => AddTerrainChange(index, startX, startY, endX, endY, makeSafe, TerrainAttributeType.Safezone);

        void AddTerrainChange(int index, byte startX, byte startY, byte endX, byte endY, bool setAttribute, TerrainAttributeType attribute)
        {
            var changeEvent = chaosCastle.ChangeEvents.Single(e => e.Index == index);
            var terrainChange = this.Context.CreateNew<MiniGameTerrainChange>();
            changeEvent.TerrainChanges.Add(terrainChange);
            terrainChange.StartX = startX;
            terrainChange.StartY = startY;
            terrainChange.EndX = endX;
            terrainChange.EndY = endY;
            terrainChange.SetTerrainAttribute = setAttribute;
            terrainChange.TerrainAttribute = attribute;

            // Only Safezone-Changes at start and end are required to be sent to the client.
            // The Blockings are automatically done by the client when updating the event state.
            // Unfortunately, these areas are hardcoded at client-side.
            terrainChange.IsClientUpdateRequired = attribute == TerrainAttributeType.Safezone;
        }
    }

    private void CreateRewards(byte level, MiniGameDefinition chaosCastle)
    {
        var jewelDropItemGroup = this.Context.CreateNew<DropItemGroup>();
        jewelDropItemGroup.SetGuid(chaosCastle.Entrance!.Map!.Number, 1, level);
        jewelDropItemGroup.Description = $"Rewarded jewels for Chaos Castle {level}";
        jewelDropItemGroup.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos"));
        jewelDropItemGroup.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Bless"));
        jewelDropItemGroup.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Soul"));
        jewelDropItemGroup.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Creation"));
        jewelDropItemGroup.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Life"));
        jewelDropItemGroup.Chance = 0.9;

        this.GameConfiguration.DropItemGroups.Add(jewelDropItemGroup);

        var jewelReward = this.Context.CreateNew<MiniGameReward>();
        jewelReward.ItemReward = jewelDropItemGroup;
        jewelReward.RewardAmount = 1;
        jewelReward.RewardType = MiniGameRewardType.ItemDrop;
        jewelReward.RequiredSuccess = MiniGameSuccessFlags.Winner | MiniGameSuccessFlags.Alive;
        chaosCastle.Rewards.Add(jewelReward);

        if (level > 1)
        {
            var ancientDropItemGroup = this.Context.CreateNew<DropItemGroup>();
            ancientDropItemGroup.SetGuid(chaosCastle.Entrance!.Map!.Number, 2, level);
            ancientDropItemGroup.Description = $"Rewarded ancient items for Chaos Castle {level}";
            ancientDropItemGroup.ItemType = SpecialItemType.Ancient;
            ancientDropItemGroup.Chance = 0.1 * level;

            this.GameConfiguration.DropItemGroups.Add(ancientDropItemGroup);

            var ancientReward = this.Context.CreateNew<MiniGameReward>();
            ancientReward.ItemReward = ancientDropItemGroup;
            ancientReward.RewardAmount = 1;
            ancientReward.RewardType = MiniGameRewardType.ItemDrop;
            ancientReward.RequiredSuccess = MiniGameSuccessFlags.Winner | MiniGameSuccessFlags.Alive;
            chaosCastle.Rewards.Add(ancientReward);
        }
    }
}