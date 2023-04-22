// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Web.AdminPanel;

var builder = DaprService.CreateBuilder("AdminPanel", args);

var plugInConfigurations = new List<PlugInConfiguration>();

var services = builder.Services;

services.AddPeristenceProvider(true)
    .AddPlugInManager(plugInConfigurations)
    .AddManageableServerRegistry();

builder.AddAdminPanel();

var app = builder.BuildAndConfigure(true);

await app.WaitForDatabaseConnectionInitializationAsync().ConfigureAwait(false);

await app.Services.TryLoadPlugInConfigurationsAsync(plugInConfigurations).ConfigureAwait(false);

app.Run();
