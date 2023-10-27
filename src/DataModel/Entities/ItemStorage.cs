// <copyright file="ItemStorage.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

using MUnique.OpenMU.Annotations;

/// <summary>
/// A storage where items can be stored.
/// </summary>
[Cloneable]
public partial class ItemStorage
{
    /// <summary>
    /// Gets or sets the items which are stored.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<Item> Items { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the money which is stored.
    /// </summary>
    public int Money { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.Items?.Count ?? 0} Items, {this.Money} Money";
    }
}