using System.Net;
using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameServer.Host;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;
using MUnique.OpenMU.Web.Map;
using GameServer = MUnique.OpenMU.GameServer.GameServer;

// Ensure GameLogic and GameServer Assemblies are loaded
_ = MUnique.OpenMU.GameLogic.Rand.NextInt(1, 2);
_ = MUnique.OpenMU.GameServer.ClientVersionResolver.DefaultVersion;

var gameServerId = byte.Parse(Environment.GetEnvironmentVariable("GS_ID") ?? "0");
var serviceName = $"GameServer{gameServerId + 1}";
var builder = DaprService.CreateBuilder(serviceName, args);

// Add services to the container.
var services = builder.Services;
services.AddSingleton<GameServer>()
    .AddSingleton<IGameServer>(s => s.GetService<GameServer>()!)
    .AddSingleton(s => s.GetService<GameServer>()!.Context)
    .AddSingleton<IGameServerStateObserver, GameServerStatePublisher>()
    .AddSingleton<ILoginServer, LoginServer>()
    .AddSingleton<IGuildServer, GuildServer>()
    .AddSingleton<IEventPublisher, EventPublisher>()
    .AddSingleton<IFriendServer, FriendServer>()
    .AddSingleton<GameServerInitializer>()
    .AddSingleton<IObservableGameServer, ObservableGameServerAdapter>()
    .AddPersistentSingleton<GameServerDefinition>(def => def.ServerID == gameServerId)
    .AddPeristenceProvider()
    .AddPlugInManager()
    .AddCustomIpResover(new IPAddress(0x7F7F7F7F)) // TODO: by config
    .AddHostedService<GameServerHostedServiceWrapper>()
    .PublishManageableServer<IGameServer>();

builder.AddMapApp();

var app = builder.BuildAndConfigure(true);

await app.WaitForUpdatedDatabase();

app.Run();
