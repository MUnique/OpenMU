// <copyright file="Startup.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using MUnique.Log4Net.CoreSignalR;
    using MUnique.OpenMU.AdminPanel.Hubs;

    /// <summary>
    /// The startup class for the admin panel web app.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940.
        /// </remarks>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new TimeSpanConverter()));
            services.AddRazorPages();
            services.AddSignalR().AddJsonProtocol(o => o.PayloadSerializerOptions.Converters.Add(new TimeSpanConverter()));
        }

        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        /// <remarks>This method gets called by the runtime. Use this method to configure the HTTP request pipeline.</remarks>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(opt =>
            {
                opt.MapHub<ServerListHub>("/signalr/hubs/serverListHub");
                opt.MapHub<WorldObserverHub>("/signalr/hubs/worldObserverHub");
                opt.MapHub<SystemHub>("/signalr/hubs/systemHub");
                opt.MapHub<LogHub>("/signalr/hubs/logHub");
                opt.MapControllers();
                opt.MapRazorPages();
            });
            app.UseStaticFiles();
        }
    }
}
