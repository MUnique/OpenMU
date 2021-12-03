// <copyright file="NewPlayersInScopePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Map.ViewPlugIns;

using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Implementation of <see cref="INewPlayersInScopePlugIn"/> which uses the javascript map app.
/// </summary>
public class NewPlayersInScopePlugIn : JsViewPlugInBase, INewPlayersInScopePlugIn
{
    private readonly IDictionary<int, ILocateable> _objects;
    private readonly Action _objectsChangedCallback;

    /// <summary>
    /// Initializes a new instance of the <see cref="NewPlayersInScopePlugIn" /> class.
    /// </summary>
    /// <param name="jsRuntime">The js runtime.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="worldAccessor">The world accessor.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="objects">The objects.</param>
    /// <param name="objectsChangedCallback">The objects changed callback.</param>
    public NewPlayersInScopePlugIn(IJSRuntime jsRuntime, ILoggerFactory loggerFactory, string worldAccessor, CancellationToken cancellationToken, IDictionary<int, ILocateable> objects, Action objectsChangedCallback)
        : base(jsRuntime, loggerFactory, $"{worldAccessor}.addOrUpdatePlayer", cancellationToken)
    {
        this._objects = objects;
        this._objectsChangedCallback = objectsChangedCallback;
    }

    /// <inheritdoc />
    public async void NewPlayersInScope(IEnumerable<Player> newObjects, bool isSpawned = true)
    {
        try
        {
            foreach (var player in newObjects)
            {
                this._objects.TryAdd(player.Id, player);

                if (this.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                await this.InvokeAsync(player.CreateMapObject());
            }

            this._objectsChangedCallback?.Invoke();
        }
        catch (Exception e)
        {
            this.Logger.LogError(e, $"Error in {nameof(this.NewPlayersInScope)}");
        }
    }
}