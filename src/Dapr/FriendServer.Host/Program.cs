using Microsoft.Extensions.DependencyInjection;

using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.FriendServer;
using MUnique.OpenMU.FriendServer.Host;
using MUnique.OpenMU.ServerClients;
using FriendServer = MUnique.OpenMU.FriendServer.FriendServer;

var builder = DaprService.CreateBuilder("FriendServer", args);

// Add services to the container.
var services = builder.Services;
services.AddSingleton<IFriendServer, FriendServer>()
    .AddSingleton<IChatServer, ChatServer>()
    .AddSingleton<IFriendNotifier, FriendNotifier>()
    .AddPeristenceProvider();

var app = builder.BuildAndConfigure();

await app.WaitForUpdatedDatabase();

app.Run();
