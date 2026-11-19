// <copyright file="IPlayerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using System.Threading;
using System.Threading.Tasks;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Persistence context which is used by in-game players.
/// </summary>
public interface IPlayerContext : IContext
{
    /// <summary>
    /// Gets the letter body by the id of its header.
    /// </summary>
    /// <param name="headerId">The id of its header.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The body of the header.
    /// </returns>
    ValueTask<LetterBody?> GetLetterBodyByHeaderIdAsync(Guid headerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines if the letter can be saved.
    /// </summary>
    /// <param name="letterHeader">The letter header.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   <c>True</c>, if successful; Otherwise, <c>false</c>.
    /// </returns>
    ValueTask<bool> CanSaveLetterAsync(LetterHeader letterHeader, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates the account by login name and password, returning minimal account state.
    /// This is a lightweight alternative to <see cref="GetAccountByLoginNameAsync(string, string, CancellationToken)"/>
    /// that avoids loading the full account data.
    /// </summary>
    /// <param name="loginName">The login name.</param>
    /// <param name="password">The password.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The <see cref="AccountState"/> if credentials are valid; otherwise, null.
    /// </returns>
    ValueTask<AccountState?> AuthenticateAsync(string loginName, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the account by login name if the password is correct.
    /// </summary>
    /// <param name="loginName">The login name.</param>
    /// <param name="password">The password.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The account, if the password is correct. Otherwise, null.
    /// </returns>
    ValueTask<Account?> GetAccountByLoginNameAsync(string loginName, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the account by login name.
    /// </summary>
    /// <param name="loginName">The login name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The account, if exists. Otherwise, null.
    /// </returns>
    ValueTask<Account?> GetAccountByLoginNameAsync(string loginName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the accounts ordered by login name.
    /// </summary>
    /// <param name="skip">The skip count.</param>
    /// <param name="count">The count.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The account objects, without dependent data.
    /// </returns>
    ValueTask<IEnumerable<Account>> GetAccountsOrderedByLoginNameAsync(int skip, int count, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the accounts which contain the search term in their login name or in one of
    /// their character names, ordered by login name.
    /// </summary>
    /// <param name="searchTerm">The search term.</param>
    /// <param name="skip">The skip count.</param>
    /// <param name="count">The count.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The account objects, without dependent data.
    /// </returns>
    ValueTask<IEnumerable<Account>> SearchAccountsAsync(string searchTerm, int skip, int count, CancellationToken cancellationToken = default);

    /// <summary>Searches login names, e-mail addresses and character names with an optional state filter.</summary>
    /// <param name="text">The case-insensitive search term.</param>
    /// <param name="state">The optional account state.</param>
    /// <param name="offset">The number of matches to skip.</param>
    /// <param name="limit">The maximum number of matches.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching account page and total count.</returns>
    ValueTask<(IReadOnlyList<Account> Accounts, int Total)> SearchAccountPageAsync(string text, AccountState? state, int offset, int limit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets statistics about the player accounts.
    /// </summary>
    /// <param name="registeredSince">The point in time (UTC) since which registered accounts are counted as new.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The statistics about the player accounts.</returns>
    ValueTask<AccountStatistics> GetAccountStatisticsAsync(DateTime registeredSince, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the leaderboard of the specified mini games, with the characters ordered by their total score.
    /// </summary>
    /// <remarks>
    /// Characters of template and bot accounts are not included.
    /// </remarks>
    /// <param name="miniGameIds">The identifiers of the mini game definitions, e.g. all levels of Blood Castle.</param>
    /// <param name="since">The optional point in time (UTC) since which finished games are counted.</param>
    /// <param name="count">The maximum number of characters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The best characters, ordered by their total score.</returns>
    ValueTask<IReadOnlyList<MiniGameRankingSummary>> GetMiniGameLeaderboardAsync(IReadOnlyCollection<Guid> miniGameIds, DateTime? since, int count, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the account by character name.
    /// </summary>
    /// <param name="characterName">The character name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The account; Otherwise, null.
    /// </returns>
    ValueTask<Account?> GetAccountByCharacterNameAsync(string characterName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the identifier of the account of a character, without loading the account.
    /// </summary>
    /// <param name="characterName">The character name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The identifier of the account; Otherwise, null, if the character doesn't exist.</returns>
    ValueTask<Guid?> GetAccountIdByCharacterNameAsync(string characterName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets pending Castle Siege rewards for a character.
    /// </summary>
    /// <param name="characterId">The persistent character identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The pending Castle Siege rewards.</returns>
    ValueTask<IReadOnlyList<CastleSiegePendingReward>> GetPendingCastleSiegeRewardsAsync(
        Guid characterId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the items of the cash shop storage of an account, ordered by the time they were added.
    /// </summary>
    /// <param name="accountId">The persistent account identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The items of the cash shop storage.</returns>
    ValueTask<IReadOnlyList<CashShopStorageItem>> GetCashShopStorageItemsAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the grants of cash shop coins to an account which are not applied yet, ordered by the time they were granted.
    /// </summary>
    /// <param name="accountId">The persistent account identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The pending grants.</returns>
    ValueTask<IReadOnlyList<CashShopCoinGrant>> GetPendingCashShopCoinGrantsAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the latest grants of cash shop coins to an account, including the applied ones, ordered from the newest to the oldest.
    /// </summary>
    /// <param name="accountId">The persistent account identifier.</param>
    /// <param name="maximumCount">The maximum number of grants.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The latest grants.</returns>
    ValueTask<IReadOnlyList<CashShopCoinGrant>> GetLatestCashShopCoinGrantsAsync(
        Guid accountId,
        int maximumCount,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the grant of cash shop coins with the specified reference.
    /// </summary>
    /// <param name="reference">The reference of the granting system.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The grant; Otherwise, null, if no grant has the reference.</returns>
    ValueTask<CashShopCoinGrant?> GetCashShopCoinGrantByReferenceAsync(
        string reference,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the identifier of an account by its login name, without loading the account.
    /// </summary>
    /// <param name="loginName">The login name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The identifier of the account; Otherwise, null, if the account doesn't exist.</returns>
    ValueTask<Guid?> GetAccountIdByLoginNameAsync(string loginName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the status of a character, e.g. if it's a game master, without loading the account.
    /// </summary>
    /// <param name="characterName">The name of the character.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The status; Otherwise, null, if the character doesn't exist.</returns>
    ValueTask<CharacterStatus?> GetCharacterStatusAsync(string characterName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the time until which the chat of an account is banned, without loading the account.
    /// </summary>
    /// <param name="accountId">The persistent account identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The time until which the chat is banned; or <c>null</c>, if it isn't banned.</returns>
    ValueTask<DateTime?> GetAccountChatBanUntilAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the bindings of the chat of a guild and of the chat of its alliance to channels of external
    /// services, without loading the bindings of other guilds.
    /// </summary>
    /// <param name="guildId">The persistent identifier of the guild; its <see cref="GuildChatScope.Guild"/> binding is returned.</param>
    /// <param name="allianceMasterGuildId">The persistent identifier of the master guild of the alliance; its <see cref="GuildChatScope.Alliance"/> binding is returned.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The bindings, at most one per scope.</returns>
    ValueTask<IReadOnlyList<GuildChatBinding>> GetGuildChatBindingsAsync(
        Guid guildId,
        Guid allianceMasterGuildId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Atomically consumes an unexpired link code and replaces the user's previous link.
    /// A rejected code must leave every existing link unchanged.
    /// </summary>
    /// <param name="provider">The external service.</param>
    /// <param name="codeHash">The hash of the one-time code.</param>
    /// <param name="nowUtc">The current UTC time.</param>
    /// <param name="externalUserId">The external user identifier.</param>
    /// <param name="externalUserName">The external user name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The account, character and replaced user; or null if the code was rejected.</returns>
    ValueTask<(Guid AccountId, string? CharacterName, string? PreviousUserId)?> TryLinkAccountAsync(
        string provider,
        string codeHash,
        DateTime nowUtc,
        string externalUserId,
        string externalUserName,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets the link of an account to a user of an external service.
    /// </summary>
    /// <param name="accountId">The persistent account identifier.</param>
    /// <param name="provider">The name of the external service, e.g. <c>discord</c>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The link; Otherwise, null, if the account has no link to the service.</returns>
    ValueTask<AccountExternalLink?> GetAccountExternalLinkAsync(
        Guid accountId,
        string provider,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the link of a user of an external service to an account.
    /// </summary>
    /// <param name="provider">The name of the external service, e.g. <c>discord</c>.</param>
    /// <param name="externalUserId">The identifier of the user in the external service.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The link; Otherwise, null, if the user isn't linked.</returns>
    ValueTask<AccountExternalLink?> GetAccountExternalLinkByUserAsync(
        string provider,
        string externalUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the link with a pending one-time code.
    /// </summary>
    /// <param name="provider">The name of the external service, e.g. <c>discord</c>.</param>
    /// <param name="codeHash">The hash of the code.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The link; Otherwise, null, if no link has the code.</returns>
    ValueTask<AccountExternalLink?> GetAccountExternalLinkByCodeAsync(
        string provider,
        string codeHash,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the identifiers of the accounts of characters, without loading the accounts.
    /// </summary>
    /// <param name="characterNames">The names of the characters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The identifiers of the accounts by the names of the existing characters.</returns>
    ValueTask<IReadOnlyDictionary<string, Guid>> GetAccountIdsByCharacterNamesAsync(
        IReadOnlyCollection<string> characterNames,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the links of accounts to users of an external service.
    /// </summary>
    /// <param name="provider">The name of the external service, e.g. <c>discord</c>.</param>
    /// <param name="accountIds">The persistent account identifiers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The links.</returns>
    ValueTask<IReadOnlyList<AccountExternalLink>> GetAccountExternalLinksAsync(
        string provider,
        IReadOnlyCollection<Guid> accountIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the links of an account to users of external services.
    /// </summary>
    /// <param name="accountId">The persistent account identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The links.</returns>
    ValueTask<IReadOnlyList<AccountExternalLink>> GetAccountExternalLinksAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the login name and the state of an account, without loading the account.
    /// </summary>
    /// <param name="accountId">The persistent account identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The login name and state; Otherwise, null, if the account doesn't exist.</returns>
    ValueTask<(string LoginName, AccountState State)?> GetAccountLoginStateAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the login ticket with the specified token hash.
    /// </summary>
    /// <param name="tokenHash">The hash of the token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The ticket; Otherwise, null, if no ticket has the hash.</returns>
    ValueTask<LoginTicket?> GetLoginTicketByHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Atomically marks an unused and unexpired login ticket as used.
    /// Of concurrent calls for the same ticket, only one succeeds, even across game servers.
    /// </summary>
    /// <param name="ticketId">The identifier of the ticket.</param>
    /// <param name="nowUtc">The current UTC time.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c>, if this call marked the ticket as used; Otherwise, <c>false</c>.</returns>
    ValueTask<bool> TryUseLoginTicketAsync(
        Guid ticketId,
        DateTime nowUtc,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Atomically adds a login ticket, unless the account already has the maximum of unexpired tickets, used or not.
    /// Concurrent calls for the same account are serialized, even across servers, so they never exceed the maximum.
    /// </summary>
    /// <param name="accountId">The persistent account identifier.</param>
    /// <param name="tokenHash">The hash of the ticket's token.</param>
    /// <param name="createdAtUtc">The UTC time of the issue; tickets expiring after it count against the maximum.</param>
    /// <param name="expiresAtUtc">The UTC time when the ticket expires.</param>
    /// <param name="maximumUnexpired">The maximum number of unexpired tickets of the account.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c>, if the ticket was added; Otherwise, <c>false</c>, if the account has the maximum already.</returns>
    ValueTask<bool> TryAddLoginTicketAsync(
        Guid accountId,
        string tokenHash,
        DateTime createdAtUtc,
        DateTime expiresAtUtc,
        int maximumUnexpired,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the expired login tickets of all accounts.
    /// </summary>
    /// <param name="nowUtc">The current UTC time.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task.</returns>
    ValueTask DeleteExpiredLoginTicketsAsync(
        DateTime nowUtc,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the gens membership of a character.
    /// </summary>
    /// <param name="characterId">The persistent character identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The gens membership; Otherwise, null, if the character never joined a gens.</returns>
    ValueTask<GensMember?> GetGensMemberAsync(
        Guid characterId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of the recent kills of a gens member by another gens member.
    /// </summary>
    /// <param name="killerId">The persistent identifier of the killing character.</param>
    /// <param name="victimId">The persistent identifier of the killed character.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The kill count; Otherwise, null, if the killer didn't kill the victim yet.</returns>
    ValueTask<GensAbuse?> GetGensAbuseAsync(
        Guid killerId,
        Guid victimId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the ranking of the characters of players, ordered by resets, master level, level and experience.
    /// Characters of bots, templates and game masters are not ranked.
    /// </summary>
    /// <remarks>
    /// The levels are attributes of the characters; their definitions are passed, because they are defined by the game logic.
    /// </remarks>
    /// <param name="levelAttributeId">The identifier of the attribute definition of the level.</param>
    /// <param name="masterLevelAttributeId">The identifier of the attribute definition of the master level.</param>
    /// <param name="resetsAttributeId">The identifier of the attribute definition of the resets.</param>
    /// <param name="count">The maximum number of entries.</param>
    /// <param name="characterClassIds">The identifiers of the character classes which are ranked; <see langword="null"/> for all classes.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The best characters.</returns>
    ValueTask<IReadOnlyList<CharacterSummary>> GetCharacterRankingAsync(
        Guid levelAttributeId,
        Guid masterLevelAttributeId,
        Guid resetsAttributeId,
        int count,
        IReadOnlyCollection<Guid>? characterClassIds = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the summary of a character.
    /// </summary>
    /// <param name="characterName">The name of the character.</param>
    /// <param name="levelAttributeId">The identifier of the attribute definition of the level.</param>
    /// <param name="masterLevelAttributeId">The identifier of the attribute definition of the master level.</param>
    /// <param name="resetsAttributeId">The identifier of the attribute definition of the resets.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The summary of the character; or <see langword="null"/>, if it doesn't exist.</returns>
    ValueTask<CharacterSummary?> GetCharacterSummaryAsync(
        string characterName,
        Guid levelAttributeId,
        Guid masterLevelAttributeId,
        Guid resetsAttributeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a character which can be ranked by <see cref="GetCharacterRankingAsync"/>, with its inventory, without loading its account.
    /// </summary>
    /// <remarks>
    /// It's for a public profile of a character, which needs one character, but not the other characters and the vault of its account.
    /// Like in the ranking, characters of bots, templates and game masters are not found.
    /// The character isn't tracked by the context, so changes to it are not saved.
    /// </remarks>
    /// <param name="characterName">The name of the character.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The character; or <see langword="null"/>, if it doesn't exist or can't be ranked.</returns>
    ValueTask<Character?> GetRankedCharacterAsync(string characterName, CancellationToken cancellationToken = default);
}
