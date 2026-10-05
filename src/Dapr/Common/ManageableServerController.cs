// <copyright file="ManageableServerController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

using Microsoft.AspNetCore.Mvc;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// An api controller to control a <see cref="IManageableServer"/>.
/// </summary>
[ApiController]
[Route("manageable-servers/{serverId:int}")]
public class ManageableServerController : ControllerBase
{
    private readonly IEnumerable<IManageableServer> _manageableServers;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManageableServerController"/> class.
    /// </summary>
    /// <param name="manageableServers">The manageable servers of this process.</param>
    public ManageableServerController(IEnumerable<IManageableServer> manageableServers)
    {
        this._manageableServers = manageableServers;
    }

    /// <summary>
    /// Shuts the manageable server down.
    /// </summary>
    /// <param name="serverId">The identifier of the server.</param>
    /// <returns>The result of the request.</returns>
    [HttpPost(nameof(IManageableServer.ShutdownAsync))]
    public async Task<IActionResult> ShutdownAsync(int serverId)
    {
        if (this._manageableServers.FirstOrDefault(server => server.Id == serverId) is not { } server)
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
        if (this._manageableServers.FirstOrDefault(server => server.Id == serverId) is not { } server)
        {
            return this.NotFound();
        }

        await server.StartAsync().ConfigureAwait(false);
        return this.NoContent();
    }
}
