// <copyright file="GameClientController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using log4net;
    using Microsoft.AspNetCore.Mvc;
    using MUnique.OpenMU.Persistence;
    using MUnique.OpenMU.Persistence.BasicModel;
    using MUnique.OpenMU.Persistence.Json;

    /// <summary>
    /// The controller for game clients in the admin panel.
    /// </summary>
    [Route("admin/[controller]")]
    [ApiController]
    public class GameClientController : ControllerBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GameClientController));
        private readonly IPersistenceContextProvider persistenceContextProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameClientController"/> class.
        /// </summary>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        public GameClientController(IPersistenceContextProvider persistenceContextProvider)
        {
            this.persistenceContextProvider = persistenceContextProvider;
        }

        /// <summary>
        /// Gets a list of all game client definitions.
        /// </summary>
        /// <returns>A list of all game client definitions.</returns>
        [HttpGet]
        public ActionResult<List<GameClientDefinition>> List()
        {
            try
            {
                Log.Info($"requested list of game client definitions.");
                using (var configContext = this.persistenceContextProvider.CreateNewConfigurationContext())
                {
                    var allClients = configContext.Get<DataModel.Configuration.GameClientDefinition>();
                    var result = allClients.OfType<IConvertibleTo<GameClientDefinition>>().Select(c => c.Convert()).ToList();
                    return this.Ok(result);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An unexpected exception occured during requesting all game client definitions", ex);
                throw;
            }
        }

        /// <summary>
        /// Saves the specified definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>The success.</returns>
        [HttpPost]
        public ActionResult<Guid> Save([FromBody]GameClientDefinition definition)
        {
            try
            {
                Log.Info($"requested list of game client definitions.");
                using (var configContext = this.persistenceContextProvider.CreateNewContext())
                {
                    var client = configContext.GetById<DataModel.Configuration.GameClientDefinition>(definition.Id)
                                 ?? configContext.CreateNew<DataModel.Configuration.GameClientDefinition>();

                    client.Season = definition.Season;
                    client.Episode = definition.Episode;
                    client.Description = definition.Description;
                    client.Language = definition.Language;
                    client.Serial = definition.Serial;
                    client.Version = definition.Version;
                    configContext.SaveChanges();
                    return this.Ok(client.GetId());
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An unexpected exception occured during saving a game client definition", ex);
                throw;
            }
        }
    }
}