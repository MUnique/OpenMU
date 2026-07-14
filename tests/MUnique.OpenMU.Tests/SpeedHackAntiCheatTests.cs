// <copyright file="SpeedHackAntiCheatTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.GameLogic.PlugIns;
using NUnit.Framework;

/// <summary>
/// Tests for the speed hack anti-cheat detection logic.
/// </summary>
[TestFixture]
public class SpeedHackAntiCheatTests
{
    private static readonly Point StartPoint = new(100, 100);

    /// <summary>
    /// Tests that the speed check detects speed hacks on walk and bans the account after exceeding limits.
    /// </summary>
    [Test]
    public async Task TestWalkSpeedHackDetectionBansAccountAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        Assert.That(player.Account?.State, Is.EqualTo(AccountState.Normal));

        // Perform rapid walks to exceed the 3-warnings limit.
        // We need 12 iterations because speedhack detection uses a rolling history of recent walks
        // which requires 3 walks to trigger the first warning. Since the history is cleared on violation,
        // we need 3 walks * 4 warnings = 12 iterations to trigger account ban.
        for (int i = 0; i < 12; i++)
        {
            var nextFrom = new Point((byte)(StartPoint.X + i * 2), StartPoint.Y);
            var nextTo = new Point((byte)(StartPoint.X + i * 2 + 2), StartPoint.Y);
            
            // Set player position to nextFrom to avoid startOffset validation check rejecting the walk.
            player.Position = nextFrom;

            // Reset the alert debounce to allow consecutive warnings.
            player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()!.SetLastAlertTime(player, DateTime.MinValue);

            WalkingStep[] steps = [new() { From = nextFrom, To = nextTo, Direction = Direction.East }];

            await player.WalkToAsync(nextTo, steps).ConfigureAwait(false);
        }

