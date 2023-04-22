// <copyright file="PlayerInMemoryContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory;

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
    public async ValueTask<MUnique.OpenMU.DataModel.Entities.LetterBody?> GetLetterBodyByHeaderIdAsync(Guid headerId)
    {
        var allLetters = await this.Provider.GetRepository<LetterBody>().GetAllAsync().ConfigureAwait(false);
        return allLetters.FirstOrDefault(body => body.Header.Id == headerId);
    }

    /// <inheritdoc/>
    public async ValueTask<MUnique.OpenMU.DataModel.Entities.Account?> GetAccountByLoginNameAsync(string loginName, string password)
    {
        var allAccounts = await this.Provider.GetRepository<Account>().GetAllAsync().ConfigureAwait(false);
        return allAccounts.FirstOrDefault(account => account.LoginName == loginName && BCrypt.Net.BCrypt.Verify(password, account.PasswordHash));
    }

    /// <inheritdoc/>
    public async ValueTask<MUnique.OpenMU.DataModel.Entities.Account?> GetAccountByLoginNameAsync(string loginName)
    {
        var allAccounts = await this.Provider.GetRepository<Account>().GetAllAsync().ConfigureAwait(false);
        return allAccounts.FirstOrDefault(account => account.LoginName == loginName);
    }

    /// <inheritdoc/>
    public async ValueTask<IEnumerable<MUnique.OpenMU.DataModel.Entities.Account>> GetAccountsOrderedByLoginNameAsync(int skip, int count)
    {
        var allAccounts = await this.Provider.GetRepository<Account>().GetAllAsync().ConfigureAwait(false);
        return allAccounts.OrderBy(a => a.LoginName).Skip(skip).Take(count);
    }

    /// <inheritdoc/>
    public async ValueTask<bool> CanSaveLetterAsync(Interfaces.LetterHeader letterHeader)
    {
        return true;
    }

    /// <inheritdoc />
    public async ValueTask<DataModel.Entities.Account?> GetAccountByCharacterNameAsync(string characterName)
    {
        var allAccounts = await this.Provider.GetRepository<Account>().GetAllAsync().ConfigureAwait(false);
        return allAccounts.FirstOrDefault(account => account.Characters.Any(c => c.Name == characterName));
    }
}