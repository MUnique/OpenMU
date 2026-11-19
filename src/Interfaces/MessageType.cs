// <copyright file="MessageType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// Types of messages.
/// </summary>
public enum MessageType
{
    /// <summary>
    /// The message is shown as centered golden message in the client.
    /// </summary>
    GoldenCenter = 0,

    /// <summary>
    /// The message is shown as blue entry.
    /// </summary>
    BlueNormal = 1,

    /// <summary>
    /// The message is a guild notice (green center).
    /// </summary>
    GuildNotice = 2,

    /// <summary>
    /// The message scrolls in bold across the top of the screen (slide notice).
    /// </summary>
    SlideNotice = 3,
}
