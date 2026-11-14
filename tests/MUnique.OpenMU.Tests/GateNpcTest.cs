// <copyright file="GateNpcTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Pathfinding;
using MonsterDefinition = MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition;

/// <summary>
/// Tests for the <see cref="GateNpc"/> (e.g. the Kalima gate of the lost map).
/// </summary>
[TestFixture]
public class GateNpcTest
{
    /// <summary>
    /// Tests that the gate is removed from the map when its lifespan is over.
    /// Before the fix, the warp task awaited itself while closing the gate, so the gate stayed on the map forever.
    /// </summary>
    [Test]
    public async Task GateIsRemovedFromMapAfterLifespanAsync()
    {
        var (map, gate) = await CreateGateAsync(TimeSpan.FromMilliseconds(200)).ConfigureAwait(false);
        Assert.That(map.GetObject(gate.Id), Is.SameAs(gate));

        Assert.That(await WaitUntilRemovedAsync(map, gate).ConfigureAwait(false), Is.True, "The gate should disappear after its lifespan.");
    }

    /// <summary>
    /// Tests that disposing the gate from outside (e.g. map shutdown) removes it and doesn't hang.
    /// </summary>
    [Test]
    public async Task DisposingGateFromOutsideRemovesItAsync()
    {
        var (map, gate) = await CreateGateAsync(TimeSpan.FromMinutes(1)).ConfigureAwait(false);

        var disposeTask = gate.DisposeAsync().AsTask();
        var finished = await Task.WhenAny(disposeTask, Task.Delay(TimeSpan.FromSeconds(5))).ConfigureAwait(false);

        Assert.That(finished, Is.SameAs(disposeTask), "DisposeAsync should not hang.");
        Assert.That(map.GetObject(gate.Id), Is.Null);
    }

    private static async Task<bool> WaitUntilRemovedAsync(GameMap map, GateNpc gate)
    {
        for (var i = 0; i < 50; i++)
        {
            if (map.GetObject(gate.Id) is null)
            {
                return true;
            }

            await Task.Delay(100).ConfigureAwait(false);
        }

        return false;
    }

    private static async ValueTask<(GameMap Map, GateNpc Gate)> CreateGateAsync(TimeSpan lifespan)
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        player.IsAlive = true;
        player.Position = new Point(10, 10);
        await player.CurrentMap!.AddAsync(player).ConfigureAwait(false);

        var map = player.CurrentMap!;
        var gateDefinition = new MonsterDefinition { Id = Guid.NewGuid(), Number = 152, ObjectKind = NpcObjectKind.Gate };
        var spawnArea = new MonsterSpawnArea
        {
            MonsterDefinition = gateDefinition,
            GameMap = map.Definition,
            X1 = 100,
            Y1 = 100,
            X2 = 100,
            Y2 = 100,
            Quantity = 1,
            SpawnTrigger = SpawnTrigger.ManuallyForEvent,
        };

        var targetGate = new Mock<ExitGate>();
        targetGate.SetupAllProperties();

        var gate = new GateNpc(spawnArea, gateDefinition, map, player, targetGate.Object, lifespan);
        gate.Initialize();
        await map.AddAsync(gate).ConfigureAwait(false);
        return (map, gate);
    }
}
