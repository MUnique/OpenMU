// <copyright file="BotChatSpeaker.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Globalization;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Delivers what a bot says to the human players, each in their own language.
/// </summary>
/// <remarks>
/// A player's chat goes through the <c>ChatMessageAction</c> as one string for all receivers. A
/// bot's line is a <see cref="LocalizedString"/> instead, so it is rendered and sent per receiver -
/// like the <c>ServerAnnouncementsPlugIn</c> does. The receivers see exactly what a player's
/// message would look like. Off-level players and other bots are skipped: they have no connection
/// to show it on.
/// </remarks>
internal static class BotChatSpeaker
{
    /// <summary>
    /// Says the line in the normal chat, to the players who see the bot.
    /// </summary>
    /// <param name="bot">The speaking bot.</param>
    /// <param name="line">The line.</param>
    /// <param name="values">The placeholder values.</param>
    internal static ValueTask SayAsync(Player bot, BotChatLine line, IReadOnlyDictionary<string, Func<CultureInfo, string>> values)
    {
        if (bot.SelectedCharacter?.Name is not { } name)
        {
            return ValueTask.CompletedTask;
        }

        return bot.ForEachObservingAsync<Player>(
            receiver => SendAsync(receiver, BotChatComposer.Render(line, receiver.Culture, values), name, ChatMessageType.Normal),
            false);
    }

    /// <summary>
    /// Whispers the line to a player.
    /// </summary>
    /// <param name="bot">The whispering bot.</param>
    /// <param name="receiver">The receiving player.</param>
    /// <param name="line">The line.</param>
    /// <param name="values">The placeholder values.</param>
    internal static ValueTask WhisperAsync(Player bot, Player receiver, BotChatLine line, IReadOnlyDictionary<string, Func<CultureInfo, string>> values)
    {
        if (bot.SelectedCharacter?.Name is not { } name)
        {
            return ValueTask.CompletedTask;
        }

        return SendAsync(receiver, BotChatComposer.Render(line, receiver.Culture, values), name, ChatMessageType.Whisper);
    }

    /// <summary>
    /// Says the line in the party chat, to the players of the bot's party.
    /// </summary>
    /// <param name="bot">The speaking bot.</param>
    /// <param name="line">The line.</param>
    /// <param name="values">The placeholder values.</param>
    internal static async ValueTask PartySayAsync(Player bot, BotChatLine line, IReadOnlyDictionary<string, Func<CultureInfo, string>> values)
    {
        if (bot.SelectedCharacter?.Name is not { } name || bot.Party is not { } party)
        {
            return;
        }

        foreach (var receiver in party.PartyList.OfType<Player>())
        {
            await SendAsync(receiver, BotChatComposer.Render(line, receiver.Culture, values), name, ChatMessageType.Party).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Posts the line to all players of the game server, in the format of the <c>/post</c> command.
    /// </summary>
    /// <param name="bot">The posting bot.</param>
    /// <param name="line">The line.</param>
    /// <param name="values">The placeholder values.</param>
    internal static async ValueTask PostAsync(Player bot, BotChatLine line, IReadOnlyDictionary<string, Func<CultureInfo, string>> values)
    {
        if (bot.SelectedCharacter?.Name is not { } name)
        {
            return;
        }

        await bot.GameContext.ForEachPlayerAsync(receiver =>
            SendAsync(
                receiver,
                PostChatCommandPlugIn.FormatPost(name, BotChatComposer.Render(line, receiver.Culture, values)),
                PostChatCommandPlugIn.PostSender,
                ChatMessageType.Gens).AsTask())
            .ConfigureAwait(false);
    }

    private static ValueTask SendAsync(Player receiver, string text, string sender, ChatMessageType type)
    {
        if (receiver is OfflinePlayer || string.IsNullOrWhiteSpace(text))
        {
            return ValueTask.CompletedTask;
        }

        return receiver.InvokeViewPlugInAsync<IChatViewPlugIn>(p => p.ChatMessageAsync(text, sender, type));
    }
}
