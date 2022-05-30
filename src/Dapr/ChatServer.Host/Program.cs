using System.Net;
using Microsoft.Extensions.DependencyInjection;

using MUnique.OpenMU.ChatServer;
using MUnique.OpenMU.ChatServer.Host;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Dapr.Common;

var builder = DaprService.CreateBuilder("ChatServer", args);

// Add services to the container.
var services = builder.Services;
services.AddSingleton<ChatServer>()
    .AddPeristenceProvider() // todo: Config API instead of using persistence?
    .AddPlugInManager()
    .AddCustomIpResover(new IPAddress(0x7F7F7F7F)) // todo; based on config?
    .AddPersistentSingleton<ChatServerDefinition>();

services.AddHostedService(p =>
{
    var settings = p.GetService<ChatServerDefinition>()?.ConvertToSettings() ?? throw new Exception($"{nameof(ChatServerSettings)} not registered.");
    var chatServer = p.GetService<ChatServer>()!;
    chatServer.Initialize(settings);
    return chatServer;
});

services.PublishManageableServer<ChatServer>();

var metricsRegistry = new MetricsRegistry();
metricsRegistry.AddNetworkMeters();
builder.AddOpenTelemetryMetrics(metricsRegistry);

var app = builder.BuildAndConfigure();
await app.WaitForUpdatedDatabase();
app.Run();
