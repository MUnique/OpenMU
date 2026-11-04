// <copyright file="ItemOption.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.DataModel.Attributes;

/// <summary>
/// Defines the option of an item.
/// </summary>
[Cloneable]
public partial class ItemOption
{
    /// <summary>
    /// Gets or sets the number.
    /// </summary>
    /// <remarks>
    /// This number in combination with the option type is a reference for the client.
    /// </remarks>
    public int Number { get; set; }

    /// <summary>
    /// Gets or sets a type of the sub option.
    /// </summary>
    /// <remarks>
    /// This is required for socket options, for example.
    /// There, it defines the element (fire, water, etc.) of the socket option.
    /// </remarks>
    public int SubOptionType { get; set; }

    /// <summary>
    /// Gets or sets the type of the option.
    /// </summary>
    public virtual ItemOptionType? OptionType { get; set; }

    /// <summary>
    /// Gets or sets the power up definition which should apply when this item is carried.
    /// </summary>
    [MemberOfAggregate]
    [Required]
    public virtual PowerUpDefinition? PowerUpDefinition { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.OptionType}: {this.PowerUpDefinition} ({this.Number})";
    }
}