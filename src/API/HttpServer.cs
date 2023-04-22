using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MUnique.OpenMU.API
{
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
