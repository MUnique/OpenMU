// <copyright file="DurabilityDecayPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Tests the wear rate of the <see cref="DurabilityDecayPlugIn"/>.
/// </summary>
[TestFixture]
public class DurabilityDecayPlugInTest
{
    /// <summary>
    /// Verifies that an item loses one point after the whole periods which are needed to exceed the threshold.
    /// </summary>
    /// <param name="wearPerPeriod">The wear per period of ten seconds.</param>
    /// <param name="threshold">The threshold which the counter has to exceed.</param>
    /// <param name="expectedSecondsPerPoint">The expected seconds until the item loses one point.</param>
    [TestCase(70, 564, 90)]
    [TestCase(63, 564, 90)]
    [TestCase(1, 564, 5650)]
    [TestCase(70, 846, 130)]
    [TestCase(1, 846, 8470)]
    public void OnePointTakesWholePeriodsToExceedThreshold(double wearPerPeriod, double threshold, double expectedSecondsPerPoint)
    {
        var decrementPerSecond = DurabilityDecayPlugIn.GetDecrementPerSecond(wearPerPeriod, threshold);

        Assert.That(1 / decrementPerSecond, Is.EqualTo(expectedSecondsPerPoint).Within(1e-9));
    }
}
