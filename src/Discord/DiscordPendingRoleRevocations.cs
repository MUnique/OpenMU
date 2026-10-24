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
/// All updates of the role run one after another, so that an older removal can't take away the role of a newer link.
/// </remarks>
internal sealed class DiscordPendingRoleRevocations
{
    private readonly ConcurrentDictionary<ulong, byte> _userIds = new();
    private readonly SemaphoreSlim _lock = new(1, 1);

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
        await this._lock.WaitAsync().ConfigureAwait(false);
        try
        {
            this._userIds.TryRemove(userId, out _);
            await addRoleAsync().ConfigureAwait(false);
        }
        finally
        {
            this._lock.Release();
        }
    }

    /// <summary>
    /// Tries to remove the roles of all pending users. The users, for which it fails, stay pending.
    /// </summary>
    /// <param name="revokeAsync">Removes the role of a user and returns, if it succeeded.</param>
    /// <returns>The number of users, for which it succeeded.</returns>
    internal async Task<int> ProcessAsync(Func<ulong, Task<bool>> revokeAsync)
    {
        var succeeded = 0;
        foreach (var userId in this._userIds.Keys)
        {
            await this._lock.WaitAsync().ConfigureAwait(false);
            try
            {
                // The user might have been linked again in the meantime.
                if (this._userIds.ContainsKey(userId) && await revokeAsync(userId).ConfigureAwait(false))
                {
                    this._userIds.TryRemove(userId, out _);
                    succeeded++;
                }
            }
            finally
            {
                this._lock.Release();
            }
        }

        return succeeded;
    }
}
