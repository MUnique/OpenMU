// <copyright file="AttackableNpcBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC;

using System.Diagnostics;
using System.Threading;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// An abstract base class for an <see cref="IAttackable"/> <see cref="NonPlayerCharacter"/>.
/// </summary>
public abstract class AttackableNpcBase : NonPlayerCharacter, IAttackable
{
    private readonly IEventStateProvider? _eventStateProvider;
    private readonly IDropGenerator _dropGenerator;
    private readonly PlugInManager _plugInManager;
    private int _health;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttackableNpcBase" /> class.
    /// </summary>
    /// <param name="spawnInfo">The spawn information.</param>
    /// <param name="stats">The stats.</param>
    /// <param name="map">The map.</param>
    /// <param name="eventStateProvider">The event state provider.</param>
    /// <param name="dropGenerator">The drop generator.</param>
    /// <param name="plugInManager">The plug in manager.</param>
    protected AttackableNpcBase(MonsterSpawnArea spawnInfo, MonsterDefinition stats, GameMap map, IEventStateProvider? eventStateProvider, IDropGenerator dropGenerator, PlugInManager plugInManager)
        : base(spawnInfo, stats, map)
    {
        this._eventStateProvider = eventStateProvider;
        this._dropGenerator = dropGenerator;
        this._plugInManager = plugInManager;
        this.MagicEffectList = new MagicEffectsList(this);
        this.Attributes = new MonsterAttributeHolder(this);
    }

    /// <summary>
    /// Occurs when this instance died.
    /// </summary>
    public event EventHandler<DeathInformation>? Died;

    /// <inheritdoc />
    public IAttributeSystem Attributes { get; }

    /// <inheritdoc />
    public MagicEffectsList MagicEffectList { get; }

    /// <inheritdoc />
    public bool IsAlive { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="IAttackable" /> is currently teleporting and can't be directly targeted.
    /// It can still receive damage, if the teleport target coordinates are within an target skill area for area attacks.
    /// </summary>
    /// <value>
    ///   <c>true</c> if teleporting; otherwise, <c>false</c>.
    /// </value>
    /// <remarks>Teleporting for monsters oor npcs is not implemented yet.</remarks>
    public bool IsTeleporting => false;

    /// <inheritdoc />
    public DeathInformation? LastDeath { get; protected set; }

    /// <inheritdoc/>
    public override Point Position
    {
        get => base.Position;
        set
        {
            if (base.Position != value)
            {
                base.Position = value;
                this._plugInManager?.GetPlugInPoint<IAttackableMovedPlugIn>()?.AttackableMoved(this);
            }
        }
    }

    /// <summary>
    /// Gets or sets the current health.
    /// </summary>
    public int Health
    {
        get => Math.Max(this._health, 0);
        set => this._health = value;
    }

    private bool ShouldRespawn => this.SpawnArea.SpawnTrigger == SpawnTrigger.Automatic
                                  || (this.SpawnArea.SpawnTrigger == SpawnTrigger.AutomaticDuringEvent && (this._eventStateProvider?.IsEventRunning ?? false))
                                  || (this.SpawnArea.SpawnTrigger == SpawnTrigger.AutomaticDuringWave && (this._eventStateProvider?.IsSpawnWaveActive(this.SpawnArea.WaveNumber) ?? false));

    /// <inheritdoc />
    public async ValueTask<HitInfo?> AttackByAsync(IAttacker attacker, SkillEntry? skill, bool isCombo, double damageFactor = 1.0)
    {
        if (this.Definition.ObjectKind == NpcObjectKind.Guard)
        {
            return null;
        }

        if (this.Attributes[Stats.IsAsleep] > 0)
        {
            await this.MagicEffectList.ClearAllEffectsProducingSpecificStatAsync(Stats.IsAsleep).ConfigureAwait(false);
        }

        var hitInfo = await attacker.CalculateDamageAsync(this, skill, isCombo, damageFactor).ConfigureAwait(false);
        await this.HitAsync(hitInfo, attacker, skill?.Skill).ConfigureAwait(false);
        if (hitInfo.HealthDamage > 0)
        {
            attacker.ApplyAmmunitionConsumption(hitInfo);
            if (attacker is Player player)
            {
                await player.AfterHitTargetAsync().ConfigureAwait(false);
            }

            if (attacker as IPlayerSurrogate is { } playerSurrogate)
            {
                await playerSurrogate.Owner.AfterHitTargetAsync().ConfigureAwait(false);
            }
        }

        return hitInfo;
    }

    /// <inheritdoc />
    public abstract ValueTask ReflectDamageAsync(IAttacker reflector, uint damage);

    /// <inheritdoc />
    public abstract ValueTask ApplyPoisonDamageAsync(IAttacker initialAttacker, uint damage);

    /// <inheritdoc/>
    public ValueTask KillInstantlyAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        this.Health = this.SpawnArea.MaximumHealthOverride ?? (int)this.Attributes[Stats.MaximumHealth];
        this.IsAlive = true;
    }

