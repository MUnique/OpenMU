// <copyright file="DiscordBot.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using global::Discord;
using global::Discord.WebSocket;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Discord.Properties;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// The Discord bot. It shows the status of the game servers, answers the slash commands
/// and can post messages, e.g. the game events.
/// </summary>
/// <remarks>
/// It only needs the <see cref="GatewayIntents.Guilds"/> intent, and no privileged one,
/// because it reacts on slash commands instead of reading messages.
/// </remarks>
public sealed class DiscordBot : IManageableServer, IDiscordMessenger, IAsyncDisposable
{
    private readonly DiscordBotSettings _settings;
    private readonly DiscordCommands _commands;
    private readonly DiscordStatusFormatter _statusFormatter;
    private readonly IDiscordGameDataProvider _data;
    private readonly ILogger<DiscordBot> _logger;
    private readonly SemaphoreSlim _lifecycleLock = new(1, 1);

    private DiscordSocketClient? _client;
    private TaskCompletionSource _ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private CancellationTokenSource? _statusLoopCancellation;
    private ulong? _statusMessageId;
    private ServerState _serverState = ServerState.Stopped;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordBot"/> class.
    /// </summary>
    /// <param name="settings">The settings of the bot.</param>
    /// <param name="commands">The slash commands.</param>
    /// <param name="statusFormatter">The formatter of the status.</param>
    /// <param name="data">The provider of the data of the game.</param>
    /// <param name="logger">The logger.</param>
    public DiscordBot(DiscordBotSettings settings, DiscordCommands commands, DiscordStatusFormatter statusFormatter, IDiscordGameDataProvider data, ILogger<DiscordBot> logger)
    {
        this._settings = settings;
        this._commands = commands;
        this._statusFormatter = statusFormatter;
        this._data = data;
        this._logger = logger;
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public int Id => 0;

    /// <inheritdoc />
    public Guid ConfigurationId => Guid.Empty;

    /// <inheritdoc />
    public string Description => "Discord Bot";

    /// <inheritdoc />
    public ServerType Type => ServerType.Discord;

    /// <inheritdoc />
    public ServerState ServerState
    {
        get => this._serverState;
        private set
        {
            if (this._serverState != value)
            {
                this._serverState = value;
                this.OnPropertyChanged();
            }
        }
    }

    /// <inheritdoc />
    /// <remarks>The bot has no connection limit; it's in as many Discord servers as it gets invited to.</remarks>
    public int MaximumConnections => int.MaxValue;

    /// <inheritdoc />
    /// <remarks>The number of the Discord servers which the bot is in.</remarks>
    public int CurrentConnections => this._client?.Guilds.Count ?? 0;

    /// <inheritdoc />
    public async ValueTask StartAsync()
    {
        await this._lifecycleLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (this._client is not null)
            {
                return;
            }

            this.ServerState = ServerState.Starting;
            this._ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
            var client = new DiscordSocketClient(new DiscordSocketConfig { GatewayIntents = GatewayIntents.Guilds });
            client.Log += this.OnLogAsync;
            client.Ready += () => this.OnReadyAsync(client);
            client.SlashCommandExecuted += this.OnSlashCommandExecutedAsync;
            this._client = client;

            await client.LoginAsync(TokenType.Bot, this._settings.Token).ConfigureAwait(false);
            await client.StartAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "The Discord bot couldn't be started. Is the token valid?");
            await this.StopClientAsync().ConfigureAwait(false);
        }
        finally
        {
            this._lifecycleLock.Release();
        }
    }

    /// <inheritdoc />
    public async ValueTask ShutdownAsync()
    {
        await this._lifecycleLock.WaitAsync().ConfigureAwait(false);
        try
        {
            this.ServerState = ServerState.Stopping;
            await this.StopClientAsync().ConfigureAwait(false);
        }
        finally
        {
            this._lifecycleLock.Release();
        }
    }

    /// <inheritdoc />
    Task Microsoft.Extensions.Hosting.IHostedService.StartAsync(CancellationToken cancellationToken) => this.StartAsync().AsTask();

    /// <inheritdoc />
    Task Microsoft.Extensions.Hosting.IHostedService.StopAsync(CancellationToken cancellationToken) => this.ShutdownAsync().AsTask();

    /// <inheritdoc />
    public async Task SendAsync(ulong channelId, IReadOnlyList<DiscordEmbed> embeds, CancellationToken cancellationToken)
    {
        await this._ready.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
        if (await this.GetMessageChannelAsync(channelId).ConfigureAwait(false) is not { } channel)
        {
            this._logger.LogError("The Discord channel {channelId} isn't available for the bot.", channelId);
            return;
        }

        await channel.SendMessageAsync(embeds: embeds.Select(ToDiscordEmbed).ToArray(), allowedMentions: AllowedMentions.None).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await this.ShutdownAsync().ConfigureAwait(false);
        this._lifecycleLock.Dispose();
    }

    private static Embed ToDiscordEmbed(DiscordEmbed embed)
    {
        var builder = new EmbedBuilder()
            .WithTitle(embed.Title)
            .WithDescription(embed.Description)
            .WithColor(new Color((uint)embed.Color))
            .WithTimestamp(new DateTimeOffset(DateTime.SpecifyKind(embed.TimestampUtc, DateTimeKind.Utc)));
        if (embed.Footer is { } footer)
        {
            builder.WithFooter(footer);
        }

        return builder.Build();
    }

    private async ValueTask StopClientAsync()
    {
        if (this._statusLoopCancellation is { } cancellation)
        {
            await cancellation.CancelAsync().ConfigureAwait(false);
            cancellation.Dispose();
            this._statusLoopCancellation = null;
        }

        if (this._client is { } client)
        {
            this._client = null;
            try
            {
                await client.StopAsync().ConfigureAwait(false);
                await client.LogoutAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogWarning(ex, "Error when stopping the Discord bot.");
            }

            await client.DisposeAsync().ConfigureAwait(false);
        }

        this._statusMessageId = null;
        this.ServerState = ServerState.Stopped;
    }

    private async Task OnReadyAsync(DiscordSocketClient client)
    {
        try
        {
            await this.RegisterCommandsAsync(client).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "The slash commands of the Discord bot couldn't be registered.");
        }

        this.ServerState = ServerState.Started;
        this._ready.TrySetResult();

        if (this._statusLoopCancellation is null)
        {
            this._statusLoopCancellation = new CancellationTokenSource();
            _ = this.RunStatusLoopAsync(this._statusLoopCancellation.Token);
        }
    }

    private async Task RegisterCommandsAsync(DiscordSocketClient client)
    {
        var commands = DiscordCommands.Definitions.Select(definition =>
        {
            var builder = new SlashCommandBuilder()
                .WithName(definition.Name)
                .WithDescription(this._commands.Text(definition.DescriptionKey));
            if (definition.OptionName is { } optionName)
            {
                builder.AddOption(optionName, ApplicationCommandOptionType.String, this._commands.Text(definition.OptionDescriptionKey ?? optionName), isRequired: true);
            }

            return (ApplicationCommandProperties)builder.Build();
        }).ToArray();

        if (this._settings.GuildId is { } guildId && client.GetGuild(guildId) is { } guild)
        {
            await guild.BulkOverwriteApplicationCommandAsync(commands).ConfigureAwait(false);
        }
        else
        {
            await ((IDiscordClient)client).BulkOverwriteGlobalApplicationCommand(commands).ConfigureAwait(false);
        }
    }

    private async Task OnSlashCommandExecutedAsync(SocketSlashCommand command)
    {
        try
        {
            // The data might come from the database, which can take longer than Discord waits for an answer.
            await command.DeferAsync().ConfigureAwait(false);
            var argument = command.Data.Options.FirstOrDefault()?.Value as string;
            var answer = await this._commands.ExecuteAsync(command.Data.Name, argument).ConfigureAwait(false);
            await command.FollowupAsync(embed: ToDiscordEmbed(answer), allowedMentions: AllowedMentions.None).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error when answering the Discord command {command}.", command.Data.Name);
        }
    }

    private async Task RunStatusLoopAsync(CancellationToken cancellationToken)
    {
        var interval = this._settings.StatusUpdateInterval > TimeSpan.Zero ? this._settings.StatusUpdateInterval : TimeSpan.FromMinutes(1);
        using var timer = new PeriodicTimer(interval);
        try
        {
            do
            {
                await this.UpdateStatusAsync().ConfigureAwait(false);
            }
            while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false));
        }
        catch (OperationCanceledException)
        {
            // stopped
        }
    }

    private async Task UpdateStatusAsync()
    {
        if (this._client is not { } client)
        {
            return;
        }

        try
        {
            var servers = this._data.GetGameServers();
            await client.SetCustomStatusAsync(this._statusFormatter.CreatePresence(servers)).ConfigureAwait(false);

            if (this._settings.StatusChannelId is { } channelId
                && await this.GetMessageChannelAsync(channelId).ConfigureAwait(false) is { } channel)
            {
                var embed = ToDiscordEmbed(this._statusFormatter.CreateStatus(servers, DateTime.UtcNow));
                if (await this.FindStatusMessageAsync(client, channel).ConfigureAwait(false) is { } message)
                {
                    await message.ModifyAsync(properties => properties.Embed = embed).ConfigureAwait(false);
                }
                else
                {
                    var newMessage = await channel.SendMessageAsync(embed: embed, allowedMentions: AllowedMentions.None).ConfigureAwait(false);
                    this._statusMessageId = newMessage.Id;
                }
            }
        }
        catch (Exception ex)
        {
            this._logger.LogWarning(ex, "The status of the Discord bot couldn't be updated.");
        }
    }

    /// <summary>
    /// Finds the status message: the last message of the bot in the status channel. So the bot keeps
    /// using the same message after a restart, without storing anything.
    /// </summary>
    private async Task<IUserMessage?> FindStatusMessageAsync(DiscordSocketClient client, IMessageChannel channel)
    {
        if (this._statusMessageId is { } messageId
            && await channel.GetMessageAsync(messageId).ConfigureAwait(false) is IUserMessage knownMessage)
        {
            return knownMessage;
        }

        var messages = await channel.GetMessagesAsync(50).FlattenAsync().ConfigureAwait(false);
        var message = messages.OfType<IUserMessage>().FirstOrDefault(m => m.Author.Id == client.CurrentUser.Id);
        this._statusMessageId = message?.Id;
        return message;
    }

    private async Task<IMessageChannel?> GetMessageChannelAsync(ulong channelId)
    {
        if (this._client is not { } client)
        {
            return null;
        }

        return client.GetChannel(channelId) as IMessageChannel
               ?? await client.GetChannelAsync(channelId).ConfigureAwait(false) as IMessageChannel;
    }

    private Task OnLogAsync(LogMessage message)
    {
        var level = message.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            _ => LogLevel.Debug,
        };
        this._logger.Log(level, message.Exception, "{source}: {message}", message.Source, message.Message);
        return Task.CompletedTask;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
