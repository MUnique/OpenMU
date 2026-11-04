// <copyright file="DiscordTestOutcome.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

/// <summary>
/// The outcome of a test message, see <see cref="DiscordIntegration.SendTestMessageAsync"/>.
/// </summary>
public enum DiscordTestOutcome
{
    /// <summary>
    /// The bot posted the message into its channel of the category.
    /// </summary>
    SentByBot,

    /// <summary>
    /// The webhook of the category posted the message.
    /// </summary>
    SentByWebhook,

    /// <summary>
    /// The category has neither a channel of the bot nor a webhook, so it isn't posted.
    /// </summary>
    NoChannel,

    /// <summary>
    /// The bot doesn't see the channel of the category, e.g. because it was deleted.
    /// </summary>
    ChannelUnavailable,

    /// <summary>
    /// The bot isn't allowed to post into the channel.
    /// </summary>
    MissingPermissions,

    /// <summary>
    /// Discord doesn't know the webhook: the URL is wrong, or the webhook was deleted.
    /// </summary>
    WebhookInvalid,

    /// <summary>
    /// The message couldn't be sent for another reason, see <see cref="DiscordTestResult.Detail"/>.
    /// </summary>
    Failed,
}
