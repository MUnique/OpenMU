// <copyright file="GameEventWebhookPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.GameEvents;

using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Forwards selected <see cref="GameEvent"/>s as signed JSON to a webhook, e.g. the news feed of the server's website.
/// </summary>
/// <remarks>
/// The events are posted in the background (see <see cref="GameEventWebhookSender"/>), so the game never waits for the receiver.
/// Chat messages, letters, logins and account links are never forwarded: they are private.
/// The events come from the <see cref="GameEventPublisherPlugIn"/>, which has to be active, too.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.GameEventWebhookPlugIn_Name), Description = nameof(PlugInResources.GameEventWebhookPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("DD4381B2-5655-40FD-B6EC-362DB5940584")]
public sealed class GameEventWebhookPlugIn
    : IGameEventPublishedPlugIn,
    ISupportCustomConfiguration<GameEventWebhookConfiguration>,
    ISupportDefaultCustomConfiguration,
    IDisabledByDefault
{
    private readonly GameEventWebhookSender _sender;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameEventWebhookPlugIn"/> class.
    /// </summary>
    public GameEventWebhookPlugIn()
        : this(NullLoggerFactory.Instance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameEventWebhookPlugIn"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    [ActivatorUtilitiesConstructor]
    public GameEventWebhookPlugIn(ILoggerFactory loggerFactory)
        : this(new GameEventWebhookSender(loggerFactory.CreateLogger<GameEventWebhookSender>()), loggerFactory.CreateLogger<GameEventWebhookPlugIn>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameEventWebhookPlugIn"/> class.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="logger">The logger.</param>
    internal GameEventWebhookPlugIn(GameEventWebhookSender sender, ILogger logger)
    {
        this._sender = sender;
        this._logger = logger;
    }

    /// <inheritdoc />
    public GameEventWebhookConfiguration? Configuration { get; set; }

    /// <summary>
    /// Determines whether the event is forwarded with the configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="gameEvent">The game event.</param>
    /// <returns><c>True</c>, if it's forwarded; Otherwise, <c>false</c>.</returns>
    public static bool ShouldForward(GameEventWebhookConfiguration configuration, GameEvent gameEvent)
    {
        return gameEvent switch
        {
            BossKilledEvent => configuration.ForwardBossKills,
            CharacterLevelMilestoneEvent => configuration.ForwardLevelMilestones,
            MonsterItemDroppedEvent drop => configuration.ForwardItemDrops
                && drop.ItemLevel >= configuration.DropMinimumItemLevel
                && (configuration.DropItemNames.Count == 0 || configuration.DropItemNames.Contains(drop.ItemName, StringComparer.OrdinalIgnoreCase)),
            MiniGameEndedEvent => configuration.ForwardMiniGameResults,
            MiniGameEntranceOpenedEvent or MiniGameStartedEvent => configuration.ForwardMiniGameOpenings,
            InvasionStartedEvent or InvasionEndedEvent => configuration.ForwardInvasions,
            CastleSiegeStateChangedEvent => configuration.ForwardCastleSiege,
            GlobalNoticeEvent => configuration.ForwardGlobalNotices,

            // An allow list: chat, letters, logins, account links and any later event stay on the server.
            _ => false,
        };
    }

    /// <inheritdoc />
    public object CreateDefaultConfig() => new GameEventWebhookConfiguration();

    /// <inheritdoc />
    public ValueTask GameEventPublishedAsync(GameEvent gameEvent)
    {
        try
        {
            if (this.Configuration is { } configuration
                && configuration.Secret is { Length: > 0 } secret
                && Uri.TryCreate(configuration.Url, UriKind.Absolute, out var url)
                && (url.Scheme == Uri.UriSchemeHttps || url.Scheme == Uri.UriSchemeHttp)
                && ShouldForward(configuration, gameEvent))
            {
                this._sender.Enqueue(new GameEventWebhookDelivery(url, secret, Guid.NewGuid(), gameEvent));
            }
        }
        catch (Exception ex)
        {
            // The caller is game code; a broken forwarder must never break it.
            this._logger.LogError(ex, "Error when queueing the game event {gameEvent} for the webhook.", gameEvent);
        }

        return ValueTask.CompletedTask;
    }
}
