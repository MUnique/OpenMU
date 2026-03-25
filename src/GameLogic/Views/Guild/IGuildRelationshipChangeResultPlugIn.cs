// <copyright file="IGuildRelationshipChangeResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild;

/// <summary>
/// The type of guild relationship.
/// </summary>
public enum GuildRelationshipType
{
    /// <summary>
    /// An undefined relationship type.
    /// </summary>
    Undefined,

    /// <summary>
    /// An alliance relationship.
    /// </summary>
    Alliance,

    /// <summary>
    /// A hostility relationship.
    /// </summary>
    Hostility,
}

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
}

/// <summary>
/// Interface of a view whose implementation informs about the result of a guild relationship change request.
/// </summary>
public interface IGuildRelationshipChangeResultPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the result of the guild relationship change request.
    /// </summary>
    /// <param name="relationshipType">The type of relationship that was being changed.</param>
    /// <param name="requestType">The type of request (join/leave).</param>
    /// <param name="success">Whether the relationship change was successful.</param>
    ValueTask ShowResultAsync(GuildRelationshipType relationshipType, GuildRelationshipRequestType requestType, bool success);
}
