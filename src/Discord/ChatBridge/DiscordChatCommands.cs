// <copyright file="DiscordChatCommands.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.ChatBridge;

using System.Globalization;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Discord.Properties;

/// <summary>
/// The slash commands of the chat bridge, and their answers.
/// </summary>
public sealed class DiscordChatCommands
{
    /// <summary>
    /// The name of the command which sends a message to the chat of the game.
    /// </summary>
    public const string SayCommandName = "say";

    /// <summary>
    /// The name of the option of the message of <see cref="SayCommandName"/>.
    /// </summary>
    public const string MessageOptionName = "message";

    /// <summary>
    /// The name of the command which manages the bindings of the chats of guilds.
    /// </summary>
    public const string GuildChatCommandName = "guildchat";

    /// <summary>
    /// The name of the sub command which binds the current channel.
    /// </summary>
    public const string BindCommandName = "bind";

    /// <summary>
    /// The name of the sub command which creates a channel on the Discord server of the game server.
    /// </summary>
    public const string CreateCommandName = "create";

    /// <summary>
    /// The name of the sub command which removes the binding of the current channel.
    /// </summary>
    public const string UnbindCommandName = "unbind";

    /// <summary>
    /// The name of the option of the scope.
    /// </summary>
    public const string ScopeOptionName = "scope";

    /// <summary>
    /// The value of the option of the scope for the alliance chat.
    /// </summary>
    public const string AllianceScope = "alliance";

    /// <summary>
    /// The value of the option of the scope for the guild chat.
    /// </summary>
    public const string GuildScope = "guild";

    private readonly CultureInfo _culture;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordChatCommands"/> class.
    /// </summary>
    /// <param name="culture">The culture of the answers.</param>
    public DiscordChatCommands(CultureInfo culture)
    {
        this._culture = culture;
    }

    /// <summary>
    /// Gets the definition of the command which sends a message to the chat of the game.
    /// </summary>
    public static DiscordCommandDefinition SayDefinition { get; } = new(SayCommandName, nameof(Resources.Command_Say_Description), MessageOptionName, nameof(Resources.Command_Say_MessageOption));

    /// <summary>
    /// Parses the value of the scope option.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The scope.</returns>
    public static GuildChatScope ParseScope(string? value) => value == AllianceScope ? GuildChatScope.Alliance : GuildChatScope.Guild;

    /// <summary>
    /// Gets the text of a resource.
    /// </summary>
    /// <param name="resourceKey">The resource key.</param>
    /// <returns>The text.</returns>
    public string Text(string resourceKey) => DiscordCommands.GetText(this._culture, resourceKey);

    /// <summary>
    /// Creates the answer to a message which couldn't be sent to the game.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="characterName">The name of the character as which the user wrote.</param>
    /// <returns>The answer.</returns>
    public DiscordEmbed CreatePostAnswer(DiscordChatPostResult result, string? characterName)
    {
        var key = result switch
        {
            DiscordChatPostResult.NotBridged => nameof(Resources.Chat_NotBridged),
            DiscordChatPostResult.NotLinked => nameof(Resources.Link_NotLinked),
            DiscordChatPostResult.NoCharacter => nameof(Resources.Chat_NoCharacter),
            DiscordChatPostResult.NotMember => nameof(Resources.Chat_NotMember),
            DiscordChatPostResult.ChatBanned => nameof(Resources.Chat_ChatBanned),
            DiscordChatPostResult.TooFast => nameof(Resources.Chat_TooFast),
            DiscordChatPostResult.Empty => nameof(Resources.Chat_Empty),
            _ => nameof(Resources.Chat_Sent),
        };

        return this.Answer(nameof(Resources.Chat_Title), key, DiscordMessageFormatter.Escape(characterName ?? "-"));
    }

    /// <summary>
    /// Creates the answer to a binding or the removal of a binding.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="successKey">The resource key of the answer on success.</param>
    /// <param name="target">The bound guild or alliance.</param>
    /// <param name="channelMention">The mention of the channel.</param>
    /// <returns>The answer.</returns>
    public DiscordEmbed CreateBindAnswer(DiscordChatBindResult result, string successKey, GuildChatTarget? target, string? channelMention)
    {
        var key = result switch
        {
            DiscordChatBindResult.Success => successKey,
            DiscordChatBindResult.ModeNotAllowed => nameof(Resources.Bind_ModeNotAllowed),
            DiscordChatBindResult.ServerNotAllowed => nameof(Resources.Bind_ServerNotAllowed),
            DiscordChatBindResult.NotLinked => nameof(Resources.Link_NotLinked),
            DiscordChatBindResult.NotGuildMaster => nameof(Resources.Bind_NotGuildMaster),
            DiscordChatBindResult.NotAllianceMaster => nameof(Resources.Bind_NotAllianceMaster),
            DiscordChatBindResult.ChannelInUse => nameof(Resources.Bind_ChannelInUse),
            _ => nameof(Resources.Bind_NotBound),
        };

        return this.Answer(nameof(Resources.Bind_Title), key, DiscordMessageFormatter.Escape(target?.GuildName ?? "-"), channelMention ?? "-");
    }

    /// <summary>
    /// Creates an answer with a text.
    /// </summary>
    /// <param name="titleKey">The resource key of the title.</param>
    /// <param name="resourceKey">The resource key of the text.</param>
    /// <param name="args">The format arguments.</param>
    /// <returns>The answer.</returns>
    public DiscordEmbed Answer(string titleKey, string resourceKey, params object[] args)
    {
        return new DiscordEmbed(
            DiscordCommands.GetText(this._culture, titleKey),
            DiscordCommands.GetText(this._culture, resourceKey, args),
            DiscordCommands.AnswerColor,
            DateTime.UtcNow,
            null);
    }
}
