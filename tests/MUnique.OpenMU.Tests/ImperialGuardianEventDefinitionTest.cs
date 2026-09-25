// <copyright file="ImperialGuardianEventDefinitionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MiniGames.ImperialGuardian;
using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;

/// <summary>
/// Tests the <see cref="ImperialGuardianEventDefinition"/>.
/// </summary>
[TestFixture]
public class ImperialGuardianEventDefinitionTest
{
    /// <summary>
    /// Tests that the wave number contains the day and the zone.
    /// </summary>
    [Test]
    public void WaveNumber()
    {
        Assert.That(ImperialGuardianEventDefinition.GetWaveNumber(1, 0), Is.EqualTo(10));
        Assert.That(ImperialGuardianEventDefinition.GetWaveNumber(7, 3), Is.EqualTo(73));
    }

    /// <summary>
    /// Tests that the experience reward depends on the level of the player and is multiplied on sunday.
    /// </summary>
    [Test]
    public void ExperienceReward()
    {
        var definition = new ImperialGuardianEventDefinition
        {
            ExperienceRewards =
            [
                new() { MaximumPlayerLevel = 200, Experience = 1_000 },
                new() { MaximumPlayerLevel = 400, Experience = 2_000 },
            ],
        };

        Assert.That(definition.GetExperienceReward(150, 1), Is.EqualTo(1_000));
        Assert.That(definition.GetExperienceReward(201, 1), Is.EqualTo(2_000));
        Assert.That(definition.GetExperienceReward(201, ImperialGuardianEventDefinition.Sunday), Is.EqualTo(4_000));
        Assert.That(definition.GetExperienceReward(401, 1), Is.Zero, "Players above all levels of the table don't get experience.");
    }

    /// <summary>
    /// Tests the number of fragments which a boss drops, depending on the chances.
    /// </summary>
    /// <param name="randomPercent">The random number from 0 to 99.</param>
    /// <param name="expectedCount">The expected number of fragments.</param>
    [TestCase(0, 1)]
    [TestCase(49, 1)]
    [TestCase(50, 2)]
    [TestCase(78, 2)]
    [TestCase(79, 3)]
    [TestCase(99, 3)]
    public void FragmentCount(int randomPercent, int expectedCount)
    {
        Assert.That(new ImperialGuardianEventDefinition().GetFragmentCount(randomPercent), Is.EqualTo(expectedCount));
    }

    /// <summary>
    /// Tests that the monsters aren't scaled by default, and that the scaling depends on the level of the players.
    /// </summary>
    [Test]
    public void MonsterScaling()
    {
        Assert.That(new ImperialGuardianEventDefinition().GetMonsterScaling(400), Is.Null);

        var low = new ImperialGuardianMonsterScaling { MaximumPlayerLevel = 200, HealthMultiplier = 1 };
        var high = new ImperialGuardianMonsterScaling { MaximumPlayerLevel = 400, HealthMultiplier = 2 };
        var definition = new ImperialGuardianEventDefinition { MonsterScalings = [high, low] };

        Assert.That(definition.GetMonsterScaling(150), Is.SameAs(low));
        Assert.That(definition.GetMonsterScaling(300), Is.SameAs(high));
        Assert.That(definition.GetMonsterScaling(800), Is.SameAs(high), "Players above all levels of the table use the highest scaling.");
    }

    /// <summary>
    /// Tests that a fixed day overrides the day of the week of the server.
    /// </summary>
    [Test]
    public void FixedDay()
    {
        var gameContext = new Mock<IGameContext>();
        gameContext.Setup(c => c.ServerTimeZone).Returns(TimeZoneInfo.Utc);
        var definition = new ImperialGuardianEventDefinition { FixedDay = ImperialGuardianEventDefinition.Sunday };

        Assert.That(EnterImperialGuardianAction.GetDay(gameContext.Object, definition), Is.EqualTo(ImperialGuardianEventDefinition.Sunday));

        definition.FixedDay = 0;
        var expectedDay = DateTime.UtcNow.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)DateTime.UtcNow.DayOfWeek;
        Assert.That(EnterImperialGuardianAction.GetDay(gameContext.Object, definition), Is.EqualTo(expectedDay));
    }
}
