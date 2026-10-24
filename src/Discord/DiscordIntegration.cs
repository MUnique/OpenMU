// <copyright file="DiscordIntegration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using MUnique.OpenMU.Discord.Properties;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The Discord integration of the server, as it's configured in the <see cref="DiscordSettings.SectionName"/> section
/// of the configuration: the bot and the notifications.
/// </summary>
/// <remarks>
/// When the section changes, e.g. on the Discord page of the admin panel, the bot and the notifier are created
/// again with the new settings, without a restart of the server. The services which they need are taken from the host.
/// </remarks>
public sealed class DiscordIntegration : IManageableServer, IGameEventListener, IAsyncDisposable
{
    /// <summary>
    /// The services of the host which the Discord integration uses, when they are available.
    /// </summary>
    private static readonly Type[] HostServiceTypes =
    [
        typeof(IServerProvider),
        typeof(IPersistenceContextProvider),
        typeof(IFriendServer),
        typeof(IGuildServer),
        typeof(PlugInManager),
        typeof(IEventPublisher),
    ];

    private readonly IConfiguration _section;
    private readonly IServiceProvider _hostServices;
    private readonly Func<TimeZoneInfo> _getServerTimeZone;
    private readonly ILogger<DiscordIntegration> _logger;
    private readonly SemaphoreSlim _lock = new(1, 1);

    /// <summary>
    /// The pending removals of roles, which survive the replacement of the bot when the settings change.
    /// </summary>
    private readonly DiscordPendingRoleRevocations _pendingRoleRevocations = new();

