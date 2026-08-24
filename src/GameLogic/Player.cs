// <copyright file="Player.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System;
using System.Globalization;
using System.Threading;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.GuildWar;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Pet;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.GameLogic.Views.MuHelper;
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
    private static readonly MagicEffectDefinition GMEffect = new GameMasterMagicEffectDefinition
    {
        InformObservers = true,
        Name = "GM MARK",
        Number = 28,
        StopByDeath = false,
    };

    private readonly PlayerExperience _experience;

    /// <summary>
    /// Serializes context mutations done by this player's action handlers against the periodic and
    /// disconnect progress saves, which run on an independent timer flow. See
    /// <see cref="RunPersistenceExclusiveAsync{T}"/>.
    /// </summary>
    private readonly PlayerPersistence _persistence;

    private readonly PlayerMovement _movement;

    private readonly PlayerSummon _summon;

    private readonly PlayerStorages _storages;

    private readonly PlayerAppearanceData _appearanceData;

    private readonly ObserverToWorldViewAdapter _observerToWorldViewAdapter;

    private readonly Lazy<MuHelper.MuHelper> _muHelperLazy;

    /// <summary>
    /// Keeps track of the point in time when each regeneration was applied the last time.
    /// </summary>
    private readonly Dictionary<Stats.Regeneration, DateTime> _lastRegenerations = new();

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
        this._persistence = new PlayerPersistence(this);
        this._experience = new PlayerExperience(this);
        this._movement = new PlayerMovement(this);
        this._summon = new PlayerSummon(this);
        this._storages = new PlayerStorages(this);

        this.MagicEffectList = new MagicEffectsList(this);
        this._appearanceData = new PlayerAppearanceData(this);
        this.PlayerState.StateChanged += async args => await (this.GameContext.PlugInManager.GetPlugInPoint<IPlayerStateChangedPlugIn>()?.PlayerStateChangedAsync(this, args.PreviousState, args.CurrentStateState) ?? ValueTask.CompletedTask).ConfigureAwait(false);
        this.PlayerState.StateChanges += async args => await (this.GameContext.PlugInManager.GetPlugInPoint<IPlayerStateChangingPlugIn>()?.PlayerStateChangingAsync(this, args) ?? ValueTask.CompletedTask).ConfigureAwait(false);
        this._observerToWorldViewAdapter = new ObserverToWorldViewAdapter(this, this.InfoRange);
        this._muHelperLazy = new Lazy<MuHelper.MuHelper>(() => new MuHelper.MuHelper(this));
        this.Culture = CultureInfo.CurrentCulture;
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
    public ILogger<Player> Logger { get; protected set; }

    /// <inheritdoc />
    public bool CanWalkOnSafezone => true;

    /// <inheritdoc />
    public bool IsWalking => this._movement.IsWalking;

    /// <inheritdoc />
    public TimeSpan StepDelay => this._movement.StepDelay;

    /// <inheritdoc />
    public Point WalkTarget => this._movement.WalkTarget;

    /// <summary>
    /// Gets a value indicating whether this instance is invisible to other players.
    /// </summary>
    public bool IsInvisible => this.Attributes?[Stats.IsInvisible] > 0;

    /// <inheritdoc />
    public bool IsTemplatePlayer => this.Account?.IsTemplate is true;

    /// <summary>
    /// Gets the culture setting of the player.
    /// </summary>
    public CultureInfo Culture { get; internal set; }

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

    /// <summary>
    /// Gets or sets a custom login result to override the default when login fails.
    /// </summary>
    public Views.Login.LoginResult? LoginResultOverride { get; set; }

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
            if (this._selectedCharacter is not { } character || character.Pose == value)
            {
                return;
            }

            character.Pose = value;

            // A resting character (sitting, leaning, hanging) recovers health and mana faster.
            this.Attributes?.SetStatAttribute(Stats.IsResting, value > CharacterPose.Standing ? 1.0f : 0.0f);
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
                if (this._account is { } account)
                {
                    this._accountLoggingScope = this.Logger.BeginScope("Account: {Name}", this._account.LoginName);
                    this.IsVaultLocked = !string.IsNullOrWhiteSpace(this._account.VaultPassword);
                    this.Culture = CultureInfo.GetCultures(CultureTypes.AllCultures)
                        .FirstOrDefault(cu => cu.TwoLetterISOLanguageName == account.LanguageIsoCode
                                              || cu.ThreeLetterISOLanguageName == account.LanguageIsoCode)
                        ?? CultureInfo.CurrentCulture;
                    this.LogInvalidVaultItems();
                }
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
                if (this.SelectedCharacter is { } selectedCharacter && value is not null)
                {
                    selectedCharacter.CurrentMap = value?.Definition;
                }

                this.GameContext.PlugInManager?.GetPlugInPoint<IAttackableMovedPlugIn>()?.AttackableMoved(this);

                if (this._currentMap is { } newMap)
                {
                    if (this.Attributes is { } attributes)
                    {
                        attributes[Stats.NearbyPartyMemberCount] = 0;
                    }

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
    /// Gets or sets the player who sent a pending alliance request to this player.
    /// </summary>
    public (Player? Player, GuildRelationshipType RelationshipType, GuildRelationshipRequestType RequestType) PendingAllianceRequest { get; set; }

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
    /// Gets the player summon.
    /// </summary>
    public (Monster, INpcIntelligence)? Summon => this._summon.Current;

    /// <inheritdoc/>
    public GuildMemberStatus? GuildStatus { get; set; }

    /// <inheritdoc/>
    public Direction Rotation { get; set; }

    /// <inheritdoc/>
    public Party? Party { get; set; }

    /// <inheritdoc/>
    public bool IsConnected => !this.PlayerState.CurrentState.IsDisconnectedOrFinished();

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

                // A moving character is not resting anymore.
                this.Pose = CharacterPose.Standing;
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
    /// Gets or sets a value indicating whether this player is online as a friend and shown as online in its friends friendlists.
    /// </summary>
    public bool OnlineAsFriend { get; set; } = true;

    /// <inheritdoc cref="IWorldObserver"/>
    public ICustomPlugInContainer<IViewPlugIn> ViewPlugIns => this._viewPlugIns ??= this.CreateViewPlugInContainer();

    /// <inheritdoc/>
    public IInventoryStorage? Inventory => this._storages.Inventory;

    /// <inheritdoc/>
    public IStorage? TemporaryStorage => this._storages.TemporaryStorage;

    /// <summary>
    /// Gets or sets the vault.
    /// </summary>
    public IStorage? Vault
    {
        get => this._storages.Vault;

        set => this._storages.Vault = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the vault of the player is currently locked by a pin.
    /// </summary>
    public bool IsVaultLocked { get; set; }

    /// <summary>
    /// Gets the shop storage.
    /// </summary>
    public IShopStorage? ShopStorage => this._storages.ShopStorage;

    /// <inheritdoc/>
    public BackupItemStorage? BackupInventory
    {
        get => this._storages.BackupInventory;

        set => this._storages.BackupInventory = value;
    }

    /// <summary>
    /// Gets or sets the deserialized MU Helper player settings.
    /// </summary>
    public IMuHelperSettings? MuHelperSettings { get; set; }

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
    /// Gets or sets the mini-game, which the player has currently entered.
    /// </summary>
    public MiniGameContext? CurrentMiniGame { get; set; }

    /// <summary>
    /// Gets the size of the inventory of the current player.
    /// </summary>
    public byte InventorySize
    {
        get
        {
            if (this.SelectedCharacter is not { } selectedCharacter)
            {
                return 0;
            }

            return (byte)InventoryConstants.GetInventorySize(selectedCharacter.InventoryExtensions);
        }
    }

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
                // Since the Raven is currently the only pet that can attack, we directly use it.
                // However, in the future we might use a factory as a strategy plugin here which creates the command manager
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
    /// Gets or sets the cancellation token source for the targeted skills with channeling.
    /// </summary>
    public SkillCancellationTokenSource? SkillCancelTokenSource { get; set; }

    /// <summary>
    /// Gets the mu helper.
    /// </summary>
    public MuHelper.MuHelper MuHelper => this._muHelperLazy.Value;

    /// <summary>
    /// Gets or sets the cooldown timestamp until no further potion can be consumed.
    /// </summary>
    public DateTime PotionCooldownUntil { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets a value indicating whether opening the player store after entering the game is supported by this instance.
    /// </summary>
    protected virtual bool IsPlayerStoreOpeningAfterEnterSupported => true;

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

            (this.SkillList as IDisposable)?.Dispose();
            this.SkillList = null;

            if (this.DuelRoom is { State: DuelState.DuelStarted } duelRoom)
            {
                await duelRoom.CancelDuelAsync().ConfigureAwait(false);
                if (this.GameContext.Configuration.DuelConfiguration?.Exit is { } exit)
                {
                    await this.PlaceAtGateAsync(exit).ConfigureAwait(false);
                }
            }

            this.DuelRoom = null;

            this._selectedCharacter = null;
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
    /// Will be called when an item has been picked up by a player.
    /// </summary>
    /// <param name="item">The item that the player has picked up.</param>
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

    /// <inheritdoc/>
    public async ValueTask<HitInfo?> AttackByAsync(IAttacker attacker, SkillEntry? skill, bool isCombo, double damageFactor = 1.0, bool? isFinalStreakHit = null)
    {
        if (this.Attributes is null)
        {
            throw new InvalidOperationException("AttributeSystem not set.");
        }

        if (this.IsAttackBlockedBySafezone(attacker))
        {
            return null;
        }

        if (!this.GameContext.PvpEnabled && this.CurrentMap?.Definition.BattleZone == null &&
            this.CurrentMiniGame?.AllowPlayerKilling is false)
        {
            return null;
        }

        var hitInfo = await attacker.CalculateDamageAsync(this, skill, isCombo, damageFactor).ConfigureAwait(false);

        if (skill?.Skill is not { } attackSkill || attackSkill.DamageType != DamageType.Fenrir)
        {
            attacker.ApplyAmmunitionConsumption(hitInfo);
        }

        if (hitInfo is { HealthDamage: 0, ShieldDamage: 0 })
        {
            await this.InvokeViewPlugInAsync<IShowHitPlugIn>(p => p.ShowHitAsync(this, hitInfo)).ConfigureAwait(false);
            if (attacker is IWorldObserver observer)
            {
                await observer.InvokeViewPlugInAsync<IShowHitPlugIn>(p => p.ShowHitAsync(this, hitInfo)).ConfigureAwait(false);
            }

            return hitInfo;
        }

        if (this.Attributes[Stats.IsAsleep] > 0)
        {
            await this.MagicEffectList.ClearAllEffectsProducingSpecificStatAsync(Stats.IsAsleep).ConfigureAwait(false);
        }

        if (Rand.NextRandomBool(this.Attributes[Stats.FullyRecoverHealthAfterHitChance]))
        {
            this.Attributes[Stats.CurrentHealth] = this.Attributes[Stats.MaximumHealth];
        }

        var manaFullyRecovered = Rand.NextRandomBool(this.Attributes[Stats.FullyRecoverManaAfterHitChance]);
        if (hitInfo.ManaToll > 0 || manaFullyRecovered)
        {
            this.Attributes[Stats.CurrentMana] = (manaFullyRecovered ? this.Attributes[Stats.MaximumMana] : this.Attributes[Stats.CurrentMana]) - hitInfo.ManaToll;
        }

        await this.HitAsync(hitInfo, attacker, skill?.Skill, isFinalStreakHit).ConfigureAwait(false);
        await this.DecreaseItemDurabilityAfterHitAsync(hitInfo, skill).ConfigureAwait(false);

        if (attacker as IPlayerSurrogate is { } playerSurrogate)
        {
            await playerSurrogate.Owner.AfterHitTargetAsync().ConfigureAwait(false);
        }

        if (attacker is Player attackerPlayer)
        {
            await attackerPlayer.AfterHitTargetAsync().ConfigureAwait(false);

            if (this.IsAlive && Rand.NextRandomBool(attackerPlayer.Attributes![Stats.MaceMasteryStunChance]))
            {
                await attackerPlayer.ApplyMaceMasteryStunEffectAsync(this).ConfigureAwait(false);
            }
        }

        return hitInfo;
    }

    /// <summary>
    /// Is called after the player successfully hit a target.
    /// </summary>
    public async ValueTask AfterHitTargetAsync()
    {
        this.Attributes![Stats.CurrentHealth] = Math.Max(this.Attributes[Stats.CurrentHealth] - this.Attributes[Stats.HealthLossAfterHit], 1);

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

    /// <inheritdoc/>
    public ValueTask ApplyBleedingDamageAsync(IAttacker initialAttacker, uint damage)
    {
        return this.HitAsync(this.GetHitInfo(damage, DamageAttributes.Undefined, initialAttacker), initialAttacker, null);
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
            await (this.SkillCancelTokenSource?.CancelAsync() ?? Task.CompletedTask).ConfigureAwait(false);

            await this._movement.StopWalkingAsync().ConfigureAwait(false);

            if (this.GameContext.PlugInManager.GetPlugInPoint<ISpeedHackCheatCheckPlugIn>() is { } speedCheck)
            {
                await speedCheck.ResetMovementStateAsync(this).ConfigureAwait(false);
            }

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
                    await this._movement.MoveOnMapAsync(map, target, MoveType.Teleport).ConfigureAwait(false);
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
    /// Teleports this player to the specified target map and point.
    /// </summary>
    /// <param name="targetMap">The target map for teleportation.</param>
    /// <param name="targetPoint">The target coordinate in the target map.</param>
    public async Task TeleportToMapAsync(GameMap targetMap, Point targetPoint)
    {
        if (!this.IsAlive)
        {
            return;
        }

        this.IsTeleporting = true;
        try
        {
            await (this.SkillCancelTokenSource?.CancelAsync() ?? Task.CompletedTask).ConfigureAwait(false);

            await this._movement.StopWalkingAsync().ConfigureAwait(false);

            await this.ForEachWorldObserverAsync<IObjectsOutOfScopePlugIn>(p => p.ObjectsOutOfScopeAsync(this.GetAsEnumerable()), false).ConfigureAwait(false);

            if (this.IsAlive)
            {
                ExitGate tempGate = new()
                {
                    Map = targetMap.Definition,
                    X1 = targetPoint.X,
                    X2 = targetPoint.X,
                    Y1 = targetPoint.Y,
                    Y2 = targetPoint.Y,
                };

                await this.WarpToAsync(tempGate).ConfigureAwait(false);
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
    /// Adds recovered mana and health to the player attributes.
    /// </summary>
    public async ValueTask AfterKilledMonsterAsync()
    {
        foreach (var recoverAfterMonsterKill in Stats.AfterMonsterKillRegenerationAttributes)
        {
            var additionalValue = (uint)((this.Attributes![recoverAfterMonsterKill.RegenerationMultiplier] * this.Attributes[recoverAfterMonsterKill.MaximumAttribute]) + this.Attributes[recoverAfterMonsterKill.AbsoluteAttribute]);
            this.Attributes[recoverAfterMonsterKill.CurrentAttribute] = (uint)Math.Min(this.Attributes[recoverAfterMonsterKill.MaximumAttribute], this.Attributes[recoverAfterMonsterKill.CurrentAttribute] + additionalValue);
        }
    }

    /// <summary>
    /// Resets the appearance cache.
    /// </summary>
    public void OnAppearanceChanged() => this._appearanceData.RaiseAppearanceChanged();

    /// <summary>
    /// Moves the player to the specified gate.
    /// </summary>
    /// <param name="gate">The gate to which the player should be moved.</param>
    public async ValueTask WarpToAsync(ExitGate gate)
    {
        var isRespawnOnSameMap = object.Equals(this.CurrentMap?.Definition, gate.Map);
        if (!await this.TryRemoveFromCurrentMapAsync(isRespawnOnSameMap).ConfigureAwait(false))
        {
            return;
        }

        await this.PlaceAtGateAsync(gate).ConfigureAwait(false);
        this.CurrentMap = null; // Will be set again, when the client acknowledged the map change by F3 12 packet.

        if (!this.PlayerState.CurrentState.IsDisconnectedOrFinished())
        {
            await this.PlayerState.TryAdvanceToAsync(GameLogic.PlayerState.ChangingMap).ConfigureAwait(false);
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
    public virtual async ValueTask RespawnAtAsync(ExitGate gate)
    {
        var isRespawnOnSameMap = object.Equals(this.CurrentMap?.Definition, gate.Map);

        if (!await this.TryRemoveFromCurrentMapAsync(isRespawnOnSameMap).ConfigureAwait(false))
        {
            return;
        }

        this.ThrowNotInitializedProperty(this.SelectedCharacter is null, nameof(this.SelectedCharacter));
        this.SelectedCharacter.ThrowNotInitializedProperty(this.SelectedCharacter.CurrentMap is null, nameof(this.SelectedCharacter.CurrentMap));
        await this.PlaceAtGateAsync(gate).ConfigureAwait(false);
        this._respawnAfterDeathCts?.Dispose();
        this._respawnAfterDeathCts = null;

        if (this.ViewPlugIns.GetPlugIn<IRespawnAfterDeathPlugIn>() is { } respawnPlugIn)
        {
            // Older clients use a separate packet for the respawn, while newer don't.
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
            await this.PlayerState.TryAdvanceToAsync(GameLogic.PlayerState.ChangingMap).ConfigureAwait(false);
            await this.InvokeViewPlugInAsync<IMapChangePlugIn>(p => p.MapChangeAsync()).ConfigureAwait(false);

            // after this, the Client will send us a F3 12 packet, to tell us it loaded
            // the map and is ready to receive the new meet player/monster etc.
            // Then ClientReadyAfterMapChange is called.
        }
    }

    /// <summary>
    /// Signals that the game client of the player is ready after a map change (data has been loaded etc.).
    /// In this event, the player enters the game map on the server side and interacts with the other objects.
    /// </summary>
    /// <remarks>
    /// This method is called after the client sent us the F3 12 packet, or after
    /// the player entered the game.
    /// </remarks>
    public async ValueTask ClientReadyAfterMapChangeAsync()
    {
        this.ThrowNotInitializedProperty(this.SelectedCharacter is null, nameof(this.SelectedCharacter));
        this.SelectedCharacter.ThrowNotInitializedProperty(this.SelectedCharacter.CurrentMap is null, nameof(this.SelectedCharacter.CurrentMap));

        if (this.CurrentMap is not null)
        {
            // Guard against a repeated F3 12 (client ready after map change) packet.
            // A map change usually leaves CurrentMap null until this handler assigns it,
            // so a non-null value means the handler already ran. The exception is the
            // IRespawnAfterDeathPlugIn branch of RespawnAtAsync, which assigns CurrentMap
            // and adds the player itself; a trailing packet is redundant there as well.
            // Without this guard, a duplicate packet adds the player (and its summon) to
            // the area of interest a second time, which the bucket does not deduplicate.
            this.Logger.LogWarning("Ignoring client-ready packet: player {0} is already on map {1}.", this, this.CurrentMap);
            return;
        }

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

        await this._summon.AddToMapAsync(this.CurrentMap).ConfigureAwait(false);
    }

    /// <summary>
    /// Adds experience points after killing the target object.
    /// </summary>
    /// <param name="killedObject">The killed object.</param>
    /// <returns>The gained experience.</returns>
    public ValueTask<int> AddExpAfterKillAsync(IAttackable killedObject) => this._experience.AddAfterKillAsync(killedObject);

    /// <summary>
    /// Calculates the amount of experience gained after a kill, without applying it to the character.
    /// </summary>
    /// <param name="killedObject">The killed monster.</param>
    /// <returns>The calculated experience amount.</returns>
    public ValueTask<int> CalculateExpAfterKillAsync(IAttackable killedObject) => this._experience.CalculateAfterKillAsync(killedObject);

    /// <summary>
    /// Adds the master experience to the current character.
    /// </summary>
    /// <param name="experience">The experience that should be added.</param>
    /// <param name="killedObject">The killed object that caused the experience gain.</param>
    public ValueTask AddMasterExperienceAsync(int experience, IAttackable? killedObject) => this._experience.AddMasterExperienceAsync(experience, killedObject);

    /// <summary>
    /// Adds the experience to the current character.
    /// </summary>
    /// <param name="experience">The experience that should be added.</param>
    /// <param name="killedObject">The killed object that caused the experience gain.</param>
    public ValueTask AddExperienceAsync(int experience, IAttackable? killedObject) => this._experience.AddExperienceAsync(experience, killedObject);

    /// <summary>
    /// Moves the player to the specified coordinate.
    /// </summary>
    /// <param name="target">The target.</param>
    public ValueTask MoveAsync(Point target) => this._movement.MoveAsync(target);

    /// <summary>
    /// Walks to the specified target coordinates using the specified steps.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="steps">The steps.</param>
    public ValueTask WalkToAsync(Point target, Memory<WalkingStep> steps) => this._movement.WalkToAsync(target, steps);

    /// <inheritdoc />
    public ValueTask<int> GetDirectionsAsync(Memory<Direction> directions) => this._movement.GetDirectionsAsync(directions);

    /// <inheritdoc />
    public ValueTask<int> GetStepsAsync(Memory<WalkingStep> steps) => this._movement.GetStepsAsync(steps);

    /// <inheritdoc />
    public ValueTask StopWalkingAsync() => this._movement.StopWalkingAsync();

    /// <summary>
    /// Regenerates the attributes specified in <see cref="Stats.IntervalRegenerationAttributes"/>.
    /// </summary>
    /// <remarks>
    /// This method is called in the interval of <see cref="GameConfiguration.RecoveryInterval"/>, but each
    /// regeneration has its own <see cref="Stats.Regeneration.Interval"/> in which its full value is applied.
    /// The applied value is scaled by the time which passed since the previous regeneration, so that the
    /// effective recovery rates stay the same, regardless of the configured recovery interval.
    /// </remarks>
    public async Task RegenerateAsync()
    {
        try
        {
            if (this.Attributes is not { } attributes)
            {
                return;
            }

            var now = DateTime.UtcNow;
            foreach (var r in Stats.IntervalRegenerationAttributes)
            {
                if (!this._lastRegenerations.TryGetValue(r, out var lastRegeneration))
                {
                    lastRegeneration = this._lastRegenerate;
                }

                // We always keep track of the time, so that a paused regeneration doesn't accumulate a value.
                this._lastRegenerations[r] = now;

                if (attributes[r.RegenerationMultiplier] <= 0 && attributes[r.AbsoluteAttribute] <= 0)
                {
                    continue;
                }

                if (r.EnabledAttribute is { } enabledAttribute && attributes[enabledAttribute] < 1)
                {
                    // For example, the shield recovery is only active at the safe-zone, except the character has
                    // a specific attribute which has the effect that it's recovered everywhere. This attribute is
                    // usually provided by a level 380 armor with a Guardian Option.
                    continue;
                }

                if (attributes[r.CurrentAttribute] >= attributes[r.MaximumAttribute])
                {
                    continue;
                }

                var elapsedIntervals = (float)((now - lastRegeneration) / r.Interval);
                if (elapsedIntervals <= 0)
                {
                    continue;
                }

                attributes[r.CurrentAttribute] = Math.Min(
                    attributes[r.CurrentAttribute] +
                        (((attributes[r.MaximumAttribute] * attributes[r.RegenerationMultiplier]) + attributes[r.AbsoluteAttribute]) * elapsedIntervals),
                    attributes[r.MaximumAttribute]);
            }

            await this.RegenerateHeroStateAsync().ConfigureAwait(false);
        }
        catch (InvalidOperationException)
        {
            // May happen after a character disconnected in the meantime.
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
        await this.CloseTradeIfNeededAsync().ConfigureAwait(false);
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

        using var l = await this.ObserverLock.WriterLockAsync();
        this.Observers.Add(observer);
        if (this.Party is not null
            && observer is Player observingPlayer
            && observingPlayer.Party == this.Party
            && observingPlayer.Attributes is { } attributes)
        {
            attributes[Stats.NearbyPartyMemberCount]++;
        }
    }

    /// <inheritdoc/>
    public async ValueTask RemoveObserverAsync(IWorldObserver observer)
    {
        using var l = await this.ObserverLock.WriterLockAsync();
        this.Observers.Remove(observer);
        if (this.Party is not null
            && observer is Player observingPlayer
            && observingPlayer.Party == this.Party
            && observingPlayer.Attributes is { } attributes)
        {
            attributes[Stats.NearbyPartyMemberCount]--;
        }
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
    /// <param name="skillEntry">The skill entry of the skill which should get performed.</param>
    /// <returns>
    ///     <c>True</c>, if the <see cref="Skill.ConsumeRequirements"/> and <see cref="Skill.Requirements"/>
    ///     are being met, and the <see cref="Skill.ConsumeRequirements"/> have been consumed; Otherwise, <c>false</c>.
    /// </returns>
    public async ValueTask<bool> TryConsumeForSkillAsync(SkillEntry skillEntry)
    {
        if (skillEntry.Skill is not { } skill)
        {
            return false;
        }

        if (skill.Requirements.Any(r => r.MinimumValue > this.Attributes![r.Attribute]))
        {
            return false;
        }

        if (skill.ConsumeRequirements.Any(r => this.GetRequiredValue(r, skillEntry) > this.Attributes![r.Attribute]))
        {
            return false;
        }

        foreach (var requirement in skill.ConsumeRequirements)
        {
            this.Attributes![requirement.Attribute] -= this.GetRequiredValue(requirement, skillEntry);
        }

        return true;
    }

    /// <summary>
    /// Creates a summoned monster for the player.
    /// </summary>
    /// <param name="definition">The definition.</param>
    /// <exception cref="InvalidOperationException">Can't add the player summon for a player which isn't spawned yet.</exception>
    public ValueTask CreateSummonedMonsterAsync(MonsterDefinition definition) => this._summon.CreateAsync(definition);

    /// <summary>
    /// Notifies the player object that the summoned monster died.
    /// </summary>
    public void SummonDied() => this._summon.OnDied();

    /// <summary>
    /// Removes the player summon.
    /// </summary>
    public ValueTask RemoveSummonAsync() => this._summon.RemoveAsync();

    /// <summary>
    /// Removes the pet command manager.
    /// </summary>
    public void RemovePetCommandManager()
    {
        this._petCommandManager?.Dispose();
        this._petCommandManager = null;
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
        if (this.Party is { } party)
        {
            await party.LeaveTemporarilyAsync(this).ConfigureAwait(false);
        }

        await this.HandleMoveToNextSafezoneAsync().ConfigureAwait(false);

        await this.RemoveFromCurrentMapAsync().ConfigureAwait(false);

        await this._storages.RestoreTemporaryStorageItemsAsync().ConfigureAwait(false);

        this.OpenedNpc = null;

        await this.SetSelectedCharacterAsync(null).ConfigureAwait(false);
        await this.MagicEffectList.ClearAllEffectsAsync().ConfigureAwait(false);

        try
        {
            await this.SaveProgressAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Couldn't save when leaving the game. Player: {player}", this);
        }
    }

    /// <summary>
    /// Saves the progress of the player.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Success of the save operation.</returns>
    public ValueTask<bool> SaveProgressAsync(CancellationToken cancellationToken = default)
        => this._persistence.SaveProgressAsync(cancellationToken);

    /// <summary>
    /// Runs the given operation while holding this player's persistence lock, so that context
    /// mutations and progress saves for the player never run concurrently.
    /// See <see cref="PlayerPersistence"/> for the rationale and the lock ordering invariant.
    /// </summary>
    /// <typeparam name="T">The result type of the operation.</typeparam>
    /// <param name="operation">The operation to run exclusively.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    public ValueTask<T> RunPersistenceExclusiveAsync<T>(Func<ValueTask<T>> operation, CancellationToken cancellationToken = default)
        => this._persistence.RunExclusiveAsync(operation, cancellationToken);

    /// <summary>
    /// Runs the given operation while holding this player's persistence lock.
    /// See <see cref="PlayerPersistence"/> for the rationale.
    /// </summary>
    /// <param name="operation">The operation to run exclusively.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A value task which completes when the operation completed.</returns>
    public ValueTask RunPersistenceExclusiveAsync(Func<ValueTask> operation, CancellationToken cancellationToken = default)
        => this._persistence.RunExclusiveAsync(operation, cancellationToken);

    /// <summary>
    /// Is called after the player killed a <see cref="Player"/>.
    /// Increment PK Level.
    /// </summary>
    /// <param name="killedPlayer">The player killed.</param>
    internal async ValueTask AfterKilledPlayerAsync(Player killedPlayer)
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
            // Self-defense is allowed.
            return;
        }

        // Killing a rival guild member (hostility) is allowed without PK penalty.
        if (this.GuildStatus is { } killerStatus
            && killedPlayer.GuildStatus is { } killedStatus
            && this.GameContext is IGameServerContext serverContext
            && serverContext.AreGuildsRival(killerStatus.GuildId, killedStatus.GuildId))
        {
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

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        await this._muHelperLazy.DisposeIfCreatedAsync().ConfigureAwait(false);

        this._petCommandManager?.Dispose();
        this._petCommandManager = null;
        this.LastAttackedTarget.SetTarget(null);

        this.PersistenceContext.Dispose();
        await this.RemoveFromCurrentMapAsync().ConfigureAwait(false);
        await this._observerToWorldViewAdapter.ClearObservingObjectsListAsync().ConfigureAwait(false);
        this._observerToWorldViewAdapter.Dispose();
        this._movement.Dispose();
        await this.MagicEffectList.DisposeAsync().ConfigureAwait(false);
        this._respawnAfterDeathCts?.Dispose();
        (this._viewPlugIns as IDisposable)?.Dispose();

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
    /// Creates the view plugin container.
    /// </summary>
    /// <returns>The created view plugin container.</returns>
    protected virtual ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer()
    {
        throw new NotImplementedException("CreateViewPlugInContainer must be overwritten in derived classes.");
    }

    /// <summary>
    /// Handles the move to next safezone logic after death or disconnect.
    /// </summary>
    private async ValueTask HandleMoveToNextSafezoneAsync()
    {
        bool moveToNextSafezone = false;

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
    }

    private async ValueTask<bool> TryRemoveFromCurrentMapAsync(bool willRespawnOnSameMap)
    {
        var currentMap = this.CurrentMap;
        if (currentMap is null)
        {
            return true;
        }

        if (willRespawnOnSameMap)
        {
            await currentMap.InitRespawnAsync(this).ConfigureAwait(false);
        }
        else
        {
            await currentMap.RemoveAsync(this).ConfigureAwait(false);
        }

        this.IsAlive = false;
        this.IsTeleporting = false;
        await this._movement.StopWalkingAsync().ConfigureAwait(false);
        await this._observerToWorldViewAdapter.ClearObservingObjectsListAsync().ConfigureAwait(false);
        await this._summon.RemoveFromMapAsync(currentMap).ConfigureAwait(false);

        return true;
    }

    private async ValueTask PlaceAtGateAsync(ExitGate gate)
    {
        this.SelectedCharacter!.PositionX = (byte)Rand.NextInt(gate.X1, gate.X2);
        this.SelectedCharacter.PositionY = (byte)Rand.NextInt(gate.Y1, gate.Y2);
        this.SelectedCharacter.CurrentMap = gate.Map;
        this.Rotation = gate.Direction;

        if (this.GameContext.PlugInManager.GetPlugInPoint<ISpeedHackCheatCheckPlugIn>() is { } speedCheck)
        {
            await speedCheck.ResetMovementStateAsync(this).ConfigureAwait(false);
        }

        this._summon.PlaceAtGate(gate);
    }

    private async ValueTask RemoveFromCurrentMapAsync()
    {
        if (this._currentMap is { } map)
        {
            await map.RemoveAsync(this).ConfigureAwait(false);
            this._currentMap = null;
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
                // Change the status.
                if (currentCharacter.State > HeroState.Normal)
                {
                    currentCharacter.State--;
                }
                else if (currentCharacter.State < HeroState.Normal)
                {
                    currentCharacter.State++;
                }
                else
                {
                    // State is already Normal, no change needed.
                }

                await this.ForEachWorldObserverAsync<IUpdateCharacterHeroStatePlugIn>(p => p.UpdateCharacterHeroStateAsync(this), true).ConfigureAwait(false);
                currentCharacter.StateRemainingSeconds = currentCharacter.State == HeroState.Normal
                    ? 0
                    : (int)TimeSpan.FromHours(1).TotalSeconds;
            }
        }
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

    private async ValueTask HitAsync(HitInfo hitInfo, IAttacker attacker, Skill? skill, bool? isFinalStreakHit = null)
    {
        this._summon.RegisterHit(attacker);
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

        if (isFinalStreakHit.HasValue)
        {
            hitInfo.Attributes |= DamageAttributes.RageFighterStreakHit;

            if (isFinalStreakHit.Value || this.Attributes[Stats.CurrentHealth] < 1)
            {
                hitInfo.Attributes |= DamageAttributes.RageFighterStreakFinalHit;
            }
        }

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

        if (attacker is IAttackable or AttackerSurrogate)
        {
            var attackableAttacker = attacker is AttackerSurrogate surrogate ? surrogate.Owner : (IAttackable)attacker;

            var reflectPercentage = this.Attributes[Stats.DamageReflection];
            if (reflectPercentage > 0)
            {
                var reflectedDamage = (hitInfo.HealthDamage + hitInfo.ShieldDamage) * reflectPercentage;
                ReflectDamage((int)reflectedDamage, attackableAttacker);
            }

            if (attacker is not AttackerSurrogate)
            {
                // Raven does not cause full reflect.
                var fullReflectPercentage = this.Attributes[Stats.FullyReflectDamageAfterHitChance];
                if (fullReflectPercentage > 0 && Rand.NextRandomBool(fullReflectPercentage))
                {
                    var reflectedDamage = attackableAttacker is Player
                        ? hitInfo.HealthDamage + hitInfo.ShieldDamage
                        : attackableAttacker.Attributes[Stats.MaximumPhysBaseDmg];
                    ReflectDamage((int)reflectedDamage, attackableAttacker);
                }
            }
        }

        void ReflectDamage(int reflectedDamage, IAttackable attackable)
        {
            if (reflectedDamage <= 0)
            {
                return;
            }

            _ = Task.Run(async () =>
            {
                await Task.Delay(500).ConfigureAwait(false);
                if (attackable.IsAlive)
                {
                    await attackable.ReflectDamageAsync(this, (uint)reflectedDamage).ConfigureAwait(false);
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

        await this._movement.StopWalkingAsync().ConfigureAwait(false);
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

                if (cancellationToken.IsCancellationRequested || this.CurrentMap is null)
                {
                    return;
                }

                await this._summon.RemoveAsync().ConfigureAwait(false);

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
            await plugInPoint.AttackableGotKilledAsync(this, killer).ConfigureAwait(false);
        }

        if (this.LastDeath is { } deathInformation)
        {
            this.Died?.Invoke(this, deathInformation);
        }
    }

    /// <summary>
    /// Called when this player is in a duel and was killed.
    /// Sets the player back to its starting position and reclaims the attributes
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

    private SkillComboDefinition? DetermineComboDefinition()
    {
        var characterClass = this.SelectedCharacter!.CharacterClass;

        while (characterClass is { })
        {
            if (characterClass.ComboDefinition is { } comboDefinition)
            {
                return comboDefinition;
            }

            // Check previous class.
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
                if (powerUpDefinition.TargetAttribute is null)
                {
                    continue;
                }

                var powerUps = PowerUpWrapper.CreateByPowerUpDefinition(powerUpDefinition, attributes);
                powerUps.ForEach(p =>
                {
                    this.PlayerLeftMap += OnPlayerLeftMap;

                    void OnPlayerLeftMap(object? o, (Player, GameMap) args)
                    {
                        this.PlayerLeftMap -= OnPlayerLeftMap;
                        p.Dispose();
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
    /// Adds the missing stat attributes, e.g., after the character class has been changed outside the game.
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
        if (this.SelectedCharacter is not { } selectedCharacter)
        {
            throw new InvalidOperationException($"The player has no selected character.");
        }

        if (selectedCharacter.CharacterClass is null)
        {
            throw new InvalidOperationException($"The character '{selectedCharacter}' has no assigned character class.");
        }

        // For characters which got created on the database or with the admin panel,
        // it's possible that they're missing the inventory. In this case, we create it here
        // and initialize with default items.
        if (selectedCharacter!.Inventory is null)
        {
            selectedCharacter.Inventory = this.PersistenceContext.CreateNew<ItemStorage>();
            this.GameContext.PlugInManager.GetPlugInPoint<ICharacterCreatedPlugIn>()?.CharacterCreated(this, selectedCharacter);
        }

        selectedCharacter.CurrentMap ??= selectedCharacter.CharacterClass?.HomeMap;
        this.AddMissingStatAttributes();

        this.Attributes = new ItemAwareAttributeSystem(this.Account!, selectedCharacter, this.GameContext.Configuration);
        this.Attributes[Stats.NearbyPartyMemberCount] = 0;
        this.LogInvalidInventoryItems();

        this._storages.CreateForCharacter(selectedCharacter);
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

        this.Attributes.AttributeValueChanged += this.OnAttributeValueChanged;
        this.Attributes.GetOrCreateAttribute(Stats.TransformationSkin).ValueChanged += this.OnTransformationSkinChanged;

        var ammoAttribute = this.Attributes.GetOrCreateAttribute(Stats.AmmunitionAmount);
        this.Attributes[Stats.AmmunitionAmount] = (float)(this.Inventory?.EquippedAmmunitionItem?.Durability ?? 0);
        ammoAttribute.ValueChanged += this.OnAmmunitionAmountChanged;

        await this.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);

        await this.InvokeViewPlugInAsync<IUpdateRotationPlugIn>(p => p.UpdateRotationAsync()).ConfigureAwait(false);
        await this.ResetPetBehaviorAsync().ConfigureAwait(false);

        if (selectedCharacter.MuHelperConfiguration is { } muHelperConfiguration)
        {
            await this.InvokeViewPlugInAsync<IMuHelperConfigurationUpdatePlugIn>(p => p.UpdateMuHelperConfigurationAsync(muHelperConfiguration)).ConfigureAwait(false);
        }

        // Add GM mark (mu logo above character's head).
        if (selectedCharacter.CharacterStatus == CharacterStatus.GameMaster)
        {
            await this.MagicEffectList.AddEffectAsync(new MagicEffect(
            TimeSpan.FromMilliseconds((double)int.MaxValue),
            GMEffect)).ConfigureAwait(false);
        }

        // Restore previously opened Store.
        var openStoreAction = new PlayerActions.PlayerStore.OpenStoreAction();
        await openStoreAction.RestoreAfterEnterWorldAsync(this, this.IsPlayerStoreOpeningAfterEnterSupported).ConfigureAwait(false);
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
    /// Current shield and mana are set to their maximum values.
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

    /// <summary>
    /// Sets the current values of the regeneration attributes to their maximum values.
    /// </summary>
    internal void SetReclaimableAttributesToMaximum()
    {
        foreach (var regeneration in Stats.IntervalRegenerationAttributes)
        {
            this.Attributes![regeneration.CurrentAttribute] = this.Attributes[regeneration.MaximumAttribute];
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    private async void OnAttributeValueChanged(object? sender, IAttribute attribute)
    {
        try
        {
            _ = LimitCurrentAttribute(Stats.MaximumHealth, Stats.CurrentHealth)
                || LimitCurrentAttribute(Stats.MaximumMana, Stats.CurrentMana)
                || LimitCurrentAttribute(Stats.MaximumShield, Stats.CurrentShield)
                || LimitCurrentAttribute(Stats.MaximumAbility, Stats.CurrentAbility);

            await this.InvokeViewPlugInAsync<IUpdateStatsPlugIn>(p => p.UpdateStatsAsync(attribute.Definition, attribute.Value)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, $"{nameof(this.OnAttributeValueChanged)} failed for attribute {attribute.Definition}.");
        }

        bool LimitCurrentAttribute(AttributeDefinition maximumDefinition, AttributeDefinition currentDefinition)
        {
            if (attribute.Definition == maximumDefinition && attribute.Value < this.Attributes![currentDefinition])
            {
                this.Attributes![currentDefinition] = attribute.Value;
                return true;
            }

            return false;
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

    private async ValueTask DecreaseItemDurabilityAfterHitAsync(HitInfo hitInfo, SkillEntry? skill)
    {
        var randomDefensiveItem = this.Inventory?.EquippedItems.Where(ItemExtensions.IsDefensiveItem).SelectRandom();
        if (randomDefensiveItem is { })
        {
            await this.DecreaseDefenseItemDurabilityAsync(randomDefensiveItem, hitInfo).ConfigureAwait(false);
        }

        if (Rand.NextRandomBool(skill?.Attributes?[Stats.RagefulBlowMasteryDurabilityDecChance] ?? 0))
        {
            var randomArmorItem = this.Inventory?.EquippedItems.Where(ItemExtensions.IsArmorItem).SelectRandom();
            if (randomArmorItem is { })
            {
                if (randomArmorItem.DecreaseDurability(randomArmorItem.GetMaximumDurabilityOfOnePiece() * this.Attributes![Stats.DurabilityReductionFactor]))
                {
                    await this.InvokeViewPlugInAsync<IItemDurabilityChangedPlugIn>(p => p.ItemDurabilityChangedAsync(randomArmorItem, false)).ConfigureAwait(false);
                }
            }
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

    private async ValueTask CloseTradeIfNeededAsync()
    {
        if (this.PlayerState.CurrentState == GameLogic.PlayerState.TradeButtonPressed
            || this.PlayerState.CurrentState == GameLogic.PlayerState.TradeOpened)
        {
            var cancelAction = new TradeCancelAction();
            await cancelAction.CancelTradeAsync(this).ConfigureAwait(false);
        }
    }
}
