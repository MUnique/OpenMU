// <copyright file="DiscordChatCommandArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;

using MUnique.OpenMU.GameLogic.AccountLinking;

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
    /// The action which turns direct messages in Discord on or off.
    /// </summary>
    public const string NotifyAction = "notify";

    /// <summary>
    /// Gets or sets the action. Without action, the state of the link is shown.
    /// </summary>
    [Argument("action", false)]
    [ValidValues(LinkAction, UnlinkAction, NotifyAction)]
    public string? Action { get; set; }

    /// <summary>
    /// Gets or sets the type of the direct messages, which is turned on or off by the <see cref="NotifyAction"/>.
    /// </summary>
    [Argument("type", false)]
    [ValidValues(AccountNotificationKeywords.LoginAttempt, AccountNotificationKeywords.LetterReceived, AccountNotificationKeywords.FriendOnline)]
    public string? Type { get; set; }
}
