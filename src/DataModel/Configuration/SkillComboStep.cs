// <copyright file="SkillComboStep.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;

/// <summary>
/// Definition for one step for one skill of a combo sequence.
/// There can be multiple steps with the same <see cref="Order"/> but different skills.
/// </summary>
[Cloneable]
public partial class SkillComboStep
{
    /// <summary>
    /// Gets or sets the order for the step in the sequence.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this step is a final step which ends the combo.
    /// </summary>
    public bool IsFinalStep { get; set; }

    /// <summary>
    /// Gets or sets the skill of this step.
    /// </summary>
    public virtual Skill? Skill { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.Order} - {this.Skill}";
    }
}