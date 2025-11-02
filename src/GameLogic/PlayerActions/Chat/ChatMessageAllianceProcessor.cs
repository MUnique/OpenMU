// <copyright file="ChatMessageAllianceProcessor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Chat;

using System.ComponentModel;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// A chat message processor for alliance chat.
/// </summary>
public class ChatMessageAllianceProcessor : BannableChatMessageBaseProcessor
{
    private readonly IEventPublisher _eventPublisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatMessageAllianceProcessor"/> class.
    /// </summary>
    /// <param name="eventPublisher">The event publisher.</param>
    public ChatMessageAllianceProcessor(IEventPublisher eventPublisher)
    {
        this._eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
    }

    /// <inheritdoc />
    public override async ValueTask SubclassProcessMessageAsync(Player sender, (string Message, string PlayerName) content)
    {
        var eventArgs = new CancelEventArgs();
        sender.GameContext.PlugInManager.GetPlugInPoint<IChatMessageReceivedPlugIn>()?.ChatMessageReceived(sender, content.Message, eventArgs);
        if (eventArgs.Cancel)
        {
            return;
        }

        if (sender.GuildStatus is null)
        {
            return;
        }

        await this._eventPublisher.AllianceMessageAsync(sender.GuildStatus.GuildId, sender.SelectedCharacter!.Name, content.Message).ConfigureAwait(false);
    }
}