// <copyright file="ICastleSiegeCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// A view which shows a guild command issued by an alliance master to same-side players.
/// </summary>
public interface ICastleSiegeCommandPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows a directional guild command.
    /// </summary>
    /// <param name="side">The side the command was issued to.</param>
    /// <param name="positionX">The target X coordinate.</param>
    /// <param name="positionY">The target Y coordinate.</param>
    /// <param name="command">The command type.</param>
    /// <returns>A task that represents the asynchronous view update.</returns>
    ValueTask ShowGuildCommandAsync(CastleSiegeJoinSide side, byte positionX, byte positionY, CastleSiegeCommandType command);
}
