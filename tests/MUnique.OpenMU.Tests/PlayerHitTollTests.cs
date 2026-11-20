// <copyright file="PlayerHitTollTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using Moq;

/// <summary>
/// Tests player health tolls applied after a successful hit.
/// </summary>
[TestFixture]
public class PlayerHitTollTests
{
    /// <summary>
    /// A skill-specific toll replaces the regular toll.
    /// </summary>
    [Test]
    public async Task SkillTollReplacesRegularTollAsync()
    {
        var attacker = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        attacker.Attributes![Stats.CurrentHealth] = 10;
        attacker.Attributes.AddElement(new SimpleElement(3, AggregateType.AddRaw), Stats.HealthLossAfterHit);
        attacker.Attributes.AddElement(new SimpleElement(1, AggregateType.AddRaw), Stats.IsImpEquipped);

        var skill = new Mock<Skill>();
        skill.Setup(existing => existing.AttributeRelationships).Returns(
        [
            new AttributeRelationship(Stats.SkillHealthLossAfterHit, 2, Stats.IsImpEquipped),
        ]);
        var skillEntry = new SkillEntry { Skill = skill.Object };

        await attacker.AfterHitTargetAsync(0, skillEntry).ConfigureAwait(false);

        Assert.That(attacker.Attributes[Stats.CurrentHealth], Is.EqualTo(8));
    }

    /// <summary>
    /// Hitting a player with a Dinorant adds the configured target toll.
    /// </summary>
    [Test]
    public async Task DinorantTargetTollIsAddedAsync()
    {
        var attacker = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var target = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        attacker.Attributes![Stats.CurrentHealth] = 10;
        attacker.Attributes.AddElement(new SimpleElement(3, AggregateType.AddRaw), Stats.HealthLossAfterHit);
        attacker.Attributes.AddElement(new SimpleElement(1, AggregateType.AddRaw), Stats.TargetHasDinorantHealthToll);
        target.Attributes!.AddElement(new SimpleElement(1, AggregateType.AddRaw), Stats.IsDinorantEquipped);

        await attacker.AfterHitTargetAsync(0, null, target).ConfigureAwait(false);

        Assert.That(attacker.Attributes[Stats.CurrentHealth], Is.EqualTo(6));
    }

    /// <summary>
    /// A health toll cannot reduce a player's health below one.
    /// </summary>
    [Test]
    public async Task HealthTollKeepsCurrentHealthAtOneAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Attributes![Stats.CurrentHealth] = 1;
        player.Attributes.AddElement(new SimpleElement(3, AggregateType.AddRaw), Stats.HealthLossAfterHit);

        await player.AfterHitTargetAsync(0, null).ConfigureAwait(false);

        Assert.That(player.Attributes[Stats.CurrentHealth], Is.EqualTo(1));
    }
}