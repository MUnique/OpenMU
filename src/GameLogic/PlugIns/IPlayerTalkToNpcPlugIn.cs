// <copyright file="IPlayerTalkToNpcPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.ComponentModel;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Plugin interface which is called when a player talks to a npc.
/// </summary>
[Guid("95E5446D-10D9-4399-BF98-D32808D2BBD1")]
[PlugInPoint("Player talks to NPC", "Plugins which will be executed when a player talks to an npc.")]
public interface IPlayerTalkToNpcPlugIn
{
    /// <summary>
    /// Is called when the player talks to a NPC.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="npc">The NPC.</param>
    /// <param name="eventArgs">The event args.</param>
    ValueTask PlayerTalksToNpcAsync(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs);
}