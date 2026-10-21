// <copyright file="DiscordGameMasterAnswer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

/// <summary>
/// The answer of a game master command.
/// </summary>
/// <param name="Embed">The answer for the game master.</param>
/// <param name="StaffAlert">The alert for the staff about the use of the command; or <c>null</c>, if the command wasn't executed.</param>
/// <param name="Announcement">The announcement for the channel of the notices, which <c>/announce</c> sent to the game; or <c>null</c>.</param>
public sealed record DiscordGameMasterAnswer(DiscordEmbed Embed, DiscordEmbed? StaffAlert, DiscordEmbed? Announcement = null);
