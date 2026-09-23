// <copyright file="KanturuNightmarePhaseSelectorTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

/// <summary>
/// Tests for <see cref="KanturuNightmarePhaseSelector"/>.
/// </summary>
[TestFixture]
public class KanturuNightmarePhaseSelectorTests
{
    private static List<KanturuNightmareHpPhase> CreatePhases()
    {
        return new List<KanturuNightmareHpPhase>
        {
            new() { HealthPercentage = 75 },
            new() { HealthPercentage = 50 },
            new() { HealthPercentage = 25 },
        };
    }

    /// <summary>
    /// Tests that health percentages map to the expected phase index.
    /// </summary>
    /// <param name="health">The current health percentage.</param>
    /// <param name="expected">The expected target phase index.</param>
    [TestCase(100f, 0)]
    [TestCase(76f, 0)]
    [TestCase(74f, 1)]
    [TestCase(51f, 1)]
    [TestCase(49f, 2)]
    [TestCase(26f, 2)]
    [TestCase(24f, 3)]
    [TestCase(0f, 3)]
    public void GetTargetPhaseIndex_MapsHealthToPhase(float health, int expected)
    {
        Assert.That(KanturuNightmarePhaseSelector.GetTargetPhaseIndex(CreatePhases(), health), Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that a phase only starts below its threshold, not exactly at it.
    /// </summary>
    [Test]
    public void GetTargetPhaseIndex_ExactlyAtThreshold_DoesNotTrigger()
    {
        // The original loop uses <, not <=.
        Assert.That(KanturuNightmarePhaseSelector.GetTargetPhaseIndex(CreatePhases(), 75f), Is.EqualTo(0));
        Assert.That(KanturuNightmarePhaseSelector.GetTargetPhaseIndex(CreatePhases(), 50f), Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that the monitor only advances to higher phase indexes.
    /// </summary>
    [Test]
    public void ShouldAdvance_OnlyWhenTargetGreater()
    {
        Assert.That(KanturuNightmarePhaseSelector.ShouldAdvance(1, 0), Is.True);
        Assert.That(KanturuNightmarePhaseSelector.ShouldAdvance(1, 1), Is.False);
        Assert.That(KanturuNightmarePhaseSelector.ShouldAdvance(0, 1), Is.False);
    }
}
