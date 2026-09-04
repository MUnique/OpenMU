// <copyright file="CastleSiegeLifeStone.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.NPC;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// An attackable, guild-owned Life Stone which heals nearby allies after its creation phase.
/// </summary>
/// <remarks>
/// The guild identity is captured when the stone is placed. Player movement and later guild-roster changes do not
/// remove it; it remains until it is destroyed, its guild captures the Crown, or the battle ends.
/// </remarks>
public sealed class CastleSiegeLifeStone : AttackableNpcBase
{
    private const byte CompletedBuildTime = 5;
    private const int HealingRange = 3;
    private const int HealingPercentage = 1;
    private const int PercentageBase = 100;
    private static readonly TimeSpan BuildStageInterval = TimeSpan.FromSeconds(12);
    private static readonly TimeSpan HealingInterval = TimeSpan.FromSeconds(1);
    private DateTime _nextHealingUtc;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeLifeStone"/> class.
    /// </summary>
    /// <param name="spawnInfo">The spawn information.</param>
    /// <param name="stats">The NPC definition.</param>
    /// <param name="map">The map on which the Life Stone is spawned.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="ownerGuildId">The runtime identifier of the guild which placed the Life Stone.</param>
    /// <param name="joinSide">The Castle Siege side of the owning guild.</param>
    /// <param name="createdAtUtc">The UTC time at which creation began.</param>
    /// <param name="dropGenerator">The drop generator.</param>
    /// <param name="plugInManager">The plug-in manager.</param>
    public CastleSiegeLifeStone(
        MonsterSpawnArea spawnInfo,
        MonsterDefinition stats,
        GameMap map,
        CastleSiegeContext context,
        uint ownerGuildId,
        CastleSiegeJoinSide joinSide,
        DateTime createdAtUtc,
        IDropGenerator dropGenerator,
        PlugInManager plugInManager)
        : base(spawnInfo, stats, map, context, dropGenerator, plugInManager)
    {
        this.Context = context;
        this.OwnerGuildId = ownerGuildId;
        this.JoinSide = joinSide;
        this.CreatedAtUtc = createdAtUtc;
    }

    /// <summary>
    /// Gets the monster number of Life Stones.
    /// </summary>
    public static short MonsterNumber { get; } = 278;

    /// <summary>
    /// Gets the guild which owns this Life Stone.
    /// </summary>
    public uint OwnerGuildId { get; }

    /// <summary>
    /// Gets the Castle Siege side which may receive healing from this Life Stone.
    /// </summary>
    public CastleSiegeJoinSide JoinSide { get; }

    /// <summary>
    /// Gets the client-visible Life Stone creation stage.
    /// </summary>
    public byte BuildTime { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the Life Stone finished its creation phase.
    /// </summary>
    public bool IsActive => this.BuildTime >= CompletedBuildTime;

    /// <summary>
    /// Gets the Castle Siege context which owns this Life Stone.
    /// </summary>
    internal CastleSiegeContext Context { get; }

    /// <summary>
    /// Gets the UTC time at which the creation phase began.
    /// </summary>
    internal DateTime CreatedAtUtc { get; }

    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();
        if (this.Attributes[Stats.MaximumHealth] <= 0)
        {
            throw new InvalidOperationException("The Life Stone monster definition requires a positive maximum health.");
        }
    }

    /// <inheritdoc />
    public override ValueTask ReflectDamageAsync(IAttacker reflector, uint damage) => ValueTask.CompletedTask;

    /// <inheritdoc />
    public override ValueTask ApplyPoisonDamageAsync(IAttacker initialAttacker, uint damage) => ValueTask.CompletedTask;

    /// <inheritdoc />
    public override ValueTask ApplyBleedingDamageAsync(IAttacker initialAttacker, uint damage) => ValueTask.CompletedTask;

