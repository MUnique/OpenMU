// <copyright file="DiscordChatBindingInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.ChatBridge;

using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// A chat of a guild or an alliance which is bound to a Discord channel.
/// </summary>
/// <param name="ChannelId">The identifier of the channel.</param>
/// <param name="GuildName">The name of the guild, or <c>null</c>, if it doesn't exist anymore.</param>
/// <param name="Scope">The scope of the chat.</param>
/// <param name="ChannelName">The name of the channel, or <c>null</c>, if the bot doesn't see it.</param>
/// <param name="ServerName">The name of the Discord server of the channel, or <c>null</c>, if the bot doesn't see it.</param>
/// <param name="IsHosted">If the channel is on the Discord server of the game server.</param>
/// <param name="BoundBy">The character which bound the chat.</param>
/// <param name="BoundAt">When the chat was bound.</param>
public sealed record DiscordChatBindingInfo(ulong ChannelId, string? GuildName, GuildChatScope Scope, string? ChannelName, string? ServerName, bool IsHosted, string? BoundBy, DateTime BoundAt);
