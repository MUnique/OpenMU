// <copyright file="DeferredHitSchedulerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;

/// <summary>
/// Tests for the deferred hit scheduler.
/// </summary>
[TestFixture]
internal class DeferredHitSchedulerTest
{
    private const short TripleShot = 24;
    private const short MultiShot = 235;
    private const short TripleShotStrengthener = 414;
    private const short TripleShotMastery = 418;

    private static readonly AreaSkillSettings TripleShotSettings = new() { UseFrustumFilter = true, ProjectileCount = 3 };
    private static readonly AreaSkillSettings MultiShotSettings = new() { UseFrustumFilter = true, ProjectileCount = 5 };

    /// <summary>
    /// Tests that the triple shot gets the extra projectile of the generic pool, e.g. from a bow.
    /// </summary>
    [Test]
    public async Task TripleShotUsesGenericPoolAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Attributes!.AddElement(new SimpleElement(1, AggregateType.AddRaw), Stats.ExtraProjectiles);

        var result = DeferredHitScheduler.GetExtraProjectiles(player, CreateTripleShotEntry(), TripleShotSettings);

        Assert.That(result, Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that the triple shot gets the extra projectile of its mastery.
    /// </summary>
    [Test]
    public async Task TripleShotUsesMasteryPoolAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Attributes!.AddElement(new SimpleElement(1, AggregateType.AddRaw), Stats.TripleShotExtraProjectiles);

        var result = DeferredHitScheduler.GetExtraProjectiles(player, CreateTripleShotEntry(), TripleShotSettings);

        Assert.That(result, Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that the generic and the mastery pools stack for the triple shot.
    /// </summary>
    [Test]
    public async Task TripleShotStacksBothPoolsAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Attributes!.AddElement(new SimpleElement(1, AggregateType.AddRaw), Stats.ExtraProjectiles);
        player.Attributes.AddElement(new SimpleElement(1, AggregateType.AddRaw), Stats.TripleShotExtraProjectiles);

        var result = DeferredHitScheduler.GetExtraProjectiles(player, CreateTripleShotEntry(), TripleShotSettings);

        Assert.That(result, Is.EqualTo(2), "Three base projectiles + bow + mastery should result in five projectiles in total.");
    }

    /// <summary>
    /// Tests that a cast of the mastery skill itself inherits the pools of the base triple shot skill.
    /// </summary>
    [Test]
    public async Task TripleShotMasteryInheritsBaseSkillPoolsAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Attributes!.AddElement(new SimpleElement(1, AggregateType.AddRaw), Stats.ExtraProjectiles);
        player.Attributes.AddElement(new SimpleElement(1, AggregateType.AddRaw), Stats.TripleShotExtraProjectiles);

        var masteryEntry = CreateMasteryEntry(CreateTripleShotEntry().Skill!);
        var result = DeferredHitScheduler.GetExtraProjectiles(player, masteryEntry, TripleShotSettings);

        Assert.That(result, Is.EqualTo(2));
    }

    /// <summary>
    /// Tests that the cached skill attributes still follow the character's attributes, so equipping or
    /// unequipping a bow between casts changes the projectile count.
    /// </summary>
    [Test]
    public async Task TripleShotReflectsLaterAttributeChangesAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var entry = CreateTripleShotEntry();

        Assert.That(DeferredHitScheduler.GetExtraProjectiles(player, entry, TripleShotSettings), Is.EqualTo(0));

        player.Attributes!.AddElement(new SimpleElement(1, AggregateType.AddRaw), Stats.ExtraProjectiles);
        Assert.That(DeferredHitScheduler.GetExtraProjectiles(player, entry, TripleShotSettings), Is.EqualTo(1));

