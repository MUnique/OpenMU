// <copyright file="CombatHandlerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offlevel;

using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.OfflineLeveling;
using MUnique.OpenMU.GameServer.RemoteView.MuHelper;
using MUnique.OpenMU.Pathfinding;
using MonsterDefinition = MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition;
using MonsterAttribute = MUnique.OpenMU.Persistence.BasicModel.MonsterAttribute;

/// <summary>
/// Tests for <see cref="CombatHandler"/>.
/// </summary>
[TestFixture]
public class CombatHandlerTests
{
    private IGameContext _gameContext = null!;
    private Point _origin = new(100, 100);

    /// <summary>
    /// Sets up a fresh game context before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._gameContext = GameContextTestHelper.CreateGameContext();
    }

    /// <summary>
    /// Tests that <see cref="CombatHandler.PerformAttackAsync"/> moves closer to the target if out of range.
    /// </summary>
    [Test]
    public async ValueTask PerformAttackAsync_MovesCloserToTargetAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        player.Position = this._origin;

        var monster = await this.CreateMonsterAsync(new Point(105, 105)).ConfigureAwait(false);
        await player.CurrentMap!.AddAsync(monster).ConfigureAwait(false);

        var config = new MuHelperSettings { HuntingRange = 10 };
        var movementHandler = new MovementHandler(player, config, this._origin);
        var buffHandler = new BuffHandler(player, config);

        var handler = new CombatHandler(player, config, movementHandler, buffHandler, this._origin);

        // Act
        await handler.PerformAttackAsync().ConfigureAwait(false);

        // Assert
        // Should have initiated a walk closer to the monster
        Assert.That(player.IsWalking, Is.True);
    }

    /// <summary>
    /// Tests that <see cref="CombatHandler.PerformDrainLifeRecoveryAsync"/> uses Drain Life when HP is low.
    /// </summary>
    [Test]
    public async ValueTask PerformDrainLifeRecoveryAsync_UsesDrainLifeWhenLowHpAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        player.Position = this._origin;
        player.Attributes![Stats.CurrentHealth] = 10; // Low HP
        player.Attributes[Stats.MaximumHealth] = 100;

        // Place monster slightly away from player so GetDirectionTo doesn't return Undefined
        var monsterPosition = new Point((byte)(this._origin.X + 1), (byte)this._origin.Y);
        var monster = await this.CreateMonsterAsync(monsterPosition).ConfigureAwait(false);
        await player.CurrentMap!.AddAsync(monster).ConfigureAwait(false);

        var config = new MuHelperSettings 
        { 
            UseDrainLife = true, 
            HealThresholdPercent = 50,
            HuntingRange = 10
        };
        
        // Add Drain Life skill to player
        var drainSkill = new TestSkill
        {
            Number = 214,
        };
        await player.SkillList!.AddLearnedSkillAsync(drainSkill).ConfigureAwait(false);

        var movementHandler = new MovementHandler(player, config, this._origin);
        var buffHandler = new BuffHandler(player, config);

        var handler = new CombatHandler(player, config, movementHandler, buffHandler, this._origin);

        // Act
        await handler.PerformDrainLifeRecoveryAsync().ConfigureAwait(false);

        // Assert
        // We verify that rotation was updated, which happens during ExecuteAttackAsync
        Assert.That(player.Rotation, Is.Not.EqualTo(default(Direction)));
    }

    private async ValueTask<OfflineLevelingPlayer> CreateOfflinePlayerAsync()
    {
        return await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
    }

    private async ValueTask<Monster> CreateMonsterAsync(Point position)
    {
        var monsterDefinition = new MonsterDefinition
        {
            ObjectKind = NpcObjectKind.Monster,
        };
        monsterDefinition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.MaximumHealth, Value = 1000 });
        monsterDefinition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.DefenseBase, Value = 100 });
        
        var map = await this._gameContext.GetMapAsync(0).ConfigureAwait(false)!;
        var spawnArea = new MonsterSpawnArea
        {
            MonsterDefinition = monsterDefinition,
            GameMap = map!.Definition,
            X1 = position.X,
            Y1 = position.Y,
            X2 = position.X,
            Y2 = position.Y,
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
        monster.Attributes[Stats.CurrentHealth] = 100;
        
        return monster;
    }

    private class TestSkill : Skill
    {
        public TestSkill()
        {
            this.Requirements = new List<AttributeRequirement>();
            this.ConsumeRequirements = new List<AttributeRequirement>();
            this.Range = 1;
            this.SkillType = SkillType.DirectHit;
            this.DamageType = DamageType.Curse;
        }
    }
}
