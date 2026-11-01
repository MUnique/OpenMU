// <copyright file="MonsterInstanceMultiplierTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MonsterAttribute = MUnique.OpenMU.Persistence.BasicModel.MonsterAttribute;
using MonsterDefinition = MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition;

/// <summary>
/// Tests that multipliers can be applied to the attributes of a single monster instance,
/// like the doppelganger event does to make its monsters stronger.
/// </summary>
[TestFixture]
public class MonsterInstanceMultiplierTest
{
    /// <summary>
    /// Tests that a multiplier which is added before the monster is initialized applies to
    /// its maximum and initial health and to its defense, but not to other instances of the same definition.
    /// </summary>
    [Test]
    public async Task MultiplierAppliesToInstanceAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        // A unique id, because the attributes of monster definitions are cached by their id.
        var monsterDefinition = new MonsterDefinition { Id = Guid.NewGuid(), ObjectKind = NpcObjectKind.Monster };
        monsterDefinition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.MaximumHealth, Value = 1000 });
        monsterDefinition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.DefenseBase, Value = 100 });

        var scaledMonster = await CreateMonsterAsync(gameContext, monsterDefinition).ConfigureAwait(false);
        scaledMonster.Attributes.AddElement(new SimpleElement(3, AggregateType.Multiplicate), Stats.MaximumHealth);
        scaledMonster.Attributes.AddElement(new SimpleElement(2, AggregateType.Multiplicate), Stats.DefenseBase);
        scaledMonster.Initialize();

        var normalMonster = await CreateMonsterAsync(gameContext, monsterDefinition).ConfigureAwait(false);
        normalMonster.Initialize();

        Assert.That(scaledMonster.Attributes[Stats.MaximumHealth], Is.EqualTo(3000));
        Assert.That(scaledMonster.Health, Is.EqualTo(3000));
        Assert.That(scaledMonster.Attributes[Stats.DefensePvm], Is.EqualTo(200));
        Assert.That(normalMonster.Attributes[Stats.MaximumHealth], Is.EqualTo(1000));
        Assert.That(normalMonster.Health, Is.EqualTo(1000));
    }

    private static async ValueTask<Monster> CreateMonsterAsync(IGameContext gameContext, MonsterDefinition monsterDefinition)
    {
        var map = await gameContext.GetMapAsync(0).ConfigureAwait(false);
        var spawnArea = new MonsterSpawnArea
        {
            MonsterDefinition = monsterDefinition,
            GameMap = map!.Definition,
            X1 = 100,
            Y1 = 100,
            X2 = 100,
            Y2 = 100,
            Quantity = 1,
        };

        return new Monster(
            spawnArea,
            monsterDefinition,
            map,
            NullDropGenerator.Instance,
            new Mock<INpcIntelligence>().Object,
            gameContext.PlugInManager,
            gameContext.PathFinderPool);
    }
}
