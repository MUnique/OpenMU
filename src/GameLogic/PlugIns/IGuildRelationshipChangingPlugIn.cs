// <copyright file="IGuildRelationshipChangingPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.ComponentModel;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Plugins which are called when the master of a guild requests to change the relationship to another guild, e.g. to create an alliance.
/// They can deny the request, e.g. when the guild masters are members of different gens.
/// </summary>
[Guid("C61E8A4F-5D30-4B92-B7E5-1A9D4C2F8E06")]
[PlugInPoint("Guild relationship changing", "Plugins which are called when a guild master requests to change the relationship to another guild. They can deny the request.")]
public interface IGuildRelationshipChangingPlugIn
{
    /// <summary>
    /// Is called when the master of a guild requests to change the relationship to another guild.
    /// </summary>
    /// <param name="requester">The guild master which requests the change.</param>
    /// <param name="targetGuildMaster">The master of the other guild.</param>
    /// <param name="relationshipType">The type of the relationship.</param>
    /// <param name="requestType">The type of the request.</param>
    /// <param name="eventArgs">The <see cref="CancelEventArgs"/> instance containing the event data. The request can be denied by setting <see cref="CancelEventArgs.Cancel"/> to <c>true</c>.
    /// The plugin should then show the reason to the requester.</param>
    ValueTask GuildRelationshipChangingAsync(Player requester, Player targetGuildMaster, GuildRelationshipType relationshipType, GuildRelationshipRequestType requestType, CancelEventArgs eventArgs);
}
