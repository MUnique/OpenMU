// <copyright file="JavascriptMapFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map.Map;

using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

/// <summary>
/// Class which manages the creation of the map which is implemented in javascript.
/// </summary>
public sealed class JavascriptMapFactory : IMapFactory
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILoggerFactory _loggerFactory;
    private readonly CircuitConnectionState _circuitState;

    private int _mapCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="JavascriptMapFactory"/> class.
    /// </summary>
    /// <param name="jsRuntime">The js runtime.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="circuitState">The connection state of the browser circuit.</param>
    public JavascriptMapFactory(IJSRuntime jsRuntime, ILoggerFactory loggerFactory, CircuitConnectionState circuitState)
    {
        this._jsRuntime = jsRuntime;
        this._loggerFactory = loggerFactory;
        this._circuitState = circuitState;
    }

    /// <inheritdoc />
    public async ValueTask<IMapController> CreateMapAsync(IObservableGameServer server, Guid mapId)
    {
        MapController? mapController = null;
        try
        {
            var appId = this.GenerateMapAppIdentifier(server.Id, mapId);
            await this._jsRuntime.InvokeVoidAsync("CreateMap", server.Id, mapId, this.GetMapContainerIdentifier(server.Id, mapId), appId).ConfigureAwait(false);
            mapController = new MapController(this._jsRuntime, this._loggerFactory, appId, server, mapId, this._circuitState);
            await server.RegisterMapObserverAsync(mapId, mapController).ConfigureAwait(false);
        }
        catch
        {
            if (mapController != null)
            {
                await mapController.DisposeAsync().ConfigureAwait(false);
            }

            throw;
        }

        return mapController;
    }

    /// <inheritdoc />
    public string GetMapContainerIdentifier(int serverId, Guid mapId) => $"map_{serverId}_{mapId:N}";

    private string GenerateMapAppIdentifier(int serverId, Guid mapId) => $"map_{serverId}_{mapId:N}_app{this._mapCount++}";
}