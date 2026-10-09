// <copyright file="DiscordChatBindResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.ChatBridge;

/// <summary>
/// The result of binding a chat of a guild or alliance to a Discord channel, or of removing the binding.
/// </summary>
public enum DiscordChatBindResult
{
    /// <summary>
    /// The binding was created, changed or removed.
    /// </summary>
    Success,

    /// <summary>
    /// The binding mode of the settings doesn't allow it.
    /// </summary>
    ModeNotAllowed,

    /// <summary>
    /// The Discord server isn't allowed by the settings.
    /// </summary>
    ServerNotAllowed,

    /// <summary>
    /// The Discord user isn't linked to a game account.
    /// </summary>
    NotLinked,

    /// <summary>
    /// The character of the Discord user isn't the guild master.
    /// </summary>
    NotGuildMaster,

    /// <summary>
    /// The guild of the Discord user isn't the master of an alliance.
    /// </summary>
    NotAllianceMaster,

    /// <summary>
    /// The channel is bound to the chat of another guild or alliance.
    /// </summary>
    ChannelInUse,

    /// <summary>
    /// The channel isn't bound.
    /// </summary>
    NotBound,
}
