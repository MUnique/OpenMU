// <copyright file="ActorRegistry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

using System.Threading;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameLogic.Offline;

/// <summary>
/// The actors of this process.
/// </summary>
/// <remarks>
/// Spawning takes the account the way a real client does - through
/// <see cref="MUnique.OpenMU.Interfaces.ILoginServer.TryLoginAsync"/>, which is the cross-server
/// lock - and additionally scans the target game server's players for the same login name, because
/// population bots and offline sessions bypass the login server. Both checks and the insertion run
/// under one lock, so two concurrent spawns of one account yield exactly one actor.
/// </remarks>
public sealed class ActorRegistry : IActorRegistry
{
    private readonly Dictionary<string, ScriptedPlayer> _actors = new(StringComparer.OrdinalIgnoreCase);
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly IGameServerContextLocator _locator;
    private readonly IActorFactory _factory;
    private readonly ILogger<ActorRegistry> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActorRegistry"/> class.
    /// </summary>
    /// <param name="locator">Resolves the game server contexts of this process.</param>
    /// <param name="factory">Creates the actors.</param>
    /// <param name="logger">The logger.</param>
    public ActorRegistry(IGameServerContextLocator locator, IActorFactory factory, ILogger<ActorRegistry> logger)
    {
        this._locator = locator;
        this._factory = factory;
        this._logger = logger;
    }

    /// <inheritdoc />
    public async ValueTask<ActorCommandResult> SpawnAsync(int serverId, string loginName, byte? characterSlot)
    {
        if (string.IsNullOrWhiteSpace(loginName))
        {
            return ActorCommandResult.Failure(ActorErrorCodes.UnknownActor, "No account was given.");
        }

        if (this._locator.GetContext(serverId) is not { } context)
        {
            return ActorCommandResult.Failure(ActorErrorCodes.UnknownServer, $"There is no game server {serverId} in this process.");
        }

        await this._lock.WaitAsync().ConfigureAwait(false);
        var loginServerAcquired = false;
        try
        {
            if (this._actors.ContainsKey(loginName))
            {
                return InUse(loginName, "an actor");
            }

            // Every game server of this process, not just the target one: the population is split
            // over the servers (BotServerPartition), so a bot animating this account may well live
            // on another one - and two players driving one character means two persistence contexts
            // saving it, which is exactly the corruption the bot code warns about.
            foreach (var (serverIdOfContext, otherContext) in this._locator.Contexts)
            {
                var players = await otherContext.GetPlayersAsync().ConfigureAwait(false);
                if (players.FirstOrDefault(p => string.Equals(p.Account?.LoginName, loginName, StringComparison.OrdinalIgnoreCase)) is { } occupant)
                {
                    return InUse(loginName, $"{DescribeOccupant(occupant)} on game server {serverIdOfContext}");
                }
            }

            if (!await context.LoginServer.TryLoginAsync(loginName, context.Id).ConfigureAwait(false))
            {
                return InUse(loginName, "a connected client or another game server");
            }

            loginServerAcquired = true;

            var actor = await this._factory.CreateAsync(context, loginName, characterSlot).ConfigureAwait(false);
            if (actor is null)
            {
                return ActorCommandResult.Failure(
                    ActorErrorCodes.SpawnFailed,
                    $"The actor for {loginName} could not be started; see the server log.");
            }

            this._actors[loginName] = actor;
            loginServerAcquired = false;
            return ActorCommandResult.Success(ActorState.Describe(loginName, actor));
        }
        finally
        {
            if (loginServerAcquired
                && this._locator.GetContext(serverId) is { } releaseContext)
            {
                await releaseContext.LoginServer.LogOffAsync(loginName, releaseContext.Id).ConfigureAwait(false);
            }

            this._lock.Release();
        }
    }

    /// <inheritdoc />
    public async ValueTask<ActorCommandResult> StopAsync(string loginName)
    {
        await this._lock.WaitAsync().ConfigureAwait(false);
        ScriptedPlayer? actor;
        try
        {
            if (!this._actors.Remove(loginName, out actor))
            {
                return ActorCommandResult.Failure(ActorErrorCodes.UnknownActor, $"No actor animates {loginName}.");
            }
        }
        finally
        {
            this._lock.Release();
        }

        await this.StopAndDisposeAsync(loginName, actor).ConfigureAwait(false);
        return ActorCommandResult.Success(new ActorEventField("actor", loginName));
    }

    /// <inheritdoc />
    public async ValueTask<int> StopAllAsync()
    {
        await this._lock.WaitAsync().ConfigureAwait(false);
        List<KeyValuePair<string, ScriptedPlayer>> actors;
        try
        {
            actors = this._actors.ToList();
            this._actors.Clear();
        }
        finally
        {
            this._lock.Release();
        }

        foreach (var (loginName, actor) in actors)
        {
            await this.StopAndDisposeAsync(loginName, actor).ConfigureAwait(false);
        }

        return actors.Count;
    }

    /// <inheritdoc />
    public IReadOnlyList<ScriptedPlayer> List()
    {
        this._lock.Wait();
        try
        {
            return this._actors.Values.ToList();
        }
        finally
        {
            this._lock.Release();
        }
    }

    /// <inheritdoc />
    public ScriptedPlayer? Find(string loginName)
    {
        this._lock.Wait();
        try
        {
            return this._actors.GetValueOrDefault(loginName);
        }
        finally
        {
            this._lock.Release();
        }
    }

    private static string DescribeOccupant(Player occupant)
    {
        return occupant switch
        {
            ScriptedPlayer => "an actor",
            _ when occupant.Account?.IsBot == true => "a bot",
            OfflinePlayer => "an offline session",
            _ => "a connected client",
        };
    }

    private static ActorCommandResult InUse(string loginName, string by)
        => ActorCommandResult.Failure(
            ActorErrorCodes.InUse,
            $"The account {loginName} is already in use by {by}.",
            new ActorEventField("actor", loginName));

    private async ValueTask StopAndDisposeAsync(string loginName, ScriptedPlayer actor)
    {
        var context = actor.GameContext as IGameServerContext;
        try
        {
            await actor.StopAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error while stopping the actor {Actor}.", loginName);
        }
        finally
        {
            if (context is not null)
            {
                await context.LoginServer.LogOffAsync(loginName, context.Id).ConfigureAwait(false);
            }

            await actor.DisposeAsync().ConfigureAwait(false);
            this._logger.LogInformation("Actor {Actor} stopped.", loginName);
        }
    }
}
