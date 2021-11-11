// <copyright file="GuildDisconnectChatCommandArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;

/// <summary>
/// Arguments used by GuildDisconnectChatCommandPlugIn.
/// </summary>
public class GuildDisconnectChatCommandArgs : ArgumentsBase
{
    /// <summary>
    /// Gets or sets the guild name.
    /// </summary>
    [Argument("guild")]
    public string? GuildName { get; set; }
}