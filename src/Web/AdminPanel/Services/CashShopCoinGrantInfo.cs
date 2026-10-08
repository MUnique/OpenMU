// <copyright file="CashShopCoinGrantInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.Text.Json.Serialization;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;

/// <summary>
/// The information about a grant of cash shop coins.
/// </summary>
/// <param name="Id">The identifier of the grant.</param>
/// <param name="CoinType">The type of the granted coins.</param>
/// <param name="Amount">The granted amount.</param>
/// <param name="Reason">The reason.</param>
/// <param name="GrantedBy">The name of the administrator or API client which granted the coins.</param>
/// <param name="Reference">The reference of the granting system.</param>
/// <param name="CreatedAt">The timestamp at which the coins were granted.</param>
/// <param name="AppliedAt">The timestamp at which the game server applied the grant; <c>null</c>, if it's pending.</param>
public record CashShopCoinGrantInfo(
    Guid Id,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] CashShopCoinType CoinType,
    int Amount,
    string? Reason,
    string? GrantedBy,
    string? Reference,
    DateTime CreatedAt,
    DateTime? AppliedAt)
{
    /// <summary>
    /// Creates the information about the specified grant.
    /// </summary>
    /// <param name="grant">The grant.</param>
    /// <returns>The information about the grant.</returns>
    public static CashShopCoinGrantInfo From(CashShopCoinGrant grant)
    {
        return new(grant.GetId(), grant.CoinType, grant.Amount, grant.Reason, grant.GrantedBy, grant.Reference, grant.CreatedAt, grant.AppliedAt);
    }
}
