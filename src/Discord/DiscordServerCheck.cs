// <copyright file="DiscordServerCheck.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

/// <summary>
/// The result of checking the Discord server whose channels receive the notifications and the status,
/// see <see cref="DiscordBot.CheckDiscordServerAsync"/>.
/// </summary>
/// <param name="ServerName">The name of the Discord server.</param>
/// <param name="MissingPermissions">The permissions which the bot needs, but doesn't have on the Discord server.</param>
/// <param name="LinkedRole">The state of the role which linked players get.</param>
/// <param name="CategoriesWithoutChannel">The categories of notifications which have no channel of the bot.</param>
/// <param name="HasStatusChannel">If the bot has a channel for the status message.</param>
public sealed record DiscordServerCheck(
    string ServerName,
    IReadOnlyList<string> MissingPermissions,
    DiscordRoleState LinkedRole,
    IReadOnlyList<DiscordChannelCategory> CategoriesWithoutChannel,
    bool HasStatusChannel);
