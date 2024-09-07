// <copyright file="AccountRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Persistence.EntityFramework.Json;
using MUnique.OpenMU.Persistence.EntityFramework.Model;
using System.Linq;
using System.Threading;

/// <summary>
/// Repository for accounts.
/// </summary>
internal class AccountRepository : CachingGenericRepository<Account>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AccountRepository" /> class.
    /// </summary>
    /// <param name="repositoryProvider">The repository provider.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public AccountRepository(IContextAwareRepositoryProvider repositoryProvider, ILoggerFactory loggerFactory)
        : base(repositoryProvider, loggerFactory)
    {
    }

    /// <inheritdoc />
    public override async ValueTask<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        (this.RepositoryProvider as ICacheAwareRepositoryProvider)?.EnsureCachesForCurrentGameConfiguration();

        using var context = this.GetContext();
        await context.Context.Database.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var accountEntry = context.Context.ChangeTracker.Entries<Account>().FirstOrDefault(a => a.Entity.Id == id);
            var account = accountEntry?.Entity;
            if (account is null || accountEntry?.References.Any(reference => !reference.IsLoaded) is true)
            {
                if (account is not null)
                {
                    context.Detach(account);
                }

                var objectLoader = new AccountJsonObjectLoader();
                account = await objectLoader.LoadObjectAsync<Account>(id, context.Context, cancellationToken).ConfigureAwait(false);
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
    /// Gets the account by character name.
    /// </summary>
    /// <param name="characterName">The character name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The account; otherwise, null.
    /// </returns>
    internal async ValueTask<DataModel.Entities.Account?> GetAccountByCharacterNameAsync(string characterName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var context = this.GetContext();
        var accountInfo = await context.Context.Set<Account>()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.RawCharacters.Any(c => c.Name == characterName), cancellationToken)
            .ConfigureAwait(false);

        if (accountInfo != null)
        {
            return await this.GetByIdAsync(accountInfo.Id, cancellationToken).ConfigureAwait(false);
        }

        return null;
    }

    /// <summary>
    /// Gets the account by login name if the password is correct.
    /// </summary>
    /// <param name="loginName">The login name.</param>
    /// <param name="password">The password.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The account, if the password is correct. Otherwise, null.
    /// </returns>
    internal async ValueTask<DataModel.Entities.Account?> GetAccountByLoginNameAsync(string loginName, string password, CancellationToken cancellationToken = default)
    {
        using var context = this.GetContext();
        return await this.LoadAccountByLoginNameByJsonQueryAsync(loginName, password, context, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the account by login name.
    /// </summary>
    /// <param name="loginName">The login name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The account, if exists. Otherwise, null.
    /// </returns>
    internal async ValueTask<DataModel.Entities.Account?> GetAccountByLoginNameAsync(string loginName, CancellationToken cancellationToken = default)
    {
        using var context = this.GetContext();

        var accountInfo = await context.Context.Set<Account>()
           .Select(a => new { a.Id, a.LoginName })
           .AsNoTracking()
           .FirstOrDefaultAsync(a => a.LoginName == loginName, cancellationToken).ConfigureAwait(false);

        if (accountInfo != null)
        {
            return await this.GetByIdAsync(accountInfo.Id, cancellationToken).ConfigureAwait(false);
        }

        return null;
    }

    private async ValueTask<Account?> LoadAccountByLoginNameByJsonQueryAsync(string loginName, string password, EntityFrameworkContextBase context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var accountInfo = await context.Context.Set<Account>()
            .Select(a => new { a.Id, a.LoginName, a.PasswordHash })
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.LoginName == loginName, cancellationToken).ConfigureAwait(false);

        if (accountInfo != null && BCrypt.Verify(password, accountInfo.PasswordHash))
        {
            return await this.GetByIdAsync(accountInfo.Id, cancellationToken).ConfigureAwait(false);
        }

        return null;
    }
}