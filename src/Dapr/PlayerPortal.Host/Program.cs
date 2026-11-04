// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Web.PlayerPortal.Api;

var builder = DaprService.CreateBuilder("PlayerPortal", args);
builder.Services.AddPeristenceProvider()
    .AddSingleton<IDataSource<GameConfiguration>, GameConfigurationDataSource>();
builder.Services.AddPlayerPortalApi(builder.Configuration);
builder.Services.AddControllers().AddApplicationPart(typeof(PlayerPortalController).Assembly);

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.ConfigureDaprService();

await app.WaitForDatabaseConnectionInitializationAsync().ConfigureAwait(false);
await app.RunAsync().ConfigureAwait(false);
