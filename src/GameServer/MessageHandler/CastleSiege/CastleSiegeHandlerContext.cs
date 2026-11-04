// <copyright file="CastleSiegeHandlerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Resolves the active Castle Siege context for packet handlers.
/// </summary>
internal static class CastleSiegeHandlerContext
{
    /// <summary>
    /// Gets the initialized context for a player's game context.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The Castle Siege context, or <see langword="null"/>.</returns>
    public static CastleSiegeContext? Get(Player player)
    {
        return player.GameContext.PlugInManager
            .GetActivePlugInsOf<IPeriodicTaskPlugIn>()
            .OfType<CastleSiegePlugIn>()
            .FirstOrDefault()
            ?.GetContext(player.GameContext);
    }
}
