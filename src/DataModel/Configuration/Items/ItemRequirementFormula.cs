// <copyright file="ItemRequirementFormula.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// How a requirement of an item grows, in an <see cref="ItemStatProfile"/>:
/// <c>Add + base * (drop level + item level * LevelWeight) * Percent / 100</c>, where the drop level includes the
/// <see cref="GameConfiguration.AncientDropLevelBonus"/> of an ancient item, or else the
/// <see cref="GameConfiguration.ExcellentDropLevelBonus"/> of an excellent item. A base value of 0 stays 0.
/// </summary>
/// <remarks>
/// The base value is the item's <see cref="AttributeRequirement.MinimumValue"/>.
/// A formula of the profile replaces the one of the profile named "default" as a whole.
/// </remarks>
[Cloneable]
public partial class ItemRequirementFormula
{
    /// <summary>
    /// Gets or sets the attribute of the item requirements (<see cref="AttributeRequirement.Attribute"/>) this formula is for.
    /// </summary>
    public virtual AttributeDefinition? Attribute { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the requirement keeps its base value, instead of growing by the formula.
    /// </summary>
    public bool KeepBase { get; set; }

    /// <summary>
    /// Gets or sets what the formula adds.
    /// </summary>
    public int Add { get; set; }

    /// <summary>
    /// Gets or sets the factor of the item level, which is added to the drop level.
    /// </summary>
    public int LevelWeight { get; set; }

    /// <summary>
    /// Gets or sets the percent of the base value which each drop level adds.
    /// </summary>
    public int Percent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the formula takes the item's level requirement instead of the drop level
    /// (without the <see cref="LevelWeight"/>), when the item has one.
    /// </summary>
    public bool LevelRequirementBasis { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        if (this.KeepBase)
        {
            return string.Format(Properties.Resources.ItemRequirementFormulaKeptBaseSummary, this.Attribute?.ToString());
        }

        var format = this.LevelRequirementBasis
            ? Properties.Resources.ItemRequirementFormulaLevelBasisSummary
            : Properties.Resources.ItemRequirementFormulaSummary;
        return string.Format(format, this.Attribute?.ToString(), this.Add, this.LevelWeight, this.Percent);
    }
}
