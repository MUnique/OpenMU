// <copyright file="AccountRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Persistence.EntityFramework.Json;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Repository for accounts.
/// </summary>
internal class AccountRepository : CachingGenericRepository<Account>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AccountRepository" /> class.
    /// </summary>
    /// <param name="repositoryManager">The repository manager.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public AccountRepository(CachingRepositoryManager repositoryManager, ILoggerFactory loggerFactory)
        : base(repositoryManager, loggerFactory)
    {
    }

    /// <inheritdoc />
    public override async ValueTask<Account?> GetByIdAsync(Guid id)
    {
        ((CachingRepositoryManager)this.RepositoryManager).EnsureCachesForCurrentGameConfiguration();
        using var context = this.GetContext();
        await context.Context.Database.OpenConnectionAsync().ConfigureAwait(false);
        try
        {
            var account = context.Context.ChangeTracker.Entries<Account>().FirstOrDefault(a => a.Entity.Id == id)?.Entity;
            if (account is null)
            {
                var objectLoader = new AccountJsonObjectLoader();
                account = await objectLoader.LoadObjectAsync<Account>(id, context.Context).ConfigureAwait(false);
                if (account != null && !(context.Context.Entry(account) is { } entry && entry.State != EntityState.Detached))
                {
                    context.Context.Attach(account);
                }
            }

            return account;
        }
        finally
        {
            await context.Context.Database.CloseConnectionAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Gets the account by login name if the password is correct.
    /// </summary>
    /// <param name="loginName">The login name.</param>
    /// <param name="password">The password.</param>
    /// <returns>The account, if the password is correct. Otherwise, null.</returns>
    internal async ValueTask<DataModel.Entities.Account?> GetAccountByLoginNameAsync(string loginName, string password)
    {
        using var context = this.GetContext();
        return await this.LoadAccountByLoginNameByJsonQueryAsync(loginName, password, context).ConfigureAwait(false);
    }

    private async ValueTask<Account?> LoadAccountByLoginNameByJsonQueryAsync(string loginName, string password, EntityFrameworkContextBase context)
    {
        var accountInfo = await context.Context.Set<Account>()
            .Select(a => new { a.Id, a.LoginName, a.PasswordHash })
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.LoginName == loginName).ConfigureAwait(false);

        if (accountInfo != null && BCrypt.Verify(password, accountInfo.PasswordHash))
        {
            return await this.GetByIdAsync(accountInfo.Id).ConfigureAwait(false);
        }

        return null;
    }
}