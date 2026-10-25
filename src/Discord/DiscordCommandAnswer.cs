// <copyright file="DiscordCommandAnswer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

/// <summary>
/// The answer of a personal command, which is only shown to the user who used it.
/// </summary>
/// <param name="Embed">The answer.</param>
/// <param name="RoleChanges">The changes of the links, after which the bot updates the roles.</param>
public sealed record DiscordCommandAnswer(DiscordEmbed Embed, IReadOnlyList<DiscordRoleChange> RoleChanges);
