// <copyright file="WorldChatChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.ComponentModel;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlayerActions.Chat;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which sends a message to the world chat, which all players of all game servers can read.
/// The normal chat only reaches the players nearby.
/// </summary>
[Guid("9529E0ED-9C80-4C00-9328-40D2E147B118")]
[PlugIn]
[Display(Name = nameof(PlugInResources.WorldChatChatCommandPlugIn_Name), Description = nameof(PlugInResources.WorldChatChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(CommandKey, CharacterStatus.Normal)]
public class WorldChatChatCommandPlugIn : IChatCommandPlugIn
{
    private const string CommandKey = "/world";

    /// <inheritdoc />
    public string Key => CommandKey;

    /// <inheritdoc />
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        var message = command.Length > CommandKey.Length ? command[CommandKey.Length..].Trim() : string.Empty;
        if (string.IsNullOrWhiteSpace(message)
            || player.SelectedCharacter is not { } character
            || player.GameContext is not IGameServerContext context)
        {
            return;
        }

        var remainingChatBan = (player.Account?.ChatBanUntil ?? default) - DateTime.UtcNow;
        if (remainingChatBan > TimeSpan.Zero)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.ChatBanMinutesRemaining), (int)Math.Ceiling(remainingChatBan.TotalMinutes)).ConfigureAwait(false);
            return;
        }

        var eventArgs = new CancelEventArgs();
        player.GameContext.PlugInManager.GetPlugInPoint<IChatMessageReceivedPlugIn>()?.ChatMessageReceived(player, message, eventArgs);
        if (eventArgs.Cancel)
        {
            return;
        }

        await context.EventPublisher.WorldChatMessageAsync(character.Name, message).ConfigureAwait(false);
        await player.NotifyChatMessageSentAsync(message, ChatMessageType.World).ConfigureAwait(false);
    }
}
