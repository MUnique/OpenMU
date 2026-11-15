// <copyright file="PostChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles post commands by sending a blue system message to all players.
/// </summary>
[Guid("ED2523C1-F66D-4B53-814E-D2FC0C1F46C0")]
[PlugIn]
[Display(Name = nameof(PlugInResources.PostChatCommandPlugIn_Name), Description = nameof(PlugInResources.PostChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(CommandKey, CharacterStatus.Normal)]
public class PostChatCommandPlugIn : IChatCommandPlugIn
{
    /// <summary>
    /// The sender name under which a post appears in the chat.
    /// </summary>
    internal const string PostSender = "[POST]";

    private const string CommandKey = "/post";

    /// <inheritdoc />
    public string Key => CommandKey;

    /// <inheritdoc />
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        var regex = new Regex(Regex.Escape(CommandKey));
        var message = regex.Replace(command, string.Empty, 1)?.Trim();

        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        await player.GameContext.SendGlobalChatMessageAsync(PostSender, FormatPost(player.SelectedCharacter?.Name, message), ChatMessageType.Gens).ConfigureAwait(false);
        if (player.GameContext.PlugInManager.GetPlugInPoint<IPostMessageSentPlugIn>() is { } plugInPoint)
        {
            await plugInPoint.PostMessageSentAsync(player, message).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Formats the text of a post as it appears in the chat: the author's name, then the message.
    /// </summary>
    /// <param name="characterName">The name of the posting character.</param>
    /// <param name="message">The posted message.</param>
    /// <returns>The text shown in the chat.</returns>
    internal static string FormatPost(string? characterName, string message) => $"{characterName}: {message}";
}