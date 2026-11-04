// <copyright file="HitShockTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using Moq;

/// <summary>
/// Tests the conditions which prevent a player from being hit-shocked.
/// </summary>
[TestFixture]
public class HitShockTests
{
    private static readonly AttributeDefinition[] MountAttributes =
    [
        Stats.IsUniriaEquipped,
        Stats.IsDinorantEquipped,
        Stats.IsHorseEquipped,
    ];

    /// <summary>
    /// A target which is not a player is never hit-shocked.
    /// </summary>
    [Test]
    public async Task NonPlayerDoesNotTriggerHitShockAsync()
    {
        var attacker = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var attributes = new Mock<IAttributeSystem>();
        attributes.Setup(a => a[It.IsAny<AttributeDefinition>()]).Returns(0);
        var defender = new Mock<IAttackable>();
        defender.Setup(d => d.Attributes).Returns(attributes.Object);

        var hitInfo = defender.Object.GetHitInfo(100, DamageAttributes.Undefined, attacker);

        Assert.That(hitInfo.TriggersHitShock, Is.False);
    }

    /// <summary>
    /// A hit below the damage threshold does not trigger a hit shock.
    /// </summary>
    [Test]
    public async Task DamageBelowThresholdDoesNotTriggerHitShockAsync()
    {
        var attacker = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var defender = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        AddHitShockSettings(defender, 1, 5);

        var hitInfo = defender.GetHitInfo(4, DamageAttributes.Undefined, attacker);

        Assert.That(hitInfo.TriggersHitShock, Is.False);
    }

    /// <summary>
    /// A hit at the configured damage threshold triggers hit shock when the chance is guaranteed.
    /// </summary>
    [Test]
    public async Task HitAtDamageThresholdTriggersWhenChanceIsGuaranteedAsync()
    {
        var attacker = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var defender = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        AddHitShockSettings(defender, 1, 5);

        var hitInfo = defender.GetHitInfo(5, DamageAttributes.Undefined, attacker);

        Assert.That(hitInfo.TriggersHitShock, Is.True);
    }

    /// <summary>
    /// A zero configured chance never triggers hit shock.
    /// </summary>
    [Test]
    public async Task ZeroHitShockChanceDoesNotTriggerHitShockAsync()
    {
        var attacker = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var defender = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        AddHitShockSettings(defender, 0, 5);

        var hitInfo = defender.GetHitInfo(100, DamageAttributes.Undefined, attacker);

        Assert.That(hitInfo.TriggersHitShock, Is.False);
    }

    /// <summary>
    /// A player equipped with Uniria, a Dinorant, or a Dark Horse is not hit-shocked.
    /// </summary>
    /// <param name="mountAttribute">The attribute indicating the equipped mount.</param>
    [TestCaseSource(nameof(MountAttributes))]
    public async Task MountedPlayerDoesNotTriggerHitShockAsync(AttributeDefinition mountAttribute)
    {
        var attacker = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var defender = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        AddHitShockSettings(defender, 1, 5);
        defender.Attributes!.AddElement(new SimpleElement(1, AggregateType.AddRaw), mountAttribute);

        var hitInfo = defender.GetHitInfo(100, DamageAttributes.Undefined, attacker);

        Assert.That(hitInfo.TriggersHitShock, Is.False);
    }

    private static void AddHitShockSettings(Player player, float chance, float damageThreshold)
    {
        player.Attributes!.AddElement(new SimpleElement(chance, AggregateType.AddRaw), Stats.HitShockChance);
        player.Attributes.AddElement(new SimpleElement(damageThreshold, AggregateType.AddRaw), Stats.HitShockDamageThreshold);
    }
}
