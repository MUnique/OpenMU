// <copyright file="HitAttributeNotificationOrderTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Collections.Concurrent;
using System.Threading;
using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// The attribute updates which a hit collects while it holds the hit lock are sent to the client after the lock
/// is released, one after another, and before the views which follow the hit.
/// </summary>
[TestFixture]
public class HitAttributeNotificationOrderTests
{
    /// <summary>
    /// A delayed health update holds back the view of its own hit, but not the hit lock: another hit is accepted
    /// and shown while the first update is still pending.
    /// </summary>
    [Test]
    public async Task HitViewFollowsItsCompletedStatUpdateAsync()
    {
        var target = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var attacker = await PlayerTestHelper.CreatePlayerAsync(target.GameContext).ConfigureAwait(false);
        target.Attributes![Stats.CurrentHealth] = 100;
        target.Attributes[Stats.CurrentShield] = 0;
        var events = new ConcurrentQueue<string>();
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var underHitLock = false;
        var armed = 1;
        View<IUpdateStatsPlugIn>(target).Setup(v => v.UpdateStatsAsync(Stats.CurrentHealth, It.IsAny<float>())).Returns(async (AttributeDefinition _, float value) =>
        {
            underHitLock |= target.IsApplyingHitOnCurrentThread;
            if (Interlocked.Exchange(ref armed, 0) == 1)
            {
                entered.TrySetResult();
                await release.Task.ConfigureAwait(false);
            }

            events.Enqueue($"stats:{value}");
        });
        View<IShowHitPlugIn>(target).Setup(v => v.ShowHitAsync(target, It.IsAny<HitInfo>())).Returns((IAttackable _, HitInfo hit) =>
        {
            events.Enqueue($"hit:{hit.HealthDamage}");
            return ValueTask.CompletedTask;
        });

        var first = Task.Run(async () => await target.ReflectDamageAsync(attacker, 10).ConfigureAwait(false));
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(target.Attributes[Stats.CurrentHealth], Is.EqualTo(90));

            // The pending update of the first hit doesn't hold the hit lock: a second hit is accepted and shown.
            await Task.Run(async () => await target.ApplyPoisonDamageAsync(attacker, 7).ConfigureAwait(false)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(target.Attributes[Stats.CurrentHealth], Is.EqualTo(83));
                Assert.That(events, Does.Contain("hit:7"));
                Assert.That(events, Does.Not.Contain("hit:10"), "The first hit is shown only after its health update completed.");
                Assert.That(first.IsCompleted, Is.False);
            });

