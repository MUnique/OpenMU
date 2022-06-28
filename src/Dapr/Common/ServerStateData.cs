// <copyright file="ServerStateData.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

using System.Diagnostics;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Data about the state of a server.
/// </summary>
public class ServerStateData
{
    private readonly Stopwatch _stopwatch = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerStateData"/> class.
    /// </summary>
    public ServerStateData()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerStateData"/> class.
    /// </summary>
    /// <param name="server">The server.</param>
    /// <exception cref="System.InvalidOperationException">Add the environment variable 'APPID' with the app-id of this dapr app.</exception>
    public ServerStateData(IManageableServer server)
    {
        this.AppId = Environment.GetEnvironmentVariable("APPID") ?? throw new InvalidOperationException("Add the environment variable 'APPID' with the app-id of this dapr app.");
        this.Id = server.Id;
        this.Description = server.Description;
        this.ConfigurationId = server.ConfigurationId;
        this.Type = server.Type;
        this.MaximumConnections = server.MaximumConnections;
        this._stopwatch.Start();
        this.UpdateState(server);
    }

    /// <summary>
    /// Gets or sets the (dapr) application identifier.
    /// </summary>
    public string AppId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the server.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the description of the server.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the configuration of the server.
    /// </summary>
    public Guid ConfigurationId { get; set; }

    /// <summary>
    /// Gets or sets the type of the server.
    /// </summary>
    public ServerType Type { get; set; }

    /// <summary>
    /// Gets or sets the state of the server.
    /// </summary>
    public ServerState State { get; set; }

    /// <summary>
    /// Gets or sets the current connection count of the server.
    /// </summary>
    public int CurrentConnections { get; set; }

    /// <summary>
    /// Gets or sets the maximum connection count which the server supports.
    /// </summary>
    public int MaximumConnections { get; set; }

    /// <summary>
    /// Gets or sets the up time of the server.
    /// </summary>
    public TimeSpan UpTime { get; set; }

    /// <summary>
    /// Updates the state of the server.
    /// </summary>
    /// <param name="server">The server.</param>
    public void UpdateState(IManageableServer server)
    {
        this.State = server.ServerState;
        this.CurrentConnections = server.CurrentConnections;
        this.UpTime = this._stopwatch.Elapsed;
    }
}