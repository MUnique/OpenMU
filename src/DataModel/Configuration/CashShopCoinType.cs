// <copyright file="CashShopCoinType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The type of coin with which a <see cref="CashShopPackage"/> is paid.
/// </summary>
public enum CashShopCoinType
{
    /// <summary>
    /// The package is paid with <see cref="Entities.Account.WCoinC"/>.
    /// </summary>
    WCoinC,

    /// <summary>
    /// The package is paid with <see cref="Entities.Account.WCoinP"/>.
    /// </summary>
    WCoinP,

    /// <summary>
    /// The package is paid with <see cref="Entities.Account.GoblinPoints"/>.
    /// </summary>
    GoblinPoints,
}
