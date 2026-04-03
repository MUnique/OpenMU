// <copyright file="OfflineMapChangePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using MUnique.OpenMU.GameLogic.Views.World;
using System.Threading.Tasks;

/// <summary>
/// Simulates a map change response for an offline leveling character.
/// </summary>
internal sealed class OfflineMapChangePlugIn : IMapChangePlugIn
{
    private readonly OfflineLevelingPlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="OfflineMapChangePlugIn"/> class.
    /// </summary>
    /// <param name="player">The offline leveling player.</param>
    public OfflineMapChangePlugIn(OfflineLevelingPlayer player)
    {
        this._player = player;
    }

    /// <inheritdoc/>
    public async ValueTask MapChangeAsync()
    {
        await this._player.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public ValueTask MapChangeFailedAsync() => ValueTask.CompletedTask;
}
