// <copyright file="RageSkillAttackTargetTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;
using MonsterAttribute = MUnique.OpenMU.Persistence.BasicModel.MonsterAttribute;
using MonsterDefinition = MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition;

/// <summary>
/// Tests the target selection of the rage fighter area skills (dark side).
/// </summary>
[TestFixture]
internal class RageSkillAttackTargetTest
{
    /// <summary>
    /// Tests that the explicit target comes first and the other targets are the
    /// nearest ones to the search center (the explicit target), not to the player.
    /// </summary>
    [Test]
    public void ExplicitTargetFirstAndNearestToCenter()
    {
        var player = CreateAttackable(10, 10);
        var explicitTarget = CreateAttackable(14, 10);
        var nearTarget = CreateAttackable(15, 10);
        var farFromTarget = CreateAttackable(11, 10);

        var targets = RageSkillAttackAction.SelectTargets(
            player,
            explicitTarget,
            explicitTarget.Position,
            new[] { farFromTarget, player, explicitTarget, nearTarget });

        Assert.That(targets, Is.EqualTo(new[] { explicitTarget, nearTarget, farFromTarget }));
    }

    /// <summary>
    /// Tests that not more than the maximum number of targets are selected.
    /// </summary>
    [Test]
    public void LimitedToMaximumTargets()
    {
        var player = CreateAttackable(10, 10);
        var candidates = Enumerable.Range(0, 8).Select(i => CreateAttackable((byte)(11 + i), 10)).ToList();

        var targets = RageSkillAttackAction.SelectTargets(player, null, player.Position, candidates);

        Assert.That(targets, Is.EqualTo(candidates.Take(RageSkillAttackAction.MaximumTargetsPerAttack)));
    }

    /// <summary>
    /// Tests that an out-of-range explicit target triggers an instant position resync
    /// and still hits active monsters near the player instead of aborting silently.
    /// </summary>
    [Test]
    public async ValueTask OutOfRangeExplicitTargetResyncsAndHitsNearbyTargetsAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Position = new Point(10, 10);
        await player.SkillList!.AddLearnedSkillAsync(new Skill { Number = 263, Range = 5 }).ConfigureAwait(false);

        var farMonster = CreateMonster(player, new Point(18, 10));
        var nearMonster = CreateMonster(player, new Point(12, 10));
        await player.CurrentMap!.AddAsync(farMonster).ConfigureAwait(false);
        await player.CurrentMap!.AddAsync(nearMonster).ConfigureAwait(false);

        await new RageSkillAttackAction().AttackAsync(player, farMonster.Id, 263).ConfigureAwait(false);

        Mock.Get(player.ViewPlugIns.GetPlugIn<IObjectMovedPlugIn>()!)
            .Verify(v => v.ObjectMovedAsync(farMonster, MoveType.Instant), Times.Once);
        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowRageAttackRangePlugIn>()!)
            .Verify(v => v.ShowRageAttackRangeAsync(263, It.Is<IEnumerable<IAttackable>>(t => t.Contains(nearMonster) && !t.Contains(farMonster))), Times.Once);
    }

    /// <summary>
    /// Tests that an explicit target within the +2 range tolerance is still accepted.
    /// </summary>
    [Test]
    public async ValueTask WithinToleranceExplicitTargetIsAttackedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Position = new Point(10, 10);
        await player.SkillList!.AddLearnedSkillAsync(new Skill { Number = 263, Range = 5 }).ConfigureAwait(false);

        var monster = CreateMonster(player, new Point(17, 10));
        await player.CurrentMap!.AddAsync(monster).ConfigureAwait(false);

        await new RageSkillAttackAction().AttackAsync(player, monster.Id, 263).ConfigureAwait(false);

        Mock.Get(player.ViewPlugIns.GetPlugIn<IObjectMovedPlugIn>()!)
            .Verify(v => v.ObjectMovedAsync(It.IsAny<ILocateable>(), It.IsAny<MoveType>()), Times.Never);
        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowRageAttackRangePlugIn>()!)
            .Verify(v => v.ShowRageAttackRangeAsync(263, It.Is<IEnumerable<IAttackable>>(t => t.Contains(monster))), Times.Once);
    }

    private static Monster CreateMonster(Player player, Point position)
    {
        var def = new MonsterDefinition { ObjectKind = NpcObjectKind.Monster };
        def.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.MaximumHealth, Value = 100 });
        var map = player.CurrentMap!;
        var spawnArea = new MonsterSpawnArea
        {
            MonsterDefinition = def,
            GameMap = map.Definition,
            X1 = position.X,
            Y1 = position.Y,
            X2 = position.X,
            Y2 = position.Y,
            Quantity = 1,
        };
        var monster = new Monster(
            spawnArea,
            def,
            map,
            NullDropGenerator.Instance,
            new Mock<INpcIntelligence>().Object,
            player.GameContext.PlugInManager,
            player.GameContext.PathFinderPool);
        monster.Initialize();
        monster.Attributes[Stats.CurrentHealth] = 100;
        return monster;
    }

    private static IAttackable CreateAttackable(byte x, byte y)
    {
        var mock = new Mock<IAttackable>();
        mock.Setup(a => a.Position).Returns(new Point(x, y));
        return mock.Object;
    }
}
