namespace MUnique.OpenMU.Dapr.Common;

using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.PlugIns;

public static class Extensions
{
    public static IServiceCollection AddPeristenceProvider(this IServiceCollection services, bool publishConfigChanges = false)
    {
        if (publishConfigChanges)
        {
            services.AddSingleton<IConfigurationChangePublisher, ConfigurationChangePublisher>();
        }
        else
        {
            services.AddSingleton(e => IConfigurationChangePublisher.None);
        }

        return services
            .AddSingleton<IPersistenceContextProvider, PersistenceContextProvider>()
            .AddSingleton(s => (PersistenceContextProvider)s.GetService<IPersistenceContextProvider>()!);
    }

    public static IServiceCollection AddPlugInManager(this IServiceCollection services)
    {
        return services
            .AddSingleton<ICollection<PlugInConfiguration>>(s => s.GetService<IPersistenceContextProvider>()?.CreateNewTypedContext<PlugInConfiguration>().Get<PlugInConfiguration>().ToList() ?? throw new Exception($"{nameof(IPersistenceContextProvider)} not registered."))
            .AddSingleton<PlugInManager>();
    }

    public static IServiceCollection AddCustomIpResover(this IServiceCollection services, IPAddress address)
    {
        return services.AddSingleton<IIpAddressResolver>(s => new CustomIpResolver(address));
    }

    public static IServiceCollection AddPersistentSingleton<T>(this IServiceCollection services, Func<T, bool>? predicate = null)
        where T : class
    {
        return services.AddPersistentSingleton<T, T>(predicate);
    }

    public static IServiceCollection AddPersistentSingleton<TTarget, TActual>(this IServiceCollection services, Func<TActual, bool>? predicate = null)
        where TActual : class, TTarget
        where TTarget : class
    {
        return services.AddSingleton(s =>
            (TTarget)s.GetService<IPersistenceContextProvider>()?.CreateNewConfigurationContext().Get<TActual>().First(predicate ?? (_ => true))!);
    }

    public static IServiceCollection AddManageableServerRegistry(this IServiceCollection services)
    {
        services.AddSingleton<ManagableServerRegistry>()
            .AddSingleton<IServerProvider>(s => s.GetService<ManagableServerRegistry>()!)
            .AddControllers().AddApplicationPart(typeof(ServerStateController).Assembly);
        return services;
    }

    public static IServiceCollection PublishManageableServer<TServer>(this IServiceCollection services)
        where TServer : IManageableServer
    {
        services.AddSingleton<IManageableServer>(s => s.GetService<TServer>()!)
            .AddHostedService<ManagableServerStatePublisher>()
            .AddControllers().AddApplicationPart(typeof(ManageableServerController).Assembly);

        return services;
    }

    public static WebApplication BuildAndConfigure(this WebApplicationBuilder builder, bool addBlazor = false)
    {
        var pathBase = Environment.GetEnvironmentVariable("PATH_BASE");
        var useReverseProxy = !string.IsNullOrWhiteSpace(pathBase);

        var app = builder.Build();
        app.ConfigureDaprService(addBlazor);

        if (useReverseProxy)
        {
            app.UsePathBase(pathBase!.TrimEnd('/'));
            app.UseStaticFiles();
        }

        return app;
    }

    public static WebApplication ConfigureDaprService(this WebApplication app, bool addBlazor = false)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        if (addBlazor)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
        }

        app.UseCloudEvents();
        app.MapControllers();
        app.MapSubscribeHandler();

        if (addBlazor)
        {
            var pathBase = Environment.GetEnvironmentVariable("PATH_BASE");
            var useReverseProxy = !string.IsNullOrWhiteSpace(pathBase);

            if (useReverseProxy)
            {
                app.MapBlazorHub(pathBase! + "_blazor");
            }
            else
            {
                app.MapBlazorHub();
            }

            // app.MapRazorPages();
            app.MapFallbackToPage("/_Host");
        }

        return app;
    }

    public static Task WaitForUpdatedDatabase(this WebApplication app)
    {
        return app.Services.GetService<PersistenceContextProvider>()!.WaitForUpdatedDatabase();
    }
}