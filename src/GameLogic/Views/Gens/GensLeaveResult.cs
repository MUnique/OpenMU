// <copyright file="GensLeaveResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Gens;

/// <summary>
/// The result of a request to leave a gens.
/// </summary>
public enum GensLeaveResult
{
    /// <summary>
    /// The player left the gens.
    /// </summary>
    Success,

    /// <summary>
    /// The player is not member of a gens.
    /// </summary>
    NotJoined,

    /// <summary>
    /// The player is the master of a guild, which can't leave the gens.
    /// </summary>
    GuildMaster,

    /// <summary>
    /// The player is member of a different gens than the one of the npc.
    /// </summary>
    DifferentGensNpc,
}
