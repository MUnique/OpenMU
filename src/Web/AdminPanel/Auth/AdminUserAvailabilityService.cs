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
    /// <summary>
    /// The time for which the answer is reused before the storage is asked again.
    /// </summary>
    private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(10);

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
    /// <remarks>
    /// This is called by the authorization of every request, so it must never wait for the
    /// database: when another caller is already asking, or when the last answer is still fresh,
    /// the known value is returned right away.
    /// </remarks>
    public async ValueTask<bool> AnyUserExistsAsync(CancellationToken cancellationToken = default)
    {
        if (this._bootstrapUserProvider.User is not null || this._anyUserExists)
        {
            return true;
        }

        if (DateTime.UtcNow < this._nextCheck || !await this._semaphore.WaitAsync(0, cancellationToken).ConfigureAwait(false))
        {
            return this._anyUserExists;
        }

        try
        {
            if (this._anyUserExists || DateTime.UtcNow < this._nextCheck)
            {
                return this._anyUserExists;
            }

            if (await this._repository.EnsureStorageAsync(cancellationToken).ConfigureAwait(false))
            {
                this._anyUserExists = await this._repository.GetCountAsync(cancellationToken).ConfigureAwait(false) > 0;
            }

            // When the storage isn't available, we can't tell - the previous answer is kept, which
            // is the initial setup mode on a fresh installation. The installation needs the database
            // as well, so there is nothing to protect at that point anyway.
            this._nextCheck = DateTime.UtcNow.Add(CheckInterval);
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
