// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.DependencyInjection;

using MUnique.OpenMU.ChatServer;
using MUnique.OpenMU.ChatServer.Host;
using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.PlugIns;

var plugInConfigurations = new List<PlugInConfiguration>();
var builder = DaprService.CreateBuilder("ChatServer", args);

// Add services to the container.
var services = builder.Services;
services.AddSingleton<ChatServer>()
    .AddPeristenceProvider() // todo: Config API instead of using persistence?
    .AddPlugInManager(plugInConfigurations)
    .AddIpResolver(args)
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

await app.WaitForUpdatedDatabaseAsync().ConfigureAwait(false);

await app.Services.TryLoadPlugInConfigurationsAsync(plugInConfigurations).ConfigureAwait(false);

app.Run();
