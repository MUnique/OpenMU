// <copyright file="ConcurrentHitApplicationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.GameLogic.PlayerActions.Party;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.GameLogic.Views.Login;
using MUnique.OpenMU.GameLogic.Views.Party;
using MUnique.OpenMU.GameLogic.Views.Quest;
using MUnique.OpenMU.GameLogic.Views.Trade;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Damage conservation through the normal target entry points, including asynchronous notifications.
/// </summary>
[TestFixture]
public class ConcurrentHitApplicationTests
{
    /// <summary>Concurrent one-point hits must not overwrite one another.</summary>
    [Test]
    public async Task PlayerConservesHealthAsync()
    {
        var target = await CreatePlayerAsync(1000, slowHealth: true).ConfigureAwait(false);
        await ConcurrentAsync(64, _ => target.ApplyPoisonDamageAsync(target, 1)).ConfigureAwait(false);
        Assert.That(target.Attributes![Stats.CurrentHealth], Is.EqualTo(936));
    }

    /// <summary>Shield overflow is part of the same transaction as health damage.</summary>
    [Test]
    public async Task PlayerConservesShieldOverflowAsync()
    {
        var target = await CreatePlayerAsync(100, 30, true).ConfigureAwait(false);
        Add(target, Stats.ShieldRatio, 0.65f);
        Assert.That(target.GetHitInfo(31, DamageAttributes.Reflected, target), Is.EqualTo(new HitInfo(10, 20, DamageAttributes.Reflected)));
        await ConcurrentAsync(2, _ => target.ReflectDamageAsync(target, 31)).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(target.Attributes![Stats.CurrentShield], Is.Zero);
            Assert.That(target.Attributes[Stats.CurrentHealth], Is.EqualTo(70));
        });
    }

    /// <summary>Two nonlethal hits together must select exactly one death.</summary>
    [Test]
    public async Task NpcCombinedLethalAsync()
    {
        var seed = await CreatePlayerAsync(1000).ConfigureAwait(false);
        for (var i = 0; i < 200; i++)
        {
            var target = new TestNpc(seed, 10);
            await ConcurrentAsync(2, _ => target.ApplyPoisonDamageAsync(seed, 6)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(target.Health, Is.Zero, $"iteration {i}");
                Assert.That(target.IsAlive, Is.False, $"iteration {i}");
                Assert.That(target.Deaths, Is.EqualTo(1), $"iteration {i}");
            });
        }
    }

    /// <summary>Many lethal hits must not duplicate death callbacks.</summary>
    [Test]
    public async Task NpcIndividuallyLethalAsync()
    {
        var seed = await CreatePlayerAsync(1000).ConfigureAwait(false);
        for (var i = 0; i < 100; i++)
        {
            var target = new TestNpc(seed, 10);
            await ConcurrentAsync(16, _ => target.ApplyBleedingDamageAsync(seed, 10)).ConfigureAwait(false);
            Assert.That(target.Deaths, Is.EqualTo(1), $"iteration {i}");
        }
    }

    /// <summary>A later hit cannot take ownership while the lethal hit's view is waiting.</summary>
    [Test]
    public async Task PlayerLethalAttributionSurvivesDelayedViewAsync()
    {
        var target = await CreatePlayerAsync(10).ConfigureAwait(false);
        var killer = await CreatePlayerAsync(100).ConfigureAwait(false);
        var lateAttacker = await CreatePlayerAsync(100).ConfigureAwait(false);
        killer.Id = 11;
        lateAttacker.Id = 22;
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var calls = 0;
        var deaths = 0;
        target.Died += (_, _) => Interlocked.Increment(ref deaths);
        View<IShowHitPlugIn>(target).Setup(v => v.ShowHitAsync(target, It.IsAny<HitInfo>()))
            .Returns(() =>
            {
                if (Interlocked.Increment(ref calls) != 1)
                {
                    return ValueTask.CompletedTask;
                }

                entered.SetResult();
                return new ValueTask(release.Task);
            });
        var lethal = target.ApplyPoisonDamageAsync(killer, 10).AsTask();
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        try
        {
            var ownedDeath = target.LastDeath;
            Assert.That(target.Attributes![Stats.CurrentHealth], Is.Zero);
            target.Attributes[Stats.CurrentHealth] = 5; // Recovery cannot reopen a reserved death.
            await target.ApplyPoisonDamageAsync(lateAttacker, 7).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(target.Attributes[Stats.CurrentHealth], Is.EqualTo(5));
                Assert.That(target.LastDeath?.KillerId, Is.EqualTo(killer.Id));
            });
            release.TrySetResult();
            await lethal.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(deaths, Is.EqualTo(1));
                Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
                Assert.That(target.IsAlive, Is.False);
                Assert.That(target.LastDeath, Is.SameAs(ownedDeath));
                Assert.That(target.LastDeath?.KillerId, Is.EqualTo(killer.Id));
            });
            View<IObjectGotKilledPlugIn>(target).Verify(v => v.ObjectGotKilledAsync(target, killer), Times.Once);
        }
        finally
        {
            release.TrySetResult();
            await lethal.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
            await killer.DisposeAsync().ConfigureAwait(false);
            await lateAttacker.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>A reserved lethal hit survives either ordering of a real map-change handshake.</summary>
    /// <param name="arrivalFirst">Whether arrival precedes the original death continuation.</param>
    /// <param name="source">The damage source, including a hit on the victim's own packet path.</param>
    /// <param name="heal">Whether health is restored while the lethal view is paused.</param>
    [TestCase(false, "poison", false)]
    [TestCase(true, "poison", false)]
    [TestCase(false, "reflection", false)]
    [TestCase(true, "reflection", false)]
    [TestCase(false, "self-reflection", false)]
    [TestCase(true, "self-reflection", false)]
    [TestCase(false, "poison", true)]
    [TestCase(true, "poison", true)]
    [TestCase(false, "reflection", true)]
    [TestCase(true, "reflection", true)]
    [TestCase(false, "self-reflection", true)]
    [TestCase(true, "self-reflection", true)]
    public async Task LethalHitDuringMapChangeCannotStrandLifeAsync(bool arrivalFirst, string source, bool heal)
    {
        var target = await CreatePlayerAsync(10).ConfigureAwait(false);
        var other = await CreatePlayerAsync(100, sameWorldAs: target).ConfigureAwait(false);
        other.Id = 11;
        var killer = source == "self-reflection" ? target : other;
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var deaths = 0;
        target.Died += (_, _) => Interlocked.Increment(ref deaths);
        View<IShowHitPlugIn>(target).Setup(v => v.ShowHitAsync(target, It.IsAny<HitInfo>()))
            .Returns(() => DelayViewAsync(entered, release, false));
        var lethal = source == "poison" ? target.ApplyPoisonDamageAsync(killer, 10).AsTask() : target.ReflectDamageAsync(killer, 10).AsTask();
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            var ownedDeath = target.LastDeath;
            if (heal)
            {
                target.Attributes![Stats.CurrentHealth] = 5;
            }

            await WarpAsync(target).ConfigureAwait(false);
            Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.ChangingMap));
            if (arrivalFirst)
            {
                await target.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);
                Assert.That(target.IsAlive, Is.False, "Arrival must not revive the reserved life.");
            }

            release.TrySetResult();
            // Must finish without a future ready packet, even for self/reflected serialized handlers.
            await lethal.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (!arrivalFirst)
            {
                Assert.That(target.IsAlive, Is.False);
                Assert.That(deaths, Is.Zero);
                await target.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);
            }

            await target.ApplyPoisonDamageAsync(other, 1).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(target.Attributes![Stats.CurrentHealth], Is.EqualTo(heal ? 5 : 0));
                Assert.That(deaths, Is.EqualTo(1));
                Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
                Assert.That(target.IsAlive, Is.False);
                Assert.That(target.LastDeath, Is.SameAs(ownedDeath));
                Assert.That(target.LastDeath?.KillerId, Is.EqualTo(killer.Id));
            });
            View<IObjectGotKilledPlugIn>(target).Verify(v => v.ObjectGotKilledAsync(target, killer), Times.Once);
            await target.RespawnAfterDeathAsync(GateOf(target)).ConfigureAwait(false);
            await target.CompleteMapChangeDeathAsync().ConfigureAwait(false);
            Assert.That(target.IsAlive, Is.True);
            Assert.That(deaths, Is.EqualTo(1), "An old owner must not kill the respawned life.");
        }
        finally
        {
            release.TrySetResult();
            await lethal.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
            await other.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Only an actual respawn restores the next life, on either map and packet path.</summary>
    /// <param name="crossMap">Whether the respawn changes maps.</param>
    /// <param name="useRespawnPlugIn">Whether the client has the legacy explicit respawn view.</param>
    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public async Task ActualRespawnClearsLethalReservationAsync(bool crossMap, bool useRespawnPlugIn)
    {
        var target = await CreatePlayerAsync(10, useRespawnPlugIn: useRespawnPlugIn).ConfigureAwait(false);
        var deaths = 0;
        target.Died += (_, _) => Interlocked.Increment(ref deaths);
        try
        {
            await target.ApplyPoisonDamageAsync(target, 10).ConfigureAwait(false);
            Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
            var gate = GateOf(target);
            if (crossMap)
            {
                var original = gate.Map!;
                var destination = new Mock<GameMapDefinition>();
                destination.SetupAllProperties();
                destination.Setup(m => m.DropItemGroups).Returns(original.DropItemGroups);
                destination.Setup(m => m.MonsterSpawns).Returns(original.MonsterSpawns);
                destination.Setup(m => m.ExitGates).Returns(original.ExitGates);
                destination.Object.Number = 1;
                destination.Object.TerrainData = original.TerrainData;
                target.GameContext.Configuration.Maps.Add(destination.Object);
                gate.Map = destination.Object;
            }

            var restoredHealth = target.Attributes![Stats.MaximumHealth];
            await target.RespawnAfterDeathAsync(gate).ConfigureAwait(false);
            if (!useRespawnPlugIn)
            {
                Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.ChangingMap));
                Assert.That(target.IsAlive, Is.False);
                await target.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);
            }

            await target.CompleteMapChangeDeathAsync().ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(target.IsAlive, Is.True);
                Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(target.CurrentMap!.Definition, Is.SameAs(gate.Map));
                Assert.That(target.Attributes[Stats.CurrentHealth], Is.EqualTo(restoredHealth));
                Assert.That(deaths, Is.EqualTo(1));
            });
            await target.ApplyPoisonDamageAsync(target, 1).ConfigureAwait(false);
            Assert.That(target.Attributes[Stats.CurrentHealth], Is.EqualTo(restoredHealth - 1));
        }
        finally
        {
            await target.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Unowned zero HP remains an eligible life after real death cancellation and normal entry, without healing it.</summary>
    /// <param name="entry">Initial character entry, same-session reselection, or a new connection after disconnect.</param>
    [TestCase("initial")]
    [TestCase("logout")]
    [TestCase("disconnect")]
    public async Task ZeroHealthNewLifeAcceptsOneLethalHitAsync(string entry)
    {
        var original = await CreatePlayerAsync(entry == "initial" ? 0 : 10).ConfigureAwait(false);
        Player target = original;
        var killer = await CreatePlayerAsync(100, sameWorldAs: target).ConfigureAwait(false);
        DeathInformation? previousDeath = null;
        try
        {
            if (entry != "initial")
            {
                var oldMap = target.CurrentMap!;
                var safe = new Mock<GameMapDefinition>();
                safe.SetupAllProperties();
                safe.Setup(m => m.DropItemGroups).Returns(oldMap.Definition.DropItemGroups);
                safe.Setup(m => m.MonsterSpawns).Returns(oldMap.Definition.MonsterSpawns);
                var gates = new List<ExitGate>();
                safe.Setup(m => m.ExitGates).Returns(gates);
                safe.Object.Number = 1;
                safe.Object.TerrainData = oldMap.Definition.TerrainData;
                gates.Add(new ExitGate { Map = safe.Object, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1, IsSpawnGate = true });
                target.GameContext.Configuration.Maps.Add(safe.Object);
                oldMap.Definition.SafezoneMap = safe.Object;
                var character = target.SelectedCharacter!;
                character.Name = "ZeroLife";
                var account = target.Account!;
                Mock.Get(account).Setup(a => a.Characters).Returns(new List<Character> { character });
                await killer.RunPersistenceExclusiveAsync(() => target.ApplyPoisonDamageAsync(killer, 10)).ConfigureAwait(false);
                previousDeath = target.LastDeath;
                if (entry == "logout")
                {
                    await target.RunPersistenceExclusiveAsync(() => new LogoutAction().LogoutAsync(target, LogoutType.BackToCharacterSelection)).ConfigureAwait(false);
                }
                else
                {
                    await target.DisconnectAsync().ConfigureAwait(false);
                    target = new TestPlayer(original.GameContext) { Account = account };
                    await target.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
                    await target.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
                }

                Assert.That(character.Attributes.Single(a => a.Definition == Stats.CurrentHealth).Value, Is.Zero, "Departure cancels the respawn timer before maximum-health restoration.");
                await target.RunPersistenceExclusiveAsync(() => new RequestCharacterListAction().RequestCharacterListAsync(target)).ConfigureAwait(false);
                await target.RunPersistenceExclusiveAsync(() => new SelectCharacterAction().SelectCharacterAsync(target, character.Name)).ConfigureAwait(false);
                await killer.WarpToAsync(GateOf(target)).ConfigureAwait(false);
                await killer.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);
            }

            Assert.That(target.IsAlive, Is.True);
            Assert.That(target.Attributes![Stats.CurrentHealth], Is.Zero, "Test must not heal the new life to make the hit work.");
            var deaths = 0;
            target.Died += (_, _) => Interlocked.Increment(ref deaths);
            View<IShowHitPlugIn>(target).Invocations.Clear();
            View<IObjectGotKilledPlugIn>(target).Invocations.Clear();
            await killer.RunPersistenceExclusiveAsync(() => target.ApplyPoisonDamageAsync(killer, 1)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(target.IsAlive, Is.False);
                Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
                Assert.That(deaths, Is.EqualTo(1));
                Assert.That(target.LastDeath, Is.Not.Null.And.Not.SameAs(previousDeath));
                Assert.That(target.LastDeath?.KillerId, Is.EqualTo(killer.Id));
            });
            View<IShowHitPlugIn>(target).Verify(v => v.ShowHitAsync(target, It.IsAny<HitInfo>()), Times.Once);
            View<IObjectGotKilledPlugIn>(target).Verify(v => v.ObjectGotKilledAsync(target, killer), Times.Once);
            var ownedDeath = target.LastDeath;
            await target.ApplyPoisonDamageAsync(target, 1).ConfigureAwait(false);
            Assert.That(target.LastDeath, Is.SameAs(ownedDeath));
            Assert.That(deaths, Is.EqualTo(1));
        }
        finally
        {
            await target.DisposeAsync().ConfigureAwait(false);
            if (!ReferenceEquals(target, original))
            {
                await original.DisposeAsync().ConfigureAwait(false);
            }

            await killer.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Initialize honors null versus explicit zero override; each fresh zero life still needs one death owner.</summary>
    /// <param name="definitionHealth">Configured monster health.</param>
    /// <param name="healthOverride">Spawn-area override, where null means no override.</param>
    [TestCase(0, null)]
    [TestCase(10, 0)]
    [TestCase(10, null)]
    [TestCase(10, 3)]
    public async Task InitializedNpcHealthSelectsOneDeathAsync(int definitionHealth, int? healthOverride)
    {
        var seed = await CreatePlayerAsync(100).ConfigureAwait(false);
        using var npc = new InitializedNpc(seed, definitionHealth, healthOverride);
        try
        {
            var initialHealth = healthOverride ?? definitionHealth;
            Assert.Multiple(() =>
            {
                Assert.That(npc.Health, Is.EqualTo(initialHealth));
                Assert.That(npc.Attributes[Stats.MaximumHealth], Is.EqualTo(initialHealth));
                Assert.That(npc.IsAlive, Is.True);
            });
            await ConcurrentAsync(32, _ => npc.ApplyPoisonDamageAsync(seed, 20)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(npc.Health, Is.Zero);
                Assert.That(npc.IsAlive, Is.False);
                Assert.That(npc.Deaths, Is.EqualTo(1));
                Assert.That(npc.Restore(10, 100), Is.Zero, "Atomic restoration must not reopen an owned death.");
            });
            npc.Initialize();
            await ConcurrentAsync(32, _ => npc.ApplyBleedingDamageAsync(seed, 20)).ConfigureAwait(false);
            Assert.That(npc.Deaths, Is.EqualTo(2), "Actual initialization establishes a fresh life, including configured zero.");
        }
        finally
        {
            await seed.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Public assignments cannot remove the claim bit; full-range HP/damage and fresh-zero restoration remain atomic.</summary>
    /// <param name="assignedHealth">The public value written before and after competing lethal hits.</param>
    [TestCase(-7)]
    [TestCase(0)]
    [TestCase(100)]
    [TestCase(int.MaxValue)]
    public async Task NpcDeathReservationSurvivesPublicHealthWritesAsync(int assignedHealth)
    {
        var seed = await CreatePlayerAsync(100).ConfigureAwait(false);
        using var npc = new InitializedNpc(seed, 0, null);
        try
        {
            await ConcurrentAsync(64, async i =>
            {
                var operation = (uint)i % 3;
                if (operation == 0)
                {
                    npc.Health = assignedHealth;
                }
                else if (operation == 1)
                {
                    npc.Restore(10, int.MaxValue);
                }
                else
                {
                    await npc.ApplyPoisonDamageAsync(seed, uint.MaxValue).ConfigureAwait(false);
                }
            }).ConfigureAwait(false);
            npc.Health = assignedHealth;
            Assert.Multiple(() =>
            {
                Assert.That(npc.Health, Is.EqualTo(Math.Max(assignedHealth, 0)));
                Assert.That(npc.Health, Is.GreaterThanOrEqualTo(0));
                Assert.That(npc.Deaths, Is.EqualTo(1));
                Assert.That(npc.Killer, Is.SameAs(seed));
                Assert.That(npc.IsAlive, Is.False);
                Assert.That(npc.Restore(10, int.MaxValue), Is.Zero);
            });
            await npc.ApplyPoisonDamageAsync(seed, uint.MaxValue).ConfigureAwait(false);
            Assert.That(npc.Deaths, Is.EqualTo(1));
            npc.Initialize();
            Assert.That(npc.Restore(5, 10), Is.EqualTo(5));
            Assert.That(npc.Health, Is.EqualTo(5));
            npc.Health = 0;
            await ConcurrentAsync(32, _ => npc.ApplyPoisonDamageAsync(seed, 0)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(npc.Health, Is.Zero);
                Assert.That(npc.Deaths, Is.EqualTo(2), "Baseline damage>=Health kills fresh zero even with zero damage.");
            });
            npc.Initialize();
            npc.Health = int.MaxValue;
            await ConcurrentAsync(32, _ => npc.ApplyPoisonDamageAsync(seed, uint.MaxValue)).ConfigureAwait(false);
            Assert.That(npc.Health, Is.Zero);
            Assert.That(npc.Deaths, Is.EqualTo(3));
        }
        finally
        {
            await seed.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>The actual delayed NPC respawn reinitializes a configured-zero life without reopening the old claim.</summary>
    [Test]
    public async Task ZeroHealthNpcActualRespawnCreatesOneNewLifeAsync()
    {
        var seed = await CreatePlayerAsync(100).ConfigureAwait(false);
        using var npc = new TestNpc(seed, 0, runDeathLifecycle: true);
        try
        {
            await seed.CurrentMap!.AddAsync(npc).ConfigureAwait(false);
            await ConcurrentAsync(32, _ => npc.ApplyPoisonDamageAsync(seed, 0)).ConfigureAwait(false);
            await npc.Respawned.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(npc.IsAlive, Is.True);
            Assert.That(npc.Health, Is.Zero);
            Assert.That(npc.Deaths, Is.EqualTo(1));
            await ConcurrentAsync(32, _ => npc.ApplyPoisonDamageAsync(seed, 1)).ConfigureAwait(false);
            Assert.That(npc.Deaths, Is.EqualTo(2));
        }
        finally
        {
            await seed.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>An externally implemented name getter cannot expose reservation without its immutable death identity.</summary>
    [Test]
    public async Task PlacementSeesCoherentLethalPublicationAsync()
    {
        var target = await CreatePlayerAsync(10).ConfigureAwait(false);
        var attacker = new Mock<IAttacker>();
        attacker.Setup(a => a.Id).Returns((ushort)7);
        var named = attacker.As<IPartyMember>();
        named.Setup(a => a.ViewPlugIns).Returns(new MockViewPlugInContainer());
        named.Setup(a => a.Logger).Returns(NullLogger.Instance);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var release = new ManualResetEventSlim();
        var nameReadUnderHit = false;
        named.Setup(a => a.Name).Returns(() =>
        {
            nameReadUnderHit |= target.IsApplyingHitOnCurrentThread;
            entered.TrySetResult();
            if (!release.Wait(TimeSpan.FromSeconds(5)))
            {
                throw new TimeoutException("Name read was not released.");
            }

            return "Original";
        });
        var hit = Task.Run(async () => await target.ApplyPoisonDamageAsync(attacker.Object, 10).ConfigureAwait(false));
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            var snapshot = target.GetPlacementDeathSnapshot();
            Assert.Multiple(() =>
            {
                Assert.That(snapshot.Reserved, Is.False, "No lethal commit may precede the external name getter.");
                Assert.That(snapshot.Death, Is.Null);
                Assert.That(nameReadUnderHit, Is.False);
            });
            await target.RespawnAtAsync(GateOf(target)).AsTask().WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(target.IsAlive, Is.True, "Metadata capture is outside exclusion and precedes the lethal placement snapshot.");
            release.Set();
            await hit.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            snapshot = target.GetPlacementDeathSnapshot();
            Assert.Multiple(() =>
            {
                Assert.That(snapshot.Reserved, Is.True);
                Assert.That(snapshot.Death, Is.SameAs(target.LastDeath));
                Assert.That(snapshot.Death?.KillerId, Is.EqualTo(7));
                Assert.That(snapshot.Death?.KillerName, Is.EqualTo("Original"));
            });
        }
        finally
        {
            release.Set();
            await hit.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Initial zero health is not a reserved lethal hit.</summary>
    [Test]
    public async Task InitialZeroHealthCharacterEntersAliveAsync()
    {
        var target = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        try
        {
            Assert.Multiple(() =>
            {
                Assert.That(target.Attributes![Stats.CurrentHealth], Is.Zero);
                Assert.That(target.IsAlive, Is.True);
                Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(target.LastDeath, Is.Null);
            });
        }
        finally
        {
            await target.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Instant hazards preserve the first lethal owner and can finish through ordinary arrival.</summary>
    /// <param name="reserved">Whether an ordinary lethal hit already owns this life.</param>
    /// <param name="warp">Whether a map change overlaps the instant kill.</param>
    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public async Task InstantKillPreservesFirstDeathAsync(bool reserved, bool warp)
    {
        var target = await CreatePlayerAsync(10).ConfigureAwait(false);
        var killer = await CreatePlayerAsync(100, sameWorldAs: target).ConfigureAwait(false);
        killer.Id = 11;
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var deaths = 0;
        target.Died += (_, _) => Interlocked.Increment(ref deaths);
        View<IShowHitPlugIn>(target).Setup(v => v.ShowHitAsync(target, It.IsAny<HitInfo>())).Returns(() => DelayViewAsync(entered, release, false));
        var original = reserved ? target.ApplyPoisonDamageAsync(killer, 10).AsTask() : Task.CompletedTask;
        try
        {
            if (reserved)
            {
                await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }

            var owned = target.LastDeath;
            if (warp)
            {
                await WarpAsync(target).ConfigureAwait(false);
            }

            await target.KillInstantlyAsync().ConfigureAwait(false);
            var instantDeath = target.LastDeath;
            release.TrySetResult();
            await original.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (warp)
            {
                await target.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);
            }

            Assert.That(deaths, Is.EqualTo(1));
            Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
            await target.KillInstantlyAsync().ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(deaths, Is.EqualTo(1));
                Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
                Assert.That(target.IsAlive, Is.False);
                Assert.That(target.LastDeath?.KillerId, Is.EqualTo(reserved ? 11 : 0));
                Assert.That(target.LastDeath, Is.SameAs(reserved ? owned : instantDeath));
                if (reserved)
                {
                    Assert.That(target.LastDeath, Is.SameAs(owned));
                }
            });
            View<IObjectGotKilledPlugIn>(target).Verify(v => v.ObjectGotKilledAsync(target, reserved ? killer : null), Times.Once);
        }
        finally
        {
            release.TrySetResult();
            await original.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
            await killer.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>The real three-second respawn survives a dead-player warp, before or after arrival.</summary>
    /// <param name="arrivalDelay">Time spent off map before the real client-ready packet.</param>
    [TestCase(100)]
    [TestCase(3300)]
    public async Task DeadWarpRetainsElapsedRespawnAsync(int arrivalDelay)
    {
        var target = await CreatePlayerAsync(10).ConfigureAwait(false);
        var spawnGate = GateOf(target);
        spawnGate.IsSpawnGate = true;
        target.CurrentMap!.Definition.ExitGates.Add(spawnGate);
        var respawned = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var respawns = 0;
        var deaths = 0;
        target.Died += (_, _) => Interlocked.Increment(ref deaths);
        View<IRespawnAfterDeathPlugIn>(target).Setup(v => v.RespawnAsync()).Returns(() =>
        {
            Interlocked.Increment(ref respawns);
            respawned.TrySetResult();
            return ValueTask.CompletedTask;
        });
        try
        {
            await target.ApplyPoisonDamageAsync(target, 10).ConfigureAwait(false);
            var started = Stopwatch.StartNew();
            await WarpAsync(target).ConfigureAwait(false);
            await Task.Delay(arrivalDelay).ConfigureAwait(false);
            Assert.That(respawns, Is.Zero, "Generic relocation must not respawn/heal the victim.");
            await target.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);
            await respawned.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            // The view callback precedes map insertion; complete the actual timer operation.
            await Task.Delay(50).ConfigureAwait(false);
            await target.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(started.Elapsed, Is.GreaterThanOrEqualTo(TimeSpan.FromMilliseconds(2900)));
                Assert.That(target.IsAlive, Is.True);
                Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(target.Attributes![Stats.CurrentHealth], Is.EqualTo(target.Attributes[Stats.MaximumHealth]));
                Assert.That(deaths, Is.EqualTo(1));
                Assert.That(respawns, Is.EqualTo(1));
            });
        }
        finally
        {
            await target.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Teardown invalidates pending timers and every awaited restoration continuation.</summary>
    /// <param name="phase">The real lifecycle boundary held across teardown.</param>
    /// <param name="dispose">Whether teardown is direct disposal rather than disconnect.</param>
    [TestCase("due", false)]
    [TestCase("due", true)]
    [TestCase("death-view", false)]
    [TestCase("death-view", true)]
    [TestCase("selection", false)]
    [TestCase("selection", true)]
    [TestCase("placement", false)]
    [TestCase("placement", true)]
    public async Task DeathRestorationCannotOutliveTeardownAsync(string phase, bool dispose)
    {
        var target = await CreatePlayerAsync(10).ConfigureAwait(false);
        var map = target.CurrentMap!;
        var gate = GateOf(target);
        gate.IsSpawnGate = true;
        map.Definition.ExitGates.Add(gate);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        async ValueTask PauseAsync()
        {
            entered.TrySetResult();
            await release.Task.ConfigureAwait(false);
        }

        var respawns = 0;
        View<IRespawnAfterDeathPlugIn>(target).Setup(v => v.RespawnAsync()).Returns(() =>
        {
            Interlocked.Increment(ref respawns);
            return phase == "placement" ? PauseAsync() : ValueTask.CompletedTask;
        });
        if (phase == "death-view")
        {
            View<IObjectGotKilledPlugIn>(target).Setup(v => v.ObjectGotKilledAsync(target, It.IsAny<IAttacker>())).Returns(PauseAsync);
        }
        else if (phase == "selection")
        {
            target.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPlayerSpawnGateSelectionPlugIn>(new PausingSpawnGatePlugIn(target, gate, PauseAsync));
        }
        else
        {
            // The due and placement phases use the respawn view configured above.
        }

        Task work = Task.CompletedTask;
        try
        {
            work = target.KillInstantlyAsync().AsTask();
            if (phase == "due")
            {
                await work.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                var destination = new Mock<GameMapDefinition>();
                destination.SetupAllProperties();
                destination.Setup(m => m.DropItemGroups).Returns(map.Definition.DropItemGroups);
                destination.Setup(m => m.MonsterSpawns).Returns(map.Definition.MonsterSpawns);
                var gates = new List<ExitGate>();
                destination.Setup(m => m.ExitGates).Returns(gates);
                destination.Object.Number = 1;
                destination.Object.TerrainData = map.Definition.TerrainData;
                gates.Add(new ExitGate { Map = destination.Object, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1, IsSpawnGate = true });
                target.GameContext.Configuration.Maps.Add(destination.Object);
                await new WarpAction().WarpToAsync(target, new WarpInfo
                {
                    Gate = new ExitGate { Map = destination.Object, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 },
                }).ConfigureAwait(false);
                await Task.Delay(3300).ConfigureAwait(false);
            }
            else
            {
                if (phase != "death-view")
                {
                    await work.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                    work = target.RespawnAfterDeathAsync(phase == "selection" ? null : gate).AsTask();
                }

                await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }

            if (dispose)
            {
                await target.DisposeAsync().ConfigureAwait(false);
            }
            else
            {
                await target.DisconnectAsync().ConfigureAwait(false);
            }

            release.TrySetResult();
            await work.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.CompleteMapChangeDeathAsync().ConfigureAwait(false);
            await target.RespawnAfterDeathAsync(gate).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(target.IsAlive, Is.False);
                Assert.That(respawns, Is.EqualTo(phase == "placement" ? 1 : 0));
                Assert.That(map.GetPlayers(), Does.Not.Contain(target));
            });
        }
        finally
        {
            release.TrySetResult();
            await work.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>An actual duel respawn restores the living partner despite its temporary non-alive flag.</summary>
    [Test]
    public async Task ActualDeathRespawnRestoresLivingDuelPartnerAsync()
    {
        var target = await CreatePlayerAsync(10).ConfigureAwait(false);
        var peer = await CreatePlayerAsync(50, sameWorldAs: target).ConfigureAwait(false);
        var gate = GateOf(target);
        var room = new DuelRoom(new DuelArea { FirstPlayerGate = gate, SecondPlayerGate = gate }, target, peer) { State = DuelState.DuelStarted };
        target.DuelRoom = room;
        peer.DuelRoom = room;
        try
        {
            await target.ApplyPoisonDamageAsync(peer, 10).ConfigureAwait(false);
            await target.RespawnAfterDeathAsync(gate).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(target.IsAlive, Is.True);
                Assert.That(peer.IsAlive, Is.True);
                Assert.That(peer.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(peer.Attributes![Stats.CurrentHealth], Is.EqualTo(peer.Attributes[Stats.MaximumHealth]));
                Assert.That(room.State, Is.EqualTo(DuelState.DuelStarted));
            });
            View<IRespawnAfterDeathPlugIn>(peer).Verify(v => v.RespawnAsync(), Times.Once);
        }
        finally
        {
            room.State = DuelState.Undefined;
            await room.DisposeAsync().ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Actual duel reset cannot mark a partner nonalive when its existing legacy arrival owns placement.</summary>
    /// <param name="overlap">Whether the partner's own death restoration is inside real map-add completion.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task DuelResetCannotInvalidatePartnerOwnedArrivalAsync(bool overlap)
    {
        var target = await CreatePlayerAsync(10).ConfigureAwait(false);
        var peer = await CreatePlayerAsync(overlap ? 10 : 50, sameWorldAs: target).ConfigureAwait(false);
        var map = target.CurrentMap!;
        var gate = GateOf(target);
        gate.IsSpawnGate = true;
        map.Definition.ExitGates.Add(gate);
        var safe = new Mock<GameMapDefinition>();
        safe.SetupAllProperties();
        safe.Setup(m => m.DropItemGroups).Returns(map.Definition.DropItemGroups);
        safe.Setup(m => m.MonsterSpawns).Returns(map.Definition.MonsterSpawns);
        safe.Setup(m => m.ExitGates).Returns(new List<ExitGate>());
        safe.Object.Number = 1;
        safe.Object.TerrainData = map.Definition.TerrainData;
        safe.Object.ExitGates.Add(new ExitGate { Map = safe.Object, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1, IsSpawnGate = true });
        target.GameContext.Configuration.Maps.Add(safe.Object);
        map.Definition.SafezoneMap = safe.Object;
        var configuration = new Mock<DuelConfiguration>();
        configuration.Object.MaximumScore = 10;
        configuration.Setup(c => c.DuelAreas).Returns(new List<DuelArea> { new() { Index = 0, FirstPlayerGate = gate, SecondPlayerGate = gate } });
        target.GameContext.Configuration.DuelConfiguration = configuration.Object;
        ((GameContext)target.GameContext).DuelRoomManager = new DuelRoomManager(configuration.Object);
        var room = (await target.GameContext.DuelRoomManager.GetFreeDuelRoomAsync(target, peer).ConfigureAwait(false))!;
        target.DuelRoom = room;
        peer.DuelRoom = room;
        target.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IObjectRemovedFromMapPlugIn>(new MUnique.OpenMU.GameLogic.PlayerActions.Duel.EndDuelWhenLeavingDuelMapPlugIn());
        target.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPlayerSpawnGateSelectionPlugIn>(new DuelSpawnGatePlugIn());
        var duel = room.RunDuelAsync();
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var peerArrival = Task.CompletedTask;
        var held = false;
        map.ObjectAdded += async args =>
        {
            if (overlap && ReferenceEquals(args.Object, peer) && peer.IsAlive && peer.PlayerState.CurrentState == PlayerState.ChangingMap && !held)
            {
                held = true;
                entered.TrySetResult();
                await release.Task.ConfigureAwait(false);
            }
        };
        try
        {
            var started = Stopwatch.StartNew();
            while (room.State != DuelState.DuelStarted && started.Elapsed < TimeSpan.FromSeconds(8))
            {
                await Task.Delay(10).ConfigureAwait(false);
            }

            Assert.That(room.State, Is.EqualTo(DuelState.DuelStarted));
            Assert.That(target.GameContext.DuelRoomManager.GetRoomByIndex(0), Is.SameAs(room));
            if (overlap)
            {
                await target.RunPersistenceExclusiveAsync(() => peer.ApplyPoisonDamageAsync(target, 10)).ConfigureAwait(false);
                await peer.RunPersistenceExclusiveAsync(() => peer.WarpToAsync(gate)).ConfigureAwait(false);
            }

            await peer.RunPersistenceExclusiveAsync(() => target.ApplyPoisonDamageAsync(peer, 10)).ConfigureAwait(false);
            await target.RunPersistenceExclusiveAsync(() => target.WarpToAsync(gate)).ConfigureAwait(false);
            var due = Stopwatch.StartNew();
            var targetDue = false;
            var peerDue = !overlap;
            while (due.Elapsed < TimeSpan.FromSeconds(6))
            {
                await using (var context = await target.PlayerState.TryBeginAdvanceToAsync(target.PlayerState.CurrentState).ConfigureAwait(false))
                {
                    context.Allowed = false;
                    targetDue = PendingDeathRespawn(target) is { Claimed: false };
                }

                if (overlap)
                {
                    await using var context = await peer.PlayerState.TryBeginAdvanceToAsync(peer.PlayerState.CurrentState).ConfigureAwait(false);
                    context.Allowed = false;
                    peerDue = PendingDeathRespawn(peer) is { Claimed: false };
                }

                if (targetDue && peerDue)
                {
                    break;
                }

                await Task.Delay(10).ConfigureAwait(false);
            }

            Assert.That(targetDue && peerDue, Is.True, "The real three-second timers must have handed their original deaths to map arrival.");
            if (overlap)
            {
                peerArrival = Task.Run(async () => await peer.RunPersistenceExclusiveAsync(() => peer.ClientReadyAfterMapChangeAsync()).ConfigureAwait(false));
                await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                Assert.That(peer.IsAlive, Is.True);
                Assert.That(peer.GetPlacementDeathSnapshot().Reserved, Is.False);
            }

            // Ready owns a complete existing due-timer retry, including the real private duel-reset caller.
            await target.RunPersistenceExclusiveAsync(() => target.ClientReadyAfterMapChangeAsync()).ConfigureAwait(false);
            Assert.That(peer.IsAlive, Is.True, "Refusing a competing reset must not invalidate the arrival which already restored liveness.");
            release.TrySetResult();
            await peerArrival.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            var before = peer.Attributes![Stats.CurrentHealth];
            await peer.ApplyPoisonDamageAsync(target, 1).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(peer.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(peer.IsAlive, Is.True);
                Assert.That(peer.Attributes[Stats.CurrentHealth], Is.EqualTo(before - 1), "The selected duelist must still accept damage, not be full-HP/nonalive.");
                Assert.That(room.State, Is.EqualTo(DuelState.DuelStarted));
            });
        }
        finally
        {
            release.TrySetResult();
            await peerArrival.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await room.CancelDuelAsync().ConfigureAwait(false);
            await duel.WaitAsync(TimeSpan.FromSeconds(8)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>A duel reset waiting in normal NPC close cannot heal or invalidate a changed partner/round.</summary>
    /// <param name="change">The real death, selection or room-state change before admitted reset.</param>
    [TestCase("death")]
    [TestCase("reselect")]
    [TestCase("cancel")]
    public async Task DuelResetRefusalPreservesChangedPartnerAsync(string change)
    {
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var release = new ManualResetEventSlim();
        var logger = new Mock<ILogger>();
        logger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        logger.Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback(new InvocationAction(i =>
            {
                if ((LogLevel)i.Arguments[0] == LogLevel.Debug && i.Arguments[2].ToString()!.Contains("closes NPC", StringComparison.Ordinal))
                {
                    entered.TrySetResult();
                    if (!release.Wait(TimeSpan.FromSeconds(8)))
                    {
                        throw new TimeoutException("Duel reset close was not released.");
                    }
                }
            }));
        var factory = new Mock<ILoggerFactory>();
        factory.Setup(f => f.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
        var config = PlayerTestHelper.CreateGameConfiguration();
        var initializer = new MapInitializer(config, NullLogger<MapInitializer>.Instance, NullDropGenerator.Instance, null);
        using var game = new GameContext(config, new InMemoryPersistenceContextProvider(), initializer, factory.Object, new PlugInManager(null, factory.Object, null, null), NullDropGenerator.Instance, new ConfigurationChangeMediator());
        initializer.PlugInManager = game.PlugInManager;
        initializer.PathFinderPool = game.PathFinderPool;
        var target = await PlayerTestHelper.CreatePlayerAsync(game).ConfigureAwait(false);
        var peer = await PlayerTestHelper.CreatePlayerAsync(game).ConfigureAwait(false);
        target.Attributes![Stats.CurrentHealth] = 10;
        peer.Attributes![Stats.CurrentHealth] = 50;
        var character = peer.SelectedCharacter!;
        character.Name = "DuelLife";
        Mock.Get(peer.Account!).Setup(a => a.Characters).Returns(new List<Character> { character });
        var map = target.CurrentMap!;
        var gate = GateOf(target);
        gate.IsSpawnGate = true;
        map.Definition.ExitGates.Add(gate);
        var safe = new Mock<GameMapDefinition>();
        safe.SetupAllProperties();
        safe.Setup(m => m.DropItemGroups).Returns(map.Definition.DropItemGroups);
        safe.Setup(m => m.MonsterSpawns).Returns(map.Definition.MonsterSpawns);
        safe.Setup(m => m.ExitGates).Returns(new List<ExitGate>());
        safe.Object.Number = 1;
        safe.Object.TerrainData = map.Definition.TerrainData;
        safe.Object.ExitGates.Add(new ExitGate { Map = safe.Object, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1, IsSpawnGate = true });
        config.Maps.Add(safe.Object);
        map.Definition.SafezoneMap = safe.Object;
        var duelConfiguration = new Mock<DuelConfiguration>();
        duelConfiguration.Object.MaximumScore = 10;
        duelConfiguration.Setup(c => c.DuelAreas).Returns(new List<DuelArea> { new() { Index = 0, FirstPlayerGate = gate, SecondPlayerGate = gate } });
        config.DuelConfiguration = duelConfiguration.Object;
        game.DuelRoomManager = new DuelRoomManager(duelConfiguration.Object);
        game.PlugInManager.RegisterPlugInAtPlugInPoint<IObjectRemovedFromMapPlugIn>(new MUnique.OpenMU.GameLogic.PlayerActions.Duel.EndDuelWhenLeavingDuelMapPlugIn());
        game.PlugInManager.RegisterPlugInAtPlugInPoint<IPlayerSpawnGateSelectionPlugIn>(new DuelSpawnGatePlugIn());
        var room = (await game.DuelRoomManager.GetFreeDuelRoomAsync(target, peer).ConfigureAwait(false))!;
        target.DuelRoom = room;
        peer.DuelRoom = room;
        var duel = room.RunDuelAsync();
        var work = Task.CompletedTask;
        try
        {
            var started = Stopwatch.StartNew();
            while (room.State != DuelState.DuelStarted && started.Elapsed < TimeSpan.FromSeconds(8))
            {
                await Task.Delay(10).ConfigureAwait(false);
            }

            Assert.That(room.State, Is.EqualTo(DuelState.DuelStarted));
            var definition = new Mock<MonsterDefinition>();
            definition.SetupAllProperties();
            definition.Setup(d => d.ItemCraftings).Returns(new List<MUnique.OpenMU.DataModel.Configuration.ItemCrafting.ItemCrafting>());
            definition.Object.Number = 239;
            definition.Object.NpcWindow = NpcWindow.ChaosMachine;
            var npc = new NonPlayerCharacter(new MonsterSpawnArea(), definition.Object, map);
            await peer.RunPersistenceExclusiveAsync(() => new TalkNpcAction().TalkToNpcAsync(peer, npc)).ConfigureAwait(false);
            await target.ApplyPoisonDamageAsync(peer, 10).ConfigureAwait(false);
            // The real owned restoration entry invokes the private duel-round caller; primary cases use due timers/Ready.
            work = Task.Run(async () => await target.RespawnAfterDeathAsync(gate).ConfigureAwait(false));
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(peer.IsAlive, Is.True, "No temporary departure/liveness mutation before placement admission.");
            var oldAttributes = peer.Attributes;
            DeathInformation? newDeath = null;
            if (change == "death")
            {
                await peer.KillInstantlyAsync().ConfigureAwait(false);
                newDeath = peer.LastDeath;
                Assert.That(peer.GetPlacementDeathSnapshot(), Is.EqualTo((newDeath, true)));
            }
            else
            {
                if (change == "reselect")
                {
                    await peer.RunPersistenceExclusiveAsync(() => new LogoutAction().LogoutAsync(peer, LogoutType.BackToCharacterSelection)).ConfigureAwait(false);
                }
                else
                {
                    await room.CancelDuelAsync().ConfigureAwait(false);
                }

                await duel.WaitAsync(TimeSpan.FromSeconds(8)).ConfigureAwait(false);
                if (change == "reselect")
                {
                    await peer.RunPersistenceExclusiveAsync(() => new RequestCharacterListAction().RequestCharacterListAsync(peer)).ConfigureAwait(false);
                    await peer.RunPersistenceExclusiveAsync(() => new SelectCharacterAction().SelectCharacterAsync(peer, character.Name)).ConfigureAwait(false);
                    Assert.That(peer.Attributes, Is.Not.SameAs(oldAttributes));
                }
                else
                {
                    if (peer.PlayerState.CurrentState == PlayerState.ChangingMap)
                    {
                        await peer.RunPersistenceExclusiveAsync(() => peer.ClientReadyAfterMapChangeAsync()).ConfigureAwait(false);
                    }
                }

                peer.Attributes![Stats.CurrentHealth] = 37;
                Assert.That(peer.IsAlive, Is.True);
            }

            release.Set();
            await work.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(peer.IsPreparingForcedPlacement, Is.False);
                if (change == "death")
                {
                    Assert.That(peer.GetPlacementDeathSnapshot(), Is.EqualTo((newDeath, true)));
                    Assert.That(peer.IsAlive, Is.False);
                    Assert.That(peer.Attributes![Stats.CurrentHealth], Is.Zero);
                }
                else
                {
                    Assert.That(peer.IsAlive, Is.True);
                    Assert.That(peer.Attributes![Stats.CurrentHealth], Is.EqualTo(37), "Rejected stale room/life cannot heal the partner.");
                }
            });
        }
        finally
        {
            release.Set();
            await work.WaitAsync(TimeSpan.FromSeconds(8)).ConfigureAwait(false);
            await room.CancelDuelAsync().ConfigureAwait(false);
            await duel.WaitAsync(TimeSpan.FromSeconds(8)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>A timer already due off-map completes on arrival without restarting its three-second delay.</summary>
    [Test]
    public async Task DueRespawnCompletesOnArrivalWithoutAnotherDelayAsync()
    {
        var target = await CreatePlayerAsync(10).ConfigureAwait(false);
        var gate = GateOf(target);
        gate.IsSpawnGate = true;
        target.CurrentMap!.Definition.ExitGates.Add(gate);
        try
        {
            await target.KillInstantlyAsync().ConfigureAwait(false);
            await WarpAsync(target).ConfigureAwait(false);
            await Task.Delay(3300).ConfigureAwait(false);
            Assert.That(target.IsAlive, Is.False);
            var arrival = Stopwatch.StartNew();
            await target.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(arrival.Elapsed, Is.LessThan(TimeSpan.FromSeconds(1)));
                Assert.That(target.IsAlive, Is.True);
                Assert.That(target.Attributes![Stats.CurrentHealth], Is.EqualTo(target.Attributes[Stats.MaximumHealth]));
            });
            View<IRespawnAfterDeathPlugIn>(target).Verify(v => v.RespawnAsync(), Times.Once);
        }
        finally
        {
            await target.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>A paused old placement and its timer cannot revive or heal a replacement death/life.</summary>
    [Test]
    public async Task OldRespawnCannotTouchReplacementLifeAsync()
    {
        var target = await CreatePlayerAsync(10).ConfigureAwait(false);
        var gate = GateOf(target);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var views = 0;
        var deaths = 0;
        target.Died += (_, _) => Interlocked.Increment(ref deaths);
        View<IRespawnAfterDeathPlugIn>(target).Setup(v => v.RespawnAsync()).Returns(() =>
            Interlocked.Increment(ref views) == 1 ? DelayViewAsync(entered, release, false) : ValueTask.CompletedTask);
        await target.KillInstantlyAsync().ConfigureAwait(false);
        var oldRespawn = target.RespawnAfterDeathAsync(gate).AsTask();
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await WarpAsync(target).ConfigureAwait(false);
            await target.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);
            Assert.That(target.IsAlive, Is.True);
            await target.ApplyPoisonDamageAsync(target, (uint)target.Attributes![Stats.CurrentHealth]).ConfigureAwait(false);
            var replacement = target.LastDeath;
            release.TrySetResult();
            await oldRespawn.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(target.LastDeath, Is.SameAs(replacement));
                Assert.That(target.IsAlive, Is.False);
                Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
                Assert.That(deaths, Is.EqualTo(2));
            });
            await target.RespawnAfterDeathAsync(gate).ConfigureAwait(false);
            target.Attributes[Stats.CurrentHealth] = 50;
            await Task.Delay(3300).ConfigureAwait(false);
            Assert.That(target.Attributes[Stats.CurrentHealth], Is.EqualTo(50));
            Assert.That(views, Is.EqualTo(2));
            Assert.That(deaths, Is.EqualTo(2));
        }
        finally
        {
            release.TrySetResult();
            await oldRespawn.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>A pending owned death is abandoned by disconnect or disposal, not applied to later work.</summary>
    /// <param name="dispose">Whether teardown is disposal instead of the ordinary disconnect.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task PendingMapChangeDeathIsAbandonedOnTeardownAsync(bool dispose)
    {
        var target = await CreatePlayerAsync(10).ConfigureAwait(false);
        var killer = await CreatePlayerAsync(100, sameWorldAs: target).ConfigureAwait(false);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var deaths = 0;
        target.Died += (_, _) => Interlocked.Increment(ref deaths);
        View<IShowHitPlugIn>(target).Setup(v => v.ShowHitAsync(target, It.IsAny<HitInfo>()))
            .Returns(() => DelayViewAsync(entered, release, false));
        var lethal = target.ApplyPoisonDamageAsync(killer, 10).AsTask();
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await WarpAsync(target).ConfigureAwait(false);
            release.TrySetResult();
            await lethal.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (dispose)
            {
                await target.DisposeAsync().ConfigureAwait(false);
            }
            else
            {
                await target.DisconnectAsync().ConfigureAwait(false);
            }

            await target.CompleteMapChangeDeathAsync().ConfigureAwait(false);
            Assert.That(deaths, Is.Zero);
            View<IObjectGotKilledPlugIn>(target).Verify(v => v.ObjectGotKilledAsync(target, It.IsAny<IAttacker>()), Times.Never);
        }
        finally
        {
            release.TrySetResult();
            await lethal.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
            await killer.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>A normal warp retains its health and a custom death veto remains intentional.</summary>
    /// <param name="lethal">Whether to reserve a lethal hit before the warp and veto its death.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task MapArrivalPreservesNormalWarpAndDeathVetoAsync(bool lethal)
    {
        var target = await CreatePlayerAsync(10).ConfigureAwait(false);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        target.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPlayerStateChangingPlugIn>(new DeathVetoPlugIn(target));
        View<IShowHitPlugIn>(target).Setup(v => v.ShowHitAsync(target, It.IsAny<HitInfo>()))
            .Returns(() => DelayViewAsync(entered, release, false));
        var hit = target.ApplyPoisonDamageAsync(target, lethal ? 10u : 1u).AsTask();
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await WarpAsync(target).ConfigureAwait(false);
            release.TrySetResult();
            await hit.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(target.IsAlive, Is.EqualTo(!lethal));
                Assert.That(target.Attributes![Stats.CurrentHealth], Is.EqualTo(lethal ? 0 : 9));
            });
            View<IObjectGotKilledPlugIn>(target).Verify(v => v.ObjectGotKilledAsync(target, It.IsAny<IAttacker>()), Times.Never);
        }
        finally
        {
            release.TrySetResult();
            await hit.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Accepted ordinary-hit costs and wake effects precede hit/death views, unlike DoT and reflection.</summary>
    /// <param name="source">The damage source.</param>
    /// <param name="lethal">Whether the hit is lethal.</param>
    /// <param name="recoverMana">Whether ordinary-hit full-mana recovery is guaranteed.</param>
    [TestCase("ordinary", false, false)]
    [TestCase("ordinary", false, true)]
    [TestCase("ordinary", true, false)]
    [TestCase("ordinary", true, true)]
    [TestCase("poison", true, true)]
    [TestCase("bleeding", true, true)]
    [TestCase("reflection", true, true)]
    public async Task AcceptedHitConsequencesPrecedeViewsAsync(string source, bool lethal, bool recoverMana)
    {
        var target = await CreatePlayerAsync(lethal ? 10 : 1000).ConfigureAwait(false);
        var attacker = await CreatePlayerAsync(1000, sameWorldAs: target).ConfigureAwait(false);
        Add(attacker, Stats.AttackRatePvp, 100000);
        Add(attacker, Stats.MinimumPhysBaseDmg, 100);
        Add(attacker, Stats.MaximumPhysBaseDmg, 100);
        Add(target, Stats.SoulBarrierManaTollPerHit, 7);
        Add(target, Stats.FullyRecoverManaAfterHitChance, recoverMana ? 1 : 0);
        target.Attributes![Stats.CurrentMana] = 80;
        var sleep = new MagicEffect(TimeSpan.FromMinutes(1), new MagicEffectDefinition { Number = 200 },
            new MagicEffect.ElementWithTarget(new SimpleElement(1, AggregateType.AddRaw), Stats.IsAsleep));
        var frozen = new MagicEffect(TimeSpan.FromMinutes(1), new MagicEffectDefinition { Number = 201 },
            new MagicEffect.ElementWithTarget(new SimpleElement(1, AggregateType.AddRaw), Stats.IsFrozen));
        await target.MagicEffectList.AddEffectAsync(sleep).ConfigureAwait(false);
        await target.MagicEffectList.AddEffectAsync(frozen).ConfigureAwait(false);
        var frozenFinish = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency + frozen.TimeLeft.TotalSeconds;
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        HitInfo observedHit = default;
        var manaAtDeath = float.NaN;
        target.Died += (_, _) => manaAtDeath = target.Attributes[Stats.CurrentMana];
        View<IShowHitPlugIn>(target).Setup(v => v.ShowHitAsync(target, It.IsAny<HitInfo>()))
            .Returns((IAttackable _, HitInfo hit) =>
            {
                Assert.That(target.IsApplyingHitOnCurrentThread, Is.False);
                observedHit = hit;
                return DelayViewAsync(entered, release, false);
            });
        var attack = source switch
        {
            "ordinary" => AttackAsync(),
            "poison" => target.ApplyPoisonDamageAsync(attacker, 10).AsTask(),
            "bleeding" => target.ApplyBleedingDamageAsync(attacker, 10).AsTask(),
            _ => target.ReflectDamageAsync(attacker, 10).AsTask(),
        };
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            var ordinary = source == "ordinary";
            var expectedMana = ordinary ? (recoverMana ? target.Attributes[Stats.MaximumMana] : 80) - observedHit.ManaToll : 80;
            Assert.Multiple(() =>
            {
                Assert.That(observedHit.HealthDamage, Is.GreaterThan(0));
                Assert.That(target.Attributes[Stats.CurrentHealth], Is.EqualTo((lethal ? 10 : 1000) - observedHit.HealthDamage));
                Assert.That(target.Attributes[Stats.CurrentMana], Is.EqualTo(expectedMana));
                Assert.That(target.MagicEffectList.ContainsEffect(sleep.Id), Is.EqualTo(!ordinary));
                var shortenedBy = frozenFinish - (Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency + frozen.TimeLeft.TotalSeconds);
                Assert.That(shortenedBy, Is.EqualTo(ordinary ? 1 : 0).Within(0.1));
            });
            release.TrySetResult();
            await attack.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (lethal)
            {
                Assert.That(manaAtDeath, Is.EqualTo(expectedMana));
                await target.RespawnAfterDeathAsync(new ExitGate { Map = target.CurrentMap!.Definition, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 }).ConfigureAwait(false);
                target.Attributes[Stats.CurrentMana] = 17;
                await target.ApplyPoisonDamageAsync(attacker, 1).ConfigureAwait(false);
                Assert.That(target.Attributes[Stats.CurrentMana], Is.EqualTo(17), "No ordinary-hit mana work may leak into the next life.");
            }
        }
        finally
        {
            release.TrySetResult();
            await attack.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
            await attacker.DisposeAsync().ConfigureAwait(false);
        }

        async Task AttackAsync() => await target.AttackByAsync(attacker, null, false).ConfigureAwait(false);
    }

    /// <summary>A real frozen-effect snapshot may expire before shortening without losing accepted-hit publication.</summary>
    /// <param name="expire">Whether the snapshotted real effect is disposed before DecreaseTimer.</param>
    /// <param name="lethal">Whether the committed hit owns death.</param>
    /// <param name="laterFailure">Whether an actual later death subscriber also faults.</param>
    [TestCase(false, false, false)]
    [TestCase(false, true, false)]
    [TestCase(true, false, false)]
    [TestCase(true, true, false)]
    [TestCase(true, true, true)]
    public async Task FrozenSnapshotFailureCannotHideAcceptedHitAsync(bool expire, bool lethal, bool laterFailure)
    {
        var target = await CreatePlayerAsync(lethal ? 10 : 1000).ConfigureAwait(false);
        var attacker = await CreatePlayerAsync(1000, sameWorldAs: target).ConfigureAwait(false);
        Add(attacker, Stats.AttackRatePvp, 100000);
        Add(attacker, Stats.MinimumPhysBaseDmg, 100);
        Add(attacker, Stats.MaximumPhysBaseDmg, 100);
        Add(target, Stats.SoulBarrierManaTollPerHit, 7);
        target.Attributes![Stats.CurrentMana] = 80;
        var asleep = new MagicEffect(TimeSpan.FromMinutes(1), new MagicEffectDefinition { Number = 200 },
            new MagicEffect.ElementWithTarget(new SimpleElement(1, AggregateType.AddRaw), Stats.IsAsleep));
        await target.MagicEffectList.AddEffectAsync(asleep).ConfigureAwait(false);
        var frozen = new MagicEffect(TimeSpan.FromMinutes(1), new MagicEffectDefinition { Number = 201 },
            new MagicEffect.ElementWithTarget(new SimpleElement(1, AggregateType.AddRaw), Stats.IsFrozen));
        await target.MagicEffectList.AddEffectAsync(frozen).ConfigureAwait(false);
        var elements = frozen.PowerUpElements.ToArray();
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var release = new ManualResetEventSlim();
        var enumerations = 0;
        frozen.PowerUpElements = EnumerateElements();
        var targetHits = 0;
        var attackerHits = 0;
        var healthUpdates = 0;
        var deaths = 0;
        var secondaryFailure = new InvalidOperationException("later death subscriber failed");
        var insideAttack = new AsyncLocal<bool>();
        ObjectDisposedException? originalFailure = null;
        EventHandler<FirstChanceExceptionEventArgs> captureFailure = (_, args) =>
        {
            if (insideAttack.Value && args.Exception is ObjectDisposedException error && error.ObjectName == nameof(MagicEffect))
            {
                Interlocked.CompareExchange(ref originalFailure, error, null);
            }
        };
        AppDomain.CurrentDomain.FirstChanceException += captureFailure;
        DeathInformation? observedDeath = null;
        target.Died += (_, death) =>
        {
            Interlocked.Increment(ref deaths);
            observedDeath = death;
            if (laterFailure)
            {
                throw secondaryFailure;
            }
        };
        var reportedHealth = float.NaN;
        var sleepAtHit = float.NaN;
        var manaAtHit = float.NaN;
        HitInfo observedHit = default;
        View<IShowHitPlugIn>(target).Setup(v => v.ShowHitAsync(target, It.IsAny<HitInfo>())).Returns((IAttackable _, HitInfo hit) =>
        {
            observedHit = hit;
            sleepAtHit = target.Attributes[Stats.IsAsleep];
            manaAtHit = target.Attributes[Stats.CurrentMana];
            Interlocked.Increment(ref targetHits);
            return ValueTask.CompletedTask;
        });
        View<IShowHitPlugIn>(attacker).Setup(v => v.ShowHitAsync(target, It.IsAny<HitInfo>())).Returns(() =>
        {
            Interlocked.Increment(ref attackerHits);
            return ValueTask.CompletedTask;
        });
        View<IUpdateStatsPlugIn>(target).Setup(v => v.UpdateStatsAsync(Stats.CurrentHealth, It.IsAny<float>())).Returns((AttributeDefinition _, float value) =>
        {
            reportedHealth = value;
            Interlocked.Increment(ref healthUpdates);
            return ValueTask.CompletedTask;
        });
        var gotHit = new Mock<IAttackableGotHitPlugIn>();
        target.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IAttackableGotHitPlugIn>(gotHit.Object);
        var gotHitAfterViews = false;
        gotHit.Setup(p => p.AttackableGotHit(target, attacker, It.IsAny<HitInfo>())).Callback(() => gotHitAfterViews = targetHits == 1 && attackerHits == 1);
        var attack = Task.Run(async () =>
        {
            insideAttack.Value = true;
            await target.AttackByAsync(attacker, null, false).ConfigureAwait(false);
        });
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(target.Attributes[Stats.CurrentHealth], Is.LessThan(lethal ? 10 : 1000), "The real resource commit precedes the effect-list snapshot enumeration.");
            Assert.That(target.Attributes[Stats.CurrentMana], Is.EqualTo(73));
            var committedHealth = target.Attributes[Stats.CurrentHealth];
            var committedShield = target.Attributes[Stats.CurrentShield];
            var deathOwner = target.LastDeath;
            if (expire)
            {
                await frozen.DisposeAsync().ConfigureAwait(false);
                Assert.That(target.MagicEffectList.ContainsEffect(frozen.Id), Is.False);
            }

            release.Set();
            if (expire)
            {
                var error = Assert.ThrowsAsync<ObjectDisposedException>(async () => await attack.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false));
                Assert.That(error, Is.SameAs(originalFailure), "First-chance original effect exception survives later notifications/death faults.");
            }
            else
            {
                await attack.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }

            Assert.Multiple(() =>
            {
                Assert.That(target.SelectedCharacter, Is.Not.Null, "The target remains connected and selected.");
                Assert.That(targetHits, Is.EqualTo(1));
                Assert.That(attackerHits, Is.EqualTo(1));
                Assert.That(healthUpdates, Is.EqualTo(1));
                Assert.That(deaths, Is.EqualTo(lethal ? 1 : 0));
                Assert.That(target.Attributes[Stats.CurrentHealth], Is.EqualTo(committedHealth));
                Assert.That(target.Attributes[Stats.CurrentShield], Is.EqualTo(committedShield));
                Assert.That(target.Attributes[Stats.CurrentMana], Is.EqualTo(73));
                Assert.That(reportedHealth, Is.EqualTo(committedHealth));
                Assert.That(observedHit.HealthDamage, Is.GreaterThan(0));
                Assert.That(sleepAtHit, Is.Zero, "Wake is attempted even if frozen shortening faults.");
                Assert.That(manaAtHit, Is.EqualTo(73));
                Assert.That(gotHitAfterViews, Is.True);
                if (lethal)
                {
                    Assert.That(observedDeath, Is.SameAs(deathOwner));
                    Assert.That(target.LastDeath, Is.SameAs(deathOwner));
                }
            });
            gotHit.Verify(p => p.AttackableGotHit(target, attacker, It.IsAny<HitInfo>()), Times.Once);
        }
        finally
        {
            release.Set();
            try
            {
                await attack.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }
            catch (Exception ex) when (expire && (ex is ObjectDisposedException || ReferenceEquals(ex, secondaryFailure)))
            {
                // Also observe the unchanged baseline's masked primary while unwinding a red assertion.
            }

            AppDomain.CurrentDomain.FirstChanceException -= captureFailure;
            await target.DisposeAsync().ConfigureAwait(false);
            await attacker.DisposeAsync().ConfigureAwait(false);
        }

        IEnumerable<MagicEffect.ElementWithTarget> EnumerateElements()
        {
            if (Interlocked.Increment(ref enumerations) == 1)
            {
                entered.TrySetResult();
                Assert.That(release.Wait(TimeSpan.FromSeconds(5)), Is.True);
            }

            foreach (var element in elements)
            {
                yield return element;
            }
        }
    }

    /// <summary>Ordinary, reflected and damage-over-time hits share one resource commit.</summary>
    [Test]
    public async Task PlayerMixedSourcesAsync()
    {
        var target = await CreatePlayerAsync(10000, slowHealth: true).ConfigureAwait(false);
        var attacker = await CreatePlayerAsync(10000).ConfigureAwait(false);
        Add(attacker, Stats.AttackRatePvp, 100000);
        Add(attacker, Stats.MinimumPhysBaseDmg, 10);
        Add(attacker, Stats.MaximumPhysBaseDmg, 10);
        var ordinaryDamage = 0;
        await ConcurrentAsync(64, async i =>
        {
            switch (i % 4)
            {
                case 0:
                    var hit = await target.AttackByAsync(attacker, null, false).ConfigureAwait(false);
                    Assert.That(hit, Is.Not.Null);
                    Interlocked.Add(ref ordinaryDamage, (int)hit!.Value.HealthDamage);
                    break;
                case 1:
                    await target.ReflectDamageAsync(attacker, 1).ConfigureAwait(false);
                    break;
                case 2:
                    await target.ApplyPoisonDamageAsync(attacker, 1).ConfigureAwait(false);
                    break;
                default:
                    await target.ApplyBleedingDamageAsync(attacker, 1).ConfigureAwait(false);
                    break;
            }
        }).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(ordinaryDamage, Is.GreaterThan(0));
            Assert.That(target.Attributes![Stats.CurrentHealth], Is.EqualTo(10000 - 48 - ordinaryDamage));
            Assert.That(target.Attributes[Stats.CurrentShield], Is.Zero);
        });
    }

    /// <summary>Zero damage cannot kill or consume resources.</summary>
    [Test]
    public async Task ZeroDamageAsync()
    {
        var target = await CreatePlayerAsync(100).ConfigureAwait(false);
        var npc = new TestNpc(target, 100);
        await ConcurrentAsync(64, async _ =>
        {
            await target.ApplyPoisonDamageAsync(target, 0).ConfigureAwait(false);
            await npc.ReflectDamageAsync(target, 0).ConfigureAwait(false);
        }).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(target.Attributes![Stats.CurrentHealth], Is.EqualTo(100));
            Assert.That(target.LastDeath, Is.Null);
            Assert.That(npc.Health, Is.EqualTo(100));
            Assert.That(npc.Deaths, Is.Zero);
        });
    }

    /// <summary>Slow or failing hit views cannot exclude another hit or target.</summary>
    [TestCase(false)]
    [TestCase(true)]
    public async Task PlayerHitViewDoesNotBlockAccountingAsync(bool fail)
    {
        var target = await CreatePlayerAsync(100).ConfigureAwait(false);
        var other = await CreatePlayerAsync(100).ConfigureAwait(false);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var calls = 0;
        View<IShowHitPlugIn>(target).Setup(v => v.ShowHitAsync(target, It.IsAny<HitInfo>()))
            .Returns(() => Interlocked.Increment(ref calls) == 1 ? DelayViewAsync(entered, release, fail) : ValueTask.CompletedTask);
        var first = target.ApplyPoisonDamageAsync(other, 1).AsTask();
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        try
        {
            await ConcurrentAsync(2, i => i == 0 ? target.ApplyPoisonDamageAsync(other, 1) : other.ApplyPoisonDamageAsync(target, 1)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(target.Attributes![Stats.CurrentHealth], Is.EqualTo(98));
                Assert.That(other.Attributes![Stats.CurrentHealth], Is.EqualTo(99));
            });
        }
        finally
        {
            release.TrySetResult();
            await first.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        }
    }

    /// <summary>Shield burst preserves eligibility without delaying damage accounting.</summary>
    [TestCase(false)]
    [TestCase(true)]
    public async Task ShieldBurstViewDoesNotBlockAccountingAsync(bool fail)
    {
        var target = await CreatePlayerAsync(100, 10).ConfigureAwait(false);
        Add(target, Stats.ShieldRatio, 0.65f);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        View<IShowEffectPlugIn>(target).Setup(v => v.ShowEffectAsync(target, IShowEffectPlugIn.EffectType.ShieldBurst))
            .Returns(() => DelayViewAsync(entered, release, fail));
        var first = target.ReflectDamageAsync(target, 31).AsTask();
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        try
        {
            await target.ApplyPoisonDamageAsync(target, 1).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(target.Attributes![Stats.CurrentHealth], Is.EqualTo(79));
                Assert.That(target.Attributes[Stats.CurrentShield], Is.Zero);
            });
        }
        finally
        {
            release.TrySetResult();
            await first.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        }

        View<IShowEffectPlugIn>(target).Verify(v => v.ShowEffectAsync(target, IShowEffectPlugIn.EffectType.ShieldBurst), Times.Once);
    }

    /// <summary>Stats callbacks may reenter and wait without holding damage exclusion.</summary>
    [TestCase(false)]
    [TestCase(true)]
    public async Task StatsViewsAreOutsideAccountingAsync(bool fail)
    {
        var target = await CreatePlayerAsync(100, slowHealth: true).ConfigureAwait(false);
        var healthAttribute = target.SelectedCharacter!.Attributes.OfType<SlowHealthAttribute>().Single();
        var notificationsDuringMutation = 0;
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var reentered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var calls = 0;
        View<IUpdateStatsPlugIn>(target).Setup(v => v.UpdateStatsAsync(Stats.CurrentHealth, It.IsAny<float>()))
            .Returns(async (AttributeDefinition _, float _) =>
            {
                if (healthAttribute.IsSettingOnCurrentThread)
                {
                    Interlocked.Increment(ref notificationsDuringMutation);
                }

                if (Interlocked.Increment(ref calls) == 1)
                {
                    await Task.Run(async () => await target.ApplyPoisonDamageAsync(target, 1).ConfigureAwait(false)).ConfigureAwait(false);
                    reentered.SetResult();
                    await DelayViewAsync(entered, release, fail).ConfigureAwait(false);
                }
            });
        // The hit awaits its own stats update, so it completes only after the delayed view is released.
        var hit = Task.Run(async () => await target.ApplyPoisonDamageAsync(target, 1).ConfigureAwait(false));
        try
        {
            await reentered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        }
        finally
        {
            release.TrySetResult();
        }

        await hit.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        target.Attributes![Stats.CurrentMana] = 17;
        Assert.That(target.Attributes[Stats.CurrentHealth], Is.EqualTo(98));
        Assert.That(notificationsDuringMutation, Is.Zero);
        View<IUpdateStatsPlugIn>(target).Verify(v => v.UpdateStatsAsync(Stats.CurrentHealth, 99), Times.Once);
        View<IUpdateStatsPlugIn>(target).Verify(v => v.UpdateStatsAsync(Stats.CurrentHealth, 98), Times.Once);
        View<IUpdateStatsPlugIn>(target).Verify(v => v.UpdateStatsAsync(Stats.CurrentMana, 17), Times.Once);
    }

    /// <summary>Both sides may reflect without nested target locks or reflecting reflection again.</summary>
    [Test]
    public async Task ReciprocalReflectionAsync()
    {
        var first = await CreatePlayerAsync(1000).ConfigureAwait(false);
        var second = await CreatePlayerAsync(1000).ConfigureAwait(false);
        Add(first, Stats.DamageReflection, 0.5f);
        Add(second, Stats.DamageReflection, 0.5f);
        var done = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var reflected = 0;
        foreach (var player in new[] { first, second })
        {
            View<IShowHitPlugIn>(player).Setup(v => v.ShowHitAsync(It.IsAny<IAttackable>(), It.Is<HitInfo>(h => (h.Attributes & DamageAttributes.Reflected) != 0)))
                .Callback(() =>
                {
                    if (Interlocked.Increment(ref reflected) == 12)
                    {
                        done.TrySetResult();
                    }
                });
        }

        await ConcurrentAsync(2, i => i == 0 ? first.ApplyBleedingDamageAsync(second, 10) : second.ApplyBleedingDamageAsync(first, 10)).ConfigureAwait(false);
        await done.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(first.Attributes![Stats.CurrentHealth], Is.EqualTo(982));
            Assert.That(second.Attributes![Stats.CurrentHealth], Is.EqualTo(982));
        });
    }

    /// <summary>Combined and individually lethal hits run the normal PK consequence once per life.</summary>
    [TestCase(2, 6u)]
    [TestCase(16, 10u)]
    public async Task PlayerDeathAndRespawnAsync(int hitCount, uint damage)
    {
        var target = await CreatePlayerAsync(10).ConfigureAwait(false);
        var killer = await CreatePlayerAsync(100).ConfigureAwait(false);
        var deaths = 0;
        target.Died += (_, _) => Interlocked.Increment(ref deaths);
        await ConcurrentAsync(hitCount, _ => target.ApplyPoisonDamageAsync(killer, damage)).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(deaths, Is.EqualTo(1));
            Assert.That(killer.SelectedCharacter!.PlayerKillCount, Is.EqualTo(1));
            Assert.That(target.IsAlive, Is.False);
        });
        Add(target, Stats.MaximumHealth, 100 - target.Attributes![Stats.MaximumHealth]);
        await target.RespawnAfterDeathAsync(new ExitGate { Map = target.CurrentMap!.Definition, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 }).ConfigureAwait(false);
        await target.ApplyPoisonDamageAsync(killer, 1).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(target.IsAlive, Is.True);
            Assert.That(target.Attributes[Stats.CurrentHealth], Is.EqualTo(99));
        });
        await ConcurrentAsync(16, _ => target.ApplyPoisonDamageAsync(killer, 100)).ConfigureAwait(false);
        Assert.That(deaths, Is.EqualTo(2));
        await target.DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>Nonlethal NPC damage and restoration share atomic health storage.</summary>
    [Test]
    public async Task NpcConservesHealthAndRestoresAsync()
    {
        var seed = await CreatePlayerAsync(1000).ConfigureAwait(false);
        var target = new TestNpc(seed, 1000);
        await ConcurrentAsync(64, _ => target.ApplyPoisonDamageAsync(seed, 1)).ConfigureAwait(false);
        Assert.That(target.Health, Is.EqualTo(936));
        await ConcurrentAsync(64, i => i % 2 == 0 ? target.ApplyPoisonDamageAsync(seed, 1) : target.RestoreAsync(1, 1000)).ConfigureAwait(false);
        Assert.That(target.Health, Is.EqualTo(936));
        Assert.That(target.Restore(1000, 1000), Is.EqualTo(64));
        Assert.That(target.Health, Is.EqualTo(1000));
    }

    /// <summary>Dead NPCs ignore hits and recovery; initialization makes a new life damageable.</summary>
    [Test]
    public async Task NpcDeathAndNewLifeAsync()
    {
        var seed = await CreatePlayerAsync(1000).ConfigureAwait(false);
        var target = new TestNpc(seed, 10);
        var died = 0;
        target.Died += (_, _) => Interlocked.Increment(ref died);
        await ConcurrentAsync(16, _ => target.ApplyPoisonDamageAsync(seed, uint.MaxValue)).ConfigureAwait(false);
        var death = target.LastDeath;
        await target.ApplyPoisonDamageAsync(seed, 1).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(target.Deaths, Is.EqualTo(1));
            Assert.That(died, Is.EqualTo(1));
            Assert.That(target.LastDeath, Is.SameAs(death));
            Assert.That(target.Health, Is.Zero);
            Assert.That(target.Restore(10, 10), Is.Zero);
        });
        target.Initialize();
        await target.ApplyPoisonDamageAsync(seed, 1).ConfigureAwait(false);
        Assert.That(target.Health, Is.EqualTo(9));
        await ConcurrentAsync(2, _ => target.ApplyPoisonDamageAsync(seed, 6)).ConfigureAwait(false);
        Assert.That(target.Deaths, Is.EqualTo(2));
        Assert.That(died, Is.EqualTo(2));
    }

    /// <summary>Current Triple Shot lanes and staggered delivery conserve damage alongside other hits.</summary>
    [TestCase(1, 3)]
    [TestCase(2, 1)]
    [TestCase(5, 1)]
    public async Task DeferredTripleShotDistanceAsync(int distance, int expectedHits)
    {
        var attacker = await CreatePlayerAsync(1000).ConfigureAwait(false);
        var target = await CreatePlayerAsync(1000, slowHealth: true, sameWorldAs: attacker).ConfigureAwait(false);
        attacker.Position = new Point(100, 100);
        target.Position = new Point(100, (byte)(100 + distance));
        Add(attacker, Stats.AttackRatePvp, 100000);
        Add(attacker, Stats.MinimumPhysBaseDmg, 10);
        Add(attacker, Stats.MaximumPhysBaseDmg, 10);
        var settings = new AreaSkillSettings
        {
            UseFrustumFilter = true,
            FrustumStartWidth = 1,
            FrustumEndWidth = 4.5f,
            FrustumDistance = 7,
            ProjectileCount = 3,
            UseDeferredHits = true,
            DelayPerOneDistance = TimeSpan.FromMilliseconds(50),
            DelayBetweenHits = TimeSpan.FromMilliseconds(100),
        };
        var skill = new Skill { Number = 24, Range = 6, AreaSkillSettings = settings };
        var entry = new SkillEntry { Skill = skill };
        var filter = AreaSkillTargetSelector.GetLaneFilter(settings);
        var projectiles = Enumerable.Range(0, 3).Where(i => filter.IsTargetWithinBounds(attacker, target, 128, i)).ToArray();
        Assert.That(projectiles, Has.Length.EqualTo(expectedHits));
        var done = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var delivered = 0;
        var damage = 0;
        var scheduler = new DeferredHitScheduler(attacker, entry, skill, settings, target.Position, 128, target.Id, 0, async t =>
        {
            var hit = await t.AttackByAsync(attacker, null, false).ConfigureAwait(false);
            Interlocked.Add(ref damage, (int)hit!.Value.HealthDamage);
            if (Interlocked.Increment(ref delivered) == expectedHits)
            {
                done.TrySetResult();
            }
        });
        foreach (var projectile in projectiles)
        {
            await scheduler.DeliverHitAsync(target, (settings.DelayPerOneDistance * distance) + (settings.DelayBetweenHits * projectile), projectile).ConfigureAwait(false);
        }

        await ConcurrentAsync(64, _ => target.ApplyPoisonDamageAsync(attacker, 1)).ConfigureAwait(false);
        await done.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        Assert.That(damage, Is.GreaterThan(0));
        Assert.That(target.Attributes![Stats.CurrentHealth], Is.EqualTo(1000 - 64 - damage));
    }

    /// <summary>A failed lethal notification must still reach the existing death lifecycle.</summary>
    [Test]
    public async Task LethalViewsRecoverAsync()
    {
        var target = await CreatePlayerAsync(100, 10).ConfigureAwait(false);
        var killer = await CreatePlayerAsync(1000).ConfigureAwait(false);
        Add(target, Stats.ShieldRatio, 0.65f);
        var deaths = 0;
        target.Died += (_, _) => Interlocked.Increment(ref deaths);
        View<IShowEffectPlugIn>(target).Setup(v => v.ShowEffectAsync(target, IShowEffectPlugIn.EffectType.ShieldBurst))
            .Returns(ValueTask.FromException(new InvalidOperationException("Expected shield burst failure.")));
        View<IShowHitPlugIn>(target).Setup(v => v.ShowHitAsync(target, It.IsAny<HitInfo>()))
            .Returns(ValueTask.FromException(new InvalidOperationException("Expected hit view failure.")));
        View<IUpdateStatsPlugIn>(target).Setup(v => v.UpdateStatsAsync(It.IsAny<AttributeDefinition>(), It.IsAny<float>()))
            .Returns(ValueTask.FromException(new InvalidOperationException("Expected stats view failure.")));
        await ConcurrentAsync(16, _ => target.ReflectDamageAsync(killer, 200)).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(deaths, Is.EqualTo(1));
            Assert.That(target.IsAlive, Is.False);
            Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
            Assert.That(killer.SelectedCharacter!.PlayerKillCount, Is.EqualTo(1));
        });
        await target.DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>The normal automatic NPC respawn makes a fresh life damageable.</summary>
    [Test]
    public async Task NpcAutomaticRespawnAsync()
    {
        var seed = await CreatePlayerAsync(1000).ConfigureAwait(false);
        var target = new TestNpc(seed, 10, true);
        await seed.CurrentMap!.AddAsync(target).ConfigureAwait(false);
        await ConcurrentAsync(16, _ => target.ApplyPoisonDamageAsync(seed, 10)).ConfigureAwait(false);
        await target.Respawned.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        await target.ApplyPoisonDamageAsync(seed, 1).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(target.Deaths, Is.EqualTo(1));
            Assert.That(target.IsAlive, Is.True);
            Assert.That(target.Health, Is.EqualTo(9));
        });
        target.Dispose();
    }

    /// <summary>The real NPC reward and delayed loot path run once for a lethal owner.</summary>
    [Test]
    public async Task NpcLootOnceAsync()
    {
        var seed = await CreatePlayerAsync(1000).ConfigureAwait(false);
        var generated = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var drops = new Mock<IDropGenerator>();
        drops.Setup(d => d.GenerateItemDropsAsync(It.IsAny<MonsterDefinition>(), It.IsAny<int>(), seed))
            .Returns(() =>
            {
                generated.TrySetResult();
                return new ValueTask<(IEnumerable<Item>, uint?)>((Array.Empty<Item>(), null));
            });
        var target = new TestNpc(seed, 10, true, true, drops.Object);
        await seed.CurrentMap!.AddAsync(target).ConfigureAwait(false);
        await ConcurrentAsync(16, _ => target.ApplyPoisonDamageAsync(seed, 10)).ConfigureAwait(false);
        await generated.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        Assert.That(target.Deaths, Is.EqualTo(1));
        drops.Verify(d => d.GenerateItemDropsAsync(target.Definition, It.IsAny<int>(), seed), Times.Once);
        target.Dispose();
    }

    /// <summary>A waiting or failed NPC hit notification cannot replace lethal attribution.</summary>
    [TestCase(false)]
    [TestCase(true)]
    public async Task NpcLethalViewAsync(bool fail)
    {
        var seed = await CreatePlayerAsync(1000).ConfigureAwait(false);
        var late = await CreatePlayerAsync(1000).ConfigureAwait(false);
        seed.Id = 11;
        late.Id = 22;
        var target = new TestNpc(seed, 10, notifyPlayer: true);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        View<IShowHitPlugIn>(seed).Setup(v => v.ShowHitAsync(target, It.IsAny<HitInfo>()))
            .Returns(() => DelayViewAsync(entered, release, fail));
        var lethal = target.ApplyPoisonDamageAsync(seed, 10).AsTask();
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        try
        {
            await target.ApplyPoisonDamageAsync(late, 10).ConfigureAwait(false);
            Assert.That(target.LastDeath?.KillerId, Is.EqualTo(seed.Id));
            Assert.That(target.Health, Is.Zero);
        }
        finally
        {
            release.TrySetResult();
            await lethal.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        }

        Assert.That(target.Deaths, Is.EqualTo(1));
    }

    /// <summary>Ordinary NPC attacks and all effect entry points conserve their combined damage.</summary>
    [Test]
    public async Task NpcMixedSourcesAsync()
    {
        var seed = await CreatePlayerAsync(1000).ConfigureAwait(false);
        Add(seed, Stats.AttackRatePvm, 100000);
        Add(seed, Stats.MinimumPhysBaseDmg, 10);
        Add(seed, Stats.MaximumPhysBaseDmg, 10);
        var target = new TestNpc(seed, 10000);
        var ordinaryDamage = 0;
        await ConcurrentAsync(64, async i =>
        {
            switch (i % 4)
            {
                case 0:
                    var hit = await target.AttackByAsync(seed, null, false).ConfigureAwait(false);
                    Assert.That(hit, Is.Not.Null);
                    Interlocked.Add(ref ordinaryDamage, (int)hit!.Value.HealthDamage);
                    break;
                case 1:
                    await target.ReflectDamageAsync(seed, 1).ConfigureAwait(false);
                    break;
                case 2:
                    await target.ApplyPoisonDamageAsync(seed, 1).ConfigureAwait(false);
                    break;
                default:
                    await target.ApplyBleedingDamageAsync(seed, 1).ConfigureAwait(false);
                    break;
            }
        }).ConfigureAwait(false);
        Assert.That(ordinaryDamage, Is.GreaterThan(0));
        Assert.That(target.Health, Is.EqualTo(10000 - 48 - ordinaryDamage));
    }

    /// <summary>Both NPC hit plug-in points see every accepted hit once, the skill one with the hit's skill.</summary>
    [Test]
    public async Task NpcHitPlugInsSeeEachAcceptedHitOnceAsync()
    {
        var seed = await CreatePlayerAsync(1000).ConfigureAwait(false);
        var skill = new Skill { Number = 42 };
        var target = new TestNpc(seed, 10, notifyPlayer: true);
        var plain = new Mock<IAttackableGotHitPlugIn>();
        var withSkill = new Mock<IAttackableGotHitWithSkillPlugIn>();
        seed.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IAttackableGotHitPlugIn>(plain.Object);
        seed.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IAttackableGotHitWithSkillPlugIn>(withSkill.Object);
        var skillsSeen = new System.Collections.Concurrent.ConcurrentBag<Skill?>();
        withSkill.Setup(p => p.AttackableGotHit(target, seed, It.IsAny<HitInfo>(), It.IsAny<Skill?>()))
            .Callback((IAttackable _, IAttacker _, HitInfo _, Skill? used) => skillsSeen.Add(used));
        var hits = 0;
        View<IShowHitPlugIn>(seed).Setup(v => v.ShowHitAsync(target, It.IsAny<HitInfo>())).Returns(() =>
        {
            Interlocked.Increment(ref hits);
            return ValueTask.CompletedTask;
        });

        // Four hits of 3 damage are accepted (the fourth is lethal); later ones hit a dead NPC.
        await ConcurrentAsync(16, _ => target.ApplySkillHitAsync(seed, 3, skill)).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(target.Deaths, Is.EqualTo(1));
            Assert.That(hits, Is.EqualTo(4));
            plain.Verify(p => p.AttackableGotHit(target, seed, It.IsAny<HitInfo>()), Times.Exactly(4));
            Assert.That(skillsSeen, Has.Count.EqualTo(4));
            Assert.That(skillsSeen, Is.All.SameAs(skill));
        });
    }

    /// <summary>Combat interactions cancel normally before death, without stranding the reservation.</summary>
    [TestCase("party")]
    [TestCase("trade-request")]
    [TestCase("trade-open")]
    [TestCase("trade-button")]
    [TestCase("npc")]
    public async Task InteractionDeathAndRespawnAsync(string interaction)
    {
        var target = await CreatePlayerAsync(10).ConfigureAwait(false);
        var peer = await CreatePlayerAsync(1000, sameWorldAs: target).ConfigureAwait(false);
        peer.Id = 11;
        target.Id = 22;
        target.Inventory!.ItemStorage.Money = 1234;
        if (interaction == "party")
        {
            await new PartyRequestAction().HandlePartyRequestAsync(peer, target).ConfigureAwait(false);
            Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.PartyRequest));
        }
        else if (interaction == "npc")
        {
            var definition = new Mock<MonsterDefinition>();
            definition.Setup(d => d.ItemCraftings).Returns(new List<MUnique.OpenMU.DataModel.Configuration.ItemCrafting.ItemCrafting>());
            definition.Object.NpcWindow = NpcWindow.ChaosMachine;
            var npc = new NonPlayerCharacter(new MonsterSpawnArea(), definition.Object, target.CurrentMap!);
            await new TalkNpcAction().TalkToNpcAsync(target, npc).ConfigureAwait(false);
            Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.NpcDialogOpened));
        }
        else
        {
            Assert.That(await new TradeRequestAction().RequestTradeAsync(peer, target).ConfigureAwait(false), Is.True);
            if (interaction != "trade-request")
            {
                await new TradeAcceptAction().HandleTradeAcceptAsync(target, true).ConfigureAwait(false);
                Assert.That(target.BackupInventory, Is.Not.Null);
                target.Inventory.ItemStorage.Money = 1;
                if (interaction == "trade-button")
                {
                    await new TradeButtonAction().TradeButtonChangedAsync(target, TradeButtonState.Checked).ConfigureAwait(false);
                    Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeButtonPressed));
                }
                else
                {
                    Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
                }
            }
        }

        var deaths = 0;
        target.Died += (_, _) => Interlocked.Increment(ref deaths);
        await ConcurrentAsync(16, _ => target.ApplyPoisonDamageAsync(peer, 10)).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(deaths, Is.EqualTo(1));
            Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
            Assert.That(target.LastPartyRequester, Is.Null);
            Assert.That(target.TradingPartner, Is.Null);
            Assert.That(peer.TradingPartner, Is.Null);
            Assert.That(target.BackupInventory, Is.Null);
            Assert.That(peer.BackupInventory, Is.Null);
            Assert.That(target.Inventory.ItemStorage.Money, Is.EqualTo(1234));
            Assert.That(target.OpenedNpc, Is.Null);
            Assert.That(peer.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
        });
        Add(target, Stats.MaximumHealth, 100 - target.Attributes![Stats.MaximumHealth]);
        await target.RespawnAfterDeathAsync(new ExitGate { Map = target.CurrentMap!.Definition, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 }).ConfigureAwait(false);
        await target.ApplyPoisonDamageAsync(peer, 1).ConfigureAwait(false);
        Assert.That(target.IsAlive, Is.True);
        Assert.That(target.Attributes[Stats.CurrentHealth], Is.EqualTo(99));
        await target.DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>Outgoing ordinary-hit costs cannot overwrite incoming ordinary damage.</summary>
    [TestCase(0)]
    [TestCase(1)]
    public async Task ReciprocalOrdinaryAttacksAsync(int healthLoss)
    {
        var first = await CreatePlayerAsync(1000, slowHealth: true).ConfigureAwait(false);
        var second = await CreatePlayerAsync(1000, slowHealth: true, sameWorldAs: first).ConfigureAwait(false);
        foreach (var player in new[] { first, second })
        {
            Add(player, Stats.AttackRatePvp, 100000);
            Add(player, Stats.MinimumPhysBaseDmg, 10);
            Add(player, Stats.MaximumPhysBaseDmg, 10);
            Add(player, Stats.HealthLossAfterHit, healthLoss);
        }

        var healthAttribute = second.SelectedCharacter!.Attributes.OfType<SlowHealthAttribute>().Single();
        var readStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var releaseRead = new ManualResetEventSlim();
        var armed = 0;
        View<IShowHitPlugIn>(first).Setup(v => v.ShowHitAsync(first, It.IsAny<HitInfo>()))
            .Callback(() => Interlocked.Exchange(ref armed, 1));
        healthAttribute.SetAfterRead(() =>
        {
            // Gate only an unprotected stale read. Never wait inside a production-locked getter.
            if (!second.IsApplyingHitOnCurrentThread && Interlocked.Exchange(ref armed, 0) == 1)
            {
                readStarted.TrySetResult();
                if (!releaseRead.Wait(TimeSpan.FromSeconds(5)))
                {
                    throw new TimeoutException("Unprotected outgoing health read was not released.");
                }
            }
        });
        var outgoing = Task.Run(async () => await first.AttackByAsync(second, null, false).ConfigureAwait(false));
        HitInfo? incoming;
        try
        {
            await Task.WhenAny(readStarted.Task, outgoing).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (outgoing.IsCompleted)
            {
                Interlocked.Exchange(ref armed, 0);
            }

            incoming = await second.AttackByAsync(first, null, false).ConfigureAwait(false);
        }
        finally
        {
            releaseRead.Set();
        }

        var outgoingHit = await outgoing.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        healthAttribute.SetAfterRead(null);
        Assert.Multiple(() =>
        {
            Assert.That(outgoingHit?.HealthDamage, Is.GreaterThan(0));
            Assert.That(incoming?.HealthDamage, Is.GreaterThan(0));
            Assert.That(first.Attributes![Stats.CurrentHealth], Is.EqualTo(1000 - outgoingHit!.Value.HealthDamage - healthLoss));
            Assert.That(second.Attributes![Stats.CurrentHealth], Is.EqualTo(1000 - incoming!.Value.HealthDamage - healthLoss));
        });
    }

    /// <summary>A rejected ordinary hit cannot consume landed costs or heal through Drain Life.</summary>
    [TestCase(false)]
    [TestCase(true)]
    public async Task RejectedOrdinaryHitDoesNotDrainLifeAsync(bool npc)
    {
        var killer = await CreatePlayerAsync(1000).ConfigureAwait(false);
        killer.Id = 11;
        var attacker = await CreatePlayerAsync(50, sameWorldAs: killer).ConfigureAwait(false);
        Add(attacker, Stats.AttackRatePvp, 100000);
        Add(attacker, Stats.AttackRatePvm, 100000);
        Add(attacker, Stats.MinimumPhysBaseDmg, 10);
        Add(attacker, Stats.MaximumPhysBaseDmg, 10);
        Add(attacker, Stats.HealthLossAfterHit, 5);
        Add(attacker, Stats.DrainLifeStrBonusHealing, 20);
        IAttackable target = npc ? new TestNpc(killer, 10, notifyPlayer: true) : await CreatePlayerAsync(10, sameWorldAs: killer).ConfigureAwait(false);
        var recipient = target as Player ?? killer;
        if (target is Player player)
        {
            Add(player, Stats.FullyRecoverHealthAfterHitChance, 1);
            Add(player, Stats.FullyRecoverManaAfterHitChance, 1);
            player.Attributes![Stats.CurrentMana] = 80;
        }

        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        View<IShowHitPlugIn>(recipient).Setup(v => v.ShowHitAsync(target, It.IsAny<HitInfo>()))
            .Returns(() => DelayViewAsync(entered, release, false));
        var lethal = target.ApplyPoisonDamageAsync(killer, 10).AsTask();
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        try
        {
            var hit = await target.AttackByAsync(attacker, null, false).ConfigureAwait(false);
            await new DrainLifeSkillPlugIn().AfterTargetGotAttackedAsync(attacker, target, new SkillEntry { Skill = new Skill { Number = 214 } }, target.Position, hit).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(hit, Is.Null);
                Assert.That(attacker.Attributes![Stats.CurrentHealth], Is.EqualTo(50));
                Assert.That(target.Attributes[Stats.CurrentHealth], Is.Zero);
                Assert.That(target.LastDeath?.KillerId, Is.EqualTo(killer.Id));
            });
            if (target is Player deadPlayer)
            {
                Assert.That(deadPlayer.Attributes![Stats.CurrentMana], Is.EqualTo(80));
            }

            View<IShowHitPlugIn>(recipient).Verify(v => v.ShowHitAsync(target, It.IsAny<HitInfo>()), Times.Once);
        }
        finally
        {
            release.TrySetResult();
            await lethal.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (target is Player deadPlayer)
            {
                await deadPlayer.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    /// <summary>Real request setup cannot publish references after the recipient's death.</summary>
    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public async Task PausedRequestCannotReattachDeadRecipientAsync(bool trade, bool cancel)
    {
        var target = await CreatePlayerAsync(10).ConfigureAwait(false);
        var peer = await CreatePlayerAsync(1000, sameWorldAs: target).ConfigureAwait(false);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var hitEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var stateMachine = trade ? peer.PlayerState : target.PlayerState;
        var pendingState = trade ? PlayerState.TradeRequested : PlayerState.PartyRequest;
        var transitions = 0;
        stateMachine.StateChanges += async args =>
        {
            if (args.NextState == pendingState && Interlocked.Increment(ref transitions) == 1)
            {
                entered.TrySetResult();
                await release.Task.ConfigureAwait(false);
                args.Cancel = cancel;
            }
        };
        View<IShowHitPlugIn>(target).Setup(v => v.ShowHitAsync(target, It.IsAny<HitInfo>())).Callback(() => hitEntered.TrySetResult());
        Task request = trade
            ? new TradeRequestAction().RequestTradeAsync(peer, target).AsTask()
            : new PartyRequestAction().HandlePartyRequestAsync(peer, target).AsTask();
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        var deaths = 0;
        target.Died += (_, _) => Interlocked.Increment(ref deaths);
        var lethal = target.ApplyPoisonDamageAsync(peer, 10).AsTask();
        try
        {
            await hitEntered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(target.IsAlive, Is.False);
            Assert.That(target.Attributes![Stats.CurrentHealth], Is.Zero);
        }
        finally
        {
            release.TrySetResult();
            await Task.WhenAll(request, lethal).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        }

        if (trade)
        {
            Assert.That(await ((Task<bool>)request).ConfigureAwait(false), Is.False);
        }

        Assert.Multiple(() =>
        {
            Assert.That(deaths, Is.EqualTo(1));
            Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
            Assert.That(target.LastPartyRequester, Is.Null);
            Assert.That(target.TradingPartner, Is.Null);
            Assert.That(peer.TradingPartner, Is.Null);
            Assert.That(peer.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
        });
        Add(target, Stats.MaximumHealth, 100 - target.Attributes![Stats.MaximumHealth]);
        await target.RespawnAfterDeathAsync(new ExitGate { Map = target.CurrentMap!.Definition, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 }).ConfigureAwait(false);
        await target.ApplyPoisonDamageAsync(peer, 1).ConfigureAwait(false);
        Assert.That(target.Attributes[Stats.CurrentHealth], Is.EqualTo(99));
        if (trade)
        {
            Assert.That(await new TradeRequestAction().RequestTradeAsync(peer, target).ConfigureAwait(false), Is.True);
            await new TradeCancelAction().CancelTradeAsync(target).ConfigureAwait(false);
        }
        else
        {
            await new PartyRequestAction().HandlePartyRequestAsync(peer, target).ConfigureAwait(false);
            Assert.That(target.LastPartyRequester, Is.SameAs(peer));
            await new PartyResponseAction().HandleResponseAsync(target, false).ConfigureAwait(false);
        }

        Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
        await target.DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>A late request-view continuation cannot roll back a newer trade.</summary>
    [Test]
    public async Task LateTradeRequestDoesNotCancelNewerTradeAsync()
    {
        var target = await CreatePlayerAsync(10).ConfigureAwait(false);
        var peer = await CreatePlayerAsync(1000, sameWorldAs: target).ConfigureAwait(false);
        var newerPeer = await CreatePlayerAsync(1000, sameWorldAs: target).ConfigureAwait(false);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        View<IShowTradeRequestPlugIn>(target).Setup(v => v.ShowTradeRequestAsync(peer)).Returns(() => DelayViewAsync(entered, release, false));
        var request = new TradeRequestAction().RequestTradeAsync(peer, target).AsTask();
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        try
        {
            await target.ApplyPoisonDamageAsync(peer, 10).ConfigureAwait(false);
            Assert.That(await new TradeRequestAction().RequestTradeAsync(peer, newerPeer).ConfigureAwait(false), Is.True);
        }
        finally
        {
            release.TrySetResult();
        }

        Assert.That(await request.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false), Is.False);
        Assert.Multiple(() =>
        {
            Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
            Assert.That(target.TradingPartner, Is.Null);
            Assert.That(peer.TradingPartner, Is.SameAs(newerPeer));
            Assert.That(newerPeer.TradingPartner, Is.SameAs(peer));
            Assert.That(peer.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeRequested));
        });
        await new TradeCancelAction().CancelTradeAsync(peer).ConfigureAwait(false);
        Assert.That(peer.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
        await target.DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>A late party request and response cannot resurrect or erase a newer invitation.</summary>
    [Test]
    public async Task LatePartyRequestDoesNotCancelNewerInviteAsync()
    {
        var requester = await CreatePlayerAsync(10).ConfigureAwait(false);
        var recipient = await CreatePlayerAsync(1000, sameWorldAs: requester).ConfigureAwait(false);
        var newerRequester = await CreatePlayerAsync(1000, sameWorldAs: requester).ConfigureAwait(false);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        View<IShowPartyRequestPlugIn>(recipient).Setup(v => v.ShowPartyRequestAsync(requester)).Returns(() => DelayViewAsync(entered, release, false));
        var request = new PartyRequestAction().HandlePartyRequestAsync(requester, recipient).AsTask();
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        try
        {
            await requester.ApplyPoisonDamageAsync(recipient, 10).ConfigureAwait(false);
            await new PartyResponseAction().HandleResponseAsync(recipient, false).ConfigureAwait(false);
            await new PartyRequestAction().HandlePartyRequestAsync(newerRequester, recipient).ConfigureAwait(false);
        }
        finally
        {
            release.TrySetResult();
        }

        await request.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        Assert.That(requester.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
        Assert.That(recipient.LastPartyRequester, Is.SameAs(newerRequester));
        await new PartyResponseAction().HandleResponseAsync(requester, false).ConfigureAwait(false);
        Assert.That(requester.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
        await new PartyResponseAction().HandleResponseAsync(recipient, false).ConfigureAwait(false);
        Assert.That(recipient.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
        Assert.That(recipient.LastPartyRequester, Is.Null);
        await requester.DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>Late real NPC denials cannot undo death or tear down a replacement dialog.</summary>
    [TestCase("no-quests", 0)]
    [TestCase("low-level", 0)]
    [TestCase("finished", 0)]
    [TestCase("guild-level", 0)]
    [TestCase("guild-member", 0)]
    [TestCase("no-quests", 1)]
    [TestCase("low-level", 1)]
    [TestCase("finished", 1)]
    [TestCase("guild-level", 1)]
    [TestCase("guild-member", 1)]
    [TestCase("no-quests", 2)]
    [TestCase("low-level", 2)]
    [TestCase("finished", 2)]
    [TestCase("guild-level", 2)]
    [TestCase("guild-member", 2)]
    public async Task LateNpcDenialPreservesLifeAndNewDialogAsync(string response, int reopen)
    {
        var target = await CreatePlayerAsync(10).ConfigureAwait(false);
        var killer = await CreatePlayerAsync(1000, sameWorldAs: target).ConfigureAwait(false);
        var (npc, message) = CreateNpcResponse(target, response);
        target.Inventory!.ItemStorage.Money = 1234;
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var newerEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var newerRelease = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var calls = 0;
        View<IShowMessageOfObjectPlugIn>(target).Setup(v => v.ShowMessageOfObjectAsync(message, npc)).Returns(() =>
            Interlocked.Increment(ref calls) == 1 ? DelayViewAsync(entered, release, false)
                : reopen == 2 ? DelayViewAsync(newerEntered, newerRelease, false) : ValueTask.CompletedTask);
        var responseTask = new TalkNpcAction().TalkToNpcAsync(target, npc).AsTask();
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        var originalDialog = target.OpenedNpcContext;
        var deaths = 0;
        target.Died += (_, _) => Interlocked.Increment(ref deaths);
        Task? newerResponse = null;
        try
        {
            await target.ApplyPoisonDamageAsync(killer, 10).ConfigureAwait(false);
            if (reopen != 0)
            {
                Add(target, Stats.MaximumHealth, 100 - target.Attributes![Stats.MaximumHealth]);
                await target.RespawnAfterDeathAsync(new ExitGate { Map = target.CurrentMap!.Definition, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 }).ConfigureAwait(false);
                if (reopen == 2)
                {
                    newerResponse = new TalkNpcAction().TalkToNpcAsync(target, npc).AsTask();
                    await newerEntered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                }
                else
                {
                    var craftingDefinition = Mock.Of<MonsterDefinition>(d => d.ItemCraftings == new List<MUnique.OpenMU.DataModel.Configuration.ItemCrafting.ItemCrafting> { new() });
                    craftingDefinition.NpcWindow = NpcWindow.ChaosMachine;
                    var newerNpc = new NonPlayerCharacter(new MonsterSpawnArea(), craftingDefinition, target.CurrentMap!);
                    await new TalkNpcAction().TalkToNpcAsync(target, newerNpc).ConfigureAwait(false);
                }
            }

            var currentDialog = target.OpenedNpcContext;
            var backup = target.BackupInventory;
            release.TrySetResult();
            await responseTask.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(deaths, Is.EqualTo(1));
                Assert.That(target.PlayerState.CurrentState, Is.EqualTo(reopen == 0 ? PlayerState.Dead : PlayerState.NpcDialogOpened));
                Assert.That(target.IsAlive, Is.EqualTo(reopen != 0));
                Assert.That(target.OpenedNpcContext, Is.SameAs(currentDialog));
                Assert.That(target.BackupInventory, Is.SameAs(backup));
                Assert.That(target.Inventory.ItemStorage.Money, Is.EqualTo(1234));
            });
            if (reopen != 0)
            {
                Assert.That(currentDialog, Is.Not.Null.And.Not.SameAs(originalDialog));
                if (reopen == 2)
                {
                    Assert.That(target.OpenedNpc, Is.SameAs(npc));
                    newerRelease.TrySetResult();
                    await newerResponse!.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                }
                else
                {
                    Assert.That(target.BackupInventory, Is.Not.Null);
                    await new CloseNpcDialogAction().CloseNpcDialogAsync(target).ConfigureAwait(false);
                }

                Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(target.OpenedNpc, Is.Null);
            }
        }
        finally
        {
            release.TrySetResult();
            newerRelease.TrySetResult();
            await responseTask.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (newerResponse is not null)
            {
                await newerResponse.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }

            await target.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Normal denial responses still close in current and older clients.</summary>
    [TestCase("no-quests", false)]
    [TestCase("low-level", false)]
    [TestCase("finished", false)]
    [TestCase("guild-level", false)]
    [TestCase("guild-member", false)]
    [TestCase("no-quests", true)]
    [TestCase("low-level", true)]
    [TestCase("finished", true)]
    [TestCase("guild-level", true)]
    [TestCase("guild-member", true)]
    public async Task LiveNpcDenialClosesExpectedDialogAsync(string response, bool olderClient)
    {
        var target = await CreatePlayerAsync(100).ConfigureAwait(false);
        var (npc, message) = CreateNpcResponse(target, response);
        TalkNpcAction action = olderClient ? new TalkNpcAction075() : new TalkNpcAction();
        await action.TalkToNpcAsync(target, npc).ConfigureAwait(false);
        View<IShowMessageOfObjectPlugIn>(target).Verify(v => v.ShowMessageOfObjectAsync(message, npc), Times.Once);
        Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
        Assert.That(target.OpenedNpc, Is.Null);
    }

    /// <summary>Eligible quest and guild responses retain the live dialog and crafting snapshot.</summary>
    [TestCase("quest-open")]
    [TestCase("guild-open")]
    public async Task LiveNpcDialogAndInventoryRemainUsableAsync(string response)
    {
        var target = await CreatePlayerAsync(100).ConfigureAwait(false);
        var (npc, _) = CreateNpcResponse(target, response);
        npc.Definition.ItemCraftings.Add(new MUnique.OpenMU.DataModel.Configuration.ItemCrafting.ItemCrafting());
        target.Inventory!.ItemStorage.Money = 1234;
        await new TalkNpcAction().TalkToNpcAsync(target, npc).ConfigureAwait(false);
        if (response == "quest-open")
        {
            View<ILegacyQuestStateDialogPlugIn>(target).Verify(v => v.ShowAsync(), Times.Once);
        }
        else
        {
            View<IShowGuildMasterDialogPlugIn>(target).Verify(v => v.ShowGuildMasterDialogAsync(), Times.Once);
        }

        Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.NpcDialogOpened));
        Assert.That(target.OpenedNpc, Is.SameAs(npc));
        Assert.That(target.BackupInventory, Is.Not.Null);
        await new CloseNpcDialogAction().CloseNpcDialogAsync(target).ConfigureAwait(false);
        Assert.That(target.OpenedNpc, Is.Null);
        Assert.That(target.BackupInventory, Is.Null);
        Assert.That(target.Inventory.ItemStorage.Money, Is.EqualTo(1234));
    }

    private static (NonPlayerCharacter Npc, string Message) CreateNpcResponse(Player player, string response)
    {
        var quests = new List<QuestDefinition>();
        var definition = Mock.Of<MonsterDefinition>(d => d.ItemCraftings == new List<MUnique.OpenMU.DataModel.Configuration.ItemCrafting.ItemCrafting>() && d.Quests == quests);
        definition.NpcWindow = response.StartsWith("guild", StringComparison.Ordinal) ? NpcWindow.GuildMaster : NpcWindow.LegacyQuest;
        var states = new List<CharacterQuestState>();
        Mock.Get(player.SelectedCharacter!).Setup(c => c.QuestStates).Returns(states);
        if (response == "low-level" || response == "finished" || response == "quest-open")
        {
            var quest = new QuestDefinition { Group = 1, Number = 1, MinimumCharacterLevel = response == "low-level" ? 100 : 0 };
            quests.Add(quest);
            if (response == "finished")
            {
                states.Add(new CharacterQuestState { Group = 1, LastFinishedQuest = quest });
            }
        }

        if (response == "guild-member" || response == "guild-open")
        {
            player.Attributes![Stats.Level] = 100;
        }

        if (response == "guild-member")
        {
            player.GuildStatus = new GuildMemberStatus(1, GuildPosition.GuildMaster);
        }

        var message = response switch
        {
            "low-level" => "I have nothing to do for you. Come back with more power.",
            "finished" => "I have nothing to do for you. You solved all my quests already.",
            "guild-level" => "Your level should be at least level 100",
            "guild-member" => "You already belong to a guild",
            _ => "I have no quests for you.",
        };
        return (new NonPlayerCharacter(new MonsterSpawnArea(), definition, player.CurrentMap!), message);
    }

    private static async ValueTask DelayViewAsync(TaskCompletionSource entered, TaskCompletionSource release, bool fail)
    {
        entered.TrySetResult();
        await release.Task.ConfigureAwait(false);
        if (fail)
        {
            throw new InvalidOperationException("Expected test view failure.");
        }
    }

    // Copy the existing due marker under state to prove a real timer handoff, not a guessed sleep.
    [System.Runtime.CompilerServices.UnsafeAccessor(System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "_pendingDeathRespawn")]
    private static extern ref (CancellationTokenSource Source, DeathInformation Death, bool Claimed)? PendingDeathRespawn(Player player);

    private static Mock<T> View<T>(Player player)
        where T : class, IViewPlugIn => Mock.Get(player.ViewPlugIns.GetPlugIn<T>()!);

    private static ExitGate GateOf(Player player)
        => new() { Map = player.SelectedCharacter!.CurrentMap, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 };

    private static ValueTask WarpAsync(Player player)
        => new WarpAction().WarpToAsync(player, new WarpInfo { Gate = GateOf(player) });

    private static async Task<Player> CreatePlayerAsync(float health, float shield = 0, bool slowHealth = false, Player? sameWorldAs = null, bool useRespawnPlugIn = true)
    {
        var seed = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var context = sameWorldAs?.GameContext ?? seed.GameContext;
        var character = PlayerTestHelper.CreateCharacter(context);
        character.Attributes.Add(slowHealth ? new SlowHealthAttribute(health) : new StatAttribute(Stats.CurrentHealth, health));
        character.Attributes.Add(new StatAttribute(Stats.CurrentShield, shield));
        var player = new TestPlayer(context, useRespawnPlugIn);
        await PlayerTestHelper.EnterWorldAsync(player, character).ConfigureAwait(false);
        player.Attributes![Stats.CurrentHealth] = health;
        player.Attributes[Stats.CurrentShield] = shield;
        return player;
    }

    private static void Add(Player player, AttributeDefinition definition, float value)
    {
        player.Attributes!.AddElement(new SimpleElement(value, AggregateType.AddRaw), definition);
    }

    private static async Task ConcurrentAsync(int count, Func<int, ValueTask> hit)
    {
        var start = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var tasks = Enumerable.Range(0, count).Select(async i =>
        {
            await start.Task.ConfigureAwait(false);
            await Task.Run(async () => await hit(i).ConfigureAwait(false)).ConfigureAwait(false);
        }).ToArray();
        start.SetResult();
        await Task.WhenAll(tasks).WaitAsync(TimeSpan.FromSeconds(15)).ConfigureAwait(false);
    }

    [System.Runtime.InteropServices.Guid("59682862-62E4-4A70-A16B-E389087CB50A")]
    private sealed class PausingSpawnGatePlugIn(Player expectedPlayer, ExitGate gate, Func<ValueTask> pause) : IPlayerSpawnGateSelectionPlugIn
    {
        private int _calls;

        public async ValueTask SelectSpawnGateAsync(Player player, SpawnGateSelectionArgs args)
        {
            Assert.That(player, Is.SameAs(expectedPlayer));
            args.Gate = gate;
            if (Interlocked.Increment(ref this._calls) == 1)
            {
                await pause().ConfigureAwait(false);
            }
        }
    }

    private sealed class TestPlayer(IGameContext context, bool useRespawnPlugIn = true) : Player(context)
    {
        protected override ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer()
            => useRespawnPlugIn ? new MockViewPlugInContainer() : new MapChangeRespawnViewPlugInContainer();
    }

    private sealed class MapChangeRespawnViewPlugInContainer : ICustomPlugInContainer<IViewPlugIn>
    {
        private readonly MockViewPlugInContainer _mocks = new();

        public T? GetPlugIn<T>()
            where T : class, IViewPlugIn
            => typeof(T) == typeof(IRespawnAfterDeathPlugIn) ? null : this._mocks.GetPlugIn<T>();
    }

    // Finite delay widens the read/write race without requiring another thread inside a production lock.
    private sealed class SlowHealthAttribute(float health) : StatAttribute(Stats.CurrentHealth, health)
    {
        private int _settingThreadId;

        private Action? _afterRead;

        public bool IsSettingOnCurrentThread => this._settingThreadId == Environment.CurrentManagedThreadId;

        public override float Value
        {
            get => base.Value;
            set
            {
                this._settingThreadId = Environment.CurrentManagedThreadId;
                try
                {
                    base.Value = value;
                }
                finally
                {
                    this._settingThreadId = 0;
                }
            }
        }

        protected override float ValueGetter
        {
            get
            {
                var value = base.ValueGetter;
                this._afterRead?.Invoke();
                Thread.SpinWait(50000);
                return value;
            }
        }

        public void SetAfterRead(Action? afterRead) => this._afterRead = afterRead;
    }

    [System.Runtime.InteropServices.Guid("B045D4DC-FA8B-4E21-AE53-AFE6EB8A8D96")]
    private sealed class DeathVetoPlugIn(Player expectedPlayer) : IPlayerStateChangingPlugIn
    {
        public ValueTask PlayerStateChangingAsync(Player player, StateMachine.StateChangeEventArgs eventArgs)
        {
            Assert.That(player, Is.SameAs(expectedPlayer));
            if (eventArgs.NextState == PlayerState.Dead)
            {
                eventArgs.Cancel = true;
            }

            return ValueTask.CompletedTask;
        }
    }

    private sealed class InitializedNpc : AttackableNpcBase
    {
        private int _deaths;

        public InitializedNpc(Player seed, int health, int? healthOverride)
            : this(seed, Mock.Of<MonsterDefinition>(d => d.Attributes == new List<MonsterAttribute>
            {
                new() { AttributeDefinition = Stats.MaximumHealth, Value = health },
                new() { AttributeDefinition = Stats.DamageReceiveDecrement, Value = 1 },
            }), healthOverride)
        {
        }

        private InitializedNpc(Player seed, MonsterDefinition definition, int? healthOverride)
            : base(new MonsterSpawnArea { MonsterDefinition = definition, MaximumHealthOverride = healthOverride }, definition, seed.CurrentMap!, null, Mock.Of<IDropGenerator>(), seed.GameContext.PlugInManager)
        {
            this.Initialize();
        }

        public int Deaths => this._deaths;

        public IAttacker? Killer { get; private set; }

        public int Restore(int amount, int maximum) => this.RestoreHealth(amount, maximum);

        public override ValueTask ReflectDamageAsync(IAttacker attacker, uint damage) => this.HitAsync(new HitInfo(damage, 0, DamageAttributes.Reflected), attacker, null);

        public override ValueTask ApplyPoisonDamageAsync(IAttacker attacker, uint damage) => this.HitAsync(new HitInfo(damage, 0, DamageAttributes.Poison), attacker, null);

        public override ValueTask ApplyBleedingDamageAsync(IAttacker attacker, uint damage) => this.HitAsync(new HitInfo(damage, 0, DamageAttributes.Undefined), attacker, null);

        protected override ValueTask OnDeathAsync(IAttacker attacker)
        {
            this.Killer = attacker;
            Interlocked.Increment(ref this._deaths);
            return ValueTask.CompletedTask;
        }
    }

    private sealed class TestNpc : AttackableNpcBase
    {
        private readonly TaskCompletionSource _respawned = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly bool _runDeathLifecycle;
        private readonly bool _notifyPlayer;
        private int _deaths;

        public TestNpc(Player seed, int health, bool runDeathLifecycle = false, bool notifyPlayer = false, IDropGenerator? dropGenerator = null)
            : base(new MonsterSpawnArea { MonsterDefinition = new MonsterDefinition(), SpawnTrigger = SpawnTrigger.Automatic }, CreateDefinition(health), seed.CurrentMap!, null, dropGenerator ?? Mock.Of<IDropGenerator>(), seed.GameContext.PlugInManager)
        {
            this._runDeathLifecycle = runDeathLifecycle;
            this._notifyPlayer = notifyPlayer;
            this.SpawnArea.X1 = (byte)seed.Position.X;
            this.SpawnArea.Y1 = (byte)seed.Position.Y;
            this.SpawnArea.X2 = this.SpawnArea.X1;
            this.SpawnArea.Y2 = this.SpawnArea.Y1;
            this.Initialize();
            this.Health = health;
        }

        public int Deaths => this._deaths;

        public Task Respawned => this._respawned.Task;

        public override void OnSpawn()
        {
            base.OnSpawn();
            this._respawned.TrySetResult();
        }

        public int Restore(int amount, int maximum) => this.RestoreHealth(amount, maximum);

        public ValueTask RestoreAsync(int amount, int maximum)
        {
            this.RestoreHealth(amount, maximum);
            return ValueTask.CompletedTask;
        }

        public override ValueTask ReflectDamageAsync(IAttacker reflector, uint damage) => this.HitAsync(new HitInfo(damage, 0, DamageAttributes.Reflected), reflector, null);

        public override ValueTask ApplyPoisonDamageAsync(IAttacker initialAttacker, uint damage) => this.HitAsync(new HitInfo(damage, 0, DamageAttributes.Poison), initialAttacker, null);

        public override ValueTask ApplyBleedingDamageAsync(IAttacker initialAttacker, uint damage) => this.HitAsync(new HitInfo(damage, 0, DamageAttributes.Undefined), initialAttacker, null);

        public ValueTask ApplySkillHitAsync(IAttacker attacker, uint damage, Skill skill) => this.HitAsync(new HitInfo(damage, 0, DamageAttributes.Undefined), attacker, skill);

        protected override async ValueTask OnDeathAsync(IAttacker attacker)
        {
            Interlocked.Increment(ref this._deaths);
            if (this._runDeathLifecycle)
            {
                await base.OnDeathAsync(attacker).ConfigureAwait(false);
            }
        }

        protected override Player? GetHitNotificationTarget(IAttacker attacker) => this._notifyPlayer ? base.GetHitNotificationTarget(attacker) : null;

        private static MonsterDefinition CreateDefinition(int health)
        {
            var definition = Mock.Of<MonsterDefinition>(d => d.Attributes == new List<MonsterAttribute>
            {
                new() { AttributeDefinition = Stats.MaximumHealth, Value = health },
                new() { AttributeDefinition = Stats.DamageReceiveDecrement, Value = 1 },
            });
            definition.RespawnDelay = TimeSpan.FromMilliseconds(100);
            return definition;
        }
    }
}
