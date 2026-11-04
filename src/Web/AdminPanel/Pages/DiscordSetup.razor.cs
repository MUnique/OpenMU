// <copyright file="DiscordSetup.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Discord;
using MUnique.OpenMU.Discord.ChatBridge;
using MUnique.OpenMU.Discord.Provisioning;
using MUnique.OpenMU.GameLogic.PlugIns.GameEvents;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.AdminPanel.Services;
using MUnique.OpenMU.Web.Shared.Components.Toast;

/// <summary>
/// The page which guides through the setup of the Discord integration, and edits all of its settings.
/// </summary>
public partial class DiscordSetup : IDisposable
{
    /// <summary>
    /// Keeps status refreshes from creating excessive Discord requests.
    /// </summary>
    private const int MinimumStatusUpdateSeconds = 10;

    private static readonly IReadOnlyList<DiscordChannelCategory> Categories = Enum.GetValues<DiscordChannelCategory>();

    private static readonly IReadOnlyList<(string Code, string Name)> Languages = [("en", "English"), ("de", "Deutsch")];

    private readonly Dictionary<DiscordChannelCategory, string> _newWebhooks = new();
    private readonly Dictionary<DiscordChannelCategory, string> _channelIdTexts = new();
    private readonly Dictionary<string, bool> _definedElsewhere = new();
    private readonly Dictionary<DiscordChannelCategory, DiscordTestResult> _testResults = new();
    private readonly HashSet<DiscordChannelCategory> _runningTests = new();
    private readonly Dictionary<string, string> _validationErrors = new();

    private DiscordSettings? _settings;
    private DiscordIntegration? _integration;
    private DiscordProvisioningResult? _setupResult;
    private DiscordServerLayout _layout = DiscordServerLayout.LoadDefault();
    private DiscordServerCheck? _serverCheck;
    private string? _loadError;
    private bool _isSaving;
    private bool _isSettingUp;
    private bool? _isPublisherActive;
    private IReadOnlyList<DiscordChatBindingInfo>? _chatBindings;
    private string _savedInputs = string.Empty;
    private bool _useBot = true;
    private ulong? _bindingToRemove;
    private string? _applyStatus;
    private GameEventPublisherConfiguration? _publisherSettings;
    private string _bossMonsterNumbersText = string.Empty;
    private string _levelMilestonesText = string.Empty;
    private string _masterLevelMilestonesText = string.Empty;

    private string _newToken = string.Empty;
    private string _serverIdText = string.Empty;
    private string _statusChannelIdText = string.Empty;
    private string _eventServerIdsText = string.Empty;
    private string _allowedServerIdsText = string.Empty;
    private int _statusUpdateSeconds;

    /// <summary>
    /// The signature of the <c>TryParse</c> methods of the number types.
    /// </summary>
    private delegate bool TryParseFunc<T>(string text, NumberStyles style, IFormatProvider provider, out T value);

    /// <summary>
    /// The state of a step of the setup, which its header shows.
    /// </summary>
    private enum StepState
    {
        /// <summary>
        /// The step is still to be done.
        /// </summary>
        Open,

        /// <summary>
        /// The step is done.
        /// </summary>
        Verified,

        /// <summary>
        /// The feature has settings but has not been verified.
        /// </summary>
        Configured,

        /// <summary>
        /// The step has a problem, which the administrator has to look at.
        /// </summary>
        Problem,
    }

    [Inject]
    private DiscordConfigurationService ConfigurationService { get; set; } = null!;

    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = null!;

    [Inject]
    private IDataSource<GameConfiguration> GameConfigurationSource { get; set; } = null!;

    [Inject]
    private IToastService ToastService { get; set; } = null!;

    [Inject]
    private ILogger<DiscordSetup> Logger { get; set; } = null!;

    private bool IsCheckingServer { get; set; }

    private bool ConfirmDiscard { get; set; }

    private bool ShowWebhookEditor { get; set; }

    private DateTimeOffset? LastCheckedAt { get; set; }

    private DiscordBot? Bot => this._integration?.Bot;

    /// <summary>
    /// Gets the token which the bot will use: the entered one, or the one which it uses now.
    /// </summary>
    private string? EffectiveToken => string.IsNullOrWhiteSpace(this._newToken)
        ? this._integration?.Settings.Bot.Token ?? this._settings?.Bot.Token
        : this._newToken.Trim();

