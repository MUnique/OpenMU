// <copyright file="RandomizerTestHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Benchmarks.Helpers;

using Moq;
using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// Helper for creating test randomizers.
/// </summary>
public static class RandomizerTestHelper
{
    /// <summary>
    /// Creates a mock randomizer with random behavior.
    /// </summary>
    /// <returns>A mock randomizer.</returns>
    public static IRandomizer Create()
    {
        var randomizer = new Mock<IRandomizer>();
        var random = new Random();
        randomizer.Setup(r => r.NextInt(It.IsAny<int>(), It.IsAny<int>())).Returns((int min, int max) => random.Next(min, max));
        randomizer.Setup(r => r.NextDouble()).Returns(() => random.NextDouble());
        randomizer.Setup(r => r.NextRandomBool(It.IsAny<int>())).Returns((int chance) => random.Next(100) < chance);
        return randomizer.Object;
    }
}