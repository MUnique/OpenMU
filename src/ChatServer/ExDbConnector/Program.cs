﻿// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer.ExDbConnector
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Linq;
    using log4net;
    using log4net.Config;
    using MUnique.OpenMU.ChatServer;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// The main entry class of the application.
    /// </summary>
    internal static class Program
    {
        private static readonly string Log4NetConfigFilePath = Directory.GetCurrentDirectory() +
                                                               Path.DirectorySeparatorChar +
                                                               typeof(Program).Assembly.GetName().Name +
                                                               ".exe.log4net.xml";

        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The arguments. </param>
        internal static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.ConfigureAndWatch(logRepository, new FileInfo(Log4NetConfigFilePath));
            var addressResolver = args.Contains("-local") ? (IIpAddressResolver)new LocalIpResolver() : new PublicIpResolver();
            var settings = new Settings("ChatServer.cfg");

            int chatServerListenerPort = settings.ChatServerListenerPort ?? 55980;
            int exDbPort = settings.ExDbPort ?? 55906;
            string exDbHost = settings.ExDbHost ?? "127.0.0.1";
            byte[] customXor32Key = settings.Xor32Key;

            try
            {
                var chatServer = new ChatServerListener(chatServerListenerPort, null, addressResolver);
                chatServer.Xor32Key = customXor32Key ?? chatServer.Xor32Key;
                chatServer.Start();
                var exDbClient = new ExDbClient(exDbHost, exDbPort, chatServer, chatServerListenerPort);
                Log.Info("ChatServer started and ready");
                while (Console.ReadLine() != "exit")
                {
                    // keep application running
                }

                exDbClient.Disconnect();
                chatServer.Shutdown();
            }
            catch (Exception ex)
            {
                Log.Fatal("Unexpected error occured", ex);
            }
        }
    }
}
