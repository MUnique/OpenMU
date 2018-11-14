// <copyright file="DataController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Controllers
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.Persistence;
    using MUnique.OpenMU.Persistence.Json;

    /// <summary>
    /// Controller to retrieve configuration data.
    /// </summary>
    [Route("admin/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly IPersistenceContextProvider persistenceContextProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataController"/> class.
        /// </summary>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        public DataController(IPersistenceContextProvider persistenceContextProvider)
        {
            this.persistenceContextProvider = persistenceContextProvider;
        }

        /// <summary>
        /// Gets the default configuration as json string.
        /// </summary>
        /// <returns>The default configuration as json string.</returns>
        [HttpGet("gameconfiguration.json")]
        public IActionResult GetDefaultConfiguration()
        {
            using (var context = this.persistenceContextProvider.CreateNewContext())
            {
                var firstConfiguration = context.Get<GameConfiguration>().First();
                if (firstConfiguration is IConvertibleTo<Persistence.BasicModel.GameConfiguration> convertibleTo)
                {
                    return this.Content(convertibleTo.Convert().ToJson(), "application/json");
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the configuration with the specified id as json string.
        /// </summary>
        /// <param name="configurationId">The identifier of the requested configuration.</param>
        /// <returns>
        /// The requested configuration as json string.
        /// </returns>
        [HttpGet("{configurationId}/gameconfiguration.json")]
        public IActionResult GetConfigurationById(Guid configurationId)
        {
            using (var context = this.persistenceContextProvider.CreateNewContext())
            {
                var firstConfiguration = context.GetById<GameConfiguration>(configurationId);
                if (firstConfiguration is IConvertibleTo<Persistence.BasicModel.GameConfiguration> convertibleTo)
                {
                    return this.Content(convertibleTo.Convert().ToJson(), "application/json");
                }

                return null;
            }
        }
    }
}
