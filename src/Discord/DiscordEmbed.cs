// <copyright file="DiscordEmbed.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

/// <summary>
/// A message which is posted as embed to a Discord channel.
/// </summary>
/// <param name="Title">The title.</param>
/// <param name="Description">The description, which may contain Discord markdown.</param>
/// <param name="Color">The color of the bar on the left side, as RGB value.</param>
/// <param name="TimestampUtc">The timestamp, which is shown below the message.</param>
/// <param name="Footer">The footer text.</param>
public sealed record DiscordEmbed(string Title, string Description, int Color, DateTime TimestampUtc, string? Footer);
