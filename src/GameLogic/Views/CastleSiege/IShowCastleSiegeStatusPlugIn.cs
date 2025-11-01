// <copyright file="IShowCastleSiegeStatusPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Interface for showing the castle siege status to the player.
/// </summary>
public interface IShowCastleSiegeStatusPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the castle siege status to the player.
    /// </summary>
    /// <param name="ownerGuildName">The name of the guild that currently owns the castle.</param>
    /// <param name="siegeStatus">The current status of the siege (e.g., "Idle", "Registration", "In Progress").</param>
    ValueTask ShowStatusAsync(string ownerGuildName, string siegeStatus);
}