    private IDisposable? _changeRegistration;
    private Instance? _instance;
    private string? _appliedSettings;
    private HttpClient? _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordIntegration"/> class.
    /// </summary>
    /// <param name="section">The configuration section with the <see cref="DiscordSettings"/>.</param>
    /// <param name="hostServices">The services of the host.</param>
    /// <param name="getServerTimeZone">The function which gets the time zone of the server, in which the schedules of the events are defined.</param>
    /// <param name="logger">The logger.</param>
    public DiscordIntegration(IConfiguration section, IServiceProvider hostServices, Func<TimeZoneInfo> getServerTimeZone, ILogger<DiscordIntegration> logger)
    {
        this._section = section;
        this._hostServices = hostServices;
        this._getServerTimeZone = getServerTimeZone;
        this._logger = logger;
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the settings with which the integration currently runs.
    /// </summary>
    public DiscordSettings Settings => this._instance?.Settings ?? new DiscordSettings();

    /// <summary>
    /// Gets the bot, if a token is configured.
    /// </summary>
    public DiscordBot? Bot => this._instance?.Bot;

    /// <summary>
    /// Gets the error, which prevented that the integration was created with the current settings.
    /// </summary>
    /// <remarks>
    /// For example, a <see cref="DiscordBotSettings.LayoutFile"/> which can't be read.
    /// </remarks>
    public string? SettingsError { get; private set; }

    /// <inheritdoc />
    public int Id => 0;

    /// <inheritdoc />
    public Guid ConfigurationId => Guid.Empty;

    /// <inheritdoc />
    public string Description => "Discord Bot";

    /// <inheritdoc />
    public ServerType Type => ServerType.Discord;

    /// <inheritdoc />
    public ServerState ServerState => this.Bot?.ServerState ?? ServerState.Stopped;

    /// <inheritdoc />
    public int MaximumConnections => int.MaxValue;

    /// <inheritdoc />
    /// <remarks>The number of the Discord servers which the bot is in.</remarks>
    public int CurrentConnections => this.Bot?.CurrentConnections ?? 0;

    /// <summary>
    /// Gets or sets the function which creates the HTTP client of the test messages; the tests replace it.
    /// </summary>
    internal Func<HttpClient> CreateHttpClient { get; set; } = DiscordHttpClient.Create;

    /// <summary>
    /// Gets the listeners of the current settings, for the tests.
    /// </summary>
    internal IReadOnlyList<IGameEventListener> Listeners => this._instance?.Listeners ?? [];

    /// <inheritdoc />
    /// <remarks>Starts the bot, after it was shut down in the admin panel.</remarks>
    public async ValueTask StartAsync()
    {
        if (this.Bot is { } bot)
        {
            await bot.StartAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    /// <remarks>Shuts the bot down. The notifications by webhooks are still posted.</remarks>
    public async ValueTask ShutdownAsync()
    {
        if (this.Bot is { } bot)
        {
            await bot.ShutdownAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    async Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        this._changeRegistration ??= ChangeToken.OnChange(this._section.GetReloadToken, this.OnSettingsChanged);
        await this.ApplySettingsAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    async Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        await this.DisposeAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask OnGameEventAsync(GameEvent gameEvent)
    {
        foreach (var listener in this.Listeners)
        {
            try
            {
                await listener.OnGameEventAsync(gameEvent).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error when passing the game event {gameEvent} to {listener}.", gameEvent.GetType().Name, listener.GetType().Name);
            }
        }
    }

    /// <summary>
    /// Posts a test message for a category of notifications in the same way as the notifications are posted:
    /// by the bot into its channel of the category, otherwise by the webhook of the category.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result.</returns>
    public async ValueTask<DiscordTestResult> SendTestMessageAsync(DiscordChannelCategory category, CancellationToken cancellationToken = default)
    {
        var settings = this.Settings;
        var culture = settings.GetCulture();
        var embed = new DiscordEmbed(
            Resources.ResourceManager.GetString(nameof(Resources.Test_Title), culture) ?? nameof(Resources.Test_Title),
            string.Format(culture, Resources.ResourceManager.GetString(nameof(Resources.Test_Description), culture) ?? nameof(Resources.Test_Description), category),
            DiscordCommands.AnswerColor,
            DateTime.UtcNow,
            null);

        var route = DiscordDeliveryRoute.Resolve(settings, this.Bot, category);
        if (this.Bot is { } bot && route.ChannelId is { } channelId)
        {
            return await bot.SendTestMessageAsync(channelId, embed).ConfigureAwait(false);
        }

        if (route.WebhookUrl is not { } url)
        {
            return new DiscordTestResult(DiscordTestOutcome.NoChannel);
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var webhookUrl))
        {
            return new DiscordTestResult(DiscordTestOutcome.WebhookInvalid);
        }

        return await this.SendWebhookTestMessageAsync(category, webhookUrl, embed, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        this._changeRegistration?.Dispose();
        this._changeRegistration = null;
        this._httpClient?.Dispose();
        this._httpClient = null;
        await this._lock.WaitAsync().ConfigureAwait(false);
        try
        {
            await this.ReplaceInstanceAsync(null).ConfigureAwait(false);
            this._appliedSettings = null;
        }
        finally
        {
            this._lock.Release();
        }
    }

    /// <summary>
    /// Creates the bot and the notifier with the settings of the configuration, if they changed since the last time.
    /// </summary>
    public async ValueTask ApplySettingsAsync()
    {
        await this._lock.WaitAsync().ConfigureAwait(false);
        try
        {
            var settings = this._section.Get<DiscordSettings>() ?? new DiscordSettings();

            // The configuration notifies about every change of its files, also when the section stayed the same.
            var serialized = JsonSerializer.Serialize(settings);
            if (serialized == this._appliedSettings)
            {
                return;
            }

            if (this._appliedSettings is not null)
            {
                this._logger.LogInformation("The settings of the Discord integration changed, so it's restarted.");
            }

            this._appliedSettings = serialized;
            await this.ReplaceInstanceAsync(null).ConfigureAwait(false);
            this.SettingsError = null;
            try
            {
                await this.ReplaceInstanceAsync(this.CreateInstance(settings)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "The Discord integration couldn't be created with its settings.");
                this.SettingsError = ex.Message;
                this.OnPropertyChanged(nameof(this.SettingsError));
                return;
            }

            if (this.Bot is { } bot)
            {
                await bot.StartAsync().ConfigureAwait(false);
            }
        }
        finally
        {
            this._lock.Release();
        }
    }

    private void OnSettingsChanged()
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await this.ApplySettingsAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "The changed settings of the Discord integration couldn't be applied.");
            }
        });
    }

