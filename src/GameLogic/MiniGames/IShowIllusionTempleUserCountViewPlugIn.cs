// <copyright file="IShowIllusionTempleUserCountViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// Interface for view plugins which show how many players are currently in each illusion temple.
/// </summary>
public interface IShowIllusionTempleUserCountViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows how many players are currently in each of the six illusion temples.
    /// </summary>
    /// <param name="userCounts">The player counts, indexed by temple (index 0 is temple 1).</param>
    ValueTask ShowUserCountAsync(IReadOnlyList<int> userCounts);
}
