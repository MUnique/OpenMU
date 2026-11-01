// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.AdminPanel.Host;
using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.EntityFramework.AdminAuth;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.ServerClients;
using MUnique.OpenMU.Telemetry;
using MUnique.OpenMU.Web.AdminPanel;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Mcp;

var builder = DaprService.CreateBuilder("AdminPanel", args);

var plugInConfigurations = new List<PlugInConfiguration>();

var services = builder.Services;

services.AddPeristenceProvider(true)
    .AddPlugInManager(plugInConfigurations)
    .AddManageableServerRegistry()
    .AddTransient<IDictionary<int, IGameServer>>(s => s.GetRequiredService<IServerProvider>().Servers.OfType<IGameServer>().ToDictionary(server => server.Id))
    .AddSingleton<ILoginServer, LoginServer>()

    // E.g. to notify the Discord bot when an administrator removes the link of an account.
    .AddSingleton<IEventPublisher, EventPublisher>()
    .AddSingleton<IGameServerInstanceManager, DockerGameServerInstanceManager>()
    .AddSingleton<IConnectServerInstanceManager, DockerConnectServerInstanceManager>()
    .AddSingleton<IBackupService, BackupService>()
    .AddSingleton<IDatabaseSnapshotService, DatabaseSnapshotService>()
    .AddAdminUserRepository();

builder.AddAdminPanel();
builder.Services.AddAdminMcp(builder.Configuration);

var metricsRegistry = new MetricsRegistry();

// todo: add some meaningful metrics
builder.AddOpenTelemetryMetrics(metricsRegistry);

var app = builder.BuildAndConfigure(false);
app.UseStaticFiles();
app.UseRouting();
app.UseAdminMcpProtection();
app.UseAdminPanelAuth();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<MUnique.OpenMU.Web.AdminPanel.Components.App>()
    .AddInteractiveServerRenderMode();
app.MapAdminPanelAuthEndpoints();
app.MapAdminMcp();

await app.WaitForDatabaseConnectionInitializationAsync().ConfigureAwait(false);

await app.Services.TryLoadPlugInConfigurationsAsync(plugInConfigurations).ConfigureAwait(false);

app.Run();
