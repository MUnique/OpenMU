// <copyright file="DiscordBotProblem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

/// <summary>
/// A problem which stopped the <see cref="DiscordBot"/>, and which the administrator has to solve.
/// </summary>
public enum DiscordBotProblem
{
    /// <summary>
    /// There is no problem.
    /// </summary>
    None,

    /// <summary>
    /// Discord rejected the token, or it isn't a token at all.
    /// </summary>
    TokenRejected,

    /// <summary>
    /// The bot requests a privileged gateway intent, which isn't enabled in the Discord developer portal.
    /// </summary>
    /// <remarks>
    /// It's requested for <see cref="ChatBridge.DiscordChatBridgeSettings.ReadMessages"/>.
    /// </remarks>
    IntentNotEnabled,

    /// <summary>
    /// The bot couldn't be started for another reason, see the log.
    /// </summary>
    StartFailed,
}
