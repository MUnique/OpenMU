// <copyright file="InMemoryAdminUserRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.AdminAuth;

using System.Threading;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// An in-memory <see cref="IAdminUserRepository"/> for the tests.
/// </summary>
internal class InMemoryAdminUserRepository : IAdminUserRepository
{
    private readonly Dictionary<Guid, AdminUser> _users = new();

    /// <inheritdoc />
    public ValueTask<bool> EnsureStorageAsync(CancellationToken cancellationToken = default) => ValueTask.FromResult(true);

    /// <inheritdoc />
    public ValueTask<int> GetCountAsync(CancellationToken cancellationToken = default) => ValueTask.FromResult(this._users.Count);

    /// <inheritdoc />
    public ValueTask<IList<AdminUser>> GetAllAsync(CancellationToken cancellationToken = default)
        => ValueTask.FromResult<IList<AdminUser>>(this._users.Values.OrderBy(u => u.LoginName).ToList());

    /// <inheritdoc />
    public ValueTask<AdminUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(this._users.GetValueOrDefault(id));

    /// <inheritdoc />
    public ValueTask<AdminUser?> GetByNormalizedLoginNameAsync(string normalizedLoginName, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(this._users.Values.FirstOrDefault(u => u.NormalizedLoginName == normalizedLoginName));

    /// <inheritdoc />
    public ValueTask AddAsync(AdminUser user, CancellationToken cancellationToken = default)
    {
        this._users[user.Id] = user;
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask UpdateAsync(AdminUser user, CancellationToken cancellationToken = default)
    {
        this._users[user.Id] = user;
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask DeleteAsync(AdminUser user, CancellationToken cancellationToken = default)
    {
        this._users.Remove(user.Id);
        return ValueTask.CompletedTask;
    }
}
