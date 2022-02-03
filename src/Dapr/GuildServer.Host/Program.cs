using Microsoft.Extensions.DependencyInjection;

using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.GuildServer;
using MUnique.OpenMU.GuildServer.Host;
var builder = DaprService.CreateBuilder("GuildServer", args);

// Add services to the container.
var services = builder.Services;
services.AddSingleton<IGuildServer, GuildServer>()
    .AddSingleton<IGuildChangePublisher, GuildChangePublisher>()
    .AddPeristenceProvider();

var app = builder.BuildAndConfigure();
await app.WaitForUpdatedDatabase();

app.Run();
