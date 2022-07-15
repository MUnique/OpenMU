// <copyright file="ObjectsOutOfScopePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map.ViewPlugIns;

using System.Collections.Concurrent;
using Microsoft.JSInterop;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Implementation of <see cref="IObjectsOutOfScopePlugIn"/> which uses the javascript map app.
/// </summary>
public class ObjectsOutOfScopePlugIn : JsViewPlugInBase, IObjectsOutOfScopePlugIn
{
    private readonly ConcurrentDictionary<int, Player> _players;

    private readonly Action _playersChangedCallback;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectsOutOfScopePlugIn" /> class.
    /// </summary>
    /// <param name="jsRuntime">The js runtime.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="worldAccessor">The world accessor.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="players">The objects.</param>
    /// <param name="playersChangedCallback">The objects changed callback.</param>
    public ObjectsOutOfScopePlugIn(IJSRuntime jsRuntime, ILoggerFactory loggerFactory, string worldAccessor, CancellationToken cancellationToken, ConcurrentDictionary<int, Player> players, Action playersChangedCallback)
        : base(jsRuntime, loggerFactory, $"{worldAccessor}.removeObject", cancellationToken)
    {
        this._players = players;
        this._playersChangedCallback = playersChangedCallback;
    }

    /// <inheritdoc />
    public async ValueTask ObjectsOutOfScopeAsync(IEnumerable<IIdentifiable> objects)
    {
        try
        {
            var isPlayerInvolved = false;
            foreach (var obj in objects)
            {
                isPlayerInvolved = this._players.TryRemove(obj.Id, out _) | isPlayerInvolved;

                if (this.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                await this.InvokeAsync(obj.Id).ConfigureAwait(false);
            }

            if (isPlayerInvolved)
            {
                this._playersChangedCallback?.Invoke();
            }
        }
        catch (Exception e)
        {
            this.Logger.LogError(e, $"Error in {nameof(this.ObjectsOutOfScopeAsync)}; objects: {string.Join(';', objects)}");
        }
    }
}