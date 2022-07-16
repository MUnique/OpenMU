// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.Web.AdminPanel;
using SixLabors.ImageSharp;
using SixLabors.Memory;

Configuration.Default.MemoryAllocator = ArrayPoolMemoryAllocator.CreateWithMinimalPooling();
var builder = DaprService.CreateBuilder("AdminPanel", args);

var services = builder.Services;

services.AddPeristenceProvider(true)
    .AddPlugInManager()
    .AddManageableServerRegistry();

builder.AddAdminPanel();

var app = builder.BuildAndConfigure(true);

await app.WaitForDatabaseConnectionInitializationAsync().ConfigureAwait(false);

app.Run();
