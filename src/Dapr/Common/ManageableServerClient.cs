// <copyright file="ManageableServerClient.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using global::Dapr.Client;
using MUnique.OpenMU.Interfaces;
using Nito.AsyncEx.Synchronous;


/// <summary>
/// A client to control a <seealso cref="IManageableServer"/>.
/// </summary>
/// <seealso cref="MUnique.OpenMU.Interfaces.IManageableServer" />
internal class ManageableServerClient : IManageableServer
{
    private readonly DaprClient _daprClient;

    private readonly string _targetAppId;
    private ServerState _serverState;
    private int _currentConnections;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManageableServerClient"/> class.
    /// </summary>
    /// <param name="daprClient">The dapr client.</param>
    /// <param name="serverData">The server data.</param>
    public ManageableServerClient(DaprClient daprClient, ServerStateData serverData)
    {
        this._daprClient = daprClient;
        this._targetAppId = serverData.AppId;
        this.Id = serverData.Id;
        this.Description = serverData.Description;
        this.ConfigurationId = serverData.ConfigurationId;
        this.CurrentConnections = serverData.CurrentConnections;
        this.MaximumConnections = serverData.MaximumConnections;
        this.ServerState = serverData.State;
        this.Type = serverData.Type;
        this.LastUpdate = DateTime.UtcNow;
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc/>
    public int Id { get; }

    /// <inheritdoc/>
    public Guid ConfigurationId { get; }

    /// <inheritdoc/>
    public string Description { get; }

    /// <inheritdoc/>
    public ServerType Type { get; }

    /// <inheritdoc/>
    public int MaximumConnections { get; }

    /// <summary>
    /// Gets the timestamp of the last update.
    /// </summary>
    public DateTime LastUpdate { get; private set; }

    /// <inheritdoc/>
    public ServerState ServerState
    {
        get => this._serverState;
        set
        {
            if (this._serverState == value)
            {
                return;
            }

            this._serverState = value;
            this.RaisePropertyChanged();
        }
    }

    /// <inheritdoc/>
    public int CurrentConnections
    {
        get => this._currentConnections;
        set
        {
            if (this._currentConnections == value)
            {
                return;
            }

            this._currentConnections = value;
            this.RaisePropertyChanged();
        }
    }

    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(IManageableServer.Start), cancellationToken);
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(IManageableServer.Shutdown), cancellationToken);
    }

    /// <inheritdoc/>
    public void Start()
    {
        this.StartAsync(default).WaitAndUnwrapException();
    }

    /// <inheritdoc/>
    public void Shutdown()
    {
        this.StopAsync(default).WaitAndUnwrapException();
    }

    /// <summary>
    /// Updates the specified server data.
    /// </summary>
    /// <param name="serverData">The server data.</param>
    public void Update(ServerStateData serverData)
    {
        this.ServerState = serverData.State;
        this.CurrentConnections = serverData.CurrentConnections;
        this.LastUpdate = DateTime.UtcNow;
    }

    private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}