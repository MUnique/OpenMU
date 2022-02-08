using System.Diagnostics;

namespace MUnique.OpenMU.Dapr.Common;

using MUnique.OpenMU.Interfaces;


public class ServerStateData
{
    private Stopwatch _stopwatch = new ();

    public ServerStateData()
    {
    }

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

    public string AppId { get; set; } = string.Empty;

    public int Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public Guid ConfigurationId { get; set; }

    public ServerType Type { get; set; }

    public ServerState State { get; set; }

    public int CurrentConnections { get; set; }

    public int MaximumConnections { get; set; }

    public TimeSpan UpTime { get; set; }

    public void UpdateState(IManageableServer server)
    {
        this.State = server.ServerState;
        this.CurrentConnections = server.CurrentConnections;
        this.UpTime = this._stopwatch.Elapsed;
    }
}