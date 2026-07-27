// <copyright file="ChatCommandController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

using System.Reflection;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Web.Shared.Models;

/// <summary>
/// A scoped controller to show the chat commands of the game configuration.
/// </summary>
/// <remarks>
/// The chat commands are also listed on the plugins page, but only with the name
/// and the type of their plugin. This controller adds what an admin actually
/// needs to know about them - the command itself, and its parameters.
/// </remarks>
public class ChatCommandController : IDataService<ChatCommandViewItem>, ISupportDataChangedNotification
{
    private readonly IDataSource<GameConfiguration> _dataSource;
    private readonly PlugInController _plugInController;

    private string _commandFilter = string.Empty;
    private CharacterStatus? _statusFilter;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatCommandController"/> class.
    /// </summary>
    /// <param name="dataSource">The data source.</param>
    /// <param name="plugInController">The plugin controller, which is used to activate and configure the commands, so that both pages stay in sync.</param>
    public ChatCommandController(IDataSource<GameConfiguration> dataSource, PlugInController plugInController)
    {
        this._dataSource = dataSource;
        this._plugInController = plugInController;
    }

    /// <inheritdoc />
    public event EventHandler? DataChanged;

    /// <summary>
    /// Gets or sets the filter for the command and its description.
    /// </summary>
    public string CommandFilter
    {
        get => this._commandFilter;
        set
        {
            this._commandFilter = value;
            this.DataChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets or sets the filter for the required character status.
    /// </summary>
    public CharacterStatus? StatusFilter
    {
        get => this._statusFilter;
        set
        {
            this._statusFilter = value;
            this.DataChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<List<ChatCommandViewItem>> GetAsync(int offset, int count)
    {
        try
        {
            var gameConfiguration = await this._dataSource.GetOwnerAsync(Guid.Empty).ConfigureAwait(true);
            var commandPlugIns = GetChatCommandTypes().ToDictionary(type => type.GUID, type => type);

            return gameConfiguration.PlugInConfigurations
                .Where(configuration => commandPlugIns.ContainsKey(configuration.TypeId))
                .Select(configuration => this.BuildViewItem(commandPlugIns[configuration.TypeId], gameConfiguration, configuration))
                .Where(item => item is { })
                .Select(item => item!)
                .Where(this.MatchesFilter)
                .OrderBy(item => item.Info.Command, StringComparer.InvariantCultureIgnoreCase)
                .Skip(offset)
                .Take(count)
                .ToList();
        }
        catch (NotImplementedException)
        {
            // Ignored, same as on the plugins page.
            return [];
        }
    }

    /// <summary>
    /// Activates or deactivates the command of the specified view item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="isActive">If set to <c>true</c>, the command gets activated; Otherwise, it gets deactivated.</param>
    public async ValueTask SetActiveAsync(ChatCommandViewItem item, bool isActive)
    {
        if (isActive)
        {
            await this._plugInController.ActivateAsync(item.PlugIn).ConfigureAwait(true);
        }
        else
        {
            await this._plugInController.DeactivateAsync(item.PlugIn).ConfigureAwait(true);
        }

        this.DataChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Shows the custom configuration of the command in a modal dialog.
    /// </summary>
    /// <param name="item">The item.</param>
    public Task ShowConfigurationAsync(ChatCommandViewItem item)
    {
        return this._plugInController.ShowPlugInConfigAsync(item.PlugIn);
    }

    private static IEnumerable<Type> GetChatCommandTypes()
    {
        return AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly =>
        {
            try
            {
                return assembly.DefinedTypes
                    .Where(type => type.GetCustomAttribute<PlugInAttribute>() is { })
                    .Where(type => typeof(IChatCommandPlugIn).IsAssignableFrom(type))
                    .Select(type => type.AsType());
            }
            catch (ReflectionTypeLoadException)
            {
                return Enumerable.Empty<Type>();
            }
        });
    }

    private ChatCommandViewItem? BuildViewItem(Type plugInType, GameConfiguration gameConfiguration, PlugInConfiguration configuration)
    {
        if (ChatCommandTypeExtensions.TryCreateChatCommandInfo(plugInType) is not { } info)
        {
            return null;
        }

        return new ChatCommandViewItem(info, PlugInController.BuildConfigurationDto(plugInType, gameConfiguration, configuration));
    }

    private bool MatchesFilter(ChatCommandViewItem item)
    {
        if (this._statusFilter is { } status && item.Info.MinimumCharacterStatus != status)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(this._commandFilter))
        {
            return true;
        }

        return item.Info.Command.Contains(this._commandFilter, StringComparison.InvariantCultureIgnoreCase)
               || item.Info.Name.Contains(this._commandFilter, StringComparison.InvariantCultureIgnoreCase)
               || item.Info.Description.Contains(this._commandFilter, StringComparison.InvariantCultureIgnoreCase);
    }
}
