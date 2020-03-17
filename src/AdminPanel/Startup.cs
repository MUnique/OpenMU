// <copyright file="Startup.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System.Linq;
    using Blazored.Modal;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using MUnique.Log4Net.CoreSignalR;
    using MUnique.OpenMU.AdminPanel.Map;
    using MUnique.OpenMU.AdminPanel.Models;
    using MUnique.OpenMU.AdminPanel.Services;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// The startup class for the blazor app.
    /// </summary>
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
            services.AddSignalR().AddJsonProtocol(o => o.PayloadSerializerOptions.Converters.Add(new TimeSpanConverter()));
            services.AddControllers()
                .ConfigureApplicationPartManager(setup =>
                    setup.FeatureProviders.Add(new GenericControllerFeatureProvider()));

            services.AddBlazoredModal();

            services.AddSingleton<ServerService>();
            services.AddSingleton<ConnectServerService>();

            services.AddScoped<AccountService>();
            services.AddScoped<IDataService<Account>>(serviceProvider => serviceProvider.GetService<AccountService>());

            services.AddScoped<PlugInController>();
            services.AddScoped<IDataService<PlugInConfigurationViewItem>>(serviceProvider => serviceProvider.GetService<PlugInController>());

            services.AddSingleton<LogService>();
            services.AddScoped<LogController>();

            services.AddScoped<IMapFactory, JavascriptMapFactory>();
            services.AddSingleton<ILookupController, PersistentObjectsLookupController>();

            services.AddScoped(provider =>
            {
                var contextProvider = provider.GetService<IPersistenceContextProvider>();
                using var initialContext = contextProvider.CreateNewConfigurationContext();
                return initialContext.Get<GameConfiguration>().First();
            });

            services.AddTransient(provider =>
            {
                var contextProvider = provider.GetService<IPersistenceContextProvider>();
                return contextProvider.CreateNewPlayerContext(provider.GetService<GameConfiguration>());
            });
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapControllers();
                endpoints.MapFallbackToPage("/_Host");

                endpoints.MapHub<LogHub>("/signalr/hubs/logHub");
            });
        }
    }
}
