// <copyright file="IGuildCreatingPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.ComponentModel;
using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Plugins which are called when a player is about to create a guild.
/// They can deny the creation, e.g. when the player is not a member of a gens.
/// </summary>
[Guid("2D7F5B19-E863-4C21-8A4D-9F0C3E6B7D58")]
[PlugInPoint("Guild creating", "Plugins which are called when a player is about to create a guild. They can deny the creation.")]
public interface IGuildCreatingPlugIn
{
    /// <summary>
    /// Is called when a player is about to create a guild.
    /// </summary>
    /// <param name="creator">The player which creates the guild.</param>
    /// <param name="eventArgs">The <see cref="CancelEventArgs"/> instance containing the event data. The creation can be denied by setting <see cref="CancelEventArgs.Cancel"/> to <c>true</c>.
    /// The plugin should then show the reason to the creator.</param>
    ValueTask GuildCreatingAsync(Player creator, CancelEventArgs eventArgs);
}
