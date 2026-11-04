// <copyright file="IPartyRequestingPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.ComponentModel;
using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Plugins which are called when a player requests a party with another player, and when the other player accepts it.
/// They can deny the party, e.g. between the members of different gens.
/// </summary>
[Guid("5B9E2C81-4A37-4F60-9D12-8C3E7A1B6F24")]
[PlugInPoint("Party requesting", "Plugins which are called when a player requests a party with another player, and when the other player accepts it. They can deny the party.")]
public interface IPartyRequestingPlugIn
{
    /// <summary>
    /// Is called when a player requests a party with another player, and when the other player accepts it.
    /// </summary>
    /// <param name="requester">The player which requested the party.</param>
    /// <param name="target">The requested player.</param>
    /// <param name="eventArgs">The <see cref="CancelEventArgs"/> instance containing the event data. The party can be denied by setting <see cref="CancelEventArgs.Cancel"/> to <c>true</c>.
    /// The plugin should then show the reason to the players.</param>
    ValueTask PartyRequestingAsync(Player requester, Player target, CancelEventArgs eventArgs);
}
