// <copyright file="JavascriptMapFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Map
{
    using System.Threading.Tasks;
    using Microsoft.JSInterop;
    using MUnique.OpenMU.AdminPanel.Services;

    /// <summary>
    /// Class which manages the creation of the map which is implemented in javascript.
    /// </summary>
    public sealed class JavascriptMapFactory : IMapFactory
    {
        private readonly IJSRuntime jsRuntime;
        private readonly ServerService serverService;

        private int mapCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="JavascriptMapFactory"/> class.
        /// </summary>
        /// <param name="jsRuntime">The js runtime.</param>
        /// <param name="serverService">The server service.</param>
        public JavascriptMapFactory(IJSRuntime jsRuntime, ServerService serverService)
        {
            this.jsRuntime = jsRuntime;
            this.serverService = serverService;
        }

        /// <inheritdoc />
        public async ValueTask<IMapController> CreateMap(int serverId, int mapId)
        {
            IMapController mapController = null;
            try
            {
                var appId = this.GenerateMapAppIdentifier(serverId, mapId);
                await this.jsRuntime.InvokeVoidAsync("CreateMap", serverId, mapId, this.GetMapContainerIdentifier(serverId, mapId), appId);
                var gameServer = this.serverService.GetGameServer(serverId);
                mapController = new MapController(this.jsRuntime, appId, gameServer, mapId);
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

        private string GenerateMapAppIdentifier(int serverId, int mapId) => $"map_{serverId}_{mapId}_app{this.mapCount++}";
    }
}
