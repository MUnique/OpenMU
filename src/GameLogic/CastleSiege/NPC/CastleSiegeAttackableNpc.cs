// <copyright file="CastleSiegeAttackableNpc.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.NPC;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Base class for attackable Castle Siege structures.
/// </summary>
public abstract class CastleSiegeAttackableNpc : AttackableNpcBase, ICastleSiegeNpc
{
    private readonly INpcIntelligence _intelligence;
    private readonly SimpleElement _defenseElement = new(0, AggregateType.AddRaw);
    private readonly SimpleElement _maximumHealthElement = new(0, AggregateType.AddRaw);
    private float _baseDefense;
    private float _baseMaximumHealth;
    private bool _statElementsAdded;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeAttackableNpc"/> class.
    /// </summary>
    /// <param name="spawnInfo">The spawn information.</param>
    /// <param name="stats">The NPC definition.</param>
    /// <param name="map">The map on which the NPC is spawned.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="runtime">The Castle Siege runtime entry.</param>
    /// <param name="intelligence">The NPC intelligence.</param>
    /// <param name="dropGenerator">The drop generator.</param>
    /// <param name="plugInManager">The plugin manager.</param>
    protected CastleSiegeAttackableNpc(
        MonsterSpawnArea spawnInfo,
        MonsterDefinition stats,
        GameMap map,
        CastleSiegeContext context,
        CastleSiegeNpcRuntime runtime,
        INpcIntelligence intelligence,
        IDropGenerator dropGenerator,
        PlugInManager plugInManager)
        : base(spawnInfo, stats, map, context, dropGenerator, plugInManager)
    {
        this.Context = context;
        this.Runtime = runtime;
        this._intelligence = intelligence;
        this._intelligence.Npc = this;
    }

    /// <inheritdoc />
    public CastleSiegeNpcRuntime Runtime { get; }

    /// <summary>
    /// Gets the persistent state of this structure.
    /// </summary>
    public CastleSiegeNpcState State =>
        this.Runtime.PersistedState
        ?? throw new InvalidOperationException("An attackable Castle Siege NPC requires a persistent state.");

    /// <summary>
    /// Gets the configured maximum health.
    /// </summary>
    public int MaximumHealth => Math.Max(0, (int)this.Attributes[Stats.MaximumHealth]);

    /// <summary>
    /// Gets the Castle Siege context.
    /// </summary>
    protected CastleSiegeContext Context { get; }

    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();
        var persistedHealth = this.State.CurrentHp;
        if (!this._statElementsAdded)
        {
            this._baseDefense = this.Attributes[Stats.DefenseBase];
            this._baseMaximumHealth = this.Attributes[Stats.MaximumHealth];
            this.Attributes.AddElement(this._defenseElement, Stats.DefenseBase);
            this.Attributes.AddElement(this._maximumHealthElement, Stats.MaximumHealth);
            this._statElementsAdded = true;
        }

        this.ApplyPersistedUpgrades(false);
        this.Health = Math.Clamp(persistedHealth, 0, this.MaximumHealth);
        this.State.CurrentHp = this.Health;
        this.Runtime.IsAlive = this.Health > 0;
    }

    /// <inheritdoc />
    public override void OnSpawn()
    {
        base.OnSpawn();
        this._intelligence.Start();
    }

    /// <summary>
    /// Applies the currently persisted upgrade levels.
    /// </summary>
    /// <param name="preserveMissingHealth">
    /// If set to <see langword="true"/>, a maximum-health increase also increases current health by the same amount.
    /// </param>
    public abstract void ApplyPersistedUpgrades(bool preserveMissingHealth);

    /// <summary>
    /// Restores this structure to full health.
    /// </summary>
    public void RestoreFullHealth()
    {
        this.Health = this.MaximumHealth;
        this.State.CurrentHp = this.Health;
        this.Runtime.IsAlive = true;
    }

    /// <inheritdoc />
    public override ValueTask ReflectDamageAsync(IAttacker reflector, uint damage)
    {
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public override ValueTask ApplyPoisonDamageAsync(IAttacker initialAttacker, uint damage)
    {
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public override ValueTask ApplyBleedingDamageAsync(IAttacker initialAttacker, uint damage)
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Applies the configured defense value to this instance.
    /// </summary>
    /// <param name="value">The target defense value.</param>
    protected void SetDefense(int value)
    {
        this._defenseElement.Value = value - this._baseDefense;
    }

    /// <summary>
    /// Applies the configured maximum-health value to this instance.
    /// </summary>
    /// <param name="value">The target maximum health.</param>
    /// <param name="preserveMissingHealth">
    /// If set to <see langword="true"/>, current health is increased by the maximum-health difference.
    /// </param>
    protected void SetMaximumHealth(int value, bool preserveMissingHealth)
    {
        var oldMaximumHealth = this.MaximumHealth;
        this._maximumHealthElement.Value = value - this._baseMaximumHealth;
        var newMaximumHealth = this.MaximumHealth;
        this.Health = preserveMissingHealth
            ? Math.Clamp(this.Health + newMaximumHealth - oldMaximumHealth, 0, newMaximumHealth)
            : Math.Clamp(this.Health, 0, newMaximumHealth);
        this.State.CurrentHp = this.Health;
    }

    /// <inheritdoc />
    protected override bool CanBeAttackedBy(IAttacker attacker)
    {
        var player = attacker as Player ?? (attacker as IPlayerSurrogate)?.Owner;
        return this.Context.CurrentState == CastleSiegeState.Start
               && player is not null
               && this.Context.GetPlayerJoinSide(player)
                   is not CastleSiegeJoinSide.None and not CastleSiegeJoinSide.Defense;
    }

    /// <inheritdoc />
    protected override async ValueTask OnDeathAsync(IAttacker attacker)
    {
        this.State.CurrentHp = 0;
        this.Runtime.IsAlive = false;
        this.Runtime.SpawnedInstance = null;
        await base.OnDeathAsync(attacker).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override void RegisterHit(IAttacker attacker)
    {
        base.RegisterHit(attacker);
        this._intelligence.RegisterHit(attacker);
    }

    /// <inheritdoc />
    protected override void Dispose(bool managed)
    {
        if (managed)
        {
            this._intelligence.Pause();
            (this._intelligence as IDisposable)?.Dispose();
        }

        base.Dispose(managed);
    }
}
