// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.CentralServer.Host;
using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.Dapr.Common.HealthChecks;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.FriendServer;
using MUnique.OpenMU.GuildServer;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;
using ChatServer = MUnique.OpenMU.ChatServer.ChatServer;
using FriendServer = MUnique.OpenMU.FriendServer.FriendServer;
using GuildServer = MUnique.OpenMU.GuildServer.GuildServer;

// The central server hosts the login, guild, friend, chat and connect servers of the distributed deployment.
// They all exist just once per deployment and keep their state in memory, so they wouldn't scale
// with additional instances anyway - but they would cost a process and a dapr sidecar each.
var plugInConfigurations = new List<PlugInConfiguration>();
var builder = DaprService.CreateBuilder("CentralServer", args);

var services = builder.Services;
services.AddPeristenceProvider()
    .AddDatabaseHealthCheck()
    .AddPlugInManager(plugInConfigurations)
    .AddIpResolver(args)
    .AddSingleton<GameServerRegistry>();

// Login server
services.AddSingleton<PersistentLoginServer>()
    .AddHostedService<LoginStateCleanup>();

// Guild server
services.AddSingleton<IGuildServer, GuildServer>()
    .AddSingleton<IGuildChangePublisher, GuildChangePublisher>();

// Friend server
services.AddSingleton<IFriendServer, FriendServer>()
    .AddSingleton<IFriendNotifier, FriendNotifier>();

// Chat server
services.AddSingleton<ChatServer>()
    .AddSingleton<IChatServer>(s => s.GetRequiredService<ChatServer>())
    .AddPersistentSingleton<ChatServerDefinition>()
    .AddHostedService<ChatServerHostedServiceWrapper>()
    .PublishManageableServer<ChatServer>();

// Connect servers, one for each connect server definition, i.e. for each client version
services.AddSingleton<ConnectServerCollection>()
    .AddHostedService<ConnectServerHostedServiceWrapper>()
    .AddHostedService<ConnectServerListUpdater>()
    .PublishManageableServers<ConnectServerCollection>();

var metricsRegistry = new MetricsRegistry();
metricsRegistry.AddNetworkMeters();
builder.AddOpenTelemetryMetrics(metricsRegistry);

var app = builder.BuildAndConfigure();
await app.WaitForDatabaseConnectionInitializationAsync().ConfigureAwait(false);

app.Run();
