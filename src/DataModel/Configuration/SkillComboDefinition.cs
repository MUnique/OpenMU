// <copyright file="SkillComboDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;

/// <summary>
/// Definition for a skill combo sequence.
/// </summary>
[Cloneable]
public partial class SkillComboDefinition
{
    /// <summary>
    /// Gets or sets the name of the combo sequence.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the maximum time until the final step has to be done.
    /// </summary>
    public TimeSpan MaximumCompletionTime { get; set; }

    /// <summary>
    /// Gets or sets the steps of the combo sequence.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<SkillComboStep> Steps { get; protected set; } = null!;

    /// <inheritdoc />
    public override string ToString()
    {
        return this.Name;
    }
}