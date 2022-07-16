// <copyright file="NewPlayersInScopePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map.ViewPlugIns;

using System.Collections.Concurrent;
using Microsoft.JSInterop;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Web.Map.Map;

/// <summary>
/// Implementation of <see cref="INewPlayersInScopePlugIn"/> which uses the javascript map app.
/// </summary>
public class NewPlayersInScopePlugIn : JsViewPlugInBase, INewPlayersInScopePlugIn
{
    private readonly ConcurrentDictionary<int, Player> _players;
    private readonly Action _playersChangedCallback;

    /// <summary>
    /// Initializes a new instance of the <see cref="NewPlayersInScopePlugIn" /> class.
    /// </summary>
    /// <param name="jsRuntime">The js runtime.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="worldAccessor">The world accessor.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="players">The objects.</param>
    /// <param name="playersChangedCallback">The players changed callback.</param>
    public NewPlayersInScopePlugIn(IJSRuntime jsRuntime, ILoggerFactory loggerFactory, string worldAccessor, CancellationToken cancellationToken, ConcurrentDictionary<int, Player> players, Action playersChangedCallback)
        : base(jsRuntime, loggerFactory, $"{worldAccessor}.addOrUpdatePlayer", cancellationToken)
    {
        this._players = players;
        this._playersChangedCallback = playersChangedCallback;
    }

    /// <inheritdoc />
    public async ValueTask NewPlayersInScopeAsync(IEnumerable<Player> newObjects, bool isSpawned = true)
    {
        try
        {
            foreach (var player in newObjects)
            {
                this._players.TryAdd(player.Id, player);

                if (this.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                await this.InvokeAsync(player.CreateMapObject()).ConfigureAwait(false);
            }

            this._playersChangedCallback?.Invoke();
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, $"Error in {nameof(this.NewPlayersInScopeAsync)}");
        }
    }
}