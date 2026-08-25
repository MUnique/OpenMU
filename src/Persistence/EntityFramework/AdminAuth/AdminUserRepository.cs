// <copyright file="AdminUserRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.AdminAuth;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Persistence.AdminAuth;
using Nito.AsyncEx;

/// <summary>
/// Implementation of the <see cref="IAdminUserRepository"/> which stores the users
/// in the <c>admin</c> schema of the configured PostgreSQL database.
/// </summary>
public class AdminUserRepository : IAdminUserRepository
{
    private readonly ILogger<AdminUserRepository> _logger;
    private readonly AsyncLock _storageLock = new();
    private bool _isStorageReady;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminUserRepository"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public AdminUserRepository(ILogger<AdminUserRepository> logger)
    {
        this._logger = logger;
    }

    /// <inheritdoc />
    public async ValueTask<bool> EnsureStorageAsync(CancellationToken cancellationToken = default)
    {
        if (this._isStorageReady)
        {
            return true;
        }

        using var l = await this._storageLock.LockAsync(cancellationToken).ConfigureAwait(false);
        if (this._isStorageReady)
        {
            return true;
        }

        try
        {
            await using var context = new AdminPanelContext();
            await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            this._isStorageReady = true;
        }
        catch (Exception ex)
        {
            // This is an expected state before the database server is reachable or the database has been created.
            // The admin panel then falls back to the configured bootstrap user.
            this._logger.LogInformation(ex, "The admin user storage is not available (yet).");
        }

        return this._isStorageReady;
    }

    /// <inheritdoc />
    public async ValueTask<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        if (!await this.EnsureStorageAsync(cancellationToken).ConfigureAwait(false))
        {
            return 0;
        }

        await using var context = new AdminPanelContext();
        return await context.AdminUsers.CountAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<IList<AdminUser>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (!await this.EnsureStorageAsync(cancellationToken).ConfigureAwait(false))
        {
            return new List<AdminUser>();
        }

        await using var context = new AdminPanelContext();
        return await context.AdminUsers
            .AsNoTracking()
            .OrderBy(u => u.LoginName)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<AdminUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!await this.EnsureStorageAsync(cancellationToken).ConfigureAwait(false))
        {
            return null;
        }

        await using var context = new AdminPanelContext();
        return await context.AdminUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<AdminUser?> GetByNormalizedLoginNameAsync(string normalizedLoginName, CancellationToken cancellationToken = default)
    {
        if (!await this.EnsureStorageAsync(cancellationToken).ConfigureAwait(false))
        {
            return null;
        }

        await using var context = new AdminPanelContext();
        return await context.AdminUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.NormalizedLoginName == normalizedLoginName, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask AddAsync(AdminUser user, CancellationToken cancellationToken = default)
    {
        await this.EnsureAvailableStorageAsync(cancellationToken).ConfigureAwait(false);

        await using var context = new AdminPanelContext();
        context.AdminUsers.Add(user);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask UpdateAsync(AdminUser user, CancellationToken cancellationToken = default)
    {
        await this.EnsureAvailableStorageAsync(cancellationToken).ConfigureAwait(false);

        await using var context = new AdminPanelContext();
        context.AdminUsers.Update(user);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask DeleteAsync(AdminUser user, CancellationToken cancellationToken = default)
    {
        await this.EnsureAvailableStorageAsync(cancellationToken).ConfigureAwait(false);

        await using var context = new AdminPanelContext();
        context.AdminUsers.Remove(user);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask EnsureAvailableStorageAsync(CancellationToken cancellationToken)
    {
        if (!await this.EnsureStorageAsync(cancellationToken).ConfigureAwait(false))
        {
            throw new InvalidOperationException("The admin user storage is not available. Please check the database connection.");
        }
    }
}
