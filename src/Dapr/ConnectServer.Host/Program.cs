using Microsoft.Extensions.DependencyInjection;

using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ConnectServer;
using MUnique.OpenMU.ConnectServer.Host;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Dapr.Common;

var builder = DaprService.CreateBuilder("ConnectServer", args);

// Add services to the container.
var services = builder.Services;
services.AddSingleton<ConnectServer>()
    .AddSingleton<GameServerRegistry>()
    .AddSingleton<IConnectServer>(s => s.GetService<ConnectServer>()!)
    .AddPeristenceProvider()
    .AddPersistentSingleton<IConnectServerSettings, ConnectServerDefinition>()
    .AddHostedService<ConnectServerHostedServiceWrapper>();

var app = builder.BuildAndConfigure();
await app.WaitForUpdatedDatabase();

app.Run();
