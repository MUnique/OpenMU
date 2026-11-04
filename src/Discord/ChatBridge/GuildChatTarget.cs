// <copyright file="GuildChatTarget.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.ChatBridge;

/// <summary>
/// The guild or alliance whose chat a guild master can bind.
/// </summary>
/// <param name="GuildId">The identifier of the guild; for an alliance, of its master guild.</param>
/// <param name="GuildName">The name of the guild.</param>
/// <param name="CharacterName">The name of the character of the guild master.</param>
public sealed record GuildChatTarget(Guid GuildId, string GuildName, string CharacterName);
