// <copyright file="ServerContainerBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Startup;

using System.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// Base class for a server container, which reacts on database recreations.
/// </summary>
public abstract class ServerContainerBase : IHostedService
{
    private readonly SetupService _setupService;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerContainerBase"/> class.
    /// </summary>
    /// <param name="setupService">The setup service.</param>
    /// <param name="logger">The logger.</param>
    protected ServerContainerBase(SetupService setupService, ILogger logger)
    {
        this._setupService = setupService;
        this._logger = logger;
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await this.StartInnerAsync(cancellationToken).ConfigureAwait(false);
        this._setupService.DatabaseInitialized += this.OnDatabaseInitializedAsync;
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await this.StopInnerAsync(cancellationToken).ConfigureAwait(false);
        this._setupService.DatabaseInitialized -= this.OnDatabaseInitializedAsync;
    }

    /// <summary>
    /// Starts the hosted service.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected abstract Task StartInnerAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Stops the hosted service.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected abstract Task StopInnerAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Starts the listeners of the hosted service.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected abstract Task StartListenersAsync(CancellationToken cancellationToken);

    private async ValueTask OnDatabaseInitializedAsync()
    {
        try
        {
            await this.StopAsync(default).ConfigureAwait(false);
            await this.StartAsync(default).ConfigureAwait(false);
            await this.StartListenersAsync(default).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            this._logger.LogError(exception, "Unexpected error when handling database creation.");
        }
    }
}