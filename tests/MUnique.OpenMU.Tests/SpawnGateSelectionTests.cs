// <copyright file="SpawnGateSelectionTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Pathfinding;
using MonsterAttribute = MUnique.OpenMU.Persistence.BasicModel.MonsterAttribute;
using MonsterDefinition = MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition;

/// <summary>
/// Tests for the selection of the gate at which a player is spawned.
/// </summary>
[TestFixture]
public class SpawnGateSelectionTests
{
    /// <summary>
    /// Tests that a plugin can define the gate at which the player is spawned, which is how
    /// duels and the guild war soccer match keep their players in place.
    /// </summary>
    [Test]
    public async ValueTask PlugInDefinesSpawnGateAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var gate = CreateGate(player, x: 77, y: 88);
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPlayerSpawnGateSelectionPlugIn>(new FixedSpawnGatePlugIn(gate));

        await player.WarpToSafezoneAsync().ConfigureAwait(false);

        Assert.That(player.SelectedCharacter!.PositionX, Is.EqualTo(77));
        Assert.That(player.SelectedCharacter.PositionY, Is.EqualTo(88));
    }

    /// <summary>
    /// Tests that a plugin doesn't overwrite the gate which another plugin selected before.
    /// </summary>
    [Test]
    public async ValueTask FirstSelectedSpawnGateWinsAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var firstGate = CreateGate(player, x: 10, y: 20);
        var secondGate = CreateGate(player, x: 30, y: 40);
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPlayerSpawnGateSelectionPlugIn>(new FixedSpawnGatePlugIn(firstGate));
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPlayerSpawnGateSelectionPlugIn>(new AnotherFixedSpawnGatePlugIn(secondGate));

        await player.WarpToSafezoneAsync().ConfigureAwait(false);

        Assert.That(player.SelectedCharacter!.PositionX, Is.EqualTo(10));
        Assert.That(player.SelectedCharacter.PositionY, Is.EqualTo(20));
    }

    /// <summary>
    /// Tests that the safezone gate of the map is used when no plugin selects one.
    /// </summary>
    [Test]
    public async ValueTask MapSafezoneGateIsUsedWithoutPlugInAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var definition = player.CurrentMap!.Definition;
        definition.ExitGates.Add(new ExitGate
        {
            Map = definition,
            X1 = 55,
            X2 = 55,
            Y1 = 66,
            Y2 = 66,
            IsSpawnGate = true,
        });

        await player.WarpToSafezoneAsync().ConfigureAwait(false);

        Assert.That(player.SelectedCharacter!.PositionX, Is.EqualTo(55));
        Assert.That(player.SelectedCharacter.PositionY, Is.EqualTo(66));
    }

    /// <summary>
    /// Tests that a player which is at a blocked position after a map change is moved to its
    /// spawn gate, without the map change of that move being disturbed.
    /// </summary>
    [Test]
    public async ValueTask BlockedPositionAfterMapChangeWarpsToSpawnGateAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var gate = CreateGate(player, x: 44, y: 45);
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPlayerSpawnGateSelectionPlugIn>(new FixedSpawnGatePlugIn(gate));

        // A summon makes the map handling after the arrival observable: it's added to the map
        // the player actually ends up on.
        await player.CreateSummonedMonsterAsync(CreateMonsterDefinition()).ConfigureAwait(false);

        // Block the position at which the player arrives, so that it has to be moved away.
        player.CurrentMap!.Terrain.WalkMap[5, 5] = false;
        await player.WarpToAsync(CreateGate(player, x: 5, y: 5)).ConfigureAwait(false);
        await player.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);

        Assert.That(player.SelectedCharacter!.PositionX, Is.EqualTo(44));
        Assert.That(player.SelectedCharacter.PositionY, Is.EqualTo(45));

        // The player is changing the map again, so it's not on a map yet.
        Assert.That(player.CurrentMap, Is.Null);
        Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.ChangingMap));
    }

    private static MonsterDefinition CreateMonsterDefinition()
    {
        var monsterDefinition = new MonsterDefinition
        {
            ObjectKind = NpcObjectKind.Monster,
        };
        monsterDefinition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.MaximumHealth, Value = 1000 });
        return monsterDefinition;
    }

    private static ExitGate CreateGate(Player player, byte x, byte y)
    {
        return new ExitGate
        {
            Map = player.CurrentMap!.Definition,
            X1 = x,
            X2 = x,
            Y1 = y,
            Y2 = y,
            Direction = Direction.West,
        };
    }

    [Guid("F1A6C3D8-9B24-4E07-8C5F-2A7D0E6B9134")]
    private sealed class FixedSpawnGatePlugIn : IPlayerSpawnGateSelectionPlugIn
    {
        private readonly ExitGate _gate;

        public FixedSpawnGatePlugIn(ExitGate gate) => this._gate = gate;

        public ValueTask SelectSpawnGateAsync(Player player, SpawnGateSelectionArgs args)
        {
            args.Gate ??= this._gate;
            return ValueTask.CompletedTask;
        }
    }

    [Guid("5B0E7F21-6C48-4A93-B1D6-8E2F0A4C7D95")]
    private sealed class AnotherFixedSpawnGatePlugIn : IPlayerSpawnGateSelectionPlugIn
    {
        private readonly ExitGate _gate;

        public AnotherFixedSpawnGatePlugIn(ExitGate gate) => this._gate = gate;

        public ValueTask SelectSpawnGateAsync(Player player, SpawnGateSelectionArgs args)
        {
            args.Gate ??= this._gate;
            return ValueTask.CompletedTask;
        }
    }
}
