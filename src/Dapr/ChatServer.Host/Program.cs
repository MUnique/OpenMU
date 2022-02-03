using System.Net;
using Microsoft.Extensions.DependencyInjection;

using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ChatServer;
using MUnique.OpenMU.ChatServer.Host;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Dapr.Common;

var builder = DaprService.CreateBuilder("ChatServer", args);

// Add services to the container.
var services = builder.Services;
services.AddSingleton<IChatServer, ChatServer>()
    .AddPeristenceProvider() // todo: Config API instead of using persistence?
    .AddPlugInManager()
    .AddSingleton(s => s.GetService<ChatServerDefinition>()?.ConvertToSettings() ?? throw new Exception($"{nameof(ChatServerSettings)} not registered."))
    .AddCustomIpResover(new IPAddress(0x7F7F7F7F)) // todo; based on config?
    .AddPersistentSingleton<ChatServerDefinition>();

services.AddHostedService(p => (ChatServer)p.GetService<IChatServer>()!);

var app = builder.BuildAndConfigure();
await app.WaitForUpdatedDatabase();
app.Run();
