// <copyright file="PlayerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

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
    public async ValueTask<DataModel.Entities.LetterBody?> GetLetterBodyByHeaderIdAsync(Guid headerId)
    {
        using var context = this.RepositoryProvider.ContextStack.UseContext(this);
        if (this.RepositoryProvider.GetRepository<LetterBody, LetterBodyRepository>() is { } repository)
        {
            return await repository.GetBodyByHeaderIdAsync(headerId).ConfigureAwait(false);
        }

        return null;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> CanSaveLetterAsync(Interfaces.LetterHeader letterHeader)
    {
        if (letterHeader is not Model.LetterHeader persistentHeader)
        {
            return false;
        }

        persistentHeader.Receiver = await this.Context.Set<Character>().FirstOrDefaultAsync(c => c.Name == letterHeader.ReceiverName).ConfigureAwait(false);
        return persistentHeader.Receiver != null;
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.Account?> GetAccountByLoginNameAsync(string loginName, string password)
    {
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            if (this.RepositoryProvider.GetRepository<Account, AccountRepository>() is { } accountRepository)
            {
                return await accountRepository.GetAccountByLoginNameAsync(loginName, password).ConfigureAwait(false);
            }
        }

        return null;
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.Account?> GetAccountByLoginNameAsync(string loginName)
    {
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            if (this.RepositoryProvider.GetRepository<Account, AccountRepository>() is { } accountRepository)
            {
                return await accountRepository.GetAccountByLoginNameAsync(loginName).ConfigureAwait(false);
            }
        }

        return null;
    }

    /// <inheritdoc />
    public async ValueTask<IEnumerable<DataModel.Entities.Account>> GetAccountsOrderedByLoginNameAsync(int skip, int count)
    {
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            return await this.Context.Set<Account>().AsNoTracking().OrderBy(a => a.LoginName).Skip(skip).Take(count).ToListAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.Account?> GetAccountByCharacterNameAsync(string characterName)
    {
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            if (this.RepositoryProvider.GetRepository<Account, AccountRepository>() is { } accountRepository)
            {
                return await accountRepository.GetAccountByCharacterNameAsync(characterName).ConfigureAwait(false);
            }
        }

        return null;
    }
}