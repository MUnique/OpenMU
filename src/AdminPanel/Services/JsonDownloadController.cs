// <copyright file="JsonDownloadController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Services
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using MUnique.OpenMU.Persistence;
    using MUnique.OpenMU.Persistence.Json;

    /// <summary>
    /// Controller to retrieve data as json.
    /// Supported types: <see cref="GenericControllerFeatureProvider.SupportedTypes"/>.
    /// </summary>
    /// <typeparam name="T">The source type of the data object.</typeparam>
    /// <typeparam name="TSerializable">The type of the serializable.</typeparam>
    [Route("download/[controller]")]
    [GenericControllerName]
    public class JsonDownloadController<T, TSerializable> : ControllerBase
        where T : class
        where TSerializable : class
    {
        private readonly IPersistenceContextProvider persistenceContextProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDownloadController{T,TSerializable}"/> class.
        /// </summary>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        public JsonDownloadController(IPersistenceContextProvider persistenceContextProvider)
        {
            this.persistenceContextProvider = persistenceContextProvider;
        }

        /// <summary>
        /// Gets the configuration with the specified id as json string.
        /// </summary>
        /// <param name="objectId">The identifier of the requested object.</param>
        /// <returns>
        /// The requested configuration as json string.
        /// </returns>
        [HttpGet("[controller]_{objectId}.json")]
        public IActionResult GetConfigurationById(Guid objectId)
        {
            using var context = this.persistenceContextProvider.CreateNewTypedContext<T>();
            var item = context.GetById<T>(objectId);
            if (item is IConvertibleTo<TSerializable> convertibleTo)
            {
                return this.Content(convertibleTo.Convert().ToJson(), "application/json");
            }

            return null;
        }
    }
}
