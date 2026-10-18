// <copyright file="AreaSkillTargetSelectorTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests for the area skill query center selection.
/// </summary>
[TestFixture]
internal class AreaSkillTargetSelectorTest
{
    /// <summary>
    /// Tests that a frustum skill is queried around the caster, not the aimed point.
    /// </summary>
    [Test]
    public void FrustumSkill_QueryCenteredOnCaster()
    {
        var skill = new Skill
        {
            Range = 6,
            AreaSkillSettings = new AreaSkillSettings { UseFrustumFilter = true, FrustumDistance = 6f },
        };

        var (center, range) = AreaSkillTargetSelector.GetQueryCenterAndRange(new Point(10, 10), new Point(13, 10), skill);

        Assert.Multiple(() =>
        {
            Assert.That(center, Is.EqualTo(new Point(10, 10)));
            Assert.That(range, Is.EqualTo(6));
        });
    }

    /// <summary>
    /// Tests that a target area skill is queried around the aimed point.
    /// </summary>
    [Test]
    public void TargetAreaSkill_QueryCenteredOnAimedPoint()
    {
        var skill = new Skill
        {
            Range = 7,
            AreaSkillSettings = new AreaSkillSettings { UseTargetAreaFilter = true, TargetAreaDiameter = 2f },
        };

        var (center, range) = AreaSkillTargetSelector.GetQueryCenterAndRange(new Point(10, 10), new Point(14, 12), skill);

        Assert.Multiple(() =>
        {
            Assert.That(center, Is.EqualTo(new Point(14, 12)));
            Assert.That(range, Is.EqualTo(7));
        });
    }

    /// <summary>
    /// Tests that an unshaped skill is queried around the caster, so the aimed
    /// point cannot extend its reach.
    /// </summary>
    [Test]
    public void UnshapedSkill_QueryCenteredOnCaster()
    {
        var skill = new Skill { Range = 2, AreaSkillSettings = new AreaSkillSettings() };

        var (center, range) = AreaSkillTargetSelector.GetQueryCenterAndRange(new Point(10, 10), new Point(14, 12), skill);

        Assert.Multiple(() =>
        {
            Assert.That(center, Is.EqualTo(new Point(10, 10)));
            Assert.That(range, Is.EqualTo(2));
        });
    }

    /// <summary>
    /// Tests that the effect range wins over the skill range when it is set.
    /// </summary>
    [Test]
    public void EffectRangeSet_EffectRangeWins()
    {
        var skill = new Skill
        {
            Range = 6,
            AreaSkillSettings = new AreaSkillSettings { EffectRange = 2 },
        };

        var (_, range) = AreaSkillTargetSelector.GetQueryCenterAndRange(new Point(10, 10), new Point(14, 12), skill);

        Assert.That(range, Is.EqualTo(2));
    }

    /// <summary>
    /// Tests that a direction angle is rounded to the nearest rotation byte instead of
    /// being truncated: truncation drops up to one full step (about 1.4 degrees), which
    /// can push a diagonally adjacent target out of a side projectile lane.
    /// </summary>
    [Test]
    public void ConvertAngleToRotation_RoundsInsteadOfTruncating()
    {
        Assert.Multiple(() =>
        {
            Assert.That(AreaSkillTargetSelector.ConvertAngleToRotation(0f), Is.EqualTo(0));
            Assert.That(AreaSkillTargetSelector.ConvertAngleToRotation(1f), Is.EqualTo(1), "truncation would give 0");
            Assert.That(AreaSkillTargetSelector.ConvertAngleToRotation(90f), Is.EqualTo(64), "63.75 rounds up, truncation gives 63");
            Assert.That(AreaSkillTargetSelector.ConvertAngleToRotation(359.9f), Is.EqualTo(255));
        });
    }
}
