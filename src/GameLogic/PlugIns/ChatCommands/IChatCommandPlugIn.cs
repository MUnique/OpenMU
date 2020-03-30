// <copyright file="IChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A plugin interface for chat commands.
    /// </summary>
    [Guid("6CABB847-AE91-4F2B-9FD2-296990950EA3")]
    [PlugInPoint("Chat commands", "Plugins which will be executed when a chat message arrives with a slash prefix.")]
    public interface IChatCommandPlugIn : IStrategyPlugIn<string>
    {
        /// <summary>
        /// Gets min Status Requirement to run a command
        /// </summary>
        public CharacterStatus MinCharacterStatusRequirement { get; }

        /// <summary>
        /// Handles the chat command.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="command">The command.</param>
        void HandleCommand(Player player, string command);
    }
}
