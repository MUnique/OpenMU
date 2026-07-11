// <copyright file="BuffRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;

/// <summary>
/// A requestable buff from an NPC.
/// </summary>
[Cloneable]
public partial class BuffRequest
{
    /// <summary>
    /// Gets or sets the magic effect definition which defines the buff and its duration.
    /// </summary>
    [MemberOfAggregate]
    public virtual MagicEffectDefinition? MagicEffectDefinition { get; set; }

    /// <summary>
    /// Gets or sets the minimum character level to be allowed to receive this buff. Optional.
    /// </summary>
    public int? MinimumLevel { get; set; }

    /// <summary>
    /// Gets or sets the maximum character level to be allowed to receive this buff. Optional.
    /// </summary>
    public int? MaximumLevel { get; set; }
}
