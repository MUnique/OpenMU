// <copyright file="GuildChatScope.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// The chat of a guild which is bound to a channel of an external service.
/// </summary>
public enum GuildChatScope
{
    /// <summary>
    /// The chat of the guild.
    /// </summary>
    Guild,

    /// <summary>
    /// The chat of the alliance whose master is the guild.
    /// </summary>
    Alliance,
}
