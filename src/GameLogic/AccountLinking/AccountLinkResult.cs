// <copyright file="AccountLinkResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.AccountLinking;

/// <summary>
/// The result of linking a user of an external service to an account.
/// </summary>
/// <param name="AccountId">The identifier of the linked account.</param>
/// <param name="CharacterName">The name of the character as which the user appears.</param>
/// <param name="ReplacedExternalUserId">The identifier of the user which was linked to the account before, if it's another one.</param>
public sealed record AccountLinkResult(Guid AccountId, string? CharacterName, string? ReplacedExternalUserId);
