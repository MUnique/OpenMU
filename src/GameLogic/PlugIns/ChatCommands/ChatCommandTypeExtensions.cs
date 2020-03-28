// <copyright file="ChatCommandTypeExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Extension methods regarding chat command types.
    /// </summary>
    public static class ChatCommandTypeExtensions
    {
        /// <summary>
        /// Gets the available chat commands of the player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>The available chat commands of the player</returns>
        public static IEnumerable<ChatCommandHelpAttribute> GetAvailableChatCommands(this Player player)
        {
            return player.GameContext.PlugInManager
                .GetKnownPlugInsOf<IChatCommandPlugIn>()
                .Select(CustomAttributeExtensions.GetCustomAttribute<ChatCommandHelpAttribute>)
                .Where(attribute => attribute is { })
                .Where(attribute => player.SelectedCharacter.CharacterStatus >= attribute.MinimumCharacterStatus);
        }
    }
}