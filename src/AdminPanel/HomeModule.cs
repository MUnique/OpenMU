// <copyright file="HomeModule.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System.Dynamic;
    using Nancy;

    /// <summary>
    /// <see cref="NancyModule"/> for the home module, shows the entry point (index page) of the admin panel.
    /// </summary>
    public class HomeModule : NancyModule
    {
        private readonly dynamic model = new ExpandoObject();

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeModule"/> class.
        /// </summary>
        public HomeModule()
            : base("/admin")
        {
            this.Get["/"] = _ => this.View["index", this.model];
        }
    }
}