    /// <summary>
    /// Gets a value indicating whether the bot is configured, or about to be: then the channels of the bot are shown.
    /// </summary>
    private bool UsesBot => this._useBot;

    private string IntegrationMode
    {
        get => this._useBot ? "bot" : "webhooks";
        set
        {
            this._useBot = value == "bot";
            this.OnModeChanged();
        }
    }

    /// <summary>
    /// Gets a value indicating whether the webhooks are shown: without the bot, and when webhooks are configured,
    /// so that they can be removed.
    /// </summary>
    private bool ShowsWebhooks => this.ShowWebhookEditor || !this.UsesBot
                                  || this._settings?.Webhooks.Count > 0
                                  || Categories.Any(category => this.IsDefinedElsewhere($"Webhooks:{category}"));

    private string DefaultStatusChannelText => this.DefaultChannelText(this._layout.Channels.FirstOrDefault(channel => channel.ShowsStatus)?.Name);

    private string? InviteUrl => DiscordBotToken.GetApplicationId(this.EffectiveToken) is { } applicationId
        ? DiscordBotToken.CreateInviteUrl(applicationId)
        : null;

    private bool IsBotConnected => this.Bot?.ServerState == ServerState.Started;

    /// <summary>
    /// Gets a value indicating whether the inputs differ from the saved settings. The tests and the setup use the saved
    /// settings, so they are only possible without unsaved changes.
    /// </summary>
    private bool HasUnsavedChanges => this._settings is not null && this.SerializeInputs() != this._savedInputs;

    /// <summary>
    /// Gets the text channels of the Discord server of the bot, from which the channels are chosen.
    /// </summary>
    private IReadOnlyList<DiscordChannelOption> TextChannels => this.IsBotConnected ? this.Bot!.GetTextChannels() : [];

    private StepState BotStepState => this.GetBotStepState();

    private StepState InviteStepState => this.IsBotConnected && this.Bot!.CurrentConnections > 0 ? StepState.Verified : StepState.Open;

    private StepState ServerStepState => this.GetServerStepState();

    private StepState NotificationsStepState => this.GetNotificationsStepState();

    /// <inheritdoc />
    public void Dispose()
    {
        if (this._integration is { } integration)
        {
            integration.PropertyChanged -= this.OnIntegrationPropertyChanged;
        }
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(true);
        if (!AdminPanelEnvironment.IsHostingEmbedded)
        {
            return;
        }

        this._integration = this.ServiceProvider.GetService<DiscordIntegration>();
        if (this._integration is { } integration)
        {
            integration.PropertyChanged += this.OnIntegrationPropertyChanged;
        }

        await this.LoadAsync().ConfigureAwait(true);
        await this.LoadChatBindingsAsync().ConfigureAwait(true);
    }

    /// <summary>
    /// Loads the layout of the Discord server, to show the default channels. A layout file which can't be read is
    /// reported in the status, so the default layout is shown then.
    /// </summary>
    private static DiscordServerLayout LoadLayout(string? layoutFile)
    {
        try
        {
            return layoutFile is { Length: > 0 } ? DiscordServerLayout.Load(layoutFile) : DiscordServerLayout.LoadDefault();
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException or InvalidOperationException)
        {
            return DiscordServerLayout.LoadDefault();
        }
    }

    private static string CategoryText(DiscordChannelCategory category)
    {
        return Resources.ResourceManager.GetString($"DiscordCategory_{category}", Resources.Culture) ?? category.ToString();
    }

    private static string BindingModeText(DiscordGuildChatBindingMode mode)
    {
        return Resources.ResourceManager.GetString($"DiscordBindingMode_{mode}", Resources.Culture) ?? mode.ToString();
    }

    private static string Join<T>(IEnumerable<T> values) => string.Join(", ", values);

    private static bool IsSent(DiscordTestResult result) => result.Outcome is DiscordTestOutcome.SentByBot or DiscordTestOutcome.SentByWebhook;

    private static string ScopeText(MUnique.OpenMU.DataModel.Entities.GuildChatScope scope)
    {
        return Resources.ResourceManager.GetString($"DiscordScope_{scope}", Resources.Culture) ?? scope.ToString();
    }

