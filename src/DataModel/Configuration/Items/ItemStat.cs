// <copyright file="ItemStat.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// The stats of an item which an <see cref="ItemStatProfile"/> has <see cref="ItemStatTerms"/> for.
/// </summary>
/// <remarks>
/// The values are stored in the database, so new ones are only appended.
/// </remarks>
public enum ItemStat
{
    /// <summary>
    /// The minimum and maximum physical damage of a weapon.
    /// </summary>
    Damage,

    /// <summary>
    /// The magic power of a staff, sceptre or book (its rise).
    /// </summary>
    MagicPower,

    /// <summary>
    /// The block rate (defense rate) of a shield.
    /// </summary>
    BlockRate,

    /// <summary>
    /// The defense of an armor piece, a shield or a wing.
    /// </summary>
    Defense,

    /// <summary>
    /// The magic defense.
    /// </summary>
    MagicDefense,
}
