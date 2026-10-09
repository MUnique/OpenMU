// <copyright file="AccountExternalLink.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// A link of an <see cref="Account"/> to a user of an external service, e.g. Discord.
/// </summary>
/// <remarks>
/// The link is created in two steps: In the game, the player requests a one-time code, which
/// is stored as hash with an expiration. In the external service, the user enters the code,
/// which links the user to the account.
/// It's not part of the <see cref="Account"/> aggregate, because the external service links it
/// while a game server may hold the account in memory. That's why it refers to its account by identifier.
/// </remarks>
[AggregateRoot]
public class AccountExternalLink
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the linked account.
    /// </summary>
    public Guid AccountId { get; set; }

    /// <summary>
    /// Gets or sets the name of the external service, e.g. <c>discord</c>.
    /// An account can be linked to one user per service.
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the user in the external service;
    /// <c>null</c>, if the link isn't confirmed yet.
    /// A user can be linked to one account.
    /// </summary>
    public string? ExternalUserId { get; set; }

    /// <summary>
    /// Gets or sets the name of the user in the external service, at the time of linking.
    /// </summary>
    public string? ExternalUserName { get; set; }

    /// <summary>
    /// Gets or sets the name of the character, as which the user appears, e.g. when chatting from the external service.
    /// </summary>
    public string? CharacterName { get; set; }

    /// <summary>
    /// Gets or sets the timestamp at which the user was linked.
    /// </summary>
    public DateTime? LinkedAt { get; set; }

    /// <summary>
    /// Gets or sets the types of notifications which the user receives in the external service.
    /// They are opt-in, so there are none by default.
    /// </summary>
    public AccountNotificationTypes Notifications { get; set; }

    /// <summary>
    /// Gets or sets the hash of the pending one-time code; <c>null</c>, if there is none.
    /// </summary>
    public string? CodeHash { get; set; }

    /// <summary>
    /// Gets or sets the timestamp until which the pending code is valid.
    /// </summary>
    public DateTime? CodeExpiresAt { get; set; }
}
