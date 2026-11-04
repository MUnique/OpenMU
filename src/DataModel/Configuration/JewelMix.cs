// <copyright file="JewelMix.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Defines a jewel mix.
/// Some <see cref="SingleJewel"/> can be mixed together to a single <see cref="MixedJewel"/> to save storage place.
/// When single jewels are needed again, the client can unmix his <see cref="MixedJewel"/> back to several <see cref="SingleJewel"/>.
/// </summary>
[Cloneable]
public partial class JewelMix
{
    /// <summary>
    /// Gets or sets gets the number of the mix.
    /// </summary>
    /// <remarks>
    /// This number is a reference for the client.
    /// </remarks>
    public byte Number { get; set; }

    /// <summary>
    /// Gets or sets gets the single jewel item definition.
    /// </summary>
    public virtual ItemDefinition? SingleJewel { get; set; }

    /// <summary>
    /// Gets or sets gets the mixed jewel item definition.
    /// </summary>
    public virtual ItemDefinition? MixedJewel { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.SingleJewel?.Name} <> {this.MixedJewel?.Name}";
    }
}