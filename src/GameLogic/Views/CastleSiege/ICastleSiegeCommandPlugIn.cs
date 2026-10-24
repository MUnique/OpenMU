// <copyright file="ICastleSiegeCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// A view which shows a guild command issued by an alliance master to same-side players.
/// </summary>
public interface ICastleSiegeCommandPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows a directional guild command.
    /// </summary>
    /// <param name="team">The issuer's command-group (squad) slot, 0-6, relayed unchanged from the request.</param>
    /// <param name="positionX">The target X coordinate.</param>
    /// <param name="positionY">The target Y coordinate.</param>
    /// <param name="command">The command type.</param>
    /// <returns>A task that represents the asynchronous view update.</returns>
    ValueTask ShowGuildCommandAsync(byte team, byte positionX, byte positionY, CastleSiegeCommandType command);
}
