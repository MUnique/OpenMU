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
[Route("")]
public class ManageableServerController
{
    private readonly IManageableServer _manageableServer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManageableServerController"/> class.
    /// </summary>
    /// <param name="manageableServer">The manageable server.</param>
    public ManageableServerController(IManageableServer manageableServer)
    {
        this._manageableServer = manageableServer;
    }

    /// <summary>
    /// Shuts the manageable server down.
    /// </summary>
    [HttpPost(nameof(IManageableServer.ShutdownAsync))]
    public void Shutdown()
    {
        this._manageableServer.ShutdownAsync();
    }

    /// <summary>
    /// Starts the manageable server.
    /// </summary>
    [HttpPost(nameof(IManageableServer.StartAsync))]
    public void Start()
    {
        this._manageableServer.StartAsync();
    }
}