// <copyright file="RespawnLogoutOrderingTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameLogic.Views.Login;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>Logout retains the safezone obligation until a scheduled death respawn has placed the character.</summary>
[TestFixture]
public class RespawnLogoutOrderingTests
{
    /// <summary>Actual logout while the real death timer is pending or admitted saves safezone coordinates.</summary>
    /// <param name="boundary">The actual respawn boundary held before logout, or an ordinary control.</param>
    [TestCase("attributes")]
    [TestCase("remove")]
    [TestCase("before-timer")]
    [TestCase("completed")]
    [TestCase("placed")]
    [TestCase("attributes-throw")]
    [TestCase("successor")]
    [TestCase("same-life-successor")]
    public async Task LogoutSavesSafezoneAsync(string boundary)
    {
        var publicationFailed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var logger = new Mock<ILogger>();
        logger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        logger.Setup(l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback(new InvocationAction(invocation =>
            {
                if (invocation.Arguments[3] is Exception { Message: "Respawn publication failure." })
                {
                    publicationFailed.TrySetResult();
                }
            }));
        var loggerFactory = new Mock<ILoggerFactory>();
        loggerFactory.Setup(factory => factory.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
        var configuration = PlayerTestHelper.CreateGameConfiguration();
        var initializer = new MapInitializer(configuration, NullLogger<MapInitializer>.Instance, NullDropGenerator.Instance, null);
        using var game = new GameContext(configuration, new InMemoryPersistenceContextProvider(), initializer, loggerFactory.Object, new PlugInManager(null, loggerFactory.Object, null, null), NullDropGenerator.Instance, new ConfigurationChangeMediator());
        initializer.PlugInManager = game.PlugInManager;
        initializer.PathFinderPool = game.PathFinderPool;
        var victim = new RespawnPlayer(game);
        await PlayerTestHelper.EnterWorldAsync(victim, PlayerTestHelper.CreateCharacter(game)).ConfigureAwait(false);
        var character = victim.SelectedCharacter!;
        var map = victim.CurrentMap!;
        var safezone = new Mock<GameMapDefinition>();
        safezone.SetupAllProperties();
        safezone.Setup(m => m.DropItemGroups).Returns(map.Definition.DropItemGroups);
        safezone.Setup(m => m.MonsterSpawns).Returns(map.Definition.MonsterSpawns);
        safezone.Setup(m => m.ExitGates).Returns(new List<ExitGate>());
        safezone.Object.Number = 1;
        safezone.Object.TerrainData = map.Definition.TerrainData;
        victim.GameContext.Configuration.Maps.Add(safezone.Object);
        map.Definition.SafezoneMap = safezone.Object;
        var gate = new ExitGate { Map = safezone.Object, X1 = 17, X2 = 17, Y1 = 19, Y2 = 19, IsSpawnGate = true };
        safezone.Object.ExitGates.Add(gate);
        character.Name = "RespawnTest";
        Mock.Get(victim.Account!).Setup(a => a.Characters).Returns(new[] { character });
        character.PositionX = 100;
        character.PositionY = 101;
        victim.Attributes![Stats.CurrentHealth] = 10;
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var returned = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var respawnPacket = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var armed = 1;
        var deaths = 0;
        var secondDeath = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var secondRespawn = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var respawns = 0;
        victim.Died += (_, _) =>
        {
            if (Interlocked.Increment(ref deaths) == 2)
            {
                secondDeath.TrySetResult();
            }
        };
        async ValueTask PauseAsync()
        {
            if (Interlocked.Exchange(ref armed, 0) == 1)
            {
                entered.TrySetResult();
                try
                {
                    await release.Task.ConfigureAwait(false);
                }
                finally
                {
                    returned.TrySetResult();
                }
            }
        }

        Mock.Get(victim.ViewPlugIns.GetPlugIn<IUpdateStatsPlugIn>()!)
            .Setup(v => v.UpdateStatsAsync(Stats.CurrentHealth, It.Is<float>(value => value > 0)))
            .Returns(async () =>
            {
                if (boundary is "attributes" or "attributes-throw" or "successor" or "same-life-successor")
                {
                    var first = Volatile.Read(ref armed) == 1;
                    await PauseAsync().ConfigureAwait(false);
                    if (first && boundary == "attributes-throw")
                    {
                        throw new InvalidOperationException("Respawn publication failure.");
                    }
                }
            });
        map.ObjectRemoved += async args =>
        {
            if (boundary == "remove" && ReferenceEquals(args.Object, victim))
            {
                await PauseAsync().ConfigureAwait(false);
            }
        };
        Mock.Get(victim.ViewPlugIns.GetPlugIn<IRespawnAfterDeathPlugIn>()!).Setup(v => v.RespawnAsync()).Returns(async () =>
        {
            respawnPacket.TrySetResult();
            if (Interlocked.Increment(ref respawns) == 2)
            {
                secondRespawn.TrySetResult();
            }
            if (boundary == "placed")
            {
                await PauseAsync().ConfigureAwait(false);
            }
        });
        try
        {
            await victim.KillInstantlyAsync().ConfigureAwait(false);
            if (boundary is "attributes" or "remove" or "attributes-throw" or "successor" or "same-life-successor")
            {
                await entered.Task.WaitAsync(TimeSpan.FromSeconds(8)).ConfigureAwait(false);
                Assert.Multiple(() =>
                {
                    Assert.That(victim.PlayerState.CurrentState, Is.EqualTo(PlayerState.ChangingMap));
                    Assert.That(victim.Attributes[Stats.CurrentHealth], Is.EqualTo(victim.Attributes[Stats.MaximumHealth]));
                    Assert.That(character.PositionX, Is.EqualTo(100));
                    Assert.That(character.PositionY, Is.EqualTo(101));
                });
            }
            else if (boundary == "completed")
            {
                await respawnPacket.Task.WaitAsync(TimeSpan.FromSeconds(8)).ConfigureAwait(false);
            }
            else
            {
                // Before-timer logout needs no wait; placed uses its dedicated view barrier below.
            }

            if (boundary == "same-life-successor")
            {
                CancellationTokenSource oldSource;
                await using (var state = await victim.PlayerState.TryBeginAdvanceToAsync(victim.PlayerState.CurrentState).ConfigureAwait(false))
                {
                    state.Allowed = false;
                    oldSource = RespawnSource(victim)!;
                }

                Assert.That(oldSource, Is.Not.Null);
                await victim.KillInstantlyAsync().ConfigureAwait(false);
                release.TrySetResult();
                await secondDeath.Task.WaitAsync(TimeSpan.FromSeconds(8)).ConfigureAwait(false);
                CancellationTokenSource newSource;
                await using (var state = await victim.PlayerState.TryBeginAdvanceToAsync(victim.PlayerState.CurrentState).ConfigureAwait(false))
                {
                    state.Allowed = false;
                    newSource = RespawnSource(victim)!;
                }

                Assert.That(newSource, Is.Not.SameAs(oldSource));
                Assert.Throws<ObjectDisposedException>(() => _ = oldSource.Token, "Replacement death owns disposal of the superseded timer.");
                var newToken = newSource.Token;
                await secondRespawn.Task.WaitAsync(TimeSpan.FromSeconds(8)).ConfigureAwait(false);
                Assert.Multiple(() =>
                {
                    Assert.That(newToken.IsCancellationRequested, Is.False);
                    Assert.That(deaths, Is.EqualTo(2));
                    Assert.That(character.PositionX, Is.EqualTo(17));
                    Assert.That(character.PositionY, Is.EqualTo(19));
                    Assert.That(victim.Attributes![Stats.CurrentHealth], Is.EqualTo(victim.Attributes[Stats.MaximumHealth]));
                });
                return;
            }

            if (boundary == "placed")
            {
                await entered.Task.WaitAsync(TimeSpan.FromSeconds(8)).ConfigureAwait(false);
                // A token lingering beyond coordinate commit would move again to this DIFFERENT configured spawn.
                map.Definition.ExitGates.Add(new ExitGate { Map = map.Definition, X1 = 31, X2 = 31, Y1 = 33, Y2 = 33, IsSpawnGate = true });
                safezone.Object.SafezoneMap = map.Definition;
            }

            if (boundary == "attributes-throw")
            {
                release.TrySetResult();
                await publicationFailed.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                await using var state = await victim.PlayerState.TryBeginAdvanceToAsync(victim.PlayerState.CurrentState).ConfigureAwait(false);
                state.Allowed = false;
                Assert.That(RespawnSource(victim), Is.Not.Null);
                Assert.That(RespawnSource(victim)!.Token.IsCancellationRequested, Is.False, "A failed placement keeps a usable logout obligation, not a disposed source.");
            }

            await victim.RunPersistenceExclusiveAsync(() => new LogoutAction().LogoutAsync(victim, LogoutType.BackToCharacterSelection))
                .AsTask().WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(character.CurrentMap, Is.SameAs(gate.Map));
                Assert.That(character.PositionX, Is.EqualTo(17), "Logout must save safezone, not the death spot.");
                Assert.That(character.PositionY, Is.EqualTo(19));
                Assert.That(victim.SelectedCharacter, Is.Null);
                Assert.That(victim.PlayerState.CurrentState, Is.EqualTo(PlayerState.Authenticated));
                Assert.That(deaths, Is.EqualTo(1));
            });
            if (boundary == "successor")
            {
                var previousAttributes = victim.Attributes;
                await victim.RunPersistenceExclusiveAsync(() => new RequestCharacterListAction().RequestCharacterListAsync(victim)).ConfigureAwait(false);
                await victim.RunPersistenceExclusiveAsync(() => new SelectCharacterAction().SelectCharacterAsync(victim, character.Name)).ConfigureAwait(false);
                await victim.RunPersistenceExclusiveAsync(() => victim.ClientReadyAfterMapChangeAsync()).ConfigureAwait(false);
                Assert.That(victim.Attributes, Is.Not.SameAs(previousAttributes));
                await victim.KillInstantlyAsync().ConfigureAwait(false);
                CancellationTokenSource successor;
                await using (var state = await victim.PlayerState.TryBeginAdvanceToAsync(victim.PlayerState.CurrentState).ConfigureAwait(false))
                {
                    state.Allowed = false;
                    successor = RespawnSource(victim)!;
                }

                Assert.That(successor, Is.Not.Null);
                var successorToken = successor.Token;
                release.TrySetResult();
                await returned.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                await respawnPacket.Task.WaitAsync(TimeSpan.FromSeconds(8)).ConfigureAwait(false);
                Assert.Multiple(() =>
                {
                    Assert.That(successorToken.IsCancellationRequested, Is.False);
                    Assert.That(character.PositionX, Is.EqualTo(17));
                    Assert.That(character.PositionY, Is.EqualTo(19));
                    Assert.That(victim.Attributes![Stats.CurrentHealth], Is.EqualTo(victim.Attributes[Stats.MaximumHealth]));
                    Assert.That(deaths, Is.EqualTo(2));
                });
            }
        }
        finally
        {
            release.TrySetResult();
            if (entered.Task.IsCompleted)
            {
                await returned.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }

            await victim.DisposeAsync().ConfigureAwait(false);
        }
    }

    private sealed class RespawnPlayer(IGameContext context) : Player(context)
    {
        protected override ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer() => new MockViewPlugInContainer();
    }

    [System.Runtime.CompilerServices.UnsafeAccessor(System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "_respawnAfterDeathCts")]
    private static extern ref CancellationTokenSource? RespawnSource(Player player);
}
