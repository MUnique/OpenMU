// <copyright file="MyBootstrapper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System.Collections.Generic;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence;
    using Nancy;
    using Nancy.Conventions;
    using Nancy.Json;

    /// <summary>
    /// The nancy bootstrapper which holds all together.
    /// </summary>
    public class MyBootstrapper : DefaultNancyBootstrapper
    {
        private readonly IList<IManageableServer> servers;

        private readonly IPersistenceContextProvider persistenceContextProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyBootstrapper"/> class.
        /// </summary>
        /// <param name="servers">All manageable servers.</param>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        public MyBootstrapper(IList<IManageableServer> servers, IPersistenceContextProvider persistenceContextProvider)
        {
            this.servers = servers;
            this.persistenceContextProvider = persistenceContextProvider;

            JsonSettings.Converters.Add(new AttributeDictionaryConverter(this.persistenceContextProvider));
            JsonSettings.Converters.Add(new TimeSpanConverter());
        }

        /// <inheritdoc/>
        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);
        }

        /// <inheritdoc/>
        protected override void ConfigureApplicationContainer(Nancy.TinyIoc.TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register(this.servers);
            container.Register(this.persistenceContextProvider);
        }
    }
}
