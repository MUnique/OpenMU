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
/// Every removal keeps its Discord server and the name of the role, so that it still applies to them when the settings change.
/// The updates of the role of a user run one after another, so that an older removal can't take away the role of a newer link.
/// The updates of different users don't wait for each other.
/// </remarks>
internal sealed class DiscordPendingRoleRevocations
{
    private readonly ConcurrentDictionary<DiscordPendingRoleRevocation, byte> _entries = new();
    private readonly Dictionary<ulong, UserLock> _locks = new();

    /// <summary>
    /// Gets the number of pending removals.
    /// </summary>
    internal int Count => this._entries.Count;

    /// <summary>
    /// Gets the pending removals.
    /// </summary>
    internal IReadOnlyCollection<DiscordPendingRoleRevocation> Entries => this._entries.Keys.ToList();

    /// <summary>
    /// Adds a user, whose role has to be removed.
    /// </summary>
    /// <param name="userId">The identifier of the Discord user.</param>
    /// <param name="guildId">The identifier of the Discord server, in which the role is removed; <c>null</c>, if it isn't known yet.</param>
    /// <param name="roleName">The name of the role, which the user got; <c>null</c> for the role of the current layout.</param>
    internal void Add(ulong userId, ulong? guildId, string? roleName = null) => this._entries.TryAdd(new DiscordPendingRoleRevocation(userId, guildId, roleName), 0);

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
            this.RemoveAll(userId);
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
    /// <param name="revokeAsync">Removes the role of a user in the Discord server of the removal and returns, if it succeeded.</param>
    /// <returns>The number of removed roles.</returns>
    internal async Task<int> ProcessAsync(Func<ulong, Task<bool?>> isLinkedAsync, Func<DiscordPendingRoleRevocation, Task<bool>> revokeAsync)
    {
        var succeeded = 0;
        foreach (var entry in this._entries.Keys)
        {
            var userLock = await this.AcquireAsync(entry.UserId).ConfigureAwait(false);
            try
            {
                // The user might have been linked again in the meantime. A new link adds the role after it's stored,
                // and under the same lock, so it can't happen between the check and the removal.
                if (!this._entries.ContainsKey(entry))
                {
                    continue;
                }

                switch (await isLinkedAsync(entry.UserId).ConfigureAwait(false))
                {
                    case null:
                        // Unknown, so it's retried later.
                        break;
                    case true:
                        this.RemoveAll(entry.UserId);
                        break;
                    case false when await revokeAsync(entry).ConfigureAwait(false):
                        this._entries.TryRemove(entry, out _);
                        succeeded++;
                        break;
                    default:
                        // The removal failed, so it's retried later.
                        break;
                }
            }
            finally
            {
                this.Release(entry.UserId, userLock);
            }
        }

        return succeeded;
    }

    private void RemoveAll(ulong userId)
    {
        foreach (var entry in this._entries.Keys.Where(entry => entry.UserId == userId))
        {
            this._entries.TryRemove(entry, out _);
        }
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
