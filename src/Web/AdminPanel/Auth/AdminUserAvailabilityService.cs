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
/// That mode is only granted when the storage said that no user exists, or when it isn't available
/// at all. As long as the answer isn't known - while the first check after the start runs, or while
/// the users are counted again after one was created or deleted - a login is required.
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
    private readonly Lock _lock = new();

    private UserState _state = UserState.Unknown;
    private DateTime _nextCheck = DateTime.MinValue;

    /// <summary>
    /// Counts the changes of the user list, so that a check which ran meanwhile doesn't overwrite them.
    /// </summary>
    private int _changeCount;

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
    /// What is known about the stored users.
    /// </summary>
    private enum UserState
    {
        /// <summary>
        /// Nothing is known yet, or a user might have been created since the last check.
        /// </summary>
        Unknown,

        /// <summary>
        /// No user exists, or the storage isn't available on a fresh installation.
        /// </summary>
        NoUser,

        /// <summary>
        /// At least one user exists.
        /// </summary>
        UserExists,
    }

    /// <summary>
    /// Determines whether at least one user exists which could log in.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c>, if at least one user exists or it's not known yet; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// This is called by the authorization of every request, so it must never wait for the
    /// database: when another caller is already asking, or when the last answer is still fresh,
    /// the known value is returned right away. <c>false</c> opens the panel without a login, so
    /// it's only returned when the storage said so.
    /// </remarks>
    public async ValueTask<bool> AnyUserExistsAsync(CancellationToken cancellationToken = default)
    {
        if (this._bootstrapUserProvider.User is not null)
        {
            return true;
        }

        if (!this.IsCheckDue(out var knownAnswer) || !await this._semaphore.WaitAsync(0, cancellationToken).ConfigureAwait(false))
        {
            return knownAnswer;
        }

        // The answer is shared by all callers, so the check runs to its end even when this caller gives up.
        // The repository can't tell a cancelled check from an unavailable storage, which would mean "no user".
        return await this.CheckAndReleaseAsync().WaitAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Remembers that a user exists, after one has been created.
    /// </summary>
    public void NotifyUserCreated()
    {
        lock (this._lock)
        {
            this._state = UserState.UserExists;
            this._nextCheck = DateTime.MaxValue;
            this._changeCount++;
        }
    }

    /// <summary>
    /// Lets the next call ask the storage again, e.g. after a user has been deleted.
    /// </summary>
    /// <remarks>
    /// Until then, the initial setup mode isn't granted: known users are assumed to still exist, and
    /// when no user existed, one might have been created meanwhile.
    /// </remarks>
    public void Invalidate()
    {
        lock (this._lock)
        {
            if (this._state == UserState.NoUser)
            {
                this._state = UserState.Unknown;
            }

            this._nextCheck = DateTime.MinValue;
            this._changeCount++;
        }
    }

    private bool IsCheckDue(out bool knownAnswer)
    {
        lock (this._lock)
        {
            knownAnswer = this._state != UserState.NoUser;
            return DateTime.UtcNow >= this._nextCheck;
        }
    }

    private async Task<bool> CheckAndReleaseAsync()
    {
        try
        {
            int changeCount;
            lock (this._lock)
            {
                if (DateTime.UtcNow < this._nextCheck)
                {
                    return this._state != UserState.NoUser;
                }

                changeCount = this._changeCount;
            }

            bool? anyUserExists = null;
            if (await this._repository.EnsureStorageAsync().ConfigureAwait(false))
            {
                anyUserExists = await this._repository.GetCountAsync().ConfigureAwait(false) > 0;
            }

            lock (this._lock)
            {
                // When a user was created or deleted meanwhile, the result might be outdated already.
                if (changeCount == this._changeCount)
                {
                    this._state = anyUserExists switch
                    {
                        true => UserState.UserExists,
                        false => UserState.NoUser,

                        // When the storage isn't available, we can't tell - the previous answer is kept. Before
                        // the first answer, this is the initial setup mode on a fresh installation. The installation
                        // needs the database as well, so there is nothing to protect at that point anyway.
                        null => this._state == UserState.Unknown ? UserState.NoUser : this._state,
                    };

                    this._nextCheck = anyUserExists is true ? DateTime.MaxValue : DateTime.UtcNow.Add(CheckInterval);
                }

                return this._state != UserState.NoUser;
            }
        }
        finally
        {
            this._semaphore.Release();
        }
    }
}
