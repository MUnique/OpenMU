// <copyright file="CastleSiegeInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

/// <summary>
/// Initializes the Castle Siege configuration and persistent state.
/// </summary>
internal sealed class CastleSiegeInitializer : InitializerBase
{
    private const short GateMonsterNumber = 277;
    private const short StatueMonsterNumber = 283;
    private const byte SignOfLordItemGroup = 14;
    private const short SignOfLordItemNumber = 21;
    private const byte SignOfLordItemLevel = 3;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public CastleSiegeInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        var configuration = this.InitializeConfiguration();
        this.InitializeData(configuration);
    }

    /// <summary>
    /// Initializes the Castle Siege configuration, if it does not exist yet.
    /// </summary>
    /// <returns>The Castle Siege configuration.</returns>
    internal CastleSiegeConfiguration InitializeConfiguration()
    {
        if (this.GameConfiguration.CastleSiegeConfiguration is { } existingConfiguration)
        {
            this.InitializeRegistration(existingConfiguration);
            return existingConfiguration;
        }

        var configuration = this.Context.CreateNew<CastleSiegeConfiguration>();
        configuration.Enabled = true;
        configuration.CrownHoldTimeSeconds = 30;
        configuration.RegisterMinLevel = 200;
        configuration.RegisterMinMembers = 20;
        this.InitializeRegistration(configuration);
        configuration.ParticipantRewardMinSeconds = 60;
        configuration.MaxAttackingGuilds = 3;
        configuration.GuildScoreCastleSiege = 0;
        configuration.GuildScoreCastleSiegeMembers = 0;
        configuration.GateBuyPrice = 9_500_000;
        configuration.StatueBuyPrice = 4_500_000;
        configuration.GateRepairCostPerHealthPoint = 5;
        configuration.StatueRepairCostPerHealthPoint = 3;
        configuration.RepairCostPerUpgradeLevel = 1_000_000;
        configuration.CastleSiegeMapDefinition = this.GameConfiguration.Maps.Single(map => map.Number == ValleyOfLoren.Number);
        configuration.LandOfTrialsMapDefinition = this.GameConfiguration.Maps.Single(map => map.Number == LandOfTrials.Number);

        this.InitializeStateSchedule(configuration);
        this.InitializeNpcDefinitions(configuration);
        this.InitializeUpgradeDefinitions(configuration);
        this.InitializeMachineZones(configuration);
        configuration.DefenseRespawnArea = this.CreateZone(74, 144, 115, 154);
        configuration.AttackRespawnArea = this.CreateZone(35, 11, 144, 48);

        this.GameConfiguration.CastleSiegeConfiguration = configuration;
        return configuration;
    }

    /// <summary>
    /// Initializes the item configuration used for Sign of Lord registration.
    /// </summary>
    /// <param name="configuration">The Castle Siege configuration.</param>
    internal void InitializeRegistration(CastleSiegeConfiguration configuration)
    {
        if (configuration.SignOfLordItemDefinition is not null)
        {
            return;
        }

        var itemDefinition = this.GameConfiguration.Items.SingleOrDefault(
            item => item.Group == SignOfLordItemGroup && item.Number == SignOfLordItemNumber);
        if (itemDefinition is null)
        {
            return;
        }

        itemDefinition.MaximumItemLevel = Math.Max(itemDefinition.MaximumItemLevel, SignOfLordItemLevel);
        configuration.SignOfLordItemDefinition = itemDefinition;
        configuration.SignOfLordItemLevel = SignOfLordItemLevel;
    }

    /// <summary>
    /// Initializes the persistent Castle Siege state.
    /// </summary>
    /// <param name="configuration">The Castle Siege configuration.</param>
    /// <returns>The persistent Castle Siege state.</returns>
    internal CastleSiegeData InitializeData(CastleSiegeConfiguration configuration)
    {
        var data = this.Context.CreateNew<CastleSiegeData>();
        data.OwnerGuildId = null;
        data.IsOccupied = false;
        data.TaxChaos = 0;
        data.TaxStore = 0;
        data.TaxHunt = 0;
        data.IsHuntZoneEnabled = false;
        data.TributeMoney = 0;

        var gateHitPoints = configuration.GateLifeUpgrades.Single(upgrade => upgrade.Level == 0).Value;
        var statueHitPoints = configuration.StatueLifeUpgrades.Single(upgrade => upgrade.Level == 0).Value;
        foreach (var npcDefinition in configuration.NpcDefinitions.Where(definition => definition.IsPersistedToDatabase))
        {
            var npcState = this.Context.CreateNew<CastleSiegeNpcState>();
            npcState.MonsterNumber = npcDefinition.MonsterDefinition!.Number;
            npcState.InstanceId = npcDefinition.InstanceId;
            npcState.DefenseLevel = 0;
            npcState.RegenLevel = 0;
            npcState.LifeLevel = 0;
            npcState.CurrentHp = npcState.MonsterNumber switch
            {
                GateMonsterNumber => gateHitPoints,
                StatueMonsterNumber => statueHitPoints,
                _ => throw new InvalidOperationException($"The persisted Castle Siege NPC monster number {npcState.MonsterNumber} is unsupported."),
            };
            data.NpcStates.Add(npcState);
        }

        return data;
    }

    private void InitializeStateSchedule(CastleSiegeConfiguration configuration)
    {
        configuration.StateSchedule.Add(this.CreateScheduleEntry(CastleSiegeState.Idle1, DayOfWeek.Sunday, 0, 0));
        configuration.StateSchedule.Add(this.CreateScheduleEntry(CastleSiegeState.RegisterGuild, DayOfWeek.Monday, 0, 0));
        configuration.StateSchedule.Add(this.CreateScheduleEntry(CastleSiegeState.Idle2, DayOfWeek.Tuesday, 0, 0));
        configuration.StateSchedule.Add(this.CreateScheduleEntry(CastleSiegeState.RegisterMark, DayOfWeek.Wednesday, 0, 0));
        configuration.StateSchedule.Add(this.CreateScheduleEntry(CastleSiegeState.Idle3, DayOfWeek.Thursday, 0, 0));
        configuration.StateSchedule.Add(this.CreateScheduleEntry(CastleSiegeState.Notify, DayOfWeek.Friday, 0, 0));
        configuration.StateSchedule.Add(this.CreateScheduleEntry(CastleSiegeState.Ready, DayOfWeek.Saturday, 18, 0));
        configuration.StateSchedule.Add(this.CreateScheduleEntry(CastleSiegeState.Start, DayOfWeek.Saturday, 20, 0));
        configuration.StateSchedule.Add(this.CreateScheduleEntry(CastleSiegeState.End, DayOfWeek.Saturday, 22, 0));
        configuration.StateSchedule.Add(this.CreateScheduleEntry(CastleSiegeState.EndCycle, DayOfWeek.Saturday, 22, 5));
    }

    private void InitializeNpcDefinitions(CastleSiegeConfiguration configuration)
    {
        this.AddNpc(configuration, 216, 1, false, CastleSiegeJoinSide.Attack1, 176, 212, Direction.SouthWest);
        this.AddNpc(configuration, 217, 1, false, CastleSiegeJoinSide.Attack1, 167, 194, Direction.NorthWest);
        this.AddNpc(configuration, 218, 1, false, CastleSiegeJoinSide.Attack1, 184, 195, Direction.NorthWest);

        this.AddNpc(configuration, 219, 1, false, CastleSiegeJoinSide.Defense, 93, 208, Direction.SouthWest);
        this.AddNpc(configuration, 219, 2, false, CastleSiegeJoinSide.Defense, 81, 165, Direction.SouthWest);
        this.AddNpc(configuration, 219, 3, false, CastleSiegeJoinSide.Defense, 107, 165, Direction.SouthWest);
        this.AddNpc(configuration, 219, 4, false, CastleSiegeJoinSide.Defense, 67, 118, Direction.SouthWest);
        this.AddNpc(configuration, 219, 5, false, CastleSiegeJoinSide.Defense, 93, 118, Direction.SouthWest);
        this.AddNpc(configuration, 219, 6, false, CastleSiegeJoinSide.Defense, 119, 118, Direction.SouthWest);

        this.AddNpc(configuration, 221, 1, false, CastleSiegeJoinSide.Attack1, 63, 19, Direction.NorthEast);
        this.AddNpc(configuration, 221, 2, false, CastleSiegeJoinSide.Attack1, 119, 19, Direction.NorthEast);
        this.AddNpc(configuration, 222, 1, false, CastleSiegeJoinSide.Defense, 80, 188, Direction.SouthWest);
        this.AddNpc(configuration, 222, 2, false, CastleSiegeJoinSide.Defense, 105, 188, Direction.SouthWest);

        this.AddNpc(configuration, GateMonsterNumber, 1, true, CastleSiegeJoinSide.Defense, 93, 204, Direction.SouthWest);
        this.AddNpc(configuration, GateMonsterNumber, 2, true, CastleSiegeJoinSide.Defense, 81, 161, Direction.SouthWest);
        this.AddNpc(configuration, GateMonsterNumber, 3, true, CastleSiegeJoinSide.Defense, 107, 161, Direction.SouthWest);
        this.AddNpc(configuration, GateMonsterNumber, 4, true, CastleSiegeJoinSide.Defense, 67, 114, Direction.SouthWest);
        this.AddNpc(configuration, GateMonsterNumber, 5, true, CastleSiegeJoinSide.Defense, 93, 114, Direction.SouthWest);
        this.AddNpc(configuration, GateMonsterNumber, 6, true, CastleSiegeJoinSide.Defense, 119, 114, Direction.SouthWest);

        this.AddNpc(configuration, StatueMonsterNumber, 1, true, CastleSiegeJoinSide.Defense, 94, 227, Direction.SouthWest);
        this.AddNpc(configuration, StatueMonsterNumber, 2, true, CastleSiegeJoinSide.Defense, 94, 182, Direction.SouthWest);
        this.AddNpc(configuration, StatueMonsterNumber, 3, true, CastleSiegeJoinSide.Defense, 82, 130, Direction.SouthWest);
        this.AddNpc(configuration, StatueMonsterNumber, 4, true, CastleSiegeJoinSide.Defense, 107, 130, Direction.SouthWest);
    }

    private void InitializeUpgradeDefinitions(CastleSiegeConfiguration configuration)
    {
        this.AddUpgrade(configuration.GateDefenseUpgrades, 0, 0, 0, 100);
        this.AddUpgrade(configuration.GateDefenseUpgrades, 1, 2, 3_000_000, 180);
        this.AddUpgrade(configuration.GateDefenseUpgrades, 2, 3, 3_000_000, 300);
        this.AddUpgrade(configuration.GateDefenseUpgrades, 3, 4, 3_000_000, 520);

        this.AddUpgrade(configuration.StatueDefenseUpgrades, 0, 0, 0, 80);
        this.AddUpgrade(configuration.StatueDefenseUpgrades, 1, 3, 3_000_000, 180);
        this.AddUpgrade(configuration.StatueDefenseUpgrades, 2, 5, 3_000_000, 340);
        this.AddUpgrade(configuration.StatueDefenseUpgrades, 3, 7, 3_000_000, 550);

        this.AddUpgrade(configuration.GateLifeUpgrades, 0, 0, 0, 1_900_000);
        this.AddUpgrade(configuration.GateLifeUpgrades, 1, 2, 1_000_000, 2_500_000);
        this.AddUpgrade(configuration.GateLifeUpgrades, 2, 3, 1_000_000, 3_500_000);
        this.AddUpgrade(configuration.GateLifeUpgrades, 3, 4, 1_000_000, 5_200_000);

        this.AddUpgrade(configuration.StatueLifeUpgrades, 0, 0, 0, 1_500_000);
        this.AddUpgrade(configuration.StatueLifeUpgrades, 1, 3, 1_000_000, 2_200_000);
        this.AddUpgrade(configuration.StatueLifeUpgrades, 2, 5, 1_000_000, 3_400_000);
        this.AddUpgrade(configuration.StatueLifeUpgrades, 3, 7, 1_000_000, 5_000_000);

        this.AddUpgrade(configuration.StatueRegenUpgrades, 0, 0, 0, 0);
        this.AddUpgrade(configuration.StatueRegenUpgrades, 1, 3, 5_000_000, 1);
        this.AddUpgrade(configuration.StatueRegenUpgrades, 2, 5, 5_000_000, 2);
        this.AddUpgrade(configuration.StatueRegenUpgrades, 3, 7, 5_000_000, 3);
    }

    private void InitializeMachineZones(CastleSiegeConfiguration configuration)
    {
        configuration.AttackMachineZones.Add(this.CreateZone(62, 103, 72, 112));
        configuration.AttackMachineZones.Add(this.CreateZone(88, 104, 124, 111));
        configuration.AttackMachineZones.Add(this.CreateZone(116, 105, 124, 112));
        configuration.AttackMachineZones.Add(this.CreateZone(73, 86, 105, 103));

        configuration.DefenseMachineZones.Add(this.CreateZone(61, 88, 93, 108));
        configuration.DefenseMachineZones.Add(this.CreateZone(92, 89, 127, 111));
        configuration.DefenseMachineZones.Add(this.CreateZone(84, 52, 102, 66));
    }

    private CastleSiegeStateScheduleEntry CreateScheduleEntry(CastleSiegeState state, DayOfWeek dayOfWeek, byte hour, byte minute)
    {
        var entry = this.Context.CreateNew<CastleSiegeStateScheduleEntry>();
        entry.State = state;
        entry.DayOfWeek = dayOfWeek;
        entry.Hour = hour;
        entry.Minute = minute;
        return entry;
    }

    private void AddNpc(
        CastleSiegeConfiguration configuration,
        short monsterNumber,
        byte instanceId,
        bool isPersisted,
        CastleSiegeJoinSide defaultSide,
        byte spawnX,
        byte spawnY,
        Direction direction)
    {
        var definition = this.Context.CreateNew<CastleSiegeNpcDefinition>();
        definition.MonsterDefinition = this.GameConfiguration.Monsters.Single(monster => monster.Number == monsterNumber);
        definition.InstanceId = instanceId;
        definition.IsPersistedToDatabase = isPersisted;
        definition.DefaultSide = defaultSide;
        definition.SpawnX = spawnX;
        definition.SpawnY = spawnY;
        definition.Direction = direction;
        configuration.NpcDefinitions.Add(definition);
    }

    private void AddUpgrade(
        ICollection<CastleSiegeUpgradeDefinition> target,
        byte level,
        int jewelCount,
        int zen,
        int value)
    {
        var upgrade = this.Context.CreateNew<CastleSiegeUpgradeDefinition>();
        upgrade.Level = level;
        upgrade.RequiredJewelOfGuardianCount = jewelCount;
        upgrade.RequiredZen = zen;
        upgrade.Value = value;
        target.Add(upgrade);
    }

    private CastleSiegeZoneDefinition CreateZone(byte x1, byte y1, byte x2, byte y2)
    {
        var zone = this.Context.CreateNew<CastleSiegeZoneDefinition>();
        zone.X1 = x1;
        zone.Y1 = y1;
        zone.X2 = x2;
        zone.Y2 = y2;
        return zone;
    }
}
