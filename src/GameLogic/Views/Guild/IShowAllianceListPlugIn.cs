// <copyright file="IShowAllianceListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Interface of a view whose implementation sends the list of guilds in an alliance to the client.
/// </summary>
public interface IShowAllianceListPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the list of guilds in the alliance.
    /// </summary>
    /// <param name="guilds">The list of alliance guild entries.</param>
    ValueTask ShowListAsync(IEnumerable<AllianceGuildEntry> guilds);
}
