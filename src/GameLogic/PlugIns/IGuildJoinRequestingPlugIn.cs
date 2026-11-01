// <copyright file="IGuildJoinRequestingPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.ComponentModel;
using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Plugins which are called when a player requests to join the guild of a guild master, and when the guild master accepts it.
/// They can deny the request, e.g. when they are members of different gens.
/// </summary>
[Guid("8E4A1D63-2C95-4B07-A3F8-6D2B9E5C1A70")]
[PlugInPoint("Guild join requesting", "Plugins which are called when a player requests to join a guild, and when the guild master accepts it. They can deny the request.")]
public interface IGuildJoinRequestingPlugIn
{
    /// <summary>
    /// Is called when a player requests to join the guild of a guild master, and when the guild master accepts it.
    /// </summary>
    /// <param name="requester">The player which requests to join the guild.</param>
    /// <param name="guildMaster">The guild master.</param>
    /// <param name="eventArgs">The <see cref="CancelEventArgs"/> instance containing the event data. The request can be denied by setting <see cref="CancelEventArgs.Cancel"/> to <c>true</c>.
    /// The plugin should then show the reason to the requester.</param>
    ValueTask GuildJoinRequestingAsync(Player requester, Player guildMaster, CancelEventArgs eventArgs);
}
