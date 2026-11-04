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
public sealed record DiscordEmbed(string Title, string Description, int Color, DateTime TimestampUtc, string? Footer)
{
    /// <summary>
    /// The maximum length of the title, which Discord accepts.
    /// </summary>
    public const int MaximumTitleLength = 256;

    /// <summary>
    /// The maximum length of the description, which Discord accepts.
    /// </summary>
    public const int MaximumDescriptionLength = 4096;

    /// <summary>
    /// The maximum length of the footer, which Discord accepts.
    /// </summary>
    public const int MaximumFooterLength = 2048;

    /// <summary>
    /// The maximum length of the texts of all embeds of one message, which Discord accepts.
    /// </summary>
    public const int MaximumTotalLength = 6000;

    /// <summary>
    /// Gets the length of all texts of the embed, which counts against <see cref="MaximumTotalLength"/>.
    /// </summary>
    public int TextLength => this.Title.Length + this.Description.Length + (this.Footer?.Length ?? 0);

    /// <summary>
    /// Gets the embed with texts which are cut to the lengths which Discord accepts.
    /// </summary>
    /// <returns>The embed, or a copy with shortened texts.</returns>
    public DiscordEmbed Limit()
    {
        if (this.Title.Length <= MaximumTitleLength
            && this.Description.Length <= MaximumDescriptionLength
            && (this.Footer?.Length ?? 0) <= MaximumFooterLength)
        {
            return this;
        }

        return this with
        {
            Title = Cut(this.Title, MaximumTitleLength),
            Description = Cut(this.Description, MaximumDescriptionLength),
            Footer = this.Footer is { } footer ? Cut(footer, MaximumFooterLength) : null,
        };
    }

    private static string Cut(string text, int maximumLength) => text.Length <= maximumLength ? text : text[..(maximumLength - 1)] + "…";
}
