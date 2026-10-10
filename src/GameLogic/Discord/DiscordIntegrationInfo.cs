// <copyright file="DiscordIntegrationInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Discord;

/// <summary>
/// How the game server is connected to Discord, from the point of view of one player.
/// </summary>
/// <param name="LinkedUserName">The name of the Discord user which the account is linked to; <c>null</c>, if it isn't linked.</param>
/// <param name="IsGuildChatBridged">If set to <c>true</c>, the chat of the guild of the character is mirrored to Discord.</param>
/// <param name="IsAllianceChatBridged">If set to <c>true</c>, the chat of the alliance of the character is mirrored to Discord.</param>
/// <param name="IsWorldChatBridged">If set to <c>true</c>, the world chat is mirrored to Discord.</param>
/// <param name="Configuration">The configured values; <c>null</c>, if the Discord integration is deactivated.</param>
public record DiscordIntegrationInfo(
    string? LinkedUserName,
    bool IsGuildChatBridged,
    bool IsAllianceChatBridged,
    bool IsWorldChatBridged,
    DiscordIntegrationConfiguration? Configuration)
{
    /// <summary>
    /// Gets the info of a server without Discord integration.
    /// </summary>
    public static DiscordIntegrationInfo None { get; } = new(null, false, false, false, null);
}
