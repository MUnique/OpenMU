// <copyright file="CashShopStorageItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// An item in the cash shop storage of an account, which was bought or received as gift,
/// but not used yet.
/// </summary>
/// <remarks>
/// It's not part of the <see cref="Account"/> aggregate, because gifts are added to the
/// storage of other accounts, which may be loaded by another game server at the same time.
/// That's why it refers to its account by identifier.
/// The product is referred by the price sequence number of the cash shop script, which is the
/// identifier the client knows it by.
/// </remarks>
[AggregateRoot]
public class CashShopStorageItem
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the account which owns the item.
    /// </summary>
    public Guid AccountId { get; set; }

    /// <summary>
    /// Gets or sets the product sequence number of the cash shop script.
    /// </summary>
    public int ProductSequence { get; set; }

    /// <summary>
    /// Gets or sets the price sequence number of the cash shop script.
    /// </summary>
    public int PriceSequence { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the item was received as gift.
    /// Gifts are shown in a separate storage tab.
    /// </summary>
    public bool IsGift { get; set; }

    /// <summary>
    /// Gets or sets the name of the character which sent the gift.
    /// </summary>
    public string? GiftSenderName { get; set; }

    /// <summary>
    /// Gets or sets the message of the gift.
    /// </summary>
    public string? GiftMessage { get; set; }

    /// <summary>
    /// Gets or sets the timestamp at which the item was added to the storage.
    /// </summary>
    public DateTime AddedAt { get; set; }
}
