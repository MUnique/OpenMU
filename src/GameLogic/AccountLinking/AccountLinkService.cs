// <copyright file="AccountLinkService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.AccountLinking;

using System.Security.Cryptography;
using System.Text;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Links accounts to users of external services, e.g. Discord.
/// </summary>
/// <remarks>
/// In the game, a player requests a one-time code with <see cref="CreateCodeAsync"/>.
/// The user enters it in the external service, which calls <see cref="LinkAsync"/>.
/// A user is linked to one account, and an account to one user per service.
/// Each method uses its own short-lived persistence context, so that it doesn't interfere
/// with the context of a player which holds the account.
/// </remarks>
public sealed class AccountLinkService
{
    /// <summary>
    /// The name of Discord as external service.
    /// </summary>
    public const string DiscordProvider = "discord";

    /// <summary>
    /// The characters of a code. Characters which are easily confused, like 0 and O, are left out.
    /// </summary>
    private const string CodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    private const int CodeLength = 8;

    private readonly Func<IPlayerContext> _createContext;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountLinkService"/> class.
    /// </summary>
    /// <param name="createContext">The function which creates a new persistence context.</param>
    /// <param name="timeProvider">The time provider.</param>
    public AccountLinkService(Func<IPlayerContext> createContext, TimeProvider? timeProvider = null)
    {
        this._createContext = createContext;
        this._timeProvider = timeProvider ?? TimeProvider.System;
    }

    /// <summary>
    /// Gets the time in which a code has to be used.
    /// </summary>
    public static TimeSpan CodeValidity { get; } = TimeSpan.FromMinutes(10);

    /// <summary>
    /// Creates a one-time code, with which a user of the external service links itself to the account.
    /// A previously created code becomes invalid. An existing link stays until the code is used.
    /// </summary>
    /// <param name="accountId">The identifier of the account.</param>
    /// <param name="provider">The name of the external service.</param>
    /// <param name="characterName">The name of the character as which the user should appear, e.g. the one which requested the code.</param>
    /// <returns>The code, e.g. <c>ABCD-EFGH</c>.</returns>
    public async ValueTask<string> CreateCodeAsync(Guid accountId, string provider, string? characterName)
    {
        var code = string.Create(CodeLength, 0, static (span, _) =>
        {
            for (var i = 0; i < span.Length; i++)
            {
                span[i] = CodeAlphabet[RandomNumberGenerator.GetInt32(CodeAlphabet.Length)];
            }
        });

        using var context = this._createContext();
        var link = await context.GetAccountExternalLinkAsync(accountId, provider).ConfigureAwait(false);
        if (link is null)
        {
            link = context.CreateNew<AccountExternalLink>();
            link.AccountId = accountId;
            link.Provider = provider;
        }

        link.CodeHash = HashCode(code);
        link.CodeExpiresAt = this._timeProvider.GetUtcNow().UtcDateTime.Add(CodeValidity);
        link.CharacterName = characterName;
        await context.SaveChangesAsync().ConfigureAwait(false);
        return $"{code[..(CodeLength / 2)]}-{code[(CodeLength / 2)..]}";
    }

