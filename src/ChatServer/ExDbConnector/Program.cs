// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer.ExDbConnector;

using System.ComponentModel.Design;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog;
using Serilog.Debugging;
using MUnique.OpenMU.ChatServer;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The main entry class of the application.
/// </summary>
internal class Program
{
    private static ILogger<Program> _logger = NullLogger<Program>.Instance;

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    /// <param name="args">The arguments. </param>
    internal static async Task Main(string[] args)
    {
        SelfLog.Enable(Console.Error);
        var logConfiguration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(logConfiguration)
            .CreateLogger();

        var loggerFactory = new LoggerFactory().AddSerilog(logger);
        _logger = loggerFactory.CreateLogger<Program>();

        var addressResolver = IpAddressResolverFactory.CreateIpResolver(args, null, loggerFactory);
        var settings = new Settings("ChatServer.cfg");
        var serviceContainer = new ServiceContainer();
        serviceContainer.AddService(typeof(ILoggerFactory), loggerFactory);

        int chatServerListenerPort = settings.ChatServerListenerPort ?? 55980;
        int exDbPort = settings.ExDbPort ?? 55906;
        string exDbHost = settings.ExDbHost ?? "127.0.0.1";

        try
        {
            // To make the chat server use our configured encryption key, we need to trick a bit. We add an endpoint with a special client version which is defined in the plugin.
            var configuration = new ChatServerSettings();
            configuration.Endpoints.Add(new ChatServerEndpoint { ClientVersion = ConfigurableNetworkEncryptionPlugIn.Version, NetworkPort = chatServerListenerPort });
            var pluginManager = new PlugInManager(null, loggerFactory, serviceContainer, null);
            pluginManager.DiscoverAndRegisterPlugInsOf<INetworkEncryptionFactoryPlugIn>();
            var chatServer = new ChatServer(addressResolver, loggerFactory, pluginManager);
            chatServer.Initialize(configuration);
            await chatServer.StartAsync().ConfigureAwait(false);
            var exDbClient = new ExDbClient(exDbHost, exDbPort, chatServer, chatServerListenerPort, loggerFactory);
            _logger.LogInformation("ChatServer started and ready");
            while (Console.ReadLine() != "exit")
            {
                // keep application running
            }

            await exDbClient.DisconnectAsync().ConfigureAwait(false);
            await chatServer.ShutdownAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unexpected error occured");
        }
    }
}