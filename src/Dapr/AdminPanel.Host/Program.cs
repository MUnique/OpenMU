// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Web.AdminPanel;

var builder = DaprService.CreateBuilder("AdminPanel", args);

// Add services to the container.
var services = builder.Services;

services.AddPeristenceProvider(true)
    .AddPlugInManager()
    .AddSingleton(new AdminPanelSettings(80))
    .AddSingleton<IList<IManageableServer>>(c => new List<IManageableServer>()); // TODO: discover all chat, connect and game servers

builder.AddAdminPanel();

var app = builder.BuildAndConfigure(true);
await app.WaitForUpdatedDatabase();
app.Run();