    /// <summary>
    /// Links a user of the external service to the account of the code.
    /// When the user was linked to another account before, that link is removed.
    /// </summary>
    /// <param name="provider">The name of the external service.</param>
    /// <param name="code">The code which the player got in the game.</param>
    /// <param name="externalUserId">The identifier of the user in the external service.</param>
    /// <param name="externalUserName">The name of the user in the external service.</param>
    /// <returns>The result; or <c>null</c>, if the code is unknown, expired or already used.</returns>
    public async ValueTask<AccountLinkResult?> LinkAsync(string provider, string code, string externalUserId, string externalUserName)
    {
        var codeHash = HashCode(NormalizeCode(code));
        using var context = this._createContext();
        if (await context.GetAccountExternalLinkByCodeAsync(provider, codeHash).ConfigureAwait(false) is not { } link
            || link.CodeExpiresAt is not { } expiresAt
            || expiresAt < this._timeProvider.GetUtcNow().UtcDateTime)
        {
            return null;
        }

        var previousUserId = link.ExternalUserId;
        if (previousUserId != externalUserId
            && await context.GetAccountExternalLinkByUserAsync(provider, externalUserId).ConfigureAwait(false) is { } otherLink)
        {
            // The user is linked to another account, which is replaced. It's saved first, because a user can only be linked once.
            await context.DeleteAsync(otherLink).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        link.ExternalUserId = externalUserId;
        link.ExternalUserName = externalUserName;
        link.LinkedAt = this._timeProvider.GetUtcNow().UtcDateTime;
        link.CodeHash = null;
        link.CodeExpiresAt = null;
        await context.SaveChangesAsync().ConfigureAwait(false);
        return new AccountLinkResult(link.AccountId, link.CharacterName, previousUserId == externalUserId ? null : previousUserId);
    }

    /// <summary>
    /// Gets the confirmed link of an account.
    /// </summary>
    /// <param name="accountId">The identifier of the account.</param>
    /// <param name="provider">The name of the external service.</param>
    /// <returns>The link; or <c>null</c>, if the account isn't linked.</returns>
    public async ValueTask<AccountExternalLink?> GetLinkAsync(Guid accountId, string provider)
    {
        using var context = this._createContext();
        var link = await context.GetAccountExternalLinkAsync(accountId, provider).ConfigureAwait(false);
        return link?.ExternalUserId is null ? null : link;
    }

    /// <summary>
    /// Gets the link of a user of an external service.
    /// </summary>
    /// <param name="provider">The name of the external service.</param>
    /// <param name="externalUserId">The identifier of the user in the external service.</param>
    /// <returns>The link; or <c>null</c>, if the user isn't linked.</returns>
    public async ValueTask<AccountExternalLink?> GetLinkByUserAsync(string provider, string externalUserId)
    {
        using var context = this._createContext();
        return await context.GetAccountExternalLinkByUserAsync(provider, externalUserId).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes the link of an account, including a pending code.
    /// </summary>
    /// <param name="accountId">The identifier of the account.</param>
    /// <param name="provider">The name of the external service.</param>
    /// <returns>The identifier of the user which was linked; or <c>null</c>, if the account wasn't linked.</returns>
    public async ValueTask<string?> UnlinkAccountAsync(Guid accountId, string provider)
    {
        using var context = this._createContext();
        if (await context.GetAccountExternalLinkAsync(accountId, provider).ConfigureAwait(false) is not { } link)
        {
            return null;
        }

        await context.DeleteAsync(link).ConfigureAwait(false);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return link.ExternalUserId;
    }

    /// <summary>
    /// Removes the link of a user of an external service.
    /// </summary>
    /// <param name="provider">The name of the external service.</param>
    /// <param name="externalUserId">The identifier of the user in the external service.</param>
    /// <returns><c>true</c>, if the user was linked; otherwise, <c>false</c>.</returns>
    public async ValueTask<bool> UnlinkUserAsync(string provider, string externalUserId)
    {
        using var context = this._createContext();
        if (await context.GetAccountExternalLinkByUserAsync(provider, externalUserId).ConfigureAwait(false) is not { } link)
        {
            return false;
        }

        await context.DeleteAsync(link).ConfigureAwait(false);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return true;
    }

    /// <summary>
    /// Selects the character of the account, as which a linked user appears.
    /// </summary>
    /// <param name="provider">The name of the external service.</param>
    /// <param name="externalUserId">The identifier of the user in the external service.</param>
    /// <param name="characterName">The name of the character.</param>
    /// <returns>The result.</returns>
    public async ValueTask<CharacterSelectionResult> SelectCharacterAsync(string provider, string externalUserId, string characterName)
    {
        using var context = this._createContext();
        if (await context.GetAccountExternalLinkByUserAsync(provider, externalUserId).ConfigureAwait(false) is not { } link)
        {
            return CharacterSelectionResult.NotLinked;
        }

        if (await context.GetAccountIdByCharacterNameAsync(characterName).ConfigureAwait(false) != link.AccountId)
        {
            return CharacterSelectionResult.CharacterNotFound;
        }

        link.CharacterName = characterName;
        await context.SaveChangesAsync().ConfigureAwait(false);
        return CharacterSelectionResult.Selected;
    }

    /// <summary>
    /// Turns a type of notifications on or off for a linked account.
    /// </summary>
    /// <param name="accountId">The identifier of the account.</param>
    /// <param name="provider">The name of the external service.</param>
    /// <param name="type">The type of notifications.</param>
    /// <param name="enabled">The new state; or <c>null</c>, to toggle the current state.</param>
    /// <returns>The types of notifications which are on now; or <c>null</c>, if the account isn't linked.</returns>
    public ValueTask<AccountNotificationTypes?> SetNotificationAsync(Guid accountId, string provider, AccountNotificationTypes type, bool? enabled)
    {
        return this.SetNotificationAsync(context => context.GetAccountExternalLinkAsync(accountId, provider), type, enabled);
    }

    /// <summary>
    /// Turns a type of notifications on or off for a linked user.
    /// </summary>
    /// <param name="provider">The name of the external service.</param>
    /// <param name="externalUserId">The identifier of the user in the external service.</param>
    /// <param name="type">The type of notifications.</param>
    /// <param name="enabled">The new state; or <c>null</c>, to toggle the current state.</param>
    /// <returns>The types of notifications which are on now; or <c>null</c>, if the user isn't linked.</returns>
    public ValueTask<AccountNotificationTypes?> SetNotificationByUserAsync(string provider, string externalUserId, AccountNotificationTypes type, bool? enabled)
    {
        return this.SetNotificationAsync(context => context.GetAccountExternalLinkByUserAsync(provider, externalUserId), type, enabled);
    }

    private async ValueTask<AccountNotificationTypes?> SetNotificationAsync(Func<IPlayerContext, ValueTask<AccountExternalLink?>> getLink, AccountNotificationTypes type, bool? enabled)
    {
        using var context = this._createContext();
        if (await getLink(context).ConfigureAwait(false) is not { ExternalUserId: not null } link)
        {
            return null;
        }

        if (type != AccountNotificationTypes.None)
        {
            var isEnabled = enabled ?? !link.Notifications.HasFlag(type);
            link.Notifications = isEnabled ? link.Notifications | type : link.Notifications & ~type;
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        return link.Notifications;
    }

    /// <summary>
    /// Normalizes an entered code, so that it doesn't matter if it's entered in lower case or with separators.
    /// </summary>
    private static string NormalizeCode(string code)
    {
        return new string(code.Where(char.IsLetterOrDigit).Select(char.ToUpperInvariant).ToArray());
    }

    /// <summary>
    /// Hashes a code, so that the stored codes can't be used by someone who can read the database.
    /// </summary>
    private static string HashCode(string code)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(code)));
    }
}
