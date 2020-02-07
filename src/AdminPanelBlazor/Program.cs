// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanelBlazor
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// The class of the entry point.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection.AddSingleton<IList<IManageableServer>>(new List<IManageableServer>());
                    serviceCollection.AddSingleton<IPersistenceContextProvider>(new NullPersistenceContextProvider());
                    serviceCollection.AddSingleton<IServerConfigurationChangeListener>(new NullServerConfigurationChangeListener());
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .Build();
            host.Run();
        }

        private class NullServerConfigurationChangeListener : IServerConfigurationChangeListener
        {
            public void ConnectionServerAdded(ConnectServerDefinition currentConfiguration)
            {
            }

            public void ConnectionServerChanged(ConnectServerDefinition currentConfiguration)
            {
            }
        }

        private class NullPersistenceContextProvider : IPersistenceContextProvider
        {
            public IContext CreateNewContext()
            {
                throw new NotImplementedException();
            }

            public IContext CreateNewContext(GameConfiguration gameConfiguration)
            {
                throw new NotImplementedException();
            }

            public IContext CreateNewConfigurationContext()
            {
                throw new NotImplementedException();
            }

            public IContext CreateNewTradeContext()
            {
                throw new NotImplementedException();
            }

            public IPlayerContext CreateNewPlayerContext(GameConfiguration gameConfiguration)
            {
                throw new NotImplementedException();
            }

            public IFriendServerContext CreateNewFriendServerContext()
            {
                throw new NotImplementedException();
            }

            public IGuildServerContext CreateNewGuildContext()
            {
                throw new NotImplementedException();
            }
        }
    }
}
