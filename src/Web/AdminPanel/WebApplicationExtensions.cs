namespace MUnique.OpenMU.Web.AdminPanel;

using Blazored.Modal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.Web.AdminPanel;
using MUnique.OpenMU.Web.AdminPanel.Models;
using MUnique.OpenMU.Web.AdminPanel.Services;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Adds the map application to the web app.
    /// When using the DaprService, call the BuildAndConfigure-Method with the parameter to add Blazor.
    /// </summary>
    /// <param name="builder">The web application builder which should be configured.</param>
    /// <returns>The web application builder.</returns>
    public static WebApplicationBuilder AddAdminPanel(this WebApplicationBuilder builder)
    {
        // Ensure that DataInitialization plugins will get collected - for the setup functionality.
        _ = DataInitialization.Id;

        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();

        var services = builder.Services;
        services.AddControllers()
            .ConfigureApplicationPartManager(setup =>
                setup.FeatureProviders.Add(new GenericControllerFeatureProvider()));

        services.AddBlazoredModal();

        // services.AddSingleton<ServerService>();
        services.AddSingleton<ILookupController, PersistentObjectsLookupController>();
        services.AddSingleton<SetupService>();

        services.AddScoped<AccountService>();
        services.AddScoped<IDataService<Account>>(serviceProvider => serviceProvider.GetService<AccountService>()!);
        services.AddScoped<PlugInController>();
        services.AddScoped<IDataService<PlugInConfigurationViewItem>>(serviceProvider => serviceProvider.GetService<PlugInController>()!);

        services.AddScoped<IChangeNotificationService, ChangeNotificationService>();

        services.AddScoped(provider =>
        {
            var contextProvider = provider.GetService<IPersistenceContextProvider>();
            using var initialContext = contextProvider!.CreateNewConfigurationContext();
            return initialContext.Get<GameConfiguration>().First();
        });

        services.AddTransient(provider =>
        {
            var contextProvider = provider.GetService<IPersistenceContextProvider>();
            return contextProvider!.CreateNewPlayerContext(provider.GetService<GameConfiguration>()!);
        });

        StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
        return builder;
    }
}