    /// <inheritdoc/>
    protected override void Dispose(bool managed)
    {
        if (managed)
        {
            this.Died = null;
            this.IsAlive = false;
        }

        base.Dispose(managed);
    }

    /// <summary>
    /// Called when the object is removed from the map.
    /// </summary>
    protected virtual void OnRemoveFromMap()
    {
        // can be overwritten to do additional stuff.
    }

    /// <summary>
    /// Hits this instance with the specified hit information.
    /// </summary>
    /// <param name="hitInfo">The hit information.</param>
    /// <param name="attacker">The attacker.</param>
    /// <param name="skill">The skill.</param>
    protected async ValueTask HitAsync(HitInfo hitInfo, IAttacker attacker, Skill? skill)
    {
        if (!this.IsAlive)
        {
            return;
        }

        var killed = this.TryHit(hitInfo.HealthDamage + hitInfo.ShieldDamage, attacker);

        var player = this.GetHitNotificationTarget(attacker);
        if (player is not null)
        {
            await player.InvokeViewPlugInAsync<IShowHitPlugIn>(p => p.ShowHitAsync(this, hitInfo)).ConfigureAwait(false);
            player.GameContext.PlugInManager.GetPlugInPoint<IAttackableGotHitPlugIn>()?.AttackableGotHit(this, attacker, hitInfo);
        }

        if (killed)
        {
            this.LastDeath = new DeathInformation(attacker.Id, attacker.GetName(), hitInfo, skill?.Number ?? 0);
            await this.OnDeathAsync(attacker).ConfigureAwait(false);
            this.Died?.Invoke(this, this.LastDeath);
            if (!this.ShouldRespawn)
            {
                await this.RemoveFromMapAndDisposeAsync().ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Gets the target of a hit notification.
    /// </summary>
    /// <param name="attacker">The attacker.</param>
    /// <returns>The target player of a hit notification.</returns>
    protected virtual Player? GetHitNotificationTarget(IAttacker attacker)
    {
        return attacker as Player ?? (attacker as IPlayerSurrogate)?.Owner;
    }

    /// <summary>
    /// Registers the hit.
    /// </summary>
    /// <param name="attacker">The attacker.</param>
    protected virtual void RegisterHit(IAttacker attacker)
    {
        // can be overwritten
    }

    private async ValueTask RemoveFromMapAndDisposeAsync()
    {
        await this.CurrentMap.RemoveAsync(this).ConfigureAwait(false);
        this.Dispose();
        this.OnRemoveFromMap();
    }

    /// <summary>
    /// Respawns this instance on the map.
    /// </summary>
    private async ValueTask RespawnAsync()
    {
        try
        {
            if (!this.ShouldRespawn)
            {
                await this.RemoveFromMapAndDisposeAsync().ConfigureAwait(false);
                return;
            }

            this.Initialize();
            await this.CurrentMap.RespawnAsync(this).ConfigureAwait(false);
            this.OnSpawn();
        }
        catch (Exception ex)
        {
            Debug.Fail(ex.Message, ex.StackTrace);
        }
    }

    private bool TryHit(uint damage, IAttacker attacker)
    {
        if (damage > 0)
        {
            this.RegisterHit(attacker);
        }

        if (damage >= this.Health)
        {
            this.IsAlive = false;
            this.Health = 0;
            return true;
        }

        try
        {
            Interlocked.Add(ref this._health, -(int)damage);
            return false;
        }
        catch
        {
            return false;
        }
    }

    private async ValueTask HandleMoneyDropAsync(uint amount, Player killer)
    {
        // We don't drop money in Devil Square, etc.
        var shouldDropMoney = killer.GameContext.Configuration.ShouldDropMoney && killer.CurrentMiniGame is null;
        if (!shouldDropMoney)
        {
            var party = killer.Party;
            if (party is null)
            {
                killer.TryAddMoney((int)amount);
            }
            else
            {
                await party.DistributeMoneyAfterKillAsync(this, killer, amount).ConfigureAwait(false);
            }

            return;
        }

        var droppedMoney = new DroppedMoney((uint)(amount * killer.Attributes![Stats.MoneyAmountRate]), this.Position, this.CurrentMap);
        await this.CurrentMap.AddAsync(droppedMoney).ConfigureAwait(false);
    }

    private async ValueTask DropItemAsync(int exp, Player killer)
    {
        var (generatedItems, droppedMoney) = await this._dropGenerator.GenerateItemDropsAsync(this.Definition, exp, killer).ConfigureAwait(false);
        if (droppedMoney > 0)
        {
            await this.HandleMoneyDropAsync(droppedMoney.Value, killer).ConfigureAwait(false);
        }

        var firstItem = !droppedMoney.HasValue;
        foreach (var item in generatedItems)
        {
            Point dropCoordinates;
            if (firstItem)
            {
                dropCoordinates = this.Position;
                firstItem = false;
            }
            else
            {
                dropCoordinates = this.CurrentMap.Terrain.GetRandomCoordinate(this.Position, 4);
            }

            var owners = killer.Party?.PartyList.AsEnumerable() ?? killer.GetAsEnumerable();
            var droppedItem = new DroppedItem(item, dropCoordinates, this.CurrentMap, null, owners);
            await this.CurrentMap.AddAsync(droppedItem).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Called when this instance died.
    /// </summary>
    /// <param name="attacker">The attacker which killed this instance.</param>
    protected virtual async ValueTask OnDeathAsync(IAttacker attacker)
    {
        if (this.ShouldRespawn)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(this.Definition.RespawnDelay).ConfigureAwait(false);
                    await this.RespawnAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Debug.Fail($"Unexpected error during respawning the attackable npc {this}: {ex}", ex.StackTrace);
                }
            });
        }

        await this.ForEachWorldObserverAsync<IObjectGotKilledPlugIn>(p => p.ObjectGotKilledAsync(this, attacker), true).ConfigureAwait(false);

        var player = this.GetHitNotificationTarget(attacker);
        if (player is { })
        {
            int exp = await ((player.Party?.DistributeExperienceAfterKillAsync(this, player) ?? player.AddExpAfterKillAsync(this)).ConfigureAwait(false));
            if (attacker == player)
            {
                await player.AfterKilledMonsterAsync().ConfigureAwait(false);
            }

            if (player.GameContext.PlugInManager.GetPlugInPoint<IAttackableGotKilledPlugIn>() is { } plugInPoint)
            {
                await plugInPoint.AttackableGotKilledAsync(this, attacker);
            }

            if (player.SelectedCharacter!.State > HeroState.Normal)
            {
                player.SelectedCharacter.StateRemainingSeconds -= (int)this.Attributes[Stats.Level];
            }

            _ = this.DropItemDelayedAsync(player, exp); // don't wait for completion.
        }
    }

    private async ValueTask DropItemDelayedAsync(Player player, int gainedExp)
    {
        try
        {
            await Task.Delay(1000).ConfigureAwait(false);
            await this.DropItemAsync(gainedExp, player).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogDebug(ex, "Dropping an item failed after killing '{this}': {ex}", this, ex);
        }
    }
}
