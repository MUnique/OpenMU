// <copyright file="BlockingAdminUserRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.AdminAuth;

using System.Threading;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// An <see cref="IAdminUserRepository"/> whose availability check blocks until it's released,
/// like a database server which is not reachable and runs into its connection timeout.
/// </summary>
internal class BlockingAdminUserRepository : IAdminUserRepository
{
    private readonly TaskCompletionSource _probeStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource _release = new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>
    /// Gets a task which completes as soon as the availability check has been entered.
    /// </summary>
    public Task ProbeStarted => this._probeStarted.Task;

    /// <summary>
    /// Lets the blocked availability check continue.
    /// </summary>
    public void Release() => this._release.TrySetResult();

    /// <inheritdoc />
    public async ValueTask<bool> EnsureStorageAsync(CancellationToken cancellationToken = default)
    {
        this._probeStarted.TrySetResult();
        await this._release.Task.ConfigureAwait(false);
        return false;
    }

    /// <inheritdoc />
    public ValueTask<int> GetCountAsync(CancellationToken cancellationToken = default) => ValueTask.FromResult(0);

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
