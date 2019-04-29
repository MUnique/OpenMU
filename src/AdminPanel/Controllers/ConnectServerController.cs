// <copyright file="ConnectServerController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using log4net;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using MUnique.OpenMU.AdminPanel.Hubs;
    using MUnique.OpenMU.AdminPanel.Models;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence;
    using MUnique.OpenMU.Persistence.BasicModel;
    using MUnique.OpenMU.Persistence.Json;

    /// <summary>
    /// The controller for connect servers in the admin panel.
    /// </summary>
    [Route("admin/[controller]")]
    [ApiController]
    public class ConnectServerController : ControllerBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ConnectServerController));
        private readonly IList<IManageableServer> servers;
        private readonly IPersistenceContextProvider persistenceContextProvider;
        private readonly IHubContext<ServerListHub> serverListHubContext;
        private readonly IServerConfigurationChangeListener changeListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectServerController" /> class.
        /// </summary>
        /// <param name="servers">The servers.</param>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        /// <param name="serverListHubContext">The server list hub context.</param>
        public ConnectServerController(IList<IManageableServer> servers, IPersistenceContextProvider persistenceContextProvider, IHubContext<ServerListHub> serverListHubContext, IServerConfigurationChangeListener changeListener)
        {
            this.servers = servers;
            this.persistenceContextProvider = persistenceContextProvider;
            this.serverListHubContext = serverListHubContext;
            this.changeListener = changeListener;
        }

        /// <summary>
        /// Gets the he connect server definition of the specified id.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <returns>The connect server definition of the specified id</returns>
        [HttpGet("{serverId}")]
        public ActionResult<ConnectServerDefinitionDto> Get(int serverId)
        {
            try
            {
                Log.Info($"requested configuration of connect server {serverId}");
                if (this.servers.FirstOrDefault(s => s.Id == serverId) is IConnectServer server
                    && server.Settings is IConvertibleTo<ConnectServerDefinition> convertible)
                {
                    var converted = convertible.Convert();
                    return this.Ok(converted);
                }

                return this.NotFound();
            }
            catch (Exception ex)
            {
                Log.Error($"An unexpected exception occured during requesting configuration for connect server {serverId}", ex);
                throw;
            }
        }

        /// <summary>
        /// Deletes the specified server.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <returns>The result.</returns>
        [HttpDelete("{serverId}")]
        public ActionResult Delete(int serverId)
        {
            try
            {
                Log.Warn($"User requested to delete connect server {serverId}.");
                if (this.servers.FirstOrDefault(s => s.Id == serverId) is IConnectServer server
                    && server.Settings is DataModel.Configuration.ConnectServerDefinition settings)
                {
                    using (var configContext = this.persistenceContextProvider.CreateNewContext())
                    {
                        var success = configContext.Delete(settings);
                        if (success)
                        {
                            server.Shutdown();
                            this.servers.Remove(server);
                            Log.Warn($"User successfully deleted connect server {serverId}.");
                            return this.Ok();
                        }

                        Log.Warn($"User couldn't delete connect server {serverId}, id {settings.GetId()}.");
                        return this.UnprocessableEntity();
                    }
                }

                Log.Warn($"Couldn't find connect server {serverId}.");
                return this.NotFound();
            }
            catch (Exception e)
            {
                Log.Error($"An unexpected error occured during the request to delete connect server {serverId}.", e);
                throw;
            }
        }

        /// <summary>
        /// Saves the specified configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The internal id of the saved object.</returns>
        [HttpPost]
        public ActionResult<Guid> Save([FromBody]ConnectServerDefinitionDto configuration)
        {
            try
            {
                Log.Info($"requested to save configuration of connect server {configuration.ServerId}");
                DataModel.Configuration.ConnectServerDefinition currentConfiguration = null;
                if (this.servers.OfType<IConnectServer>().FirstOrDefault(s => s.Settings.GetId() == configuration.Id) is IConnectServer server)
                {
                    currentConfiguration = server.Settings as DataModel.Configuration.ConnectServerDefinition;
                }

                bool isNew = false;

                using (var configContext = this.persistenceContextProvider.CreateNewContext())
                {
                    if (currentConfiguration != null)
                    {
                        configContext.Attach(currentConfiguration);
                    }
                    else
                    {
                        currentConfiguration = configContext.CreateNew<DataModel.Configuration.ConnectServerDefinition>();
                        currentConfiguration.ServerId = configuration.ServerId;
                        var existingConfigs = configContext.Get<DataModel.Configuration.ConnectServerDefinition>().ToList();
                        if (existingConfigs.Count > 0)
                        {
                            var newId = existingConfigs.Max(c => c.ServerId) + 1;
                            currentConfiguration.ServerId = (byte)newId;
                        }

                        isNew = true;
                    }

                    currentConfiguration.CheckMaxConnectionsPerAddress = configuration.CheckMaxConnectionsPerAddress;
                    currentConfiguration.ListenerBacklog = configuration.ListenerBacklog;
                    currentConfiguration.MaxConnections = configuration.MaxConnections;
                    currentConfiguration.MaxConnectionsPerAddress = configuration.MaxConnectionsPerAddress;
                    currentConfiguration.MaxFtpRequests = configuration.MaxFtpRequests;
                    currentConfiguration.MaxIpRequests = configuration.MaxIpRequests;
                    currentConfiguration.MaximumReceiveSize = configuration.MaximumReceiveSize;
                    currentConfiguration.MaxServerListRequests = configuration.MaxServerListRequests;
                    currentConfiguration.ClientListenerPort = configuration.ClientListenerPort;
                    currentConfiguration.CurrentPatchVersion = configuration.CurrentPatchVersion;
                    currentConfiguration.Timeout = configuration.Timeout;
                    currentConfiguration.PatchAddress = configuration.PatchAddress;
                    currentConfiguration.Description = configuration.Description;
                    currentConfiguration.DisconnectOnUnknownPacket = configuration.DisconnectOnUnknownPacket;
                    if (!Equals(currentConfiguration.Client, configuration.GameClient))
                    {
                        currentConfiguration.Client = configContext.GetById<DataModel.Configuration.GameClientDefinition>(configuration.GameClient.Id);
                    }

                    configContext.SaveChanges();
                }

                if (isNew)
                {
                    this.changeListener?.ConnectionServerAdded(currentConfiguration);
                }
                else
                {
                    this.changeListener?.ConnectionServerChanged(currentConfiguration);
                }

                if (this.serverListHubContext != null)
                {
                    Task.Run(() => ServerListHub.InitializeAllClients(this.serverListHubContext, this.servers, this.persistenceContextProvider).ConfigureAwait(false));
                }

                return this.Ok(currentConfiguration.GetId());
            }
            catch (Exception ex)
            {
                Log.Error($"An unexpected exception occured during saving the connect server configuration for server id {configuration.ServerId}", ex);
                throw;
            }
        }
    }
}
