// <copyright file="PlayerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading;

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
    public async ValueTask<DataModel.Entities.LetterBody?> GetLetterBodyByHeaderIdAsync(Guid headerId, CancellationToken cancellationToken = default)
    {
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
        if (letterHeader is not Model.LetterHeader persistentHeader)
        {
            return false;
        }

        persistentHeader.Receiver = await this.Context.Set<Character>().FirstOrDefaultAsync(c => c.Name == letterHeader.ReceiverName, cancellationToken).ConfigureAwait(false);
        return persistentHeader.Receiver != null;
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.Account?> GetAccountByLoginNameAsync(string loginName, string password, CancellationToken cancellationToken = default)
    {
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
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            return await this.Context.Set<Account>().AsNoTracking().OrderBy(a => a.LoginName).Skip(skip).Take(count).ToListAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.Account?> GetAccountByCharacterNameAsync(string characterName, CancellationToken cancellationToken = default)
    {
        using (this.RepositoryProvider.ContextStack.UseContext(this))
        {
            if (this.RepositoryProvider.GetRepository<Account, AccountRepository>() is { } accountRepository)
            {
                return await accountRepository.GetAccountByCharacterNameAsync(characterName, cancellationToken).ConfigureAwait(false);
            }
        }

        return null;
    }
}