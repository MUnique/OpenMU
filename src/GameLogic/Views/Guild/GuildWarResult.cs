// <copyright file="GuildWarResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild;

/// <summary>
/// The result of a guild war.
/// </summary>
public enum GuildWarResult
{
    /// <summary>
    /// The guild war was lost.
    /// </summary>
    Lost,

    /// <summary>
    /// The guild war was won.
    /// </summary>
    Won,

    /// <summary>
    /// The other guild master cancelled the guild war.
    /// </summary>
    OtherGuildMasterCancelledWar,

    /// <summary>
    /// The own guild master cancelled the guild war.
    /// </summary>
    CancelledWar,
}