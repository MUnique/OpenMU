// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.AdminPanel.Host;
using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.ServerClients;
using MUnique.OpenMU.Persistence.EntityFramework.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel;
using MUnique.OpenMU.Web.AdminPanel.Auth;

var builder = DaprService.CreateBuilder("AdminPanel", args);

var plugInConfigurations = new List<PlugInConfiguration>();

var services = builder.Services;

services.AddPeristenceProvider(true)
    .AddPlugInManager(plugInConfigurations)
    .AddManageableServerRegistry()
    .AddSingleton<ILoginServer, LoginServer>()
    .AddSingleton<IGameServerInstanceManager, DockerGameServerInstanceManager>()
    .AddSingleton<IConnectServerInstanceManager, DockerConnectServerInstanceManager>()
    .AddAdminUserRepository();

builder.AddAdminPanel();

var metricsRegistry = new MetricsRegistry();

// todo: add some meaningful metrics
builder.AddOpenTelemetryMetrics(metricsRegistry);

var app = builder.BuildAndConfigure(false);
app.UseStaticFiles();
app.UseRouting();
app.UseAdminPanelAuth();
app.UseAntiforgery();
app.MapRazorComponents<MUnique.OpenMU.Web.AdminPanel.Components.App>()
    .AddInteractiveServerRenderMode();
app.MapAdminPanelAuthEndpoints();

await app.WaitForDatabaseConnectionInitializationAsync().ConfigureAwait(false);

await app.Services.TryLoadPlugInConfigurationsAsync(plugInConfigurations).ConfigureAwait(false);

app.Run();
