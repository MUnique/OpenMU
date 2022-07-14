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
    /// <param name="repositoryManager">The repository manager.</param>
    /// <param name="logger">The logger.</param>
    public PlayerContext(DbContext context, RepositoryManager repositoryManager, ILogger<PlayerContext> logger)
        : base(context, repositoryManager, logger)
    {
    }

    /// <inheritdoc/>
    public async ValueTask<DataModel.Entities.LetterBody?> GetLetterBodyByHeaderIdAsync(Guid headerId)
    {
        using (this.RepositoryManager.ContextStack.UseContext(this))
        {
            if (this.RepositoryManager.GetRepository<LetterBody, LetterBodyRepository>() is { } repository)
            {
                return await repository.GetBodyByHeaderIdAsync(headerId);
            }
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

        persistentHeader.Receiver = await this.Context.Set<Character>().FirstOrDefaultAsync(c => c.Name == letterHeader.ReceiverName);
        return persistentHeader.Receiver != null;
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.Account?> GetAccountByLoginNameAsync(string loginName, string password)
    {
        using (this.RepositoryManager.ContextStack.UseContext(this))
        {
            if (this.RepositoryManager.GetRepository<Account, AccountRepository>() is { } accountRepository)
            {
                return await accountRepository.GetAccountByLoginNameAsync(loginName, password);
            }
        }

        return null;
    }

    /// <inheritdoc />
    public async ValueTask<IEnumerable<DataModel.Entities.Account>> GetAccountsOrderedByLoginNameAsync(int skip, int count)
    {
        using (this.RepositoryManager.ContextStack.UseContext(this))
        {
            return await this.Context.Set<Account>().OrderBy(a => a.LoginName).Skip(skip).Take(count).ToListAsync();
        }
    }
}