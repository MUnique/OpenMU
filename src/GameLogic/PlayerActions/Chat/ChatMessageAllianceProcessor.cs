// <copyright file="ChatMessageAllianceProcessor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Chat;

using System.ComponentModel;
using MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// A chat message processor for alliance chat.
/// </summary>
public class ChatMessageAllianceProcessor : BannableChatMessageBaseProcessor
{
    /// <inheritdoc />
    public override async ValueTask SubclassProcessMessageAsync(Player sender, (string Message, string PlayerName) content)
    {
        var eventArgs = new CancelEventArgs();
        sender.GameContext.PlugInManager.GetPlugInPoint<IChatMessageReceivedPlugIn>()?.ChatMessageReceived(sender, content.Message, eventArgs);
        if (eventArgs.Cancel)
        {
            return;
        }

        if (!(sender.GuildStatus != null && (sender.GameContext as IGameServerContext)?.EventPublisher is { } publisher))
        {
            return;
        }

        // TODO: Use DI to get the IEventPublisher
        await publisher.AllianceMessageAsync(sender.GuildStatus.GuildId, sender.SelectedCharacter!.Name, content.Message).ConfigureAwait(false);
    }
}