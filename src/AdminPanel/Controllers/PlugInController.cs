// <copyright file="PlugInController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using log4net;
    using Microsoft.AspNetCore.Mvc;
    using MUnique.OpenMU.AdminPanel.Models;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.Persistence;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Controller for the plugin list.
    /// </summary>
    [Route("admin/[controller]/[action]")]
    public class PlugInController : ControllerBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PlugInController));
        private readonly IPersistenceContextProvider persistenceContextProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlugInController"/> class.
        /// </summary>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        public PlugInController(IPersistenceContextProvider persistenceContextProvider)
        {
            this.persistenceContextProvider = persistenceContextProvider;
        }

        /// <summary>
        /// Returns a slice of the plugin list, defined by an offset and a count.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns>A slice of the plugin list, defined by an offset and a count.</returns>
        [HttpGet("{offset}/{count}")]
        public ActionResult<List<PlugInConfigurationDto>> List(int offset, int count)
        {
            var allPlugIns = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.DefinedTypes.Where(type => type.GetCustomAttribute<PlugInAttribute>() != null))
                .ToDictionary(t => t.GUID, t => t);
            var skipped = 0;

            using (var context = this.persistenceContextProvider.CreateNewContext())
            {
                var result = new List<PlugInConfigurationDto>();
                foreach (var gameConfig in context.Get<GameConfiguration>())
                {
                    var rest = count - result.Count;
                    if (rest == 0)
                    {
                        break;
                    }

                    foreach (var plugInConfiguration in gameConfig.PlugInConfigurations.Skip(offset - skipped).Take(rest))
                    {
                        if (!allPlugIns.TryGetValue(plugInConfiguration.TypeId, out var plugInType))
                        {
                            continue;
                        }

                        var plugInAttribute = plugInType.GetCustomAttribute<PlugInAttribute>();
                        var plugInPoint = plugInType.GetInterfaces().FirstOrDefault(intf => intf.GetCustomAttribute<PlugInPointAttribute>() != null)?.GetCustomAttribute<PlugInPointAttribute>();
                        var customPlugInContainer = plugInType.GetInterfaces().FirstOrDefault(intf => intf.GetCustomAttribute<CustomPlugInContainerAttribute>() != null)?.GetCustomAttribute<CustomPlugInContainerAttribute>();
                        var customPlugInInterface = plugInType.GetInterfaces().FirstOrDefault(intf => intf.GetInterfaces().Any(i => i.GetCustomAttribute<CustomPlugInContainerAttribute>() != null));

                        var dto = new PlugInConfigurationDto
                        {
                            Id = plugInConfiguration.GetId(),
                            GameConfigurationId = gameConfig.GetId(),
                            CustomPlugInSource = plugInConfiguration.CustomPlugInSource,
                            ExternalAssemblyName = plugInConfiguration.ExternalAssemblyName,
                            IsActive = plugInConfiguration.IsActive,
                            TypeId = plugInConfiguration.TypeId,
                            TypeName = plugInType.FullName,
                            PlugInName = plugInAttribute?.Name,
                            PlugInDescription = plugInAttribute?.Description,
                        };

                        if (plugInPoint != null)
                        {
                            dto.PlugInPointName = plugInPoint.Name;
                            dto.PlugInPointDescription = plugInPoint.Description;
                        }
                        else if (customPlugInContainer != null)
                        {
                            dto.PlugInPointName = $"{customPlugInContainer.Name} - {customPlugInInterface?.Name}";
                            dto.PlugInPointDescription = customPlugInContainer.Description;
                        }
                        else
                        {
                            dto.PlugInPointName = "N/A";
                            dto.PlugInPointDescription = "N/A";
                        }

                        result.Add(dto);
                    }

                    skipped += Math.Min(gameConfig.PlugInConfigurations.Count, offset);
                }

                return result;
            }
        }

        /// <summary>
        /// Saves the specified plugin.
        /// Creates a new plugin, if the <see cref="PlugInConfigurationDto.Id"/> is empty, or updates an existing plugin if the <see cref="PlugInConfigurationDto.Id"/> isn't empty.
        /// </summary>
        /// <param name="pluginDto">The plugin.</param>
        /// <returns>The success of the operation.</returns>
        [HttpPost]
        public IActionResult Save([FromBody] PlugInConfigurationDto pluginDto)
        {
            try
            {
                GameConfiguration gameConfiguration;
                using (var context = this.persistenceContextProvider.CreateNewContext())
                {
                    gameConfiguration = context.GetById<GameConfiguration>(pluginDto.GameConfigurationId);
                }

                using (var context = this.persistenceContextProvider.CreateNewContext(gameConfiguration))
                {
                    PlugInConfiguration plugin = pluginDto.Id == Guid.Empty
                        ? context.CreateNew<PlugInConfiguration>()
                        : context.GetById<PlugInConfiguration>(pluginDto.Id);

                    plugin.IsActive = pluginDto.IsActive;
                    plugin.CustomPlugInSource = pluginDto.CustomPlugInSource;
                    plugin.ExternalAssemblyName = pluginDto.ExternalAssemblyName;
                    if (pluginDto.Id == Guid.Empty)
                    {
                        plugin.TypeId = pluginDto.TypeId;
                    }

                    context.SaveChanges();
                    return this.Ok(plugin.GetId());
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Deletes the plugin with the specified id.
        /// </summary>
        /// <param name="pluginId">The plugin id.</param>
        /// <returns>The success of the operation.</returns>
        [HttpPost]
        public IActionResult Delete(Guid pluginId)
        {
            try
            {
                using (var context = this.persistenceContextProvider.CreateNewContext())
                {
                    var plugin = context.GetById<PlugInConfiguration>(pluginId);
                    var success = context.Delete(plugin);

                    return this.Ok(success && context.SaveChanges());
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw;
            }
        }
    }
}
