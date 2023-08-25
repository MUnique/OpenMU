// <copyright file="NoticeChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles post commands by sending a golden notice message to all players.
/// </summary>
[Guid("2BFC9464-4B76-4D76-8CE1-69B712B65E6C")]
[PlugIn("Notice chat command", "Handles the chat command '/goldnotice message'. Sends a global golden notice message to all players of the game.")]
public class NoticeChatCommandPlugIn : IChatCommandPlugIn
{
    private const string CommandKey = "/goldnotice";

    /// <inheritdoc />
    public string Key => CommandKey;

    /// <inheritdoc />
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        var regex = new Regex(Regex.Escape(CommandKey));
        var message = regex.Replace(command, string.Empty, 1)?.Trim();

        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        await player.GameContext.SendGlobalMessageAsync(message, Interfaces.MessageType.GoldenCenter).ConfigureAwait(false);
    }
}