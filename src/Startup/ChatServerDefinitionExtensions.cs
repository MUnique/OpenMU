// <copyright file="ChatServerDefinitionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Startup
{
    using MUnique.OpenMU.ChatServer;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network.PlugIns;

    /// <summary>
    /// Extensions for <see cref="ChatServerDefinition"/>.
    /// </summary>
    public static class ChatServerDefinitionExtensions
    {
        /// <summary>
        /// Converts the <see cref="ChatServerDefinition"/> into corresponding <see cref="ChatServerSettings"/>.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>The settings.</returns>
        public static ChatServerSettings ConvertToSettings(this ChatServerDefinition definition)
        {
            var result = new ChatServerSettings();
            result.MaximumConnections = definition.MaximumConnections;
            result.ClientTimeout = definition.ClientTimeout;
            result.ClientCleanUpInterval = definition.ClientCleanUpInterval;
            result.RoomCleanUpInterval = definition.RoomCleanUpInterval;
            result.Description = definition.Description;
            result.ServerId = definition.ServerId + SpecialServerIds.ChatServer;
            foreach (var endpoint in definition.Endpoints)
            {
                result.Endpoints.Add(new OpenMU.ChatServer.ChatServerEndpoint
                {
                    ClientVersion = new ClientVersion(endpoint.Client.Season, endpoint.Client.Episode, endpoint.Client.Language),
                    NetworkPort = endpoint.NetworkPort,
                });
            }

            return result;
        }
    }
}