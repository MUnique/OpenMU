// <copyright file="DiscordPendingRoleRevocations.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// The Discord users whose role of linked players still has to be removed,
/// e.g. because their link was removed in the admin panel while the bot wasn't connected.
/// </summary>
/// <remarks>
/// The users are kept in memory, so they are retried until the bot succeeds or the process ends.
/// The updates of the role of a user run one after another, so that an older removal can't take away the role of a newer link.
/// The updates of different users don't wait for each other.
/// </remarks>
internal sealed class DiscordPendingRoleRevocations
{
    private readonly ConcurrentDictionary<ulong, byte> _userIds = new();
    private readonly Dictionary<ulong, UserLock> _locks = new();

    /// <summary>
    /// Gets the number of pending users.
    /// </summary>
    internal int Count => this._userIds.Count;

    /// <summary>
    /// Adds a user, whose role has to be removed.
    /// </summary>
    /// <param name="userId">The identifier of the Discord user.</param>
    internal void Add(ulong userId) => this._userIds.TryAdd(userId, 0);

    /// <summary>
    /// Removes a pending removal of a user, who was linked again, and adds the role.
    /// It waits for a running removal, so that the role is added after it.
    /// </summary>
    /// <param name="userId">The identifier of the Discord user.</param>
    /// <param name="addRoleAsync">Adds the role of the user.</param>
    /// <returns>The task.</returns>
    internal async Task LinkAsync(ulong userId, Func<Task> addRoleAsync)
    {
        var userLock = await this.AcquireAsync(userId).ConfigureAwait(false);
        try
        {
            this._userIds.TryRemove(userId, out _);
            await addRoleAsync().ConfigureAwait(false);
        }
        finally
        {
            this.Release(userId, userLock);
        }
    }

    /// <summary>
    /// Tries to remove the roles of all pending users. The users, for which it fails, stay pending.
    /// The users, which are linked again, are dropped without removing the role, because an unlink might be delivered late,
    /// e.g. after the user was linked again.
    /// </summary>
    /// <param name="isLinkedAsync">Determines, if a user is currently linked; <c>null</c>, if it's unknown, e.g. because the database isn't available. Then the user stays pending.</param>
    /// <param name="revokeAsync">Removes the role of a user and returns, if it succeeded.</param>
    /// <returns>The number of users, for which the role was removed.</returns>
    internal async Task<int> ProcessAsync(Func<ulong, Task<bool?>> isLinkedAsync, Func<ulong, Task<bool>> revokeAsync)
    {
        var succeeded = 0;
        foreach (var userId in this._userIds.Keys)
        {
            var userLock = await this.AcquireAsync(userId).ConfigureAwait(false);
            try
            {
                // The user might have been linked again in the meantime. A new link adds the role after it's stored,
                // and under the same lock, so it can't happen between the check and the removal.
                if (!this._userIds.ContainsKey(userId))
                {
                    continue;
                }

                var isLinked = await isLinkedAsync(userId).ConfigureAwait(false);
                if (isLinked is null)
                {
                    continue;
                }

                if (isLinked.Value)
                {
                    this._userIds.TryRemove(userId, out _);
                }
                else if (await revokeAsync(userId).ConfigureAwait(false))
                {
                    this._userIds.TryRemove(userId, out _);
                    succeeded++;
                }
            }
            finally
            {
                this.Release(userId, userLock);
            }
        }

        return succeeded;
    }

    private async Task<UserLock> AcquireAsync(ulong userId)
    {
        UserLock userLock;
        lock (this._locks)
        {
            if (!this._locks.TryGetValue(userId, out userLock!))
            {
                userLock = new UserLock();
                this._locks.Add(userId, userLock);
            }

            userLock.Users++;
        }

        await userLock.Semaphore.WaitAsync().ConfigureAwait(false);
        return userLock;
    }

    private void Release(ulong userId, UserLock userLock)
    {
        userLock.Semaphore.Release();
        lock (this._locks)
        {
            // The lock is removed when nobody uses it, so that the locks don't pile up.
            if (--userLock.Users == 0)
            {
                this._locks.Remove(userId);
            }
        }
    }

    /// <summary>
    /// The lock of a user, and the number of operations which use or wait for it.
    /// </summary>
    private sealed class UserLock
    {
        /// <summary>
        /// Gets the semaphore.
        /// </summary>
        public SemaphoreSlim Semaphore { get; } = new(1, 1);

        /// <summary>
        /// Gets or sets the number of operations which use or wait for the lock.
        /// </summary>
        public int Users { get; set; }
    }
}
