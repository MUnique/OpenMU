// <copyright file="WeightedValueTable.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// A table of values with relative weights, from which a value is selected randomly.
/// The higher the weight of a value, the more often it's selected.
/// </summary>
/// <remarks>
/// It allows non-linear distributions where the game would otherwise roll uniformly within a range,
/// e.g. the number of sockets of a dropped socket item: one socket is common, three sockets are rare.
/// </remarks>
[Cloneable]
public partial class WeightedValueTable
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public LocalizedString Name { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public LocalizedString Description { get; set; }

    /// <summary>
    /// Gets or sets the values with their weights.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<WeightedValue> Values { get; protected set; } = null!;
}
