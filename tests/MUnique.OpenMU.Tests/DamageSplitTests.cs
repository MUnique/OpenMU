// <copyright file="DamageSplitTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using Moq;

/// <summary>
/// Tests splitting hit damage between health and shield.
/// </summary>
[TestFixture]
public class DamageSplitTests
{
    /// <summary>
    /// A single point of damage remains health damage when shielding claims no part of it.
    /// </summary>
    [Test]
    public async Task SingleDamagePointRemainsHealthDamageAsync()
    {
        var attacker = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var attributes = new Mock<IAttributeSystem>();
        attributes.Setup(a => a[Stats.CurrentShield]).Returns(100);
        attributes.Setup(a => a[Stats.ShieldRatio]).Returns(0.75f);

        var defender = new Mock<IAttackable>();
        defender.Setup(d => d.Attributes).Returns(attributes.Object);

        var hitInfo = defender.Object.GetHitInfo(1, DamageAttributes.Undefined, attacker);

        Assert.Multiple(() =>
        {
            Assert.That(hitInfo.HealthDamage, Is.EqualTo((uint)1));
            Assert.That(hitInfo.ShieldDamage, Is.Zero);
            Assert.That(hitInfo.TotalDamage, Is.EqualTo((uint)1));
        });
    }

    /// <summary>
    /// Player poison damage uses shield until it is exhausted, then damages health.
    /// </summary>
    [Test]
    public async Task PlayerPoisonDamageUsesShieldUntilExhaustedAsync()
    {
        var attacker = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var defender = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        defender.Attributes![Stats.CurrentHealth] = 100;
        defender.Attributes[Stats.MaximumHealth] = 100;
        defender.Attributes[Stats.CurrentShield] = 100;

        await defender.ApplyPoisonDamageAsync(attacker, 40).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(defender.Attributes[Stats.CurrentHealth], Is.EqualTo(100));
            Assert.That(defender.Attributes[Stats.CurrentShield], Is.EqualTo(60));
        });

        await defender.ApplyPoisonDamageAsync(attacker, 60).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(defender.Attributes[Stats.CurrentHealth], Is.EqualTo(100));
            Assert.That(defender.Attributes[Stats.CurrentShield], Is.Zero);
        });

        await defender.ApplyPoisonDamageAsync(attacker, 40).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(defender.Attributes[Stats.CurrentHealth], Is.EqualTo(60));
            Assert.That(defender.Attributes[Stats.CurrentShield], Is.Zero);
        });
    }
}
