// <copyright file="WarpInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;

/// <summary>
/// Defines a warp list entry.
/// </summary>
[Cloneable]
public partial class WarpInfo
{
    /// <summary>
    /// Gets or sets the index.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the warp costs.
    /// </summary>
    public int Costs { get; set; }

    /// <summary>
    /// Gets or sets the level requirement which a character needs to fulfill so that it can warp to the <see cref="Gate"/>.
    /// </summary>
    public int LevelRequirement { get; set; }

    /// <summary>
    /// Gets or sets the gate.
    /// </summary>
    [Required]
    public virtual ExitGate? Gate { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return this.Name;
    }
}