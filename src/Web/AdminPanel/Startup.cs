// <copyright file="Startup.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel;

using System.IO;
using System.Globalization;
using Blazored.Modal;
using Blazored.Toast;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Web.AdminPanel.Models;
using MUnique.OpenMU.Web.AdminPanel.Services;
using MUnique.OpenMU.Web.AdminPanel.Localization;

/// <summary>
/// The startup class for the blazor app.
/// </summary>
/// <remarks>
/// This class is only used when running as all-in-one deployment.
/// </remarks>
public class Startup
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Startup"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public Startup(IConfiguration configuration)
    {
        this.Configuration = configuration;
    }

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    /// <value>
    /// The configuration.
    /// </value>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// This method gets called by the runtime. Use this method to add services to the container.
    /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddServerSideBlazor();

        services.AddLocalization();
        // Prefer the Startup registration; only add if missing.
        Microsoft.Extensions.DependencyInjection.Extensions.ServiceCollectionDescriptorExtensions.TryAddSingleton(
            services,
            provider =>
            {
                var env = provider.GetRequiredService<IWebHostEnvironment>();
                var options = new MUnique.OpenMU.Localization.LocalizationOptions
                {
                    ResourceDirectory = Path.Combine(env.ContentRootPath, "Localization"),
                };
                return new MUnique.OpenMU.Localization.LocalizationService(options);
            });
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var cultureNames = new[] { "en-US", "es-ES" };
            var supportedCultures = new List<CultureInfo>();
            foreach (var name in cultureNames)
            {
                try
                {
                    supportedCultures.Add(new CultureInfo(name));
                }
                catch (CultureNotFoundException)
                {
                    // Globalization-invariant mode or culture not available; skip.
                }
            }

            if (supportedCultures.Count == 0)
            {
                supportedCultures.Add(CultureInfo.InvariantCulture);
            }

            options.DefaultRequestCulture = new RequestCulture("en-US");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });

        services.AddSignalR().AddJsonProtocol(o => o.PayloadSerializerOptions.Converters.Add(new TimeSpanConverter()));

        services.AddControllers()
            .ConfigureApplicationPartManager(setup =>
                setup.FeatureProviders.Add(new GenericControllerFeatureProvider()));

        services.AddBlazoredModal();
        services.AddBlazoredToast();
        services.AddScoped<AccountService>();
        services.AddScoped<IDataService<Account>>(serviceProvider => serviceProvider.GetService<AccountService>()!);

        services.AddScoped<PlugInController>();
        services.AddScoped<IDataService<PlugInConfigurationViewItem>>(serviceProvider => serviceProvider.GetService<PlugInController>()!);

        services.AddSingleton<ILookupController, PersistentObjectsLookupController>();

        services.AddScoped<IChangeNotificationService, ChangeNotificationService>();
    }

    /// <summary>
    /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    /// </summary>
    /// <param name="app">The app builder.</param>
    /// <param name="env">The web host environment.</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");

            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        // Localization (fallbacks to InvariantCulture if globalization-invariant)
        var locOptions = app.ApplicationServices.GetRequiredService<Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>().Value;
        CultureInfo.DefaultThreadCurrentCulture = locOptions.DefaultRequestCulture.Culture;
        CultureInfo.DefaultThreadCurrentUICulture = locOptions.DefaultRequestCulture.UICulture;
        app.UseRequestLocalization(locOptions);

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "logs")),
            RequestPath = "/logs",
        });

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapBlazorHub();
            endpoints.MapControllers();
            endpoints.MapFallbackToPage("/_Host");
        });
    }
}
