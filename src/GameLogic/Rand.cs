// <copyright file="Rand.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// A static class to provide random functions by encapsulating the default Randomizer <see cref="System.Random"/>.
/// </summary>
public static class Rand
{
    private static readonly IRandomizer Randomizer = new SimpleRandomizer();

    [ThreadStatic]
    private static Random? _randomInstance;

    private static Random RandomInstance => _randomInstance ??= new Random();

    /// <summary>
    /// Gets the default Randomizer which is implementing the interface <see cref="IRandomizer"/>.
    /// </summary>
    /// <returns>The default Randomizer which is implementing the interface <see cref="IRandomizer"/>.</returns>
    public static IRandomizer GetRandomizer()
    {
        return Randomizer;
    }

    /// <summary>
    /// Returns a random boolean value.
    /// </summary>
    /// <returns>Random boolean value.</returns>
    public static bool NextRandomBool()
    {
        int a = RandomInstance.Next(0, 2);
        return a == 1;
    }

    /// <summary>
    /// Returns a random boolean value, with <paramref name="percent"/> percent of a chance to be true.
    /// </summary>
    /// <param name="percent">Percentage of true.</param>
    /// <returns>Random boolean value.</returns>
    public static bool NextRandomBool(int percent)
    {
        if (percent == 0)
        {
            return false;
        }

        if (percent == 100)
        {
            return true;
        }

        int a = RandomInstance.Next(0, 100);
        return a <= percent;
    }

    /// <summary>
    /// Returns a random boolean value, with <paramref name="chance"/> between 0 and 1 to be true.
    /// </summary>
    /// <param name="chance">Chance between 0 and 1 to be true.</param>
    /// <returns>Random boolean value.</returns>
    public static bool NextRandomBool(double chance)
    {
        if (chance == 0.0)
        {
            return false;
        }

        var lot = RandomInstance.NextDouble();
        return lot <= chance;
    }

    /// <summary>
    /// Returns a random boolean value with the chance <paramref name="chance" /> of <paramref name="basis"/>.
    /// </summary>
    /// <param name="chance">Chance to be true.</param>
    /// <param name="basis">Base of the chance.</param>
    /// <returns>Random boolean value.</returns>
    public static bool NextRandomBool(int chance, int basis)
    {
        int a = RandomInstance.Next(0, basis);
        return a <= chance;
    }

    /// <summary>
    /// Returns a random integer between min and max.
    /// </summary>
    /// <param name="min">The inclusive lower bound of the random number returned.</param>
    /// <param name="max">The exclusive upper bound of the random number returned.</param>
    /// <returns>A random number.</returns>
    public static int NextInt(int min, int max)
    {
        return RandomInstance.Next(min, max);
    }

    /// <summary>
    /// Returns a random integer between min and max.
    /// </summary>
    /// <param name="min">The inclusive lower bound of the random number returned.</param>
    /// <param name="max">The exclusive upper bound of the random number returned.</param>
    /// <returns>A random number.</returns>
    public static int NextInt(uint min, uint max)
    {
        return RandomInstance.Next((int)min, (int)max);
    }

    /// <summary>
    /// Returns a random integer between min and max.
    /// </summary>
    /// <param name="min">The inclusive lower bound of the random number returned.</param>
    /// <param name="max">The exclusive upper bound of the random number returned.</param>
    /// <returns>A random number.</returns>
    public static uint NextUInt(uint min, uint max)
    {
        return (uint)RandomInstance.Next((int)System.Math.Max(min, 0), (int)max);
    }

    /// <summary>
    /// Returns a random double between 0 and 1.
    /// </summary>
    /// <returns>A random double between 0 and 1.</returns>
    public static double NextDouble()
    {
        return RandomInstance.NextDouble();
    }

    private class SimpleRandomizer : IRandomizer
    {
        bool IRandomizer.NextRandomBool()
        {
            return Rand.NextRandomBool();
        }

        bool IRandomizer.NextRandomBool(int percent)
        {
            return Rand.NextRandomBool(percent);
        }

        bool IRandomizer.NextRandomBool(int chance, int basis)
        {
            return Rand.NextRandomBool(chance, basis);
        }

        bool IRandomizer.NextRandomBool(double chance)
        {
            return Rand.NextRandomBool(chance);
        }

        int IRandomizer.NextInt(int min, int max)
        {
            return Rand.NextInt(min, max);
        }

        int IRandomizer.NextInt(uint min, uint max)
        {
            return Rand.NextInt(min, max);
        }

        uint IRandomizer.NextUInt(uint min, uint max)
        {
            return Rand.NextUInt(min, max);
        }

        double IRandomizer.NextDouble()
        {
            return Rand.NextDouble();
        }
    }
}