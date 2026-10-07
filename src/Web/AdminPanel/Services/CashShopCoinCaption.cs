// <copyright file="CashShopCoinCaption.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Provides the captions of the cash shop coin types.
/// </summary>
public static class CashShopCoinCaption
{
    /// <summary>
    /// Gets the caption of a coin type, which is the caption of its balance on the <see cref="Account"/>.
    /// </summary>
    /// <param name="coinType">The coin type.</param>
    /// <returns>The caption.</returns>
    public static string Get(CashShopCoinType coinType)
    {
        return typeof(Account).GetPropertyCaption(coinType.ToString());
    }
}
