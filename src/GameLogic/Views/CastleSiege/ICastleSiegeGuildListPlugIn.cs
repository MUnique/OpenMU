// <copyright file="ICastleSiegeGuildListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

using MUnique.OpenMU.GameLogic.CastleSiege;

/// <summary>
/// A view which shows the selected Castle Siege guilds.
/// </summary>
public interface ICastleSiegeGuildListPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the final participating guild list.
    /// </summary>
    /// <param name="result">The request result; 1 for success and 2 when the list is unavailable.</param>
    /// <param name="guilds">The selected guilds.</param>
    /// <returns>A task that represents the asynchronous show operation.</returns>
    ValueTask ShowGuildListAsync(byte result, IReadOnlyCollection<CastleSiegeGuildParticipant> guilds);
}
