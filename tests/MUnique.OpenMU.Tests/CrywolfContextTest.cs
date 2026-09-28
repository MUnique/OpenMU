// <copyright file="CrywolfContextTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Crywolf;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Pathfinding;
using MonsterAttribute = MUnique.OpenMU.Persistence.BasicModel.MonsterAttribute;
using MonsterDefinition = MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition;

/// <summary>
/// Tests the run of the <see cref="CrywolfContext"/>.
/// </summary>
[TestFixture]
public class CrywolfContextTest
{
    private static readonly Point AltarPosition = new(125, 27);

    /// <summary>
    /// Tests that the battle is lost, when no altar is contracted when it starts, and that the occupation is saved in the database.
    /// </summary>
    [Test]
    public async Task BattleIsLostWithoutContractedAltarAsync()
    {
        var (context, _, gameContext) = await CreateContextAsync().ConfigureAwait(false);

        await ProceedToAsync(context, CrywolfState.Start).ConfigureAwait(false);

        Assert.That(context.State, Is.EqualTo(CrywolfState.End));
        Assert.That(context.Occupation, Is.EqualTo(CrywolfOccupationState.Occupied));
        using var persistenceContext = gameContext.PersistenceContextProvider.CreateNewTypedContext(typeof(CrywolfData), false, gameContext.Configuration);
        var data = (await persistenceContext.GetAsync<CrywolfData>().ConfigureAwait(false)).ToList();
        Assert.That(data, Has.Count.EqualTo(1), "The occupation is saved.");
        Assert.That(data[0].IsOccupied, Is.True);
        Assert.That(data[0].LastBattleEnd, Is.Not.Null);
    }

    /// <summary>
    /// Tests that the saved occupation is loaded, so that it survives a restart of the server.
    /// </summary>
    [Test]
    public async Task SavedOccupationIsLoadedAsync()
    {
        var gameContext = (GameContext)GameContextTestHelper.CreateGameContext();
        using (var persistenceContext = gameContext.PersistenceContextProvider.CreateNewTypedContext(typeof(CrywolfData), false, gameContext.Configuration))
        {
            persistenceContext.CreateNew<CrywolfData>().IsOccupied = true;
            await persistenceContext.SaveChangesAsync().ConfigureAwait(false);
        }

        var (context, _, _) = await CreateContextAsync(gameContext).ConfigureAwait(false);

        Assert.That(context.Occupation, Is.EqualTo(CrywolfOccupationState.Occupied));
    }

    /// <summary>
    /// Tests that an altar can't be contracted while the event isn't running.
    /// </summary>
    [Test]
    public async Task AltarCantBeContractedOutsideTheEventAsync()
    {
        var (context, player, _) = await CreateContextAsync().ConfigureAwait(false);
        var altar = context.Altars[0];

        await context.ContractAltarAsync(player, altar.Npc.Id).ConfigureAwait(false);

        Assert.That(altar.State, Is.EqualTo(CrywolfAltarState.Free));
        Assert.That(altar.ContractCount, Is.Zero);
    }

    /// <summary>
    /// Tests that a contracted altar keeps the battle running, and that a repeated request doesn't change the contract.
    /// </summary>
    [Test]
    public async Task ContractedAltarKeepsTheBattleRunningAsync()
    {
        var (context, player, _) = await CreateContextAsync().ConfigureAwait(false);
        var altar = context.Altars[0];

        await ProceedToAsync(context, CrywolfState.Ready).ConfigureAwait(false);
        await context.ContractAltarAsync(player, altar.Npc.Id).ConfigureAwait(false);
        await context.ContractAltarAsync(player, altar.Npc.Id).ConfigureAwait(false);
        Assert.That(altar.ContractCount, Is.EqualTo(1), "The repeated request doesn't count as contract.");

        await context.TickAsync().ConfigureAwait(false);
        Assert.That(altar.State, Is.EqualTo(CrywolfAltarState.Contracted));

        await ProceedToAsync(context, CrywolfState.Start).ConfigureAwait(false);
        await context.TickAsync().ConfigureAwait(false);
        Assert.That(context.State, Is.EqualTo(CrywolfState.Start));
        Assert.That(context.ArmyStage, Is.EqualTo(CrywolfArmyStage.Advancing));
    }

