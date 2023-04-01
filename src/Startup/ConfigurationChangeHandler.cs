﻿// <copyright file="ConfigurationChangeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Startup;

using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// An implementation of <see cref="IConfigurationChangePublisher"/> which directly handles the changes.
/// </summary>
public class ConfigurationChangeHandler : IConfigurationChangePublisher
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationChangeHandler" /> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public ConfigurationChangeHandler(IServiceProvider serviceProvider)
    {
        this._serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public async Task ConfigurationChangedAsync(Type type, Guid id, object configuration)
    {
        if (configuration is PlugInConfiguration plugInConfiguration)
        {
            this.OnPlugInConfigurationChanged(id, plugInConfiguration);
        }

        if (configuration is ConnectServerDefinition connectServerDefinition)
        {
            await this.OnConnectServerDefinitionChangedAsync(id, connectServerDefinition).ConfigureAwait(false);
        }

        if (configuration is SystemConfiguration systemConfiguration)
        {
            this.OnSystemConfigurationChanged(id, systemConfiguration);
        }
    }

    /// <inheritdoc />
    public async Task ConfigurationAddedAsync(Type type, Guid id, object configuration)
    {
        if (configuration is not PlugInConfiguration plugInConfiguration)
        {
            return;
        }

        // todo: find out what to do, because usually, plugin configs are not added during runtime.
    }

    /// <inheritdoc />
    public async Task ConfigurationRemovedAsync(Type type, Guid id)
    {
        if (type.IsAssignableTo(typeof(PlugInConfiguration)) && this._serviceProvider.GetService<PlugInManager>() is { } plugInManager)
        {
            plugInManager.DeactivatePlugIn(id);
        }
    }

    private void OnSystemConfigurationChanged(Guid id, SystemConfiguration systemConfiguration)
    {
        if (this._serviceProvider.GetService<IIpAddressResolver>() is not ConfigurableIpResolver ipAddressResolver)
        {
            return;
        }

        ipAddressResolver.Configure(systemConfiguration.IpResolver, systemConfiguration.IpResolverParameter);
    }

    private async ValueTask OnConnectServerDefinitionChangedAsync(Guid id, ConnectServerDefinition connectServerDefinition)
    {
        if (this._serviceProvider.GetService<ConnectServerContainer>() is not { } connectServerContainer)
        {
            return;
        }

        foreach (var connectServer in connectServerContainer)
        {
            if (connectServer.ServerState == ServerState.Started)
            {
                await connectServer.ShutdownAsync().ConfigureAwait(false);

                //// todo: is applying new settings required?
                await connectServer.StartAsync().ConfigureAwait(false);
            }
        }
    }

    private void OnPlugInConfigurationChanged(Guid id, PlugInConfiguration plugInConfiguration)
    {
        if (this._serviceProvider.GetService<PlugInManager>() is not { } plugInManager)
        {
            return;
        }

        var currentlyActive = plugInManager.IsPlugInActive(id);
        if (currentlyActive && !plugInConfiguration.IsActive)
        {
            plugInManager.DeactivatePlugIn(id);
        }
        else if (!currentlyActive && plugInConfiguration.IsActive)
        {
            plugInManager.ActivatePlugIn(id);
        }
        else
        {
            plugInManager.ConfigurePlugIn(id, plugInConfiguration);
        }
    }
}