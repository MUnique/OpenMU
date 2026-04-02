// <copyright file="OfflineViewPlugInContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin container for the <see cref="OfflineLevelingPlayer"/>, providing stub implementations 
/// of client-facing views necessary for successful safe-zone respawns.
/// </summary>
internal sealed class OfflineViewPlugInContainer : ICustomPlugInContainer<IViewPlugIn>
{
    private readonly IRespawnAfterDeathPlugIn _respawnPlugIn = new OfflineRespawnPlugIn();
    private readonly IMapChangePlugIn _mapChangePlugIn;

    /// <summary>
    /// Initializes a new instance of the <see cref="OfflineViewPlugInContainer"/> class.
    /// </summary>
    /// <param name="player">The offline leveling player.</param>
    public OfflineViewPlugInContainer(OfflineLevelingPlayer player)
    {
        this._mapChangePlugIn = new OfflineMapChangePlugIn(player);
    }

    /// <inheritdoc/>
    public T? GetPlugIn<T>()
        where T : class, IViewPlugIn
    {
        if (typeof(T) == typeof(IRespawnAfterDeathPlugIn))
        {
            return (T)this._respawnPlugIn;
        }

        if (typeof(T) == typeof(IMapChangePlugIn))
        {
            return (T)this._mapChangePlugIn;
        }

        return null;
    }
}