    private static MarkupString StepBadge(StepState state) => state switch
    {
        StepState.Verified => new MarkupString($"<span class=\"badge text-bg-success ms-2\">✓ {System.Net.WebUtility.HtmlEncode(Resources.DiscordStepDone)}</span>"),
        StepState.Configured => new MarkupString($"<span class=\"badge text-bg-secondary ms-2\">{System.Net.WebUtility.HtmlEncode(UiText("DiscordConfigured"))}</span>"),
        StepState.Problem => new MarkupString($"<span class=\"badge text-bg-warning ms-2\">⚠ {System.Net.WebUtility.HtmlEncode(Resources.DiscordStepProblem)}</span>"),
        _ => new MarkupString($"<span class=\"badge text-bg-light ms-2\">{System.Net.WebUtility.HtmlEncode(UiText("DiscordNotConfigured"))}</span>"),
    };

    private static string TestResultText(DiscordTestResult result)
    {
        var text = Resources.ResourceManager.GetString($"DiscordTest_{result.Outcome}", Resources.Culture) ?? result.Outcome.ToString();
        return string.Format(text, result.Detail);
    }

    private static string LinkedRoleText(DiscordRoleState state)
    {
        return Resources.ResourceManager.GetString($"DiscordCheckLinkedRole_{state}", Resources.Culture) ?? state.ToString();
    }

    private static string UiText(string key, params object[] args)
    {
        var text = Resources.ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
        return args.Length == 0 ? text : string.Format(CultureInfo.CurrentUICulture, text, args);
    }

    private string DefaultChannelText(string? channelName)
    {
        return channelName is null ? Resources.DiscordNoDefaultChannel : string.Format(Resources.DiscordDefaultChannel, channelName);
    }

    private string DefaultChannelText(DiscordChannelCategory category)
    {
        return this.DefaultChannelText(this._layout.Channels.FirstOrDefault(channel => channel.Notifications.Contains(category))?.Name);
    }

    private bool IsDefinedElsewhere(string key)
    {
        if (!this._definedElsewhere.TryGetValue(key, out var result))
        {
            result = this.ConfigurationService.IsDefinedElsewhere(key);
            this._definedElsewhere[key] = result;
        }

        return result;
    }

