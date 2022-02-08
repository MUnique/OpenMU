using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using Dapr.Client;
using MUnique.OpenMU.Interfaces;
using Nito.AsyncEx.Synchronous;

namespace MUnique.OpenMU.Dapr.Common;

internal class ManageableServerClient : IManageableServer
{
    private readonly DaprClient _daprClient;

    private readonly string _targetAppId;
    private ServerState _serverState;
    private int _currentConnections;

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

    public int Id { get; }
    public Guid ConfigurationId { get; }
    public string Description { get; }
    public ServerType Type { get; }

    public int MaximumConnections { get; }

    public DateTime LastUpdate { get; private set; }

    public ServerState ServerState
    {
        get => _serverState;
        set
        {
            if (_serverState == value)
            {
                return;
            }

            _serverState = value;
            this.RaisePropertyChanged();
        }
    }

    private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public int CurrentConnections
    {
        get => _currentConnections;
        set
        {
            if (this._currentConnections == value)
            {
                return;
            }

            _currentConnections = value;
            this.RaisePropertyChanged();
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(IManageableServer.Start), cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(IManageableServer.Shutdown), cancellationToken);
    }

    public void Start()
    {
        this.StartAsync(default).WaitAndUnwrapException();
    }

    public void Shutdown()
    {
        this.StopAsync(default).WaitAndUnwrapException();
    }

    public void Update(ServerStateData serverData)
    {
        this.ServerState = serverData.State;
        this.CurrentConnections = serverData.CurrentConnections;
        this.LastUpdate = DateTime.UtcNow;
    }
}