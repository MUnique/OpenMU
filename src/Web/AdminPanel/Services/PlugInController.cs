// <copyright file="PlugInController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.Reflection;
using Blazored.Modal;
using Blazored.Modal.Services;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Web.AdminPanel.Components.Form;
using MUnique.OpenMU.Web.AdminPanel.Models;

/// <summary>
/// A scoped controller to manage plugins.
/// </summary>
public class PlugInController : IDataService<PlugInConfigurationViewItem>, ISupportDataChangedNotification
{
    private readonly IDataSource<GameConfiguration> _dataSource;
    private readonly IModalService _modalService;
    private string _nameFilter = string.Empty;
    private Guid _pointFilter;
    private string _typeFilter = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlugInController" /> class.
    /// </summary>
    /// <param name="dataSource">The data source.</param>
    /// <param name="modalService">The modal service.</param>
    public PlugInController(IDataSource<GameConfiguration> dataSource, IModalService modalService)
    {
        this._dataSource = dataSource;
        this._modalService = modalService;
    }

    /// <inheritdoc />
    public event EventHandler? DataChanged;

    /// <summary>
    /// Gets or sets the name filter.
    /// </summary>
    /// <value>
    /// The name filter.
    /// </value>
    public string NameFilter
    {
        get => this._nameFilter;
        set
        {
            this._nameFilter = value;
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
        get => this._typeFilter;
        set
        {
            this._typeFilter = value;
            this.DataChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets or sets the extension point filter.
    /// </summary>
    public Guid PointFilter
    {
        get => this._pointFilter;
        set
        {
            this._pointFilter = value;
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
    public async Task<List<PlugInConfigurationViewItem>> GetAsync(int offset, int count)
    {
        var result = new List<PlugInConfigurationViewItem>();

        try
        {
            var gameConfiguration = await this._dataSource.GetOwnerAsync(Guid.Empty);
            var allPlugIns = GetPluginTypes().ToDictionary(t => t.GUID, t => t);

            var rest = count - result.Count;

            result.AddRange(this.GetPluginsOfConfig(offset, allPlugIns, gameConfiguration, rest));
        }
        catch (NotImplementedException)
        {
            // swallow
        }

        return result;
    }

    /// <summary>
    /// Activates the plugin of the specified view item.
    /// </summary>
    /// <param name="item">The item.</param>
    public ValueTask ActivateAsync(PlugInConfigurationViewItem item)
    {
        return this.ChangeActiveFlagAsync(item, true);
    }

    /// <summary>
    /// Deactivates the plugin of the specified view item.
    /// </summary>
    /// <param name="item">The item.</param>
    public ValueTask DeactivateAsync(PlugInConfigurationViewItem item)
    {
        return this.ChangeActiveFlagAsync(item, false);
    }

    /// <summary>
    /// Shows the custom plug in configuration in a modal dialog.
    /// </summary>
    /// <param name="item">The item.</param>
    public async Task ShowPlugInConfigAsync(PlugInConfigurationViewItem item)
    {
        if (item.ConfigurationType is null)
        {
            throw new ArgumentException($"{nameof(item.ConfigurationType)} must not be null.", nameof(item));
        }

        var referenceResolver = new ByDataSourceReferenceHandler(this._dataSource);

        var configuration = item.Configuration.GetConfiguration(item.ConfigurationType, referenceResolver)
                            ?? Activator.CreateInstance(item.ConfigurationType);
        var parameters = new ModalParameters();
        parameters.Add(nameof(ModalCreateNew<object>.Item), configuration!);
        parameters.Add(nameof(ModalCreateNew<object>.PersistenceContext), await this._dataSource.GetContextAsync());
        var options = new ModalOptions
        {
            DisableBackgroundCancel = true,
        };

        var modal = this._modalService.Show(
            typeof(ModalCreateNew<>).MakeGenericType(item.ConfigurationType),
            item.PlugInName ?? string.Empty,
            parameters,
            options);

        var result = await modal.Result.ConfigureAwait(false);
        if (!result.Cancelled)
        {
            item.Configuration.SetConfiguration(configuration!, referenceResolver);
            await (await this._dataSource.GetContextAsync().ConfigureAwait(false)).SaveChangesAsync().ConfigureAwait(false);
            this.DataChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private static IEnumerable<Type> GetPluginTypes()
    {
        return AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly =>
        {
            try
            {
                return assembly.DefinedTypes.Where(type => type.GetCustomAttribute<PlugInAttribute>() != null);
            }
            catch (ReflectionTypeLoadException)
            {
                return Enumerable.Empty<Type>();
            }
        });
    }

    private static string GetPlugInName(Type plugInType)
    {
        return plugInType.GetCustomAttribute<PlugInAttribute>()!.Name;
    }

    private static Type? GetPlugInExtensionPointType(Type plugInType)
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
            ConfigurationType = plugInType.GetCustomConfigurationType(),
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
            viewItem.PlugInPointName = customPlugInInterface is null ? customPlugInContainer.Name : $"{customPlugInContainer.Name} - {customPlugInInterface.Name}";
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
                                                          && this.FilterByTypeName(plugInType))
            .OrderBy(p => p.CustomConfiguration is null); // First the ones which can be configured
    }

    private async ValueTask ChangeActiveFlagAsync(PlugInConfigurationViewItem item, bool value)
    {
        item.Configuration.IsActive = value;
        await (await this._dataSource.GetContextAsync().ConfigureAwait(true)).SaveChangesAsync().ConfigureAwait(true);
        this.DataChanged?.Invoke(this, EventArgs.Empty);
    }

    private bool FilterByTypeName(Type plugInType)
    {
        return string.IsNullOrWhiteSpace(this.TypeFilter) || (plugInType.FullName ?? plugInType.Name).Contains(this.TypeFilter, StringComparison.InvariantCultureIgnoreCase);
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