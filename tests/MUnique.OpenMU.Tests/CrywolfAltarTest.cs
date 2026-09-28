// <copyright file="CrywolfAltarTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Crywolf;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Pathfinding;
using MonsterDefinition = MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition;

/// <summary>
/// Tests the <see cref="CrywolfAltar"/> and the terrain of the crywolf event.
/// </summary>
[TestFixture]
public class CrywolfAltarTest
{
    private static readonly Point AltarPosition = new(125, 27);

    private readonly CrywolfEventDefinition _definition = new()
    {
        // The test player has the class number 0 and the level 0.
        ContractCharacterClassNumbers = new List<byte> { 0 },
        MinimumContractLevel = 0,
    };

    /// <summary>
    /// Tests that the contract gets valid when the elf stays at the altar,
    /// and that the attempt already counts as contract.
    /// </summary>
    [Test]
    public async Task ContractGetsValidAsync()
    {
        var (altar, player) = await this.CreateAltarAsync(AltarPosition).ConfigureAwait(false);
        var now = DateTime.UtcNow;

        Assert.That(altar.TryStartContract(player, this._definition, now), Is.EqualTo(CrywolfContractResult.Success));
        Assert.That(altar.State, Is.EqualTo(CrywolfAltarState.Attempting));
        Assert.That(altar.GetClientState(this._definition.ContractsPerAltar), Is.EqualTo(0x21), "Attempting, one remaining contract.");

        Assert.That(altar.Update(this._definition, now.AddSeconds(1)), Is.EqualTo(CrywolfContractChange.None));
        Assert.That(altar.Update(this._definition, now + this._definition.ContractDelay), Is.EqualTo(CrywolfContractChange.Validated));
        Assert.That(altar.State, Is.EqualTo(CrywolfAltarState.Contracted));
        Assert.That(altar.GetClientState(this._definition.ContractsPerAltar), Is.EqualTo(0x11));
        Assert.That(altar.Display.Effect, Is.EqualTo(CrywolfEffect.AltarContracted));
    }

    /// <summary>
    /// Tests that the contract is cancelled when the elf moves, and that the altar is exhausted after its contracts.
    /// </summary>
    [Test]
    public async Task ContractIsCancelledWhenMovingAsync()
    {
        var (altar, player) = await this.CreateAltarAsync(AltarPosition).ConfigureAwait(false);
        var now = DateTime.UtcNow;

        altar.TryStartContract(player, this._definition, now);
        player.Position = new Point(126, 27);
        Assert.That(altar.Update(this._definition, now.AddSeconds(1)), Is.EqualTo(CrywolfContractChange.Cancelled));
        Assert.That(altar.State, Is.EqualTo(CrywolfAltarState.Free));
        Assert.That(altar.Contractor, Is.Null);

        player.Position = AltarPosition;
        Assert.That(altar.TryStartContract(player, this._definition, now.AddSeconds(2)), Is.EqualTo(CrywolfContractResult.Cooldown));

        var later = now + this._definition.AltarCooldown + TimeSpan.FromSeconds(2);
        Assert.That(altar.TryStartContract(player, this._definition, later), Is.EqualTo(CrywolfContractResult.Success));
        altar.Cancel(this._definition, later);
        Assert.That(altar.State, Is.EqualTo(CrywolfAltarState.Exhausted), "Both contracts are used.");
        Assert.That(altar.Display.Effect, Is.EqualTo(CrywolfEffect.AltarDisabled));
        Assert.That(altar.TryStartContract(player, this._definition, later + this._definition.AltarCooldown), Is.EqualTo(CrywolfContractResult.NotAvailable));

        altar.Reset();
        Assert.That(altar.State, Is.EqualTo(CrywolfAltarState.Free));
        Assert.That(altar.GetClientState(this._definition.ContractsPerAltar), Is.EqualTo(0x02));
    }

    /// <summary>
    /// Tests that only qualified characters which stand at the altar can contract it.
    /// </summary>
    [Test]
    public async Task ContractRequirementsAsync()
    {
        var (altar, player) = await this.CreateAltarAsync(new Point(126, 28)).ConfigureAwait(false);
        var now = DateTime.UtcNow;

        Assert.That(altar.TryStartContract(player, this._definition, now), Is.EqualTo(CrywolfContractResult.WrongPosition));
        Assert.That(altar.TryStartContract(player, new CrywolfEventDefinition(), now), Is.EqualTo(CrywolfContractResult.NotQualified), "The player is no elf.");
        Assert.That(altar.ContractCount, Is.Zero);
    }

    /// <summary>
    /// Tests that the safezone of the fortress is removed while it's not in peace, and restored afterward.
    /// </summary>
    [Test]
    public void TerrainOfTheOccupation()
    {
        var terrainData = new byte[(256 * 256) + 3];
        Array.Fill(terrainData, (byte)TerrainAttributeType.Safezone, 3, terrainData.Length - 3);
        var terrain = new GameMapTerrain(terrainData);
        var (areaX, areaY, _, _) = CrywolfTerrain.SafezoneRemovedAreas[0];
        var (blockedX, blockedY, _, _) = CrywolfTerrain.BlockedAreas[0];

        CrywolfTerrain.Apply(terrain, false);
        Assert.That(terrain.SafezoneMap[areaX, areaY], Is.False);
        Assert.That(terrain.WalkMap[blockedX, blockedY], Is.False);

        CrywolfTerrain.Apply(terrain, true);
        Assert.That(terrain.SafezoneMap[areaX, areaY], Is.True);
        Assert.That(terrain.WalkMap[blockedX, blockedY], Is.True);
    }

    private async ValueTask<(CrywolfAltar Altar, Player Player)> CreateAltarAsync(Point playerPosition)
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var map = await gameContext.GetMapAsync(0).ConfigureAwait(false);
        var altarDefinition = new MonsterDefinition { Id = Guid.NewGuid(), Number = 205, ObjectKind = NpcObjectKind.PassiveNpc };
        var spawnArea = new MonsterSpawnArea
        {
            MonsterDefinition = altarDefinition,
            GameMap = map!.Definition,
            X1 = AltarPosition.X,
            X2 = AltarPosition.X,
            Y1 = AltarPosition.Y,
            Y2 = AltarPosition.Y,
            Quantity = 1,
        };
        var npc = new NonPlayerCharacter(spawnArea, altarDefinition, map);
        npc.Initialize();
        await map.AddAsync(npc).ConfigureAwait(false);

        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        player.IsAlive = true;
        player.Position = playerPosition;
        await player.CurrentMap!.AddAsync(player).ConfigureAwait(false);
        return (new CrywolfAltar(0, npc), player);
    }
}
