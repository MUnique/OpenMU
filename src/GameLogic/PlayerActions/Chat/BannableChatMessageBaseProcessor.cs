// <copyright file="BannableChatMessageBaseProcessor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Chat;

/// <summary>
/// A chat message processor for normal chat.
/// </summary>
public abstract class BannableChatMessageBaseProcessor : IChatMessageProcessor
{
    /// <inheritdoc />
    public async ValueTask ProcessMessageAsync(Player sender, (string Message, string PlayerName) content)
    {
        TimeSpan remainingChatBan = this.RemainingChatBanTimeSpan(sender);
        if (this.IsSenderBanned(remainingChatBan))
        {
            if (remainingChatBan.TotalMinutes >= 1)
            {
                await sender.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.ChatBanMinutesRemaining), (int)Math.Ceiling(remainingChatBan.TotalMinutes)).ConfigureAwait(false);
            }
            else
            {
                await sender.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.ChatBanSecondsRemaining), (int)Math.Ceiling(remainingChatBan.TotalSeconds)).ConfigureAwait(false);
            }

            return;
        }

        await this.SubclassProcessMessageAsync(sender, content).ConfigureAwait(false);
    }

    /// <summary>
    /// A method to be overriden for processing a specific chat message.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="content">The content.</param>
    /// <returns>A value task with the result.</returns>
    public abstract ValueTask SubclassProcessMessageAsync(Player sender, (string Message, string PlayerName) content);

    private TimeSpan RemainingChatBanTimeSpan(Player sender)
    {
        DateTime chatBanUntil = sender.Account?.ChatBanUntil ?? default;
        DateTime currentDateTime = DateTime.UtcNow;
        return chatBanUntil - currentDateTime;
    }

    private bool IsSenderBanned(TimeSpan remainingChatBan)
    {
        return remainingChatBan > TimeSpan.Zero;
    }
}