// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.DependencyInjection;

using MUnique.OpenMU.ChatServer.Host;
using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;
using ChatServer = MUnique.OpenMU.ChatServer.ChatServer;

var plugInConfigurations = new List<PlugInConfiguration>();
var builder = DaprService.CreateBuilder("ChatServer", args);

// Add services to the container.
var services = builder.Services;
services.AddSingleton<ChatServer>()
    .AddPeristenceProvider() // todo: Config API instead of using persistence?
    .AddPlugInManager(plugInConfigurations)
    .AddIpResolver(args)
    .AddPersistentSingleton<ChatServerDefinition>();

services.AddHostedService<ChatServerHostedServiceWrapper>();

services.PublishManageableServer<ChatServer>();

var metricsRegistry = new MetricsRegistry();
metricsRegistry.AddNetworkMeters();
builder.AddOpenTelemetryMetrics(metricsRegistry);

var app = builder.BuildAndConfigure();
await app.WaitForDatabaseConnectionInitializationAsync().ConfigureAwait(false);

app.Run();
