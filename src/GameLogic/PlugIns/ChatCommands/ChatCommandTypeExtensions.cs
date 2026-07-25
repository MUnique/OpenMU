// <copyright file="ChatCommandTypeExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Reflection;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Extension methods regarding chat command types.
/// </summary>
public static class ChatCommandTypeExtensions
{
    /// <summary>
    /// Gets the available chat commands of the player.
    /// Deactivated command plugins are not included, because they can't be
    /// executed either - the <see cref="PlugInManager"/> only resolves active
    /// plugins when a command is dispatched.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The available chat commands of the player.</returns>
    public static IEnumerable<ChatCommandHelpAttribute> GetAvailableChatCommands(this Player player)
    {
        var plugInManager = player.GameContext?.PlugInManager;
        if (plugInManager is null)
        {
            return Enumerable.Empty<ChatCommandHelpAttribute>();
        }

        return plugInManager
            .GetKnownPlugInsOf<IChatCommandPlugIn>()
            .Where(plugInManager.IsPlugInActive)
            .Select(CustomAttributeExtensions.GetCustomAttribute<ChatCommandHelpAttribute>)
            .Where(attribute => attribute is { })
            .Where(attribute => player.SelectedCharacter?.CharacterStatus >= attribute!.MinimumCharacterStatus)
            .Select(attribute => attribute!);
    }
}