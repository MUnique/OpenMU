// <copyright file="WebApplicationExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel;

using System.IO;
using Blazored.Modal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.Web.AdminPanel.Models;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// Extensions for the <see cref="WebApplicationBuilder"/>.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Adds the map application to the web app.
    /// When using the DaprService, call the BuildAndConfigure-Method with the parameter to add Blazor.
    /// </summary>
    /// <param name="builder">The web application builder which should be configured.</param>
    /// <param name="includeMapApp">If set to <c>true</c>, the map app is included.</param>
    /// <returns>
    /// The web application builder.
    /// </returns>
    public static WebApplicationBuilder AddAdminPanel(this WebApplicationBuilder builder, bool includeMapApp = false)
    {
        // Ensure that DataInitialization plugins will get collected - for the setup functionality.
        _ = DataInitialization.Id;

        var mvcBuilder = builder.Services.AddRazorPages();
        if (includeMapApp)
        {
            AdminPanelEnvironment.IsHostingEmbedded = true;
            mvcBuilder.AddApplicationPart(typeof(Map.Exports).Assembly);
        }

        builder.Services.AddServerSideBlazor();

        var services = builder.Services;
        services.AddControllers()
            .ConfigureApplicationPartManager(setup =>
                setup.FeatureProviders.Add(new GenericControllerFeatureProvider()));

        services.AddBlazoredModal();

        services.AddScoped<ILookupController, PersistentObjectsLookupController>();

        services.AddSingleton<IDataSource<GameConfiguration>, GameConfigurationDataSource>();
        services.AddSingleton<IDataSource<Account>, AccountDataSource>();
        services.AddScoped<SetupService>();
        services.AddScoped<DataUpdateService>();
        services.AddScoped<AccountService>();
        services.AddScoped<IDataService<Account>>(serviceProvider => serviceProvider.GetService<AccountService>()!);
        services.AddScoped<PlugInController>();
        services.AddScoped<IDataService<PlugInConfigurationViewItem>>(serviceProvider => serviceProvider.GetService<PlugInController>()!);
        services.AddScoped<IUserService, NginxHtpasswdFileUserService>();
        services.AddScoped<IChangeNotificationService, ChangeNotificationService>();

        services.AddScoped<LoggedInAccountService>();
        services.AddScoped<IDataService<LoggedInAccount>>(serviceProvider => serviceProvider.GetService<LoggedInAccountService>()!);

        StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
        return builder;
    }

    /// <summary>
    /// Configures the admin panel.
    /// </summary>
    /// <param name="app">The application.</param>
    /// <returns>The configured web application.</returns>
    public static WebApplication ConfigureAdminPanel(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseStaticFiles();
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "logs")),
            RequestPath = "/logs",
        });

        app.UseRouting();
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        app.MapControllers();

        AdminPanelEnvironment.IsHostingEmbedded = true;

        return app;
    }
}