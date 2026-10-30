// <copyright file="CrystalSwordIceChanceTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
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
        var iceSkill = new Skill { Number = 7, DamageType = DamageType.Physical };
        gameContext.Configuration.Skills.Add(iceSkill);

        await using var target = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        attacker.Attributes!.AddElement(new ConstValueAttribute(1, Stats.CrystalSwordIceChance), Stats.CrystalSwordIceChance);

        var hitInfo = await target.AttackByAsync(attacker, null, false).ConfigureAwait(false);

        Assert.That(hitInfo, Is.Not.Null);
        Assert.That(hitInfo!.Value.SkillEntry?.Skill, Is.SameAs(iceSkill));
    }
}
