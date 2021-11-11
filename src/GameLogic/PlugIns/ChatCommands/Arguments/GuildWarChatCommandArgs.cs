// <copyright file="GuildWarChatCommandArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;

/// <summary>
/// Arguments used by <see cref="GuildWarChatCommandPlugIn"/> and <see cref="GuildBattleSoccerChatCommandPlugIn"/>.
/// </summary>
public class GuildWarChatCommandArgs : ArgumentsBase
{
    /// <summary>
    /// Gets or sets the guild name.
    /// </summary>
    [Argument("guildname")]
    public string GuildName { get; set; } = string.Empty;
}