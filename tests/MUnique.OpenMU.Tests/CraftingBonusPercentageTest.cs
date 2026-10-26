// <copyright file="CraftingBonusPercentageTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;

/// <summary>
/// Tests for the application of the <see cref="Stats.CraftingBonusPercentage"/> to the success rate of craftings.
/// </summary>
[TestFixture]
public class CraftingBonusPercentageTest
{
    /// <summary>
    /// Tests the resulting success rate for a given success rate and bonus.
    /// </summary>
    /// <param name="successRate">The success rate of the required items.</param>
    /// <param name="bonus">The bonus in percentage points.</param>
    /// <returns>The resulting success rate.</returns>
    [TestCase((byte)50, 0f, ExpectedResult = (byte)50)]
    [TestCase((byte)50, 10f, ExpectedResult = (byte)60)]
    [TestCase((byte)50, -10f, ExpectedResult = (byte)40)]
    [TestCase((byte)95, 10f, ExpectedResult = (byte)100)]
    [TestCase((byte)5, -10f, ExpectedResult = (byte)1)]
    [TestCase((byte)0, 50f, ExpectedResult = (byte)0)]
    [TestCase((byte)100, -50f, ExpectedResult = (byte)100)]
    public byte ApplyCraftingBonus(byte successRate, float bonus)
    {
        return BaseItemCraftingHandler.ApplyCraftingBonus(successRate, bonus);
    }
}
