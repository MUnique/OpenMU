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
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.GameServer.MessageHandler;
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
    /// <summary>
    /// The time between two walk packets of a player who clicks frequently, in milliseconds.
    /// </summary>
    private const int WalkPacketIntervalMs = 150;

    /// <summary>
    /// A violation weighs at least 1, so with this threshold the first walk violation is a rejection.
    /// </summary>
    private const double RejectOnFirstViolationThreshold = 0.5;

    private static readonly Point StartPoint = new(100, 100);

    /// <summary>
    /// Tests that the speed check detects speed hacks on walk and bans the account after exceeding limits.
    /// </summary>
    [Test]
    public async Task TestWalkSpeedHackDetectionBansAccountAsync()
    {
        var (player, plugin, timeProvider) = await CreateStrictWalkPlayerAsync().ConfigureAwait(false);
        Assert.That(player.Account?.State, Is.EqualTo(AccountState.Normal));

        // Each executed step covers far more distance than the one second window allows, so every step is
        // its own violation. The debounce is reset to count each one.
        await RecordRapidStepsAsync(player, plugin, timeProvider, 12, 8, TimeSpan.FromMilliseconds(1100)).ConfigureAwait(false);

        // The account should be banned because the executed steps triggered more than 3 warnings.
        Assert.That(player.Account?.State, Is.EqualTo(AccountState.Banned));
    }

    /// <summary>
    /// Tests that a large walk start offset is rejected: the server keeps its position and pulls the
    /// client back, so a modified client can't teleport.
    /// </summary>
    [Test]
    public async Task TestWalkStartOffsetBeyondBoundIsRejectedAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        player.Position = StartPoint;

        var startPointTooFar = new Point((byte)(StartPoint.X + 10), StartPoint.Y);
        var targetPoint = new Point((byte)(StartPoint.X + 12), StartPoint.Y);

        WalkingStep[] steps = [new() { From = startPointTooFar, To = targetPoint, Direction = Direction.East }];

        await player.WalkToAsync(targetPoint, steps).ConfigureAwait(false);

        Assert.That(player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()!.GetWarningCount(player), Is.EqualTo(1));
        var objectMovedPlugIn = player.ViewPlugIns.GetPlugIn<IObjectMovedPlugIn>();
        Assert.That(objectMovedPlugIn, Is.Not.Null);
        Mock.Get<IObjectMovedPlugIn>(objectMovedPlugIn!).Verify(m => m.ObjectMovedAsync(player, MoveType.Instant), Times.Once);
        Assert.Multiple(() =>
        {
            Assert.That(player.Position, Is.EqualTo(StartPoint));
            Assert.That(player.IsWalking, Is.False);
        });
    }

    /// <summary>
    /// Tests that a client-reported position on blocked terrain is not adopted by the server.
    /// </summary>
    [Test]
    public async Task TestWalkStartOnBlockedTerrainDoesNotSynchronizeServerAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()!.Configuration!.MaxAllowedWalkStartOffset = 20;
        player.Position = StartPoint;

        var blockedStart = new Point((byte)(StartPoint.X + 10), StartPoint.Y);
        var targetPoint = new Point((byte)(blockedStart.X + 1), blockedStart.Y);
        player.CurrentMap!.Terrain.WalkMap[blockedStart.X, blockedStart.Y] = false;

        WalkingStep[] steps = [new() { From = blockedStart, To = targetPoint, Direction = Direction.East }];

        await player.WalkToAsync(targetPoint, steps).ConfigureAwait(false);

        Assert.That(player.Position, Is.EqualTo(StartPoint));
        Assert.That(player.IsWalking, Is.False);
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
        await PerformRapidWalkRequestsAsync(player).ConfigureAwait(false);

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

        var speedCheck = player.GameContext.PlugInManager.GetPlugInPoint<IAttackSpeedCheckPlugIn>();
        Assert.That(speedCheck, Is.Not.Null);

        // The first attack sets the timestamp tracker
        var argsFirst = new SpeedHackCheckEventArgs();
        await speedCheck!.AttackCheatCheckAsync(player, null, argsFirst).ConfigureAwait(false);
        Assert.That(argsFirst.IsCheatDetected, Is.False);

        // Perform rapid attacks in succession (0ms elapsed)
        // With attack speed 20, min expected interval is 450 - (20 * 1.2) = 426ms.
        // Doing this multiple times should trigger the rolling average detection.
        bool detectedHack = false;
        for (int i = 0; i < 5; i++)
        {
            var args = new SpeedHackCheckEventArgs();
            await speedCheck.AttackCheatCheckAsync(player, null, args).ConfigureAwait(false);
            if (args.IsCheatDetected)
            {
                detectedHack = true;
                break;
            }
        }

        Assert.That(detectedHack, Is.True);
    }

    /// <summary>
    /// Tests that area skills with explicit hits are rate-limited on their attack packet,
    /// even though their damage is applied later by the hit packets.
    /// </summary>
    [Test]
    public async Task TestExplicitAreaSkillAttackIsRateLimitedAsync()
    {
        var (player, plugin, _) = await CreatePlayerWithManualClockAsync().ConfigureAwait(false);
        var skill = await SetupExplicitAreaSkillAsync(player, range: 5).ConfigureAwait(false);

        player.Attributes![Stats.AttackSpeed] = 20;
        plugin.Configuration!.MaxAttackTokens = 1;

        Assert.That(player.SkillList?.GetSkill((ushort)skill.Number), Is.Not.Null);
        Assert.That(player.IsAtSafezone(), Is.False);

        var action = new AreaSkillAttackAction();
        await action.AttackAsync(player, 0, (ushort)skill.Number, player.Position, 0).ConfigureAwait(false);
        await action.AttackAsync(player, 0, (ushort)skill.Number, player.Position, 0).ConfigureAwait(false);

        Assert.That(plugin.GetWarningCount(player), Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that a speed-rejected area skill attack doesn't consume mana.
    /// </summary>
    [Test]
    public async Task TestSpeedRejectedAreaSkillDoesNotConsumeManaAsync()
    {
        var (player, plugin, _) = await CreatePlayerWithManualClockAsync().ConfigureAwait(false);
        var skill = await SetupExplicitAreaSkillAsync(player, range: 5, consumeRequirements: [new() { Attribute = Stats.CurrentMana, MinimumValue = 10 }]).ConfigureAwait(false);

        player.Attributes![Stats.CurrentMana] = 100;
        player.Attributes[Stats.AttackSpeed] = 20;
        plugin.Configuration!.MaxAttackTokens = 1;

        var action = new AreaSkillAttackAction();
        await action.AttackAsync(player, 0, (ushort)skill.Number, player.Position, 0).ConfigureAwait(false);
        var manaAfterFirstAttack = player.Attributes[Stats.CurrentMana];
        await action.AttackAsync(player, 0, (ushort)skill.Number, player.Position, 0).ConfigureAwait(false);

        Assert.That(manaAfterFirstAttack, Is.EqualTo(90));
        Assert.That(player.Attributes[Stats.CurrentMana], Is.EqualTo(manaAfterFirstAttack));
        Assert.That(plugin.GetWarningCount(player), Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that a truncated area skill hit packet is ignored instead of throwing.
    /// </summary>
    [Test]
    public async Task TestTruncatedAreaSkillHitPacketIsIgnoredAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        await SetupExplicitAreaSkillAsync(player).ConfigureAwait(false);

        var packet = new byte[11];
        packet[3] = 0;
        packet[4] = 1;
        packet[8] = 1;

        var handler = new AreaSkillHitHandlerPlugIn();
        Assert.DoesNotThrowAsync(async () => await handler.HandlePacketAsync(player, packet).ConfigureAwait(false));
    }

    /// <summary>
    /// Tests that a delayed explicit area hit doesn't damage anything when the attacker entered a safezone
    /// after casting outside of it. The recorded hit area alone must not be enough.
    /// </summary>
    [Test]
    public async Task TestDelayedAreaSkillHitIsIgnoredAfterAttackerEntersSafezoneAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        var skill = await SetupExplicitAreaSkillAsync(player, range: 5).ConfigureAwait(false);
        player.Position = StartPoint;

        var targetPosition = new Point((byte)(StartPoint.X + 2), StartPoint.Y);
        var target = new Mock<IAttackable>();
        target.SetupGet(t => t.IsAlive).Returns(true);
        target.SetupGet(t => t.CurrentMap).Returns(player.CurrentMap);
        target.SetupGet(t => t.Position).Returns(targetPosition);

        // The cast happens outside of the safezone and records the hit area.
        await new AreaSkillAttackAction().AttackAsync(player, 0, (ushort)skill.Number, player.Position, 0).ConfigureAwait(false);
        Assert.That(player.SkillHitValidator.IsTargetInArea((ushort)skill.Number, targetPosition), Is.True, "The cast did not record its area.");

        var skillEntry = player.SkillList!.GetSkill((ushort)skill.Number)!;
        var hitAction = new AreaSkillHitAction();
        await hitAction.AttackTargetAsync(player, target.Object, skillEntry).ConfigureAwait(false);
        target.Verify(t => t.AttackByAsync(It.IsAny<IAttacker>(), It.IsAny<SkillEntry?>(), It.IsAny<bool>(), It.IsAny<double>(), It.IsAny<bool?>()), Times.Once);

        // The attacker enters the safezone, and the delayed hit arrives afterwards.
        player.CurrentMap!.Terrain.SafezoneMap[StartPoint.X, StartPoint.Y] = true;
        Assert.That(player.IsAtSafezone(), Is.True);
        await hitAction.AttackTargetAsync(player, target.Object, skillEntry).ConfigureAwait(false);

        target.Verify(t => t.AttackByAsync(It.IsAny<IAttacker>(), It.IsAny<SkillEntry?>(), It.IsAny<bool>(), It.IsAny<double>(), It.IsAny<bool?>()), Times.Once);
    }

    /// <summary>
    /// Tests that attack validation rejects a stunned player.
    /// </summary>
    [Test]
    public async Task TestAttackValidationRejectsStunnedPlayerAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        player.Attributes!.AddElement(new SimpleElement(1, AggregateType.AddRaw), Stats.IsStunned);

        Assert.That(await AttackActionValidator.CanAttackAsync(player, new Skill()), Is.False);
    }

    /// <summary>
    /// Tests that attack validation rejects a sleeping player.
    /// </summary>
    [Test]
    public async Task TestAttackValidationRejectsSleepingPlayerAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        player.Attributes!.AddElement(new SimpleElement(1, AggregateType.AddRaw), Stats.IsAsleep);

        Assert.That(await AttackActionValidator.CanAttackAsync(player, new Skill()), Is.False);
    }

    /// <summary>
    /// Tests that attack validation rejects a player in the safezone.
    /// </summary>
    [Test]
    public async Task TestAttackValidationRejectsSafezoneAttackAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        var terrain = player.CurrentMap!.Terrain;
        for (int x = 0; x < terrain.SafezoneMap.GetLength(0); x++)
        {
            for (int y = 0; y < terrain.SafezoneMap.GetLength(1); y++)
            {
                terrain.SafezoneMap[x, y] = true;
            }
        }

        Assert.That(await AttackActionValidator.CanAttackAsync(player, new Skill()), Is.False);
    }

    /// <summary>
    /// Tests that attack validation requires a weapon for skills which move the attacker to the target.
    /// </summary>
    [Test]
    public async Task TestAttackValidationRequiresWeaponForMovingSkillAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        var skill = new Skill { MovesToTarget = true };

        Assert.That(await AttackActionValidator.CanAttackAsync(player, skill), Is.False);
    }

    /// <summary>
    /// Tests that attack validation requires enough ammunition.
    /// </summary>
    [Test]
    public async Task TestAttackValidationRequiresAmmunitionAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        player.Attributes!.AddElement(new SimpleElement(2, AggregateType.AddRaw), Stats.AmmunitionConsumptionRate);
        player.Attributes.AddElement(new SimpleElement(1, AggregateType.AddRaw), Stats.AmmunitionAmount);
        var skill = new Skill { Number = 1 };

        Assert.That(await AttackActionValidator.CanAttackAsync(player, skill), Is.False);
    }

    /// <summary>
    /// Tests that attack validation allows a regular attack with sufficient state.
    /// </summary>
    [Test]
    public async Task TestAttackValidationAllowsRegularAttackAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        var skill = new Skill { Number = 1 };

        Assert.That(await AttackActionValidator.CanAttackAsync(player, skill), Is.True);
    }

    /// <summary>
    /// Tests that wizardry skills are limited by the magic speed instead of the attack speed.
    /// </summary>
    [Test]
    public async Task TestMagicDamageSkillUsesMagicSpeedAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        player.Attributes![Stats.AttackSpeed] = 200;
        player.Attributes[Stats.MagicSpeed] = 100;

        var config = new SpeedHackDetectConfiguration
        {
            AttackSpeedBaseDelayMs = 450,
            AttackSpeedScalingFactor = 1.2,
            AttackSpeedMinIntervalMs = 60,
            MagicSpeedBaseDelayMs = 900,
            MagicSpeedScalingFactor = 2.0,
            MagicSpeedMinIntervalMs = 100,
        };
        var skill = new Skill { Number = 1, DamageType = DamageType.Wizardry };

        var limit = AttackSpeedModel.Resolve(config, player, skill);

        Assert.That(limit.IntervalMs, Is.EqualTo(700).Within(0.001));
        Assert.That(limit.SpeedSource, Is.EqualTo(AttackSpeedSource.MagicSpeed));
        Assert.That(limit.Speed, Is.EqualTo(100).Within(0.001));
    }

    /// <summary>
    /// Tests that a per-skill profile replaces the common attack speed settings.
    /// </summary>
    [Test]
    public async Task TestPerSkillProfileOverridesCommonSettingsAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        player.Attributes![Stats.AttackSpeed] = 200;

        var config = new SpeedHackDetectConfiguration
        {
            AttackSpeedProfiles = new List<AttackSpeedProfile>
            {
                new()
                {
                    SkillId = 42,
                    SpeedSource = AttackSpeedSource.MagicSpeed,
                    BaseDelayMs = 1000,
                    ScalingFactor = 0,
                    MinimumIntervalMs = 100,
                },
            },
        };
        var skill = new Skill { Number = 42, DamageType = DamageType.Physical };

        var limit = AttackSpeedModel.Resolve(config, player, skill);

        Assert.That(limit.IntervalMs, Is.EqualTo(1000));
        Assert.That(limit.SpeedSource, Is.EqualTo(AttackSpeedSource.MagicSpeed));
    }

    /// <summary>
    /// Tests that the per-skill profiles survive the JSON serialization of the plugin configuration.
    /// </summary>
    [Test]
    public void TestAttackSpeedProfilesRoundTrip()
    {
        var config = new SpeedHackDetectConfiguration
        {
            AttackSpeedProfiles = new List<AttackSpeedProfile>
            {
                new()
                {
                    SkillId = 42,
                    SpeedSource = AttackSpeedSource.MagicSpeed,
                    BaseDelayMs = 1000,
                    ScalingFactor = 0,
                    MinimumIntervalMs = 100,
                },
            },
        };
        var plugInConfiguration = new PlugInConfiguration();
        plugInConfiguration.SetConfiguration(config, null);

        var restored = plugInConfiguration.GetConfiguration<SpeedHackDetectConfiguration>(null);

        Assert.That(restored, Is.Not.Null);
        Assert.That(restored!.AttackSpeedProfiles, Has.Count.EqualTo(1));
        var profile = restored.AttackSpeedProfiles.Single();
        Assert.That(profile.SkillId, Is.EqualTo(42));
        Assert.That(profile.SpeedSource, Is.EqualTo(AttackSpeedSource.MagicSpeed));
        Assert.That(profile.BaseDelayMs, Is.EqualTo(1000));
        Assert.That(profile.ScalingFactor, Is.EqualTo(0));
        Assert.That(profile.MinimumIntervalMs, Is.EqualTo(100));
    }

    /// <summary>
    /// Tests that the attack check resolves the speed source from the used skill.
    /// </summary>
    [Test]
    public async Task TestAttackCheckUsesMagicSpeedForWizardrySkillAsync()
    {
        var (player, plugin, timeProvider) = await CreatePlayerWithManualClockAsync().ConfigureAwait(false);
        player.Attributes![Stats.AttackSpeed] = 200;
        player.Attributes[Stats.MagicSpeed] = 0;

        plugin.Configuration!.MaxAttackTokens = 1;

        var skill = new Skill { Number = 7, DamageType = DamageType.Wizardry };
        await plugin.AttackCheatCheckAsync(player, skill, new SpeedHackCheckEventArgs()).ConfigureAwait(false);

        // 300 ms would be enough for the attack speed interval (210 ms), but not for the magic speed interval (450 ms).
        timeProvider.Advance(TimeSpan.FromMilliseconds(300));
        var args = new SpeedHackCheckEventArgs();
        await plugin.AttackCheatCheckAsync(player, skill, args).ConfigureAwait(false);

        Assert.That(args.IsCheatDetected, Is.True);
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
        var (player, plugin, timeProvider) = await CreateStrictWalkPlayerAsync().ConfigureAwait(false);
        player.Position = StartPoint;
        Assert.That(player.Account?.State, Is.EqualTo(AccountState.Normal));

        await RecordRapidStepsAsync(player, plugin, timeProvider, 16, 8, TimeSpan.FromMilliseconds(1100)).ConfigureAwait(false);
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
        var (player, plugin, timeProvider) = await CreateStrictWalkPlayerAsync(inventoryItems).ConfigureAwait(false);
        player.Attributes![Stats.MovementSpeed] = 17;
        Assert.That(player.StepDelay.TotalMilliseconds, Is.EqualTo(TimeSpan.FromMilliseconds(4000.0 / 17.0).TotalMilliseconds).Within(1.0));
        Assert.That(player.Account?.State, Is.EqualTo(AccountState.Normal));

        await RecordRapidStepsAsync(player, plugin, timeProvider, 16, 8, TimeSpan.FromMilliseconds(1100)).ConfigureAwait(false);
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
        var (player, plugin, timeProvider) = await CreatePlayerWithManualClockAsync().ConfigureAwait(false);
        player.Attributes![Stats.MovementSpeed] = 100;
        player.Position = StartPoint;

        // Perform executed steps spaced according to the expected 40ms step delay.
        // Even though 40ms is below the 50ms minimum limit floor, it should not trigger violations
        // because the expected time is floored to the actual step delay.
        await RecordRapidStepsAsync(player, plugin, timeProvider, 20, 2, TimeSpan.FromMilliseconds(80)).ConfigureAwait(false);

        // The account should remain normal.
        Assert.That(player.Account?.State, Is.EqualTo(AccountState.Normal));
        Assert.That(plugin.GetWarningCount(player), Is.EqualTo(0));
    }

    /// <summary>
    /// Tests that walk packets which arrive in a burst after a network stall don't trigger violations (issue #1119).
    /// </summary>
    [Test]
    public async Task TestWalkPacketBurstAfterStallDoesNotTriggerFalsePositivesAsync()
    {
        var (player, plugin, timeProvider) = await CreatePlayerWithManualClockAsync().ConfigureAwait(false);
        plugin.Configuration!.WalkSpeedWindowSeconds = 1;
        var stepDelay = player.StepDelay;

        for (int i = 0; i < 10; i++)
        {
            var nextFrom = new Point((byte)(StartPoint.X + i), StartPoint.Y);
            var nextTo = new Point((byte)(StartPoint.X + i + 1), StartPoint.Y);

            if (i < 3)
            {
                // Walking normally at first.
                timeProvider.Advance(stepDelay);
            }
            else if (i == 3)
            {
                // The connection stalls, then all following walk steps are executed at once.
                timeProvider.Advance(stepDelay * 7);
            }
            else
            {
                timeProvider.Advance(stepDelay);
            }

            plugin.ResetAlertDebounce(player);
            await RecordStepAndCheckAsync(player, plugin, nextFrom, nextTo).ConfigureAwait(false);
        }

        Assert.That(plugin.GetWarningCount(player), Is.EqualTo(0));
        Assert.That(player.Account?.State, Is.EqualTo(AccountState.Normal));
    }

    /// <summary>
    /// Tests that a violation restarts the walk window. The excess which caused it must not flag every
    /// following walk of the same burst again until it leaves the window; that would turn one incident
    /// into a chain of violations and score.
    /// </summary>
    [Test]
    public async Task TestWalkViolationRestartsWindowAsync()
    {
        var (player, plugin, _) = await CreateStrictWalkPlayerAsync().ConfigureAwait(false);
        player.Position = StartPoint;

        var seedFrom = StartPoint;
        var seedTo = new Point((byte)(StartPoint.X + 1), StartPoint.Y);
        RecordStep(player, plugin, seedFrom, seedTo);

        // 4 more tiles take more than the 1 second window allows, so this step is a violation.
        var firstFrom = seedTo;
        var firstTo = new Point((byte)(firstFrom.X + 4), firstFrom.Y);
        var firstSeverity = await RecordStepAndCheckAsync(player, plugin, firstFrom, firstTo).ConfigureAwait(false);

        // One more tile right after it fits into the restarted window.
        var secondFrom = firstTo;
        var secondTo = new Point((byte)(secondFrom.X + 1), secondFrom.Y);
        var secondSeverity = await RecordStepAndCheckAsync(player, plugin, secondFrom, secondTo).ConfigureAwait(false);

        Assert.That(firstSeverity, Is.EqualTo(SpeedHackViolationSeverity.Soft));
        Assert.That(secondSeverity, Is.EqualTo(SpeedHackViolationSeverity.None));
        Assert.That(plugin.GetWarningCount(player), Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that walking in circles with frequent clicks is not flagged. Only the steps which the
    /// server actually executed are counted, so a walk request which runs ahead of the character
    /// doesn't inflate the measured distance.
    /// </summary>
    [Test]
    public async Task TestWalkingInCirclesWithFrequentClicksIsNotFlaggedAsync()
    {
        var (player, plugin, timeProvider) = await CreatePlayerWithManualClockAsync(new DateTime(2026, 10, 22, 0, 0, 0, DateTimeKind.Utc)).ConfigureAwait(false);

        var severities = await WalkInCirclesAsync(player, plugin, timeProvider, speedFactor: 1.0, duration: TimeSpan.FromMinutes(2)).ConfigureAwait(false);

        Assert.That(severities, Is.All.EqualTo(SpeedHackViolationSeverity.None));
        Assert.That(plugin.GetWarningCount(player), Is.EqualTo(0));
        Assert.That(plugin.GetViolationScore(player), Is.EqualTo(0));
    }

    /// <summary>
    /// Tests that walking at twice the allowed pace is detected with the same frequent clicks,
    /// within a time which is a small multiple of the window.
    /// </summary>
    [Test]
    public async Task TestWalkingInCirclesAtDoubleSpeedIsDetectedAsync()
    {
        var (player, plugin, timeProvider) = await CreatePlayerWithManualClockAsync(new DateTime(2026, 10, 22, 0, 0, 0, DateTimeKind.Utc)).ConfigureAwait(false);

        var severities = await WalkInCirclesAsync(player, plugin, timeProvider, speedFactor: 2.0, duration: TimeSpan.FromSeconds(30)).ConfigureAwait(false);

        var firstDetection = severities.FindIndex(severity => severity is not SpeedHackViolationSeverity.None);
        Assert.That(firstDetection, Is.GreaterThanOrEqualTo(0), "The double speed was not detected.");
        Assert.That(firstDetection * WalkPacketIntervalMs, Is.LessThan(20_000), "The double speed was detected too late.");
    }

    /// <summary>
    /// Tests that time without walking doesn't bank walking time beyond the window:
    /// a speed hack which starts after a long idle time is detected as quickly as any other.
    /// </summary>
    [Test]
    public async Task TestIdleTimeDoesNotBankWalkingTimeAsync()
    {
        var (player, plugin, timeProvider) = await CreatePlayerWithManualClockAsync(new DateTime(2026, 10, 22, 0, 0, 0, DateTimeKind.Utc)).ConfigureAwait(false);
        player.Position = StartPoint;

        RecordStep(player, plugin, StartPoint, new Point((byte)(StartPoint.X + 1), StartPoint.Y));
        timeProvider.Advance(TimeSpan.FromMinutes(10));

        var severities = await WalkInCirclesAsync(player, plugin, timeProvider, speedFactor: 3.0, duration: TimeSpan.FromSeconds(30)).ConfigureAwait(false);

        Assert.That(severities.Any(severity => severity is not SpeedHackViolationSeverity.None), Is.True, "The speed hack after the idle time was not detected.");
    }

    /// <summary>
    /// Tests that a wall-clock adjustment does not affect the walk budget, because
    /// the check has to use a monotonic clock for its intervals.
    /// </summary>
    [Test]
    public async Task TestWalkWallClockJumpDoesNotTriggerViolationAsync()
    {
        var (player, plugin, timeProvider) = await CreatePlayerWithManualClockAsync().ConfigureAwait(false);
        plugin.Configuration!.WalkSpeedToleranceMs = 900;
        player.Position = StartPoint;

        var from = StartPoint;
        var seedTo = new Point((byte)(StartPoint.X + 1), StartPoint.Y);
        RecordStep(player, plugin, from, seedTo);

        timeProvider.Advance(TimeSpan.FromSeconds(1));
        timeProvider.JumpUtc(TimeSpan.FromHours(-2));

        var nextTo = new Point((byte)(seedTo.X + 1), seedTo.Y);
        var severity = await RecordStepAndCheckAsync(player, plugin, seedTo, nextTo).ConfigureAwait(false);

        Assert.That(severity, Is.EqualTo(SpeedHackViolationSeverity.None));
        Assert.That(plugin.GetWarningCount(player), Is.EqualTo(0));
    }

    /// <summary>
    /// Tests that a wall-clock adjustment does not drain the attack token bucket,
    /// because the check has to use a monotonic clock for its intervals.
    /// </summary>
    [Test]
    public async Task TestAttackWallClockJumpDoesNotTriggerViolationAsync()
    {
        var (player, plugin, timeProvider) = await CreatePlayerWithManualClockAsync().ConfigureAwait(false);
        player.Attributes![Stats.AttackSpeed] = 20;

        await plugin.AttackCheatCheckAsync(player, null, new SpeedHackCheckEventArgs()).ConfigureAwait(false);

        timeProvider.Advance(TimeSpan.FromMilliseconds(500));
        timeProvider.JumpUtc(TimeSpan.FromHours(-2));

        var args = new SpeedHackCheckEventArgs();
        await plugin.AttackCheatCheckAsync(player, null, args).ConfigureAwait(false);

        Assert.That(args.IsCheatDetected, Is.False);
        Assert.That(plugin.GetWarningCount(player), Is.EqualTo(0));
    }

    /// <summary>
    /// Tests that a slow multi-tile walk is never flagged, no matter how small the
    /// tolerance is (false positive from the field: 4 tiles in 2296 ms
    /// flagged with a ~800 ms tolerance, although slower than expected).
    /// </summary>
    [Test]
    public async Task TestSlowMultiTileWalkDoesNotTriggerViolationAsync()
    {
        var (player, plugin, timeProvider) = await CreatePlayerWithManualClockAsync().ConfigureAwait(false);
        plugin.Configuration!.WalkSpeedToleranceMs = 900;
        player.Position = StartPoint;

        var seedTo = new Point((byte)(StartPoint.X + 1), StartPoint.Y);
        RecordStep(player, plugin, StartPoint, seedTo);

        // Walked slowly: well above the expected ~4 * 278 ms for these 4 tiles.
        timeProvider.Advance(TimeSpan.FromMilliseconds(2300));

        var walkTo = new Point((byte)(seedTo.X + 4), seedTo.Y);
        var severity = await RecordStepAndCheckAsync(player, plugin, seedTo, walkTo).ConfigureAwait(false);

        Assert.That(severity, Is.EqualTo(SpeedHackViolationSeverity.None));
        Assert.That(plugin.GetWarningCount(player), Is.EqualTo(0));
        Assert.That(player.Account?.State, Is.EqualTo(AccountState.Normal));
    }

    /// <summary>
    /// Tests that quickly clicking short walks on a horse is not flagged (false positive from the field:
    /// a game master on a horse got banned). The client reports the tile it's about to enter as the
    /// start of each walk, so every click runs one tile ahead of the time which passed.
    /// </summary>
    [Test]
    public async Task TestQuickShortWalksOnHorseDoNotTriggerViolationAsync()
    {
        var timeProvider = new ManualTimeProvider(new DateTime(2026, 10, 17, 0, 0, 0, DateTimeKind.Utc));
        var player = await CreatePlayerWithSpeedAttributesAsync(timeProvider: timeProvider).ConfigureAwait(false);
        player.Attributes![Stats.MovementSpeed] = 17; // horse
        player.Position = StartPoint;
        var plugin = player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()!;
        plugin.Configuration!.WalkSpeedToleranceMs = 900;

        var severities = new List<SpeedHackViolationSeverity>();
        var from = StartPoint;
        for (int i = 0; i < plugin.Configuration.MaxAllowedWalkStartOffset; i++)
        {
            var to = new Point((byte)(from.X + 1), from.Y);
            severities.Add(await RecordStepAndCheckAsync(player, plugin, from, to).ConfigureAwait(false));
            from = to;
            timeProvider.Advance(TimeSpan.FromMilliseconds(3));
        }

        Assert.That(severities, Is.All.EqualTo(SpeedHackViolationSeverity.None));
        Assert.That(plugin.GetWarningCount(player), Is.EqualTo(0));
    }

    /// <summary>
    /// Tests that privileged accounts (game masters, spectators) are never banned or disconnected, even when exceeding the warning limit.
    /// </summary>
    /// <param name="accountState">The privileged account state.</param>
    [TestCase(AccountState.GameMaster)]
    [TestCase(AccountState.Spectator)]
    public async Task TestPrivilegedAccountIsNotBannedOrDisconnectedAsync(AccountState accountState)
    {
        var (player, plugin, timeProvider) = await CreateStrictWalkPlayerAsync().ConfigureAwait(false);
        player.Account!.State = accountState;
        plugin.Configuration!.DisconnectOnViolation = true;

        await RecordRapidStepsAsync(player, plugin, timeProvider, 12, 8, TimeSpan.FromMilliseconds(1100)).ConfigureAwait(false);

        Assert.That(plugin.GetWarningCount(player), Is.GreaterThan(plugin.Configuration.MaxWarnings));
        Assert.That(player.Account.State, Is.EqualTo(accountState));
        Assert.That(player.IsConnected, Is.True);
    }

    /// <summary>
    /// Tests that the default configuration only mitigates; it does not disconnect or ban.
    /// </summary>
    [Test]
    public void TestDefaultConfigurationIsMitigationOnly()
    {
        var config = new SpeedHackDetectConfiguration();

        Assert.That(config.AutoBan, Is.False);
        Assert.That(config.DisconnectOnViolation, Is.False);
        Assert.That(config.ObserveOnly, Is.False);
        Assert.That(config.EnforceWalkSpeed, Is.False);
        Assert.That(config.WalkSpeedWindowSeconds, Is.EqualTo(WalkSpeedValidator.DefaultWindowSeconds));
    }

    /// <summary>
    /// Tests that a rejected walk stops the previous walk and synchronizes the
    /// server position with the client, so the server does not keep walking
    /// along the old path while the client is already somewhere else.
    /// </summary>
    [Test]
    public async Task TestRejectedWalkStopsPreviousWalkAndSynchronizesClientPositionAsync()
    {
        var (player, plugin, timeProvider) = await CreateStrictWalkPlayerAsync().ConfigureAwait(false);
        plugin.Configuration!.ViolationScoreRejectThreshold = RejectOnFirstViolationThreshold;
        player.Position = StartPoint;

        await SeedMovementStateWithShortWalkAsync(player).ConfigureAwait(false);

        // Record executed steps which exceed the one second window, so the next walk is rejected.
        ExceedWalkWindowWithRecordedSteps(player, plugin, timeProvider);

        var rejectedFrom = new Point((byte)(StartPoint.X + 9), StartPoint.Y);
        player.Position = rejectedFrom;

        WalkingStep[] rejectedSteps = [new() { From = rejectedFrom, To = new Point((byte)(rejectedFrom.X + 1), rejectedFrom.Y), Direction = Direction.East }];
        await player.WalkToAsync(rejectedSteps[^1].To, rejectedSteps).ConfigureAwait(false);

        Assert.That(plugin.GetWarningCount(player), Is.EqualTo(1));
        Assert.That(player.Position, Is.EqualTo(rejectedFrom));
        Assert.That(player.IsWalking, Is.False);
        var objectMovedPlugIn = player.ViewPlugIns.GetPlugIn<IObjectMovedPlugIn>();
        Mock.Get<IObjectMovedPlugIn>(objectMovedPlugIn!).Verify(m => m.ObjectMovedAsync(player, MoveType.Instant), Times.Once);
    }

    /// <summary>
    /// Tests that a rejected walk leaves the server position alone and pulls the client back to it:
    /// validation runs before any client position is adopted.
    /// </summary>
    [Test]
    public async Task TestRejectedWalkDoesNotAdoptClientPositionAsync()
    {
        var (player, speedHackDetectPlugIn, timeProvider) = await CreateStrictWalkPlayerAsync().ConfigureAwait(false);
        speedHackDetectPlugIn.Configuration!.ViolationScoreRejectThreshold = RejectOnFirstViolationThreshold;
        player.Position = StartPoint;

        await SeedMovementStateWithShortWalkAsync(player).ConfigureAwait(false);

        // Record executed steps which exceed the one second window, so the next walk is rejected.
        ExceedWalkWindowWithRecordedSteps(player, speedHackDetectPlugIn, timeProvider);

        var rejectedFrom = new Point((byte)(StartPoint.X + 9), StartPoint.Y);
        player.Position = rejectedFrom;
        WalkingStep[] rejectedSteps = [new() { From = rejectedFrom, To = new Point((byte)(rejectedFrom.X + 1), rejectedFrom.Y), Direction = Direction.East }];
        await player.WalkToAsync(rejectedSteps[^1].To, rejectedSteps).ConfigureAwait(false);

        var objectMovedPlugIn = player.ViewPlugIns.GetPlugIn<IObjectMovedPlugIn>();
        Assert.That(objectMovedPlugIn, Is.Not.Null);
        Mock.Get<IObjectMovedPlugIn>(objectMovedPlugIn!).Verify(m => m.ObjectMovedAsync(player, MoveType.Instant), Times.Once);
        Assert.That(player.Position, Is.EqualTo(rejectedFrom));
        Assert.That(player.IsWalking, Is.False);
    }

    /// <summary>
    /// Tests that positions which the server takes over from the client count as walked distance.
    /// A client which reports a position further ahead with every walk request would otherwise cover
    /// distance which the walk speed check never sees, because it only counts executed steps.
    /// </summary>
    [Test]
    public async Task TestTakenOverClientPositionsCountTowardsWalkSpeedAsync()
    {
        var timeProvider = new ManualTimeProvider(new DateTime(2026, 10, 26, 0, 0, 0, DateTimeKind.Utc));
        var player = await CreatePlayerWithSpeedAttributesAsync(timeProvider: timeProvider).ConfigureAwait(false);
        var configuration = player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()!.Configuration!;
        configuration.WalkSpeedToleranceMs = 0;
        configuration.WalkSpeedWindowSeconds = 1;
        configuration.MaxAllowedWalkStartOffset = 5;
        configuration.ViolationScoreRejectThreshold = RejectOnFirstViolationThreshold;
        player.Position = StartPoint;

        // A jump of four tiles is more than the offset which is taken as rounding, and costs more walking
        // time than the one second window has. The executed steps of the started walk only add to that.
        var firstFrom = new Point((byte)(StartPoint.X + 4), StartPoint.Y);
        var firstTo = new Point((byte)(firstFrom.X + 1), firstFrom.Y);
        WalkingStep[] firstSteps = [new() { From = firstFrom, To = firstTo, Direction = Direction.East }];
        await player.WalkToAsync(firstTo, firstSteps).ConfigureAwait(false);
        await player.StopWalkingAsync().ConfigureAwait(false);
        Assert.That(player.Position.X, Is.GreaterThanOrEqualTo(firstFrom.X), "The first client position was not taken over.");

        var positionBeforeSecondJump = player.Position;
        var secondFrom = new Point((byte)(positionBeforeSecondJump.X + 4), positionBeforeSecondJump.Y);
        var secondTo = new Point((byte)(secondFrom.X + 1), secondFrom.Y);
        WalkingStep[] secondSteps = [new() { From = secondFrom, To = secondTo, Direction = Direction.East }];
        await player.WalkToAsync(secondTo, secondSteps).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(player.Position, Is.EqualTo(positionBeforeSecondJump), "The second jump should have been rejected.");
            Assert.That(player.IsWalking, Is.False);
        });
    }

    /// <summary>
    /// Tests that a real <see cref="PlayerMovement"/> and <see cref="Walker"/> detect a client which
    /// keeps reporting a position ahead without waiting for the server step, while the server still
    /// paces the player's own steps.
    /// </summary>
    [Test]
    public async Task TestRealFastClientIsDetectedAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        var plugin = player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()!;
        plugin.Configuration!.AutoBan = false;
        plugin.Configuration.MaxWarnings = 100;
        plugin.Configuration.WalkSpeedToleranceMs = 0;
        plugin.Configuration.WalkSpeedWindowSeconds = 1;
        plugin.Configuration.ViolationScoreRejectThreshold = RejectOnFirstViolationThreshold;
        player.Attributes![Stats.MovementSpeed] = 100;
        player.Position = StartPoint;

        var clientPosition = StartPoint;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        while (stopwatch.Elapsed < TimeSpan.FromMilliseconds(800))
        {
            var from = clientPosition;
            var to = new Point((byte)(from.X + 1), from.Y);
            WalkingStep[] steps = [new() { From = from, To = to, Direction = Direction.East }];
            await player.WalkToAsync(to, steps).ConfigureAwait(false);
            clientPosition = to;
            await Task.Delay(10).ConfigureAwait(false);
        }

        Assert.That(plugin.GetWarningCount(player), Is.GreaterThan(0), "The fast client should be detected.");
    }

    /// <summary>
    /// Tests that a walk request which starts on the diagonal neighbour of the server position is
    /// taken over while walking. That's what a client reports when it turns, and it's never closer
    /// than 1.41 tiles, which is more than the lag bound right after a step.
    /// </summary>
    [Test]
    public async Task TestWalkStartOnDiagonalNeighbourWhileWalkingIsTakenOverAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        player.Position = StartPoint;

        Point P(int offset) => new((byte)(StartPoint.X + offset), (byte)(StartPoint.Y + offset));
        WalkingStep[] steps =
        [
            new() { From = P(0), To = P(1), Direction = Direction.SouthEast },
            new() { From = P(1), To = P(2), Direction = Direction.SouthEast },
            new() { From = P(2), To = P(3), Direction = Direction.SouthEast },
        ];
        await player.WalkToAsync(P(3), steps).ConfigureAwait(false);
        Assert.That(await WaitUntilAsync(() => player.Position == P(1), TimeSpan.FromSeconds(5)).ConfigureAwait(false), Is.True);

        WalkingStep[] turnSteps = [new() { From = P(2), To = P(3), Direction = Direction.SouthEast }];
        await player.WalkToAsync(P(3), turnSteps).ConfigureAwait(false);

        Assert.That(player.Position.X, Is.GreaterThanOrEqualTo(P(2).X), "The diagonal client position was not taken over.");
    }

    /// <summary>
    /// Tests that observe-only mode detects without punishing or resynchronizing:
    /// walks are performed and no warnings are recorded.
    /// </summary>
    [Test]
    public async Task TestObserveOnlyModeNeitherPunishesNorResynchronizesAsync()
    {
        var (player, plugin, timeProvider) = await CreateStrictWalkPlayerAsync().ConfigureAwait(false);
        plugin.Configuration!.ObserveOnly = true;
        player.Position = StartPoint;

        await SeedMovementStateWithShortWalkAsync(player).ConfigureAwait(false);

        // Record executed steps which exceed the one second window, so the next walk is a violation.
        ExceedWalkWindowWithRecordedSteps(player, plugin, timeProvider);

        // Would be a violation with enforcement on: the executed steps exceeded the one second window.
        var fastFrom = new Point((byte)(StartPoint.X + 9), StartPoint.Y);
        player.Position = fastFrom;
        WalkingStep[] fastSteps = [new() { From = fastFrom, To = new Point((byte)(fastFrom.X + 1), fastFrom.Y), Direction = Direction.East }];
        await player.WalkToAsync(fastSteps[^1].To, fastSteps).ConfigureAwait(false);

        Assert.That(plugin.GetWarningCount(player), Is.EqualTo(0));
        Assert.That(player.Account?.State, Is.EqualTo(AccountState.Normal));
        var fastTarget = fastSteps[^1].To;
        Assert.That(
            await WaitUntilAsync(() => player.Position == fastTarget, TimeSpan.FromSeconds(5)).ConfigureAwait(false),
            Is.True,
            "The walk was not performed.");
        var objectMovedPlugIn = player.ViewPlugIns.GetPlugIn<IObjectMovedPlugIn>();
        Mock.Get<IObjectMovedPlugIn>(objectMovedPlugIn!).Verify(m => m.ObjectMovedAsync(player, MoveType.Instant), Times.Never);
    }

    /// <summary>
    /// Tests that observe-only mode doesn't score the speed violation and the position mismatch
    /// of the same walk as two separate violations.
    /// </summary>
    [Test]
    public async Task TestObserveOnlyDoesNotScoreWalkViolationTwiceAsync()
    {
        var (player, plugin, _) = await CreatePlayerWithManualClockAsync().ConfigureAwait(false);
        plugin.Configuration!.ObserveOnly = true;
        plugin.Configuration.WalkSpeedToleranceMs = 0;
        plugin.Configuration.MaxAllowedWalkStartOffset = 3;
        plugin.Configuration.WalkSpeedWindowSeconds = 1;
        player.Position = StartPoint;

        var seedTo = new Point((byte)(StartPoint.X + 1), StartPoint.Y);
        RecordStep(player, plugin, StartPoint, seedTo);

        var fastFrom = seedTo;
        var fastTo = new Point((byte)(fastFrom.X + 8), fastFrom.Y);
        var severity = await RecordStepAndCheckAsync(player, plugin, fastFrom, fastTo).ConfigureAwait(false);

        Assert.That(severity, Is.EqualTo(SpeedHackViolationSeverity.None));
        Assert.That(plugin.GetWarningCount(player), Is.EqualTo(0));
        Assert.That(plugin.GetViolationScore(player), Is.LessThan(3.0));
    }

    /// <summary>
    /// Tests that walk speed observations are diagnostic only by default: a walk which would
    /// exceed the budget of the window is accepted, not rejected, and does not warn or score.
    /// </summary>
    [Test]
    public async Task TestWalkSpeedViolationDoesNotBlockByDefaultAsync()
    {
        var (player, plugin, timeProvider) = await CreateStrictWalkPlayerAsync().ConfigureAwait(false);
        plugin.Configuration!.EnforceWalkSpeed = false;
        player.Position = StartPoint;

        ExceedWalkWindowWithRecordedSteps(player, plugin, timeProvider);

        var fastFrom = new Point((byte)(StartPoint.X + 9), StartPoint.Y);
        var fastTarget = new Point((byte)(fastFrom.X + 1), fastFrom.Y);
        player.Position = fastFrom;
        WalkingStep[] fastSteps = [new() { From = fastFrom, To = fastTarget, Direction = Direction.East }];

        await player.WalkToAsync(fastTarget, fastSteps).ConfigureAwait(false);

        Assert.That(plugin.GetWarningCount(player), Is.EqualTo(0));
        Assert.That(plugin.GetViolationScore(player), Is.EqualTo(0));
        Assert.That(
            await WaitUntilAsync(() => player.Position == fastTarget, TimeSpan.FromSeconds(5)).ConfigureAwait(false),
            Is.True,
            "The client-authoritative walk was not performed.");
    }

    /// <summary>
    /// Tests that a soft walk violation is only recorded, while the walk is still performed.
    /// </summary>
    [Test]
    public async Task TestSoftWalkViolationIsRecordedButWalkIsPerformedAsync()
    {
        var (player, plugin, target) = await RequestWalkAfterViolationAsync(_ => { }).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(plugin.GetWarningCount(player), Is.EqualTo(1));
            Assert.That(plugin.GetViolationScore(player), Is.GreaterThan(0));
        });
        Assert.That(
            await WaitUntilAsync(() => player.Position == target, TimeSpan.FromSeconds(5)).ConfigureAwait(false),
            Is.True,
            "The walk of a soft violation was not performed.");
        var objectMovedPlugIn = player.ViewPlugIns.GetPlugIn<IObjectMovedPlugIn>();
        Mock.Get<IObjectMovedPlugIn>(objectMovedPlugIn!).Verify(m => m.ObjectMovedAsync(player, MoveType.Instant), Times.Never);
    }

    /// <summary>
    /// Tests that a rejected walk violation blocks the walk and resynchronizes the client.
    /// </summary>
    [Test]
    public async Task TestRejectWalkViolationBlocksWalkAndResynchronizesClientAsync()
    {
        var (player, plugin, target) = await RequestWalkAfterViolationAsync(c => c.ViolationScoreRejectThreshold = RejectOnFirstViolationThreshold).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(plugin.GetWarningCount(player), Is.EqualTo(1));
            Assert.That(player.Position, Is.Not.EqualTo(target));
            Assert.That(player.IsWalking, Is.False);
        });
        var objectMovedPlugIn = player.ViewPlugIns.GetPlugIn<IObjectMovedPlugIn>();
        Mock.Get<IObjectMovedPlugIn>(objectMovedPlugIn!).Verify(m => m.ObjectMovedAsync(player, MoveType.Instant), Times.Once);
    }

    /// <summary>
    /// Tests that an observed-only walk violation is neither recorded nor blocked.
    /// </summary>
    [Test]
    public async Task TestObserveOnlyWalkViolationIsNotRecordedAndWalkIsPerformedAsync()
    {
        var (player, plugin, target) = await RequestWalkAfterViolationAsync(c =>
        {
            c.ObserveOnly = true;
            c.ViolationScoreRejectThreshold = RejectOnFirstViolationThreshold;
        }).ConfigureAwait(false);

        Assert.That(plugin.GetWarningCount(player), Is.EqualTo(0));
        Assert.That(
            await WaitUntilAsync(() => player.Position == target, TimeSpan.FromSeconds(5)).ConfigureAwait(false),
            Is.True,
            "The walk of an observed violation was not performed.");
        var objectMovedPlugIn = player.ViewPlugIns.GetPlugIn<IObjectMovedPlugIn>();
        Mock.Get<IObjectMovedPlugIn>(objectMovedPlugIn!).Verify(m => m.ObjectMovedAsync(player, MoveType.Instant), Times.Never);
    }

    /// <summary>
    /// Tests that repeated small position corrections of the client can't stay free: after a bounded
    /// total they are charged to the walk speed check like any other taken-over distance.
    /// </summary>
    [Test]
    public async Task TestRepeatedSmallPositionCorrectionsAreChargedAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        var recorder = new RecordingWalkStepPlugIn();
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IWalkStepPlugIn>(recorder);
        player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()!.Configuration!.EnforceWalkSpeed = false;
        player.Position = StartPoint;

        // Every request starts one tile ahead of the server, which is taken over as rounding.
        const int requestCount = 10;
        for (int i = 0; i < requestCount; i++)
        {
            var from = new Point((byte)(player.Position.X + 1), player.Position.Y);
            var to = new Point((byte)(from.X + 1), from.Y);
            WalkingStep[] steps = [new() { From = from, To = to, Direction = Direction.East }];
            await player.WalkToAsync(to, steps).ConfigureAwait(false);
            await player.StopWalkingAsync().ConfigureAwait(false);
        }

        Assert.That(recorder.TakenOverSteps, Is.InRange(1, requestCount - 1), "The first corrections should be free, the later ones charged.");
    }

    /// <summary>
    /// Tests that a walk whose first path step is blocked doesn't move the player, even if the
    /// reported start position is offset, and that the client is pulled back.
    /// </summary>
    [Test]
    public async Task TestBlockedFirstPathStepWithOffsetStartDoesNotMovePlayerAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        player.Position = StartPoint;

        var offsetStart = new Point((byte)(StartPoint.X + 2), StartPoint.Y);
        var blockedStep = new Point((byte)(offsetStart.X + 1), offsetStart.Y);
        player.CurrentMap!.Terrain.WalkMap[blockedStep.X, blockedStep.Y] = false;
        WalkingStep[] steps = [new() { From = offsetStart, To = blockedStep, Direction = Direction.East }];

        await player.WalkToAsync(blockedStep, steps).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(player.Position, Is.EqualTo(StartPoint));
            Assert.That(player.IsWalking, Is.False);
        });
        var objectMovedPlugIn = player.ViewPlugIns.GetPlugIn<IObjectMovedPlugIn>();
        Mock.Get<IObjectMovedPlugIn>(objectMovedPlugIn!).Verify(m => m.ObjectMovedAsync(player, MoveType.Instant), Times.Once);
    }

    /// <summary>
    /// Tests that a rapid burst of attack violations is debounced into a single warning,
    /// so legitimate bursts (lag, re-attempted denied attacks) cannot ban within a second.
    /// </summary>
    [Test]
    public async Task TestRapidAttackViolationsAreDebouncedAsync()
    {
        var (player, _, _) = await CreatePlayerWithManualClockAsync().ConfigureAwait(false);
        player.Attributes![Stats.AttackSpeed] = 20;

        var speedCheck = player.GameContext.PlugInManager.GetPlugInPoint<IAttackSpeedCheckPlugIn>();
        Assert.That(speedCheck, Is.Not.Null);

        for (int i = 0; i < 10; i++)
        {
            var args = new SpeedHackCheckEventArgs();
            await speedCheck!.AttackCheatCheckAsync(player, null, args).ConfigureAwait(false);
        }

        var plugin = player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()!;
        Assert.That(plugin.GetWarningCount(player), Is.EqualTo(1));
        Assert.That(player.Account?.State, Is.EqualTo(AccountState.Normal));
    }

    /// <summary>
    /// Tests that sustained violations escalate to a hard rejection, while the alert debounce
    /// still prevents each rejection from flooding the warning history.
    /// </summary>
    [Test]
    public async Task TestSustainedViolationsEscalateToRejectAsync()
    {
        var (player, plugin, timeProvider) = await CreateStrictWalkPlayerAsync().ConfigureAwait(false);
        plugin.Configuration!.ViolationScoreRejectThreshold = 4.0;
        player.Position = StartPoint;

        var seedTo = new Point((byte)(StartPoint.X + 1), StartPoint.Y);
        RecordStep(player, plugin, StartPoint, seedTo);

        // Four rapid 6-tile jumps: the first counts, the following ones are inside the alert
        // debounce. The severity still escalates past the score threshold, but the warning
        // history is not filled by each rejected request.
        var severities = new List<SpeedHackViolationSeverity>();
        for (int i = 1; i <= 4; i++)
        {
            timeProvider.Advance(TimeSpan.FromSeconds(1));
            var from = new Point((byte)(StartPoint.X + (i * 6)), StartPoint.Y);
            var to = new Point((byte)(from.X + 6), from.Y);
            severities.Add(await RecordStepAndCheckAsync(player, plugin, from, to).ConfigureAwait(false));
        }

        Assert.That(severities, Is.EqualTo(new[]
        {
            SpeedHackViolationSeverity.Soft,
            SpeedHackViolationSeverity.Soft,
            SpeedHackViolationSeverity.Reject,
            SpeedHackViolationSeverity.Reject,
        }));
        Assert.That(plugin.GetWarningCount(player), Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that the walk window only sums up the cost within its length.
    /// </summary>
    [Test]
    public void TestWalkSpeedWindowSumsOnlyCostWithinWindow()
    {
        var window = new WalkSpeedWindow();

        Assert.That(window.Add(0, 100, 1000), Is.EqualTo(100));
        Assert.That(window.Add(500, 200, 1000), Is.EqualTo(300));

        // The cost at 0 ms is not part of the window which starts at 0 ms.
        Assert.That(window.Add(1000, 50, 1000), Is.EqualTo(250));
        Assert.That(window.Add(1600, 10, 1000), Is.EqualTo(60));
    }

    /// <summary>
    /// Tests that the walk window doesn't bank idle time: whatever is walked after a long
    /// pause is compared to a window of the configured length, not to the whole pause.
    /// </summary>
    [Test]
    public void TestWalkSpeedWindowDoesNotBankIdleTime()
    {
        var window = new WalkSpeedWindow();

        window.Add(0, 100, 1000);

        Assert.That(window.Add(600_000, 400, 1000), Is.EqualTo(400));
    }

    /// <summary>
    /// Tests that clearing the walk window starts it empty.
    /// </summary>
    [Test]
    public void TestWalkSpeedWindowClearRestartsWindow()
    {
        var window = new WalkSpeedWindow();
        window.Add(0, 100, 1000);
        window.Add(100, 100, 1000);

        window.Clear();

        Assert.That(window.SampleCount, Is.EqualTo(0));
        Assert.That(window.Add(200, 5, 1000), Is.EqualTo(5));
    }

    /// <summary>
    /// Tests that a client which floods the server with walk packets can't make the window grow without bounds,
    /// and the sum stays accurate.
    /// </summary>
    [Test]
    public void TestWalkSpeedWindowBoundsStoredSamples()
    {
        var window = new WalkSpeedWindow();
        var maxSampleCount = 0;
        var lastWindowCost = 0.0;

        for (var timeMs = 1; timeMs <= 5000; timeMs++)
        {
            lastWindowCost = window.Add(timeMs, 1, 1000);
            maxSampleCount = Math.Max(maxSampleCount, window.SampleCount);
        }

        Assert.That(maxSampleCount, Is.LessThanOrEqualTo((1000 / WalkSpeedWindow.MinSampleSpacingMs) + 2));
        Assert.That(lastWindowCost, Is.EqualTo(1000).Within(WalkSpeedWindow.MinSampleSpacingMs));
    }

    /// <summary>
    /// Tests that the expected walking time of a distance scales with the step delay and
    /// charges diagonal distances more, like the client takes longer for them.
    /// </summary>
    [Test]
    public void TestWalkSpeedValidatorExpectedTime()
    {
        // 300 ms step delay minus the 50 ms margin.
        Assert.That(WalkSpeedValidator.GetExpectedTimeMs(1, 300), Is.EqualTo(250).Within(0.001));

        // The step delay of a very fast character is below the minimum floor, which is then the step delay itself.
        Assert.That(WalkSpeedValidator.GetExpectedTimeMs(2, 40), Is.EqualTo(80).Within(0.001));

        Assert.That(WalkSpeedValidator.GetExpectedTimeMs(Math.Sqrt(2), 300), Is.EqualTo(250 * Math.Sqrt(2)).Within(0.001));
    }

    /// <summary>
    /// Tests that the walk window evaluation allows exactly the window plus the tolerance.
    /// </summary>
    [Test]
    public void TestWalkSpeedValidatorEvaluatesBudgetOfWindowAndTolerance()
    {
        var atLimit = WalkSpeedValidator.Evaluate(windowCostMs: 17_000, windowMs: 15_000, toleranceMs: 2_000);
        var aboveLimit = WalkSpeedValidator.Evaluate(windowCostMs: 17_500, windowMs: 15_000, toleranceMs: 2_000);

        Assert.That(atLimit.IsViolation, Is.False);
        Assert.That(aboveLimit.IsViolation, Is.True);
        Assert.That(aboveLimit.BudgetMs, Is.EqualTo(17_000));
        Assert.That(aboveLimit.OvershootMs, Is.EqualTo(500));
    }

    /// <summary>
    /// Tests that existing databases, which predate the window setting and deserialize it as 0,
    /// fall back to the default.
    /// </summary>
    [Test]
    public void TestWalkSpeedWindowSecondsFallBackToDefault()
    {
        Assert.That(WalkSpeedValidator.ResolveWindowSeconds(0), Is.EqualTo(WalkSpeedValidator.DefaultWindowSeconds));
        Assert.That(WalkSpeedValidator.ResolveWindowSeconds(-5), Is.EqualTo(WalkSpeedValidator.DefaultWindowSeconds));
        Assert.That(WalkSpeedValidator.ResolveWindowSeconds(30), Is.EqualTo(30));
    }

    /// <summary>
    /// Tests that the ping clock check detects a client which claims more time passed than the server observed.
    /// </summary>
    [Test]
    public void TestPingClockValidatorDetectsClientAhead()
    {
        var result = PingClockValidator.Evaluate(previousTickCount: 1000, currentTickCount: 6000, serverElapsedMs: 1000, toleranceMs: 3000);

        Assert.That(result.ClientElapsedMs, Is.EqualTo(5000));
        Assert.That(result.IsClientAhead, Is.True);
        Assert.That(result.IsClockReset, Is.False);
    }

    /// <summary>
    /// Tests that the ping clock check accepts the 32-bit tick count wrap.
    /// </summary>
    [Test]
    public void TestPingClockValidatorAcceptsTickWrap()
    {
        var result = PingClockValidator.Evaluate(previousTickCount: uint.MaxValue - 100, currentTickCount: 99, serverElapsedMs: 200, toleranceMs: 0);

        Assert.That(result.ClientElapsedMs, Is.EqualTo(200));
        Assert.That(result.IsClientAhead, Is.False);
        Assert.That(result.IsClockReset, Is.False);
    }

    /// <summary>
    /// Tests that a large tick count jump is treated as a client restart instead of a clock violation.
    /// </summary>
    [Test]
    public void TestPingClockValidatorTreatsLargeJumpAsReset()
    {
        var result = PingClockValidator.Evaluate(previousTickCount: 1000, currentTickCount: 0, serverElapsedMs: 1000, toleranceMs: 3000);

        Assert.That(result.IsClientAhead, Is.False);
        Assert.That(result.IsClockReset, Is.True);
    }

    /// <summary>
    /// Tests that ping inconsistencies are observed only and never cause a warning, disconnect or ban.
    /// </summary>
    [Test]
    public async Task TestPingInconsistencyDoesNotSanctionAsync()
    {
        var (player, plugin, timeProvider) = await CreatePlayerWithManualClockAsync().ConfigureAwait(false);
        player.Attributes![Stats.AttackSpeed] = 20;

        await plugin.PingCheatCheckAsync(player, tickCount: 1000, attackSpeed: 20).ConfigureAwait(false);
        timeProvider.Advance(TimeSpan.FromSeconds(1));
        await plugin.PingCheatCheckAsync(player, tickCount: 6000, attackSpeed: 100).ConfigureAwait(false);

        Assert.That(plugin.GetWarningCount(player), Is.EqualTo(0));
        Assert.That(player.Account?.State, Is.EqualTo(AccountState.Normal));
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
        await PerformRapidWalkRequestsAsync(player).ConfigureAwait(false);

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
        var (player, plugin, timeProvider) = await CreatePlayerWithManualClockAsync().ConfigureAwait(false);
        Assert.That(plugin, Is.Not.Null);

        // Customize configuration: 5 warnings limit, no autoban, no disconnect
        plugin!.Configuration!.MaxWarnings = 5;
        plugin.Configuration.AutoBan = false;
        plugin.Configuration.DisconnectOnViolation = false;
        plugin.Configuration.WalkSpeedToleranceMs = 0;
        plugin.Configuration.MaxAllowedWalkStartOffset = 0;
        plugin.Configuration.WalkSpeedWindowSeconds = 1;

        // Perform rapid executed steps to exceed the original 3 warnings limit, and the new 5 warnings limit.
        // Each step is a separate violation, because a violation restarts the window.
        await RecordRapidStepsAsync(player, plugin, timeProvider, 18, 8, TimeSpan.FromMilliseconds(1100)).ConfigureAwait(false);

        // The account should still be normal (no autoban) and not disconnected
        Assert.That(player.Account?.State, Is.EqualTo(AccountState.Normal));
        Assert.That(player.IsConnected, Is.True);
        Assert.That(player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()!.GetWarningCount(player), Is.GreaterThan(5));
    }

    /// <summary>
    /// Tests that an offline player - whose walks are issued by the server itself - is not checked at all.
    /// </summary>
    [Test]
    public async Task TestOfflinePlayerIsNotCheckedOnWalkAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        var offlinePlayer = await CreateOfflinePlayerAsync(player).ConfigureAwait(false);
        var plugin = player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()!;

        // The same walk pattern which bans a regular player in TestWalkSpeedHackDetectionBansAccountAsync.
        for (int i = 0; i < 12; i++)
        {
            var nextFrom = new Point((byte)(StartPoint.X + (i * 2)), StartPoint.Y);
            var nextTo = new Point((byte)(StartPoint.X + (i * 2) + 2), StartPoint.Y);

            offlinePlayer.Position = nextFrom;
            plugin.ResetAlertDebounce(offlinePlayer);

            WalkingStep[] steps = [new() { From = nextFrom, To = nextTo, Direction = Direction.East }];

            await offlinePlayer.WalkToAsync(nextTo, steps).ConfigureAwait(false);
        }

        Assert.That(plugin.GetWarningCount(offlinePlayer), Is.EqualTo(0));
        Assert.That(offlinePlayer.Account?.State, Is.EqualTo(AccountState.Normal));
    }

    /// <summary>
    /// Tests that an offline player is not checked on attacks. Its MU Helper tick performs the same work
    /// cycle as the original client helper (recover, then attack), which draws more than one token from a
    /// single tick and would eventually empty the bucket.
    /// </summary>
    [Test]
    public async Task TestOfflinePlayerIsNotCheckedOnAttackAsync()
    {
        var player = await CreatePlayerWithSpeedAttributesAsync().ConfigureAwait(false);
        var offlinePlayer = await CreateOfflinePlayerAsync(player).ConfigureAwait(false);
        offlinePlayer.Attributes![Stats.AttackSpeed] = 20;

        var speedCheck = player.GameContext.PlugInManager.GetPlugInPoint<IAttackSpeedCheckPlugIn>()!;

        // The same burst which is detected for a regular player in TestAttackSpeedHackDetectionAsync.
        for (int i = 0; i < 20; i++)
        {
            var args = new SpeedHackCheckEventArgs();
            await speedCheck.AttackCheatCheckAsync(offlinePlayer, null, args).ConfigureAwait(false);
            Assert.That(args.IsCheatDetected, Is.False);
        }

        Assert.That(player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()!.GetWarningCount(offlinePlayer), Is.EqualTo(0));
        Assert.That(offlinePlayer.Account?.State, Is.EqualTo(AccountState.Normal));
    }

    /// <summary>
    /// Creates a player with the walk speed check set up as strictly as possible: no tolerance, no allowed
    /// start offset and a one second window, driven by a manual clock.
    /// </summary>
    /// <param name="inventoryItems">The inventory items of the player.</param>
    /// <returns>The player, its speed hack plugin and the clock which the plugin uses.</returns>
    private static async ValueTask<(Player Player, SpeedHackDetectPlugIn PlugIn, ManualTimeProvider TimeProvider)> CreateStrictWalkPlayerAsync(List<Item>? inventoryItems = null)
    {
        var timeProvider = new ManualTimeProvider(new DateTime(2026, 10, 12, 0, 0, 0, DateTimeKind.Utc));
        var player = await CreatePlayerWithSpeedAttributesAsync(inventoryItems, timeProvider).ConfigureAwait(false);
        var plugin = player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()!;
        plugin.Configuration!.WalkSpeedToleranceMs = 0;
        plugin.Configuration.MaxAllowedWalkStartOffset = 0;
        plugin.Configuration.WalkSpeedWindowSeconds = 1;
        return (player, plugin, timeProvider);
    }

    /// <summary>
    /// Records executed walk steps one after another, starting at <see cref="StartPoint"/>. Before each step
    /// the clock advances by the interval and the alert debounce is reset, so every step is evaluated on its own.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="plugin">The speed hack plugin.</param>
    /// <param name="timeProvider">The clock of the plugin.</param>
    /// <param name="stepCount">The number of steps.</param>
    /// <param name="tilesPerStep">The distance of each step in tiles.</param>
    /// <param name="interval">The time which passes before each step.</param>
    private static async ValueTask RecordRapidStepsAsync(Player player, SpeedHackDetectPlugIn plugin, ManualTimeProvider timeProvider, int stepCount, int tilesPerStep, TimeSpan interval)
    {
        for (int i = 0; i < stepCount; i++)
        {
            var from = new Point((byte)(StartPoint.X + (i * tilesPerStep)), StartPoint.Y);
            var to = new Point((byte)(from.X + tilesPerStep), from.Y);
            timeProvider.Advance(interval);
            plugin.ResetAlertDebounce(player);
            await RecordStepAndCheckAsync(player, plugin, from, to).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Records a walk step which the server executed, as reported by the walker, and evaluates the
    /// walk rate afterwards. Simulates a real server step instead of a client walk request.
    /// </summary>
    private static async ValueTask<SpeedHackViolationSeverity> RecordStepAndCheckAsync(Player player, SpeedHackDetectPlugIn plugin, Point from, Point to)
    {
        RecordStep(player, plugin, from, to);
        var args = new SpeedHackCheckEventArgs();
        await plugin.WalkCheatCheckAsync(player, new[] { new WalkingStep { From = to, To = to, Direction = Direction.East } }, args).ConfigureAwait(false);
        return args.Severity;
    }

    /// <summary>
    /// Records a walk step which the server executed, without evaluating the walk rate.
    /// </summary>
    private static void RecordStep(Player player, SpeedHackDetectPlugIn plugin, Point from, Point to)
    {
        var stepDelay = TimeSpan.FromTicks((long)(player.StepDelay.Ticks * Math.Max(1.0, from.EuclideanDistanceTo(to))));
        plugin.WalkStepPerformed(player, new WalkingStep { From = from, To = to, Direction = Direction.East }, stepDelay);
        player.Position = to;
    }

    private static async ValueTask<OfflinePlayer> CreateOfflinePlayerAsync(Player regularPlayer)
    {
        var offlinePlayer = new OfflinePlayer(regularPlayer.GameContext) { Account = regularPlayer.Account };
        await offlinePlayer.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await offlinePlayer.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
        await offlinePlayer.PlayerState.TryAdvanceToAsync(PlayerState.CharacterSelection).ConfigureAwait(false);
        await offlinePlayer.SetSelectedCharacterAsync(regularPlayer.SelectedCharacter!).ConfigureAwait(false);
        return offlinePlayer;
    }

    private static async ValueTask<Player> CreatePlayerWithSpeedAttributesAsync(List<Item>? inventoryItems = null, TimeProvider? timeProvider = null)
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
                new(Stats.MagicSpeed, 0, false),
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
        // Auto-ban and walk-speed enforcement are disabled by default, but enabled here so that
        // the tests can verify the punishment and rejection code paths.
        var speedHackDetectConfiguration = new SpeedHackDetectConfiguration
        {
            AutoBan = true,
            EnforceWalkSpeed = true,
        };
        var speedHackDetectPlugIn = timeProvider is null
            ? new SpeedHackDetectPlugIn { Configuration = speedHackDetectConfiguration }
            : new SpeedHackDetectPlugIn(timeProvider) { Configuration = speedHackDetectConfiguration };
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IFeaturePlugIn>(speedHackDetectPlugIn);
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<ISpeedHackCheatCheckPlugIn>(speedHackDetectPlugIn);
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IAttackSpeedCheckPlugIn>(speedHackDetectPlugIn);
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPingCheckPlugIn>(speedHackDetectPlugIn);
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IMovementStateResetPlugIn>(speedHackDetectPlugIn);
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IWalkRequestValidationPlugIn>(speedHackDetectPlugIn);
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IWalkStepPlugIn>(speedHackDetectPlugIn);
        player.GameContext.FeaturePlugIns.AddPlugIn(speedHackDetectPlugIn, true);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.CharacterSelection).ConfigureAwait(false);
        await player.SetSelectedCharacterAsync(character).ConfigureAwait(false);

        return player;
    }

    /// <summary>
    /// Simulates a player who walks in circles, with the server executing a walk step every
    /// <see cref="WalkPacketIntervalMs"/>. The character moves with <paramref name="speedFactor"/>
    /// times the allowed pace; the recorded steps are the ones the server actually executed.
    /// </summary>
    /// <returns>The severities of all walk checks.</returns>
    private static async ValueTask<List<SpeedHackViolationSeverity>> WalkInCirclesAsync(Player player, SpeedHackDetectPlugIn plugin, ManualTimeProvider timeProvider, double speedFactor, TimeSpan duration)
    {
        var ring = CreateRing(StartPoint, 9);
        var stepDelayMs = player.StepDelay.TotalMilliseconds;
        var severities = new List<SpeedHackViolationSeverity>();
        var previousTileIndex = 1;
        for (var elapsedMs = 0; elapsedMs <= duration.TotalMilliseconds; elapsedMs += WalkPacketIntervalMs)
        {
            var tileIndex = (int)(elapsedMs * speedFactor / stepDelayMs) + 1;
            if (tileIndex != previousTileIndex)
            {
                var from = ring[previousTileIndex % ring.Length];
                var to = ring[tileIndex % ring.Length];
                RecordStep(player, plugin, from, to);
                previousTileIndex = tileIndex;
            }

            var checkPosition = player.Position;
            var args = new SpeedHackCheckEventArgs();
            await plugin.WalkCheatCheckAsync(player, new[] { new WalkingStep { From = checkPosition, To = checkPosition, Direction = Direction.East } }, args).ConfigureAwait(false);
            severities.Add(args.Severity);
            timeProvider.Advance(TimeSpan.FromMilliseconds(WalkPacketIntervalMs));
        }

        return severities;
    }

    /// <summary>
    /// Creates the tiles of a square ring, which is walked around in circles.
    /// </summary>
    private static Point[] CreateRing(Point origin, int size)
    {
        var ring = new List<Point>();
        for (var i = 0; i < size - 1; i++)
        {
            ring.Add(new Point((byte)(origin.X + i), origin.Y));
        }

        for (var i = 0; i < size - 1; i++)
        {
            ring.Add(new Point((byte)(origin.X + size - 1), (byte)(origin.Y + i)));
        }

        for (var i = 0; i < size - 1; i++)
        {
            ring.Add(new Point((byte)(origin.X + size - 1 - i), (byte)(origin.Y + size - 1)));
        }

        for (var i = 0; i < size - 1; i++)
        {
            ring.Add(new Point(origin.X, (byte)(origin.Y + size - 1 - i)));
        }

        return ring.ToArray();
    }

    /// <summary>
    /// Creates a player whose executed steps exceed the walk budget, and lets it request one more walk.
    /// </summary>
    /// <param name="configure">Adjusts the configuration, e.g. to select the severity of the violation.</param>
    /// <returns>The player, the speed hack plugin and the target of the requested walk.</returns>
    private static async ValueTask<(Player Player, SpeedHackDetectPlugIn PlugIn, Point Target)> RequestWalkAfterViolationAsync(Action<SpeedHackDetectConfiguration> configure)
    {
        var (player, plugin, timeProvider) = await CreateStrictWalkPlayerAsync().ConfigureAwait(false);
        configure(plugin.Configuration!);
        player.Position = StartPoint;

        // Executed steps which exceed the one second window, so the next walk request is a violation.
        ExceedWalkWindowWithRecordedSteps(player, plugin, timeProvider);

        var from = player.Position;
        var target = new Point((byte)(from.X + 1), from.Y);
        WalkingStep[] steps = [new() { From = from, To = target, Direction = Direction.East }];
        await player.WalkToAsync(target, steps).ConfigureAwait(false);
        return (player, plugin, target);
    }

    private static async ValueTask<bool> WaitUntilAsync(Func<bool> condition, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            if (condition())
            {
                return true;
            }

            await Task.Delay(10).ConfigureAwait(false);
        }

        return condition();
    }

    private static void ExceedWalkWindowWithRecordedSteps(Player player, SpeedHackDetectPlugIn plugin, ManualTimeProvider timeProvider, int stepCount = 5)
    {
        var recordedFrom = StartPoint;
        for (int i = 0; i < stepCount; i++)
        {
            var recordedTo = new Point((byte)(recordedFrom.X + 1), recordedFrom.Y);
            RecordStep(player, plugin, recordedFrom, recordedTo);
            recordedFrom = recordedTo;
        }

        timeProvider.Advance(TimeSpan.FromMilliseconds(10));
    }

    private static async Task SeedMovementStateWithShortWalkAsync(Player player)
    {
        var firstTarget = new Point((byte)(StartPoint.X + 2), StartPoint.Y);
        WalkingStep[] firstSteps =
        [
            new() { From = StartPoint, To = new Point((byte)(StartPoint.X + 1), StartPoint.Y), Direction = Direction.East },
            new() { From = new Point((byte)(StartPoint.X + 1), StartPoint.Y), To = firstTarget, Direction = Direction.East },
        ];
        await player.WalkToAsync(firstTarget, firstSteps).ConfigureAwait(false);
        await player.StopWalkingAsync().ConfigureAwait(false);
    }

    private static async Task PerformRapidWalkRequestsAsync(Player player, int count = 12)
    {
        for (int i = 0; i < count; i++)
        {
            var nextFrom = new Point((byte)(StartPoint.X + (i * 2)), StartPoint.Y);
            var nextTo = new Point((byte)(StartPoint.X + (i * 2) + 2), StartPoint.Y);

            player.Position = nextFrom;
            player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()?.ResetAlertDebounce(player);

            WalkingStep[] steps = [new() { From = nextFrom, To = nextTo, Direction = Direction.East }];

            await player.WalkToAsync(nextTo, steps).ConfigureAwait(false);
        }
    }

    private static async Task<Skill> SetupExplicitAreaSkillAsync(Player player, short skillNumber = 1, short range = 0, List<AttributeRequirement>? consumeRequirements = null)
    {
        var skillMock = new Mock<Skill>();
        skillMock.SetupAllProperties();
        skillMock.Setup(s => s.Requirements).Returns(new List<AttributeRequirement>());
        skillMock.Setup(s => s.AttributeRelationships).Returns(new List<AttributeRelationship>());
        skillMock.Setup(s => s.ConsumeRequirements).Returns(consumeRequirements ?? new List<AttributeRequirement>());
        var skill = skillMock.Object;
        skill.Number = skillNumber;
        skill.SkillType = SkillType.AreaSkillExplicitHits;
        skill.Range = range;
        await player.SkillList!.AddLearnedSkillAsync(skill).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        return skill;
    }

    private static async ValueTask<(Player Player, SpeedHackDetectPlugIn PlugIn, ManualTimeProvider TimeProvider)> CreatePlayerWithManualClockAsync(DateTime? initialTime = null)
    {
        var timeProvider = new ManualTimeProvider(initialTime ?? new DateTime(2026, 10, 12, 0, 0, 0, DateTimeKind.Utc));
        var player = await CreatePlayerWithSpeedAttributesAsync(timeProvider: timeProvider).ConfigureAwait(false);
        var plugin = player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()!;
        return (player, plugin, timeProvider);
    }

    /// <summary>
    /// Counts the walk steps which the server took over from the client, instead of executing them itself.
    /// </summary>
    [System.Runtime.InteropServices.Guid("5D1B3C7E-8F0A-4C52-9E4B-6A1F0C2D7B93")]
    private sealed class RecordingWalkStepPlugIn : IWalkStepPlugIn
    {
        private int _takenOverSteps;

        public int TakenOverSteps => System.Threading.Volatile.Read(ref this._takenOverSteps);

        public void WalkStepPerformed(Player player, WalkingStep step, TimeSpan stepDelay)
        {
            if (step.Direction == Direction.Undefined)
            {
                System.Threading.Interlocked.Increment(ref this._takenOverSteps);
            }
        }
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

    private sealed class ManualTimeProvider(DateTime startUtc) : TimeProvider
    {
        private DateTimeOffset _utcNow = new(startUtc);
        private long _timestamp;

        /// <inheritdoc />
        public override long TimestampFrequency => TimeSpan.TicksPerSecond;

        public void Advance(TimeSpan delta)
        {
            this._utcNow = this._utcNow.Add(delta);
            this._timestamp += delta.Ticks;
        }

        public void JumpUtc(TimeSpan delta) => this._utcNow = this._utcNow.Add(delta);

        /// <inheritdoc />
        public override DateTimeOffset GetUtcNow() => this._utcNow;

        /// <inheritdoc />
        public override long GetTimestamp() => this._timestamp;
    }
}
