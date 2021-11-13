// <copyright file="ObjectsOutOfScopePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Map.ViewPlugIns;

using System.Reflection;
using System.Threading;
using log4net;
using Microsoft.JSInterop;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Implementation of <see cref="IObjectsOutOfScopePlugIn"/> which uses the javascript map app.
/// </summary>
public class ObjectsOutOfScopePlugIn : JsViewPlugInBase, IObjectsOutOfScopePlugIn
{
    private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);

    private readonly IDictionary<int, ILocateable> _objects;

    private readonly Action _objectsChangedCallback;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectsOutOfScopePlugIn" /> class.
    /// </summary>
    /// <param name="jsRuntime">The js runtime.</param>
    /// <param name="worldAccessor">The world accessor.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="objects">The objects.</param>
    /// <param name="objectsChangedCallback">The objects changed callback.</param>
    public ObjectsOutOfScopePlugIn(IJSRuntime jsRuntime, string worldAccessor, CancellationToken cancellationToken, IDictionary<int, ILocateable> objects, Action objectsChangedCallback)
        : base(jsRuntime, $"{worldAccessor}.removeObject", cancellationToken)
    {
        this._objects = objects;
        this._objectsChangedCallback = objectsChangedCallback;
    }

    /// <inheritdoc />
    public async void ObjectsOutOfScope(IEnumerable<IIdentifiable> objects)
    {
        try
        {
            foreach (var obj in objects)
            {
                this._objects.Remove(obj.Id);

                if (this.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                await this.InvokeAsync(obj.Id);
            }

            this._objectsChangedCallback?.Invoke();
        }
        catch (Exception e)
        {
            Log.Error($"Error in {nameof(this.ObjectsOutOfScope)}; objects: {string.Join(';', objects)}", e);
        }
    }
}