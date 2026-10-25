// <copyright file="CashShopCoinGrantRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.API;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// A request to grant cash shop coins to an account through the public API.
/// </summary>
public class CashShopCoinGrantRequest
{
    /// <summary>
    /// The maximum length of the reason.
    /// </summary>
    internal const int ReasonMaximumLength = 200;

    /// <summary>
    /// The maximum length of the reference.
    /// </summary>
    internal const int ReferenceMaximumLength = 100;

    /// <summary>
    /// Gets or sets the type of the granted coins: <c>WCoinC</c>, <c>WCoinP</c> or <c>GoblinPoints</c>.
    /// </summary>
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CashShopCoinType? CoinType { get; set; }

    /// <summary>
    /// Gets or sets the amount. A negative amount takes coins, but not below zero. It must not be zero.
    /// </summary>
    [Required]
    public int? Amount { get; set; }

    /// <summary>
    /// Gets or sets the reason, e.g. the purchase or the event for which the coins are granted.
    /// </summary>
    [MaxLength(ReasonMaximumLength)]
    public string? Reason { get; set; }

    /// <summary>
    /// Gets or sets an optional reference of the granting system, e.g. the identifier of a payment.
    /// A reference is only granted once, so the request can be repeated safely.
    /// </summary>
    [MaxLength(ReferenceMaximumLength)]
    public string? Reference { get; set; }
}
