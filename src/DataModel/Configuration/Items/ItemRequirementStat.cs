// <copyright file="ItemRequirementStat.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// The stat requirements of an item which an <see cref="ItemStatProfile"/> has <see cref="ItemRequirementFormula"/>s for.
/// </summary>
/// <remarks>
/// The values are stored in the database, so new ones are only appended.
/// Each one stands for one of the total requirement value attributes, e.g. the total strength requirement value.
/// </remarks>
public enum ItemRequirementStat
{
    /// <summary>
    /// The strength requirement.
    /// </summary>
    Strength,

    /// <summary>
    /// The agility requirement.
    /// </summary>
    Agility,

    /// <summary>
    /// The energy requirement.
    /// </summary>
    Energy,

    /// <summary>
    /// The vitality requirement.
    /// </summary>
    Vitality,

    /// <summary>
    /// The leadership (command) requirement.
    /// </summary>
    Leadership,
}
