// <copyright file="MonsterAttributeReloadTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Pathfinding;
using MonsterDefinition = MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition;
using MonsterAttribute = MUnique.OpenMU.Persistence.BasicModel.MonsterAttribute;

/// <summary>
/// Tests for applying changes of a <see cref="MonsterDefinition"/> to an already spawned
/// <see cref="AttackableNpcBase"/> via <see cref="AttackableNpcBase.ReloadAttributes"/>.
/// </summary>
[TestFixture]
public class MonsterAttributeReloadTests
{
    private IGameContext _gameContext = null!;

    /// <summary>
    /// Sets up a fresh game context before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._gameContext = GameContextTestHelper.CreateGameContext();
    }

    /// <summary>
    /// Tests that changing a value of a <see cref="MonsterAttribute"/> takes effect on an
    /// already spawned monster after <see cref="AttackableNpcBase.ReloadAttributes"/> is called.
    /// </summary>
    [Test]
    public async ValueTask ReloadAttributesAppliesChangedValueAsync()
    {
        var monster = await this.CreateMonsterAsync().ConfigureAwait(false);
        var maximumHealthAttribute = monster.Definition.Attributes.First(a => a.AttributeDefinition == Stats.MaximumHealth);

        Assert.That(monster.Attributes[Stats.MaximumHealth], Is.EqualTo(1000));

        maximumHealthAttribute.Value = 2000;
        monster.ReloadAttributes();

        Assert.That(monster.Attributes[Stats.MaximumHealth], Is.EqualTo(2000));
    }

    /// <summary>
    /// Tests that adding a new <see cref="MonsterAttribute"/> takes effect on an already spawned
    /// monster after <see cref="AttackableNpcBase.ReloadAttributes"/> is called.
    /// </summary>
    [Test]
    public async ValueTask ReloadAttributesAppliesAddedAttributeAsync()
    {
        var monster = await this.CreateMonsterAsync().ConfigureAwait(false);

        Assert.That(monster.Attributes[Stats.AttackRatePvm], Is.EqualTo(0));

        monster.Definition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.AttackRatePvm, Value = 50 });
        monster.ReloadAttributes();

        Assert.That(monster.Attributes[Stats.AttackRatePvm], Is.EqualTo(50));
    }

    /// <summary>
    /// Tests that all spawned instances of the same monster definition pick up an attribute change.
    /// </summary>
    [Test]
    public async ValueTask ReloadAttributesAppliesToAllInstancesOfDefinitionAsync()
    {
        var monsterDefinition = CreateMonsterDefinition();
        var monster1 = await this.CreateMonsterAsync(monsterDefinition).ConfigureAwait(false);
        var monster2 = await this.CreateMonsterAsync(monsterDefinition).ConfigureAwait(false);
        var maximumHealthAttribute = monsterDefinition.Attributes.First(a => a.AttributeDefinition == Stats.MaximumHealth);

        maximumHealthAttribute.Value = 2000;
        monster1.ReloadAttributes();
        monster2.ReloadAttributes();

        Assert.That(monster1.Attributes[Stats.MaximumHealth], Is.EqualTo(2000));
        Assert.That(monster2.Attributes[Stats.MaximumHealth], Is.EqualTo(2000));
    }

    private static MonsterDefinition CreateMonsterDefinition()
    {
        var monsterDefinition = new MonsterDefinition
        {
            ObjectKind = NpcObjectKind.Monster,
        };
        monsterDefinition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.MaximumHealth, Value = 1000 });
        monsterDefinition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.DefenseBase, Value = 100 });
        return monsterDefinition;
    }

    private ValueTask<Monster> CreateMonsterAsync()
    {
        return this.CreateMonsterAsync(CreateMonsterDefinition());
    }

    private async ValueTask<Monster> CreateMonsterAsync(MonsterDefinition monsterDefinition)
    {
        var map = await this._gameContext.GetMapAsync(0).ConfigureAwait(false);
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

        var monster = new Monster(
            spawnArea,
            monsterDefinition,
            map,
            NullDropGenerator.Instance,
            new Mock<INpcIntelligence>().Object,
            this._gameContext.PlugInManager,
            this._gameContext.PathFinderPool);

        monster.Initialize();

        return monster;
    }
}
