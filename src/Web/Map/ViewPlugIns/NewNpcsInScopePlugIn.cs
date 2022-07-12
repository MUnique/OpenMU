// <copyright file="NewNpcsInScopePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map.ViewPlugIns;

using Microsoft.JSInterop;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Web.Map.Map;

/// <summary>
/// Implementation of <see cref="INewNpcsInScopePlugIn"/> which uses the javascript map app.
/// </summary>
public class NewNpcsInScopePlugIn : JsViewPlugInBase, INewNpcsInScopePlugIn
{
    private readonly IDictionary<int, ILocateable> _objects;
    private readonly Action _objectsChangedCallback;

    /// <summary>
    /// Initializes a new instance of the <see cref="NewNpcsInScopePlugIn" /> class.
    /// </summary>
    /// <param name="jsRuntime">The js runtime.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="worldAccessor">The world accessor.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="objects">The objects.</param>
    /// <param name="objectsChangedCallback">The objects changed callback.</param>
    public NewNpcsInScopePlugIn(IJSRuntime jsRuntime, ILoggerFactory loggerFactory, string worldAccessor, CancellationToken cancellationToken, IDictionary<int, ILocateable> objects, Action objectsChangedCallback)
        : base(jsRuntime, loggerFactory, $"{worldAccessor}.addOrUpdateNpc", cancellationToken)
    {
        this._objects = objects;
        this._objectsChangedCallback = objectsChangedCallback;
    }

    /// <inheritdoc />
    public async ValueTask NewNpcsInScopeAsync(IEnumerable<NonPlayerCharacter> newObjects, bool isSpawned = true)
    {
        try
        {
            foreach (var npc in newObjects)
            {
                this._objects.TryAdd(npc.Id, npc);

                if (this.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                await this.InvokeAsync(npc.CreateMapObject());
            }

            this._objectsChangedCallback?.Invoke();
        }
        catch (TaskCanceledException)
        {
            // don't need to handle that.
        }
        catch (Exception e)
        {
            this.Logger.LogError(e, $"Error in {nameof(this.NewNpcsInScopeAsync)}");
        }
    }
}