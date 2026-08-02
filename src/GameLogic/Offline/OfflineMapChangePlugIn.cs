// <copyright file="OfflineMapChangePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Offline;

using System.Threading.Tasks;
using MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Simulates a map change response for an offline character.
/// </summary>
internal sealed class OfflineMapChangePlugIn : IMapChangePlugIn
{
    private readonly OfflinePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="OfflineMapChangePlugIn"/> class.
    /// </summary>
    /// <param name="player">The offline player.</param>
    public OfflineMapChangePlugIn(OfflinePlayer player)
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
