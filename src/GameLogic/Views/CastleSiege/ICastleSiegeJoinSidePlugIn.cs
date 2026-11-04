// <copyright file="ICastleSiegeJoinSidePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// A view which informs a player about their Castle Siege side.
/// </summary>
public interface ICastleSiegeJoinSidePlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the assigned Castle Siege side.
    /// </summary>
    /// <param name="side">The assigned side.</param>
    /// <returns>A task that represents the asynchronous show operation.</returns>
    ValueTask ShowJoinSideAsync(CastleSiegeJoinSide side);
}
