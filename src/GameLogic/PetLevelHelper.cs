// <copyright file="PetLevelHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Collections.Concurrent;
using MUnique.OpenMU.DataModel.Configuration.Items;
using org.mariuszgromada.math.mxparser;

/// <summary>
/// Helper class for pet related stuff.
/// </summary>
public static class PetLevelHelper
{
    private static readonly ConcurrentDictionary<(string, byte), uint[]> ValueResultCache = new();

    /// <summary>
    /// Gets the experience of level.
    /// </summary>
    /// <param name="petDefinition">The pet definition.</param>
    /// <param name="petLevel">The pet level.</param>
    /// <param name="maximumLevel">The maximum level.</param>
    /// <returns>The calculated experience of the specified pet level.</returns>
    public static uint GetExperienceOfPetLevel(this ItemDefinition petDefinition, byte petLevel, byte maximumLevel)
    {
        if (petLevel <= 0 || petLevel > maximumLevel)
        {
            return uint.MaxValue;
        }

        var formula = petDefinition.PetExperienceFormula;
        if (formula is null)
        {
            return uint.MaxValue;
        }

        if (ValueResultCache.TryGetValue((formula, maximumLevel), out var results))
        {
            return results[petLevel - 1];
        }

        results = new uint[maximumLevel];
        var argument = new Argument("level", 0);
        var expression = new Expression(formula);
        expression.addArguments(argument);
        for (int i = 0; i < maximumLevel; i++)
        {
            argument.setArgumentValue(i + 1);
            results[i] = (uint)expression.calculate();
        }

        ValueResultCache.TryAdd((formula, petLevel), results);

        return results[petLevel - 1];
    }
}