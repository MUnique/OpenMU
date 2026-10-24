// <copyright file="DiscordPendingRoleRevocations.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.Collections.Concurrent;
using System.Threading.Tasks;

/// <summary>
/// The Discord users whose role of linked players still has to be removed,
/// e.g. because their link was removed in the admin panel while the bot wasn't connected.
/// </summary>
/// <remarks>
/// The users are kept in memory, so they are retried until the bot succeeds or the process ends.
/// </remarks>
internal sealed class DiscordPendingRoleRevocations
{
    private readonly ConcurrentDictionary<ulong, byte> _userIds = new();

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
    /// Removes a user, e.g. because the user was linked again.
    /// </summary>
    /// <param name="userId">The identifier of the Discord user.</param>
    internal void Remove(ulong userId) => this._userIds.TryRemove(userId, out _);

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
            if (await revokeAsync(userId).ConfigureAwait(false))
            {
                this._userIds.TryRemove(userId, out _);
                succeeded++;
            }
        }

        return succeeded;
    }
}
