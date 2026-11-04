// <copyright file="ManageableServerController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

using global::Dapr;
using Microsoft.AspNetCore.Mvc;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// An api controller to control a <see cref="IManageableServer"/>.
/// </summary>
[ApiController]
[Route("manageable-servers/{serverId:int}")]
public class ManageableServerController : ControllerBase
{
    /// <summary>
    /// The topic of the <see cref="ManageableServerCommandArguments"/>.
    /// </summary>
    public const string CommandTopic = "ManageableServerCommand";

    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManageableServerController"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider, which provides the manageable servers of this process.</param>
    public ManageableServerController(IServiceProvider serviceProvider)
    {
        this._serviceProvider = serviceProvider;
    }

    private IEnumerable<IManageableServer> ManageableServers => this._serviceProvider.GetManageableServers();

    /// <summary>
    /// Shuts the manageable server down.
    /// </summary>
    /// <param name="serverId">The identifier of the server.</param>
    /// <returns>The result of the request.</returns>
    [HttpPost(nameof(IManageableServer.ShutdownAsync))]
    public async Task<IActionResult> ShutdownAsync(int serverId)
    {
        if (this.ManageableServers.FirstOrDefault(server => server.Id == serverId) is not { } server)
        {
            return this.NotFound();
        }

        await server.ShutdownAsync().ConfigureAwait(false);
        return this.NoContent();
    }

    /// <summary>
    /// Starts the manageable server.
    /// </summary>
    /// <param name="serverId">The identifier of the server.</param>
    /// <returns>The result of the request.</returns>
    [HttpPost(nameof(IManageableServer.StartAsync))]
    public async Task<IActionResult> StartAsync(int serverId)
    {
        if (this.ManageableServers.FirstOrDefault(server => server.Id == serverId) is not { } server)
        {
            return this.NotFound();
        }

        await server.StartAsync().ConfigureAwait(false);
        return this.NoContent();
    }

    /// <summary>
    /// Handles a command which got published to all processes. It's executed when the server is hosted by this process.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>The result of the request.</returns>
    [HttpPost("~/manageable-servers/commands")]
    [Topic(DaprClientExtensions.PubSubName, CommandTopic)]
    public async Task<IActionResult> HandleCommandAsync([FromBody] ManageableServerCommandArguments command)
    {
        if (this.ManageableServers.FirstOrDefault(server => server.Id == command.ServerId) is not { } server)
        {
            // It's hosted by another process.
            return this.NoContent();
        }

        switch (command.Command)
        {
            case nameof(IManageableServer.StartAsync):
                await server.StartAsync().ConfigureAwait(false);
                break;
            case nameof(IManageableServer.ShutdownAsync):
                await server.ShutdownAsync().ConfigureAwait(false);
                break;
            default:
                return this.BadRequest();
        }

        return this.NoContent();
    }
}