    private void OnIntegrationPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        _ = this.InvokeAsync(() =>
        {
            if (this.Bot?.ServerState == ServerState.Started)
            {
                this._applyStatus = UiText("DiscordApplied");
            }

            this.StateHasChanged();
        });
    }

    private async Task LoadAsync()
    {
        try
        {
            var settings = await this.ConfigurationService.LoadForEditingAsync().ConfigureAwait(true);
            this._newToken = string.Empty;
            this._newWebhooks.Clear();
            this.LoadRoutingInputs(settings);
            this._definedElsewhere.Clear();
            this._layout = LoadLayout(this._integration?.Settings.Bot.LayoutFile ?? settings.Bot.LayoutFile);
            this._settings = settings;
            this._useBot = settings.UseBot ?? (settings.Bot.IsEnabled || settings.Webhooks.Count == 0);
            this._loadError = null;
            this.ConfirmDiscard = false;
            this._validationErrors.Clear();
            this.InvalidateChecks();
            await this.LoadPublisherStateAsync().ConfigureAwait(true);
            this._savedInputs = this.SerializeInputs();
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException or InvalidOperationException)
        {
            this.Logger.LogError(ex, "Couldn't load the Discord settings from {filePath}.", this.ConfigurationService.FilePath);
            this._settings = null;
            this._loadError = ex.Message;
        }
    }

    private void LoadRoutingInputs(DiscordSettings settings)
    {
        this._channelIdTexts.Clear();
        foreach (var (category, channelId) in settings.Bot.Channels)
        {
            this._channelIdTexts[category] = channelId.ToString(CultureInfo.InvariantCulture);
        }

        this._serverIdText = settings.Bot.GuildId?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        this._statusChannelIdText = settings.Bot.StatusChannelId?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        this._eventServerIdsText = Join(settings.EventAnnouncingServerIds);
        this._allowedServerIdsText = Join(settings.Bot.ChatBridge.AllowedDiscordServerIds);
        this._statusUpdateSeconds = (int)settings.Bot.StatusUpdateInterval.TotalSeconds;
    }

    private async Task LoadPublisherStateAsync()
    {
        try
        {
            var gameConfiguration = await this.GameConfigurationSource.GetOwnerAsync().ConfigureAwait(true);
            var publisher = gameConfiguration.PlugInConfigurations.FirstOrDefault(configuration => configuration.TypeId == typeof(GameEventPublisherPlugIn).GUID);
            this._isPublisherActive = publisher?.IsActive ?? false;
            var publisherSettings = publisher?.GetConfiguration<GameEventPublisherConfiguration>(new ByDataSourceReferenceHandler(this.GameConfigurationSource))
                                    ?? new GameEventPublisherConfiguration();
            this._publisherSettings = publisherSettings;
            this._bossMonsterNumbersText = Join(publisherSettings.BossMonsterNumbers);
            this._levelMilestonesText = Join(publisherSettings.LevelMilestones);
            this._masterLevelMilestonesText = Join(publisherSettings.MasterLevelMilestones);
        }
        catch (Exception ex)
        {
            this.Logger.LogWarning(ex, "Couldn't determine if the game event publisher is active.");
            this._isPublisherActive = null;
        }
    }

    private async Task SavePublisherSettingsAsync()
    {
        if (this._publisherSettings is not { } publisherSettings || this._isPublisherActive is not { } active)
        {
            return;
        }

        var gameConfiguration = await this.GameConfigurationSource.GetOwnerAsync().ConfigureAwait(true);
        var context = await this.GameConfigurationSource.GetContextAsync().ConfigureAwait(true);
        var publisher = gameConfiguration.PlugInConfigurations.FirstOrDefault(configuration => configuration.TypeId == typeof(GameEventPublisherPlugIn).GUID);
        if (publisher is null)
        {
            publisher = context.CreateNew<PlugInConfiguration>();
            publisher.TypeId = typeof(GameEventPublisherPlugIn).GUID;
            gameConfiguration.PlugInConfigurations.Add(publisher);
        }

        publisher.IsActive = active;
        publisher.SetConfiguration(publisherSettings, new ByDataSourceReferenceHandler(this.GameConfigurationSource));
        await context.SaveChangesAsync().ConfigureAwait(true);
    }

    private void RemoveToken()
    {
        if (this._settings is { } settings)
        {
            settings.Bot.Token = null;
            this._newToken = string.Empty;
        }
    }

    private void RemoveWebhook(DiscordChannelCategory category)
    {
        this._settings?.Webhooks.Remove(category);
        this._newWebhooks.Remove(category);
    }

    private async Task SaveAsync()
    {
        if (this._settings is not { } settings || !this.TryApplyInputs(settings))
        {
            return;
        }

        this._isSaving = true;
        try
        {
            this._applyStatus = UiText("DiscordApplying");
            await this.ConfigurationService.SaveAsync(settings).ConfigureAwait(true);
            await this.SavePublisherSettingsAsync().ConfigureAwait(true);
            if (this._integration is { } integration)
            {
                await integration.ApplySettingsAsync().ConfigureAwait(true);
                if (integration.SettingsError is { } error)
                {
                    throw new InvalidOperationException(error);
                }
            }

            this.ShowApplyStatus();

            await this.LoadAsync().ConfigureAwait(true);
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException or InvalidOperationException)
        {
            this._applyStatus = string.Format(Resources.DiscordSaveError, ex.Message);
            this.Logger.LogError(ex, "Couldn't save the Discord settings to {filePath}.", this.ConfigurationService.FilePath);
            this.ToastService.ShowError(string.Format(Resources.DiscordSaveError, ex.Message));
        }
        finally
        {
            this._isSaving = false;
        }
    }

    private void ShowApplyStatus()
    {
        if (this.Bot is { Problem: not DiscordBotProblem.None })
        {
            this._applyStatus = UiText("DiscordSavedConnectionFailed");
            this.ToastService.ShowWarning(this._applyStatus);
            return;
        }

        this._applyStatus = this.Bot?.ServerState == ServerState.Starting ? Resources.DiscordStatusConnecting : UiText("DiscordApplied");
        this.ToastService.ShowSuccess(Resources.DiscordSaved);
    }

    /// <summary>
    /// Takes the inputs which aren't bound directly to the settings.
    /// </summary>
    private bool TryApplyInputs(DiscordSettings settings)
    {
        this._validationErrors.Clear();
        this.ValidateMinimum("discordMessageLength", settings.Bot.ChatBridge.MaximumMessageLength, 1);
        this.ValidateMinimum("discordMessagesPerMinute", settings.Bot.ChatBridge.MaximumMessagesPerMinute, 1);
        this.ValidateMinimum("discordQueuedMessages", settings.MaximumQueuedMessages, 1);
        this.ValidateMinimum("discordStatusInterval", this._statusUpdateSeconds, MinimumStatusUpdateSeconds);
        if (this._validationErrors.Count > 0)
        {
            return false;
        }

        if (!this.TryParseId(this._serverIdText, out var serverId, "discordServer")
            || !this.TryParseId(this._statusChannelIdText, out var statusChannelId, "discordStatusChannel")
            || !this.TryParseList<byte>(this._eventServerIdsText, byte.TryParse, out var eventServerIds)
            || !this.TryParseList<ulong>(this._allowedServerIdsText, ulong.TryParse, out var allowedServerIds))
        {
            return false;
        }

        if (!this.TryApplyPublisherInputs())
        {
            return false;
        }

        if (!this.TryReadChannelInputs(out var channels))
        {
            return false;
        }

        if (this._useBot && !string.IsNullOrWhiteSpace(this._newToken))
        {
            settings.Bot.Token = this._newToken.Trim();
        }

        foreach (var (category, url) in this._newWebhooks.Where(entry => !string.IsNullOrWhiteSpace(entry.Value)))
        {
            settings.Webhooks[category] = url.Trim();
        }

        settings.UseBot = this._useBot;
        settings.Bot.GuildId = serverId;
        settings.Bot.StatusChannelId = statusChannelId;
        settings.Bot.Channels = channels;
        settings.EventAnnouncingServerIds = eventServerIds;
        settings.Bot.ChatBridge.AllowedDiscordServerIds = allowedServerIds;
        settings.Bot.StatusUpdateInterval = TimeSpan.FromSeconds(Math.Max(this._statusUpdateSeconds, MinimumStatusUpdateSeconds));
        return true;
    }

    private bool TryReadChannelInputs(out Dictionary<DiscordChannelCategory, ulong> channels)
    {
        channels = new Dictionary<DiscordChannelCategory, ulong>();
        foreach (var (category, text) in this._channelIdTexts)
        {
            if (!this.TryParseId(text, out var channelId, "discordChannel-" + category))
            {
                return false;
            }

            if (channelId is { } id)
            {
                channels[category] = id;
            }
        }

        return true;
    }

    private bool TryApplyPublisherInputs()
    {
        if (!this.TryParseList<short>(this._bossMonsterNumbersText, short.TryParse, out var bosses)
            || !this.TryParseList<int>(this._levelMilestonesText, int.TryParse, out var levels)
            || !this.TryParseList<int>(this._masterLevelMilestonesText, int.TryParse, out var masterLevels))
        {
            return false;
        }

        if (this._publisherSettings is { } publisherSettings)
        {
            publisherSettings.BossMonsterNumbers = bosses;
            publisherSettings.LevelMilestones = levels;
            publisherSettings.MasterLevelMilestones = masterLevels;
        }

        return true;
    }

    private bool TryParseId(string text, out ulong? id, string field = "")
    {
        id = null;
        if (string.IsNullOrWhiteSpace(text))
        {
            return true;
        }

        if (ulong.TryParse(text.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var value) && value > 0)
        {
            id = value;
            return true;
        }

        this._validationErrors[field] = string.Format(Resources.DiscordInvalidId, text);
        this.ToastService.ShowError(this._validationErrors[field]);
        return false;
    }

    private bool TryParseList<T>(string text, TryParseFunc<T> tryParse, out List<T> values)
    {
        values = new List<T>();
        foreach (var part in text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (!tryParse(part, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
            {
                this.ToastService.ShowError(string.Format(Resources.DiscordInvalidId, part));
                return false;
            }

            values.Add(value);
        }

        return true;
    }

    /// <summary>
    /// Serializes the inputs, to find out if they differ from the saved settings.
    /// </summary>
    private string SerializeInputs()
    {
        return JsonSerializer.Serialize(new
        {
            Settings = this._settings,
            Publisher = this._publisherSettings,
            this._isPublisherActive,
            this._useBot,
            this._bossMonsterNumbersText,
            this._levelMilestonesText,
            this._masterLevelMilestonesText,
            this._newToken,
            NewWebhooks = this._newWebhooks.Where(entry => !string.IsNullOrWhiteSpace(entry.Value)).OrderBy(entry => entry.Key).ToList(),
            ChannelIds = this._channelIdTexts.Where(entry => !string.IsNullOrWhiteSpace(entry.Value)).OrderBy(entry => entry.Key).ToList(),
            this._serverIdText,
            this._statusChannelIdText,
            this._eventServerIdsText,
            this._allowedServerIdsText,
            this._statusUpdateSeconds,
        });
    }

    private async Task TestAllAsync()
    {
        foreach (var category in Categories.Where(this.IsCategoryEnabled))
        {
            await this.TestAsync(category).ConfigureAwait(true);
        }

        if (this.IsBotConnected)
        {
            await this.CheckServerAsync().ConfigureAwait(true);
        }
    }

    private async Task LoadChatBindingsAsync()
    {
        try
        {
            this._chatBindings = this.IsBotConnected ? await this.Bot!.GetChatBindingsAsync().ConfigureAwait(true) : null;
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Couldn't load the bound chats.");
            this._chatBindings = null;
        }
    }

    private async Task RemoveChatBindingAsync(ulong channelId)
    {
        if (this.Bot is not { } bot)
        {
            return;
        }

        try
        {
            if (await bot.RemoveChatBindingAsync(channelId).ConfigureAwait(true))
            {
                this.ToastService.ShowSuccess(Resources.DiscordChatBindingRemoved);
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Couldn't remove the binding of the Discord channel {channelId}.", channelId);
            this.ToastService.ShowError(ex.Message);
        }

        await this.LoadChatBindingsAsync().ConfigureAwait(true);
    }

    private async Task TestAsync(DiscordChannelCategory category)
    {
        if (this._integration is not { } integration || !this._runningTests.Add(category))
        {
            return;
        }

        try
        {
            this._testResults[category] = await integration.SendTestMessageAsync(category).ConfigureAwait(true);
            this.LastCheckedAt = DateTimeOffset.UtcNow;
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error when testing the Discord category {category}.", category);
            this._testResults[category] = new DiscordTestResult(DiscordTestOutcome.Failed, ex.Message);
        }
        finally
        {
            this._runningTests.Remove(category);
        }
    }

    private async Task CheckServerAsync()
    {
        this.IsCheckingServer = true;
        try
        {
            this._serverCheck = this.Bot is { } bot ? await bot.CheckDiscordServerAsync().ConfigureAwait(true) : null;
            this.LastCheckedAt = DateTimeOffset.UtcNow;
            if (this._serverCheck is null)
            {
                this.ToastService.ShowError(Resources.DiscordCheckServerNeedsServer);
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error when checking the Discord server.");
            this.ToastService.ShowError(ex.Message);
        }
        finally
        {
            this.IsCheckingServer = false;
        }
    }

    private async Task SetUpServerAsync()
    {
        var serverId = this.TryParseId(this._serverIdText, out var id) ? id ?? this.Bot?.RoutingServerId : null;
        if (this.Bot is not { } bot || serverId is not { } discordServerId)
        {
            this.ToastService.ShowError(Resources.DiscordSetUpServerNeedsBot);
            return;
        }

        this._isSettingUp = true;
        try
        {
            this._setupResult = await bot.SetUpDiscordServerAsync(discordServerId).ConfigureAwait(true);
            if (this._setupResult is null)
            {
                this.ToastService.ShowError(Resources.DiscordSetUpServerNeedsBot);
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error when setting up the Discord server {serverId}.", discordServerId);
            this.ToastService.ShowError(ex.Message);
        }
        finally
        {
            this._isSettingUp = false;
        }
    }

    private bool PublisherFlag(string name) => DiscordPublisherOptions.GetFlag(this._publisherSettings, name);

    private void SetWebhookEditorVisible(bool visible) => this.ShowWebhookEditor = visible;

    private void SetPublisherFlag(string name, bool value) => DiscordPublisherOptions.SetFlag(this._publisherSettings, name, value);

    private StepState GetBotStepState()
    {
        if (this.Bot?.Problem is DiscordBotProblem.TokenRejected)
        {
            return StepState.Problem;
        }

        if (this.IsBotConnected)
        {
            return StepState.Verified;
        }

        return this.EffectiveToken is not null ? StepState.Configured : StepState.Open;
    }

    private StepState GetServerStepState()
    {
        if (this.HasUnsavedChanges)
        {
            return StepState.Configured;
        }

        if (this._serverCheck is { } check)
        {
            return check.MissingPermissions.Count > 0 || check.LinkedRole != DiscordRoleState.Assignable
                ? StepState.Problem : StepState.Verified;
        }

        return this.Bot?.RoutingServerId is not null ? StepState.Configured : StepState.Open;
    }

    private StepState GetNotificationsStepState()
    {
        if (this.HasUnsavedChanges || this._testResults.Count == 0)
        {
            return this._settings is { IsNotificationEnabled: true } ? StepState.Configured : StepState.Open;
        }

        if (this._testResults.Values.Any(result => !IsSent(result)))
        {
            return StepState.Problem;
        }

        var enabled = Categories.Where(this.IsCategoryEnabled).ToList();
        return enabled.Count > 0 && enabled.All(category => this._testResults.TryGetValue(category, out var result) && IsSent(result))
            ? StepState.Verified : StepState.Configured;
    }

    private bool IsCategoryEnabled(DiscordChannelCategory category) => this._settings is null || !this._settings.DisabledNotifications.Contains(category);

    private void InvalidateChecks()
    {
        this._testResults.Clear();
        this._serverCheck = null;
        this.LastCheckedAt = null;
    }

    private void OnModeChanged()
    {
        this.InvalidateChecks();
    }

    private void SetCategoryEnabled(DiscordChannelCategory category, bool enabled)
    {
        if (this._settings is not { } settings)
        {
            return;
        }

        settings.DisabledNotifications.Remove(category);
        if (!enabled)
        {
            settings.DisabledNotifications.Add(category);
        }
    }

    private string RouteText(DiscordChannelCategory category)
    {
        if (!this.IsCategoryEnabled(category))
        {
            return UiText("DiscordOff");
        }

        var route = DiscordDeliveryRoute.Resolve(this._integration?.Settings ?? this._settings ?? new DiscordSettings(), this.Bot, category);
        if (route.ChannelId is { } id)
        {
            var name = this.TextChannels.FirstOrDefault(channel => channel.Id == id)?.Name ?? id.ToString(CultureInfo.InvariantCulture);
            return UiText("DiscordRouteBot", name);
        }

        return route.WebhookUrl is not null ? UiText("DiscordRouteWebhook") : UiText("DiscordRouteNone");
    }

    private string ManagedHint(string key) => UiText("DiscordManagedKey", this.ConfigurationService.GetEnvironmentKey(key));

    private void ValidateMinimum(string field, int value, int minimum)
    {
        if (value < minimum)
        {
            this._validationErrors[field] = UiText("DiscordMinimum", minimum);
        }
    }

    private MarkupString FieldError(string field) => this._validationErrors.TryGetValue(field, out var message)
        ? new MarkupString($"<div class=\"invalid-feedback d-block\" role=\"alert\">{System.Net.WebUtility.HtmlEncode(message)}</div>") : default;

    private async Task RequestDiscardAsync()
    {
        if (this.HasUnsavedChanges)
        {
            this.ConfirmDiscard = true;
        }
        else
        {
            await this.LoadAsync().ConfigureAwait(true);
        }
    }

    private void PreventUnsavedNavigation(LocationChangingContext context)
    {
        if (this.HasUnsavedChanges)
        {
            context.PreventNavigation();
            this.ToastService.ShowWarning(UiText("DiscordNavigationBlocked"));
        }
    }
}
