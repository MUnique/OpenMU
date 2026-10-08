// <copyright file="DiscordChannelAccess.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.Provisioning;

/// <summary>
/// The access to a channel, which the provisioning sets.
/// </summary>
[Flags]
public enum DiscordChannelAccess
{
    /// <summary>
    /// No access.
    /// </summary>
    None = 0,

    /// <summary>
    /// The channel can be seen and its messages can be read.
    /// </summary>
    View = 1,

    /// <summary>
    /// Messages can be sent into the channel.
    /// </summary>
    Send = 2,
}
