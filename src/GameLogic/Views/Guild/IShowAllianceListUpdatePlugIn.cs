// <copyright file="IShowAllianceListUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild;

/// <summary>
/// Interface of a view whose implementation notifies about an alliance list update.
/// </summary>
public interface IShowAllianceListUpdatePlugIn : IViewPlugIn
{
    /// <summary>
    /// Notifies that the alliance list should be updated.
    /// </summary>
    ValueTask UpdateAllianceListAsync();
}
