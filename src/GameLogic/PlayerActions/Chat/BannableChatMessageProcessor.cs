// <copyright file="BannableChatMessageBaseProcessor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Chat;

using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// A chat message processor for normal chat.
/// </summary>
public abstract class BannableChatMessageBaseProcessor : IChatMessageProcessor
{
    /// <inheritdoc />
    public async ValueTask ProcessMessageAsync(Player sender, (string Message, string PlayerName) content)
    {
        TimeSpan remainingChatBan = this.RemainingChatBanTimeSpan(sender);
        if (IsSenderBanned(remainingChatBan))
        {
            if(remainingChatBan.TotalMinutes >= 1)
            {
                await this.SendMessageToPlayerAsync(sender, $"Chat Ban: {(int)Math.Ceiling(remainingChatBan.TotalMinutes)} minute(s) remaining.", MessageType.BlueNormal).ConfigureAwait(false);
            }
            else
            {
                await this.SendMessageToPlayerAsync(sender, $"Chat Ban: {(int)Math.Ceiling(remainingChatBan.TotalSeconds)} second(s) remaining.", MessageType.BlueNormal).ConfigureAwait(false);
            }
            return;
        }

        await this.SubclassProcessMessageAsync(sender, content);
    }

    public abstract ValueTask SubclassProcessMessageAsync(Player sender, (string Message, string PlayerName) content);

    private TimeSpan RemainingChatBanTimeSpan(Player sender)
    {
        DateTime chatBanUntil = sender.Account?.ChatBanUntil ?? default;
        DateTime currentDateTime = DateTime.UtcNow;
        return (chatBanUntil - currentDateTime);
    }

    private bool IsSenderBanned(TimeSpan remainingChatBan)
    {
        return (remainingChatBan > TimeSpan.Zero);
    }

    private async ValueTask SendMessageToPlayerAsync(Player player, string message, MessageType type)
    {
        await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, type)).ConfigureAwait(false);
    }
}