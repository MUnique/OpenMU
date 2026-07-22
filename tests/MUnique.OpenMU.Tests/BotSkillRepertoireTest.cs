// <copyright file="BotSkillRepertoireTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Bots;
using NUnit.Framework;

/// <summary>
/// Tests for the skills a bot may learn - the gate which decides what it owns to fight with.
/// </summary>
[TestFixture]
public class BotSkillRepertoireTest
{
    /// <summary>
    /// Tests that the castle siege skills are refused. They carry the highest damage numbers of their
    /// classes, so a "strongest first" rule walks straight into them, and the game activates them
    /// nowhere but the siege map.
    /// </summary>
    /// <param name="skillNumber">The number of the castle siege skill.</param>
    [TestCase((short)44)]  // Crescent Moon Slash
    [TestCase((short)45)]  // Lance
    [TestCase((short)46)]  // Starfall
    [TestCase((short)57)]  // Spiral Slash
    [TestCase((short)73)]  // Mana Rays
    [TestCase((short)74)]  // Fire Blast
    [TestCase((short)269)] // Charge
    public void CastleSiegeSkillIsNotLearned(short skillNumber)
    {
        var skill = CreateAttackSkill(skillNumber, attackDamage: 120);

        Assert.That(BotProgression.IsBotLearnableSkill(skill), Is.False);
    }

    /// <summary>
    /// Tests that the skills of the siege roles stay out as well. Stun is the one that matters: it is an
    /// area skill with no damage and a single hit, which is exactly what Twisting Slash looks like, so
    /// nothing but its number tells the two apart.
    /// </summary>
    [Test]
    public void SiegeRoleSkillIsNotLearned()
    {
        var stun = CreateAttackSkill(67, attackDamage: 0, skillType: SkillType.AreaSkillAutomaticHits);

        Assert.That(BotProgression.IsBotLearnableSkill(stun), Is.False);
    }

    /// <summary>
    /// Tests that a skill which carries no flat damage bonus but strikes several times is learned. The
    /// Rage Fighter's whole arsenal is built that way - its damage comes from the weapon - so judging by
    /// <see cref="Skill.AttackDamage"/> alone left the class with nothing to fight with.
    /// </summary>
    [Test]
    public void MultiHitSkillWithoutFlatDamageIsLearned()
    {
        var killingBlow = CreateAttackSkill(260, attackDamage: 0, hits: 4);

        Assert.That(BotProgression.IsBotLearnableSkill(killingBlow), Is.True);
    }

    /// <summary>
    /// Tests that an area skill is learned even without a flat damage bonus: its worth is the number of
    /// monsters it catches, not a bonus per hit.
    /// </summary>
    [Test]
    public void AreaSkillWithoutFlatDamageIsLearned()
    {
        var twistingSlash = CreateAttackSkill(41, attackDamage: 0, skillType: SkillType.AreaSkillAutomaticHits);

        Assert.That(BotProgression.IsBotLearnableSkill(twistingSlash), Is.True);
    }

    /// <summary>
    /// Tests that a single-hit skill without a damage bonus stays out: it is worth no more than a plain
    /// attack while costing mana. These are the combo swings, which the combo handler drives separately.
    /// </summary>
    [Test]
    public void PlainSingleHitSkillWithoutDamageIsNotLearned()
    {
        var lunge = CreateAttackSkill(20, attackDamage: 0);

        Assert.That(BotProgression.IsBotLearnableSkill(lunge), Is.False);
    }

    /// <summary>
    /// Tests that an ordinary attack skill is still learned.
    /// </summary>
    [Test]
    public void OrdinaryAttackSkillIsLearned()
    {
        var evilSpirit = CreateAttackSkill(9, attackDamage: 45);

        Assert.That(BotProgression.IsBotLearnableSkill(evilSpirit), Is.True);
    }

    /// <summary>
    /// Tests that a pet's skill is recognized as one. Plasma Storm belongs to the Fenrir, but its damage
    /// attribute is derived from the character's own stats, so it looks strong on a character riding
    /// nothing - and being the longest ranged skill most classes own, it won the tie-break for the whole
    /// population until the pet was checked for.
    /// </summary>
    [Test]
    public void PetSkillIsRecognizedAsOne()
    {
        var plasmaStorm = CreateAttackSkill(76, attackDamage: 60, damageType: DamageType.Fenrir);
        var strikeOfDestruction = CreateAttackSkill(232, attackDamage: 110);

        Assert.That(BotProgression.RequiresPet(plasmaStorm), Is.True);
        Assert.That(BotProgression.RequiresPet(strikeOfDestruction), Is.False);
    }

    private static Skill CreateAttackSkill(short number, int attackDamage, byte hits = 1, SkillType skillType = SkillType.DirectHit, DamageType damageType = DamageType.Physical)
    {
        return new Skill
        {
            Number = number,
            AttackDamage = attackDamage,
            NumberOfHitsPerAttack = hits,
            SkillType = skillType,
            DamageType = damageType,
            Range = 6,
        };
    }
}
