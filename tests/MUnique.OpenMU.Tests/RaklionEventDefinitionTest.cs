// <copyright file="RaklionEventDefinitionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic.Raklion;

/// <summary>
/// Tests the <see cref="RaklionEventDefinition"/>.
/// </summary>
[TestFixture]
public class RaklionEventDefinitionTest
{
    /// <summary>
    /// Tests that the pattern of Selupan is determined by its remaining health.
    /// </summary>
    /// <param name="healthPercentage">The remaining health in percent.</param>
    /// <param name="expectedPattern">The expected pattern.</param>
    [TestCase(100, 1)]
    [TestCase(80.5, 1)]
    [TestCase(80, 2)]
    [TestCase(61, 2)]
    [TestCase(60, 3)]
    [TestCase(50, 4)]
    [TestCase(40, 5)]
    [TestCase(20, 6)]
    [TestCase(10, 7)]
    [TestCase(0, 7)]
    public void PatternByHealth(double healthPercentage, int expectedPattern)
    {
        Assert.That(new RaklionEventDefinition().GetPattern(healthPercentage), Is.EqualTo(expectedPattern));
    }

    /// <summary>
    /// Tests the berserk levels of the patterns.
    /// </summary>
    [Test]
    public void BerserkLevels()
    {
        var definition = new RaklionEventDefinition();
        var levels = Enumerable.Range(1, 7).Select(definition.GetBerserkLevel).ToList();

        Assert.That(levels, Is.EqualTo(new[] { 0, 1, 2, 2, 3, 4, 4 }));
        Assert.That(definition.GetBerserkLevel(99), Is.EqualTo(4), "Patterns above the configured ones use the last level.");
    }

    /// <summary>
    /// Tests that Selupan uses more skills in the higher patterns.
    /// </summary>
    [Test]
    public void SkillsByPattern()
    {
        var definition = new RaklionEventDefinition();
        SelupanSkill[] basicSkills = [SelupanSkill.Poison, SelupanSkill.IceStorm, SelupanSkill.IceStrike, SelupanSkill.Teleport];

        Assert.That(definition.GetSkills(1), Is.EquivalentTo(basicSkills));
        Assert.That(definition.GetSkills(3), Is.EquivalentTo(basicSkills.Append(SelupanSkill.Freeze)));
        Assert.That(definition.GetSkills(4), Is.EquivalentTo(basicSkills.Append(SelupanSkill.Freeze).Append(SelupanSkill.Heal)));
        Assert.That(definition.GetSkills(6), Is.EquivalentTo(basicSkills.Append(SelupanSkill.Freeze).Append(SelupanSkill.Heal).Append(SelupanSkill.Summon)));
        Assert.That(definition.GetSkills(7), Does.Contain(SelupanSkill.Invincibility));
        Assert.That(definition.GetSkills(7), Does.Not.Contain(SelupanSkill.Fall), "The fall is only used when Selupan appears.");
    }
}
