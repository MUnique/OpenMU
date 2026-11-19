// <copyright file="WebsiteAccount.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations.Website;

/// <summary>
/// The account of a player who logged in at the website, with its characters.
/// </summary>
/// <param name="LoginName">The login name of the account.</param>
/// <param name="State">The state of the account, e.g. <c>Normal</c> or <c>Banned</c>, so the website can decide what a banned player may do.</param>
/// <param name="IsOnline">
/// A value indicating whether the account is in the game right now. Only the player sees it, after the login:
/// the password of an online account can't be changed, so the website can ask the player to leave the game first.
/// </param>
/// <param name="IsOfflineLeveling">
/// A value indicating whether a character of the account is offline leveling right now. Such an account isn't
/// <paramref name="IsOnline"/>, because it's logged off from the login server, but its character is still in the game.
/// </param>
/// <param name="Characters">The characters of the account.</param>
public sealed record WebsiteAccount(string LoginName, string State, bool IsOnline, bool IsOfflineLeveling, IReadOnlyList<WebsiteCharacterSummary> Characters);
