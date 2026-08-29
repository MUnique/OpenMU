// <copyright file="ICastleSiegeOwnershipChangePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// A view which announces the current Castle Siege owner.
/// </summary>
public interface ICastleSiegeOwnershipChangePlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the guild which owns the castle.
    /// </summary>
    /// <param name="guildName">The owner guild name, or an empty string when the castle has no owner.</param>
    /// <returns>A task that represents the asynchronous show operation.</returns>
    ValueTask ShowOwnershipChangeAsync(string guildName);
}
