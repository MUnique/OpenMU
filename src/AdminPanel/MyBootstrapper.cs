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

        private readonly IRepositoryManager repositoryManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyBootstrapper"/> class.
        /// </summary>
        /// <param name="servers">All manageable servers.</param>
        /// <param name="repositoryManager">The repository manager.</param>
        public MyBootstrapper(IList<IManageableServer> servers, IRepositoryManager repositoryManager)
        {
            this.servers = servers;
            this.repositoryManager = repositoryManager;

            JsonSettings.Converters.Add(new AttributeDictionaryConverter(this.repositoryManager));
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
            container.Register(this.repositoryManager);
        }
    }
}
