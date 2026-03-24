// <copyright file="IShowGuildRelationshipRequestPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild;

/// <summary>
/// Interface of a view whose implementation informs a guild master about an incoming guild relationship change request.
/// </summary>
public interface IShowGuildRelationshipRequestPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the incoming guild relationship change request.
    /// </summary>
    /// <param name="requestingGuildName">Name of the requesting guild.</param>
    /// <param name="relationshipType">The type of relationship being requested.</param>
    /// <param name="requestType">The type of request (join/leave).</param>
    ValueTask ShowRequestAsync(string requestingGuildName, GuildRelationshipType relationshipType, GuildRelationshipRequestType requestType);
}
