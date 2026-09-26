// <copyright file="INpcTalkStartedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when a player starts talking to any NPC, e.g. a merchant.
/// </summary>
/// <remarks>
/// Unlike <see cref="IPlayerTalkToNpcPlugIn"/>, which is only called for NPCs without a window, it's called for every NPC.
/// It can't change what happens, it's just a notification.
/// </remarks>
[Guid("5A0D3E7C-8B41-4F26-9C1E-2F7A6B9D4C58")]
[PlugInPoint("NPC talk started", "Plugins which are called when a player starts talking to any NPC.")]
public interface INpcTalkStartedPlugIn
{
    /// <summary>
    /// Is called when a player starts talking to an NPC.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="npc">The NPC.</param>
    ValueTask NpcTalkStartedAsync(Player player, NonPlayerCharacter npc);
}
