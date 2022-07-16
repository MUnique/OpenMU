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

    private int _mapCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="JavascriptMapFactory"/> class.
    /// </summary>
    /// <param name="jsRuntime">The js runtime.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public JavascriptMapFactory(IJSRuntime jsRuntime, ILoggerFactory loggerFactory)
    {
        this._jsRuntime = jsRuntime;
        this._loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public async ValueTask<IMapController> CreateMapAsync(IObservableGameServer gameServer, Guid mapId)
    {
        MapController? mapController = null;
        try
        {
            var appId = this.GenerateMapAppIdentifier(gameServer.Id, mapId);
            await this._jsRuntime.InvokeVoidAsync("CreateMap", gameServer.Id, mapId, this.GetMapContainerIdentifier(gameServer.Id, mapId), appId).ConfigureAwait(false);
            mapController = new MapController(this._jsRuntime, this._loggerFactory, appId, gameServer, mapId);
            await gameServer.RegisterMapObserverAsync(mapId, mapController).ConfigureAwait(false);
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