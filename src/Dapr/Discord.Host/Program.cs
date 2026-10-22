// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.Discord;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.ServerClients;

var builder = DaprService.CreateBuilder("Discord", args);

var plugInConfigurations = new List<PlugInConfiguration>();
var settings = builder.Configuration.GetSection(DiscordSettings.SectionName).Get<DiscordSettings>() ?? new DiscordSettings();

builder.Services
    .AddPeristenceProvider()
    .AddPlugInManager(plugInConfigurations)
    .AddManageableServerRegistry()
    .AddSingleton<IFriendServer, FriendServer>()
    .AddSingleton<IGuildServer, GuildServer>()

    // The game servers of the distributed deployment run in UTC, too.
    .AddDiscord(settings, () => TimeZoneInfo.Utc);

var app = builder.BuildAndConfigure();

await app.WaitForDatabaseConnectionInitializationAsync().ConfigureAwait(false);
await app.Services.TryLoadPlugInConfigurationsAsync(plugInConfigurations).ConfigureAwait(false);

app.Run();
