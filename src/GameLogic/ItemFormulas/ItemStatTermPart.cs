// <copyright file="ItemStatTermPart.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.ItemFormulas;

using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// The excellent or the ancient part of the terms of an attribute in an item stat profile:
/// <c>value × Times ÷ d + Add + d ÷ DropLevelDivisor</c>, with d being the drop level the part uses.
/// </summary>
/// <param name="State">Whether the part is written, and whether it has values.</param>
/// <param name="Times">The factor of the value.</param>
/// <param name="Add">What the part adds.</param>
/// <param name="DropLevelDivisor">The divisor of the drop level, or 0 to leave that out.</param>
/// <param name="AfterLevelBonus">A value indicating whether the value includes the level bonus (only for the ancient part).</param>
public readonly record struct ItemStatTermPart(ItemStatTermState State, int Times, int Add, int DropLevelDivisor, bool AfterLevelBonus)
{
    /// <summary>
    /// Gets a value indicating whether the part has values, so that it adds something.
    /// </summary>
    public bool HasValues => this.State == ItemStatTermState.Values;

    /// <summary>
    /// Computes the part with integers, from left to right. A factor of 0, or a drop level which isn't above 0, leaves
    /// out the value, as in the game client, and a divisor of 0 leaves out the drop level.
    /// </summary>
    /// <param name="value">The value, e.g. the base value of the attribute.</param>
    /// <param name="dropLevel">The drop level.</param>
    /// <returns>The part.</returns>
    public int Compute(int value, int dropLevel)
    {
        // With long, a large factor, which an owner can configure, doesn't wrap to a negative part.
        var result = this.Times == 0 || dropLevel <= 0 ? 0 : (long)value * this.Times / dropLevel;
        result += this.Add;
        if (this.DropLevelDivisor != 0)
        {
            result += dropLevel / this.DropLevelDivisor;
        }

        return (int)Math.Clamp(result, int.MinValue, int.MaxValue);
    }
}
