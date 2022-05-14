// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.LoginServer.Host;

var builder = DaprService.CreateBuilder("LoginServer", args);

// Add services to the container.
builder.Services
    .AddSingleton<PersistentLoginServer>()
    .AddHostedService<LoginStateCleanup>()
    .AddSingleton<GameServerRegistry>()
    .AddManageableServerRegistry(); // todo: remove this line; somehow it picks up the ServerStateController, which is not used by the login server.

var app = builder.BuildAndConfigure();

app.Run();