    /// <summary>
    /// Sends the current creation phase to all current observers.
    /// </summary>
    /// <returns>A task that represents the notification.</returns>
    internal ValueTask BroadcastBuildTimeAsync()
    {
        return this.ForEachWorldObserverAsync<ICastleSiegeLifeStoneStatePlugIn>(
            view => view.ShowLifeStoneBuildTimeAsync(this.Id, this.BuildTime),
            true);
    }

    /// <summary>
    /// Destroys this Life Stone without creating loot or experience rewards.
    /// </summary>
    /// <returns>A task that represents the destruction operation.</returns>
    internal async ValueTask DestroyAsync()
    {
        this.Context.RemoveLifeStone(this);
        if (this.CurrentMap.GetObject(this.Id) is not null)
        {
            await this.CurrentMap.RemoveAsync(this).ConfigureAwait(false);
        }

        this.Dispose();
        this.OnRemoveFromMap();
    }

    /// <summary>
    /// Executes one creation/healing tick.
    /// </summary>
    /// <param name="utcNow">The current UTC time.</param>
    /// <returns>A task that represents the update operation.</returns>
    internal async ValueTask TickAsync(DateTime utcNow)
    {
        if (!this.IsAlive || this.Context.CurrentState != CastleSiegeState.Start)
        {
            return;
        }

        var buildTime = (byte)Math.Min(
            CompletedBuildTime,
            Math.Max(0, (utcNow - this.CreatedAtUtc).Ticks / BuildStageInterval.Ticks));
        if (buildTime > this.BuildTime)
        {
            this.BuildTime = buildTime;
            await this.BroadcastBuildTimeAsync().ConfigureAwait(false);
        }

        if (!this.IsActive || utcNow < this._nextHealingUtc)
        {
            return;
        }

        this._nextHealingUtc = utcNow + HealingInterval;
        foreach (var player in this.Context.GetSiegePlayers())
        {
            if (player is not { IsAlive: true, Attributes: { } attributes }
                || this.Context.GetPlayerJoinSide(player) != this.JoinSide
                || !player.IsInRange(this.Position, HealingRange))
            {
                continue;
            }

            RestorePercentage(attributes, Stats.CurrentHealth, Stats.MaximumHealth);
            RestorePercentage(attributes, Stats.CurrentMana, Stats.MaximumMana);
            RestorePercentage(attributes, Stats.CurrentAbility, Stats.MaximumAbility);
        }
    }

    /// <inheritdoc />
    protected override bool CanBeAttackedBy(IAttacker attacker)
    {
        var player = attacker as Player ?? (attacker as IPlayerSurrogate)?.Owner;
        if (this.Context.CurrentState != CastleSiegeState.Start || player is not { IsAlive: true })
        {
            return false;
        }

        var side = this.Context.GetPlayerJoinSide(player);
        return side is not CastleSiegeJoinSide.None && side != this.JoinSide;
    }

    /// <inheritdoc />
    protected override async ValueTask OnDeathAsync(IAttacker attacker)
    {
        // Deliberately skip the base implementation: Life Stones must not respawn, grant experience, drop loot,
        // or invoke generic attackable-kill rewards. Observers still need the standard death notification.
        this.Context.RemoveLifeStone(this);
        await this.ForEachWorldObserverAsync<IObjectGotKilledPlugIn>(
                view => view.ObjectGotKilledAsync(this, attacker),
                true)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override void Dispose(bool managed)
    {
        if (managed)
        {
            this.Context.RemoveLifeStone(this);
        }

        base.Dispose(managed);
    }

    private static void RestorePercentage(
        IAttributeSystem attributes,
        AttributeDefinition currentAttribute,
        AttributeDefinition maximumAttribute)
    {
        var maximum = attributes[maximumAttribute];
        if (maximum <= 0)
        {
            return;
        }

        var restored = maximum * HealingPercentage / PercentageBase;
        attributes[currentAttribute] = Math.Min(maximum, attributes[currentAttribute] + restored);
    }
}
