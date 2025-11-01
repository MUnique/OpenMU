// <copyright file="PendingAllianceRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Represents a pending alliance request from another guild.
/// </summary>
public class PendingAllianceRequest
{
    /// <summary>
    /// Gets or sets the name of the requesting guild.
    /// </summary>
    public required string RequesterGuildName { get; init; }

    /// <summary>
    /// Gets or sets the name of the player who sent the request.
    /// </summary>
    public required string RequesterPlayerName { get; init; }
}