            release.TrySetResult();
            await first.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            var order = events.ToList();
            Assert.Multiple(() =>
            {
                Assert.That(order.IndexOf("stats:90"), Is.GreaterThanOrEqualTo(0));
                Assert.That(order.IndexOf("stats:90"), Is.LessThan(order.IndexOf("hit:10")));
                Assert.That(order.IndexOf("stats:83"), Is.LessThan(order.IndexOf("hit:7")));
                Assert.That(underHitLock, Is.False);
            });
        }
        finally
        {
            release.TrySetResult();
            await first.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
            await attacker.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>The changes of one hit are sent one after another, in the order in which the hit made them.</summary>
    [Test]
    public async Task StatUpdatesOfOneHitCompleteInOrderAsync()
    {
        var target = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var attacker = await PlayerTestHelper.CreatePlayerAsync(target.GameContext).ConfigureAwait(false);
        target.Attributes![Stats.CurrentHealth] = 100;
        target.Attributes.AddElement(new SimpleElement(100, AggregateType.AddRaw), Stats.MaximumShield);
        target.Attributes.AddElement(new SimpleElement(0.5f, AggregateType.AddRaw), Stats.ShieldRatio);
        target.Attributes[Stats.CurrentShield] = 20;
        var events = new ConcurrentQueue<string>();
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var armed = 1;
        View<IUpdateStatsPlugIn>(target).Setup(v => v.UpdateStatsAsync(It.IsAny<AttributeDefinition>(), It.IsAny<float>())).Returns(async (AttributeDefinition definition, float _) =>
        {
            events.Enqueue($"start:{definition.Designation}");
            if (definition == Stats.CurrentShield && Interlocked.Exchange(ref armed, 0) == 1)
            {
                entered.TrySetResult();
                await release.Task.ConfigureAwait(false);
            }

            events.Enqueue($"end:{definition.Designation}");
        });
        var hit = Task.Run(async () => await target.ReflectDamageAsync(attacker, 30).ConfigureAwait(false));
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(events, Does.Not.Contain($"start:{Stats.CurrentHealth.Designation}"), "The next update starts only after the previous one completed.");
            release.TrySetResult();
            await hit.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(events.ToList(), Is.EqualTo(new[]
            {
                $"start:{Stats.CurrentShield.Designation}",
                $"end:{Stats.CurrentShield.Designation}",
                $"start:{Stats.CurrentHealth.Designation}",
                $"end:{Stats.CurrentHealth.Designation}",
            }));
        }
        finally
        {
            release.TrySetResult();
            await hit.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
            await attacker.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>A failing update is handled and logged; its hit and later hits are still shown.</summary>
    [Test]
    public async Task FailingStatUpdateDoesNotStopHitPublicationAsync()
    {
        var target = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var attacker = await PlayerTestHelper.CreatePlayerAsync(target.GameContext).ConfigureAwait(false);
        target.Attributes![Stats.CurrentHealth] = 100;
        target.Attributes[Stats.CurrentShield] = 0;
        View<IUpdateStatsPlugIn>(target).Setup(v => v.UpdateStatsAsync(Stats.CurrentHealth, It.IsAny<float>()))
            .Returns(ValueTask.FromException(new InvalidOperationException("view failed")));
        try
        {
            await target.ReflectDamageAsync(attacker, 10).ConfigureAwait(false);
            await target.ApplyPoisonDamageAsync(attacker, 5).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(target.Attributes[Stats.CurrentHealth], Is.EqualTo(85));
                View<IShowHitPlugIn>(target).Verify(v => v.ShowHitAsync(target, It.IsAny<HitInfo>()), Times.Exactly(2));
            });
        }
        finally
        {
            await target.DisposeAsync().ConfigureAwait(false);
            await attacker.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>An update which itself causes another hit on the same player completes without deadlock.</summary>
    [Test]
    public async Task ReentrantHitFromStatUpdateCompletesAsync()
    {
        var target = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var attacker = await PlayerTestHelper.CreatePlayerAsync(target.GameContext).ConfigureAwait(false);
        target.Attributes![Stats.CurrentHealth] = 100;
        target.Attributes[Stats.CurrentShield] = 0;
        var reentered = 0;
        View<IUpdateStatsPlugIn>(target).Setup(v => v.UpdateStatsAsync(Stats.CurrentHealth, It.IsAny<float>())).Returns(async () =>
        {
            if (Interlocked.Exchange(ref reentered, 1) == 0)
            {
                await target.ApplyPoisonDamageAsync(attacker, 5).ConfigureAwait(false);
            }
        });
        try
        {
            await Task.Run(async () => await target.ReflectDamageAsync(attacker, 10).ConfigureAwait(false)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(target.Attributes[Stats.CurrentHealth], Is.EqualTo(85));
                View<IShowHitPlugIn>(target).Verify(v => v.ShowHitAsync(target, It.IsAny<HitInfo>()), Times.Exactly(2));
            });
        }
        finally
        {
            await target.DisposeAsync().ConfigureAwait(false);
            await attacker.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>The health loss after hitting a target is sent before the attack continues.</summary>
    [Test]
    public async Task HealthLossAfterHitIsSentBeforeTheAttackContinuesAsync()
    {
        var attacker = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        attacker.Attributes![Stats.CurrentHealth] = 100;
        attacker.Attributes.AddElement(new SimpleElement(4, AggregateType.AddRaw), Stats.HealthLossAfterHit);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        View<IUpdateStatsPlugIn>(attacker).Setup(v => v.UpdateStatsAsync(Stats.CurrentHealth, It.IsAny<float>())).Returns(async () =>
        {
            entered.TrySetResult();
            await release.Task.ConfigureAwait(false);
        });
        var afterHit = Task.Run(async () => await attacker.AfterHitTargetAsync(0, DamageType.Physical).ConfigureAwait(false));
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(attacker.Attributes[Stats.CurrentHealth], Is.EqualTo(96));
                Assert.That(attacker.IsApplyingHitOnCurrentThread, Is.False);
            });
            await Task.Delay(100).ConfigureAwait(false);
            Assert.That(afterHit.IsCompleted, Is.False, "The attack continues after its health update completed.");
            release.TrySetResult();
            await afterHit.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        }
        finally
        {
            release.TrySetResult();
            await afterHit.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await attacker.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>An instant kill sends its health update before the death follows.</summary>
    [Test]
    public async Task InstantKillSendsHealthBeforeDeathAsync()
    {
        var target = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        target.Attributes![Stats.CurrentHealth] = 100;
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        View<IUpdateStatsPlugIn>(target).Setup(v => v.UpdateStatsAsync(Stats.CurrentHealth, It.IsAny<float>())).Returns(async () =>
        {
            entered.TrySetResult();
            await release.Task.ConfigureAwait(false);
        });
        var deaths = 0;
        target.Died += (_, _) => Interlocked.Increment(ref deaths);
        var kill = Task.Run(async () => await target.KillInstantlyAsync().ConfigureAwait(false));
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await Task.Delay(100).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(target.IsAlive, Is.False);
                Assert.That(Volatile.Read(ref deaths), Is.Zero, "The death follows the completed health update.");
                Assert.That(target.PlayerState.CurrentState, Is.Not.EqualTo(PlayerState.Dead));
            });
            release.TrySetResult();
            await kill.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(Volatile.Read(ref deaths), Is.EqualTo(1));
                Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
            });
        }
        finally
        {
            release.TrySetResult();
            await kill.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>The restored attributes of a respawn are sent before the respawn itself.</summary>
    [Test]
    public async Task RespawnSendsRestoredAttributesBeforeTheRespawnAsync()
    {
        var target = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var attacker = await PlayerTestHelper.CreatePlayerAsync(target.GameContext).ConfigureAwait(false);
        target.Attributes![Stats.CurrentHealth] = 10;
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var respawns = 0;
        View<IRespawnAfterDeathPlugIn>(target).Setup(v => v.RespawnAsync()).Returns(() =>
        {
            Interlocked.Increment(ref respawns);
            return ValueTask.CompletedTask;
        });
        var respawn = Task.CompletedTask;
        try
        {
            await target.ApplyPoisonDamageAsync(attacker, 10).ConfigureAwait(false);
            Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
            View<IUpdateStatsPlugIn>(target).Setup(v => v.UpdateStatsAsync(Stats.CurrentHealth, It.IsAny<float>())).Returns(async () =>
            {
                entered.TrySetResult();
                await release.Task.ConfigureAwait(false);
            });
            var gate = new ExitGate { Map = target.CurrentMap!.Definition, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 };
            respawn = Task.Run(async () => await target.RespawnAfterDeathAsync(gate).ConfigureAwait(false));
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await Task.Delay(100).ConfigureAwait(false);
            Assert.That(Volatile.Read(ref respawns), Is.Zero, "The respawn follows the completed health update.");
            release.TrySetResult();
            await respawn.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(Volatile.Read(ref respawns), Is.EqualTo(1));
                Assert.That(target.IsAlive, Is.True);
                Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
            });
        }
        finally
        {
            release.TrySetResult();
            await respawn.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
            await attacker.DisposeAsync().ConfigureAwait(false);
        }
    }

    private static Mock<T> View<T>(Player player)
        where T : class, MUnique.OpenMU.GameLogic.Views.IViewPlugIn => Mock.Get(player.ViewPlugIns.GetPlugIn<T>()!);
}
