// <copyright file="ICustomNpcTalkHandlerPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A plugin interface for chat commands.
    /// </summary>
    [Guid("91C7F1C5-1DBD-4407-8B2F-DC20CAA6B0EF")]
    [PlugInPoint("Custom Chat Handler", "Plugins which will be handle custom npc talk like reset.")]
    public interface ICustomNpcTalkHandlerPlugin : IStrategyPlugIn<string>
    {
        /// <summary>
        /// Handles the player-to-npc talk.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="npc">The npc.</param>
        /// <param name="eventArgs">The event args.</param>
        void HandleNpcTalk(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs);
    }
}
