// <copyright file="ScriptedIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

using System.Threading;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Chat;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// What the MU Helper AI is to an offline player, this is to a scripted actor: the one place which
/// drives the character - except that it executes commands instead of hunting.
/// </summary>
/// <remarks>
/// Every command runs inside <see cref="Player.RunPersistenceExclusiveAsync(Func{ValueTask}, CancellationToken)"/>,
/// so a command never overlaps the periodic save or another command, and the engine's non-thread-safe
/// attribute system is only ever touched from this one flow. Long commands (a walk, a repeated attack)
/// release the lock between their steps, so a <c>halt</c> or a later command can interrupt them: the
/// interrupted command answers its caller with <see cref="ActorErrorCodes.Interrupted"/> and the
/// progress it made, and an <c>interrupted</c> event is recorded.
/// </remarks>
public sealed class ScriptedIntelligence : IAsyncDisposable
{
    private const byte MeleeAttackRange = 1;
    private const byte BowAttackRange = 6;

    /// <summary>How many steps one <see cref="Player.WalkToAsync"/> call takes at most.</summary>
    private const int MaxStepsPerWalk = 16;

    private const int WalkPollMilliseconds = 100;

    /// <summary>
    /// How many nodes the actor's path finder may expand before it gives up on a target.
    /// </summary>
    private const int PathSearchLimit = 20000;

    private static readonly TargetedSkillDefaultPlugin DefaultSkillPlugin = new();

    private readonly ScriptedPlayer _player;
    private readonly Channel<PendingCommand> _queue;
    private readonly CancellationTokenSource _stopSource = new();
    private readonly object _syncRoot = new();