        // The account should be banned because we sent multiple walk requests triggering 4 warnings
        Assert.That(player.Account?.State, Is.EqualTo(AccountState.Banned));
    }

    /// <summary>
    /// Tests that a large walk start offset triggers rubberbanding and doesn't record a speed hack warning.
    /// </summary>
    [Test]
    public async Task TestWalkStartOffsetRubberbandsPlayerAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        player.Position = StartPoint;

        var startPointTooFar = new Point((byte)(StartPoint.X + 10), StartPoint.Y);
        var targetPoint = new Point((byte)(StartPoint.X + 12), StartPoint.Y);

        WalkingStep[] steps = [new() { From = startPointTooFar, To = targetPoint, Direction = Direction.East }];

        await player.WalkToAsync(targetPoint, steps).ConfigureAwait(false);

        // Verify that no violation was recorded
        Assert.That(player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()!.GetWarningCount(player), Is.EqualTo(0));

        // Verify that the view plugin container for IObjectMovedPlugIn was called to warp the player back
        var objectMovedPlugIn = player.ViewPlugIns.GetPlugIn<IObjectMovedPlugIn>();
        Assert.That(objectMovedPlugIn, Is.Not.Null);
        var objectMovedMock = Mock.Get<IObjectMovedPlugIn>(objectMovedPlugIn!);
        objectMovedMock.Verify(m => m.ObjectMovedAsync(player, MoveType.Instant), Times.Once);
    }

    /// <summary>
    /// Tests that walking in a safe zone does not trigger speed hack warnings/violations.
    /// </summary>
    [Test]
    public async Task TestWalkSpeedHackBypassedInSafezoneAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        Assert.That(player.Account?.State, Is.EqualTo(AccountState.Normal));

        // Mark current map terrain to have safe zone at the positions player walks through
        var mapTerrain = player.CurrentMap!.Terrain;
        for (int x = 0; x < mapTerrain.SafezoneMap.GetLength(0); x++)
        {
            for (int y = 0; y < mapTerrain.SafezoneMap.GetLength(1); y++)
            {
                mapTerrain.SafezoneMap[x, y] = true;
            }
        }

        // Verify that IsAtSafezone returns true
        Assert.That(player.IsAtSafezone(), Is.True);

        // Perform rapid walks to show it is bypassed.
        for (int i = 0; i < 12; i++)
        {
            var nextFrom = new Point((byte)(StartPoint.X + i * 2), StartPoint.Y);
            var nextTo = new Point((byte)(StartPoint.X + i * 2 + 2), StartPoint.Y);
            
            player.Position = nextFrom;
            player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()!.SetLastAlertTime(player, DateTime.MinValue);

            WalkingStep[] steps = [new() { From = nextFrom, To = nextTo, Direction = Direction.East }];

            await player.WalkToAsync(nextTo, steps).ConfigureAwait(false);
        }

        // Account should remain normal
        Assert.That(player.Account?.State, Is.EqualTo(AccountState.Normal));
        Assert.That(player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()!.GetWarningCount(player), Is.EqualTo(0));
    }

    /// <summary>
    /// Tests that attacking too fast triggers attack speed hack detection.
    /// </summary>
    [Test]
    public async Task TestAttackSpeedHackDetectionAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        player.Attributes![Stats.AttackSpeed] = 20;

        var speedCheck = player.GameContext.PlugInManager.GetPlugInPoint<ISpeedHackCheatCheckPlugIn>();
        Assert.That(speedCheck, Is.Not.Null);

        // The first attack sets the timestamp tracker
        var argsFirst = new SpeedHackCheckEventArgs();
        await speedCheck!.AttackCheatCheckAsync(player, argsFirst).ConfigureAwait(false);
        Assert.That(argsFirst.IsCheatDetected, Is.False);

        // Perform rapid attacks in succession (0ms elapsed)
        // With attack speed 20, min expected interval is 1000 - 60 = 940ms.
        // Doing this multiple times should trigger the rolling average detection.
        bool detectedHack = false;
        for (int i = 0; i < 5; i++)
        {
            var args = new SpeedHackCheckEventArgs();
            await speedCheck.AttackCheatCheckAsync(player, args).ConfigureAwait(false);
            if (args.IsCheatDetected)
            {
                detectedHack = true;
                break;
            }
        }

        Assert.That(detectedHack, Is.True);
    }

    /// <summary>
    /// Tests that teleporting/warping to a different coordinate clears recent walks
    /// and doesn't trigger speed hack warnings on subsequent movements.
    /// </summary>
    [Test]
    public async Task TestWarpClearsWalkHistoryAndPreventsFalsePositiveAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        player.Position = StartPoint;

        // Perform a normal walk
        WalkingStep[] steps1 = [new() { From = StartPoint, To = new Point((byte)(StartPoint.X + 1), StartPoint.Y), Direction = Direction.East }];
        await player.WalkToAsync(new Point((byte)(StartPoint.X + 1), StartPoint.Y), steps1).ConfigureAwait(false);

        // Simulate instant warp to different coordinates far away (e.g. via WarpToAsync/PlaceAtGate)
        var gate = new ExitGate
        {
            Map = player.CurrentMap!.Definition,
            X1 = 200,
            X2 = 200,
            Y1 = 200,
            Y2 = 200,
            Direction = Direction.West
        };
        await player.WarpToAsync(gate).ConfigureAwait(false);

        // Perform a walk immediately at the new coordinates
        var newPosition = player.Position;
        WalkingStep[] steps2 = [new() { From = newPosition, To = new Point((byte)(newPosition.X + 1), newPosition.Y), Direction = Direction.East }];
        await player.WalkToAsync(new Point((byte)(newPosition.X + 1), newPosition.Y), steps2).ConfigureAwait(false);

        // Verify that no violation was recorded
        Assert.That(player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()!.GetWarningCount(player), Is.EqualTo(0));
    }

    /// <summary>
    /// Tests that a speedhacker using Cheat Engine (moving extremely fast without waiting for the server's Position to update)
    /// is successfully detected and banned by the speed check.
    /// </summary>
    [Test]
    public async Task TestWalkSpeedHackDetectionWithCheatEngineAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        player.Position = StartPoint;
        Assert.That(player.Account?.State, Is.EqualTo(AccountState.Normal));

        var objectMovedPlugIn = player.ViewPlugIns.GetPlugIn<IObjectMovedPlugIn>();
        var objectMovedMock = Mock.Get(objectMovedPlugIn!);

        Point currentClientPos = StartPoint;
        objectMovedMock.Setup(m => m.ObjectMovedAsync(player, MoveType.Instant))
            .Callback(() => currentClientPos = player.Position);

        // Perform rapid walks to exceed the warnings limit.
        // We run 16 iterations because warnings occur every 3rd step, followed by an offset desync rubberband on the 4th,
        // requiring 16 iterations to reach 4 warnings and trigger account ban.
        for (int i = 0; i < 16; i++)
        {
            var nextFrom = currentClientPos;
            var nextTo = new Point((byte)(nextFrom.X + 2), nextFrom.Y);
            currentClientPos = nextTo;

            // Reset the alert debounce to allow consecutive warnings.
            player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()!.SetLastAlertTime(player, DateTime.MinValue);

            WalkingStep[] steps = [new() { From = nextFrom, To = nextTo, Direction = Direction.East }];

            await player.WalkToAsync(nextTo, steps).ConfigureAwait(false);
        }

        // The account should be banned because we sent rapid walks, even though the server position was out of sync.
        Assert.That(player.Account?.State, Is.EqualTo(AccountState.Banned));
    }

    /// <summary>
    /// Tests that a player equipped with a mount walking with Cheat Engine (rapid walk requests)
    /// is successfully detected and banned.
    /// </summary>
    [Test]
    public async Task TestWalkSpeedHackWithMountDetectedAsync()
    {
        var mountItem = new Item
        {
            ItemSlot = MUnique.OpenMU.DataModel.InventoryConstants.PetSlot,
            Definition = new ItemDefinition
            {
                Group = 13,
                Number = 37, // Fenrir
            }
        };
        var inventoryItems = new List<Item> { mountItem };
        var player = await CreatePlayerWithSpeedAttributesAsync(inventoryItems).ConfigureAwait(false);
        player.Attributes![Stats.MovementSpeed] = 17;
        Assert.That(player.StepDelay.TotalMilliseconds, Is.EqualTo(TimeSpan.FromMilliseconds(4000.0 / 17.0).TotalMilliseconds).Within(1.0));
        Assert.That(player.Account?.State, Is.EqualTo(AccountState.Normal));

        var objectMovedPlugIn = player.ViewPlugIns.GetPlugIn<IObjectMovedPlugIn>();
        var objectMovedMock = Mock.Get(objectMovedPlugIn!);

        Point currentClientPos = StartPoint;
        objectMovedMock.Setup(m => m.ObjectMovedAsync(player, MoveType.Instant))
            .Callback(() => currentClientPos = player.Position);

        // Perform rapid walks to exceed the warnings limit.
        for (int i = 0; i < 16; i++)
        {
            var nextFrom = currentClientPos;
            var nextTo = new Point((byte)(nextFrom.X + 2), nextFrom.Y);
            currentClientPos = nextTo;

            // Reset the alert debounce to allow consecutive warnings.
            player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()!.SetLastAlertTime(player, DateTime.MinValue);

            WalkingStep[] steps = [new() { From = nextFrom, To = nextTo, Direction = Direction.East }];

            await player.WalkToAsync(nextTo, steps).ConfigureAwait(false);
        }

        // The account should be banned because we sent rapid walks.
        Assert.That(player.Account?.State, Is.EqualTo(AccountState.Banned));
    }

    /// <summary>
    /// Tests that a high movement speed character walking at their maximum speed (which resolves
    /// to step delay below the minimum expected delay floor of 50ms) does not trigger false positive bans.
    /// </summary>
    [Test]
    public async Task TestHighMovementSpeedDoesNotTriggerFalsePositivesAsync()
    {
        // Speed attribute is 100, which yields expected step delay: 4000.0 / 100.0 = 40ms.
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        player.Attributes![Stats.MovementSpeed] = 100;
        player.Position = StartPoint;

        // Perform rapid walks spaced according to the expected 40ms step delay.
        // Even though 40ms is below the 50ms minimum limit floor, it should not trigger violations
        // because of our dynamic flooring scaling fix.
        for (int i = 0; i < 20; i++)
        {
            var nextFrom = new Point((byte)(StartPoint.X + i * 2), StartPoint.Y);
            var nextTo = new Point((byte)(StartPoint.X + i * 2 + 2), StartPoint.Y);
            player.Position = nextFrom;
            player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()!.SetLastAlertTime(player, DateTime.MinValue);

            WalkingStep[] steps = [new() { From = nextFrom, To = nextTo, Direction = Direction.East }];

            // Simulate walk packet processing spaced at 80ms interval (40ms expected per tile * 2 tiles).
            await Task.Delay(80).ConfigureAwait(false);
            await player.WalkToAsync(nextTo, steps).ConfigureAwait(false);
        }

        // The account should remain normal.
        Assert.That(player.Account?.State, Is.EqualTo(AccountState.Normal));
        Assert.That(player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()!.GetWarningCount(player), Is.EqualTo(0));
    }

    /// <summary>
    /// Tests that the speed check is bypassed when the plugin is disabled.
    /// </summary>
    [Test]
    public async Task TestBypassedWhenPluginDisabledAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        // Deactivate the plugin
        player.GameContext.PlugInManager.DeactivatePlugIn<SpeedHackDetectPlugIn>();

        Assert.That(player.Account?.State, Is.EqualTo(AccountState.Normal));

        // Rapid walks which would normally ban the player
        for (int i = 0; i < 12; i++)
        {
            var nextFrom = new Point((byte)(StartPoint.X + i * 2), StartPoint.Y);
            var nextTo = new Point((byte)(StartPoint.X + i * 2 + 2), StartPoint.Y);
            
            player.Position = nextFrom;
            player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()?.SetLastAlertTime(player, DateTime.MinValue);

            WalkingStep[] steps = [new() { From = nextFrom, To = nextTo, Direction = Direction.East }];

            await player.WalkToAsync(nextTo, steps).ConfigureAwait(false);
        }

        // The account should remain normal because plugin is disabled
        Assert.That(player.Account?.State, Is.EqualTo(AccountState.Normal));
        Assert.That(player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()?.GetWarningCount(player) ?? 0, Is.EqualTo(0));
    }

    /// <summary>
    /// Tests that customized warning thresholds and ban rules are respected.
    /// </summary>
    [Test]
    public async Task TestCustomizedWarningThresholdAndNoBanAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        var plugin = player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>();
        Assert.That(plugin, Is.Not.Null);
        
        // Customize configuration: 5 warnings limit, no autoban, no disconnect
        plugin!.Configuration!.MaxWarnings = 5;
        plugin.Configuration.AutoBan = false;
        plugin.Configuration.DisconnectOnViolation = false;

        // Perform rapid walks to exceed the original 3 warnings limit, and the new 5 warnings limit.
        // Needs 3 walks per warning, so 3 * 6 warnings = 18 walks to trigger warnings beyond limit.
        for (int i = 0; i < 18; i++)
        {
            var nextFrom = new Point((byte)(StartPoint.X + i * 2), StartPoint.Y);
            var nextTo = new Point((byte)(StartPoint.X + i * 2 + 2), StartPoint.Y);
            
            player.Position = nextFrom;
            player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()!.SetLastAlertTime(player, DateTime.MinValue);

            WalkingStep[] steps = [new() { From = nextFrom, To = nextTo, Direction = Direction.East }];

            await player.WalkToAsync(nextTo, steps).ConfigureAwait(false);
        }

        // The account should still be normal (no autoban) and not disconnected
        Assert.That(player.Account?.State, Is.EqualTo(AccountState.Normal));
        Assert.That(player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()!.GetWarningCount(player), Is.GreaterThan(5));
    }

    private static async ValueTask<Player> CreatePlayerWithSpeedAttributesAsync(List<Item>? inventoryItems = null)
    {
        var gameConfig = new Mock<GameConfiguration>();
        gameConfig.SetupAllProperties();
        gameConfig.Setup(c => c.Maps).Returns(new List<GameMapDefinition>());
        gameConfig.Setup(c => c.Items).Returns(new List<ItemDefinition>());
        gameConfig.Setup(c => c.Skills).Returns(new List<Skill>());
        gameConfig.Setup(c => c.PlugInConfigurations).Returns(new List<PlugInConfiguration>());
        gameConfig.Setup(c => c.CharacterClasses).Returns(new List<CharacterClass>());
        gameConfig.Setup(c => c.GlobalAttributeCombinations).Returns(new List<AttributeRelationship>());
        gameConfig.Setup(c => c.GlobalBaseAttributeValues).Returns(new List<ConstValueAttribute>
        {
            new(1, Stats.MoneyAmountRate),
        });

        var mapDefinition = new Mock<GameMapDefinition>();
        mapDefinition.SetupAllProperties();
        mapDefinition.Setup(m => m.DropItemGroups).Returns(new List<DropItemGroup>());
        mapDefinition.Setup(m => m.MonsterSpawns).Returns(new List<MonsterSpawnArea>());
        mapDefinition.Object.TerrainData = new byte[ushort.MaxValue + 3];
        gameConfig.Object.RecoveryInterval = int.MaxValue;
        gameConfig.Object.Maps.Add(mapDefinition.Object);

        var mapInitializer = new MapInitializer(gameConfig.Object, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var gameContext = new GameContext(gameConfig.Object, new InMemoryPersistenceContextProvider(), mapInitializer, new NullLoggerFactory(), new PlugInManager(null, new NullLoggerFactory(), null, null), NullDropGenerator.Instance, new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = gameContext.PlugInManager;
        mapInitializer.PathFinderPool = gameContext.PathFinderPool;

        var characterMock = new Mock<Character>();
        characterMock.SetupAllProperties();
        characterMock.Setup(c => c.LearnedSkills).Returns(new List<SkillEntry>());
        characterMock.Setup(c => c.Attributes).Returns(new List<StatAttribute>());
        characterMock.Setup(c => c.DropItemGroups).Returns(new List<DropItemGroup>());

        var inventoryMock = new Mock<ItemStorage>();
        inventoryMock.SetupAllProperties();
        inventoryMock.Setup(i => i.Items).Returns(inventoryItems ?? new List<Item>());

        var character = characterMock.Object;
        character.Inventory = inventoryMock.Object;
        character.CurrentMap = gameContext.Configuration.Maps.FirstOrDefault(m => m.Number == 0);

        var characterClassMock = new Mock<CharacterClass>();
        characterClassMock.Setup(c => c.StatAttributes).Returns(
            new List<StatAttributeDefinition>
            {
                new(Stats.Level, 1, false),
                new(Stats.BaseStrength, 28, true),
                new(Stats.BaseAgility, 20, true),
                new(Stats.BaseVitality, 25, true),
                new(Stats.BaseEnergy, 10, true),
                new(Stats.CurrentHealth, 100, false),
                new(Stats.CurrentMana, 100, false),
                new(Stats.CurrentShield, 100, false),
                new(Stats.Resets, 0, false),
                new(Stats.AttackSpeed, 20, false), // Added Stats.AttackSpeed to StatAttributes
                new(Stats.MovementSpeed, 0, false),
                new(Stats.MovementSpeedUnderwater, 0, false),
                new(Stats.MovementSpeedFactor, 1f, false)
            });

        characterClassMock.Setup(c => c.AttributeCombinations).Returns(new List<AttributeRelationship>
        {
            new(Stats.TotalStrength, 1, Stats.BaseStrength),
            new(Stats.TotalAgility, 1, Stats.BaseAgility),
            new(Stats.TotalVitality, 1, Stats.BaseVitality),
            new(Stats.TotalEnergy, 1, Stats.BaseEnergy),
            new(Stats.MaximumMana, 1, Stats.BaseEnergy),
            new(Stats.MaximumHealth, 2, Stats.BaseVitality),
        });

        characterClassMock.Setup(c => c.BaseAttributeValues).Returns(new List<ConstValueAttribute>
        {
            new(10, Stats.MaximumMana),
            new(35, Stats.MaximumHealth),
        });

        character.CharacterClass = characterClassMock.Object;

        foreach (var attributeDef in character.CharacterClass.StatAttributes)
        {
            character.Attributes.Add(new StatAttribute(attributeDef.Attribute!, attributeDef.BaseValue));
        }

        var account = new TestAccount { State = AccountState.Normal };

        var player = new TestPlayer(gameContext) { Account = account };
        var speedHackDetectPlugIn = new SpeedHackDetectPlugIn { Configuration = new SpeedHackDetectConfiguration() };
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IFeaturePlugIn>(speedHackDetectPlugIn);
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<ISpeedHackCheatCheckPlugIn>(speedHackDetectPlugIn);
        player.GameContext.FeaturePlugIns.AddPlugIn(speedHackDetectPlugIn, true);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.CharacterSelection).ConfigureAwait(false);
        await player.SetSelectedCharacterAsync(character).ConfigureAwait(false);

        return player;
    }

    private class TestAccount : Account
    {
        public TestAccount()
        {
            this.Attributes = new List<StatAttribute>();
            this.UnlockedCharacterClasses = new List<CharacterClass>();
            this.Characters = new List<Character>();
        }
    }

    private class TestPlayer : Player
    {
        public TestPlayer(IGameContext gameContext)
            : base(gameContext)
        {
        }

        protected override ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer()
        {
            return new MockViewPlugInContainer();
        }
    }
}
