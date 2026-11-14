// <copyright file="IBotPartyJoinedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Plugins which are called when a server-side bot accepted the party invitation of a player
/// (see <see cref="BotPartyHandler"/>).
/// </summary>
[Guid("224C6C87-3028-4A8B-8785-ECB0E2D3A0D7")]
[PlugInPoint("Bot joined a player's party", "Plugins which are called when a server-side bot accepted the party invitation of a player.")]
public interface IBotPartyJoinedPlugIn
{
    /// <summary>
    /// Is called when the bot joined the party of the inviting player.
    /// </summary>
    /// <param name="bot">The bot.</param>
    /// <param name="inviter">The player whose invitation the bot accepted.</param>
    void BotJoinedParty(Player bot, Player inviter);
}
