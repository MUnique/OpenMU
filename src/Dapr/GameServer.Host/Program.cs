// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameServer.Host;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.ServerClients;
using MUnique.OpenMU.Web.Map;
using GameServer = MUnique.OpenMU.GameServer.GameServer;

// Ensure GameLogic and GameServer Assemblies are loaded
_ = MUnique.OpenMU.GameLogic.Rand.NextInt(1, 2);
_ = MUnique.OpenMU.GameServer.ClientVersionResolver.DefaultVersion;

var gameServerId = byte.Parse(Environment.GetEnvironmentVariable("GS_ID") ?? "0");
var serviceName = $"GameServer{gameServerId + 1}";
var builder = DaprService.CreateBuilder(serviceName, args);
var plugInConfigurations = new List<PlugInConfiguration>();

// Add services to the container.
var services = builder.Services;
services.AddSingleton<GameServer>()
    .AddSingleton<IGameServer>(s => s.GetService<GameServer>()!)
    .AddSingleton<IList<IManageableServer>>(s => new List<IManageableServer>() { s.GetService<GameServer>()! })
    .AddSingleton(s => s.GetService<GameServer>()!.Context)
    .AddSingleton<IGameServerStateObserver, GameServerStatePublisher>()
    .AddSingleton<ConfigurationChangeMediator>()
    .AddSingleton<IConfigurationChangeMediator>(s => s.GetRequiredService<ConfigurationChangeMediator>())
    .AddSingleton<IConfigurationChangeMediatorListener>(s => s.GetRequiredService<ConfigurationChangeMediator>())
    .AddSingleton<ILoginServer, LoginServer>()
    .AddSingleton<IGuildServer, GuildServer>()
    .AddSingleton<IEventPublisher, EventPublisher>()
    .AddSingleton<IFriendServer, FriendServer>()
    .AddSingleton<GameServerInitializer>()
    .AddSingleton<IObservableGameServer, ObservableGameServerAdapter>()
    .AddPersistentSingleton<GameServerDefinition>(def => def.ServerID == gameServerId)
    .AddPeristenceProvider()
    .AddPlugInManager(plugInConfigurations)
    .AddIpResolver(args)
    .AddHostedService<GameServerHostedServiceWrapper>()
    .PublishManageableServer<IGameServer>();

builder.AddMapApp();

var metricsRegistry = new MetricsRegistry();
metricsRegistry.AddNetworkMeters();
metricsRegistry.AddMeters(MUnique.OpenMU.GameLogic.Metrics.Meters);
builder.AddOpenTelemetryMetrics(metricsRegistry);

var app = builder.BuildAndConfigure(true);
app.UseStaticFiles();
await app.WaitForUpdatedDatabaseAsync().ConfigureAwait(false);

await app.Services.TryLoadPlugInConfigurationsAsync(plugInConfigurations).ConfigureAwait(false);
await ((ObservableGameServerAdapter)app.Services.GetRequiredService<IObservableGameServer>()).InitializeAsync().ConfigureAwait(false);
app.Run();
