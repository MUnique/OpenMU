// <copyright file="DiscordSetup.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Discord;
using MUnique.OpenMU.Discord.ChatBridge;
using MUnique.OpenMU.Discord.Provisioning;
using MUnique.OpenMU.GameLogic.PlugIns.GameEvents;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.AdminPanel.Services;
using MUnique.OpenMU.Web.Shared.Components.Toast;

/// <summary>
/// The page which guides through the setup of the Discord integration, and edits all of its settings.
/// </summary>
public partial class DiscordSetup : IDisposable
{
    private static readonly IReadOnlyList<DiscordChannelCategory> Categories = Enum.GetValues<DiscordChannelCategory>();

    private static readonly IReadOnlyList<(string Code, string Name)> Languages = [("en", "English"), ("de", "Deutsch")];

    private readonly Dictionary<DiscordChannelCategory, string> _newWebhooks = new();
    private readonly Dictionary<DiscordChannelCategory, string> _channelIdTexts = new();
    private readonly Dictionary<string, bool> _definedElsewhere = new();
    private readonly Dictionary<DiscordChannelCategory, DiscordTestResult> _testResults = new();
    private readonly HashSet<DiscordChannelCategory> _runningTests = new();

    private DiscordSettings? _settings;
    private DiscordIntegration? _integration;
    private DiscordProvisioningResult? _setupResult;
    private DiscordServerCheck? _serverCheck;
    private string? _loadError;
    private bool _isSaving;
    private bool _isSettingUp;
    private bool? _isPublisherActive;

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

    private DiscordBot? Bot => this._integration?.Bot;

    /// <summary>
    /// Gets the token which the bot will use: the entered one, or the one which it uses now.
    /// </summary>
    private string? EffectiveToken => string.IsNullOrWhiteSpace(this._newToken)
        ? this._integration?.Settings.Bot.Token ?? this._settings?.Bot.Token
        : this._newToken.Trim();

    private string? InviteUrl => DiscordBotToken.GetApplicationId(this.EffectiveToken) is { } applicationId
        ? DiscordBotToken.CreateInviteUrl(applicationId)
        : null;

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
        await this.LoadPublisherStateAsync().ConfigureAwait(true);
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

    private static string TestResultText(DiscordTestResult result)
    {
        var text = Resources.ResourceManager.GetString($"DiscordTest_{result.Outcome}", Resources.Culture) ?? result.Outcome.ToString();
        return string.Format(text, result.Detail);
    }

    private static string LinkedRoleText(DiscordRoleState state)
    {
        return Resources.ResourceManager.GetString($"DiscordCheckLinkedRole_{state}", Resources.Culture) ?? state.ToString();
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
        _ = this.InvokeAsync(this.StateHasChanged);
    }

    private async Task LoadAsync()
    {
        try
        {
            var settings = await this.ConfigurationService.LoadAsync().ConfigureAwait(true);
            this._newToken = string.Empty;
            this._newWebhooks.Clear();
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
            this._definedElsewhere.Clear();
            this._settings = settings;
            this._loadError = null;
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException or InvalidOperationException)
        {
            this.Logger.LogError(ex, "Couldn't load the Discord settings from {filePath}.", this.ConfigurationService.FilePath);
            this._settings = null;
            this._loadError = ex.Message;
        }
    }

    private async Task LoadPublisherStateAsync()
    {
        try
        {
            var gameConfiguration = await this.GameConfigurationSource.GetOwnerAsync().ConfigureAwait(true);
            this._isPublisherActive = gameConfiguration.PlugInConfigurations
                .FirstOrDefault(configuration => configuration.TypeId == typeof(GameEventPublisherPlugIn).GUID)?.IsActive ?? false;
        }
        catch (Exception ex)
        {
            this.Logger.LogWarning(ex, "Couldn't determine if the game event publisher is active.");
            this._isPublisherActive = null;
        }
    }

    private async Task ActivatePublisherAsync()
    {
        try
        {
            var gameConfiguration = await this.GameConfigurationSource.GetOwnerAsync().ConfigureAwait(true);
            if (gameConfiguration.PlugInConfigurations.FirstOrDefault(configuration => configuration.TypeId == typeof(GameEventPublisherPlugIn).GUID) is not { } publisher)
            {
                return;
            }

            // Like on the plugins page: the game servers apply the saved change at once.
            publisher.IsActive = true;
            await (await this.GameConfigurationSource.GetContextAsync().ConfigureAwait(true)).SaveChangesAsync().ConfigureAwait(true);
            this._isPublisherActive = true;
            this.ToastService.ShowSuccess(Resources.DiscordPublisherActivated);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Couldn't activate the game event publisher.");
            this.ToastService.ShowError(ex.Message);
        }
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
            await this.ConfigurationService.SaveAsync(settings).ConfigureAwait(true);
            this.ToastService.ShowSuccess(Resources.DiscordSaved);
            await this.LoadAsync().ConfigureAwait(true);
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
        {
            this.Logger.LogError(ex, "Couldn't save the Discord settings to {filePath}.", this.ConfigurationService.FilePath);
            this.ToastService.ShowError(string.Format(Resources.DiscordSaveError, ex.Message));
        }
        finally
        {
            this._isSaving = false;
        }
    }

    /// <summary>
    /// Takes the inputs which aren't bound directly to the settings.
    /// </summary>
    private bool TryApplyInputs(DiscordSettings settings)
    {
        if (!this.TryParseId(this._serverIdText, out var serverId)
            || !this.TryParseId(this._statusChannelIdText, out var statusChannelId)
            || !this.TryParseList<byte>(this._eventServerIdsText, byte.TryParse, out var eventServerIds)
            || !this.TryParseList<ulong>(this._allowedServerIdsText, ulong.TryParse, out var allowedServerIds))
        {
            return false;
        }

        var channels = new Dictionary<DiscordChannelCategory, ulong>();
        foreach (var (category, text) in this._channelIdTexts)
        {
            if (!this.TryParseId(text, out var channelId))
            {
                return false;
            }

            if (channelId is { } id)
            {
                channels[category] = id;
            }
        }

        if (!string.IsNullOrWhiteSpace(this._newToken))
        {
            settings.Bot.Token = this._newToken.Trim();
        }

        foreach (var (category, url) in this._newWebhooks.Where(entry => !string.IsNullOrWhiteSpace(entry.Value)))
        {
            settings.Webhooks[category] = url.Trim();
        }

        settings.Bot.GuildId = serverId;
        settings.Bot.StatusChannelId = statusChannelId;
        settings.Bot.Channels = channels;
        settings.EventAnnouncingServerIds = eventServerIds;
        settings.Bot.ChatBridge.AllowedDiscordServerIds = allowedServerIds;
        settings.Bot.StatusUpdateInterval = TimeSpan.FromSeconds(Math.Max(this._statusUpdateSeconds, 10));
        return true;
    }

    private bool TryParseId(string text, out ulong? id)
    {
        id = null;
        if (string.IsNullOrWhiteSpace(text))
        {
            return true;
        }

        if (ulong.TryParse(text.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var value))
        {
            id = value;
            return true;
        }

        this.ToastService.ShowError(string.Format(Resources.DiscordInvalidId, text));
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

    private async Task TestAsync(DiscordChannelCategory category)
    {
        if (this._integration is not { } integration || !this._runningTests.Add(category))
        {
            return;
        }

        try
        {
            this._testResults[category] = await integration.SendTestMessageAsync(category).ConfigureAwait(true);
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
        try
        {
            this._serverCheck = this.Bot is { } bot ? await bot.CheckDiscordServerAsync().ConfigureAwait(true) : null;
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
}
