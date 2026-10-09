// <copyright file="PlayerInMemoryContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory;

using System.Threading;
using MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// In-memory context implementation for <see cref="IPlayerContext"/>.
/// </summary>
public class PlayerInMemoryContext : InMemoryContext, IPlayerContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerInMemoryContext"/> class.
    /// </summary>
    /// <param name="provider">The manager which holds the memory repositories.</param>
    public PlayerInMemoryContext(InMemoryRepositoryProvider provider)
        : base(provider)
    {
    }

    /// <inheritdoc/>
    public async ValueTask<MUnique.OpenMU.DataModel.Entities.LetterBody?> GetLetterBodyByHeaderIdAsync(Guid headerId, CancellationToken cancellationToken = default)
    {
        var allLetters = await this.Provider.GetRepository<LetterBody>().GetAllAsync(cancellationToken).ConfigureAwait(false);
        return allLetters.FirstOrDefault(body => body.Header.Id == headerId);
    }

    /// <inheritdoc/>
    public async ValueTask<MUnique.OpenMU.DataModel.Entities.AccountState?> AuthenticateAsync(string loginName, string password, CancellationToken cancellationToken = default)
    {
        var allAccounts = await this.Provider.GetRepository<Account>().GetAllAsync(cancellationToken).ConfigureAwait(false);
        var account = allAccounts.FirstOrDefault(a => a.LoginName == loginName && BCrypt.Net.BCrypt.Verify(password, a.PasswordHash));
        return account?.State;
    }

    /// <inheritdoc/>
    public async ValueTask<MUnique.OpenMU.DataModel.Entities.Account?> GetAccountByLoginNameAsync(string loginName, string password, CancellationToken cancellationToken = default)
    {
        var allAccounts = await this.Provider.GetRepository<Account>().GetAllAsync(cancellationToken).ConfigureAwait(false);
        return allAccounts.FirstOrDefault(account => account.LoginName == loginName && BCrypt.Net.BCrypt.Verify(password, account.PasswordHash));
    }

    /// <inheritdoc/>
    public async ValueTask<MUnique.OpenMU.DataModel.Entities.Account?> GetAccountByLoginNameAsync(string loginName, CancellationToken cancellationToken = default)
    {
        var allAccounts = await this.Provider.GetRepository<Account>().GetAllAsync(cancellationToken).ConfigureAwait(false);
        return allAccounts.FirstOrDefault(account => account.LoginName == loginName);
    }

    /// <inheritdoc/>
    public async ValueTask<IEnumerable<MUnique.OpenMU.DataModel.Entities.Account>> GetAccountsOrderedByLoginNameAsync(int skip, int count, CancellationToken cancellationToken = default)
    {
        var allAccounts = await this.Provider.GetRepository<Account>().GetAllAsync(cancellationToken).ConfigureAwait(false);
        return allAccounts.OrderBy(a => a.LoginName).Skip(skip).Take(count);
    }

    /// <inheritdoc/>
    public async ValueTask<IEnumerable<MUnique.OpenMU.DataModel.Entities.Account>> SearchAccountsAsync(string searchTerm, int skip, int count, CancellationToken cancellationToken = default)
    {
        var allAccounts = await this.Provider.GetRepository<Account>().GetAllAsync(cancellationToken).ConfigureAwait(false);
        return allAccounts
            .Where(a => a.LoginName.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase)
                        || a.Characters.Any(c => c.Name.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase)))
            .OrderBy(a => a.LoginName)
            .Skip(skip)
            .Take(count);
    }

    /// <inheritdoc/>
    public async ValueTask<bool> CanSaveLetterAsync(Interfaces.LetterHeader letterHeader, CancellationToken cancellationToken = default)
    {
        return true;
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.Account?> GetAccountByCharacterNameAsync(string characterName, CancellationToken cancellationToken = default)
    {
        var allAccounts = await this.Provider.GetRepository<Account>().GetAllAsync(cancellationToken).ConfigureAwait(false);
        return allAccounts.FirstOrDefault(account => account.Characters.Any(c => c.Name == characterName));
    }

    /// <inheritdoc />
    public async ValueTask<Guid?> GetAccountIdByCharacterNameAsync(string characterName, CancellationToken cancellationToken = default)
    {
        var account = await this.GetAccountByCharacterNameAsync(characterName, cancellationToken).ConfigureAwait(false);
        return account?.GetId();
    }

    /// <inheritdoc />
    public async ValueTask<IReadOnlyList<DataModel.Entities.CastleSiegePendingReward>> GetPendingCastleSiegeRewardsAsync(
        Guid characterId,
        CancellationToken cancellationToken = default)
    {
        var pendingRewards = await this.Provider.GetRepository<CastleSiegePendingReward>()
            .GetAllAsync(cancellationToken)
            .ConfigureAwait(false);
        return pendingRewards.Where(reward => reward.CharacterId == characterId).ToList();
    }

    /// <inheritdoc />
    public async ValueTask<IReadOnlyList<DataModel.Entities.CashShopStorageItem>> GetCashShopStorageItemsAsync(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        var items = await this.Provider.GetRepository<CashShopStorageItem>()
            .GetAllAsync(cancellationToken)
            .ConfigureAwait(false);
        return items
            .Where(item => item.AccountId == accountId)
            .OrderBy(item => item.AddedAt)
            .ThenBy(item => item.Id)
            .ToList();
    }

    /// <inheritdoc />
    public async ValueTask<IReadOnlyList<DataModel.Entities.CashShopCoinGrant>> GetPendingCashShopCoinGrantsAsync(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        var grants = await this.Provider.GetRepository<CashShopCoinGrant>()
            .GetAllAsync(cancellationToken)
            .ConfigureAwait(false);
        return grants
            .Where(grant => grant.AccountId == accountId && grant.AppliedAt is null)
            .OrderBy(grant => grant.CreatedAt)
            .ToList();
    }

    /// <inheritdoc />
    public async ValueTask<IReadOnlyList<DataModel.Entities.CashShopCoinGrant>> GetLatestCashShopCoinGrantsAsync(
        Guid accountId,
        int maximumCount,
        CancellationToken cancellationToken = default)
    {
        var grants = await this.Provider.GetRepository<CashShopCoinGrant>()
            .GetAllAsync(cancellationToken)
            .ConfigureAwait(false);
        return grants
            .Where(grant => grant.AccountId == accountId)
            .OrderByDescending(grant => grant.CreatedAt)
            .Take(maximumCount)
            .ToList();
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.CashShopCoinGrant?> GetCashShopCoinGrantByReferenceAsync(
        string reference,
        CancellationToken cancellationToken = default)
    {
        var grants = await this.Provider.GetRepository<CashShopCoinGrant>()
            .GetAllAsync(cancellationToken)
            .ConfigureAwait(false);
        return grants.FirstOrDefault(grant => grant.Reference == reference);
    }

    /// <inheritdoc />
    public async ValueTask<Guid?> GetAccountIdByLoginNameAsync(string loginName, CancellationToken cancellationToken = default)
    {
        var accounts = await this.Provider.GetRepository<Account>()
            .GetAllAsync(cancellationToken)
            .ConfigureAwait(false);
        return accounts.FirstOrDefault(account => account.LoginName == loginName)?.Id;
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.CharacterStatus?> GetCharacterStatusAsync(string characterName, CancellationToken cancellationToken = default)
    {
        var characters = await this.Provider.GetRepository<Character>()
            .GetAllAsync(cancellationToken)
            .ConfigureAwait(false);
        return characters.FirstOrDefault(character => character.Name == characterName)?.CharacterStatus;
    }

    /// <inheritdoc />
    public async ValueTask<DateTime?> GetAccountChatBanUntilAsync(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        var accounts = await this.Provider.GetRepository<Account>()
            .GetAllAsync(cancellationToken)
            .ConfigureAwait(false);
        return accounts.FirstOrDefault(account => account.Id == accountId)?.ChatBanUntil;
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.AccountExternalLink?> GetAccountExternalLinkAsync(
        Guid accountId,
        string provider,
        CancellationToken cancellationToken = default)
    {
        var links = await this.Provider.GetRepository<AccountExternalLink>()
            .GetAllAsync(cancellationToken)
            .ConfigureAwait(false);
        return links.FirstOrDefault(link => link.AccountId == accountId && link.Provider == provider);
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.AccountExternalLink?> GetAccountExternalLinkByUserAsync(
        string provider,
        string externalUserId,
        CancellationToken cancellationToken = default)
    {
        var links = await this.Provider.GetRepository<AccountExternalLink>()
            .GetAllAsync(cancellationToken)
            .ConfigureAwait(false);
        return links.FirstOrDefault(link => link.Provider == provider && link.ExternalUserId == externalUserId);
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.AccountExternalLink?> GetAccountExternalLinkByCodeAsync(
        string provider,
        string codeHash,
        CancellationToken cancellationToken = default)
    {
        var links = await this.Provider.GetRepository<AccountExternalLink>()
            .GetAllAsync(cancellationToken)
            .ConfigureAwait(false);
        return links.FirstOrDefault(link => link.Provider == provider && link.CodeHash == codeHash);
    }

    /// <inheritdoc />
    public async ValueTask<IReadOnlyList<DataModel.Entities.AccountExternalLink>> GetAccountExternalLinksAsync(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        var links = await this.Provider.GetRepository<AccountExternalLink>()
            .GetAllAsync(cancellationToken)
            .ConfigureAwait(false);
        return links.Where(link => link.AccountId == accountId).OrderBy(link => link.Provider).ToList();
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.GensMember?> GetGensMemberAsync(
        Guid characterId,
        CancellationToken cancellationToken = default)
    {
        var members = await this.Provider.GetRepository<GensMember>()
            .GetAllAsync(cancellationToken)
            .ConfigureAwait(false);
        return members.FirstOrDefault(member => member.CharacterId == characterId);
    }

    /// <inheritdoc />
    public async ValueTask<IReadOnlyList<CharacterSummary>> GetCharacterRankingAsync(
        Guid levelAttributeId,
        Guid masterLevelAttributeId,
        Guid resetsAttributeId,
        int count,
        CancellationToken cancellationToken = default)
    {
        var accounts = await this.Provider.GetRepository<Account>()
            .GetAllAsync(cancellationToken)
            .ConfigureAwait(false);

        return accounts
            .Where(account => !account.IsBot && !account.IsTemplate)
            .SelectMany(account => account.Characters)
            .Where(character => character.CharacterStatus == DataModel.Entities.CharacterStatus.Normal)
            .OrderByDescending(character => GetValue(character, resetsAttributeId))
            .ThenByDescending(character => GetValue(character, masterLevelAttributeId))
            .ThenByDescending(character => GetValue(character, levelAttributeId))
            .ThenByDescending(character => character.MasterExperience)
            .ThenByDescending(character => character.Experience)
            .Take(count)
            .Select(character => ToSummary(character, levelAttributeId, masterLevelAttributeId, resetsAttributeId))
            .ToList();
    }

    /// <inheritdoc />
    public async ValueTask<CharacterSummary?> GetCharacterSummaryAsync(
        string characterName,
        Guid levelAttributeId,
        Guid masterLevelAttributeId,
        Guid resetsAttributeId,
        CancellationToken cancellationToken = default)
    {
        var accounts = await this.Provider.GetRepository<Account>()
            .GetAllAsync(cancellationToken)
            .ConfigureAwait(false);

        return accounts
            .SelectMany(account => account.Characters)
            .Where(character => character.Name == characterName)
            .Select(character => ToSummary(character, levelAttributeId, masterLevelAttributeId, resetsAttributeId))
            .FirstOrDefault();
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.GensAbuse?> GetGensAbuseAsync(
        Guid killerId,
        Guid victimId,
        CancellationToken cancellationToken = default)
    {
        var abuses = await this.Provider.GetRepository<GensAbuse>()
            .GetAllAsync(cancellationToken)
            .ConfigureAwait(false);
        return abuses.FirstOrDefault(abuse => abuse.KillerId == killerId && abuse.VictimId == victimId);
    }

    private static float GetValue(DataModel.Entities.Character character, Guid attributeId)
        => character.Attributes.FirstOrDefault(a => a.Definition?.Id == attributeId)?.Value ?? 0;

    private static CharacterSummary ToSummary(DataModel.Entities.Character character, Guid levelAttributeId, Guid masterLevelAttributeId, Guid resetsAttributeId)
        => new(
            character.Name,
            (string?)character.CharacterClass?.Name ?? string.Empty,
            (int)GetValue(character, levelAttributeId),
            (int)GetValue(character, masterLevelAttributeId),
            (int)GetValue(character, resetsAttributeId));
}