        player.Attributes.AddElement(new SimpleElement(1, AggregateType.AddRaw), Stats.TripleShotExtraProjectiles);
        Assert.That(DeferredHitScheduler.GetExtraProjectiles(player, entry, TripleShotSettings), Is.EqualTo(2));
    }

    /// <summary>
    /// Tests that the multi shot gets the extra projectile of the generic pool, e.g. from a bow.
    /// </summary>
    [Test]
    public async Task MultiShotUsesGenericPoolAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Attributes!.AddElement(new SimpleElement(1, AggregateType.AddRaw), Stats.ExtraProjectiles);

        var result = DeferredHitScheduler.GetExtraProjectiles(player, CreateMultiShotEntry(), MultiShotSettings);

        Assert.That(result, Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that the triple shot mastery bonus does not leak into the multi shot.
    /// </summary>
    [Test]
    public async Task MultiShotIgnoresMasteryPoolAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Attributes!.AddElement(new SimpleElement(1, AggregateType.AddRaw), Stats.TripleShotExtraProjectiles);

        var result = DeferredHitScheduler.GetExtraProjectiles(player, CreateMultiShotEntry(), MultiShotSettings);

        Assert.That(result, Is.EqualTo(0), "The mastery bonus is exclusive to the triple shot.");
    }

    /// <summary>
    /// Tests that there are no extra projectiles without the frustum filter.
    /// </summary>
    [Test]
    public async Task GetExtraProjectilesZeroWithoutFrustumFilterAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Attributes!.AddElement(new SimpleElement(1, AggregateType.AddRaw), Stats.ExtraProjectiles);
        var settings = new AreaSkillSettings { UseFrustumFilter = false, ProjectileCount = 3 };

        var result = DeferredHitScheduler.GetExtraProjectiles(player, CreateTripleShotEntry(), settings);

        Assert.That(result, Is.EqualTo(0));
    }

    /// <summary>
    /// Tests that there are no extra projectiles for single-projectile skills.
    /// </summary>
    [Test]
    public async Task GetExtraProjectilesZeroForSingleProjectileAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Attributes!.AddElement(new SimpleElement(1, AggregateType.AddRaw), Stats.ExtraProjectiles);
        var settings = new AreaSkillSettings { UseFrustumFilter = true, ProjectileCount = 1 };

        var result = DeferredHitScheduler.GetExtraProjectiles(player, CreateTripleShotEntry(), settings);

        Assert.That(result, Is.EqualTo(0));
    }

    private static SkillEntry CreateTripleShotEntry()
        => CreateSkillEntry(
            TripleShot,
            (Stats.SkillExtraProjectiles, Stats.ExtraProjectiles),
            (Stats.SkillExtraProjectiles, Stats.TripleShotExtraProjectiles));

    private static SkillEntry CreateMultiShotEntry()
        => CreateSkillEntry(MultiShot, (Stats.SkillExtraProjectiles, Stats.ExtraProjectiles));

    private static SkillEntry CreateSkillEntry(short number, params (AttributeDefinition Target, AttributeDefinition Source)[] relationships)
    {
        var skill = new Persistence.BasicModel.Skill { Number = number };
        foreach (var (target, source) in relationships)
        {
            skill.AttributeRelationships.Add(new Persistence.BasicModel.AttributeRelationship
            {
                TargetAttribute = target,
                InputAttribute = source,
                InputOperand = 1,
                InputOperator = InputOperator.Multiply,
            });
        }

        return new Persistence.BasicModel.SkillEntry { Skill = skill };
    }

    private static SkillEntry CreateMasteryEntry(Skill baseSkill)
    {
        var strengthener = new Persistence.BasicModel.Skill
        {
            Number = TripleShotStrengthener,
            MasterDefinition = new Persistence.BasicModel.MasterSkillDefinition { ReplacedSkill = baseSkill },
        };
        var mastery = new Persistence.BasicModel.Skill
        {
            Number = TripleShotMastery,
            MasterDefinition = new Persistence.BasicModel.MasterSkillDefinition { ReplacedSkill = strengthener },
        };
        return new Persistence.BasicModel.SkillEntry { Skill = mastery };
    }
}
