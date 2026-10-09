// <copyright file="DiscordChatCommandArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;

/// <summary>
/// Arguments for the discord chat command.
/// </summary>
public class DiscordChatCommandArgs : ArgumentsBase
{
    /// <summary>
    /// The action which creates a code for linking.
    /// </summary>
    public const string LinkAction = "link";

    /// <summary>
    /// The action which removes the link.
    /// </summary>
    public const string UnlinkAction = "unlink";

    /// <summary>
    /// Gets or sets the action. Without action, the state of the link is shown.
    /// </summary>
    [Argument("action", false)]
    [ValidValues(LinkAction, UnlinkAction)]
    public string? Action { get; set; }
}
