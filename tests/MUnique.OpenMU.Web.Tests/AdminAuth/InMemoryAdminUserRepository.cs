// <copyright file="InMemoryAdminUserRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.AdminAuth;

using System.Text.Json;
using System.Threading;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// An in-memory <see cref="IAdminUserRepository"/> for the tests.
/// </summary>
/// <remarks>
/// By default, all callers share the stored user instances. In the detached mode, it behaves like the
/// database backed repository instead: each read returns its own copy, every call yields so that
/// concurrent callers interleave, and <see cref="UpdateAsync"/> doesn't write
/// <see cref="AdminUser.LastAcceptedTotpStep"/>.
/// </remarks>
internal class InMemoryAdminUserRepository : IAdminUserRepository
{
    private readonly Dictionary<Guid, AdminUser> _users = new();

    private readonly Lock _lock = new();

    private readonly bool _detached;

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryAdminUserRepository"/> class.
    /// </summary>
    /// <param name="detached">If set to <c>true</c>, each caller gets its own copies, like from a database.</param>
    public InMemoryAdminUserRepository(bool detached = false)
    {
        this._detached = detached;
    }

    /// <inheritdoc />
    public ValueTask<bool> EnsureStorageAsync(CancellationToken cancellationToken = default) => ValueTask.FromResult(true);

    /// <inheritdoc />
    public ValueTask<int> GetCountAsync(CancellationToken cancellationToken = default) => ValueTask.FromResult(this._users.Count);

    /// <inheritdoc />
    public ValueTask<IList<AdminUser>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (this._lock)
        {
            return ValueTask.FromResult<IList<AdminUser>>(this._users.Values.OrderBy(u => u.LoginName).Select(this.Detach).ToList());
        }
    }

    /// <inheritdoc />
    public async ValueTask<AdminUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await this.YieldIfDetachedAsync().ConfigureAwait(false);
        lock (this._lock)
        {
            return this._users.GetValueOrDefault(id) is { } user ? this.Detach(user) : null;
        }
    }

    /// <inheritdoc />
    public async ValueTask<AdminUser?> GetByNormalizedLoginNameAsync(string normalizedLoginName, CancellationToken cancellationToken = default)
    {
        await this.YieldIfDetachedAsync().ConfigureAwait(false);
        lock (this._lock)
        {
            return this._users.Values.FirstOrDefault(u => u.NormalizedLoginName == normalizedLoginName) is { } user ? this.Detach(user) : null;
        }
    }

    /// <inheritdoc />
    public async ValueTask AddAsync(AdminUser user, CancellationToken cancellationToken = default)
    {
        await this.YieldIfDetachedAsync().ConfigureAwait(false);
        lock (this._lock)
        {
            this._users[user.Id] = this.Detach(user);
        }
    }

    /// <inheritdoc />
    public async ValueTask UpdateAsync(AdminUser user, CancellationToken cancellationToken = default)
    {
        await this.YieldIfDetachedAsync().ConfigureAwait(false);
        lock (this._lock)
        {
            var stored = this.Detach(user);
            if (this._detached && this._users.TryGetValue(user.Id, out var existing))
            {
                // Like the database backed repository: only TryAdvanceTotpStepAsync changes it.
                stored.LastAcceptedTotpStep = existing.LastAcceptedTotpStep;
            }

            this._users[user.Id] = stored;
        }
    }

    /// <inheritdoc />
    public async ValueTask<bool> TryAdvanceTotpStepAsync(Guid userId, long step, CancellationToken cancellationToken = default)
    {
        await this.YieldIfDetachedAsync().ConfigureAwait(false);
        lock (this._lock)
        {
            if (!this._users.TryGetValue(userId, out var user) || step <= user.LastAcceptedTotpStep)
            {
                return false;
            }

            user.LastAcceptedTotpStep = step;
            return true;
        }
    }

    /// <inheritdoc />
    public ValueTask DeleteAsync(AdminUser user, CancellationToken cancellationToken = default)
    {
        lock (this._lock)
        {
            this._users.Remove(user.Id);
        }

        return ValueTask.CompletedTask;
    }

    // All members of a user are public, settable values, so a serialization round trip copies it completely.
    private AdminUser Detach(AdminUser user) => this._detached ? JsonSerializer.Deserialize<AdminUser>(JsonSerializer.Serialize(user))! : user;

    private async ValueTask YieldIfDetachedAsync()
    {
        if (this._detached)
        {
            await Task.Yield();
        }
    }
}
