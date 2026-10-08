// <copyright file="CashShopCoinTypeExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CashShop;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Extensions to convert the coin types of the cash shop script, which the client sends.
/// </summary>
internal static class CashShopCoinTypeExtensions
{
    // The cash types of the packages in the script of the client (IBSPackage.txt).
    private const uint WCoinCScriptType = 508;
    private const uint WCoinPScriptType = 509;
    private const uint GoblinPointsScriptType = 0;

    /// <summary>
    /// Converts the cash type of the client script to the <see cref="CashShopCoinType"/>.
    /// </summary>
    /// <param name="scriptCashType">The cash type of the client script.</param>
    /// <returns>The coin type; <c>null</c>, if the cash type is unknown.</returns>
    public static CashShopCoinType? ToCashShopCoinType(this uint scriptCashType)
    {
        return scriptCashType switch
        {
            WCoinCScriptType => CashShopCoinType.WCoinC,
            WCoinPScriptType => CashShopCoinType.WCoinP,
            GoblinPointsScriptType => CashShopCoinType.GoblinPoints,
            _ => null,
        };
    }
}
