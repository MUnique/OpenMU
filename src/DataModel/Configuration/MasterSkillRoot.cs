// <copyright file="MasterSkillRoot.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;

/// <summary>
/// The root of a master skill tree. One character can have more than one root.
/// </summary>
[Cloneable]
public partial class MasterSkillRoot
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <inheritdoc />
    public override string ToString()
    {
        return this.Name;
    }
}