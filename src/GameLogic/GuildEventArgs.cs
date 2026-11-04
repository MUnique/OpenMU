// <copyright file="GuildEventArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Event args for a deleted guild.
/// </summary>
public class GuildEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GuildEventArgs"/> class.
    /// </summary>
    /// <param name="guildId">The guild identifier of the deleted guild.</param>
    public GuildEventArgs(uint guildId)
    {
        this.GuildId = guildId;
    }

    /// <summary>
    /// Gets the guild identifier of the deleted guild.
    /// </summary>
    /// <value>
    /// The guild identifier of the deleted guild.
    /// </value>
    public uint GuildId { get; }
}