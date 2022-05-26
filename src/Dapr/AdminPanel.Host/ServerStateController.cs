// <copyright file="ServerStateController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Host;

using global::Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Dapr.Common;

/// <summary>
/// Controller which receives server state updates from the pub/sub component.
/// </summary>
[ApiController]
[Route("")]
public class ServerStateController
{
    private readonly ManagableServerRegistry _registry;
    private readonly ILogger<ServerStateController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerStateController"/> class.
    /// </summary>
    /// <param name="registry">The registry.</param>
    /// <param name="logger">The logger.</param>
    public ServerStateController(ManagableServerRegistry registry, ILogger<ServerStateController> logger)
    {
        this._registry = registry;
        this._logger = logger;
    }

    /// <summary>
    /// Handles the server state update.
    /// </summary>
    /// <param name="data">The data.</param>
    [HttpPost(ManagableServerStatePublisher.TopicName)]
    [Topic("pubsub", ManagableServerStatePublisher.TopicName)]
    public void ServerStateUpdate([FromBody] ServerStateData data)
    {
        try
        {
            this._registry.HandleUpdate(data);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error updating the ManagableServerRegistry");
        }
    }
}