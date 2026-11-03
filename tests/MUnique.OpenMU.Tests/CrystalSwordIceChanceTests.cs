// <copyright file="CrystalSwordIceChanceTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Tests the Crystal Sword's chance to replace an attack with the ice skill.
/// </summary>
[TestFixture]
public class CrystalSwordIceChanceTests
{
    /// <summary>
    /// Tests that an attack with a guaranteed Crystal Sword proc carries the configured ice skill in its hit info.
    /// </summary>
    [Test]
    public async Task AttackUsesIceSkillWhenCrystalSwordProcSucceedsAsync()
    {
        await using var attacker = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var gameContext = (GameContext)attacker.GameContext;
        gameContext.Configuration.Maps.Single().BattleZone = new BattleZoneDefinition();
        // The skill needs initialized collections: when the attack roll hits, the damage calculation reads them.
        var iceSkillMock = new Mock<Skill>();
        iceSkillMock.SetupAllProperties();
        iceSkillMock.Setup(s => s.AttributeRelationships).Returns(new List<AttributeRelationship>());
        iceSkillMock.Setup(s => s.Requirements).Returns(new List<AttributeRequirement>());
        iceSkillMock.Setup(s => s.ConsumeRequirements).Returns(new List<AttributeRequirement>());
        var iceSkill = iceSkillMock.Object;
        iceSkill.Number = 7;
        iceSkill.DamageType = DamageType.Physical;
        gameContext.Configuration.Skills.Add(iceSkill);

        await using var target = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        attacker.Attributes!.AddElement(new ConstValueAttribute(1, Stats.CrystalSwordIceChance), Stats.CrystalSwordIceChance);

        var hitInfo = await target.AttackByAsync(attacker, null, false).ConfigureAwait(false);

        Assert.That(hitInfo, Is.Not.Null);
        Assert.That(hitInfo!.Value.SkillEntry?.Skill, Is.SameAs(iceSkill));
    }
}
