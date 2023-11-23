// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.DependencyInjection;

using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.GuildServer;
using MUnique.OpenMU.GuildServer.Host;
using MUnique.OpenMU.Interfaces;

var builder = DaprService.CreateBuilder("GuildServer", args);

// Add services to the container.
var services = builder.Services;
services.AddSingleton<IGuildServer, GuildServer>()
    .AddSingleton<IGuildChangePublisher, GuildChangePublisher>()
    .AddPeristenceProvider();

var metricsRegistry = new MetricsRegistry();
// todo: add some meaningful metrics
builder.AddOpenTelemetryMetrics(metricsRegistry);

var app = builder.BuildAndConfigure();
await app.WaitForUpdatedDatabaseAsync().ConfigureAwait(false);

app.Run();
