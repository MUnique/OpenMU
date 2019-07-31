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
    using Microsoft.Extensions.Primitives;
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
        /// Returns all plugin extension points.
        /// </summary>
        /// <returns>All plugin extension points.</returns>
        [HttpGet]
        public ActionResult<List<PlugInPointDto>> ExtensionPoints()
        {
            var groupedPlugIns = GetPluginTypes().GroupBy(GetPlugInExtensionPointType);

            var result = groupedPlugIns.Select(group =>
            {
                var dto = new PlugInPointDto
                {
                    PlugInCount = group.Count(),
                    Id = group.Key?.GUID ?? default,
                };

                var plugInPoint = group.Key?.GetCustomAttribute<PlugInPointAttribute>();
                var customContainer = group.Key?.GetCustomAttribute<CustomPlugInContainerAttribute>();
                if (plugInPoint != null)
                {
                    dto.Name = plugInPoint.Name;
                }
                else if (customContainer != null)
                {
                    dto.Name = customContainer.Name;
                }
                else
                {
                    dto.Name = "N/A";
                }

                return dto;
            }).ToList();

            return result;
        }

        /// <summary>
        /// Returns a slice of the plugin list, defined by an offset and a count.
        /// </summary>
        /// <param name="pointId">The extension point identifier.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns>
        /// A slice of the plugin list, defined by an offset and a count.
        /// </returns>
        [HttpGet("{pointId}/{offset}/{count}")]
        public ActionResult<List<PlugInConfigurationDto>> List(Guid pointId, int offset, int count)
        {
            var allPlugIns = GetPluginTypes().ToDictionary(t => t.GUID, t => t);
            var skipped = 0;
            this.Request.Query.TryGetValue("name", out var nameFilter);
            this.Request.Query.TryGetValue("type", out var typeFilter);
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

                    var configurations = GetConfigurations(allPlugIns, pointId, gameConfig, nameFilter, typeFilter);

                    foreach (var plugInConfiguration in configurations.Skip(offset - skipped).Take(rest))
                    {
                        if (!allPlugIns.TryGetValue(plugInConfiguration.TypeId, out var plugInType))
                        {
                            continue;
                        }

                        result.Add(BuildConfigurationDto(plugInType, gameConfig, plugInConfiguration));
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

        private static IEnumerable<Type> GetPluginTypes() => AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.DefinedTypes.Where(type => type.GetCustomAttribute<PlugInAttribute>() != null));

        private static bool FilterByTypeName(Type plugInType, StringValues typeFilter)
        {
            return typeFilter.Count == 0 || plugInType.FullName.Contains(typeFilter[0], StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool FilterByName(Type plugInType, StringValues nameFilter)
        {
            return nameFilter.Count == 0 || GetPlugInName(plugInType).Contains(nameFilter[0], StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool FilterByPoint(Type plugInType, Guid pointId)
        {
            return pointId == Guid.Empty || pointId == GetPlugInExtensionPointType(plugInType)?.GUID;
        }

        private static string GetPlugInName(Type plugInType)
        {
            return plugInType.GetCustomAttribute<PlugInAttribute>().Name;
        }

        private static Type GetPlugInExtensionPointType(Type plugInType)
        {
            var plugInPoint = plugInType.GetInterfaces().FirstOrDefault(i => i.GetCustomAttribute<PlugInPointAttribute>() != null);
            var customPlugInContainer = plugInType.GetInterfaces().FirstOrDefault(i => i.GetCustomAttribute<CustomPlugInContainerAttribute>() != null);
            return plugInPoint ?? customPlugInContainer;
        }

        private static PlugInConfigurationDto BuildConfigurationDto(Type plugInType, GameConfiguration gameConfiguration, PlugInConfiguration plugInConfiguration)
        {
            var plugInAttribute = plugInType.GetCustomAttribute<PlugInAttribute>();
            var plugInPoint = plugInType.GetInterfaces().FirstOrDefault(i => i.GetCustomAttribute<PlugInPointAttribute>() != null)?.GetCustomAttribute<PlugInPointAttribute>();
            var customPlugInContainer = plugInType.GetInterfaces().FirstOrDefault(i => i.GetCustomAttribute<CustomPlugInContainerAttribute>() != null)?.GetCustomAttribute<CustomPlugInContainerAttribute>();

            var dto = new PlugInConfigurationDto
            {
                Id = plugInConfiguration.GetId(),
                GameConfigurationId = gameConfiguration.GetId(),
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
                var customPlugInInterface = plugInType.GetInterfaces().FirstOrDefault(intf => intf.GetInterfaces().Any(i => i.GetCustomAttribute<CustomPlugInContainerAttribute>() != null));
                dto.PlugInPointName = customPlugInInterface == null ? customPlugInContainer.Name : $"{customPlugInContainer.Name} - {customPlugInInterface.Name}";
                dto.PlugInPointDescription = customPlugInContainer.Description;
            }
            else
            {
                dto.PlugInPointName = "N/A";
                dto.PlugInPointDescription = "N/A";
            }

            return dto;
        }

        private static IEnumerable<PlugInConfiguration> GetConfigurations(Dictionary<Guid, Type> allPlugIns, Guid pointId, GameConfiguration gameConfig, StringValues nameFilter, StringValues typeFilter)
        {
            return gameConfig.PlugInConfigurations.Where(c => allPlugIns.TryGetValue(c.TypeId, out var plugInType)
                            && FilterByPoint(plugInType, pointId)
                            && FilterByName(plugInType, nameFilter)
                            && FilterByTypeName(plugInType, typeFilter));
        }
    }
}
