// <copyright file="AdminUserAvailabilityService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using System.Threading;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// Keeps track of whether the admin panel has any user at all.
/// </summary>
/// <remarks>
/// On a fresh installation there is neither a database nor a user, and the admin panel is the tool
/// which creates both. Until the first user exists, the panel has to stay reachable - it then runs
/// in an unprotected initial setup mode and says so. Configuring a bootstrap user avoids that state.
/// </remarks>
public class AdminUserAvailabilityService
{
    private readonly IAdminUserRepository _repository;
    private readonly BootstrapAdminUserProvider _bootstrapUserProvider;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private DateTime _nextCheck = DateTime.MinValue;
    private bool _anyUserExists;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminUserAvailabilityService"/> class.
    /// </summary>
    /// <param name="repository">The repository of the stored users.</param>
    /// <param name="bootstrapUserProvider">The provider of the bootstrap user.</param>
    public AdminUserAvailabilityService(IAdminUserRepository repository, BootstrapAdminUserProvider bootstrapUserProvider)
    {
        this._repository = repository;
        this._bootstrapUserProvider = bootstrapUserProvider;
    }

    /// <summary>
    /// Determines whether at least one user exists which could log in.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c>, if at least one user exists; otherwise, <c>false</c>.</returns>
    public async ValueTask<bool> AnyUserExistsAsync(CancellationToken cancellationToken = default)
    {
        if (this._bootstrapUserProvider.User is not null)
        {
            return true;
        }

        if (this._anyUserExists)
        {
            return true;
        }

        if (DateTime.UtcNow < this._nextCheck)
        {
            return false;
        }

        await this._semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (this._anyUserExists || DateTime.UtcNow < this._nextCheck)
            {
                return this._anyUserExists;
            }

            this._anyUserExists = await this._repository.GetCountAsync(cancellationToken).ConfigureAwait(false) > 0;

            // The database might not be reachable yet, so don't hammer it on every render.
            this._nextCheck = DateTime.UtcNow.AddSeconds(5);
            return this._anyUserExists;
        }
        finally
        {
            this._semaphore.Release();
        }
    }

    /// <summary>
    /// Invalidates the cached result, e.g. after a user has been created or deleted.
    /// </summary>
    public void Invalidate()
    {
        this._anyUserExists = false;
        this._nextCheck = DateTime.MinValue;
    }
}
