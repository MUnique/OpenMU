// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.FriendServer;
using MUnique.OpenMU.FriendServer.Host;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;
using FriendServer = MUnique.OpenMU.FriendServer.FriendServer;

var builder = DaprService.CreateBuilder("FriendServer", args);

// Add services to the container.
var services = builder.Services;
services.AddSingleton<IFriendServer, FriendServer>()
    .AddSingleton<IChatServer, ChatServer>()
    .AddSingleton<IFriendNotifier, FriendNotifier>()
    .AddPeristenceProvider();

var metricsRegistry = new MetricsRegistry();
// todo: add some meaningful metrics
builder.AddOpenTelemetryMetrics(metricsRegistry);

var app = builder.BuildAndConfigure();

await app.WaitForUpdatedDatabaseAsync().ConfigureAwait(false);

app.Run();