    private Instance CreateInstance(DiscordSettings settings)
    {
        var services = new ServiceCollection()
            .AddSingleton(this._hostServices.GetRequiredService<ILoggerFactory>())
            .AddSingleton(typeof(ILogger<>), typeof(Logger<>));
        var isHostService = this._hostServices.GetService<IServiceProviderIsService>();
        foreach (var serviceType in HostServiceTypes.Where(type => isHostService?.IsService(type) ?? true))
        {
            // Resolved when it's needed, because the event publisher of the all-in-one deployment depends on this integration.
            services.AddSingleton(serviceType, _ => this._hostServices.GetRequiredService(serviceType));
        }

        services.AddSingleton(this._pendingRoleRevocations);
        services.AddDiscord(settings, this._getServerTimeZone);
        var provider = services.BuildServiceProvider();
        try
        {
            return new Instance(settings, provider, provider.GetService<DiscordBot>(), provider.GetServices<IGameEventListener>().ToList());
        }
        catch
        {
            provider.Dispose();
            throw;
        }
    }

    private async ValueTask ReplaceInstanceAsync(Instance? newInstance)
    {
        var oldInstance = this._instance;
        if (oldInstance is null && newInstance is null)
        {
            return;
        }

        this._instance = newInstance;
        if (oldInstance is not null)
        {
            if (oldInstance.Bot is { } oldBot)
            {
                oldBot.PropertyChanged -= this.OnBotPropertyChanged;
            }

            await oldInstance.Services.DisposeAsync().ConfigureAwait(false);
        }

        if (newInstance?.Bot is { } newBot)
        {
            newBot.PropertyChanged += this.OnBotPropertyChanged;
        }

        this.OnPropertyChanged(nameof(this.Bot));
        this.OnPropertyChanged(nameof(this.ServerState));
    }

    private void OnBotPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        this.PropertyChanged?.Invoke(this, e);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async ValueTask<DiscordTestResult> SendWebhookTestMessageAsync(
        DiscordChannelCategory category,
        Uri webhookUrl,
        DiscordEmbed embed,
        CancellationToken cancellationToken)
    {
        try
        {
            this._httpClient ??= this.CreateHttpClient();
            var statusCode = await DiscordWebhookSender.PostOnceAsync(webhookUrl, this._httpClient, [embed], cancellationToken).ConfigureAwait(false);
            return statusCode switch
            {
                >= HttpStatusCode.OK and < HttpStatusCode.Ambiguous => new DiscordTestResult(DiscordTestOutcome.SentByWebhook),
                HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden or HttpStatusCode.NotFound => new DiscordTestResult(DiscordTestOutcome.WebhookInvalid),
                _ => new DiscordTestResult(DiscordTestOutcome.Failed, $"{(int)statusCode} {statusCode}"),
            };
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            this._logger.LogWarning(ex, "The test message of the category {category} couldn't be posted by its webhook.", category);
            return new DiscordTestResult(DiscordTestOutcome.Failed, ex.Message);
        }
    }

    /// <summary>
    /// The services of the integration, which were created for one version of the settings.
    /// </summary>
    /// <param name="Settings">The settings.</param>
    /// <param name="Services">The services.</param>
    /// <param name="Bot">The bot, if a token is configured.</param>
    /// <param name="Listeners">The listeners of the game events.</param>
    private sealed record Instance(DiscordSettings Settings, ServiceProvider Services, DiscordBot? Bot, IReadOnlyList<IGameEventListener> Listeners);
}
