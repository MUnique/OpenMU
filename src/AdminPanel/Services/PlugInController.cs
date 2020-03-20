// <copyright file="PlugInController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using MUnique.OpenMU.AdminPanel.Models;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.Persistence;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A scoped controller to manage plugins.
    /// </summary>
    public class PlugInController : IDataService<PlugInConfigurationViewItem>, ISupportDataChangedNotification
    {
        private readonly IPersistenceContextProvider persistenceContextProvider;
        private string nameFilter;
        private Guid pointFilter;
        private string typeFilter;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlugInController"/> class.
        /// </summary>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        public PlugInController(IPersistenceContextProvider persistenceContextProvider)
        {
            this.persistenceContextProvider = persistenceContextProvider;
        }

        /// <inheritdoc />
        public event EventHandler DataChanged;

        /// <summary>
        /// Gets or sets the name filter.
        /// </summary>
        /// <value>
        /// The name filter.
        /// </value>
        public string NameFilter
        {
            get => this.nameFilter;
            set
            {
                this.nameFilter = value;
                this.DataChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the type filter.
        /// </summary>
        /// <value>
        /// The type filter.
        /// </value>
        public string TypeFilter
        {
            get => this.typeFilter;
            set
            {
                this.typeFilter = value;
                this.DataChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the extension point filter.
        /// </summary>
        public Guid PointFilter
        {
            get => this.pointFilter;
            set
            {
                this.pointFilter = value;
                this.DataChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the extension points.
        /// </summary>
        public List<PlugInPointViewItem> ExtensionPoints
        {
            get
            {
                var groupedPlugIns = GetPluginTypes().GroupBy(GetPlugInExtensionPointType);

                var result = groupedPlugIns.Select(group =>
                {
                    var dto = new PlugInPointViewItem
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
        }

        /// <inheritdoc />
        public Task<List<PlugInConfigurationViewItem>> Get(int offset, int count)
        {
            var result = new List<PlugInConfigurationViewItem>();

            try
            {
                using var context = this.persistenceContextProvider.CreateNewContext();
                var allPlugIns = GetPluginTypes().ToDictionary(t => t.GUID, t => t);
                foreach (var gameConfig in context.Get<GameConfiguration>())
                {
                    var rest = count - result.Count;
                    if (rest == 0)
                    {
                        break;
                    }

                    result.AddRange(this.GetPluginsOfConfig(offset, allPlugIns, gameConfig, rest));
                }
            }
            catch (NotImplementedException)
            {
                // swallow
            }

            return Task.FromResult(result);
        }

        /// <summary>
        /// Activates the plugin of the specified view item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Activate(PlugInConfigurationViewItem item)
        {
            this.ChangeActiveFlag(item, true);
        }

        /// <summary>
        /// Deactivates the plugin of the specified view item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Deactivate(PlugInConfigurationViewItem item)
        {
            this.ChangeActiveFlag(item, false);
        }

        private static IEnumerable<Type> GetPluginTypes() => AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.DefinedTypes.Where(type => type.GetCustomAttribute<PlugInAttribute>() != null));

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

        private static PlugInConfigurationViewItem BuildConfigurationDto(Type plugInType, GameConfiguration gameConfiguration, PlugInConfiguration plugInConfiguration)
        {
            var plugInAttribute = plugInType.GetCustomAttribute<PlugInAttribute>();
            var plugInPoint = plugInType.GetInterfaces().FirstOrDefault(i => i.GetCustomAttribute<PlugInPointAttribute>() != null)?.GetCustomAttribute<PlugInPointAttribute>();
            var customPlugInContainer = plugInType.GetInterfaces().FirstOrDefault(i => i.GetCustomAttribute<CustomPlugInContainerAttribute>() != null)?.GetCustomAttribute<CustomPlugInContainerAttribute>();

            var viewItem = new PlugInConfigurationViewItem(plugInConfiguration)
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
                viewItem.PlugInPointName = plugInPoint.Name;
                viewItem.PlugInPointDescription = plugInPoint.Description;
            }
            else if (customPlugInContainer != null)
            {
                var customPlugInInterface = plugInType.GetInterfaces().FirstOrDefault(intf => intf.GetInterfaces().Any(i => i.GetCustomAttribute<CustomPlugInContainerAttribute>() != null));
                viewItem.PlugInPointName = customPlugInInterface == null ? customPlugInContainer.Name : $"{customPlugInContainer.Name} - {customPlugInInterface.Name}";
                viewItem.PlugInPointDescription = customPlugInContainer.Description;
            }
            else
            {
                viewItem.PlugInPointName = "N/A";
                viewItem.PlugInPointDescription = "N/A";
            }

            return viewItem;
        }

        private IEnumerable<PlugInConfigurationViewItem> GetPluginsOfConfig(int offset, Dictionary<Guid, Type> allPlugIns, GameConfiguration gameConfig, int rest)
        {
            IEnumerable<PlugInConfigurationViewItem> GetRange(IEnumerable<PlugInConfiguration> plugInConfigurations, int actualOffset, int count)
            {
                foreach (var plugInConfiguration in plugInConfigurations.Skip(actualOffset).Take(count))
                {
                    if (!allPlugIns.TryGetValue(plugInConfiguration.TypeId, out var plugInType))
                    {
                        continue;
                    }

                    yield return BuildConfigurationDto(plugInType, gameConfig, plugInConfiguration);
                }
            }

            var configurations = this.GetConfigurations(allPlugIns, gameConfig);
            return GetRange(configurations, offset, rest);
        }

        private IEnumerable<PlugInConfiguration> GetConfigurations(Dictionary<Guid, Type> allPlugIns, GameConfiguration gameConfig)
        {
            return gameConfig.PlugInConfigurations.Where(c => allPlugIns.TryGetValue(c.TypeId, out var plugInType)
                                                              && this.FilterByPoint(plugInType)
                                                              && this.FilterByName(plugInType)
                                                              && this.FilterByTypeName(plugInType));
        }

        private void ChangeActiveFlag(PlugInConfigurationViewItem item, bool value)
        {
            using var context = this.persistenceContextProvider.CreateNewContext();
            context.Attach(item.Configuration);
            item.Configuration.IsActive = value;
            context.SaveChanges();
            this.DataChanged?.Invoke(this, EventArgs.Empty);
        }

        private bool FilterByTypeName(Type plugInType)
        {
            return string.IsNullOrWhiteSpace(this.TypeFilter) || plugInType.FullName.Contains(this.TypeFilter, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool FilterByName(Type plugInType)
        {
            return string.IsNullOrWhiteSpace(this.NameFilter) || GetPlugInName(plugInType).Contains(this.NameFilter, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool FilterByPoint(Type plugInType)
        {
            return this.PointFilter == Guid.Empty || this.PointFilter == GetPlugInExtensionPointType(plugInType)?.GUID;
        }
    }
}