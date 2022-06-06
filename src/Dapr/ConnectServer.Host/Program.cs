// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.ConnectServer;
using MUnique.OpenMU.ConnectServer.Host;
using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;

var builder = DaprService.CreateBuilder("ConnectServer", args);

// Add services to the container.
var services = builder.Services;
services.AddSingleton<ConnectServer>()
    .AddSingleton<GameServerRegistry>()
    .AddSingleton<IConnectServer>(s => s.GetService<ConnectServer>()!)
    .AddPeristenceProvider()
    .AddPersistentSingleton<IConnectServerSettings, ConnectServerDefinition>()
    .AddHostedService<ConnectServerHostedServiceWrapper>()
    .PublishManageableServer<IConnectServer>();

var metricsRegistry = new MetricsRegistry();
metricsRegistry.AddNetworkMeters();
builder.AddOpenTelemetryMetrics(metricsRegistry);

var app = builder.BuildAndConfigure();
await app.WaitForUpdatedDatabase();

app.Run();
