// <copyright file="CastleSiegeContextResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Resolves the Castle Siege context which belongs to a player.
/// </summary>
internal static class CastleSiegeContextResolver
{
    /// <summary>
    /// Gets the initialized Castle Siege context of a player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The context, or <see langword="null"/> when Castle Siege is not active.</returns>
    internal static CastleSiegeContext? GetContext(Player player)
    {
        return player.GameContext.PlugInManager
            .GetActivePlugInsOf<IPeriodicTaskPlugIn>()
            .OfType<CastleSiegePlugIn>()
            .FirstOrDefault()
            ?.GetContext(player.GameContext);
    }
}
