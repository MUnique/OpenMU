// <copyright file="ApiKeyRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.AdminAuth;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// Implementation of the <see cref="IApiKeyRepository"/> which stores the keys
/// in the <c>admin</c> schema of the configured PostgreSQL database.
/// </summary>
public class ApiKeyRepository : IApiKeyRepository
{
    private readonly IAdminUserRepository _adminUserRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyRepository"/> class.
    /// </summary>
    /// <param name="adminUserRepository">The repository of the admin users, which owns the migration of the shared context.</param>
    public ApiKeyRepository(IAdminUserRepository adminUserRepository)
    {
        this._adminUserRepository = adminUserRepository;
    }

    /// <inheritdoc />
    /// <remarks>
    /// Both tables live in the same context, so the migration is run by the admin user repository
    /// and doesn't have to be triggered a second time here.
    /// </remarks>
    public ValueTask<bool> EnsureStorageAsync(CancellationToken cancellationToken = default)
        => this._adminUserRepository.EnsureStorageAsync(cancellationToken);

    /// <inheritdoc />
    public async ValueTask<IList<ApiKey>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (!await this.EnsureStorageAsync(cancellationToken).ConfigureAwait(false))
        {
            return new List<ApiKey>();
        }

        await using var context = new AdminPanelContext();
        return await context.ApiKeys
            .AsNoTracking()
            .OrderBy(k => k.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<ApiKey?> GetEnabledByHashAsync(string keyHash, CancellationToken cancellationToken = default)
    {
        if (!await this.EnsureStorageAsync(cancellationToken).ConfigureAwait(false))
        {
            return null;
        }

        await using var context = new AdminPanelContext();
        return await context.ApiKeys
            .AsNoTracking()
            .FirstOrDefaultAsync(k => k.KeyHash == keyHash && !k.IsDisabled, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask AddAsync(ApiKey apiKey, CancellationToken cancellationToken = default)
    {
        await this.EnsureAvailableStorageAsync(cancellationToken).ConfigureAwait(false);

        await using var context = new AdminPanelContext();
        context.ApiKeys.Add(apiKey);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask UpdateAsync(ApiKey apiKey, CancellationToken cancellationToken = default)
    {
        await this.EnsureAvailableStorageAsync(cancellationToken).ConfigureAwait(false);

        await using var context = new AdminPanelContext();
        context.ApiKeys.Update(apiKey);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask DeleteAsync(ApiKey apiKey, CancellationToken cancellationToken = default)
    {
        await this.EnsureAvailableStorageAsync(cancellationToken).ConfigureAwait(false);

        await using var context = new AdminPanelContext();
        context.ApiKeys.Remove(apiKey);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask EnsureAvailableStorageAsync(CancellationToken cancellationToken)
    {
        if (!await this.EnsureStorageAsync(cancellationToken).ConfigureAwait(false))
        {
            throw new InvalidOperationException("The API key storage is not available. Please check the database connection.");
        }
    }
}
