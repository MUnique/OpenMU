// <copyright file="IChatCommandListViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views;

using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

/// <summary>
/// Interface of a view whose client can show the available chat commands to the player.
/// </summary>
public interface IChatCommandListViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the chat commands which are available to the player, so that the client
    /// can offer them without requiring the player to know or type any of them.
    /// </summary>
    /// <param name="commands">The available chat commands.</param>
    ValueTask ShowChatCommandListAsync(IReadOnlyCollection<ChatCommandInfo> commands);
}