    /// <summary>
    /// Tests that the game master command lets Balgass appear during the battle, instead of ending it.
    /// </summary>
    [Test]
    public async Task SkipDuringTheBattleLetsBalgassAppearAsync()
    {
        var (context, player, _) = await CreateContextAsync().ConfigureAwait(false);
        var altar = context.Altars[0];

        await ProceedToAsync(context, CrywolfState.Ready).ConfigureAwait(false);
        await context.ContractAltarAsync(player, altar.Npc.Id).ConfigureAwait(false);
        await context.TickAsync().ConfigureAwait(false);
        await ProceedToAsync(context, CrywolfState.Start).ConfigureAwait(false);

        context.SkipWaitingTime();
        await context.TickAsync().ConfigureAwait(false);

        Assert.That(context.State, Is.EqualTo(CrywolfState.Start));
        Assert.That(context.ArmyStage, Is.EqualTo(CrywolfArmyStage.AttackingStatue));
        var balgass = context.Map!.GetNpcsInRange(new Point(110, 79), 3).OfType<Monster>().FirstOrDefault(monster => monster.Definition.Number == 349);
        Assert.That(balgass, Is.Not.Null);
    }

    private static async ValueTask ProceedToAsync(CrywolfContext context, CrywolfState state)
    {
        for (var i = 0; i < 10 && context.State != state; i++)
        {
            context.SkipWaitingTime();
            await context.TickAsync().ConfigureAwait(false);
        }
    }

    private static async ValueTask<(CrywolfContext Context, Player Player, GameContext GameContext)> CreateContextAsync(GameContext? gameContext = null)
    {
        gameContext ??= (GameContext)GameContextTestHelper.CreateGameContext();
        var map = (await gameContext.GetMapAsync(0).ConfigureAwait(false))!;
        var definition = new CrywolfEventDefinition
        {
            MapNumber = 0,
            ContractCharacterClassNumbers = new List<byte> { 0 },
            MinimumContractLevel = 0,
            ContractDelay = TimeSpan.Zero,
            MonsterGroups = new List<CrywolfMonsterGroup>(),
        };

        await AddNpcAsync(map, definition.StatueNumber, new Point(121, 31)).ConfigureAwait(false);
        for (var i = 0; i < definition.AltarNumbers.Count; i++)
        {
            await AddNpcAsync(map, definition.AltarNumbers[i], new Point((byte)(AltarPosition.X - (i * 2)), AltarPosition.Y)).ConfigureAwait(false);
        }

        var balgass = new MonsterDefinition { Id = Guid.NewGuid(), Number = 349, ObjectKind = NpcObjectKind.Monster, AttackDelay = TimeSpan.FromHours(1) };
        balgass.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.MaximumHealth, Value = 100 });
        map.Definition.MonsterSpawns.Add(new Persistence.BasicModel.MonsterSpawnArea
        {
            MonsterDefinition = balgass,
            GameMap = map.Definition,
            X1 = 110,
            X2 = 110,
            Y1 = 79,
            Y2 = 79,
            Quantity = 1,
            SpawnTrigger = SpawnTrigger.OnceAtWaveStart,
            WaveNumber = definition.BalgassWaveNumber,
        });

        var context = new CrywolfContext(gameContext, definition);
        await context.InitializeAsync().ConfigureAwait(false);

        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        player.IsAlive = true;
        player.Position = AltarPosition;
        await player.CurrentMap!.AddAsync(player).ConfigureAwait(false);
        return (context, player, gameContext);
    }

    private static async ValueTask AddNpcAsync(GameMap map, short number, Point position)
    {
        var npcDefinition = new MonsterDefinition { Id = Guid.NewGuid(), Number = number, ObjectKind = NpcObjectKind.PassiveNpc };
        var spawnArea = new MonsterSpawnArea
        {
            MonsterDefinition = npcDefinition,
            GameMap = map.Definition,
            X1 = position.X,
            X2 = position.X,
            Y1 = position.Y,
            Y2 = position.Y,
            Quantity = 1,
        };
        var npc = new NonPlayerCharacter(spawnArea, npcDefinition, map);
        npc.Initialize();
        await map.AddAsync(npc).ConfigureAwait(false);
    }
}
