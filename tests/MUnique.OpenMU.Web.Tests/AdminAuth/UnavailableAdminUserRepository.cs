// <copyright file="UnavailableAdminUserRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.AdminAuth;

using System.Threading;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// An <see cref="IAdminUserRepository"/> which behaves like an unreachable database and counts
/// how often it was asked for its availability.
/// </summary>
internal class UnavailableAdminUserRepository : IAdminUserRepository
{
    /// <summary>
    /// Gets the number of calls to <see cref="EnsureStorageAsync"/>.
    /// </summary>
    public int EnsureStorageCallCount { get; private set; }

    /// <inheritdoc />
    public ValueTask<bool> EnsureStorageAsync(CancellationToken cancellationToken = default)
    {
        this.EnsureStorageCallCount++;
        return ValueTask.FromResult(false);
    }

    /// <inheritdoc />
    public ValueTask<int> GetCountAsync(CancellationToken cancellationToken = default)
        => throw new InvalidOperationException("The storage is not available, so it must not be queried.");

    /// <inheritdoc />
    public ValueTask<IList<AdminUser>> GetAllAsync(CancellationToken cancellationToken = default)
        => ValueTask.FromResult<IList<AdminUser>>(new List<AdminUser>());

    /// <inheritdoc />
    public ValueTask<AdminUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => ValueTask.FromResult<AdminUser?>(null);

    /// <inheritdoc />
    public ValueTask<AdminUser?> GetByNormalizedLoginNameAsync(string normalizedLoginName, CancellationToken cancellationToken = default)
        => ValueTask.FromResult<AdminUser?>(null);

    /// <inheritdoc />
    public ValueTask AddAsync(AdminUser user, CancellationToken cancellationToken = default) => throw new InvalidOperationException();

    /// <inheritdoc />
    public ValueTask UpdateAsync(AdminUser user, CancellationToken cancellationToken = default) => throw new InvalidOperationException();

    /// <inheritdoc />
    public ValueTask DeleteAsync(AdminUser user, CancellationToken cancellationToken = default) => throw new InvalidOperationException();
}
