// <copyright file="DiscordLinkCodeResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Discord;

/// <summary>
/// The result of a request of a code to link the account to a Discord user.
/// </summary>
public enum DiscordLinkCodeResult
{
    /// <summary>
    /// The code was created.
    /// </summary>
    Created,

    /// <summary>
    /// Linking isn't available, e.g. because the Discord integration is deactivated.
    /// </summary>
    NotAvailable,

    /// <summary>
    /// A code was requested a moment ago; see <see cref="PlayerActions.Discord.DiscordIntegrationAction.LinkCodeCooldown"/>.
    /// </summary>
    TooSoon,
}
