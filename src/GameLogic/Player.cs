// <copyright file="Player.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Threading;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.GuildWar;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Pet;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.GameLogic.Views.MuHelper;
using MUnique.OpenMU.GameLogic.Views.Pet;
using MUnique.OpenMU.GameLogic.Views.Quest;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;
using Nito.AsyncEx;

/// <summary>
/// The base implementation of a player.
/// </summary>
public class Player : AsyncDisposable, IBucketMapObserver, IAttackable, IAttacker, ITrader, IPartyMember, IRotatable, IHasBucketInformation, ISupportWalk, IMovable, ILoggerOwner<Player>
{
    private readonly AsyncLock _moveLock = new();

    private readonly Walker _walker;

    private readonly AppearanceDataAdapter _appearanceData;

    private readonly ObserverToWorldViewAdapter _observerToWorldViewAdapter;

    private readonly Lazy<MuHelper.MuHelper> _muHelperLazy;

    private CancellationTokenSource? _respawnAfterDeathCts;

    private Character? _selectedCharacter;

    private ICustomPlugInContainer<IViewPlugIn>? _viewPlugIns;

    private DateTime _lastRegenerate = DateTime.UtcNow;

    private GameMap? _currentMap;

    private IDisposable? _accountLoggingScope;

    private Account? _account;

    private SkillHitValidator? _skillHitValidator;

    private IPetCommandManager? _petCommandManager;

    private Lazy<ComboStateMachine>? _comboStateLazy;

    /// <summary>
    /// Initializes a new instance of the <see cref="Player" /> class.
    /// </summary>di
    /// <param name="gameContext">The game context.</param>
    public Player(IGameContext gameContext)
    {
        this.GameContext = gameContext;
        this.Logger = gameContext.LoggerFactory.CreateLogger<Player>();
        this.PersistenceContext = this.GameContext.PersistenceContextProvider.CreateNewPlayerContext(gameContext.Configuration);
        this._walker = new Walker(this, this.GetStepDelay);

        this.MagicEffectList = new MagicEffectsList(this);
        this._appearanceData = new AppearanceDataAdapter(this);
        this.PlayerState.StateChanged += async args => await (this.GameContext.PlugInManager.GetPlugInPoint<IPlayerStateChangedPlugIn>()?.PlayerStateChangedAsync(this, args.PreviousState, args.CurrentStateState) ?? ValueTask.CompletedTask).ConfigureAwait(false);
        this.PlayerState.StateChanges += async args => await (this.GameContext.PlugInManager.GetPlugInPoint<IPlayerStateChangingPlugIn>()?.PlayerStateChangingAsync(this, args) ?? ValueTask.CompletedTask).ConfigureAwait(false);
        this._observerToWorldViewAdapter = new ObserverToWorldViewAdapter(this, this.InfoRange);
        this._muHelperLazy = new Lazy<MuHelper.MuHelper>(() => new MuHelper.MuHelper(this));
    }

    /// <summary>
    /// Occurs when the player has or got disconnected from the game.
    /// </summary>
    public event AsyncEventHandler<Player>? PlayerDisconnected;

    /// <summary>
    /// Occurs when the player entered the world with his selected character.
    /// </summary>
    public event AsyncEventHandler<Player>? PlayerEnteredWorld;

    /// <summary>
    /// Occurs when the player left the world with his selected character.
    /// </summary>
    public event AsyncEventHandler<Player>? PlayerLeftWorld;

    /// <summary>
    /// Occurs when the player entered the map with his selected character.
    /// </summary>
    public event EventHandler<(Player, GameMap)>? PlayerEnteredMap;

    /// <summary>
    /// Occurs when the player left the map with his selected character.
    /// </summary>
    public event EventHandler<(Player, GameMap)>? PlayerLeftMap;

    /// <summary>
    /// Occurs when the player picked up an item.
    /// </summary>
    public event AsyncEventHandler<(Player, ILocateable)>? PlayerPickedUpItem;

    /// <summary>
    /// Occurs when this instance died.
    /// </summary>
    public event EventHandler<DeathInformation>? Died;

    /// <inheritdoc />
    ILogger ILoggerOwner.Logger => this.Logger;

    /// <inheritdoc />
    public ILogger<Player> Logger { get; }

    /// <inheritdoc />
    public bool IsWalking => this._walker.CurrentTarget != default;

    /// <inheritdoc />
    public TimeSpan StepDelay => this.GetStepDelay();

    /// <inheritdoc />
    public Point WalkTarget => this._walker.CurrentTarget;

    /// <summary>
    /// Gets a value indicating whether this instance is invisible to other players.
    /// </summary>
    public bool IsInvisible => this.Attributes?[Stats.IsInvisible] > 0;

    /// <summary>
    /// Gets the skill hit validator.
    /// </summary>
    public SkillHitValidator SkillHitValidator => this._skillHitValidator ??= new SkillHitValidator(this.Logger);

    /// <inheritdoc/>
    public int Money
    {
        get => this.SelectedCharacter?.Inventory?.Money ?? 0;

        set
        {
            if (this.SelectedCharacter is null)
            {
                return;
            }

            this.SelectedCharacter.ThrowNotInitializedProperty(this.SelectedCharacter.Inventory is null, nameof(this.SelectedCharacter.Inventory));

            if (this.SelectedCharacter != null && this.SelectedCharacter.Inventory.Money != value)
            {
                this.SelectedCharacter.Inventory.Money = value;
                _ = this.InvokeViewPlugInAsync<IUpdateMoneyPlugIn>(p => p.UpdateMoneyAsync());
            }
        }
    }

    /// <summary>
    /// Gets the persistence context.
    /// </summary>
    public IPlayerContext PersistenceContext { get; }

    /// <inheritdoc/>
    public ushort Id { get; set; }

    /// <inheritdoc cref="IPartyMember" />
    public string Name => this.SelectedCharacter?.Name ?? string.Empty;

    /// <inheritdoc/>
    public int Level => (int)(this.Attributes?[Stats.Level] ?? 0);

    /// <summary>
    /// Gets the selected character.
    /// </summary>
    public Character? SelectedCharacter => this._selectedCharacter;

    /// <summary>
    /// Gets or sets the pose of the currently selected character.
    /// </summary>
    public CharacterPose Pose
    {
        get => this._selectedCharacter?.Pose ?? default;

        set
        {
            var character = this._selectedCharacter;
            if (character is null || character.Pose == this.Pose)
            {
                return;
            }

            character.Pose = value;
            this._appearanceData.RaiseAppearanceChanged();
        }
    }

    /// <summary>
    /// Gets or sets the account.
    /// </summary>
    public Account? Account
    {
        get => this._account;
        set
        {
            if (this._account != value)
            {
                this._account = value;
                this._accountLoggingScope?.Dispose();
                this._accountLoggingScope = this.Logger.BeginScope("Account: {Name}", this._account!.LoginName);
                this.IsVaultLocked = !string.IsNullOrWhiteSpace(this._account.VaultPassword);
                this.LogInvalidVaultItems();
            }
        }
    }

    /// <summary>
    /// Gets the magic effect list.
    /// </summary>
    public MagicEffectsList MagicEffectList { get; }

    /// <summary>
    /// Gets or sets the Monster of the current opened Monster dialog.
    /// </summary>
    public NonPlayerCharacter? OpenedNpc { get; set; }

    /// <inheritdoc/>
    public StateMachine PlayerState { get; } = new(GameLogic.PlayerState.Initial);

    // TODO: TradeContext-object?

    /// <inheritdoc/>
    public ITrader? TradingPartner { get; set; }

    /// <inheritdoc/>
    public int TradingMoney { get; set; }

    /// <summary>
    /// Gets or sets the duel room in which the player is currently fighting or spectating.
    /// </summary>
    public DuelRoom? DuelRoom { get; set; }

    /// <inheritdoc/>
    public GameMap? CurrentMap
    {
        get => this._currentMap;

        private set
        {
            if (this._currentMap != value)
            {
                if (this._currentMap is { } oldMap)
                {
                    this.RaisePlayerLeftMap(oldMap);
                }

                this._currentMap = value;
                this.GameContext.PlugInManager?.GetPlugInPoint<IAttackableMovedPlugIn>()?.AttackableMoved(this);

                if (this._currentMap is { } newMap)
                {
                    this.RaisePlayerEnteredMap(newMap);
                }
            }
        }
    }

    /// <inheritdoc/>
    public ISet<IWorldObserver> Observers { get; } = new HashSet<IWorldObserver>();

    /// <inheritdoc/>
    public AsyncReaderWriterLock ObserverLock { get; } = new();

    /// <inheritdoc/>
    public IPartyMember? LastPartyRequester { get; set; }

    /// <summary>
    /// Gets or sets the last guild requester.
    /// </summary>
    public Player? LastGuildRequester { get; set; }

    /// <summary>
    /// Gets or sets the guild war context.
    /// </summary>
    public GuildWarContext? GuildWarContext { get; set; }

    /// <summary>
    /// Gets the skill list.
    /// </summary>
    public ISkillList? SkillList { get; private set; }

    /// <inheritdoc />
    public ComboStateMachine? ComboState => this.Attributes?[Stats.IsSkillComboAvailable] > 0 ? this._comboStateLazy?.Value : null;

    /// <summary>
    /// Gets the summon.
    /// </summary>
    public (Monster, INpcIntelligence)? Summon { get; private set; }

    /// <inheritdoc/>
    public GuildMemberStatus? GuildStatus { get; set; }

    /// <inheritdoc/>
    public Direction Rotation { get; set; }

    /// <inheritdoc/>
    public Party? Party { get; set; }

    /// <inheritdoc/>
    public bool IsAlive { get; set; }

    /// <inheritdoc/>
    public bool IsTeleporting { get; private set; }

    /// <inheritdoc/>
    public DeathInformation? LastDeath { get; private set; }

    /// <inheritdoc/>
    public Point Position
    {
        get => new(this.SelectedCharacter?.PositionX ?? 0, this.SelectedCharacter?.PositionY ?? 0);

        set
        {
            if (this.Position != value && this.SelectedCharacter is { } character)
            {
                character.PositionX = value.X;
                character.PositionY = value.Y;
                this.GameContext.PlugInManager?.GetPlugInPoint<IAttackableMovedPlugIn>()?.AttackableMoved(this);
            }
        }
    }

    /// <summary>
    /// Gets or sets Position with a randomized shim.
    /// </summary>
    public Point RandomPosition
    {
        get => this._currentMap!.Terrain.GetRandomCoordinate(this.Position, 1);

        set => this.Position = this._currentMap!.Terrain.GetRandomCoordinate(value, 1);
    }

    /// <inheritdoc/>
    public uint MaximumHealth => (uint)(this.Attributes?[Stats.MaximumHealth] ?? 0);

    /// <inheritdoc/>
    public uint CurrentHealth => (uint)(this.Attributes?[Stats.CurrentHealth] ?? 0);

    /// <summary>
    /// Gets or sets a value indicating whether this player is online as friend, and shown as online in its friends friendlists.
    /// </summary>
    public bool OnlineAsFriend { get; set; } = true;

