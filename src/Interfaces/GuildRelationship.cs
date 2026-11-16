// <copyright file="GuildRelationship.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// Describes the relationship between two guilds.
/// </summary>
public enum GuildRelationship
{
    /// <summary>
    /// No special relationship.
    /// </summary>
    None = 0,

    /// <summary>
    /// Both guilds are in the same alliance.
    /// </summary>
    Union = 1,

    /// <summary>
    /// The guilds are rivals / hostile to each other.
    /// </summary>
    Rival = 2,
}
