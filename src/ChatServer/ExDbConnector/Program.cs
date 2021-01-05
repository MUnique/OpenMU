// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer.ExDbConnector
{
    using System;
    using System.ComponentModel.Design;
    using System.IO;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using MUnique.OpenMU.ChatServer;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The main entry class of the application.
    /// </summary>
    internal class Program
    {
        private static readonly string Log4NetConfigFilePath = Directory.GetCurrentDirectory() +
                                                               Path.DirectorySeparatorChar +
                                                               typeof(Program).Assembly.GetName().Name +
                                                               ".exe.log4net.xml";

        private static ILogger<Program> logger = NullLogger<Program>.Instance;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The arguments. </param>
        internal static void Main(string[] args)
        {
            // todo: create with HostBuilder
            var loggerFactory = new NullLoggerFactory().AddLog4Net(Log4NetConfigFilePath, true);
            logger = loggerFactory.CreateLogger<Program>();
            var addressResolver = IpAddressResolverFactory.DetermineIpResolver(args, loggerFactory);
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
                var pluginManager = new PlugInManager(null, loggerFactory, serviceContainer);
                pluginManager.DiscoverAndRegisterPlugInsOf<INetworkEncryptionFactoryPlugIn>();
                var chatServer = new ChatServer(configuration, addressResolver, loggerFactory, pluginManager);

                chatServer.Start();
                var exDbClient = new ExDbClient(exDbHost, exDbPort, chatServer, chatServerListenerPort, loggerFactory);
                logger.LogInformation("ChatServer started and ready");
                while (Console.ReadLine() != "exit")
                {
                    // keep application running
                }

                exDbClient.Disconnect();
                chatServer.Shutdown();
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Unexpected error occured");
            }
        }
    }
}
