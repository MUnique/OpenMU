// <copyright file="GameServerHostedServiceWrapper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.Host;

using System.Threading;
using Microsoft.Extensions.Hosting;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// A wrapper which takes a <see cref="Interfaces.IGameServer"/> and wraps it as <see cref="IHostedService"/>,
/// so that additional initialization can be done before actually starting it.
/// TODO: listen to configuration changes/database reinit.
/// See also: ServerContainerBase.
/// </summary>
public class GameServerHostedServiceWrapper : IHostedService
{
    private readonly IGameServer _gameServer;
    private readonly GameServerInitializer _initializer;
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameServerHostedServiceWrapper"/> class.
    /// </summary>
    /// <param name="gameServer">The game server.</param>
    /// <param name="initializer">The initializer.</param>
    public GameServerHostedServiceWrapper(IGameServer gameServer, GameServerInitializer initializer)
    {
        this._gameServer = gameServer;
        this._initializer = initializer;
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!this._isInitialized)
        {
            await this._initializer.InitializeAsync();
            this._isInitialized = true;
        }

        await this._gameServer.StartAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return this._gameServer.StopAsync(cancellationToken);
    }
}