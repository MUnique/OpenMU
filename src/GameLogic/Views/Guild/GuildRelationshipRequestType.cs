// <copyright file="GuildRelationshipRequestType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild;

/// <summary>
/// The type of guild relationship request.
/// </summary>
public enum GuildRelationshipRequestType
{
    /// <summary>
    /// An undefined request type.
    /// </summary>
    Undefined,

    /// <summary>
    /// A join request.
    /// </summary>
    Join,

    /// <summary>
    /// A leave request.
    /// </summary>
    Leave,

    /// <summary>
    /// A disband request? Only possible for <see cref="GuildRelationshipType.Alliance"/>.
    /// </summary>
    Disband,
}
