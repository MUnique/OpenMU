// <copyright file="ICastleSiegeNpcListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// A view which shows Castle Siege gates and Guardian Statues.
/// </summary>
public interface ICastleSiegeNpcListPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows a Castle Siege NPC list.
    /// </summary>
    /// <param name="npcs">The NPC information.</param>
    /// <returns>A task that represents the asynchronous view update.</returns>
    ValueTask ShowNpcListAsync(IReadOnlyList<CastleSiegeNpcInfo> npcs);
}
