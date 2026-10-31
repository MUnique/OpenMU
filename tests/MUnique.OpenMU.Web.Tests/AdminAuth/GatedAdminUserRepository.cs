// <copyright file="GatedAdminUserRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.AdminAuth;

using System.Threading;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// An in-memory <see cref="IAdminUserRepository"/> whose storage check and user count can be held,
/// like a database which is still running its migrations or answers slowly.
/// </summary>
internal sealed class GatedAdminUserRepository : IAdminUserRepository
{
    private readonly InMemoryAdminUserRepository _users = new();

    /// <summary>
    /// Gets the gate in front of <see cref="EnsureStorageAsync"/>.
    /// </summary>
    public Gate StorageGate { get; } = new();

    /// <summary>
    /// Gets the gate in front of <see cref="GetCountAsync"/>.
    /// </summary>
    public Gate CountGate { get; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="GetCountAsync"/> counts the users before it waits
    /// at the <see cref="CountGate"/>, so that its result is outdated when users change meanwhile.
    /// </summary>
    public bool CountsBeforeGate { get; set; }

    /// <inheritdoc />
    public async ValueTask<bool> EnsureStorageAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await this.StorageGate.PassAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Like the database backed repository: a probe which didn't finish counts as "not available".
            return false;
        }

        return await this._users.EnsureStorageAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        var countBeforeGate = await this._users.GetCountAsync(cancellationToken).ConfigureAwait(false);
        await this.CountGate.PassAsync(cancellationToken).ConfigureAwait(false);
        return this.CountsBeforeGate ? countBeforeGate : await this._users.GetCountAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public ValueTask<IList<AdminUser>> GetAllAsync(CancellationToken cancellationToken = default)
        => this._users.GetAllAsync(cancellationToken);

    /// <inheritdoc />
    public ValueTask<AdminUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => this._users.GetByIdAsync(id, cancellationToken);

    /// <inheritdoc />
    public ValueTask<AdminUser?> GetByNormalizedLoginNameAsync(string normalizedLoginName, CancellationToken cancellationToken = default)
        => this._users.GetByNormalizedLoginNameAsync(normalizedLoginName, cancellationToken);

    /// <inheritdoc />
    public ValueTask AddAsync(AdminUser user, CancellationToken cancellationToken = default)
        => this._users.AddAsync(user, cancellationToken);

    /// <inheritdoc />
    public ValueTask UpdateAsync(AdminUser user, CancellationToken cancellationToken = default)
        => this._users.UpdateAsync(user, cancellationToken);

    /// <inheritdoc />
    public ValueTask<bool> TryAdvanceTotpStepAsync(Guid userId, long step, CancellationToken cancellationToken = default)
        => this._users.TryAdvanceTotpStepAsync(userId, step, cancellationToken);

    /// <inheritdoc />
    public ValueTask DeleteAsync(AdminUser user, CancellationToken cancellationToken = default)
        => this._users.DeleteAsync(user, cancellationToken);

    /// <summary>
    /// A gate which lets every caller through until it's closed, and then holds them until it's opened again.
    /// </summary>
    internal sealed class Gate
    {
        private TaskCompletionSource _entered = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private TaskCompletionSource? _release;

        /// <summary>
        /// Gets a task which completes as soon as a caller waits at the closed gate.
        /// </summary>
        public Task Entered => this._entered.Task;

        /// <summary>
        /// Closes the gate, so that the next callers wait.
        /// </summary>
        public void Close()
        {
            this._entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            this._release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        /// <summary>
        /// Opens the gate and lets the waiting callers continue.
        /// </summary>
        public void Open() => this._release?.TrySetResult();

        /// <summary>
        /// Passes the gate, waiting while it's closed.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task.</returns>
        public async Task PassAsync(CancellationToken cancellationToken)
        {
            if (this._release is { } release)
            {
                this._entered.TrySetResult();
                await release.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
