// <copyright file="JavascriptMapFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Map;

using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MUnique.OpenMU.AdminPanel.Services;

/// <summary>
/// Class which manages the creation of the map which is implemented in javascript.
/// </summary>
public sealed class JavascriptMapFactory : IMapFactory
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ServerService _serverService;
    private readonly ILoggerFactory _loggerFactory;

    private int _mapCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="JavascriptMapFactory"/> class.
    /// </summary>
    /// <param name="jsRuntime">The js runtime.</param>
    /// <param name="serverService">The server service.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public JavascriptMapFactory(IJSRuntime jsRuntime, ServerService serverService, ILoggerFactory loggerFactory)
    {
        this._jsRuntime = jsRuntime;
        this._serverService = serverService;
        this._loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public async ValueTask<IMapController> CreateMap(int serverId, int mapId)
    {
        IMapController? mapController = null;
        try
        {
            var appId = this.GenerateMapAppIdentifier(serverId, mapId);
            await this._jsRuntime.InvokeVoidAsync("CreateMap", serverId, mapId, this.GetMapContainerIdentifier(serverId, mapId), appId);
            var gameServer = this._serverService.GetGameServer(serverId);
            mapController = new MapController(this._jsRuntime, this._loggerFactory, appId, gameServer, mapId);
            gameServer.RegisterMapObserver((ushort)mapId, mapController);
        }
        catch
        {
            if (mapController != null)
            {
                await mapController.DisposeAsync();
            }

            throw;
        }

        return mapController;
    }

    /// <inheritdoc />
    public string GetMapContainerIdentifier(int serverId, int mapId) => $"map_{serverId}_{mapId}";

    private string GenerateMapAppIdentifier(int serverId, int mapId) => $"map_{serverId}_{mapId}_app{this._mapCount++}";
}