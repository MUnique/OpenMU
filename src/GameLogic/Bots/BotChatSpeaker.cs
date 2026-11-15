// <copyright file="BotChatSpeaker.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Globalization;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
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
    /// <param name="text">The text.</param>
    /// <param name="values">The placeholder values.</param>
    /// <param name="language">The language to say it in, for every receiver; <c>null</c> for each receiver's own.</param>
    /// <param name="gesture">The gesture the bot makes with it.</param>
    /// <param name="toward">The player the bot talks to, to turn to for the gesture.</param>
    internal static async ValueTask SayAsync(Player bot, LocalizedString text, IReadOnlyDictionary<string, Func<CultureInfo, string>> values, CultureInfo? language = null, BotChatGesture gesture = BotChatGesture.None, Player? toward = null)
    {
        if (bot.SelectedCharacter?.Name is not { } name)
        {
            return;
        }

        await bot.ForEachObservingAsync<Player>(
            receiver => SendAsync(receiver, BotChatComposer.Render(text, language ?? receiver.Culture, values), name, ChatMessageType.Normal),
            false).ConfigureAwait(false);
        await GestureAsync(bot, gesture, toward).ConfigureAwait(false);
    }

    /// <summary>
    /// Whispers the line to a player.
    /// </summary>
    /// <param name="bot">The whispering bot.</param>
    /// <param name="receiver">The receiving player.</param>
    /// <param name="text">The text.</param>
    /// <param name="values">The placeholder values.</param>
    /// <param name="language">The language to whisper in; <c>null</c> for the receiver's own.</param>
    /// <param name="gesture">The gesture the bot makes with it, seen when the receiver stands near.</param>
    internal static async ValueTask WhisperAsync(Player bot, Player receiver, LocalizedString text, IReadOnlyDictionary<string, Func<CultureInfo, string>> values, CultureInfo? language = null, BotChatGesture gesture = BotChatGesture.None)
    {
        if (bot.SelectedCharacter?.Name is not { } name)
        {
            return;
        }

        await SendAsync(receiver, BotChatComposer.Render(text, language ?? receiver.Culture, values), name, ChatMessageType.Whisper).ConfigureAwait(false);
        await GestureAsync(bot, gesture, receiver).ConfigureAwait(false);
    }

    /// <summary>
    /// Says the line in the party chat, to the players of the bot's party - only if the bot is still in
    /// the party it meant to talk to: the line is said after a "typing time", meanwhile the bot may have
    /// left it and joined another.
    /// </summary>
    /// <param name="bot">The speaking bot.</param>
    /// <param name="party">The party the bot meant to talk to.</param>
    /// <param name="text">The text.</param>
    /// <param name="values">The placeholder values.</param>
    /// <param name="language">The language to say it in, for every receiver; <c>null</c> for each receiver's own.</param>
    /// <param name="gesture">The gesture the bot makes with it.</param>
    /// <param name="toward">The player the bot talks to, to turn to for the gesture.</param>
    internal static async ValueTask PartySayAsync(Player bot, Party party, LocalizedString text, IReadOnlyDictionary<string, Func<CultureInfo, string>> values, CultureInfo? language = null, BotChatGesture gesture = BotChatGesture.None, Player? toward = null)
    {
        if (bot.SelectedCharacter?.Name is not { } name || !ReferenceEquals(bot.Party, party))
        {
            return;
        }

        foreach (var receiver in party.PartyList.OfType<Player>())
        {
            await SendAsync(receiver, BotChatComposer.Render(text, language ?? receiver.Culture, values), name, ChatMessageType.Party).ConfigureAwait(false);
        }

        await GestureAsync(bot, gesture, toward).ConfigureAwait(false);
    }

    /// <summary>
    /// Posts the line to all players of the game server, in the format of the <c>/post</c> command.
    /// </summary>
    /// <param name="bot">The posting bot.</param>
    /// <param name="text">The text.</param>
    /// <param name="values">The placeholder values.</param>
    internal static async ValueTask PostAsync(Player bot, LocalizedString text, IReadOnlyDictionary<string, Func<CultureInfo, string>> values)
    {
        if (bot.SelectedCharacter?.Name is not { } name)
        {
            return;
        }

        await bot.GameContext.ForEachPlayerAsync(receiver =>
            SendAsync(
                receiver,
                PostChatCommandPlugIn.FormatPost(name, BotChatComposer.Render(text, receiver.Culture, values)),
                PostChatCommandPlugIn.PostSender,
                ChatMessageType.Gens).AsTask())
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Makes the bot perform a gesture for the players who see it, turned towards the player it talks
    /// to - like a player typing /hello.
    /// </summary>
    /// <param name="bot">The bot.</param>
    /// <param name="gesture">The gesture.</param>
    /// <param name="toward">The player the bot talks to, if it's near.</param>
    internal static ValueTask GestureAsync(Player bot, BotChatGesture gesture, Player? toward)
    {
        if (gesture == BotChatGesture.None || bot.CurrentMap is not { } map)
        {
            return ValueTask.CompletedTask;
        }

        if (toward is not null && ReferenceEquals(toward.CurrentMap, map))
        {
            bot.Rotation = bot.GetDirectionTo(toward);
        }

        var rotation = bot.Rotation;
        return bot.ForEachWorldObserverAsync<IShowAnimationPlugIn>(p => p.ShowAnimationAsync(bot, (byte)gesture, null, rotation), false);
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
