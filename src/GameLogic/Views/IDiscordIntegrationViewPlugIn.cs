// <copyright file="IDiscordIntegrationViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views;

using MUnique.OpenMU.GameLogic.Discord;

/// <summary>
/// The view of the Discord integration of the game client.
/// </summary>
public interface IDiscordIntegrationViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows how the game server is connected to Discord. The client asked for it,
    /// so it supports the Discord integration from then on.
    /// </summary>
    /// <param name="info">The info.</param>
    ValueTask ShowDiscordIntegrationInfoAsync(DiscordIntegrationInfo info);

    /// <summary>
    /// Shows the one-time code to link the account to a Discord user.
    /// </summary>
    /// <param name="result">The result of the request.</param>
    /// <param name="code">The code; <c>null</c>, unless it was <see cref="DiscordLinkCodeResult.Created"/>.</param>
    /// <param name="validity">The time the code is valid.</param>
    ValueTask ShowDiscordLinkCodeAsync(DiscordLinkCodeResult result, string? code, TimeSpan validity);

    /// <summary>
    /// Shows a chat message which was written outside of the game, if the client supports it.
    /// </summary>
    /// <param name="scope">The chat which the message belongs to.</param>
    /// <param name="senderName">The name of the sender, without prefix.</param>
    /// <param name="message">The message, without the prefix of its chat.</param>
    /// <returns><c>true</c>, if it was shown; <c>false</c>, if the client didn't announce the support for it.</returns>
    ValueTask<bool> TryShowExternalChatMessageAsync(ExternalChatScope scope, string senderName, string message);
}
