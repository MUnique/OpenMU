// <copyright file="KanturuMayaWideAttackerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MiniGames.Kanturu;
using MUnique.OpenMU.GameLogic.NPC;
using MonsterAttribute = MUnique.OpenMU.Persistence.BasicModel.MonsterAttribute;
using MonsterDefinition = MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition;

/// <summary>
/// Tests for the <see cref="KanturuMayaWideAttacker"/> attacker selection.
/// </summary>
[TestFixture]
public class KanturuMayaWideAttackerTests
{
    private IGameContext _gameContext = null!;

    private GameMap _map = null!;

    /// <summary>
    /// Sets up a fresh game context and map before each test.
    /// </summary>
    [SetUp]
    public async Task SetUpAsync()
    {
        this._gameContext = GameContextTestHelper.CreateGameContext();
        this._map = (await this._gameContext.GetMapAsync(0).ConfigureAwait(false))!;
    }

    /// <summary>
    /// Tests that a living Maya monster is preferred over other monsters.
    /// </summary>
    [Test]
    public void FindAttacker_PrefersLivingMayaMonster()
    {
        var minion = this.CreateMonster(354, initialize: true);
        var hand = this.CreateMonster(362, initialize: true);

        Assert.That(KanturuMayaWideAttacker.FindAttacker([minion, hand]), Is.SameAs(hand));
    }

    /// <summary>
    /// Tests that another living monster is used when no Maya monster is alive.
    /// </summary>
    [Test]
    public void FindAttacker_FallsBackToOtherLivingMonster()
    {
        var deadHand = this.CreateMonster(362, initialize: false);
        var minion = this.CreateMonster(354, initialize: true);

        Assert.That(KanturuMayaWideAttacker.FindAttacker([deadHand, minion]), Is.SameAs(minion));
    }

    /// <summary>
    /// Tests that no attacker is found when no monster is alive.
    /// </summary>
    [Test]
    public void FindAttacker_WithoutLivingMonster_ReturnsNull()
    {
        var deadHand = this.CreateMonster(362, initialize: false);

        Assert.That(KanturuMayaWideAttacker.FindAttacker([deadHand]), Is.Null);
        Assert.That(KanturuMayaWideAttacker.FindAttacker([]), Is.Null);
    }

    /// <summary>
    /// Tests that the attacker stops without any broadcast when already cancelled.
    /// </summary>
    [Test]
    public async Task RunAsync_StopsImmediately_WhenCancelled()
    {
        static ValueTask FailOnBroadcast(Func<Player, Task> _)
        {
            throw new InvalidOperationException("Must not broadcast when cancelled.");
        }

        var attacker = new KanturuMayaWideAttacker(this._map, FailOnBroadcast, NullLogger.Instance);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync().ConfigureAwait(false);

        await attacker.RunAsync(TimeSpan.FromMilliseconds(10), () => false, cts.Token).ConfigureAwait(false);
    }

    private Monster CreateMonster(short number, bool initialize)
    {
        var definition = new MonsterDefinition { ObjectKind = NpcObjectKind.Monster, Number = number };
        definition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.MaximumHealth, Value = 1000 });
        var spawnArea = new MonsterSpawnArea
        {
            MonsterDefinition = definition,
            X1 = 100,
            Y1 = 100,
            X2 = 100,
            Y2 = 100,
            Quantity = 1,
        };
        var monster = new Monster(
            spawnArea,
            definition,
            this._map,
            NullDropGenerator.Instance,
            new Mock<INpcIntelligence>().Object,
            this._gameContext.PlugInManager,
            this._gameContext.PathFinderPool);
        if (initialize)
        {
            monster.Initialize();
        }

        return monster;
    }
}
