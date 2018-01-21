// <copyright file="LogModule.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using Nancy;

    /// <summary>
    /// <see cref="NancyModule"/> for the log view.
    /// </summary>
    public class LogModule : NancyModule
    {
        private readonly dynamic model = new ExpandoObject();

        /// <summary>
        /// Initializes a new instance of the <see cref="LogModule"/> class.
        /// </summary>
        public LogModule()
            : base("/admin/log")
        {
            this.Get["/"] = _ => this.View["log", this.model];
            this.Get["loggers"] = this.GetLoggers;
        }

        private IEnumerable<string> GetLoggers(dynamic parameters)
        {
            return log4net.LogManager.GetCurrentLoggers().Select(log => log.Logger.Name);
        }
    }
}