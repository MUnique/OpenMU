// <copyright file="DiscordBotToken.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.Text;

/// <summary>
/// Reads the token of a Discord bot.
/// </summary>
public static class DiscordBotToken
{
    /// <summary>
    /// Gets the identifier of the application of the bot, which is the first part of its token.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <returns>The identifier of the application, or <c>null</c>, if the token has a different format.</returns>
    /// <remarks>
    /// The first part of a token is the identifier of the user of the bot, encoded as base64. For bots,
    /// it's the identifier of their application, too.
    /// </remarks>
    public static ulong? GetApplicationId(string? token)
    {
        if (token?.Split('.') is not [{ Length: > 0 } encodedId, _, _])
        {
            return null;
        }

        var padded = encodedId.Replace('-', '+').Replace('_', '/');
        padded = padded.PadRight(padded.Length + ((4 - (padded.Length % 4)) % 4), '=');
        var bytes = new byte[padded.Length];
        if (!Convert.TryFromBase64String(padded, bytes, out var length)
            || !ulong.TryParse(Encoding.ASCII.GetString(bytes, 0, length), out var id))
        {
            return null;
        }

        return id;
    }

    /// <summary>
    /// Creates the link with which the bot is invited to a Discord server, with the permissions which it needs.
    /// </summary>
    /// <param name="applicationId">The identifier of the application of the bot.</param>
    /// <returns>The link.</returns>
    public static string CreateInviteUrl(ulong applicationId)
    {
        // View Channels, Send Messages, Embed Links, Read Message History, Add Reactions,
        // Manage Channels, Manage Roles, Manage Events and Manage Messages, which the bot gives the guild masters in their channels.
        const ulong permissions = (1UL << 10) | (1UL << 11) | (1UL << 14) | (1UL << 16) | (1UL << 6) | (1UL << 4) | (1UL << 28) | (1UL << 33) | (1UL << 13);
        return $"https://discord.com/oauth2/authorize?client_id={applicationId}&scope=bot+applications.commands&permissions={permissions}";
    }
}
