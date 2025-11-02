// <copyright file="CashShopTransaction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Represents a transaction in the cash shop for audit and history tracking.
/// </summary>
public class CashShopTransaction
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the account that made the transaction.
    /// </summary>
    public virtual Account? Account { get; set; }

    /// <summary>
    /// Gets or sets the product ID that was purchased.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the amount of currency spent.
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    /// Gets or sets the type of currency used.
    /// 0 = WCoinC, 1 = WCoinP, 2 = GoblinPoints.
    /// </summary>
    public byte CoinType { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the transaction occurred.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the type of transaction.
    /// </summary>
    public CashShopTransactionType TransactionType { get; set; }

    /// <summary>
    /// Gets or sets the name of the character who made the transaction.
    /// </summary>
    public string CharacterName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the receiver (for gift transactions).
    /// If null or empty, the transaction was for the account's own use.
    /// </summary>
    public string? ReceiverName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the transaction was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets optional notes or error information about the transaction.
    /// </summary>
    public string? Notes { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        var receiver = string.IsNullOrEmpty(this.ReceiverName) ? "self" : this.ReceiverName;
        return $"{this.Timestamp:yyyy-MM-dd HH:mm:ss} - {this.CharacterName} -> {receiver}: Product {this.ProductId} for {this.Amount} (Type {this.CoinType}) - {this.TransactionType} ({(this.Success ? "Success" : "Failed")})";
    }
}

/// <summary>
/// Defines the type of cash shop transaction.
/// </summary>
public enum CashShopTransactionType
{
    /// <summary>
    /// Direct purchase for own account.
    /// </summary>
    Purchase,

    /// <summary>
    /// Gift sent to another player.
    /// </summary>
    Gift,

    /// <summary>
    /// Refund of a previous purchase.
    /// </summary>
    Refund,
}