    /// <inheritdoc cref="IWorldObserver"/>
    public ICustomPlugInContainer<IViewPlugIn> ViewPlugIns => this._viewPlugIns ??= this.CreateViewPlugInContainer();

    /// <inheritdoc/>
    public IInventoryStorage? Inventory { get; private set; }

    /// <inheritdoc/>
    public IStorage? TemporaryStorage { get; private set; }

    /// <summary>
    /// Gets or sets the vault.
    /// </summary>
    public IStorage? Vault { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the vault of the player is currently locked by a pin.
    /// </summary>
    public bool IsVaultLocked { get; set; }

    /// <summary>
    /// Gets the shop storage.
    /// </summary>
    public IShopStorage? ShopStorage { get; private set; }

    /// <inheritdoc/>
    public BackupItemStorage? BackupInventory { get; set; }

    /// <summary>
    /// Gets the appearance data.
    /// </summary>
    public IAppearanceData AppearanceData => this._appearanceData;

    /// <summary>
    /// Gets the game context.
    /// </summary>
    public IGameContext GameContext { get; }

    /// <inheritdoc/>
    public IList<Bucket<ILocateable>> ObservingBuckets => this._observerToWorldViewAdapter.ObservingBuckets;

    /// <inheritdoc/>
    public int InfoRange => this.GameContext.Configuration.InfoRange;

    /// <inheritdoc/>
    IAttributeSystem IAttackable.Attributes => this.Attributes!;

    /// <inheritdoc/>
    IAttributeSystem IAttacker.Attributes => this.Attributes!;

    /// <summary>
    /// Gets the attribute system.
    /// </summary>
    public ItemAwareAttributeSystem? Attributes { get; private set; }

    /// <inheritdoc/>
    public Bucket<ILocateable>? NewBucket { get; set; }

    /// <inheritdoc/>
    public Bucket<ILocateable>? OldBucket { get; set; }

    /// <summary>
    /// Gets or sets the mini game, which the player has currently entered.
    /// </summary>
    public MiniGameContext? CurrentMiniGame { get; set; }

    /// <summary>
    /// Gets the pet command manager.
    /// </summary>
    public IPetCommandManager? PetCommandManager
    {
        get
        {
            if (this._petCommandManager is null
                && this.Inventory?.GetItem(InventoryConstants.RightHandSlot) is { } pet && pet.IsTrainablePet())
            {
                // Since the Raven is currently the only pet which can attack, we directly use it.
                // However, in the future we might use a factory as strategy plugin here which creates the command manager
                // depending on the actual pet.
                this._petCommandManager = new RavenCommandManager(this, pet);
            }

            return this._petCommandManager;
        }
    }

    /// <summary>
    /// Gets the last attacked target.
    /// </summary>
    public WeakReference<IAttackable?> LastAttackedTarget { get; } = new(null);

    /// <summary>
    /// Gets or sets the last requested player store.
    /// </summary>
    public WeakReference<Player>? LastRequestedPlayerStore { get; set; }

    /// <summary>
    /// Gets or sets the cancellation token source for the nova skill.
    /// </summary>
    public NovaCancellationTokenSource? NovaCancellationTokenSource { get; set; }

    /// <summary>
    /// Gets the mu helper.
    /// </summary>
    public MuHelper.MuHelper MuHelper => this._muHelperLazy.Value;

    /// <summary>
    /// Gets or sets the cooldown timestamp until no further potion can be consumed.
    /// </summary>
    public DateTime PotionCooldownUntil { get; set; } = DateTime.UtcNow;

    private static readonly MagicEffectDefinition GMEffect = new GMMagicEffectDefinition
    {
        InformObservers = true,
        Name = "GM MARK",
        Number = 28,
        StopByDeath = false,
    };

    /// <summary>
    /// Sets the selected character.
    /// </summary>
    /// <param name="character">The character.</param>
    public async ValueTask SetSelectedCharacterAsync(Character? character)
    {
        if (this._selectedCharacter == character)
        {
            return;
        }

        if (character is null)
        {
            if (this._muHelperLazy.IsValueCreated)
            {
                await this._muHelperLazy.Value.StopAsync().ConfigureAwait(false);
            }

            this.RemovePetCommandManager();
            this.LastAttackedTarget.SetTarget(null);
            this._comboStateLazy = null;

            this._appearanceData.RaiseAppearanceChanged();

            await this.PlayerLeftWorld.SafeInvokeAsync(this).ConfigureAwait(false);
            this._selectedCharacter = null;
            (this.SkillList as IDisposable)?.Dispose();
            this.SkillList = null;

            if (this.DuelRoom is { State: DuelState.DuelStarted } duelRoom)
            {
                await duelRoom.CancelDuelAsync().ConfigureAwait(false);
                if (this.GameContext.Configuration.DuelConfiguration?.Exit is { } exit)
                {
                    this.PlaceAtGate(exit);
                }
            }

            this.DuelRoom = null;
        }
        else
        {
            this._selectedCharacter = character;
            await this.OnPlayerEnteredWorldAsync().ConfigureAwait(false);
            await this.PlayerEnteredWorld.SafeInvokeAsync(this).ConfigureAwait(false);

            this._appearanceData.RaiseAppearanceChanged();
        }
    }

    /// <summary>
    /// Will be called when an item has been picked up by player.
    /// </summary>
    /// <param name="item">The item, which the player has picked up.</param>
    public async ValueTask OnPickedUpItemAsync(ILocateable item)
    {
        if (this.PlayerPickedUpItem is { } eventHandler)
        {
            await eventHandler((this, item)).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask KillInstantlyAsync()
    {
        if (this.Attributes is null)
        {
            throw new InvalidOperationException("AttributeSystem not set.");
        }

        var hitInfo = new HitInfo((uint)this.Attributes[Stats.CurrentHealth], (uint)this.Attributes[Stats.CurrentShield], DamageAttributes.Undefined);
        this.Attributes[Stats.CurrentHealth] = 0;

        this.LastDeath = new DeathInformation(0, string.Empty, hitInfo, 0);
        await this.OnDeathAsync(null).ConfigureAwait(false);
    }

    /// <summary>
    /// Determines whether the self defense is active for the specified attacker.
    /// </summary>
    /// <param name="attacker">The attacker.</param>
    /// <returns>
    ///   <c>true</c> if the self defense is active for the specified attacker; otherwise, <c>false</c>.
    /// </returns>
    public bool IsSelfDefenseActive(Player attacker)
    {
        if (this.GameContext.SelfDefenseState.TryGetValue((attacker, this), out var timeout))
        {
            return timeout > DateTime.UtcNow;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the self defense is active for any attacker.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if any self defense is active; otherwise, <c>false</c>.
    /// </returns>
    public bool IsAnySelfDefenseActive()
    {
        var selfDefenses = this.GameContext.SelfDefenseState.Keys.Where(c => c.Attacker == this).ToList();
        return selfDefenses.Any(sd =>
            this.GameContext.SelfDefenseState.TryGetValue(sd, out var timeout)
            && timeout >= DateTime.UtcNow);
    }

    /// <inheritdoc/>
    public async ValueTask<HitInfo?> AttackByAsync(IAttacker attacker, SkillEntry? skill, bool isCombo, double damageFactor = 1.0)
    {
        if (this.Attributes is null)
        {
            throw new InvalidOperationException("AttributeSystem not set.");
        }

        var hitInfo = await attacker.CalculateDamageAsync(this, skill, isCombo, damageFactor).ConfigureAwait(false);

        if (hitInfo.HealthDamage == 0)
        {
            await this.InvokeViewPlugInAsync<IShowHitPlugIn>(p => p.ShowHitAsync(this, hitInfo)).ConfigureAwait(false);
            if (attacker is IWorldObserver observer)
            {
                await observer.InvokeViewPlugInAsync<IShowHitPlugIn>(p => p.ShowHitAsync(this, hitInfo)).ConfigureAwait(false);
            }

            return hitInfo;
        }

        attacker.ApplyAmmunitionConsumption(hitInfo);

        if (Rand.NextRandomBool(this.Attributes[Stats.FullyRecoverHealthAfterHitChance]))
        {
            this.Attributes[Stats.CurrentHealth] = this.Attributes[Stats.MaximumHealth];
            await this.InvokeViewPlugInAsync<IUpdateCurrentHealthPlugIn>(p => p.UpdateCurrentHealthAsync()).ConfigureAwait(false);
        }

        if (Rand.NextRandomBool(this.Attributes[Stats.FullyRecoverManaAfterHitChance]))
        {
            this.Attributes[Stats.CurrentMana] = this.Attributes[Stats.MaximumMana];
            await this.InvokeViewPlugInAsync<IUpdateCurrentManaPlugIn>(p => p.UpdateCurrentManaAsync()).ConfigureAwait(false);
        }

        await this.HitAsync(hitInfo, attacker, skill?.Skill).ConfigureAwait(false);
        await this.DecreaseItemDurabilityAfterHitAsync(hitInfo).ConfigureAwait(false);

        if (attacker as IPlayerSurrogate is { } playerSurrogate)
        {
            await playerSurrogate.Owner.AfterHitTargetAsync().ConfigureAwait(false);
        }

        if (attacker is Player attackerPlayer)
        {
            await attackerPlayer.AfterHitTargetAsync().ConfigureAwait(false);
        }

        return hitInfo;
    }

    /// <summary>
    /// Is called after the player successfully hit a target.
    /// </summary>
    public async ValueTask AfterHitTargetAsync()
    {
        this.Attributes![Stats.CurrentHealth] = Math.Max(this.Attributes[Stats.CurrentHealth] - this.Attributes[Stats.HealthLossAfterHit], 1);
        this.Attributes![Stats.CurrentMana] = Math.Max(this.Attributes[Stats.CurrentMana] - this.Attributes[Stats.ManaLossAfterHit], 0);
        await this.DecreaseWeaponDurabilityAfterHitAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public ValueTask ReflectDamageAsync(IAttacker reflector, uint damage)
    {
        return this.HitAsync(this.GetHitInfo(damage, DamageAttributes.Reflected, reflector), reflector, null);
    }

    /// <inheritdoc/>
    public ValueTask ApplyPoisonDamageAsync(IAttacker initialAttacker, uint damage)
    {
        return this.HitAsync(new HitInfo(damage, 0, DamageAttributes.Poison), initialAttacker, null);
    }

    /// <summary>
    /// Teleports this player to the specified target with the specified skill animation.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="teleportSkill">The teleport skill.</param>
    public async Task TeleportAsync(Point target, Skill teleportSkill)
    {
        if (!this.IsAlive)
        {
            return;
        }

        this.IsTeleporting = true;
        try
        {
            await (this.NovaCancellationTokenSource?.CancelAsync() ?? Task.CompletedTask).ConfigureAwait(false);

            await this._walker.StopAsync().ConfigureAwait(false);

            var previous = this.Position;
            this.Position = target;

            await this.ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(p => p.ShowSkillAnimationAsync(this, this, teleportSkill, true), true).ConfigureAwait(false);

            await Task.Delay(300).ConfigureAwait(false);

            await this.ForEachWorldObserverAsync<IObjectsOutOfScopePlugIn>(p => p.ObjectsOutOfScopeAsync(this.GetAsEnumerable()), false).ConfigureAwait(false);

            await Task.Delay(1500).ConfigureAwait(false);

            if (this.IsAlive)
            {
                await this.InvokeViewPlugInAsync<ITeleportPlugIn>(p => p.ShowTeleportedAsync()).ConfigureAwait(false);

                // We need to restore the previous position to make the Moving on the map data structure work correctly.
                this.Position = previous;
                if (this.CurrentMap is { } map)
                {
                    await map.MoveAsync(this, target, this._moveLock, MoveType.Teleport).ConfigureAwait(false);
                }
            }
        }
        catch (Exception e)
        {
            this.Logger.LogWarning(e, "Error during teleport");
        }

        this.IsTeleporting = false;
    }

    /// <summary>
    /// Is called after the player killed a <see cref="Monster"/>.
    /// Adds recovered mana and health to the players attributes.
    /// </summary>
    public async ValueTask AfterKilledMonsterAsync()
    {
        foreach (var recoverAfterMonsterKill in Stats.AfterMonsterKillRegenerationAttributes)
        {
            var additionalValue = (uint)((this.Attributes![recoverAfterMonsterKill.RegenerationMultiplier] * this.Attributes[recoverAfterMonsterKill.MaximumAttribute]) + this.Attributes[recoverAfterMonsterKill.AbsoluteAttribute]);
            this.Attributes[recoverAfterMonsterKill.CurrentAttribute] = (uint)Math.Min(this.Attributes[recoverAfterMonsterKill.MaximumAttribute], this.Attributes[recoverAfterMonsterKill.CurrentAttribute] + additionalValue);
        }

        await this.InvokeViewPlugInAsync<IUpdateCurrentManaPlugIn>(p => p.UpdateCurrentManaAsync()).ConfigureAwait(false);
        await this.InvokeViewPlugInAsync<IUpdateCurrentHealthPlugIn>(p => p.UpdateCurrentHealthAsync()).ConfigureAwait(false);
    }

    /// <summary>
    /// Resets the appearance cache.
    /// </summary>
    public void OnAppearanceChanged() => this._appearanceData.RaiseAppearanceChanged();

    /// <summary>
    /// Determines whether the player complies with the requirements of the specified item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns><c>True</c>, if the player complies with the requirements of the specified item; Otherwise, <c>false</c>.</returns>
    public bool CompliesRequirements(Item item)
    {
        item.ThrowNotInitializedProperty(item.Definition is null, nameof(item.Definition));

        foreach (var requirement in item.Definition.Requirements.Select(item.GetRequirement))
        {
            if (this.Attributes![requirement.Item1] < requirement.Item2)
            {
                return false;
            }
        }

        return item.Definition.QualifiedCharacters.Contains(this.SelectedCharacter!.CharacterClass!);
    }

    /// <summary>
    /// Tries to remove the money from the players inventory.
    /// </summary>
    /// <param name="value">The value which should be removed.</param>
    /// <returns><c>True</c>, if the players inventory had enough money to remove; Otherwise, <c>false</c>.</returns>
    public bool TryRemoveMoney(int value)
    {
        if (this.Money < value)
        {
            return false;
        }

        this.Money = checked(this.Money - value);
        return true;
    }

    /// <summary>
    /// Tries to deposit the money from the players inventory.
    /// </summary>
    /// <param name="value">The value which should be moved the the vault.</param>
    /// <returns><c>True</c>, if the players inventory had enough money to move; Otherwise, <c>false</c>.</returns>
    public bool TryDepositVaultMoney(int value)
    {
        if (this.Vault is null)
        {
            return false;
        }

        if (this.Vault.ItemStorage.Money + value > this.GameContext?.Configuration?.MaximumVaultMoney)
        {
            return false;
        }

        if (this.TryRemoveMoney(value))
        {
            return this.Vault.TryAddMoney(value);
        }

        return false;
    }

    /// <summary>
    /// Tries to take the money from the the vault.
    /// </summary>
    /// <param name="value">The value which should be retrieve the the vault.</param>
    /// <returns><c>True</c>, if the vault had enough money to move and player inventory isn't maximum; Otherwise, <c>false</c>.</returns>
    public bool TryTakeVaultMoney(int value)
    {
        if (this.Vault is null)
        {
            return false;
        }

        if (this.Money + value > this.GameContext?.Configuration?.MaximumInventoryMoney)
        {
            return false;
        }

        if (this.Vault.TryRemoveMoney(value))
        {
            return this.TryAddMoney(value);
        }

        return false;
    }

    /// <summary>
    /// Tries to add the money from the players inventory.
    /// </summary>
    /// <param name="value">The value which should be added.</param>
    /// <returns><c>True</c>, if the players inventory had space to add money; Otherwise, <c>false</c>.</returns>
    public virtual bool TryAddMoney(int value)
    {
        if (this.Money + value > this.GameContext?.Configuration?.MaximumInventoryMoney)
        {
            return false;
        }

        if (this.Money + value < 0)
        {
            return false;
        }

        this.Money = checked(this.Money + value);
        return true;
    }

    /// <summary>
    /// Adds the invisible effect.
    /// </summary>
    public async ValueTask AddInvisibleEffectAsync()
    {
        var invisibleEffect = this.GameContext.Configuration.MagicEffects.FirstOrDefault(e => e.PowerUpDefinitions.Any(e => e.TargetAttribute == Stats.IsInvisible));
        if (invisibleEffect is null)
        {
            this.Logger.LogError("Invisible effect not found!");
        }
        else
        {
            var (duration, powerUps) = this.CreateMagicEffectPowerUp(invisibleEffect);
            var magicEffect = new MagicEffect(TimeSpan.FromSeconds(duration.Value), invisibleEffect, powerUps.Select(p => new MagicEffect.ElementWithTarget(p.BuffPowerUp, p.Target)).ToArray());
            await this.MagicEffectList.AddEffectAsync(magicEffect).ConfigureAwait(false);

            if (this._currentMap is { } currentMap)
            {
                await currentMap.RespawnAsync(this).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Removes the invisible effect.
    /// </summary>
    public async ValueTask RemoveInvisibleEffectAsync()
    {
        var invisibleEffect = this.GameContext.Configuration.MagicEffects.FirstOrDefault(e => e.PowerUpDefinitions.Any(e => e.TargetAttribute == Stats.IsInvisible));
        if (invisibleEffect is null)
        {
            return;
        }

        var activeEffect = this.MagicEffectList.ActiveEffects.Values.FirstOrDefault(e => e.Definition == invisibleEffect);
        if (activeEffect is null)
        {
            return;
        }

        await activeEffect.DisposeAsync().ConfigureAwait(false);
        if (this._currentMap is { } currentMap)
        {
            await currentMap.RespawnAsync(this).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Moves the player to the specified gate.
    /// </summary>
    /// <param name="gate">The gate to which the player should be moved.</param>
    public async ValueTask WarpToAsync(ExitGate gate)
    {
        if (!await this.TryRemoveFromCurrentMapAsync().ConfigureAwait(false))
        {
            return;
        }

        this.PlaceAtGate(gate);
        this.CurrentMap = null; // Will be set again, when the client acknowledged the map change by F3 12 packet.

        if (!this.PlayerState.CurrentState.IsDisconnectedOrFinished())
        {
            await this.InvokeViewPlugInAsync<IMapChangePlugIn>(p => p.MapChangeAsync()).ConfigureAwait(false);
        }

        // after this, the Client will send us a F3 12 packet, to tell us it loaded
        // the map and is ready to receive the new meet player/monster etc.
        // Then ClientReadyAfterMapChange is called.
    }

    /// <summary>
    /// Moves the player to the safe zone.
    /// </summary>
    public async ValueTask WarpToSafezoneAsync() => await this.WarpToAsync(await this.GetSpawnGateOfCurrentMapAsync().ConfigureAwait(false)).ConfigureAwait(false);

    /// <summary>
    /// Respawns the player to the specified gate.
    /// </summary>
    /// <param name="gate">The gate at which the player should be respawned.</param>
    public async ValueTask RespawnAtAsync(ExitGate gate)
    {
        if (!await this.TryRemoveFromCurrentMapAsync().ConfigureAwait(false))
        {
            return;
        }

        this.ThrowNotInitializedProperty(this.SelectedCharacter is null, nameof(this.SelectedCharacter));
        this.SelectedCharacter.ThrowNotInitializedProperty(this.SelectedCharacter.CurrentMap is null, nameof(this.SelectedCharacter.CurrentMap));
        this.PlaceAtGate(gate);
        this._respawnAfterDeathCts?.Dispose();
        this._respawnAfterDeathCts = null;

        if (this.ViewPlugIns.GetPlugIn<IRespawnAfterDeathPlugIn>() is { } respawnPlugIn)
        {
            // Older clients use separate packet for the respawn, while newer don't.
            // It requires a slightly different logic.
            this.CurrentMap = await this.GameContext.GetMapAsync(this.SelectedCharacter!.CurrentMap!.Number.ToUnsigned()).ConfigureAwait(false) ?? throw new InvalidOperationException("Current map not found.");
            await respawnPlugIn.RespawnAsync().ConfigureAwait(false);
            await this.PlayerState.TryAdvanceToAsync(GameLogic.PlayerState.EnteredWorld).ConfigureAwait(false);
            this.IsAlive = true;
            await this.CurrentMap!.AddAsync(this).ConfigureAwait(false);
        }
        else
        {
            this.CurrentMap = null; // Will be set again, when the client acknowledged the map change by F3 12 packet.
            await this.InvokeViewPlugInAsync<IMapChangePlugIn>(p => p.MapChangeAsync()).ConfigureAwait(false);

            // after this, the Client will send us a F3 12 packet, to tell us it loaded
            // the map and is ready to receive the new meet player/monster etc.
            // Then ClientReadyAfterMapChange is called.
        }
    }

    /// <summary>
    /// Signals that the game client of the player is ready after a map change (data has been loaded etc).
    /// On this event, the player enters the game map on the server side, and interacts with the other objects.
    /// </summary>
    /// <remarks>
    /// This method is called after the client sent us the F3 12 packet, of after
    /// the player entered the game.
    /// </remarks>
    public async ValueTask ClientReadyAfterMapChangeAsync()
    {
        this.ThrowNotInitializedProperty(this.SelectedCharacter is null, nameof(this.SelectedCharacter));
        this.SelectedCharacter.ThrowNotInitializedProperty(this.SelectedCharacter.CurrentMap is null, nameof(this.SelectedCharacter.CurrentMap));

        if (this.CurrentMiniGame is { } currentMiniGame)
        {
            this.CurrentMap = currentMiniGame.Map;
        }
        else
        {
            this.CurrentMap = await this.GameContext.GetMapAsync(this.SelectedCharacter!.CurrentMap.Number.ToUnsigned()).ConfigureAwait(false);
        }

        await this.PlayerState.TryAdvanceToAsync(GameLogic.PlayerState.EnteredWorld).ConfigureAwait(false);
        this.IsAlive = true;

        await this.CurrentMap!.AddAsync(this).ConfigureAwait(false);
        if (!this.CurrentMap.Terrain.WalkMap[this.SelectedCharacter.PositionX, this.SelectedCharacter.PositionY])
        {
            await this.WarpToSafezoneAsync().ConfigureAwait(false);
        }

        if (this.Summon?.Item1 is { IsAlive: true } summon)
        {
            await this.CurrentMap.AddAsync(summon).ConfigureAwait(false);
            summon.OnSpawn();
        }
    }

    /// <summary>
    /// Adds experience points after killing the target object.
    /// </summary>
    /// <param name="killedObject">The killed object.</param>
    /// <returns>The gained experience.</returns>
    public async ValueTask<int> AddExpAfterKillAsync(IAttackable killedObject)
    {
        if (this.SelectedCharacter?.CharacterClass is not { } characterClass)
        {
            return 0;
        }

        var addMasterExperience = characterClass.IsMasterClass
                            && (short)this.Attributes![Stats.Level] == this.GameContext.Configuration.MaximumLevel;
        var expRateAttribute = addMasterExperience ? Stats.MasterExperienceRate : Stats.ExperienceRate;

        var totalLevel = this.Attributes![Stats.Level] + this.Attributes![Stats.MasterLevel];
        var experience = killedObject.CalculateBaseExperience(totalLevel);
        experience *= this.GameContext.ExperienceRate;
        experience *= this.Attributes[expRateAttribute];
        experience *= this.CurrentMap?.Definition.ExpMultiplier ?? 1;
        experience = Rand.NextInt((int)(experience * 0.8), (int)(experience * 1.2));

        if (addMasterExperience)
        {
            await this.AddMasterExperienceAsync((int)experience, killedObject).ConfigureAwait(false);
        }
        else
        {
            await this.AddExperienceAsync((int)experience, killedObject).ConfigureAwait(false);
        }

        await this.AddPetExperienceAsync(experience).ConfigureAwait(false);

        return (int)experience;
    }

    /// <summary>
    /// Adds the master experience to the current character.
    /// </summary>
    /// <param name="experience">The experience which should be added.</param>
    /// <param name="killedObject">The killed object which caused the experience gain.</param>
    public async ValueTask AddMasterExperienceAsync(int experience, IAttackable? killedObject)
    {
        if (this.Attributes![Stats.MasterLevel] >= this.GameContext.Configuration.MaximumMasterLevel)
        {
            await this.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("You already reached maximum master Level.", MessageType.BlueNormal)).ConfigureAwait(false);
            return;
        }

        if (killedObject is not null && killedObject.Attributes[Stats.Level] < this.GameContext.Configuration.MinimumMonsterLevelForMasterExperience)
        {
            await this.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("You need to kill stronger monsters to gain master experience.", MessageType.BlueNormal)).ConfigureAwait(false);
            return;
        }

        long exp = experience;

        // Add the Exp
        bool lvlup = false;
        var expTable = this.GameContext.MasterExperienceTable;
        if (expTable[(int)this.Attributes[Stats.MasterLevel] + 1] - this.SelectedCharacter!.MasterExperience < exp)
        {
            exp = expTable[(int)this.Attributes[Stats.MasterLevel] + 1] - this.SelectedCharacter.MasterExperience;
            lvlup = true;
        }

        this.SelectedCharacter.MasterExperience += exp;

        // Tell it to the Player
        await this.InvokeViewPlugInAsync<IAddExperiencePlugIn>(p => p.AddExperienceAsync((int)exp, killedObject)).ConfigureAwait(false);

        // Check the lvl up
        if (lvlup)
        {
            this.Attributes[Stats.MasterLevel]++;
            this.SelectedCharacter.MasterLevelUpPoints += (int)this.Attributes[Stats.MasterPointsPerLevelUp];
            this.SetReclaimableAttributesToMaximum();
            this.Logger.LogDebug("Character {0} leveled up to master level {1}", this.SelectedCharacter.Name, this.Attributes[Stats.MasterLevel]);
            await this.InvokeViewPlugInAsync<IUpdateLevelPlugIn>(p => p.UpdateMasterLevelAsync()).ConfigureAwait(false);
            await this.ForEachWorldObserverAsync<IShowEffectPlugIn>(p => p.ShowEffectAsync(this, IShowEffectPlugIn.EffectType.LevelUp), true).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Adds the experience to the current character.
    /// </summary>
    /// <param name="experience">The experience which should be added.</param>
    /// <param name="killedObject">The killed object which caused the experience gain.</param>
    public async ValueTask AddExperienceAsync(int experience, IAttackable? killedObject)
    {
        if (this.Attributes![Stats.Level] >= this.GameContext.Configuration.MaximumLevel)
        {
            await this.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("You already reached maximum Level.", MessageType.BlueNormal)).ConfigureAwait(false);
            return;
        }

        long exp = experience;
        bool isLevelUp = false;
        var expTable = this.GameContext.ExperienceTable;
        var expForNextLevel = expTable[(int)this.Attributes[Stats.Level] + 1];
        if (expForNextLevel - this.SelectedCharacter!.Experience < exp)
        {
            exp = expForNextLevel - this.SelectedCharacter.Experience;
            isLevelUp = true;
        }

        this.SelectedCharacter.Experience += exp;

        // Tell it to the Player
        await this.InvokeViewPlugInAsync<IAddExperiencePlugIn>(p => p.AddExperienceAsync((int)exp, killedObject)).ConfigureAwait(false);

        // Check the lvl up
        if (isLevelUp)
        {
            this.Attributes[Stats.Level]++;
            this.SelectedCharacter.LevelUpPoints += (int)this.Attributes[Stats.PointsPerLevelUp];
            this.SetReclaimableAttributesToMaximum();
            this.Logger.LogDebug("Character {0} leveled up to {1}", this.SelectedCharacter.Name, this.Attributes[Stats.Level]);

            this.GameContext.PlugInManager.GetPlugInPoint<ICharacterLevelUpPlugIn>()?.CharacterLeveledUp(this);

            await this.InvokeViewPlugInAsync<IUpdateLevelPlugIn>(p => p.UpdateLevelAsync()).ConfigureAwait(false);
            await this.ForEachWorldObserverAsync<IShowEffectPlugIn>(p => p.ShowEffectAsync(this, IShowEffectPlugIn.EffectType.LevelUp), true).ConfigureAwait(false);

            var remainingExp = experience - exp;
            if (remainingExp > 0 && this.Attributes![Stats.Level] < this.GameContext.Configuration.MaximumLevel)
            {
                await this.AddExperienceAsync((int)remainingExp, killedObject).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Moves the player to the specified coordinate.
    /// </summary>
    /// <param name="target">The target.</param>
    public async ValueTask MoveAsync(Point target)
    {
        this.Logger.LogDebug("MoveAsync: Player is moving to {0}", target);
        await this._walker.StopAsync().ConfigureAwait(false);
        await this.CurrentMap!.MoveAsync(this, target, this._moveLock, MoveType.Instant).ConfigureAwait(false);
        this.Logger.LogDebug("MoveAsync: Observer Count: {0}", this.Observers.Count);
    }

    /// <summary>
    /// Walks to the specified target coordinates using the specified steps.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="steps">The steps.</param>
    public async ValueTask WalkToAsync(Point target, Memory<WalkingStep> steps)
    {
        var currentMap = this.CurrentMap;
        if (currentMap == null)
        {
            return;
        }

        if (this.Attributes is not { } attributes)
        {
            return;
        }

        if (attributes[Stats.IsFrozen] > 0 || attributes[Stats.IsStunned] > 0 || attributes[Stats.IsAsleep] > 0)
        {
            return;
        }

        await this._walker.StopAsync().ConfigureAwait(false);
        if (currentMap.Terrain.WalkMap[target.X, target.Y])
        {
            this.Logger.LogDebug("WalkToAsync: Player is walking to {0}", target);
            await this._walker.WalkToAsync(target, steps).ConfigureAwait(false);
            await currentMap.MoveAsync(this, target, this._moveLock, MoveType.Walk).ConfigureAwait(false);
            this.Logger.LogDebug("WalkToAsync: Observer Count: {0}", this.Observers.Count);
        }
        else
        {
            this.Logger.LogWarning("WalkToAsync: Player requested to walk to {0}, but it's not an allowed target", target);

            // We'll send the current coordinates back to the client, so it doesn't appear in the invalid coordinates.
            await this.InvokeViewPlugInAsync<IObjectMovedPlugIn>(p => p.ObjectMovedAsync(this, MoveType.Instant)).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public ValueTask<int> GetDirectionsAsync(Memory<Direction> directions) => this._walker.GetDirectionsAsync(directions);

    /// <inheritdoc />
    public ValueTask<int> GetStepsAsync(Memory<WalkingStep> steps) => this._walker.GetStepsAsync(steps);

    /// <inheritdoc />
    public ValueTask StopWalkingAsync() => this._walker.StopAsync();

    /// <summary>
    /// Regenerates the attributes specified in <see cref="Stats.IntervalRegenerationAttributes"/>.
    /// </summary>
    public async Task RegenerateAsync()
    {
        try
        {
            var attributes = this.Attributes;
            if (attributes is null)
            {
                return;
            }

            foreach (var r in Stats.IntervalRegenerationAttributes.Where(r =>
                         attributes[r.RegenerationMultiplier] > 0 || attributes[r.AbsoluteAttribute] > 0))
            {
                if (r.CurrentAttribute == Stats.CurrentShield && !this.IsAtSafezone() &&
                    attributes[Stats.ShieldRecoveryEverywhere] < 1)
                {
                    // Shield recovery is only possible at safe-zone, except the character has an specific attribute which has the effect that it's recovered everywhere.
                    // This attribute is usually provided by a level 380 armor and a Guardian Option.
                    continue;
                }

                attributes[r.CurrentAttribute] = Math.Min(
                    attributes[r.CurrentAttribute] +
                    ((attributes[r.MaximumAttribute] * attributes[r.RegenerationMultiplier]) +
                     attributes[r.AbsoluteAttribute]),
                    attributes[r.MaximumAttribute]);
            }

            await this.InvokeViewPlugInAsync<IUpdateCurrentHealthPlugIn>(p => p.UpdateCurrentHealthAsync()).ConfigureAwait(false);
            await this.InvokeViewPlugInAsync<IUpdateCurrentManaPlugIn>(p => p.UpdateCurrentManaAsync()).ConfigureAwait(false);

            await this.RegenerateHeroStateAsync().ConfigureAwait(false);
        }
        catch (InvalidOperationException)
        {
            // may happen after a character disconnected in the mean time.
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Unexpected error when regenerating.");
        }
        finally
        {
            this._lastRegenerate = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Disconnects the player from the game. Remote connections will be closed and data will be saved.
    /// </summary>
    public async ValueTask DisconnectAsync()
    {
        if (await this.PlayerState.TryAdvanceToAsync(GameLogic.PlayerState.Disconnected).ConfigureAwait(false))
        {
            try
            {
                await this.InternalDisconnectAsync().ConfigureAwait(false);
                if (this.PlayerDisconnected is { } disconnectedEventHandler)
                {
                    this.PlayerDisconnected = null;
                    await disconnectedEventHandler(this).ConfigureAwait(false);
                }
            }
            finally
            {
                await this.PlayerState.TryAdvanceToAsync(GameLogic.PlayerState.Finished).ConfigureAwait(false);
            }
        }
    }

    /// <inheritdoc/>
    public async ValueTask AddObserverAsync(IWorldObserver observer)
    {
        if (this.IsInvisible && observer != this)
        {
            return;
        }

        using var _ = await this.ObserverLock.WriterLockAsync();
        this.Observers.Add(observer);
    }

    /// <inheritdoc/>
    public async ValueTask RemoveObserverAsync(IWorldObserver observer)
    {
        using var _ = await this.ObserverLock.WriterLockAsync();
        this.Observers.Remove(observer);
    }

    /// <inheritdoc/>
    public ValueTask LocateableAddedAsync(ILocateable item)
    {
        return this._observerToWorldViewAdapter.LocateableAddedAsync(item);
    }

    /// <inheritdoc/>
    public ValueTask LocateableRemovedAsync(ILocateable item)
    {
        return this._observerToWorldViewAdapter.LocateableRemovedAsync(item);
    }

    /// <inheritdoc/>
    public ValueTask LocateablesOutOfScopeAsync(IEnumerable<ILocateable> oldObjects)
    {
        return this._observerToWorldViewAdapter.LocateablesOutOfScopeAsync(oldObjects);
    }

    /// <inheritdoc/>
    public ValueTask NewLocateablesInScopeAsync(IEnumerable<ILocateable> newObjects)
    {
        return this._observerToWorldViewAdapter.NewLocateablesInScopeAsync(newObjects);
    }

    /// <summary>
    /// Tries to consume the <see cref="Skill.ConsumeRequirements"/> of a skill.
    /// </summary>
    /// <param name="skill">The skill which should get performed.</param>
    /// <returns>
    ///     <c>True</c>, if the <see cref="Skill.ConsumeRequirements"/> and <see cref="Skill.Requirements"/>
    ///     are being met, and the <see cref="Skill.ConsumeRequirements"/> have been consumed; Otherwise, <c>false</c>.
    /// </returns>
    public async ValueTask<bool> TryConsumeForSkillAsync(Skill skill)
    {
        if (skill.Requirements.Any(r => r.MinimumValue > this.Attributes![r.Attribute]))
        {
            return false;
        }

        if (skill.ConsumeRequirements.Any(r => this.GetRequiredValue(r) > this.Attributes![r.Attribute]))
        {
            return false;
        }

        foreach (var requirement in skill.ConsumeRequirements)
        {
            this.Attributes![requirement.Attribute] -= this.GetRequiredValue(requirement);
        }

        await this.InvokeViewPlugInAsync<IUpdateCurrentManaPlugIn>(p => p.UpdateCurrentManaAsync()).ConfigureAwait(false);
        return true;
    }

    /// <summary>
    /// Creates the magic effect power up for the given skill entry.
    /// </summary>
    /// <param name="skillEntry">The skill entry.</param>
    public void CreateMagicEffectPowerUp(SkillEntry skillEntry)
    {
        skillEntry.ThrowNotInitializedProperty(skillEntry.Skill is null, nameof(skillEntry.Skill));

        var skill = skillEntry.Skill;

        if (skill.MagicEffectDef?.PowerUpDefinitions.Any(d => d.Boost is null) ?? true)
        {
            throw new InvalidOperationException($"Skill {skill.Name} ({skill.Number}) has no magic effect definition or is without a PowerUpDefinition.");
        }

        if (skill.MagicEffectDef.Duration is null)
        {
            throw new InvalidOperationException($"Skill {skill.Name} ({skill.Number}) has no duration in MagicEffectDef.");
        }

        int i = 0;
        var result = new (AttributeDefinition Target, IElement BuffPowerUp)[skill.MagicEffectDef.PowerUpDefinitions.Count];
        AddSkillPowersToResult(skill);
        skillEntry.PowerUpDuration = this.Attributes!.CreateDurationElement(skill.MagicEffectDef.Duration);
        skillEntry.PowerUps = result;

        void AddSkillPowersToResult(Skill skill)
        {
            foreach (var powerUpDef in skill.MagicEffectDef!.PowerUpDefinitions)
            {
                IElement powerUp;
                if (skillEntry.Level > 0)
                {
                    powerUp = this.Attributes!.CreateElement(powerUpDef);

                    foreach (var masterSkillDefinition in GetMasterSkillDefinitions(skill.MasterDefinition))
                    {
                        // Apply either for all, or just for the specified TargetAttribute of the master skill
                        powerUp = AppedMasterSkillPowerUp(masterSkillDefinition, powerUpDef, powerUp);
                    }
                }
                else
                {
                    powerUp = this.Attributes!.CreateElement(powerUpDef);
                }

                result[i] = (powerUpDef.TargetAttribute!, powerUp);
                i++;
            }
        }

        IEnumerable<MasterSkillDefinition> GetMasterSkillDefinitions(MasterSkillDefinition? masterSkillDefinition)
        {
            var current = masterSkillDefinition;
            while (current is not null)
            {
                yield return current;
                current = current.ReplacedSkill?.MasterDefinition;
            }
        }

        IElement AppedMasterSkillPowerUp(MasterSkillDefinition? masterSkillDefinition, PowerUpDefinition powerUpDef, IElement powerUp)
        {
            if (masterSkillDefinition is null)
            {
                return powerUp;
            }

            if (masterSkillDefinition?.TargetAttribute is not { } masterSkillTargetAttribute
                || masterSkillTargetAttribute == powerUpDef.TargetAttribute)
            {
                var additionalValue = new SimpleElement(skillEntry.CalculateValue(), skillEntry.Skill.MasterDefinition?.Aggregation ?? powerUp.AggregateType);
                powerUp = new CombinedElement(powerUp, additionalValue);
            }

            return powerUp;
        }
    }

    /// <summary>
    /// Creates the magic effect power up for the given definition.
    /// </summary>
    /// <param name="magicEffectDefinition">The definition for a magic effect.</param>
    /// <returns></returns>
    public (IElement DurationInSeconds, (AttributeDefinition Target, IElement BuffPowerUp)[] PowerUps) CreateMagicEffectPowerUp(MagicEffectDefinition magicEffectDefinition)
    {
        ArgumentNullException.ThrowIfNull(magicEffectDefinition);

        if (magicEffectDefinition.PowerUpDefinitions.Any(d => d.Boost is null))
        {
            throw new InvalidOperationException($"Magic effect definition {magicEffectDefinition.Name} ({magicEffectDefinition.Number}) is without a PowerUpDefinition.");
        }

        if (magicEffectDefinition.Duration is null)
        {
            throw new InvalidOperationException($"Magic effect definition {magicEffectDefinition.Name} ({magicEffectDefinition.Number}) has no duration.");
        }

        int i = 0;
        var result = new (AttributeDefinition Target, IElement BuffPowerUp)[magicEffectDefinition.PowerUpDefinitions.Count];
        foreach (var powerUpDef in magicEffectDefinition.PowerUpDefinitions)
        {
            IElement powerUp = this.Attributes!.CreateElement(powerUpDef);

            result[i] = (powerUpDef.TargetAttribute!, powerUp);
            i++;
        }

        return (this.Attributes!.CreateDurationElement(magicEffectDefinition.Duration), result);
    }

    /// <summary>
    /// Creates a summoned monster for the player.
    /// </summary>
    /// <param name="definition">The definition.</param>
    /// <exception cref="InvalidOperationException">Can't add a summon for a player which isn't spawned yet.</exception>
    public async ValueTask CreateSummonedMonsterAsync(MonsterDefinition definition)
    {
        if (this.CurrentMap is not { } gameMap)
        {
            throw new InvalidOperationException("Can't add a summon for a player which isn't spawned yet.");
        }

        var area = new MonsterSpawnArea
        {
            GameMap = gameMap.Definition,
            MonsterDefinition = definition,
            SpawnTrigger = SpawnTrigger.OnceAtEventStart,
            Quantity = 1,
            X1 = (byte)Math.Max(this.Position.X - 3, byte.MinValue),
            X2 = (byte)Math.Min(this.Position.X + 3, byte.MaxValue),
            Y1 = (byte)Math.Max(this.Position.Y - 3, byte.MinValue),
            Y2 = (byte)Math.Min(this.Position.Y + 3, byte.MaxValue),
        };
        var intelligence = new SummonedMonsterIntelligence(this);
        var monster = new Monster(area, definition, gameMap, NullDropGenerator.Instance, intelligence, this.GameContext.PlugInManager, this.GameContext.PathFinderPool);
        area.MaximumHealthOverride = (int)monster.Attributes[Stats.MaximumHealth];
        area.MaximumHealthOverride += (int)(monster.Attributes[Stats.MaximumHealth] * this.Attributes?[Stats.SummonedMonsterHealthIncrease] ?? 0);
        // todo: Stats.SummonedMonsterDefenseIncrease
        this.Summon = (monster, intelligence);
        monster.Initialize();
        await gameMap.AddAsync(monster).ConfigureAwait(false);
        monster.OnSpawn();
    }

    /// <summary>
    /// Notifies the player object that the summon died.
    /// </summary>
    public void SummonDied()
    {
        this.Summon = null;
    }

    /// <summary>
    /// Removes the summon.
    /// </summary>
    public async ValueTask RemoveSummonAsync()
    {
        if (this.Summon is { } summon)
        {
            // remove summon, if exists
            await summon.Item1.CurrentMap.RemoveAsync(summon.Item1).ConfigureAwait(false);
            summon.Item1.Dispose();
            this.SummonDied();
        }
    }

    /// <summary>
    /// Removes the pet command manager.
    /// </summary>
    public void RemovePetCommandManager()
    {
        this._petCommandManager?.Dispose();
        this._petCommandManager = null;
    }

    /// <summary>
    /// Destroys an item of the <see cref="Inventory"/>.
    /// </summary>
    /// <param name="item">The item.</param>
    public async ValueTask DestroyInventoryItemAsync(Item item)
    {
        await this.Inventory!.RemoveItemAsync(item).ConfigureAwait(false);
        await this.PersistenceContext.DeleteAsync(item).ConfigureAwait(false);
        await this.InvokeViewPlugInAsync<IItemRemovedPlugIn>(p => p.RemoveItemAsync(item.ItemSlot)).ConfigureAwait(false);
        this.GameContext.PlugInManager.GetPlugInPoint<IItemDestroyedPlugIn>()?.ItemDestroyed(item);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        string accountName = string.Empty;
        string characterName = string.Empty;
        if (this.Account != null)
        {
            accountName = this.Account.LoginName;
            if (this._selectedCharacter != null)
            {
                characterName = this._selectedCharacter.Name;
            }
        }

        return $"Account: [{accountName}], Character:[{characterName}]";
    }

    /// <summary>
    /// Gets the size of the inventory of the specified player.
    /// </summary>
    /// <returns>The size of the inventory.</returns>
    public byte GetInventorySize()
    {
        if (this.SelectedCharacter is not { } selectedCharacter)
        {
            return 0;
        }

        return (byte)InventoryConstants.GetInventorySize(selectedCharacter.InventoryExtensions);
    }

    /// <summary>
    /// Resets the pet behavior.
    /// </summary>
    public async ValueTask ResetPetBehaviorAsync()
    {
        if (this.PetCommandManager is { } petCommandManager)
        {
            await petCommandManager.SetBehaviourAsync(PetBehaviour.Idle, null).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Removes the player from the game and saves its state.
    /// </summary>
    public async ValueTask RemoveFromGameAsync()
    {
        var moveToNextSafezone = false;
        if (this._respawnAfterDeathCts is { IsCancellationRequested: false })
        {
            await this._respawnAfterDeathCts.CancelAsync().ConfigureAwait(false);
            moveToNextSafezone = true;
        }

        if (this.CurrentMiniGame is { })
        {
            moveToNextSafezone = true;
        }

        if (this.DuelRoom is { })
        {
            moveToNextSafezone = true;
        }

        if (moveToNextSafezone)
        {
            await this.WarpToSafezoneAsync().ConfigureAwait(false);
        }

        await this.RemoveFromCurrentMapAsync().ConfigureAwait(false);
        if (this.Party is { } party)
        {
            await party.KickMySelfAsync(this).ConfigureAwait(false);
        }

        await this.SetSelectedCharacterAsync(null).ConfigureAwait(false);
        await this.MagicEffectList.ClearAllEffectsAsync().ConfigureAwait(false);

        try
        {
            await this.PersistenceContext.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Couldn't save when leaving the game. Player: {player}", this);
        }
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        await this._muHelperLazy.DisposeIfCreatedAsync().ConfigureAwait(false);

        this._petCommandManager?.Dispose();
        this._petCommandManager = null;
        this.LastAttackedTarget.SetTarget(null);

        this.PersistenceContext.Dispose();
        await this.RemoveFromCurrentMapAsync().ConfigureAwait(false);
        if (this.Party is { } party)
        {
            await party.KickMySelfAsync(this).ConfigureAwait(false);
        }

        await this._observerToWorldViewAdapter.ClearObservingObjectsListAsync().ConfigureAwait(false);
        this._observerToWorldViewAdapter.Dispose();
        this._walker.Dispose();
        await this.MagicEffectList.DisposeAsync().ConfigureAwait(false);
        this._respawnAfterDeathCts?.Dispose();

        this.PlayerDisconnected = null;
        this.PlayerEnteredWorld = null;
        this.PlayerLeftWorld = null;
        this.PlayerPickedUpItem = null;

        await base.DisposeAsyncCore().ConfigureAwait(false);
    }

    /// <summary>
    /// Is called, when <see cref="DisconnectAsync"/> is called.
    /// </summary>
    protected virtual async ValueTask InternalDisconnectAsync()
    {
        await this.RemoveFromGameAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Creates the view plug in container.
    /// </summary>
    /// <returns>The created view plug in container.</returns>
    protected virtual ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer()
    {
        throw new NotImplementedException("CreateViewPlugInContainer must be overwritten in derived classes.");
    }

    private async ValueTask<bool> TryRemoveFromCurrentMapAsync()
    {
        var currentMap = this.CurrentMap;
        if (currentMap is null)
        {
            return false;
        }

        await currentMap.RemoveAsync(this).ConfigureAwait(false);
        this.IsAlive = false;
        this.IsTeleporting = false;
        await this._walker.StopAsync().ConfigureAwait(false);
        await this._observerToWorldViewAdapter.ClearObservingObjectsListAsync().ConfigureAwait(false);
        if (this.Summon?.Item1 is { IsAlive: true } summon)
        {
            await currentMap.RemoveAsync(summon).ConfigureAwait(false);
        }

        return true;
    }

    private void PlaceAtGate(ExitGate gate)
    {
        this.SelectedCharacter!.PositionX = (byte)Rand.NextInt(gate.X1, gate.X2);
        this.SelectedCharacter.PositionY = (byte)Rand.NextInt(gate.Y1, gate.Y2);
        this.SelectedCharacter.CurrentMap = gate.Map;
        this.Rotation = gate.Direction;

        if (this.Summon?.Item1 is { IsAlive: true } summon)
        {
            summon.Position = gate.GetRandomPoint();
            summon.Rotation = gate.Direction;
        }
    }

    private async ValueTask RemoveFromCurrentMapAsync()
    {
        if (this.CurrentMap != null)
        {
            await this.CurrentMap.RemoveAsync(this).ConfigureAwait(false);
            this.CurrentMap = null;
        }
    }

    private async ValueTask RegenerateHeroStateAsync()
    {
        var currentCharacter = this._selectedCharacter;
        if (currentCharacter?.StateRemainingSeconds > 0)
        {
            var secondsSinceLastRegenerate = this._lastRegenerate.Subtract(DateTime.UtcNow).TotalSeconds;
            currentCharacter.StateRemainingSeconds -= (int)Math.Round(secondsSinceLastRegenerate);
            if (currentCharacter.StateRemainingSeconds <= 0)
            {
                // Change the status
                if (currentCharacter.State > HeroState.Normal)
                {
                    currentCharacter.State--;
                }
                else if (currentCharacter.State < HeroState.Normal)
                {
                    currentCharacter.State++;
                }

                await this.ForEachWorldObserverAsync<IUpdateCharacterHeroStatePlugIn>(p => p.UpdateCharacterHeroStateAsync(this), true).ConfigureAwait(false);
                currentCharacter.StateRemainingSeconds = currentCharacter.State == HeroState.Normal
                    ? 0
                    : (int)TimeSpan.FromHours(1).TotalSeconds;
            }
        }
    }

    /// <summary>
    /// Gets the step delay depending on the equipped items.
    /// </summary>
    /// <returns>The current step delay, depending on equipped items.</returns>
    private TimeSpan GetStepDelay()
    {
        if (this.Inventory?.EquippedItems.Any(item => item.Definition?.ItemSlot?.ItemSlots.Contains(7) ?? false) ?? false)
        {
            // Wings
            return TimeSpan.FromMilliseconds(300);
        }

        // TODO: Consider pets etc.
        return TimeSpan.FromMilliseconds(500);
    }

    private async ValueTask<ExitGate> GetSpawnGateOfCurrentMapAsync()
    {
        if (this.CurrentMap is null)
        {
            throw new InvalidOperationException("CurrentMap is not set. Can't determine spawn gate.");
        }

        if (this.DuelRoom is { State: DuelState.DuelAccepted or DuelState.DuelStarted } duelRoom
            && duelRoom.GetSpawnGate(this) is { } duelExitGate)
        {
            return duelExitGate;
        }

        if (this.GuildWarContext?.WarType == GuildWarType.Soccer
            && this.GuildWarContext.State == GuildWarState.Started
            && this.CurrentMap is SoccerGameMap soccerGameMap
            && soccerGameMap.Definition.BattleZone?.Ground is { } ground)
        {
            return new ExitGate
            {
                Map = soccerGameMap.Definition,
                X1 = ground.X1,
                X2 = ground.X2,
                Y1 = ground.Y1,
                Y2 = ground.Y2,
            };
        }

        var spawnTargetMapDefinition = this.CurrentMap.Definition.SafezoneMap ?? this.CurrentMap.Definition;
        var targetMap = await this.GameContext.GetMapAsync((ushort)spawnTargetMapDefinition.Number, false).ConfigureAwait(false);
        return targetMap?.SafeZoneSpawnGate
               ?? spawnTargetMapDefinition.GetSafezoneGate()
               ?? throw new InvalidOperationException($"Game map {spawnTargetMapDefinition} has no spawn gate.");
    }

    private async ValueTask HitAsync(HitInfo hitInfo, IAttacker attacker, Skill? skill)
    {
        this.Summon?.Item2.RegisterHit(attacker);
        var healthDamage = hitInfo.HealthDamage;
        int oversd = (int)(this.Attributes![Stats.CurrentShield] - hitInfo.ShieldDamage);
        if (oversd < 0)
        {
            this.Attributes[Stats.CurrentShield] = 0;
            healthDamage += (uint)(oversd * (-1));
        }
        else
        {
            this.Attributes[Stats.CurrentShield] = oversd;
        }

        this.Attributes[Stats.CurrentHealth] -= healthDamage;
        await this.InvokeViewPlugInAsync<IShowHitPlugIn>(p => p.ShowHitAsync(this, hitInfo)).ConfigureAwait(false);
        if (attacker is IWorldObserver observer)
        {
            await observer.InvokeViewPlugInAsync<IShowHitPlugIn>(p => p.ShowHitAsync(this, hitInfo)).ConfigureAwait(false);
        }

        this.GameContext.PlugInManager.GetPlugInPoint<IAttackableGotHitPlugIn>()?.AttackableGotHit(this, attacker, hitInfo);

        if (this.Attributes[Stats.CurrentHealth] < 1)
        {
            this.LastDeath = new DeathInformation(attacker.Id, attacker.GetName(), hitInfo, skill?.Number ?? 0);
            await this.OnDeathAsync(attacker).ConfigureAwait(false);
        }

        if (hitInfo.Attributes.HasFlag(DamageAttributes.Poison))
        {
            // Poison Damage does not reflect to the attacker.
            return;
        }

        var reflectPercentage = this.Attributes[Stats.DamageReflection];
        if (reflectPercentage > 0 && attacker is IAttackable attackableAttacker)
        {
            var reflectedDamage = (int)((hitInfo.HealthDamage + hitInfo.ShieldDamage) * reflectPercentage);
            if (reflectedDamage <= 0)
            {
                return;
            }

            _ = Task.Run(async () =>
            {
                await Task.Delay(500).ConfigureAwait(false);
                if (attackableAttacker.IsAlive)
                {
                    await attackableAttacker.ReflectDamageAsync(this, (uint)reflectedDamage).ConfigureAwait(false);
                }
            });
        }
    }

    private async ValueTask OnDeathAsync(IAttacker? killer)
    {
        if (!await this.PlayerState.TryAdvanceToAsync(GameLogic.PlayerState.Dead).ConfigureAwait(false))
        {
            return;
        }

        await this._walker.StopAsync().ConfigureAwait(false);
        this.IsAlive = false;
        this._respawnAfterDeathCts = new CancellationTokenSource();
        await this.ForEachWorldObserverAsync<IObjectGotKilledPlugIn>(p => p.ObjectGotKilledAsync(this, killer), true).ConfigureAwait(false);

        if (killer is Player killerAfterKilled
            && !(killerAfterKilled.GuildWarContext?.Score is { } score && score == this.GuildWarContext?.Score)
            && this.CurrentMiniGame?.AllowPlayerKilling is not true)
        {
            await killerAfterKilled.AfterKilledPlayerAsync(this).ConfigureAwait(false);
        }

        // TODO: Drop items
        async Task RespawnAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(3000, cancellationToken).ConfigureAwait(false);
                if (this.Summon?.Item1 is { } summon)
                {
                    await summon.CurrentMap.RemoveAsync(summon).ConfigureAwait(false);
                    summon.Dispose();
                    this.Summon = null;
                }

                await this.MagicEffectList.ClearEffectsAfterDeathAsync().ConfigureAwait(false);
                this.SetReclaimableAttributesToMaximum();
                await this.RespawnAtAsync(await this.GetSpawnGateOfCurrentMapAsync().ConfigureAwait(false)).ConfigureAwait(false);
                await this.RespawnOfDuelPartnerIfInDuelAsync().ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Intended exception, so no need to handle that.
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Unexpected error during respawning the character {this}: {ex}", this, ex);
            }
        }

        _ = RespawnAsync(this._respawnAfterDeathCts.Token);

        if (this.GameContext.PlugInManager.GetPlugInPoint<IAttackableGotKilledPlugIn>() is { } plugInPoint)
        {
            await plugInPoint.AttackableGotKilledAsync(this, killer);
        }

        if (this.LastDeath is { } deathInformation)
        {
            this.Died?.Invoke(this, deathInformation);
        }
    }

    /// <summary>
    /// Called when this player is in a duel and was killed.
    /// Sets the player back to its starting position and reclaims the attributes,
    /// so that they're ready for the next round.
    /// </summary>
    private async ValueTask RespawnOfDuelPartnerIfInDuelAsync()
    {
        if (this.DuelRoom is { State: DuelState.DuelStarted } duelRoom
            && duelRoom.IsDuelist(this)
            && (duelRoom.Requester == this ? duelRoom.Opponent : duelRoom.Requester) is { IsAlive: true, CurrentMap: not null } duelPartner
            && duelRoom.GetSpawnGate(duelPartner) is { } partnerSpawnGate)
        {
            duelPartner.IsAlive = false; // Avoid ending the duel...
            duelPartner.SetReclaimableAttributesToMaximum();
            await duelPartner.RespawnAtAsync(partnerSpawnGate).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Is called after the player killed a <see cref="Player"/>.
    /// Increment PK Level.
    /// </summary>
    private async ValueTask AfterKilledPlayerAsync(Player killedPlayer)
    {
        if (this.DuelRoom?.State == DuelState.DuelStarted)
        {
            return;
        }

        var killedPlayerState = killedPlayer.SelectedCharacter?.State;
        if (killedPlayerState is null)
        {
            return;
        }

        if (killedPlayerState >= HeroState.PlayerKiller1stStage)
        {
            // Killing PKs is allowed.
            return;
        }

        if (killedPlayerState <= HeroState.PlayerKillWarning
            && this.IsSelfDefenseActive(killedPlayer))
        {
            // Self defense is allowed.
            return;
        }

        if (this._selectedCharacter!.State != HeroState.PlayerKiller2ndStage)
        {
            if (this._selectedCharacter.State < HeroState.Normal)
            {
                this._selectedCharacter.State = HeroState.PlayerKillWarning;
            }
            else
            {
                this._selectedCharacter.State++;
            }
        }

        this._selectedCharacter.StateRemainingSeconds += (int)TimeSpan.FromHours(1).TotalSeconds;
        this._selectedCharacter.PlayerKillCount += 1;
        await this.ForEachWorldObserverAsync<IUpdateCharacterHeroStatePlugIn>(o => o.UpdateCharacterHeroStateAsync(this), true).ConfigureAwait(false);
    }

    private SkillComboDefinition? DetermineComboDefinition()
    {
        var characterClass = this.SelectedCharacter!.CharacterClass;

        while (characterClass is { })
        {
            if (characterClass.ComboDefinition is { } comboDefinition)
            {
                return comboDefinition;
            }

            // Check previous class
            characterClass = this.GameContext.Configuration.CharacterClasses.FirstOrDefault(c => c.NextGenerationClass == characterClass);
        }

        return null;
    }

    private void RaisePlayerEnteredMap(GameMap map)
    {
        this.PlayerEnteredMap?.Invoke(this, (this, map));
        if (map.Definition.CharacterPowerUpDefinitions is { Count: > 0 } powerUpDefinitions
            && this.Attributes is { } attributes)
        {
            foreach (var powerUpDefinition in powerUpDefinitions)
            {
                if (powerUpDefinition.TargetAttribute is not { } targetAttribute)
                {
                    continue;
                }

                var powerUps = PowerUpWrapper.CreateByPowerUpDefinition(powerUpDefinition, attributes);
                powerUps.ForEach(p =>
                {
                    this.Attributes?.AddElement(p, targetAttribute);
                    this.PlayerLeftMap += OnPlayerLeftMap;

                    void OnPlayerLeftMap(object? o, (Player, GameMap) args)
                    {
                        this.PlayerLeftMap -= OnPlayerLeftMap;
                        attributes.RemoveElement(p, targetAttribute);
                    }
                });
            }
        }
    }

    private void RaisePlayerLeftMap(GameMap map)
    {
        this.PlayerLeftMap?.Invoke(this, (this, map));
    }

    /// <summary>
    /// Adds the missing stat attributes, e.g. after the character class has been changed outside of the game.
    /// </summary>
    private void AddMissingStatAttributes()
    {
        if (this.SelectedCharacter is not { CharacterClass: { } characterClass } character)
        {
            throw new InvalidOperationException($"The character {this.SelectedCharacter} has no assigned character class.");
        }

        var missingStats = characterClass.StatAttributes.Where(a => this.SelectedCharacter.Attributes.All(c => c.Definition != a.Attribute));

        var attributes = missingStats.Select(a => this.PersistenceContext.CreateNew<StatAttribute>(a.Attribute, a.BaseValue)).ToList();
        attributes.ForEach(character.Attributes.Add);
    }

    private async ValueTask OnPlayerEnteredWorldAsync()
    {
        if (this.SelectedCharacter is null)
        {
            throw new InvalidOperationException($"The player has no selected character.");
        }

        if (this.SelectedCharacter?.CharacterClass is null)
        {
            throw new InvalidOperationException($"The character '{this.SelectedCharacter}' has no assigned character class.");
        }

        // For characters which got created on the database or with the admin panel,
        // it's possible that they're missing the inventory. In this case, we create it here
        // and initialize with default items.
        if (this.SelectedCharacter!.Inventory is null)
        {
            this.SelectedCharacter.Inventory = this.PersistenceContext.CreateNew<ItemStorage>();
            this.GameContext.PlugInManager.GetPlugInPoint<ICharacterCreatedPlugIn>()?.CharacterCreated(this, this.SelectedCharacter);
        }

        this.SelectedCharacter.CurrentMap ??= this.SelectedCharacter.CharacterClass?.HomeMap;
        this.AddMissingStatAttributes();

        this.Attributes = new ItemAwareAttributeSystem(this.Account!, this.SelectedCharacter!);
        this.LogInvalidInventoryItems();

        this.Inventory = new InventoryStorage(this, this.GameContext);
        this.ShopStorage = new ShopStorage(this);
        this.TemporaryStorage = new Storage(InventoryConstants.TemporaryStorageSize, new TemporaryItemStorage());
        this.Vault = null; // vault storage is getting set when vault npc is opened.
        this.SkillList = new SkillList(this);
        this.SetReclaimableAttributesBeforeEnterGame();
        if (this.DetermineComboDefinition() is { } comboDefinition)
        {
            this._comboStateLazy = new Lazy<ComboStateMachine>(() => ComboStateMachine.Create(comboDefinition));
        }

        await this.InvokeViewPlugInAsync<IUpdateCharacterStatsPlugIn>(p => p.UpdateCharacterStatsAsync()).ConfigureAwait(false);
        await this.InvokeViewPlugInAsync<IUpdateInventoryListPlugIn>(p => p.UpdateInventoryListAsync()).ConfigureAwait(false);
        await this.InvokeViewPlugInAsync<ISkillListViewPlugIn>(p => p.UpdateSkillListAsync()).ConfigureAwait(false);
        await this.InvokeViewPlugInAsync<IApplyKeyConfigurationPlugIn>(p => p.ApplyKeyConfigurationAsync()).ConfigureAwait(false);
        await this.InvokeViewPlugInAsync<IQuestStateResponsePlugIn>(p => p.ShowQuestStateAsync(null)).ConfigureAwait(false); // Legacy quest system
        await this.InvokeViewPlugInAsync<ICurrentlyActiveQuestsPlugIn>(p => p.ShowActiveQuestsAsync()).ConfigureAwait(false); // New quest system

        this.Attributes.GetOrCreateAttribute(Stats.MaximumMana).ValueChanged += this.OnMaximumManaOrAbilityChanged;
        this.Attributes.GetOrCreateAttribute(Stats.MaximumAbility).ValueChanged += this.OnMaximumManaOrAbilityChanged;
        this.Attributes.GetOrCreateAttribute(Stats.MaximumHealth).ValueChanged += this.OnMaximumHealthOrShieldChanged;
        this.Attributes.GetOrCreateAttribute(Stats.MaximumShield).ValueChanged += this.OnMaximumHealthOrShieldChanged;
        this.Attributes.GetOrCreateAttribute(Stats.TransformationSkin).ValueChanged += this.OnTransformationSkinChanged;

        var ammoAttribute = this.Attributes.GetOrCreateAttribute(Stats.AmmunitionAmount);
        this.Attributes[Stats.AmmunitionAmount] = (float)(this.Inventory?.EquippedAmmunitionItem?.Durability ?? 0);
        ammoAttribute.ValueChanged += this.OnAmmunitionAmountChanged;

        await this.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);

        await this.InvokeViewPlugInAsync<IUpdateRotationPlugIn>(p => p.UpdateRotationAsync()).ConfigureAwait(false);
        await this.ResetPetBehaviorAsync().ConfigureAwait(false);

        if (this.SelectedCharacter?.MuHelperConfiguration is { } muHelperConfiguration)
        {
            await this.InvokeViewPlugInAsync<IMuHelperConfigurationUpdatePlugIn>(p => p.UpdateMuHelperConfigurationAsync(muHelperConfiguration)).ConfigureAwait(false);
        }

        // Add GM mark (mu logo above character's head)
        if (this.SelectedCharacter?.CharacterStatus == CharacterStatus.GameMaster)
        {
            await this.MagicEffectList.AddEffectAsync(new MagicEffect(
            TimeSpan.FromMilliseconds((double)int.MaxValue),
            GMEffect)).ConfigureAwait(false);
        }
    }

    private void LogInvalidVaultItems()
    {
        var invalidItems = this.Account?.Vault?.Items.Where(i => i.Definition is null);
        if (invalidItems is null)
        {
            return;
        }

        foreach (var item in invalidItems)
        {
            this.Logger.LogWarning("Account {name} has item without definition in vault, Slot: {slot}, ID: {id}", this.Account?.LoginName, item.ItemSlot, item.GetId());
        }
    }

    private void LogInvalidInventoryItems()
    {
        var invalidItems = this.SelectedCharacter?.Inventory?.Items.Where(i => i.Definition is null);
        if (invalidItems is null)
        {
            return;
        }

        foreach (var item in invalidItems)
        {
            this.Logger.LogWarning("Character {name} has item without definition in inventory, Slot: {slot}, ID: {id}", this.SelectedCharacter?.Name, item.ItemSlot, item.GetId());
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    private async void OnTransformationSkinChanged(object? sender, EventArgs args)
    {
        try
        {
            await this.ForEachWorldObserverAsync<INewPlayersInScopePlugIn>(p => p.NewPlayersInScopeAsync(this.GetAsEnumerable()), true).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, nameof(this.OnTransformationSkinChanged));
        }
    }

    /// <summary>
    /// Sets the reclaimable attributes before a character enters the game.
    /// Current shield and mana is set to their maximum values.
    /// Current ability starts at the half of the maximum (as at the original server).
    /// The current health value was restored from the previous session and is not set to the maximum value - it's just limited by the maximum value.
    /// </summary>
    private void SetReclaimableAttributesBeforeEnterGame()
    {
        this.Attributes![Stats.CurrentShield] = this.Attributes[Stats.MaximumShield];
        this.Attributes[Stats.CurrentMana] = this.Attributes[Stats.MaximumMana];
        this.Attributes[Stats.CurrentAbility] = this.Attributes[Stats.MaximumAbility] / 2;
        this.Attributes[Stats.CurrentHealth] = Math.Min(this.Attributes[Stats.CurrentHealth], this.Attributes[Stats.MaximumHealth]);
    }

    private void SetReclaimableAttributesToMaximum()
    {
        foreach (var regeneration in Stats.IntervalRegenerationAttributes)
        {
            this.Attributes![regeneration.CurrentAttribute] = this.Attributes[regeneration.MaximumAttribute];
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    private async void OnMaximumHealthOrShieldChanged(object? sender, EventArgs args)
    {
        try
        {
            this.Attributes![Stats.CurrentHealth] = Math.Min(this.Attributes[Stats.CurrentHealth], this.Attributes[Stats.MaximumHealth]);
            this.Attributes[Stats.CurrentShield] = Math.Min(this.Attributes[Stats.CurrentShield], this.Attributes[Stats.MaximumShield]);
            await this.InvokeViewPlugInAsync<IUpdateMaximumHealthPlugIn>(p => p.UpdateMaximumHealthAsync()).ConfigureAwait(false);
            await this.InvokeViewPlugInAsync<IUpdateCurrentHealthPlugIn>(p => p.UpdateCurrentHealthAsync()).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, nameof(this.OnMaximumHealthOrShieldChanged));
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    private async void OnAmmunitionAmountChanged(object? sender, EventArgs args)
    {
        try
        {
            var value = Math.Max((byte)this.Attributes![Stats.AmmunitionAmount], (byte)0);
            if (this.Inventory?.EquippedAmmunitionItem is { } ammoItem
                && (int)ammoItem.Durability != value)
            {
                ammoItem.Durability = value;
                if (ammoItem.Durability == 0)
                {
                    await this.DestroyInventoryItemAsync(ammoItem).ConfigureAwait(false);
                }
                else
                {
                    await this.InvokeViewPlugInAsync<IItemDurabilityChangedPlugIn>(p => p.ItemDurabilityChangedAsync(ammoItem, false)).ConfigureAwait(false);
                }
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, nameof(this.OnAmmunitionAmountChanged));
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    private async void OnMaximumManaOrAbilityChanged(object? sender, EventArgs args)
    {
        try
        {
            this.Attributes![Stats.CurrentMana] = Math.Min(this.Attributes[Stats.CurrentMana], this.Attributes[Stats.MaximumMana]);
            this.Attributes[Stats.CurrentAbility] = Math.Min(this.Attributes[Stats.CurrentAbility], this.Attributes[Stats.MaximumAbility]);
            await this.InvokeViewPlugInAsync<IUpdateMaximumManaPlugIn>(p => p.UpdateMaximumManaAsync()).ConfigureAwait(false);
            await this.InvokeViewPlugInAsync<IUpdateCurrentManaPlugIn>(p => p.UpdateCurrentManaAsync()).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, nameof(this.OnMaximumManaOrAbilityChanged));
        }
    }

    private async ValueTask DecreaseItemDurabilityAfterHitAsync(HitInfo hitInfo)
    {
        var randomArmorItem = this.Inventory?.EquippedItems.Where(ItemExtensions.IsDefensiveItem).SelectRandom();
        if (randomArmorItem is { })
        {
            await this.DecreaseDefenseItemDurabilityAsync(randomArmorItem, hitInfo).ConfigureAwait(false);
        }

        if (this.Inventory?.GetItem(InventoryConstants.PetSlot) is { Durability: > 0.0 } pet)
        {
            await this.DecreaseDefenseItemDurabilityAsync(pet, hitInfo).ConfigureAwait(false);
            if (pet.Durability == 0.0)
            {
                if (pet.IsTrainablePet())
                {
                    var minimumExp = pet.Definition!.GetExperienceOfPetLevel(pet.Level, pet.Definition!.MaximumItemLevel);
                    pet.PetExperience = (int)Math.Max((int)(pet.PetExperience * 0.9), minimumExp);
                }
                else
                {
                    await this.DestroyInventoryItemAsync(pet).ConfigureAwait(false);
                }
            }
        }
    }

    private async ValueTask DecreaseDefenseItemDurabilityAsync(Item targetItem, HitInfo hitInfo)
    {
        var itemDurationIncrease = targetItem.IsTrainablePet() ? this.Attributes?[Stats.PetDurationIncrease] : this.Attributes?[Stats.ItemDurationIncrease];
        if (itemDurationIncrease == 0)
        {
            itemDurationIncrease = 1;
        }

        var damageDivisor = targetItem.IsTrainablePet() ? this.GameContext.Configuration.DamagePerOnePetDurability : this.GameContext.Configuration.DamagePerOneItemDurability;
        if (itemDurationIncrease.HasValue)
        {
            damageDivisor *= (double)itemDurationIncrease;
        }

        var decrement = hitInfo.HealthDamage / damageDivisor;
        if (targetItem.DecreaseDurability(decrement))
        {
            await this.InvokeViewPlugInAsync<IItemDurabilityChangedPlugIn>(p => p.ItemDurabilityChangedAsync(targetItem, false)).ConfigureAwait(false);
        }
    }

    private async ValueTask DecreaseWeaponDurabilityAfterHitAsync()
    {
        var targetItem = this.Inventory?.GetRandomOffensiveItem();
        if (targetItem is null || targetItem.Durability == 0)
        {
            return;
        }

        var decrement = 1.0 / this.GameContext.Configuration.HitsPerOneItemDurability;
        if (targetItem.DecreaseDurability(decrement))
        {
            await this.InvokeViewPlugInAsync<IItemDurabilityChangedPlugIn>(p => p.ItemDurabilityChangedAsync(targetItem, false)).ConfigureAwait(false);

            if (targetItem is { Durability: 0.0 } pet && pet.IsTrainablePet())
            {
                var minimumExp = pet.Definition!.GetExperienceOfPetLevel(pet.Level, pet.Definition!.MaximumItemLevel);
                pet.PetExperience = (int)Math.Max((int)(pet.PetExperience * 0.9), minimumExp);
                await this.ResetPetBehaviorAsync().ConfigureAwait(false);
            }
        }
    }

    private async ValueTask AddPetExperienceAsync(double gainedExperience)
    {
        async ValueTask AddExpToPetAsync(Item pet, double experience)
        {
            pet.PetExperience += (int)experience;

            while (pet.PetExperience >= pet.Definition!.GetExperienceOfPetLevel((byte)(pet.Level + 1), pet.Definition!.MaximumItemLevel))
            {
                pet.Level++;

                await this.InvokeViewPlugInAsync<IPetInfoViewPlugIn>(p => p.ShowPetInfoAsync(pet, pet.ItemSlot, PetStorageLocation.InventoryPetSlot)).ConfigureAwait(false);
            }
        }

        Item? GetTrainablePet(byte inventorySlot)
        {
            if (this.Inventory?.GetItem(inventorySlot) is
                {
                    Definition.PetExperienceFormula: not null,
                    Definition.MaximumItemLevel: > 0,
                    Durability: > 0
                } pet
                && pet.Level < pet.Definition.MaximumItemLevel)
            {
                return pet;
            }

            return null;
        }

        const double petShare = 0.2;
        Item? movePet = GetTrainablePet(InventoryConstants.PetSlot);
        Item? attackPet = GetTrainablePet(InventoryConstants.RightHandSlot);

        if (movePet is null && attackPet is null)
        {
            return;
        }

        var petExperience = (int)(gainedExperience * petShare);

        if (movePet is not null && attackPet is not null)
        {
            // Both are there, so each gains just the half.
            petExperience /= 2;
        }

        if (petExperience < 1)
        {
            return;
        }

        if (movePet is { })
        {
            await AddExpToPetAsync(movePet, petExperience).ConfigureAwait(false);
        }

        if (attackPet is { })
        {
            await AddExpToPetAsync(attackPet, petExperience).ConfigureAwait(false);
        }
    }

    private sealed class TemporaryItemStorage : ItemStorage
    {
        public TemporaryItemStorage()
        {
            this.Items = new List<Item>();
        }
    }

    private class AppearanceDataAdapter : IAppearanceData
    {
        private readonly Player _player;
        private bool? _fullAncientSetEquipped;

        public AppearanceDataAdapter(Player player)
        {
            this._player = player;
        }

        public event EventHandler? AppearanceChanged;

        public CharacterClass? CharacterClass => this._player.SelectedCharacter?.CharacterClass;

        public CharacterPose Pose => this._player.SelectedCharacter?.Pose ?? default;

        public bool FullAncientSetEquipped => (this._fullAncientSetEquipped ??= this._player.SelectedCharacter?.HasFullAncientSetEquipped()) ?? false;

        public IEnumerable<ItemAppearance> EquippedItems
        {
            get
            {
                if (this._player.Inventory != null)
                {
                    return this._player.Inventory.EquippedItems.Select(item => item.GetAppearance());
                }

                return Enumerable.Empty<ItemAppearance>();
            }
        }

        /// <summary>
        /// Raises the <see cref="AppearanceChanged"/> event.
        /// </summary>
        public void RaiseAppearanceChanged()
        {
            this._fullAncientSetEquipped = null;
            this.AppearanceChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private protected sealed class GMMagicEffectDefinition : MagicEffectDefinition
    {
        public GMMagicEffectDefinition()
        {
            this.PowerUpDefinitions = new List<PowerUpDefinition>(0);
        }
    }
}
