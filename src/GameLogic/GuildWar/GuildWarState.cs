// <copyright file="GuildWarState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.GuildWar;

/// <summary>
/// The state of a guild war.
/// </summary>
public enum GuildWarState
{
    /// <summary>
    /// The guild war was requested.
    /// </summary>
    Requested,

    /// <summary>
    /// The guild war was started and is ongoing.
    /// </summary>
    Started,

    /// <summary>
    /// The guild war has ended.
    /// </summary>
    Ended,
}