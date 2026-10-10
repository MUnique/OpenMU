// <copyright file="PlayerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Persistence context which is used by in-game players.
/// </summary>
internal class PlayerContext : CachingEntityFrameworkContext, IPlayerContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerContext" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="repositoryProvider">The repository provider.</param>
    /// <param name="logger">The logger.</param>
    public PlayerContext(DbContext context, IContextAwareRepositoryProvider repositoryProvider, ILogger<PlayerContext> logger)
        : base(context, repositoryProvider, null, logger)
    {
    }

    /// <inheritdoc/>
    public async ValueTask<DataModel.Entities.LetterBody?> GetLetterBodyByHeaderIdAsync(Guid headerId, CancellationToken cancellationToken = default)
    {
        using var l = await this.LockAsync(cancellationToken).ConfigureAwait(false);
        using var context = this.RepositoryProvider.ContextStack.UseContext(this);
        if (this.RepositoryProvider.GetRepository<LetterBody, LetterBodyRepository>() is { } repository)
        {
            return await repository.GetBodyByHeaderIdAsync(headerId, cancellationToken).ConfigureAwait(false);
        }

        return null;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> CanSaveLetterAsync(Interfaces.LetterHeader letterHeader, CancellationToken cancellationToken = default)
    {
        using var l = await this.LockAsync(cancellationToken).ConfigureAwait(false);
        if (letterHeader is not Model.LetterHeader persistentHeader)
        {
            return false;
        }

        var receiverId = await this.Context.Set<Character>()
            .Where(c => c.Name == letterHeader.ReceiverName)
            .Select(c => (Guid?)c.Id)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
        if (receiverId is not { } id)
        {
            return false;
        }

        // Just the foreign key is required to save the letter, so we don't load and track the whole receiver.
        persistentHeader.ReceiverId = id;
        return true;
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.AccountState?> AuthenticateAsync(string loginName, string password, CancellationToken cancellationToken = default)
    {
        using var l = await this.LockAsync(cancellationToken).ConfigureAwait(false);
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            if (this.RepositoryProvider.GetRepository<Account, AccountRepository>() is { } accountRepository)
            {
                return await accountRepository.AuthenticateAsync(loginName, password, cancellationToken).ConfigureAwait(false);
            }
        }

        return null;
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.Account?> GetAccountByLoginNameAsync(string loginName, string password, CancellationToken cancellationToken = default)
    {
        using var l = await this.LockAsync(cancellationToken).ConfigureAwait(false);
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            if (this.RepositoryProvider.GetRepository<Account, AccountRepository>() is { } accountRepository)
            {
                return await accountRepository.GetAccountByLoginNameAsync(loginName, password, cancellationToken).ConfigureAwait(false);
            }
        }

        return null;
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.Account?> GetAccountByLoginNameAsync(string loginName, CancellationToken cancellationToken = default)
    {
        using var l = await this.LockAsync(cancellationToken).ConfigureAwait(false);
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            if (this.RepositoryProvider.GetRepository<Account, AccountRepository>() is { } accountRepository)
            {
                return await accountRepository.GetAccountByLoginNameAsync(loginName, cancellationToken).ConfigureAwait(false);
            }
        }

        return null;
    }

    /// <inheritdoc />
    public async ValueTask<IEnumerable<DataModel.Entities.Account>> GetAccountsOrderedByLoginNameAsync(int skip, int count, CancellationToken cancellationToken = default)
    {
        using var l = await this.LockAsync(cancellationToken).ConfigureAwait(false);
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            return await this.Context.Set<Account>().AsNoTracking().OrderBy(a => a.LoginName).Skip(skip).Take(count).ToListAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask<IEnumerable<DataModel.Entities.Account>> SearchAccountsAsync(string searchTerm, int skip, int count, CancellationToken cancellationToken = default)
    {
        using var l = await this.LockAsync(cancellationToken).ConfigureAwait(false);
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            // Invariant: this one runs in .NET, so it must not depend on the server's locale - in a
            // Turkish one, "I".ToLower() is a dotless "ı" and the term would match nothing. The
            // ToLower() calls inside the query below are a different matter: they are translated to
            // the database's own lower(), which is why they cannot take a culture.
            var term = searchTerm.ToLowerInvariant();
            return await this.Context.Set<Account>().AsNoTracking()
                .Where(a => a.LoginName.ToLower().Contains(term)
                            || a.RawCharacters.Any(c => c.Name.ToLower().Contains(term)))
                .OrderBy(a => a.LoginName)
                .Skip(skip)
                .Take(count)
                .ToListAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.Account?> GetAccountByCharacterNameAsync(string characterName, CancellationToken cancellationToken = default)
    {
        using var l = await this.LockAsync(cancellationToken).ConfigureAwait(false);
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            if (this.RepositoryProvider.GetRepository<Account, AccountRepository>() is { } accountRepository)
            {
                return await accountRepository.GetAccountByCharacterNameAsync(characterName, cancellationToken).ConfigureAwait(false);
            }
        }

        return null;
    }

    /// <inheritdoc />
    public async ValueTask<Guid?> GetAccountIdByCharacterNameAsync(string characterName, CancellationToken cancellationToken = default)
    {
        using var l = await this.LockAsync(cancellationToken).ConfigureAwait(false);
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            return await this.Context.Set<Account>()
                .Where(account => account.RawCharacters.Any(character => character.Name == characterName))
                .Select(account => (Guid?)account.Id)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask<IReadOnlyList<DataModel.Entities.CastleSiegePendingReward>> GetPendingCastleSiegeRewardsAsync(
        Guid characterId,
        CancellationToken cancellationToken = default)
    {
        using var l = await this.LockAsync(cancellationToken).ConfigureAwait(false);
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            return await this.Context.Set<CastleSiegePendingReward>()
                .Where(reward => reward.CharacterId == characterId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask<IReadOnlyList<DataModel.Entities.CashShopStorageItem>> GetCashShopStorageItemsAsync(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        using var l = await this.LockAsync(cancellationToken).ConfigureAwait(false);
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            return await this.Context.Set<CashShopStorageItem>()
                .Where(item => item.AccountId == accountId)
                .OrderBy(item => item.AddedAt)
                .ThenBy(item => item.Id)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask<IReadOnlyList<DataModel.Entities.CashShopCoinGrant>> GetPendingCashShopCoinGrantsAsync(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        using var l = await this.LockAsync(cancellationToken).ConfigureAwait(false);
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            return await this.Context.Set<CashShopCoinGrant>()
                .Where(grant => grant.AccountId == accountId && grant.AppliedAt == null)
                .OrderBy(grant => grant.CreatedAt)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask<IReadOnlyList<DataModel.Entities.CashShopCoinGrant>> GetLatestCashShopCoinGrantsAsync(
        Guid accountId,
        int maximumCount,
        CancellationToken cancellationToken = default)
    {
        using var l = await this.LockAsync(cancellationToken).ConfigureAwait(false);
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            return await this.Context.Set<CashShopCoinGrant>()
                .Where(grant => grant.AccountId == accountId)
                .OrderByDescending(grant => grant.CreatedAt)
                .Take(maximumCount)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.CashShopCoinGrant?> GetCashShopCoinGrantByReferenceAsync(
        string reference,
        CancellationToken cancellationToken = default)
    {
        using var l = await this.LockAsync(cancellationToken).ConfigureAwait(false);
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            return await this.Context.Set<CashShopCoinGrant>()
                .FirstOrDefaultAsync(grant => grant.Reference == reference, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask<DateTime?> GetAccountChatBanUntilAsync(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        using var l = await this.LockAsync(cancellationToken).ConfigureAwait(false);
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            return await this.Context.Set<Account>()
                .Where(account => account.Id == accountId)
                .Select(account => account.ChatBanUntil)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask<IReadOnlyList<DataModel.Entities.GuildChatBinding>> GetGuildChatBindingsAsync(
        Guid guildId,
        Guid allianceMasterGuildId,
        CancellationToken cancellationToken = default)
    {
        using var l = await this.LockAsync(cancellationToken).ConfigureAwait(false);
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            return await CreateGuildChatBindingsQuery(this.Context, guildId, allianceMasterGuildId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Creates the query of <see cref="GetGuildChatBindingsAsync"/>.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="guildId">The persistent identifier of the guild.</param>
    /// <param name="allianceMasterGuildId">The persistent identifier of the master guild of the alliance.</param>
    /// <returns>The query.</returns>
    internal static IQueryable<GuildChatBinding> CreateGuildChatBindingsQuery(DbContext context, Guid guildId, Guid allianceMasterGuildId)
    {
        return context.Set<GuildChatBinding>()
            .Where(binding => (binding.GuildId == guildId && binding.Scope == DataModel.Entities.GuildChatScope.Guild)
                              || (binding.GuildId == allianceMasterGuildId && binding.Scope == DataModel.Entities.GuildChatScope.Alliance));
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.AccountExternalLink?> GetAccountExternalLinkAsync(
        Guid accountId,
        string provider,
        CancellationToken cancellationToken = default)
    {
        using var l = await this.LockAsync(cancellationToken).ConfigureAwait(false);
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            return await this.Context.Set<AccountExternalLink>()
                .FirstOrDefaultAsync(link => link.AccountId == accountId && link.Provider == provider, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.AccountExternalLink?> GetAccountExternalLinkByUserAsync(
        string provider,
        string externalUserId,
        CancellationToken cancellationToken = default)
    {
        using var l = await this.LockAsync(cancellationToken).ConfigureAwait(false);
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            return await this.Context.Set<AccountExternalLink>()
                .FirstOrDefaultAsync(link => link.Provider == provider && link.ExternalUserId == externalUserId, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.AccountExternalLink?> GetAccountExternalLinkByCodeAsync(
        string provider,
        string codeHash,
        CancellationToken cancellationToken = default)
    {
        using var l = await this.LockAsync(cancellationToken).ConfigureAwait(false);
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            return await this.Context.Set<AccountExternalLink>()
                .FirstOrDefaultAsync(link => link.Provider == provider && link.CodeHash == codeHash, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask<IReadOnlyList<DataModel.Entities.AccountExternalLink>> GetAccountExternalLinksAsync(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        using var l = await this.LockAsync(cancellationToken).ConfigureAwait(false);
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            return await this.Context.Set<AccountExternalLink>()
                .Where(link => link.AccountId == accountId)
                .OrderBy(link => link.Provider)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.GensMember?> GetGensMemberAsync(
        Guid characterId,
        CancellationToken cancellationToken = default)
    {
        using var l = await this.LockAsync(cancellationToken).ConfigureAwait(false);
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            return await this.Context.Set<GensMember>()
                .FirstOrDefaultAsync(member => member.CharacterId == characterId, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask<IReadOnlyList<CharacterSummary>> GetCharacterRankingAsync(
        Guid levelAttributeId,
        Guid masterLevelAttributeId,
        Guid resetsAttributeId,
        int count,
        CancellationToken cancellationToken = default)
    {
        using var l = await this.LockAsync(cancellationToken).ConfigureAwait(false);
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            var entries = await CreateCharacterRankingQuery(this.Context.Set<Account>(), levelAttributeId, masterLevelAttributeId, resetsAttributeId, count)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return entries
                .Select(entry => new CharacterSummary(entry.Name, entry.ClassName, (int)entry.Level, (int)entry.MasterLevel, (int)entry.Resets))
                .ToList();
        }
    }

    /// <inheritdoc />
    public async ValueTask<CharacterSummary?> GetCharacterSummaryAsync(
        string characterName,
        Guid levelAttributeId,
        Guid masterLevelAttributeId,
        Guid resetsAttributeId,
        CancellationToken cancellationToken = default)
    {
        using var l = await this.LockAsync(cancellationToken).ConfigureAwait(false);
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            var entry = await CreateCharacterSummaryQuery(this.Context.Set<Character>().Where(character => character.Name == characterName), levelAttributeId, masterLevelAttributeId, resetsAttributeId)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            return entry is null ? null : new CharacterSummary(entry.Name, entry.ClassName, (int)entry.Level, (int)entry.MasterLevel, (int)entry.Resets);
        }
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.GensAbuse?> GetGensAbuseAsync(
        Guid killerId,
        Guid victimId,
        CancellationToken cancellationToken = default)
    {
        using var l = await this.LockAsync(cancellationToken).ConfigureAwait(false);
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            return await this.Context.Set<GensAbuse>()
                .FirstOrDefaultAsync(abuse => abuse.KillerId == killerId && abuse.VictimId == victimId, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Creates the query of the character ranking.
    /// </summary>
    /// <param name="accounts">The accounts.</param>
    /// <param name="levelAttributeId">The identifier of the attribute definition of the level.</param>
    /// <param name="masterLevelAttributeId">The identifier of the attribute definition of the master level.</param>
    /// <param name="resetsAttributeId">The identifier of the attribute definition of the resets.</param>
    /// <param name="count">The maximum number of entries.</param>
    /// <returns>The query.</returns>
    internal static IQueryable<CharacterRankingRow> CreateCharacterRankingQuery(
        IQueryable<Account> accounts,
        Guid levelAttributeId,
        Guid masterLevelAttributeId,
        Guid resetsAttributeId,
        int count)
    {
        // EF can't order by the members of the projected record, so the ordering happens before the projection.
        var characters = accounts
            .Where(account => !account.IsBot && !account.IsTemplate)
            .SelectMany(account => account.RawCharacters)
            .Where(character => character.CharacterStatus == DataModel.Entities.CharacterStatus.Normal)
            .OrderByDescending(character => character.RawAttributes.Where(a => a.DefinitionId == resetsAttributeId).Select(a => a.Value).FirstOrDefault())
            .ThenByDescending(character => character.RawAttributes.Where(a => a.DefinitionId == masterLevelAttributeId).Select(a => a.Value).FirstOrDefault())
            .ThenByDescending(character => character.RawAttributes.Where(a => a.DefinitionId == levelAttributeId).Select(a => a.Value).FirstOrDefault())
            .ThenByDescending(character => character.MasterExperience)
            .ThenByDescending(character => character.Experience)
            .Take(count);

        return CreateCharacterSummaryQuery(characters, levelAttributeId, masterLevelAttributeId, resetsAttributeId);
    }

    /// <summary>
    /// Creates the query which projects the characters to their summary.
    /// </summary>
    /// <param name="characters">The characters.</param>
    /// <param name="levelAttributeId">The identifier of the attribute definition of the level.</param>
    /// <param name="masterLevelAttributeId">The identifier of the attribute definition of the master level.</param>
    /// <param name="resetsAttributeId">The identifier of the attribute definition of the resets.</param>
    /// <returns>The query.</returns>
    internal static IQueryable<CharacterRankingRow> CreateCharacterSummaryQuery(
        IQueryable<Character> characters,
        Guid levelAttributeId,
        Guid masterLevelAttributeId,
        Guid resetsAttributeId)
    {
        return characters.Select(character => new CharacterRankingRow(
            character.Name,
            character.RawCharacterClass!.Name,
            character.RawAttributes.Where(a => a.DefinitionId == levelAttributeId).Select(a => a.Value).FirstOrDefault(),
            character.RawAttributes.Where(a => a.DefinitionId == masterLevelAttributeId).Select(a => a.Value).FirstOrDefault(),
            character.RawAttributes.Where(a => a.DefinitionId == resetsAttributeId).Select(a => a.Value).FirstOrDefault()));
    }
}
