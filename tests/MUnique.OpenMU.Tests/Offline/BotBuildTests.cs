// <copyright file="BotBuildTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offline;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// Tests for <see cref="BotBuild"/> and <see cref="BotPartyPolicy"/>.
/// </summary>
[TestFixture]
public class BotBuildTests
{
    /// <summary>
    /// Tests that the build variant is stable for the same character name.
    /// </summary>
    [Test]
    public void GetVariant_IsStablePerName()
    {
        Assert.That(BotBuild.GetVariant("A"), Is.EqualTo(1));
        Assert.That(BotBuild.GetVariant("B"), Is.EqualTo(0));
        Assert.That(BotBuild.GetVariant("A"), Is.EqualTo(BotBuild.GetVariant("A")));
    }

    /// <summary>
    /// Tests that only the energy elf build counts as a support elf.
    /// </summary>
    [Test]
    public void IsSupportElf_OnlyEnergyElf()
    {
        var elf = new CharacterClass { Number = 8 };
        var knight = new CharacterClass { Number = 4 };

        Assert.That(BotBuild.IsSupportElf(elf, "A"), Is.True);
        Assert.That(BotBuild.IsSupportElf(elf, "B"), Is.False);
        Assert.That(BotBuild.IsSupportElf(knight, "A"), Is.False);
        Assert.That(BotBuild.IsSupportElf(null, "A"), Is.False);
        Assert.That(BotBuild.IsSupportElf(elf, null), Is.False);
    }

    /// <summary>
    /// Tests that generations of one class line share a build key.
    /// </summary>
    [Test]
    public void GetBuildKey_NormalizesGenerations()
    {
        var knight = new CharacterClass { Number = 4 };
        var bladeKnight = new CharacterClass { Number = 6 };
        var wizard = new CharacterClass { Number = 0 };

        Assert.That(BotBuild.GetBuildKey(bladeKnight, "A"), Is.EqualTo(BotBuild.GetBuildKey(knight, "A")));
        Assert.That(BotBuild.GetBuildKey(knight, "A"), Is.Not.EqualTo(BotBuild.GetBuildKey(knight, "B")));
        Assert.That(BotBuild.GetBuildKey(wizard, "A"), Is.EqualTo((0, 0)));
    }

    /// <summary>
    /// Tests the party count estimate of the party policy.
    /// </summary>
    [Test]
    public void EstimatePartyCount_MatchesPolicy()
    {
        Assert.That(BotPartyPolicy.EstimatePartyCount(0), Is.EqualTo(0));
        Assert.That(BotPartyPolicy.EstimatePartyCount(50), Is.EqualTo(14));
    }

    /// <summary>
    /// Tests that the wanted variant follows the support flag.
    /// </summary>
    [Test]
    public void WantedVariant_MatchesSupportFlag()
    {
        Assert.That(BotBuild.WantedVariant(true), Is.EqualTo(1));
        Assert.That(BotBuild.WantedVariant(false), Is.EqualTo(0));
    }
}
