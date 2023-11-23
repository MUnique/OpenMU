// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.LoginServer.Host;

var builder = DaprService.CreateBuilder("LoginServer", args);

// Add services to the container.
builder.Services
    .AddSingleton<PersistentLoginServer>()
    .AddHostedService<LoginStateCleanup>()
    .AddSingleton<GameServerRegistry>();

var metricsRegistry = new MetricsRegistry();
// todo: add some meaningful metrics
builder.AddOpenTelemetryMetrics(metricsRegistry);

var app = builder.BuildAndConfigure();

app.Run();
