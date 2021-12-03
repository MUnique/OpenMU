// <copyright file="ConnectServerService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Services;

using Microsoft.Extensions.Logging;
using Mapster;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;

/// <summary>
/// A service which provides functions to manage <see cref="IConnectServer"/>s.
/// </summary>
public class ConnectServerService
{
    private readonly ILogger<ConnectServerService> _logger;
    private readonly IList<IManageableServer> _servers;
    private readonly IPersistenceContextProvider _persistenceContextProvider;
    private readonly IServerConfigurationChangeListener _changeListener;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectServerService"/> class.
    /// </summary>
    /// <param name="servers">The servers.</param>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="changeListener">The change listener.</param>
    /// <param name="logger">The logger.</param>
    public ConnectServerService(IList<IManageableServer> servers, IPersistenceContextProvider persistenceContextProvider, IServerConfigurationChangeListener changeListener, ILogger<ConnectServerService> logger)
    {
        this._servers = servers;
        this._persistenceContextProvider = persistenceContextProvider;
        this._changeListener = changeListener;
        this._logger = logger;
    }

    /// <summary>
    /// Gets the available client definitions.
    /// </summary>
    /// <returns>The available client definitions.</returns>
    public IEnumerable<GameClientDefinition> GetClients()
    {
        using var configContext = this._persistenceContextProvider.CreateNewContext();
        return configContext.Get<GameClientDefinition>();
    }

    /// <summary>
    /// Deletes the specified connect server and shuts it down.
    /// </summary>
    /// <param name="connectServer">The connect server.</param>
    public void Delete(IConnectServer connectServer)
    {
        try
        {
            this._logger.LogWarning($"User requested to delete connect server {connectServer.Id}.");
            if (connectServer.Settings is ConnectServerDefinition settings)
            {
                using var configContext = this._persistenceContextProvider.CreateNewContext();
                var success = configContext.Delete(settings);
                if (success)
                {
                    connectServer.Shutdown();
                    this._servers.Remove(connectServer);
                    this._logger.LogWarning($"User successfully deleted connect server {connectServer.Id}.");
                }
            }
            else
            {
                this._logger.LogError($"User couldn't delete connect server {connectServer.Id}, id {connectServer.Settings.GetId()}.");
            }
        }
        catch (Exception e)
        {
            this._logger.LogError(e, $"An unexpected error occured during the request to delete connect server {connectServer.Id}.");
            throw;
        }
    }

    /// <summary>
    /// Saves the specified configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public void Save(ConnectServerDefinition configuration)
    {
        try
        {
            this._logger.LogInformation($"requested to save configuration of connect server {configuration.ServerId}");
            DataModel.Configuration.ConnectServerDefinition? currentConfiguration = null;
            if (this._servers.OfType<IConnectServer>().FirstOrDefault(s => s.Settings.GetId() == configuration.GetId()) is { } server)
            {
                currentConfiguration = server.Settings as DataModel.Configuration.ConnectServerDefinition;
            }

            bool isNew = false;

            using (var configContext = this._persistenceContextProvider.CreateNewContext())
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

                configuration.Adapt(currentConfiguration);
                if (!Equals(currentConfiguration.Client, configuration.Client))
                {
                    currentConfiguration.Client = configContext.GetById<DataModel.Configuration.GameClientDefinition>(configuration.Client!.GetId());
                }

                configContext.SaveChanges();
            }

            if (isNew)
            {
                this._changeListener?.ConnectionServerAdded(currentConfiguration);
            }
            else
            {
                this._changeListener?.ConnectionServerChanged(currentConfiguration);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError($"An unexpected exception occured during saving the connect server configuration for server id {configuration.ServerId}", ex);
            throw;
        }
    }
}