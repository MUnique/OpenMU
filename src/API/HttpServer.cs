namespace MUnique.OpenMU.API
{
    using System.IO;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;

    public class HttpServer
    {
        public void Start()
        {

            var configuration = new ConfigurationBuilder()
           .AddJsonFile("appsettings-api.json", optional: false, reloadOnChange: true)
           .Build();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseConfiguration(configuration)
            .UseUrls("http://localhost:5000")
            .UseStartup<Startup>()
            .Build();

            _ = host.RunAsync();
        }
    }
}
