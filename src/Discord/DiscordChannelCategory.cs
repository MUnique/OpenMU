// <copyright file="DiscordChannelCategory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

/// <summary>
/// The categories of notifications, each of which is posted to its own Discord channel.
/// </summary>
public enum DiscordChannelCategory
{
    /// <summary>
    /// Events like mini games and invasions.
    /// </summary>
    Events,

    /// <summary>
    /// The state changes of the castle siege.
    /// </summary>
    CastleSiege,

    /// <summary>
    /// News about players, like boss kills, rare drops and level milestones.
    /// </summary>
    WorldNews,

    /// <summary>
    /// Notices of game masters.
    /// </summary>
    Notices,
}
