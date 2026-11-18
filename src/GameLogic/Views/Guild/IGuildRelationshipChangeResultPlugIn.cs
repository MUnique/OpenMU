// <copyright file="IGuildRelationshipChangeResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild;

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
    /// <param name="resultType">The result of the relationship change request.</param>
    /// <param name="guildMasterId">The id of the guild master which was asked for the relationship change.</param>
    ValueTask ShowResultAsync(GuildRelationshipType relationshipType, GuildRelationshipRequestType requestType, GuildRelationshipChangeResultType resultType, ushort? guildMasterId);

    /// <summary>
    /// Shows the result of a requested remove request, when a guild is removed from an alliance by the alliance master.
    /// </summary>
    /// <param name="result">The success flag.</param>
    /// <param name="relationshipType">Type of the relationship, usually <see cref="GuildRelationshipType.Alliance"/>.</param>
    /// <param name="requestType">Type of the request, usually <see cref="GuildRelationshipRequestType.Leave"/>.</param>
    ValueTask ShowRemoveResultAsync(bool result, GuildRelationshipType relationshipType = GuildRelationshipType.Alliance, GuildRelationshipRequestType requestType = GuildRelationshipRequestType.Leave);
}
