// <copyright file="GensPlayerExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Gens;

using MUnique.OpenMU.GameLogic.Views.Gens;

/// <summary>
/// Extensions for the gens of a <see cref="Player"/>.
/// </summary>
public static class GensPlayerExtensions
{
    /// <summary>
    /// Gets the gens of the player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The gens of the player; <see cref="GensType.None"/>, if it's not a member of a gens.</returns>
    public static GensType GetGens(this Player player)
    {
        return player.GensMember?.Gens ?? GensType.None;
    }

    /// <summary>
    /// Determines whether the player is in a map of the battle zone.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="configuration">The configuration of the gens system.</param>
    /// <returns><c>true</c>, if the player is in a map of the battle zone; otherwise, <c>false</c>.</returns>
    public static bool IsInBattleZone(this Player player, GensConfiguration configuration)
    {
        return player.CurrentMap?.Definition is { } map && configuration.IsBattleZone(map.Number);
    }

    /// <summary>
    /// Shows the changed gens, rank or contribution of the player to itself and to the observing players.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The task.</returns>
    public static async ValueTask ShowChangedGensAsync(this Player player)
    {
        await player.InvokeViewPlugInAsync<IGensViewPlugIn>(p => p.ShowGensInfoAsync()).ConfigureAwait(false);
        await player.ForEachWorldObserverAsync<IAssignPlayersToGensPlugIn>(p => p.AssignPlayersToGensAsync([player]), false).ConfigureAwait(false);
    }
}
