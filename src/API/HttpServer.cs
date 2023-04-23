namespace MUnique.OpenMU.API
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Server
    /// </summary>
    public class HttpServer
    {
        private IDictionary<int, IGameServer> _gameServers = new Dictionary<int, IGameServer>();
        public void Start()
        {
            var configuration = new ConfigurationBuilder()
           .AddJsonFile("appsettings-api.json", optional: false, reloadOnChange: true)
           .Build();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseConfiguration(configuration)
            .UseUrls("http://localhost:5000")
            .ConfigureServices(services =>
            {
                services.AddSingleton(this._gameServers);
                services.AddSingleton(this._gameServers.Values);
            })
            .UseStartup<Startup>()
            .Build();

            _ = host.RunAsync();
        }

        /// <summary>
        /// Set list
        /// </summary>
        /// <param name="list"></param>
        public void SetServers(IDictionary<int, IGameServer> list)
        {
            _gameServers = list;
        }
    }
}
