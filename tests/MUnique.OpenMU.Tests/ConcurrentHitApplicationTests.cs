// <copyright file="ConcurrentHitApplicationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading;
using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Party;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.GameLogic.Views.Party;
using MUnique.OpenMU.GameLogic.Views.Quest;
using MUnique.OpenMU.GameLogic.Views.Trade;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

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
            Assert.That(target.Attributes![Stats.CurrentHealth], Is.Zero);
            target.Attributes[Stats.CurrentHealth] = 5; // Recovery cannot reopen a reserved death.
            await target.ApplyPoisonDamageAsync(lateAttacker, 7).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(target.Attributes[Stats.CurrentHealth], Is.EqualTo(5));
                Assert.That(target.LastDeath?.KillerId, Is.EqualTo(killer.Id));
            });
        }
        finally
        {
            release.TrySetResult();
            await lethal.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
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
        await target.ApplyPoisonDamageAsync(target, 1).ConfigureAwait(false);
        await reentered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        release.SetResult();
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
        target.Attributes![Stats.CurrentHealth] = 100;
        await target.RespawnAtAsync(new ExitGate { Map = target.CurrentMap!.Definition, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 }).ConfigureAwait(false);
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
        target.Attributes![Stats.CurrentHealth] = 100;
        await target.RespawnAtAsync(new ExitGate { Map = target.CurrentMap!.Definition, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 }).ConfigureAwait(false);
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
        target.Attributes![Stats.CurrentHealth] = 100;
        await target.RespawnAtAsync(new ExitGate { Map = target.CurrentMap!.Definition, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 }).ConfigureAwait(false);
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
                target.Attributes![Stats.CurrentHealth] = 100;
                await target.RespawnAtAsync(new ExitGate { Map = target.CurrentMap!.Definition, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 }).ConfigureAwait(false);
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

    private static Mock<T> View<T>(Player player)
        where T : class, IViewPlugIn => Mock.Get(player.ViewPlugIns.GetPlugIn<T>()!);

    private static async Task<Player> CreatePlayerAsync(float health, float shield = 0, bool slowHealth = false, Player? sameWorldAs = null)
    {
        var seed = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var context = sameWorldAs?.GameContext ?? seed.GameContext;
        var character = PlayerTestHelper.CreateCharacter(context);
        character.Attributes.Add(slowHealth ? new SlowHealthAttribute(health) : new StatAttribute(Stats.CurrentHealth, health));
        character.Attributes.Add(new StatAttribute(Stats.CurrentShield, shield));
        var player = new TestPlayer(context);
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

    private sealed class TestPlayer(IGameContext context) : Player(context)
    {
        protected override ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer() => new MockViewPlugInContainer();
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