    private CancellationTokenSource? _currentSource;
    private Task? _loopTask;
    private PathFinder? _pathFinder;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptedIntelligence"/> class.
    /// </summary>
    /// <param name="player">The actor this intelligence drives.</param>
    public ScriptedIntelligence(ScriptedPlayer player)
    {
        this._player = player;
        this._queue = Channel.CreateUnbounded<PendingCommand>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false,
        });
    }

    /// <summary>
    /// Starts the command loop.
    /// </summary>
    public void Start()
    {
        this._loopTask ??= Task.Run(() => this.RunAsync(this._stopSource.Token));
    }

    /// <summary>
    /// Queues a command. A command which is still in flight is interrupted first: at most one long
    /// command runs per actor, and the last one wins.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <returns>The result of the command, once it ran.</returns>
    public ValueTask<ActorCommandResult> ExecuteAsync(ActorCommand command)
    {
        this.CancelCurrent();
        var pending = new PendingCommand(command);
        if (!this._queue.Writer.TryWrite(pending))
        {
            return ValueTask.FromResult(ActorCommandResult.Failure(
                ActorErrorCodes.NotReady,
                "The actor is stopping and does not accept commands any more."));
        }

        return new ValueTask<ActorCommandResult>(pending.Completion.Task);
    }

    /// <summary>
    /// Cancels the command in flight, keeping the actor in the world.
    /// </summary>
    /// <returns><c>true</c> if there was a command to interrupt.</returns>
    public bool Halt()
    {
        return this.CancelCurrent();
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        this._queue.Writer.TryComplete();
        await this._stopSource.CancelAsync().ConfigureAwait(false);
        if (this._loopTask is { } loopTask)
        {
            try
            {
                await loopTask.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._player.Logger.LogError(ex, "The command loop of actor {Actor} failed while stopping.", this._player.AccountLoginName);
            }
        }

        this._stopSource.Dispose();
    }

    private static byte GetEffectiveAttackRange(Player player)
    {
        // The same rule the MU Helper's CombatHandler applies: there is no attack-range attribute.
        if (player.Attributes is { } attributes
            && (attributes[Stats.IsBowEquipped] > 0 || attributes[Stats.IsCrossBowEquipped] > 0))
        {
            return BowAttackRange;
        }

        return MeleeAttackRange;
    }

    private bool CancelCurrent()
    {
        CancellationTokenSource? source;
        lock (this._syncRoot)
        {
            source = this._currentSource;
        }

        if (source is null || source.IsCancellationRequested)
        {
            return false;
        }

        source.Cancel();
        return true;
    }

    private async Task RunAsync(CancellationToken stopToken)
    {
        try
        {
            await foreach (var pending in this._queue.Reader.ReadAllAsync(stopToken).ConfigureAwait(false))
            {
                using var commandSource = CancellationTokenSource.CreateLinkedTokenSource(stopToken);
                lock (this._syncRoot)
                {
                    this._currentSource = commandSource;
                }

                ActorCommandResult result;
                try
                {
                    result = await this.ExecuteCommandAsync(pending.Command, commandSource.Token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    this._player.Logger.LogError(ex, "Actor {Actor} failed to execute {Command}.", this._player.AccountLoginName, pending.Command.Name);
                    result = ActorCommandResult.Failure(ActorErrorCodes.Failed, ex.Message);
                }
                finally
                {
                    lock (this._syncRoot)
                    {
                        this._currentSource = null;
                    }
                }

                this.LogOutcome(pending.Command, result);
                pending.Completion.TrySetResult(result);
            }
        }
        catch (OperationCanceledException)
        {
            // The actor is stopping.
        }
        finally
        {
            while (this._queue.Reader.TryRead(out var pending))
            {
                pending.Completion.TrySetResult(ActorCommandResult.Failure(
                    ActorErrorCodes.NotReady,
                    "The actor stopped before the command could run."));
            }
        }
    }

    private ValueTask<ActorCommandResult> ExecuteCommandAsync(ActorCommand command, CancellationToken cancellationToken)
    {
        if (this._player.PlayerState.CurrentState != PlayerState.EnteredWorld)
        {
            return ValueTask.FromResult(ActorCommandResult.Failure(
                ActorErrorCodes.NotReady,
                $"The actor is in state '{this._player.PlayerState.CurrentState.Name}'."));
        }

        if (!this._player.IsAlive)
        {
            return ValueTask.FromResult(ActorCommandResult.Failure(ActorErrorCodes.Dead, "The actor is dead."));
        }

        return command switch
        {
            WalkCommand walk => this.WalkAsync(walk, cancellationToken),
            AttackCommand attack => this.AttackAsync(attack, cancellationToken),
            SkillCommand skill => this.SkillAsync(skill),
            SayCommand say => this.SayAsync(say),
            PickupCommand pickup => this.PickupAsync(pickup),
            WarpCommand warp => this.WarpAsync(warp),
            _ => ValueTask.FromResult(ActorCommandResult.Failure(ActorErrorCodes.Failed, $"Unknown command '{command.Name}'.")),
        };
    }

    private async ValueTask<ActorCommandResult> WalkAsync(WalkCommand command, CancellationToken cancellationToken)
    {
        var target = new Point(command.X, command.Y);
        var start = this._player.Position;
        if (start == target)
        {
            // Already there: a walk to the current tile is done, not a missing path.
            return ActorCommandResult.Success(
                new ActorEventField("steps", 0),
                new ActorEventField("walked", 0),
                new ActorEventField("x", start.X),
                new ActorEventField("y", start.Y));
        }

        var path = this.FindPath(target);
        if (path is null || path.Count == 0)
        {
            return ActorCommandResult.Failure(
                ActorErrorCodes.NoPath,
                $"No path from {start} to {target}.");
        }

        // The engine walks at most a handful of steps per request, so a long path is handed over in
        // chunks - the actor stays interruptible between them, and the persistence lock is only held
        // while a chunk is handed over, never while walking it.
        var walked = 0;
        for (var offset = 0; offset < path.Count; offset += MaxStepsPerWalk)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return await this.InterruptWalkAsync(path.Count, walked).ConfigureAwait(false);
            }

            var chunk = path.Skip(offset).Take(MaxStepsPerWalk).ToList();
            await this._player.RunPersistenceExclusiveAsync(
                () => this.StartWalkAsync(chunk),
                CancellationToken.None).ConfigureAwait(false);

            while (this._player.IsWalking)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return await this.InterruptWalkAsync(path.Count, walked).ConfigureAwait(false);
                }

                try
                {
                    await Task.Delay(WalkPollMilliseconds, cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // Handled by the check at the top of the loop.
                }
            }

            walked += chunk.Count;
        }

        return ActorCommandResult.Success(
            new ActorEventField("steps", path.Count),
            new ActorEventField("walked", walked),
            new ActorEventField("x", this._player.Position.X),
            new ActorEventField("y", this._player.Position.Y));
    }

    private async ValueTask<ActorCommandResult> InterruptWalkAsync(int planned, int walked)
    {
        await this._player.StopWalkingAsync().ConfigureAwait(false);
        return this.Interrupted(
            "walk",
            new ActorEventField("steps", planned),
            new ActorEventField("walked", walked),
            new ActorEventField("x", this._player.Position.X),
            new ActorEventField("y", this._player.Position.Y));
    }

    /// <summary>
    /// Finds the way to the target over the whole map.
    /// </summary>
    /// <remarks>
    /// Deliberately NOT the game context's pooled path finders: those use a
    /// <see cref="ScopedGridNetwork"/> whose 16 tile segment both caps a request at about 12 tiles
    /// and refuses some shorter ones outright, because the segment is placed around the midpoint and
    /// can leave the start tile outside it. A scenario would have to chain lucky waypoints. The
    /// actor keeps its own full-grid path finder instead, so one <c>walk</c> is one request to
    /// anywhere on the current map, and the pooled finders stay available for the engine.
    /// Safe-zone tiles are included, or an actor standing in town could not move at all.
    /// </remarks>
    private IList<PathResultNode>? FindPath(Point target)
    {
        if (this._player.CurrentMap is not { } map)
        {
            return null;
        }

        this._pathFinder ??= new PathFinder(new FullGridNetwork(true))
        {
            SearchLimit = PathSearchLimit,
            Heuristic = new ActorPathHeuristic(),
        };

        this._pathFinder.ResetPathFinder();
        return this._pathFinder.FindPath(this._player.Position, target, map.Terrain.AIgrid, true);
    }

    private async ValueTask StartWalkAsync(IList<PathResultNode> chunk)
    {
        var steps = new WalkingStep[chunk.Count];
        for (var i = 0; i < chunk.Count; i++)
        {
            var previous = i == 0 ? this._player.Position : steps[i - 1].To;
            steps[i] = new WalkingStep(previous, chunk[i].Point, previous.GetDirectionTo(chunk[i].Point));
        }

        await this._player.WalkToAsync(steps[^1].To, steps).ConfigureAwait(false);
    }

    private async ValueTask<ActorCommandResult> AttackAsync(AttackCommand command, CancellationToken cancellationToken)
    {
        var times = Math.Max(1, command.Times);
        var hits = new List<object?>();
        string? stoppedBecause = null;

        for (var i = 0; i < times; i++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return this.Interrupted("attack", new ActorEventField("hits", hits));
            }

            var attempt = await this._player.RunPersistenceExclusiveAsync(
                () => this.AttackOnceAsync(command.Target),
                CancellationToken.None).ConfigureAwait(false);

            if (attempt.Failure is { } failure)
            {
                if (i == 0)
                {
                    return failure;
                }

                stoppedBecause = failure.Code;
                break;
            }

            hits.Add(attempt.Hit);

            if (i + 1 < times && command.IntervalMs > 0)
            {
                try
                {
                    await Task.Delay(command.IntervalMs, cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    return this.Interrupted("attack", new ActorEventField("hits", hits));
                }
            }
        }

        return ActorCommandResult.Success(
            new ActorEventField("hits", hits),
            new ActorEventField("stopped_because", stoppedBecause));
    }

    private async ValueTask<(ActorCommandResult? Failure, Dictionary<string, object?>? Hit)> AttackOnceAsync(string targetSpec)
    {
        var (target, failure) = this.ResolveAttackTarget(targetSpec, GetEffectiveAttackRange(this._player));
        if (failure is not null || target is null)
        {
            return (failure ?? ActorCommandResult.Failure(ActorErrorCodes.NotInView, $"{targetSpec} is not in view."), null);
        }

        var hitInfo = await target.AttackByAsync(this._player, null, false).ConfigureAwait(false);
        this._player.Logger.LogInformation(
            "Actor {Actor} ({Character}) attacked {Target} (id {TargetId}): {Damage} damage.",
            this._player.AccountLoginName,
            this._player.Name,
            ActorObjects.GetName(target),
            target.Id,
            hitInfo?.HealthDamage ?? 0);

        return (null, new Dictionary<string, object?>
        {
            ["target_id"] = target.Id,
            ["target"] = ActorObjects.GetName(target),
            ["target_kind"] = ActorObjects.GetKind(target),
            ["health_damage"] = hitInfo?.HealthDamage ?? 0,
            ["shield_damage"] = hitInfo?.ShieldDamage ?? 0,
            ["miss"] = hitInfo is null || (hitInfo.Value.HealthDamage == 0 && hitInfo.Value.ShieldDamage == 0),
        });
    }

    private async ValueTask<ActorCommandResult> SkillAsync(SkillCommand command)
    {
        return await this._player.RunPersistenceExclusiveAsync(
            async () =>
            {
                if (this._player.SkillList?.GetSkill(command.SkillNumber) is not { Skill: { } skill } skillEntry)
                {
                    return ActorCommandResult.Failure(
                        ActorErrorCodes.UnknownSkill,
                        $"The character has not learned skill {command.SkillNumber}.");
                }

                var range = skill.Range > 0 ? (byte)skill.Range : GetEffectiveAttackRange(this._player);
                var (target, failure) = this.ResolveAttackTarget(command.Target, range);
                if (failure is not null || target is null)
                {
                    return failure ?? ActorCommandResult.Failure(ActorErrorCodes.NotInView, $"'{command.Target}' is not in view.");
                }

                if (this.FindUnaffordableRequirement(skill, skillEntry) is { } missing)
                {
                    return ActorCommandResult.Failure(
                        ActorErrorCodes.InsufficientResources,
                        $"The character cannot pay the {missing} cost of skill {command.SkillNumber}.");
                }

                var strategy = this._player.GameContext.PlugInManager.GetStrategy<short, ITargetedSkillPlugin>(skill.Number)
                               ?? DefaultSkillPlugin;

                // The engine's skill plugin answers a refused cast by returning silently (stunned,
                // safe zone, target restriction, missing mana or requirements, the speed-hack check).
                // A command must never report a no-op as success, so the actor's own stream decides:
                // a cast which got as far as the skill animation appended a 'skill' event, and one
                // which landed appended a 'hit' event as well.
                var sequenceBefore = this._player.EventLog.LastSequence;
                await strategy.PerformSkillAsync(this._player, target, command.SkillNumber).ConfigureAwait(false);
                var recorded = this._player.EventLog.Since(sequenceBefore);
                var performed = recorded.Any(e => e.Type is "skill" or "hit");
                if (!performed)
                {
                    var refusal = $"The game refused skill {command.SkillNumber} without a reason: check the character's mana,"
                                  + " its skill requirements (weapon, level), the target's restrictions, and that it is not in a safe zone.";
                    return ActorCommandResult.Failure(
                        ActorErrorCodes.SkillRefused,
                        refusal,
                        new ActorEventField("skill", skill.Number),
                        new ActorEventField("target_id", target.Id));
                }

                var hit = recorded.FirstOrDefault(e => e.Type == "hit");
                var healthDamage = hit?.Fields.FirstOrDefault(f => f.Name == "health_damage").Value ?? 0u;

                this._player.Logger.LogInformation(
                    "Actor {Actor} ({Character}) cast skill {Skill} on {Target} (id {TargetId}).",
                    this._player.AccountLoginName,
                    this._player.Name,
                    skill.Number,
                    ActorObjects.GetName(target),
                    target.Id);

                return ActorCommandResult.Success(
                    new ActorEventField("skill", skill.Number),
                    new ActorEventField("target_id", target.Id),
                    new ActorEventField("target", ActorObjects.GetName(target)),
                    new ActorEventField("target_alive", target.IsAlive),
                    new ActorEventField("hit", hit is not null),
                    new ActorEventField("health_damage", healthDamage));
            },
            CancellationToken.None).ConfigureAwait(false);
    }

    private async ValueTask<ActorCommandResult> SayAsync(SayCommand command)
    {
        return await this._player.RunPersistenceExclusiveAsync(
            async () =>
            {
                await new ChatMessageAction()
                    .ChatMessageAsync(this._player, this._player.Name, command.Text, false)
                    .ConfigureAwait(false);
                this._player.Logger.LogInformation(
                    "Actor {Actor} ({Character}) said {Message}.",
                    this._player.AccountLoginName,
                    this._player.Name,
                    command.Text);
                return ActorCommandResult.Success(new ActorEventField("message", command.Text));
            },
            CancellationToken.None).ConfigureAwait(false);
    }

    private async ValueTask<ActorCommandResult> PickupAsync(PickupCommand command)
    {
        return await this._player.RunPersistenceExclusiveAsync(
            async () =>
            {
                if (this._player.CurrentMap?.GetDrop(command.DropId) is null)
                {
                    return ActorCommandResult.Failure(
                        ActorErrorCodes.NotInView,
                        $"There is no drop with id {command.DropId} on this map.");
                }

                await new PickupItemAction().PickupItemAsync(this._player, command.DropId).ConfigureAwait(false);
                var stillThere = this._player.CurrentMap?.GetDrop(command.DropId) is not null;
                if (stillThere)
                {
                    return ActorCommandResult.Failure(
                        ActorErrorCodes.PickupFailed,
                        $"The drop {command.DropId} could not be picked up.");
                }

                this._player.Logger.LogInformation(
                    "Actor {Actor} ({Character}) picked up drop {DropId}.",
                    this._player.AccountLoginName,
                    this._player.Name,
                    command.DropId);
                return ActorCommandResult.Success(new ActorEventField("id", command.DropId));
            },
            CancellationToken.None).ConfigureAwait(false);
    }

    private async ValueTask<ActorCommandResult> WarpAsync(WarpCommand command)
    {
        return await this._player.RunPersistenceExclusiveAsync(
            async () =>
            {
                var warpInfo = this._player.GameContext.Configuration.WarpList
                    .FirstOrDefault(w => w.Index == command.GateNumber);
                if (warpInfo is null)
                {
                    return ActorCommandResult.Failure(
                        ActorErrorCodes.UnknownGate,
                        $"There is no warp list entry with index {command.GateNumber}.");
                }

                var mapBefore = this._player.CurrentMap;
                await new WarpAction().WarpToAsync(this._player, warpInfo).ConfigureAwait(false);
                if (ReferenceEquals(mapBefore, this._player.CurrentMap) && warpInfo.Gate?.Map != mapBefore?.Definition)
                {
                    // WarpAction answers a refused warp with a blue message only; the map tells us.
                    return ActorCommandResult.Failure(
                        ActorErrorCodes.WarpRefused,
                        $"The warp to '{warpInfo.Name}' was refused (level, zen or map rules).");
                }

                this._player.Logger.LogInformation(
                    "Actor {Actor} ({Character}) warped to {Warp}.",
                    this._player.AccountLoginName,
                    this._player.Name,
                    warpInfo.Name);

                return ActorCommandResult.Success(
                    new ActorEventField("warp", warpInfo.Name.ToString()),
                    new ActorEventField("map", this._player.CurrentMap?.Definition.Name.ToString() ?? string.Empty),
                    new ActorEventField("x", this._player.Position.X),
                    new ActorEventField("y", this._player.Position.Y));
            },
            CancellationToken.None).ConfigureAwait(false);
    }

    private string? FindUnaffordableRequirement(Skill skill, SkillEntry skillEntry)
    {
        if (this._player.Attributes is not { } attributes)
        {
            return "attribute";
        }

        foreach (var requirement in skill.ConsumeRequirements)
        {
            if (requirement.Attribute is not { } attribute)
            {
                continue;
            }

            var required = this._player.GetRequiredValue(requirement, skillEntry);
            if (attributes[attribute] < required)
            {
                return attribute.Designation ?? "resource";
            }
        }

        return null;
    }

    private (IAttackable? Target, ActorCommandResult? Failure) ResolveAttackTarget(string targetSpec, byte range)
    {
        if (this._player.CurrentMap is not { } map)
        {
            return (null, ActorCommandResult.Failure(ActorErrorCodes.NotReady, "The actor is not on a map."));
        }

        var candidates = map.GetAttackablesInRange(this._player.Position, this._player.InfoRange);
        var target = ushort.TryParse(targetSpec, out var id)
            ? candidates.FirstOrDefault(c => c.Id == id)
            : candidates.FirstOrDefault(c => string.Equals(ActorObjects.GetName(c), targetSpec, StringComparison.OrdinalIgnoreCase));

        if (target is null)
        {
            return (null, ActorCommandResult.Failure(ActorErrorCodes.NotInView, $"{targetSpec} is not in the actor's view."));
        }

        if (ReferenceEquals(target, this._player))
        {
            return (null, ActorCommandResult.Failure(ActorErrorCodes.InvalidTarget, "An actor cannot attack itself."));
        }

        if (!target.IsAlive)
        {
            return (null, ActorCommandResult.Failure(ActorErrorCodes.InvalidTarget, $"{targetSpec} is dead."));
        }

        if (this._player.IsAtSafezone())
        {
            return (null, ActorCommandResult.Failure(ActorErrorCodes.SafeZone, "The actor stands in a safe zone."));
        }

        if (target.IsAtSafezone())
        {
            return (null, ActorCommandResult.Failure(ActorErrorCodes.SafeZone, $"{targetSpec} stands in a safe zone."));
        }

        if (!target.IsInRange(this._player.Position, range))
        {
            var distance = this._player.Position.EuclideanDistanceTo(target.Position);
            return (null, ActorCommandResult.Failure(
                ActorErrorCodes.OutOfRange,
                $"{targetSpec} is {distance:0.#} tiles away, the attack range is {range}."));
        }

        return (target, null);
    }

    private ActorCommandResult Interrupted(string command, params ActorEventField[] progress)
    {
        this._player.EventLog.Append("interrupted", new ActorEventField("command", command));
        this._player.Logger.LogInformation(
            "The {Command} command of actor {Actor} was interrupted.",
            command,
            this._player.AccountLoginName);
        return ActorCommandResult.Failure(ActorErrorCodes.Interrupted, $"The {command} command was interrupted.", progress);
    }

    private void LogOutcome(ActorCommand command, ActorCommandResult result)
    {
        if (result.Ok || result.Code == ActorErrorCodes.Interrupted)
        {
            return;
        }

        this._player.Logger.LogWarning(
            "Actor {Actor} refused {Command}: {Code} - {Error}",
            this._player.AccountLoginName,
            command.Name,
            result.Code,
            result.Error);
        this._player.EventLog.Append(
            "error",
            new ActorEventField("command", command.Name),
            new ActorEventField("code", result.Code),
            new ActorEventField("error", result.Error));
    }

    private sealed record PendingCommand(ActorCommand Command)
    {
        public TaskCompletionSource<ActorCommandResult> Completion { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }
}
