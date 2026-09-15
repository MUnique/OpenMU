// <copyright file="BotsController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The <c>bots</c> command of the control endpoint: switches the server's own population bots on and
/// off, and reports how many of them are animated.
/// </summary>
/// <remarks>
/// The switch goes through the persisted <see cref="PlugInConfiguration"/> of the bot feature - the
/// same route the admin panel's plugin dialog takes - so it survives a restart and is visible in the
/// panel. <see cref="PlugInManager"/> listens to the configuration's <c>PropertyChanged</c> and
/// pushes it to the live plugin.
/// </remarks>
public sealed class BotsController
{
    /// <summary>
    /// The number of animated bot characters <c>bots on</c> asks for when no count is given.
    /// </summary>
    public const int DefaultBotCount = 4;

    private readonly IGameServerContextLocator _locator;
    private readonly ILogger<BotsController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="BotsController"/> class.
    /// </summary>
    /// <param name="locator">Resolves the game server contexts of this process.</param>
    /// <param name="logger">The logger.</param>
    public BotsController(IGameServerContextLocator locator, ILogger<BotsController> logger)
    {
        this._locator = locator;
        this._logger = logger;
    }

    /// <summary>
    /// Applies the requested bot configuration to the given one, so that <paramref name="count"/> is
    /// the number of animated bot characters in total - not accounts times characters.
    /// </summary>
    /// <param name="configuration">The configuration to change.</param>
    /// <param name="enabled">Whether the feature should be on.</param>
    /// <param name="count">The number of bot characters; ignored when switching off.</param>
    public static void Apply(BotConfiguration configuration, bool enabled, int? count)
    {
        configuration.Enabled = enabled;
        if (enabled)
        {
            configuration.NumberOfAccounts = Math.Max(0, count ?? DefaultBotCount);

            // One character per account, so the requested number is what stands in the world, and no
            // bot logs out again while a scenario runs.
            configuration.MaxCharactersPerAccount = 1;
            configuration.PresenceRotation = false;
        }
    }

    /// <summary>
    /// Handles a <c>bots</c> request.
    /// </summary>
    /// <param name="action">One of <c>on</c>, <c>off</c> and <c>status</c>.</param>
    /// <param name="count">The requested number of bot characters, for <c>on</c>.</param>
    /// <returns>The result.</returns>
    public async ValueTask<ActorCommandResult> HandleAsync(string action, int? count)
    {
        if (this._locator.GetContexts() is not { Count: > 0 } contexts)
        {
            return ActorCommandResult.Failure(ActorErrorCodes.UnknownServer, "This process hosts no game server.");
        }

        var context = contexts[0].Context;
        switch (action)
        {
            case "on":
            case "off":
                var enabled = action == "on";
                if (BotFeaturePlugIn.GetConfiguration(context) is not { } configuration)
                {
                    return ActorCommandResult.Failure(
                        ActorErrorCodes.Failed,
                        "The bot feature plugin is not loaded in this server.");
                }

                Apply(configuration, enabled, count);
                if (await this.PersistAsync(context, configuration).ConfigureAwait(false) is { } failure)
                {
                    return failure;
                }

                this._logger.LogInformation(
                    "Bots switched {State} ({Count} account(s), one character each).",
                    enabled ? "on" : "off",
                    configuration.NumberOfAccounts);
                return await this.StatusAsync(contexts).ConfigureAwait(false);

            case "status":
                return await this.StatusAsync(contexts).ConfigureAwait(false);

            default:
                return ActorCommandResult.Failure(
                    ActorErrorCodes.BadRequest,
                    $"Unknown bots action '{action}'; expected 'on', 'off' or 'status'.");
        }
    }

    private async ValueTask<ActorCommandResult?> PersistAsync(IGameServerContext context, BotConfiguration configuration)
    {
        try
        {
            // A fresh context, so the PlugInConfiguration entity is tracked and the change is saved;
            // the cached in-memory configuration graph is not tracked.
            using var persistenceContext = context.PersistenceContextProvider.CreateNewContext();
            var typeId = typeof(BotFeaturePlugIn).GUID;
            var gameConfiguration = (await persistenceContext.GetAsync<GameConfiguration>().ConfigureAwait(false)).FirstOrDefault();
            var entity = gameConfiguration?.PlugInConfigurations.FirstOrDefault(c => c.TypeId == typeId);
            if (entity is null)
            {
                return ActorCommandResult.Failure(
                    ActorErrorCodes.Failed,
                    "The bot plugin configuration row does not exist in the database.");
            }

            entity.SetConfiguration(configuration, context.PlugInManager.CustomConfigReferenceHandler);
            await persistenceContext.SaveChangesAsync().ConfigureAwait(false);
            return null;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Failed to persist the bot plugin configuration.");
            return ActorCommandResult.Failure(ActorErrorCodes.Failed, ex.Message);
        }
    }

    private async ValueTask<ActorCommandResult> StatusAsync(IReadOnlyList<(int ServerId, IGameServerContext Context)> contexts)
    {
        var configuration = BotFeaturePlugIn.GetConfiguration(contexts[0].Context);
        var perServer = new List<Dictionary<string, object?>>();
        var total = 0;
        foreach (var (serverId, context) in contexts)
        {
            // The plugin's own per-server BotManager is private, so the animated bots are counted
            // where they actually are: among the players of that server's world.
            var players = await context.GetPlayersAsync().ConfigureAwait(false);
            var bots = players.Where(p => p.Account?.IsBot == true).ToList();
            total += bots.Count;
            perServer.Add(new Dictionary<string, object?>
            {
                ["server"] = serverId,
                ["animated"] = bots.Count,

                // Where they actually are, so a scenario can walk an actor to one instead of
                // hunting for it: a bot's saved position is only where it last logged out.
                ["bots"] = bots.Select(bot => new Dictionary<string, object?>
                {
                    ["account"] = bot.Account?.LoginName ?? string.Empty,
                    ["character"] = bot.Name,
                    ["id"] = bot.Id,
                    ["level"] = (int)(bot.Attributes?[Stats.Level] ?? 0),
                    ["map"] = bot.CurrentMap?.Definition.Name.ToString() ?? string.Empty,
                    ["map_number"] = bot.CurrentMap?.Definition.Number ?? -1,
                    ["x"] = bot.Position.X,
                    ["y"] = bot.Position.Y,
                    ["alive"] = bot.IsAlive,
                }).ToList(),
            });
        }

        return ActorCommandResult.Success(
            new ActorEventField("enabled", configuration?.Enabled ?? false),
            new ActorEventField("accounts", configuration?.NumberOfAccounts ?? 0),
            new ActorEventField("characters_per_account", configuration?.MaxCharactersPerAccount ?? 0),
            new ActorEventField("servers", perServer),
            new ActorEventField("animated", total));
    }
}
