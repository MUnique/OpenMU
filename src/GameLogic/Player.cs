// <copyright file="Player.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.ExceptionServices;
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
    private const int IceSkillNumber = 7;

    private static readonly MagicEffectDefinition GMEffect = new GameMasterMagicEffectDefinition
    {
        InformObservers = true,
        Name = "GM MARK",
        Number = 28,
        StopByDeath = false,
    };

    /// <summary>
    /// How long an outlaw (player killer) state lasts until it falls back one step. Each player kill
    /// (re)starts it, and kills which can't escalate the state any further stack on top of it.
    /// It can be shortened by killing monsters.
    /// </summary>
    private static readonly TimeSpan PlayerKillerStateDuration = TimeSpan.FromHours(3);

    /// <summary>
    /// The duration until a hero state falls back one step.
    /// </summary>
    private static readonly TimeSpan HeroStateDuration = TimeSpan.FromHours(1);

    // Only synchronous hit accounting takes this target-local lock. Attribute observers snapshot
    // notifications here; views, plug-ins, death and cross-target effects run after release.
    // Map/life transitions may take PlayerState's exclusion before this lock; hit commits never
    // acquire the state exclusion while holding this lock, and never await external callbacks here.
    private readonly object _hitLock = new();

    private readonly MoveItemAction _moveAction = new();

    private readonly PlayerExperience _experience;

    /// <summary>
    /// Serializes context mutations done by this player's action handlers against the periodic and
    /// disconnect progress saves, which run on an independent timer flow. See
    /// <see cref="RunPersistenceExclusiveAsync{T}"/>.
    /// </summary>
    private readonly PlayerPersistence _persistence;

    private readonly PlayerMovement _movement;

    private readonly PlayerSummon _summon;

    private readonly PlayerMapTransitions _mapTransitions;

    private readonly PlayerStorages _storages;

    private readonly PlayerAppearanceData _appearanceData;

    private readonly ObserverToWorldViewAdapter _observerToWorldViewAdapter;

    private readonly Lazy<MuHelper.MuHelper> _muHelperLazy;

    private readonly MapPowerUps _mapPowerUps = new();

    private readonly AccountAttributeGrantPowerUps _accountAttributeGrantPowerUps = new();

    /// <summary>
    /// The attribute changes which the thread holding <see cref="_hitLock"/> makes, see <see cref="CollectAttributeChangesUnderHitLock"/>.
    /// </summary>
    private List<(AttributeDefinition Definition, float Value)>? _hitAttributeChanges;

    private CancellationTokenSource? _respawnAfterDeathCts;

    // One elapsed respawn, bound to the original death timer; state exclusion owns claim/defer.
    private (CancellationTokenSource Source, DeathInformation Death, bool Claimed)? _pendingDeathRespawn;

    // An already-owned lethal hit can finish after a warp, without waiting for another packet here.
    // Publication and consumption use PlayerState's exclusion; the death identity excludes later lives.
    private (IAttacker? Killer, DeathInformation Death)? _pendingMapChangeDeath;

    private bool _hasReservedLethalHit;

    // State exclusion owns preparation; an old life cannot release a replacement's admission owner.
    private ItemAwareAttributeSystem? _forcedPlacementPreparation;

    private Character? _selectedCharacter;

    private ICustomPlugInContainer<IViewPlugIn>? _viewPlugIns;

    private DateTime _lastRegenerate = DateTime.UtcNow;

    /// <summary>
    /// The fraction of a second which elapsed since the last regeneration, but wasn't subtracted from
    /// <see cref="Character.StateRemainingSeconds"/> yet, because it only counts in whole seconds.
    /// </summary>
    private double _heroStateSecondsRemainder;

    private GameMap? _currentMap;

    private IDisposable? _accountLoggingScope;

    private Account? _account;

    /// <summary>
    /// The login name with which this player is registered at the login server, until it's released.
    /// </summary>
    private string? _loginServerRegistration;

    /// <summary>
    /// The UTC ticks of the last time the client reported that it's alive, or 0, if it didn't yet.
    /// </summary>
    private long _lastAliveReportTicks;

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
        this._mapTransitions = new PlayerMapTransitions(this, this._movement, this._summon);

        this.MagicEffectList = new MagicEffectsList(this);
        this._appearanceData = new PlayerAppearanceData(this);
        this.PlayerState.StateChanged += async args =>
        {
            if (args.CurrentStateState.IsDisconnectedOrFinished()
                || args.CurrentStateState == GameLogic.PlayerState.Authenticated
                || args.CurrentStateState == GameLogic.PlayerState.CharacterSelection)
            {
                this._pendingMapChangeDeath = null;
                this._pendingDeathRespawn = null;
                lock (this._hitLock)
                {
                    this._hasReservedLethalHit = false;
                }
            }

            // The game context counts its players first, so that the plugins already see the new numbers.
            this.GameContext.OnPlayerStateChanged(this, args.PreviousState, args.CurrentStateState);
            await (this.GameContext.PlugInManager.GetPlugInPoint<IPlayerStateChangedPlugIn>()?.PlayerStateChangedAsync(this, args.PreviousState, args.CurrentStateState) ?? ValueTask.CompletedTask).ConfigureAwait(false);
        };
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
    /// Occurs when the player has been logged in, so that its <see cref="Account"/> is known.
    /// </summary>
    public event AsyncEventHandler<Player>? PlayerLoggedIn;

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
    public SkillHitValidator SkillHitValidator => this._skillHitValidator ??= new SkillHitValidator(
        this.Logger,
        this.GameContext.PlugInManager,
        configuration: this.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()?.Configuration);

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

    /// <summary>
    /// Gets the point in time (UTC) when the client of this player reported the last time that it's alive,
    /// e.g. by its periodic ping, while the player is in the world. It's <c>null</c>, if the client didn't report
    /// it since the player entered the world, e.g. because it doesn't support it, or because this player has no
    /// client at all.
    /// </summary>
    public DateTime? LastAliveReport => Interlocked.Read(ref this._lastAliveReportTicks) is var ticks and > 0
        ? new DateTime(ticks, DateTimeKind.Utc)
        : null;

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

        internal set
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

    /// <summary>
    /// Gets or sets the gens membership of the selected character.
    /// It's <c>null</c>, if the character never joined a gens.
    /// </summary>
    public GensMember? GensMember { get; set; }

    /// <inheritdoc/>
    public Direction Rotation { get; set; }

    /// <inheritdoc/>
    public Party? Party { get; set; }

    /// <inheritdoc/>
    public bool IsConnected => !this.PlayerState.CurrentState.IsDisconnectedOrFinished();

    /// <inheritdoc/>
    public bool IsAlive { get; set; }

    /// <inheritdoc/>
    public bool IsTeleporting { get; internal set; }

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
    /// Gets or sets a value indicating whether the cash shop is opened by the player.
    /// </summary>
    public bool IsCashShopOpen { get; set; }

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
    /// Gets or sets the timestamp of when the shield hiatus was last accrued.
    /// </summary>
    public DateTime LastShieldRecoveryHiatusAccrual { get; set; } = DateTime.UtcNow;

    /// <summary>Gets a value indicating whether this life's forced placement is closing its interactions.</summary>
    internal bool IsPreparingForcedPlacement => this.Attributes is { } attributes
        && ReferenceEquals(Volatile.Read(ref this._forcedPlacementPreparation), attributes);

    /// <summary>Gets a value indicating whether a live character can publish trade state.</summary>
    internal bool IsAvailableForTrade => !this.IsPreparingForcedPlacement && this.SelectedCharacter is not null && this.IsAlive;

    /// <summary>
    /// Gets a value indicating whether this thread is committing this player's hit resources.
    /// </summary>
    internal bool IsApplyingHitOnCurrentThread => Monitor.IsEntered(this._hitLock);

    /// <summary>
    /// Gets a value indicating whether opening the player store after entering the game is supported by this instance.
    /// </summary>
    protected virtual bool IsPlayerStoreOpeningAfterEnterSupported => true;

    /// <summary>
    /// Sets the account of the player after a successful login and notifies the subscribers
    /// of <see cref="PlayerLoggedIn"/>.
    /// </summary>
    /// <param name="account">The account of the player.</param>
    /// <returns>The async task.</returns>
    public async ValueTask SetAccountAsync(Account account)
    {
        this.Account = account;
        await this.PlayerLoggedIn.SafeInvokeAsync(this).ConfigureAwait(false);
    }

    /// <summary>
    /// Remembers that the client of this player reported that it's alive, if the player is in the world.
    /// </summary>
    /// <remarks>
    /// Outside of the world, it's not remembered, because not every client reports it there. E.g. the web client
    /// only sends its ping while a character is in the world - a player which waits at the character selection
    /// would otherwise be considered as lost.
    /// </remarks>
    public void ReportAlive()
    {
        if (this.SelectedCharacter is null)
        {
            return;
        }

        Interlocked.Exchange(ref this._lastAliveReportTicks, DateTime.UtcNow.Ticks);
    }

    /// <summary>
    /// Remembers that the login server accepted the login of this player with the specified login name.
    /// </summary>
    /// <remarks>
    /// It's remembered right when the login server accepted it, before the account is assigned, so that
    /// the registration is released at the login server even when the player disconnects in between.
    /// Otherwise, the account would stay registered as connected, and every further login would be rejected.
    /// </remarks>
    /// <param name="loginName">The login name.</param>
    public void SetLoginServerRegistration(string loginName)
    {
        Volatile.Write(ref this._loginServerRegistration, loginName);
    }

    /// <summary>
    /// Releases the registration of this player at the login server. The returned login name has to be logged off there.
    /// </summary>
    /// <returns>The login name with which this player was registered at the login server, or <c>null</c>, if it isn't registered (anymore).
    /// Each registration is returned only once, so it's logged off only once.</returns>
    public string? ReleaseLoginServerRegistration()
    {
        return Interlocked.Exchange(ref this._loginServerRegistration, null);
    }

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

            this.ReleaseSkillList();

            if (this.DuelRoom is { State: DuelState.DuelStarted } duelRoom)
            {
                await duelRoom.CancelDuelAsync().ConfigureAwait(false);
                if (this.GameContext.Configuration.DuelConfiguration?.Exit is { } exit)
                {
                    await this._mapTransitions.PlaceAtGateAsync(exit).ConfigureAwait(false);
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

        DeathInformation ownedDeath;
        List<(AttributeDefinition Definition, float Value)> changes = [];
        lock (this._hitLock)
        {
            if (this._hasReservedLethalHit || this.IsDisposed || this.IsDisposing
                || this.SelectedCharacter is null || this.PlayerState.CurrentState.IsDisconnectedOrFinished()
                || this.PlayerState.CurrentState == GameLogic.PlayerState.Dead)
            {
                return;
            }

            var hitInfo = new HitInfo((uint)this.Attributes[Stats.CurrentHealth], (uint)this.Attributes[Stats.CurrentShield], DamageAttributes.Undefined);
            using (this.CollectAttributeChangesUnderHitLock(changes))
            {
                this.Attributes[Stats.CurrentHealth] = 0;
                this._hasReservedLethalHit = true;
                this.IsAlive = false;
                ownedDeath = new DeathInformation(0, string.Empty, hitInfo, 0);
                this.LastDeath = ownedDeath;
            }
        }

        try
        {
            await this.PublishAttributeChangesAsync(changes).ConfigureAwait(false);
        }
        finally
        {
            await this.OnDeathAsync(null, ownedDeath).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async ValueTask<HitInfo?> AttackByAsync(IAttacker attacker, SkillEntry? skill, bool isCombo, double damageFactor = 1.0, bool? isFinalStreakHit = null)
    {
        if (this.Attributes is null)
        {
            throw new InvalidOperationException("AttributeSystem not set.");
        }

        if (!this.IsAlive || this.IsAttackBlockedBySafezone(attacker)
            || this.IsAttackPreventedByPlugIns(attacker))
        {
            return null;
        }

        if (!this.GameContext.PvpEnabled && this.CurrentMap?.Definition.BattleZone == null &&
            this.CurrentMiniGame?.AllowPlayerKilling is false)
        {
            return null;
        }

        if (Rand.NextRandomBool(attacker.Attributes[Stats.CrystalSwordIceChance])
            && this.GameContext.Configuration.Skills.FirstOrDefault(s => s.Number == IceSkillNumber) is { } ice)
        {
            skill = new SkillEntry { Skill = ice };
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

        var recoverHealthBeforeHit = Rand.NextRandomBool(this.Attributes[Stats.FullyRecoverHealthAfterHitChance]);
        var manaFullyRecovered = Rand.NextRandomBool(this.Attributes[Stats.FullyRecoverManaAfterHitChance]);
        if (!await this.HitAsync(
                hitInfo,
                attacker,
                skill?.Skill,
                isFinalStreakHit,
                recoverHealthBeforeHit,
                applyOrdinaryHitEffects: true,
                recoverManaBeforeHit: manaFullyRecovered).ConfigureAwait(false))
        {
            return null;
        }

        bool isPvpDamage = attacker is Player or IPlayerSurrogate;
        float damage = isPvpDamage ? hitInfo.TotalDamage : attacker.Attributes[Stats.MinimumPhysBaseDmg];
        await this.DecreaseItemDurabilityAfterHitAsync(damage, isPvpDamage, skill, hitInfo.PetDamage).ConfigureAwait(false);

        if (attacker as IPlayerSurrogate is { } playerSurrogate)
        {
            await playerSurrogate.Owner.DecreaseRavenDurabilityAfterHitAsync(hitInfo.TotalDamage).ConfigureAwait(false);
        }

        if (attacker is Player attackerPlayer)
        {
            await attackerPlayer.AfterHitTargetAsync(this.Attributes[Stats.DefenseFinal], skill?.Skill?.DamageType).ConfigureAwait(false);

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
    /// <param name="targetDefense">The target's defense.</param>
    /// <param name="damageType">The damage type.</param>
    public async ValueTask AfterHitTargetAsync(float targetDefense, DamageType? damageType)
    {
        var healthLoss = this.Attributes![Stats.HealthLossAfterHit];
        if (healthLoss > 0)
        {
            await this.ApplyHealthLossAfterHitAsync(healthLoss).ConfigureAwait(false);
        }

        await this.DecreaseWeaponDurabilityAfterHitAsync(targetDefense, damageType ?? DamageType.Physical).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask ReflectDamageAsync(IAttacker reflector, uint damage)
    {
        await this.HitAsync(this.GetHitInfo(damage, DamageAttributes.Reflected, reflector), reflector, null).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask ApplyPoisonDamageAsync(IAttacker initialAttacker, uint damage)
    {
        await this.HitAsync(new HitInfo(damage, 0, DamageAttributes.Poison), initialAttacker, null).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask ApplyBleedingDamageAsync(IAttacker initialAttacker, uint damage)
    {
        await this.HitAsync(this.GetHitInfo(damage, DamageAttributes.Undefined, initialAttacker), initialAttacker, null).ConfigureAwait(false);
    }

    /// <summary>
    /// Teleports this player to the specified target with the specified skill animation.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="teleportSkill">The teleport skill.</param>
    public Task TeleportAsync(Point target, Skill teleportSkill) => this._mapTransitions.TeleportAsync(target, teleportSkill);

    /// <summary>
    /// Teleports this player to the specified target map and point.
    /// </summary>
    /// <param name="targetMap">The target map for teleportation.</param>
    /// <param name="targetPoint">The target coordinate in the target map.</param>
    public Task TeleportToMapAsync(GameMap targetMap, Point targetPoint) => this._mapTransitions.TeleportToMapAsync(targetMap, targetPoint);

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
    public ValueTask WarpToAsync(ExitGate gate) => this._mapTransitions.WarpToAsync(gate);

    /// <summary>
    /// Moves the player to the safe zone.
    /// </summary>
    public ValueTask WarpToSafezoneAsync() => this._mapTransitions.WarpToSafezoneAsync();

    /// <summary>
    /// Respawns the player to the specified gate.
    /// </summary>
    /// <param name="gate">The gate at which the player should be respawned.</param>
    public virtual ValueTask RespawnAtAsync(ExitGate gate) => this._mapTransitions.RespawnAtAsync(gate);

    /// <summary>
    /// Signals that the game client of the player is ready after a map change (data has been loaded etc.).
    /// In this event, the player enters the game map on the server side and interacts with the other objects.
    /// </summary>
    /// <remarks>
    /// This method is called after the client sent us the F3 12 packet, or after
    /// the player entered the game.
    /// </remarks>
    public ValueTask ClientReadyAfterMapChangeAsync() => this._mapTransitions.ClientReadyAfterMapChangeAsync();

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
    /// Ends a running walk early, at the position the game client reports it stopped at.
    /// </summary>
    /// <param name="stopPoint">The position the client reports it stopped the walk at.</param>
    public ValueTask StopWalkAtAsync(Point stopPoint) => this._movement.StopWalkAtAsync(stopPoint);

    /// <summary>
    /// Regenerates the attributes specified in <see cref="Stats.IntervalRegenerationAttributes"/>.
    /// </summary>
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
                if ((r.EnablerAttribute is { } enabler && attributes[enabler] < 1)
                    || (r.HiatusAttribute is { } hiatus && attributes[hiatus] < r.HiatusThreshold))
                {
                    continue;
                }

                var factor = 0f;
                var interval = r.Interval;
                if (attributes[Stats.IsResting] > 0)
                {
                    interval = r.IntervalResting;

                    if (r.CurrentAttribute == Stats.CurrentMana)
                    {
                        // Mana recovery while resting is on top of regular recovery
                        factor += (float)((now - this._lastRegenerate) / r.Interval);
                    }
                }

                factor += (float)((now - this._lastRegenerate) / interval);

                attributes[r.CurrentAttribute] = Math.Min(
                    attributes[r.CurrentAttribute] +
                        (((attributes[r.MaximumAttribute] * attributes[r.RegenerationMultiplier]) + attributes[r.AbsoluteAttribute]) * factor),
                    attributes[r.MaximumAttribute]);

                // this.Logger.LogDebug($"Regenerated {r.CurrentAttribute} with elapsed time {now - this._lastRegenerate} and factor {factor}");
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
                try
                {
                    await this.InternalDisconnectAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    // The subscribers below are what take the player out of the game context's player
                    // list - and, for a remote player, dispose it. A teardown which throws half way
                    // must not skip them: the state machine has already left the state this method
                    // needs to advance from, so no later call can raise the event again. The player
                    // would stay in the list, counting towards the server's maximum player count and
                    // keeping its whole object graph alive, until the process restarts.
                    this.Logger.LogError(ex, "Error while disconnecting player {Player}; continuing the teardown.", this);

                    // Saving is the LAST step of the regular teardown (see RemoveFromGameAsync), so
                    // the throw above may well have skipped it. Without a save, continuing would turn
                    // the failure into silent data loss: the caller sees a successful logout, disposes
                    // this instance, and the next session loads the character from the database -
                    // rolled back to its last periodic save.
                    try
                    {
                        await this.SaveProgressAsync().ConfigureAwait(false);
                    }
                    catch (Exception saveException)
                    {
                        this.Logger.LogError(saveException, "Couldn't save the progress of player {Player} after its teardown failed.", this);
                    }
                }

                if (this.PlayerDisconnected is { } disconnectedEventHandler)
                {
                    this.PlayerDisconnected = null;

                    // One by one instead of awaiting the multicast delegate: invoking a multicast
                    // ValueTask delegate awaits only the LAST subscriber's task, so the earlier ones
                    // (the removal from the game context's player list) would run unobserved and
                    // possibly still be in flight while the last one (which disposes a remote player)
                    // executes. Isolating each subscriber also keeps one failing handler from skipping
                    // the others - the handler list is already dropped, so nothing could re-run them.
                    foreach (var subscriber in disconnectedEventHandler.GetInvocationList())
                    {
                        try
                        {
                            await ((AsyncEventHandler<Player>)subscriber)(this).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            this.Logger.LogError(ex, "A PlayerDisconnected subscriber of player {Player} failed.", this);
                        }
                    }
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

        using var l = await this.ObserverLock.WriterLockAsync().ConfigureAwait(false);
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
        using var l = await this.ObserverLock.WriterLockAsync().ConfigureAwait(false);
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
    /// Determines whether the player complies with the <see cref="Skill.Requirements"/> of a skill, so that it can use it.
    /// </summary>
    /// <param name="skill">The skill.</param>
    /// <returns><c>true</c>, if the player complies with the requirements; Otherwise, <c>false</c>.</returns>
    public bool CompliesRequirements(Skill skill)
    {
        return !skill.Requirements.Any(r => r.MinimumValue > this.Attributes![r.Attribute]);
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

        if (!this.CompliesRequirements(skill))
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
    /// Decreases the dark raven durability after it has attacked.
    /// </summary>
    /// <remarks>
    /// A raven's durability only decreases after it attacks, or if it's the target of a fenrir skill effect.
    /// In other words, it's more like a weapon than a pet.
    /// </remarks>
    /// <param name="damage">The damage dealt.</param>
    public async ValueTask DecreaseRavenDurabilityAfterHitAsync(float damage)
    {
        var item = this.Inventory?.GetItem(InventoryConstants.RightHandSlot);
        if (item is null || !item.IsTrainablePet() || item.Durability == 0 || this.Attributes is not { } attributes)
        {
            return;
        }

        double hitsPerOneItemDurability = this.GameContext.Configuration.HitsPerOneItemDurability;
        hitsPerOneItemDurability *= 800f / 564; // The original values. We keep the same relative factor.
        var decrement = damage * 0.02 / (hitsPerOneItemDurability + attributes[Stats.TrainablePetDurationIncrease]);
        await this.DecreaseItemDurabilityAsync(item, decrement).ConfigureAwait(false);

        if (item.Durability == 0.0)
        {
            var minimumExp = item.Definition!.GetExperienceOfPetLevel(item.Level, item.Definition!.MaximumItemLevel);
            item.PetExperience = (int)Math.Max((int)(item.PetExperience * 0.9), minimumExp);
            await this.ResetPetBehaviorAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Decreases an item's durability and raises the <see cref="InventoryStorage.EquippedItemsChanged"/> event, if justified.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="decrement">The decrement.</param>
    public async ValueTask DecreaseItemDurabilityAsync(Item item, double decrement)
    {
        if (this.Attributes?[Stats.ItemDurabilityProtection] > 0
            && item.ItemSlot != InventoryConstants.PetSlot
            && !item.IsTrainablePet())
        {
            // E.g. the Talisman of Item Protection keeps the durability of all equipped items, except pets.
            return;
        }

        var previousFactor = item.GetCurrentDurabilityFactor();
        if (item.DecreaseDurability(decrement))
        {
            await this.InvokeViewPlugInAsync<IItemDurabilityChangedPlugIn>(p => p.ItemDurabilityChangedAsync(item, false)).ConfigureAwait(false);
        }

        if (previousFactor != item.GetCurrentDurabilityFactor())
        {
            await this._storages.Inventory!.AsInventoryStorage!.RaiseEquippedItemsChangedAsync(item, true, true).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Returns the items which are still in the temporary storage (the tray of a crafting NPC)
    /// to the inventory and tells the client about it. Without the update, the client keeps
    /// showing neither the tray nor the inventory slots until the next login.
    /// </summary>
    public async ValueTask ReturnTemporaryStorageItemsAsync()
    {
        if (await this.TryReturnTemporaryStorageItemsAsync().ConfigureAwait(false))
        {
            await this.InvokeViewPlugInAsync<IUpdateInventoryListPlugIn>(p => p.UpdateInventoryListAsync()).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Removes the player from the game and saves its state.
    /// </summary>
    public async ValueTask RemoveFromGameAsync()
    {
        // A trade can't continue without this character; the partner gets its items back as with a cancel.
        await this.CloseTradeIfNeededAsync().ConfigureAwait(false);

        if (this.Party is { } party)
        {
            await party.LeaveTemporarilyAsync(this).ConfigureAwait(false);
        }

        await this.HandleMoveToNextSafezoneAsync().ConfigureAwait(false);

        await this._mapTransitions.RemoveFromCurrentMapAsync().ConfigureAwait(false);

        // The player always observes itself, so leaving the map doesn't remove it from its own
        // observed objects. Without clearing them, entering the world again (e.g. after going back
        // to the character selection) wouldn't show the player to itself as a new player in scope.
        await this._observerToWorldViewAdapter.ClearObservingObjectsListAsync().ConfigureAwait(false);

        await this._storages.RestoreTemporaryStorageItemsAsync().ConfigureAwait(false);

        this.OpenedNpc = null;

        await this.SetSelectedCharacterAsync(null).ConfigureAwait(false);

        // The client may stop to report that it's alive when it leaves the world, see ReportAlive.
        Interlocked.Exchange(ref this._lastAliveReportTicks, 0);

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
    /// Applies a class change of the selected character, which happened while it's in the game (quest
    /// evolution), to the running attribute system. The system is built with the class of the moment of
    /// entering the game, so without this a fresh master class would have no Master Level stat and no
    /// master points per level up until the next login: the master level-ups would be lost silently.
    /// Only the stat attributes and the base values are applied; the attribute combinations of the new
    /// class follow with the next login.
    /// </summary>
    /// <param name="previousClass">The character class before the change.</param>
    public void ApplyCharacterClassChange(CharacterClass previousClass)
    {
        if (this.SelectedCharacter is not { CharacterClass: { } newClass } character
            || this.Attributes is not { } attributes
            || ReferenceEquals(previousClass, newClass))
        {
            return;
        }

        this.AddMissingStatAttributes();
        foreach (var statAttribute in character.Attributes)
        {
            attributes.TryAddStatAttribute(statAttribute);
        }

        foreach (var oldValue in previousClass.BaseAttributeValues
                     .Where(o => !newClass.BaseAttributeValues.Any(n => n.Definition == o.Definition && n.Value == o.Value))
                     .ToList())
        {
            attributes.RemoveElement(oldValue, oldValue.Definition);
        }

        foreach (var newValue in newClass.BaseAttributeValues
                     .Where(n => !previousClass.BaseAttributeValues.Any(o => o.Definition == n.Definition && o.Value == n.Value))
                     .ToList())
        {
            attributes.AddElement(newValue, newValue.Definition);
        }

        this.Logger.LogInformation(
            "Character {Name} changed class in the game from {Previous} to {New}; master level {MasterLevel}, master points per level up {Points}.",
            character.Name,
            previousClass.Name,
            newClass.Name,
            attributes[Stats.MasterLevel],
            attributes[Stats.MasterPointsPerLevelUp]);
    }

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

        if (this.IsExemptedFromPlayerKillPenalty(killedPlayer))
        {
            // e.g. a kill between the members of different gens in a battle zone.
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

            // Stepping up to the next outlaw state restarts the clock for that state. Math.Max, so that
            // a kill can never shorten an already longer remaining time.
            this._selectedCharacter.StateRemainingSeconds = Math.Max(
                this._selectedCharacter.StateRemainingSeconds,
                (int)PlayerKillerStateDuration.TotalSeconds);
        }
        else
        {
            // Further kills as a 2nd stage outlaw can't escalate the state anymore, so they stack on
            // top of the remaining time instead.
            this._selectedCharacter.StateRemainingSeconds += (int)PlayerKillerStateDuration.TotalSeconds;
        }

        this._selectedCharacter.PlayerKillCount += 1;
        await this.ForEachWorldObserverAsync<IUpdateCharacterHeroStatePlugIn>(o => o.UpdateCharacterHeroStateAsync(this), true).ConfigureAwait(false);
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

    /// <summary>
    /// Sets the current map without raising the enter/leave map events, when the player is
    /// removed from the game.
    /// </summary>
    /// <param name="map">The map.</param>
    internal void SetCurrentMapSilently(GameMap? map)
    {
        this._currentMap = map;
    }

    /// <summary>
    /// Returns the items of the temporary storage like <see cref="ReturnTemporaryStorageItemsAsync"/>, without telling the client,
    /// so that a dialog owner can move them while it holds the state and update the client afterwards.
    /// </summary>
    /// <returns><c>true</c>, if items have been moved.</returns>
    internal ValueTask<bool> TryReturnTemporaryStorageItemsAsync() => this._storages.TryReturnTemporaryStorageItemsAsync();

    /// <summary>
    /// Clears the list of the objects which are observed by this player.
    /// </summary>
    internal ValueTask ClearObservingObjectsListAsync()
    {
        return this._observerToWorldViewAdapter.ClearObservingObjectsListAsync();
    }

    /// <summary>
    /// Gets the gate at which the player is spawned, e.g. after its death.
    /// </summary>
    /// <returns>The spawn gate.</returns>
    internal ValueTask<ExitGate> GetSpawnGateOfCurrentMapAsync()
    {
        return this._mapTransitions.GetSpawnGateOfCurrentMapAsync();
    }

    /// <summary>
    /// Completes the map-arrival state without reviving an already-reserved lethal hit.
    /// </summary>
    /// <returns>Whether the arrival state transition was allowed.</returns>
    internal async ValueTask<bool> EnterWorldAfterMapChangeAsync()
    {
        await using var context = await this.PlayerState.TryBeginAdvanceToAsync(GameLogic.PlayerState.EnteredWorld).ConfigureAwait(false);
        if (!context.Allowed)
        {
            return false;
        }

        this.RestoreLivenessAfterPlacement();
        return true;
    }

    /// <summary>Reads lethal ownership together with its identity for a bounded placement.</summary>
    /// <returns>The last death and whether it is reserved for this life, read under hit exclusion.</returns>
    internal (DeathInformation? Death, bool Reserved) GetPlacementDeathSnapshot()
    {
        lock (this._hitLock)
        {
            return (this.LastDeath, this._hasReservedLethalHit);
        }
    }

    /// <summary>Closes recognized interactions and claims map change before removing the player.</summary>
    /// <param name="isCurrentLife">Validates the captured character, attributes and death before claiming placement.</param>
    /// <param name="tryPreparePlacement">Optional owned restoration commit; synchronous and invoked only inside successful state admission.</param>
    /// <returns>Whether this operation owns the existing changing-map state.</returns>
    internal async ValueTask<bool> PrepareForForcedPlacementAsync(Func<bool> isCurrentLife, Func<bool>? tryPreparePlacement = null)
    {
        var attributes = this.Attributes;
        var acquired = false;
        try
        {
            await using (var context = await this.PlayerState.TryBeginAdvanceToAsync(this.PlayerState.CurrentState).ConfigureAwait(false))
            {
                context.Allowed = false;
                if (attributes is null || !isCurrentLife() || ReferenceEquals(this._forcedPlacementPreparation, attributes))
                {
                    return false;
                }

                this._forcedPlacementPreparation = attributes;
                acquired = true;
            }

            while (true)
            {
                State state;
                NonPlayerCharacter? dialog;
                await using (var context = await this.PlayerState.TryBeginAdvanceToAsync(GameLogic.PlayerState.ChangingMap).ConfigureAwait(false))
                {
                    state = this.PlayerState.CurrentState;
                    dialog = this.OpenedNpc;
                    context.Allowed &= isCurrentLife() && state != GameLogic.PlayerState.NpcDialogOpened;
                    if (context.Allowed)
                    {
                        try
                        {
                            context.Allowed &= tryPreparePlacement?.Invoke() ?? true;
                            return context.Allowed;
                        }
                        catch
                        {
                            context.Allowed = false;
                            throw;
                        }
                    }

                    if (!isCurrentLife() || (state != GameLogic.PlayerState.PartyRequest
                        && state != GameLogic.PlayerState.TradeRequested && state != GameLogic.PlayerState.TradeOpened
                        && state != GameLogic.PlayerState.TradeButtonPressed && state != GameLogic.PlayerState.NpcDialogOpened))
                    {
                        return false;
                    }
                }

                if (state == GameLogic.PlayerState.PartyRequest)
                {
                    await new GameLogic.PlayerActions.Party.PartyResponseAction().HandleResponseAsync(this, false).ConfigureAwait(false);
                }
                else if (state == GameLogic.PlayerState.NpcDialogOpened)
                {
                    if (dialog is not null)
                    {
                        await new CloseNpcDialogAction().CloseNpcDialogAsync(this).ConfigureAwait(false);
                    }
                    else
                    {
                        await using var context = await this.PlayerState.TryBeginAdvanceToAsync(GameLogic.PlayerState.EnteredWorld).ConfigureAwait(false);
                        context.Allowed &= isCurrentLife() && this.PlayerState.CurrentState == GameLogic.PlayerState.NpcDialogOpened && this.OpenedNpc is null;
                    }
                }
                else
                {
                    await this.CloseTradeIfNeededAsync().ConfigureAwait(false);
                }

                if (this.PlayerState.CurrentState == state)
                {
                    return false;
                }
            }
        }
        finally
        {
            if (acquired)
            {
                await using var context = await this.PlayerState.TryBeginAdvanceToAsync(this.PlayerState.CurrentState).ConfigureAwait(false);
                context.Allowed = false;
                if (ReferenceEquals(this._forcedPlacementPreparation, attributes))
                {
                    this._forcedPlacementPreparation = null;
                }
            }
        }
    }

    /// <summary>Restores liveness after placement only when no lethal hit owns this life.</summary>
    internal void RestoreLivenessAfterPlacement()
    {
        lock (this._hitLock)
        {
            if (!this._hasReservedLethalHit)
            {
                this.IsAlive = true;
            }
        }
    }

    /// <summary>Runs actual death restoration, distinct from generic siege or duel placement.</summary>
    /// <param name="gate">The selected respawn gate, or the ordinary spawn selection.</param>
    /// <param name="expectedSource">The original death timer, when invoked by its continuation.</param>
    /// <param name="expectedDeath">The original death identity, when invoked by its continuation.</param>
    internal async ValueTask RespawnAfterDeathAsync(ExitGate? gate = null, CancellationTokenSource? expectedSource = null, DeathInformation? expectedDeath = null)
    {
        CancellationTokenSource source;
        CancellationToken token;
        DeathInformation death;
        await using (var context = await this.PlayerState.TryBeginAdvanceToAsync(this.PlayerState.CurrentState).ConfigureAwait(false))
        {
            context.Allowed = false;
            if (this.IsDisposed || this.IsDisposing || this._respawnAfterDeathCts is not { } current || this.LastDeath is not { } currentDeath
                || (expectedSource is not null && !ReferenceEquals(current, expectedSource))
                || (expectedDeath is not null && !ReferenceEquals(currentDeath, expectedDeath)))
            {
                return;
            }

            source = current;
            token = source.Token;
            death = currentDeath;
        }

        if (!await this.ContinueDeathRespawnAsync(source, death, token, claim: true).ConfigureAwait(false))
        {
            return;
        }

        var prepared = false;
        var releaseSource = false;
        try
        {
            await this._summon.RemoveAsync().ConfigureAwait(false);
            if (!await this.ContinueDeathRespawnAsync(source, death, token, claim: false).ConfigureAwait(false))
            {
                return;
            }

            await this.MagicEffectList.ClearEffectsAfterDeathAsync().ConfigureAwait(false);
            if (!await this.ContinueDeathRespawnAsync(source, death, token, claim: false).ConfigureAwait(false))
            {
                return;
            }

            gate ??= await this._mapTransitions.GetSpawnGateOfCurrentMapAsync().ConfigureAwait(false);
            if (!await this.ContinueDeathRespawnAsync(source, death, token, claim: false).ConfigureAwait(false))
            {
                return;
            }

            List<(AttributeDefinition Definition, float Value)> changes = [];
            await this._mapTransitions.RespawnWithPreparationAsync(
                gate,
                () => prepared = this.TryPrepareForRespawn(source, death, token, changes),
                () => this.PublishAttributeChangesAsync(changes),
                () => releaseSource = this.TryCompleteDeathRespawn(source, death)).ConfigureAwait(false);
            if (!prepared)
            {
                await this.DeferDeathRespawnClaimAsync(source, death).ConfigureAwait(false);
                return;
            }

            if (!this.IsDisposed && !this.IsDisposing && this.SelectedCharacter is not null
                && ReferenceEquals(this.LastDeath, death) && this._respawnAfterDeathCts is null)
            {
                await this.RespawnOfDuelPartnerIfInDuelAsync().ConfigureAwait(false);
            }
        }
        catch
        {
            if (!prepared)
            {
                await this.DeferDeathRespawnClaimAsync(source, death).ConfigureAwait(false);
            }

            throw;
        }
        finally
        {
            if (releaseSource)
            {
                source.Dispose();
            }
        }
    }

    /// <summary>
    /// Finishes a lethal hit handed off during a map change, after the player was added to the map.
    /// </summary>
    internal async ValueTask CompleteMapChangeDeathAsync()
    {
        (IAttacker? Killer, DeathInformation Death)? pending;
        (CancellationTokenSource Source, DeathInformation Death, bool Claimed)? respawn;
        await using (var context = await this.PlayerState.TryBeginAdvanceToAsync(this.PlayerState.CurrentState).ConfigureAwait(false))
        {
            context.Allowed = false;
            pending = this._pendingMapChangeDeath;
            this._pendingMapChangeDeath = null;
            respawn = this._pendingDeathRespawn;
        }

        if (pending is { } death)
        {
            await this.OnDeathAsync(death.Killer, death.Death).ConfigureAwait(false);
        }

        if (respawn is { Claimed: false } due)
        {
            await this.RespawnAfterDeathAsync(expectedSource: due.Source, expectedDeath: due.Death).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        var respawnCancellation = await this.TakeRespawnCancellationAsync().ConfigureAwait(false);
        if (respawnCancellation is not null)
        {
            await respawnCancellation.CancelAsync().ConfigureAwait(false);
            respawnCancellation.Dispose();
        }

        // A disposed player must never stay reachable from the game context: it would keep counting
        // towards the server's maximum player count (which turns real clients away) and keep its whole
        // object graph alive. The regular teardown removes it through the PlayerDisconnected event, but
        // a player which is disposed WITHOUT having been disconnected - a login or spawn which failed
        // after the player was already added to the game - never raises that event, and the fields
        // below drop the handler which could still do it. Removing here is the last line of defense,
        // and it is idempotent, so it does nothing on the regular path.
        try
        {
            await this.GameContext.RemovePlayerAsync(this).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Must not abort the rest of the disposal - the remaining resources have to be released.
            this.Logger.LogError(ex, "Error while removing player {Player} from the game context during disposal.", this);
        }

        await this._muHelperLazy.DisposeIfCreatedAsync().ConfigureAwait(false);

        this._petCommandManager?.Dispose();
        this._petCommandManager = null;
        this.LastAttackedTarget.SetTarget(null);

        this.PersistenceContext.Dispose();
        await this._mapTransitions.RemoveFromCurrentMapAsync().ConfigureAwait(false);
        await this._observerToWorldViewAdapter.ClearObservingObjectsListAsync().ConfigureAwait(false);
        this._observerToWorldViewAdapter.Dispose();
        this._movement.Dispose();
        await this.MagicEffectList.DisposeAsync().ConfigureAwait(false);
        this.ReleaseSkillList();
        this.ReleaseAttributeSystem();
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

        var respawnCancellation = await this.TakeRespawnCancellationAsync().ConfigureAwait(false);
        if (respawnCancellation is not null)
        {
            if (!respawnCancellation.IsCancellationRequested)
            {
                await respawnCancellation.CancelAsync().ConfigureAwait(false);
                moveToNextSafezone = true;
            }

            respawnCancellation.Dispose();
        }

        if (this.CurrentMiniGame is { } miniGame)
        {
            if (this.PlayerState.CurrentState.IsDisconnectedOrFinished())
            {
                // The connection got lost, so the player may come back while the game is still running.
                miniGame.RememberDisconnectedPlayer(this);
            }

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

    private async ValueTask RegenerateHeroStateAsync()
    {
        // A newly created character has no hero state yet, so there is nothing to count down.
        if (this._selectedCharacter is not { } currentCharacter
            || currentCharacter.State is HeroState.Normal or HeroState.New)
        {
            this._heroStateSecondsRemainder = 0;
            return;
        }

        // Only whole seconds are subtracted and the fraction is kept for the next tick. Rounding each tick
        // made the countdown depend on the recovery interval, e.g. at 500 ms it never counted down at all.
        var elapsedSeconds = DateTime.UtcNow.Subtract(this._lastRegenerate).TotalSeconds + this._heroStateSecondsRemainder;
        var elapsedWholeSeconds = Math.Floor(elapsedSeconds);
        this._heroStateSecondsRemainder = elapsedSeconds - elapsedWholeSeconds;
        currentCharacter.StateRemainingSeconds -= (int)elapsedWholeSeconds;
        if (currentCharacter.StateRemainingSeconds > 0)
        {
            return;
        }

        // The time is up, so the state falls back one step towards the normal state. Killed monsters may
        // have pushed the remaining time below zero, so the surplus is carried over to the next step.
        var surplusSeconds = -currentCharacter.StateRemainingSeconds;
        if (currentCharacter.State > HeroState.Normal)
        {
            currentCharacter.State--;
        }
        else
        {
            currentCharacter.State++;
        }

        if (currentCharacter.State == HeroState.Normal)
        {
            currentCharacter.StateRemainingSeconds = 0;
            currentCharacter.PlayerKillCount = 0;
        }
        else
        {
            var stateDuration = currentCharacter.State > HeroState.Normal ? PlayerKillerStateDuration : HeroStateDuration;

            // May still be below zero, if the surplus exceeds this step as well. Then the next tick steps down again.
            currentCharacter.StateRemainingSeconds = (int)stateDuration.TotalSeconds - surplusSeconds;
        }

        await this.ForEachWorldObserverAsync<IUpdateCharacterHeroStatePlugIn>(p => p.UpdateCharacterHeroStateAsync(this), true).ConfigureAwait(false);
    }

    /// <summary>
    /// Limits the remaining time of the hero state to the longest time which the current state can
    /// legitimately have. Characters of servers which ran with the formerly broken countdown can have
    /// a remaining time which grew by all the time they spent online.
    /// </summary>
    private void LimitHeroStateRemainingTime()
    {
        if (this._selectedCharacter is not { } character
            || character.State is HeroState.Normal or HeroState.New)
        {
            return;
        }

        var maximumSeconds = character.State switch
        {
            // Every kill after the one which reached the 2nd stage adds another state duration on top.
            HeroState.PlayerKiller2ndStage => (int)PlayerKillerStateDuration.TotalSeconds * Math.Max(character.PlayerKillCount - 2, 1),
            > HeroState.Normal => (int)PlayerKillerStateDuration.TotalSeconds,
            _ => (int)HeroStateDuration.TotalSeconds,
        };

        if (character.StateRemainingSeconds > maximumSeconds)
        {
            this.Logger.LogInformation(
                "Limited the remaining hero state time of character {CharacterName} ({HeroState}) from {RemainingSeconds} to {MaximumSeconds} seconds.",
                character.Name,
                character.State,
                character.StateRemainingSeconds,
                maximumSeconds);
            character.StateRemainingSeconds = maximumSeconds;
        }
    }

    private async ValueTask ApplyHealthLossAfterHitAsync(float healthLoss)
    {
        List<(AttributeDefinition Definition, float Value)> attributeChanges = [];
        lock (this._hitLock)
        {
            if (!this.IsAlive)
            {
                return;
            }

            using (this.CollectAttributeChangesUnderHitLock(attributeChanges))
            {
                this.Attributes![Stats.CurrentHealth] = Math.Max(this.Attributes[Stats.CurrentHealth] - healthLoss, 1);
            }
        }

        await this.PublishAttributeChangesAsync(attributeChanges).ConfigureAwait(false);
    }

    private async ValueTask<bool> HitAsync(HitInfo hitInfo, IAttacker attacker, Skill? skill, bool? isFinalStreakHit = null, bool recoverHealthBeforeHit = false, bool applyOrdinaryHitEffects = false, bool recoverManaBeforeHit = false)
    {
        bool killed;
        bool shieldBurst;
        DeathInformation? ownedDeath = null;
        var skillNumber = skill?.Number ?? 0;
        var attackerId = attacker.Id;
        var attackerName = attacker.GetName();
        List<(AttributeDefinition Definition, float Value)> attributeChanges = [];
        lock (this._hitLock)
        {
            if (!this.IsAlive || this._hasReservedLethalHit)
            {
                return false;
            }

            var attributes = this.Attributes!;
            using (this.CollectAttributeChangesUnderHitLock(attributeChanges))
            {
                if (recoverHealthBeforeHit)
                {
                    attributes[Stats.CurrentHealth] = attributes[Stats.MaximumHealth];
                }

                if (applyOrdinaryHitEffects && (hitInfo.ManaToll > 0 || recoverManaBeforeHit))
                {
                    attributes[Stats.CurrentMana] = (recoverManaBeforeHit ? attributes[Stats.MaximumMana] : attributes[Stats.CurrentMana]) - hitInfo.ManaToll;
                }

                var healthDamage = hitInfo.HealthDamage;
                int oversd = (int)(attributes[Stats.CurrentShield] - hitInfo.ShieldDamage);
                shieldBurst = false;
                if (oversd < 0)
                {
                    healthDamage += (uint)(oversd * (-1));
                    shieldBurst = attributes[Stats.CurrentShield] > 0
                        && this.CurrentMiniGame?.Definition.Type != MiniGameType.ChaosCastle
                        && healthDamage > attributes[Stats.MaximumHealth] * attributes[Stats.OverShieldBurstDamageRatio];
                    attributes[Stats.CurrentShield] = 0;
                }
                else
                {
                    attributes[Stats.CurrentShield] = oversd;
                }

                attributes[Stats.CurrentHealth] -= healthDamage;
                killed = attributes[Stats.CurrentHealth] < 1;
                if (isFinalStreakHit.HasValue)
                {
                    hitInfo.Attributes |= DamageAttributes.RageFighterStreakHit;
                    if (isFinalStreakHit.Value || killed)
                    {
                        hitInfo.Attributes |= DamageAttributes.RageFighterStreakFinalHit;
                    }
                }

                if (killed)
                {
                    ownedDeath = new DeathInformation(attackerId, attackerName, hitInfo, skillNumber);
                    this.LastDeath = ownedDeath;
                    this._hasReservedLethalHit = true;
                    this.IsAlive = false;
                }
            }
        }

        ExceptionDispatchInfo? effectFailure = null;
        try
        {
            if (applyOrdinaryHitEffects)
            {
                try
                {
                    if (this.Attributes![Stats.IsFrozen] > 0)
                    {
                        await this.MagicEffectList.ShortenEffectsProducingSpecificStatAsync(Stats.IsFrozen, 1).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    effectFailure = ExceptionDispatchInfo.Capture(ex);
                }

                try
                {
                    if (this.Attributes![Stats.IsAsleep] > 0)
                    {
                        await this.MagicEffectList.ClearAllEffectsProducingSpecificStatAsync(Stats.IsAsleep).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    if (effectFailure is null)
                    {
                        effectFailure = ExceptionDispatchInfo.Capture(ex);
                    }
                    else
                    {
                        this.Logger.LogError(ex, "Error waking the character after an earlier hit-effect failure.");
                    }
                }
            }

            await this.PublishAttributeChangesAsync(attributeChanges).ConfigureAwait(false);
            this._summon.RegisterHit(attacker);
            if (shieldBurst)
            {
                await this.ForEachWorldObserverAsync<IShowEffectPlugIn>(p => p.ShowEffectAsync(this, IShowEffectPlugIn.EffectType.ShieldBurst), true).ConfigureAwait(false);
            }

            await this.InvokeViewPlugInAsync<IShowHitPlugIn>(p => p.ShowHitAsync(this, hitInfo)).ConfigureAwait(false);
            if (attacker is IWorldObserver observer)
            {
                await observer.InvokeViewPlugInAsync<IShowHitPlugIn>(p => p.ShowHitAsync(this, hitInfo)).ConfigureAwait(false);
            }

            this.GameContext.PlugInManager.GetPlugInPoint<IAttackableGotHitPlugIn>()?.AttackableGotHit(this, attacker, hitInfo);
        }
        catch (Exception ex) when (effectFailure is not null)
        {
            this.Logger.LogError(ex, "Error publishing an accepted hit after its effect update failed.");
        }
        finally
        {
            if (killed)
            {
                try
                {
                    await this.OnDeathAsync(attacker, ownedDeath).ConfigureAwait(false);
                }
                catch (Exception ex) when (effectFailure is not null)
                {
                    this.Logger.LogError(ex, "Error completing death after an earlier hit-effect failure.");
                }
            }
        }

        effectFailure?.Throw();

        if (hitInfo.Attributes.HasFlag(DamageAttributes.Poison))
        {
            // Poison Damage does not reflect to the attacker.
            return true;
        }

        if (attacker is IAttackable or AttackerSurrogate)
        {
            var attackableAttacker = attacker is AttackerSurrogate surrogate ? surrogate.Owner : (IAttackable)attacker;

            var reflectPercentage = this.Attributes![Stats.DamageReflection];
            if (reflectPercentage > 0)
            {
                var reflectedDamage = hitInfo.TotalDamage * reflectPercentage;
                ReflectDamage((int)reflectedDamage, attackableAttacker);
            }

            if (attacker is not AttackerSurrogate)
            {
                // Raven does not cause full reflect.
                var fullReflectPercentage = this.Attributes![Stats.FullyReflectDamageAfterHitChance];
                if (fullReflectPercentage > 0 && Rand.NextRandomBool(fullReflectPercentage))
                {
                    var reflectedDamage = attackableAttacker is Player
                        ? hitInfo.TotalDamage
                        : attackableAttacker.Attributes[Stats.MaximumPhysBaseDmg];
                    ReflectDamage((int)reflectedDamage, attackableAttacker);
                }
            }
        }

        return true;

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

    private async ValueTask<bool> TryEnterDeadStateAsync(IAttacker? killer, DeathInformation? ownedDeath)
    {
        // Death cancels combat-reachable interactions using their normal cleanup. A changing-map
        // life keeps its original lethal owner until arrival; disconnected states still refuse death.
        while (true)
        {
            State state;
            NonPlayerCharacter? dialog;
            await using (var context = await this.PlayerState.TryBeginAdvanceToAsync(GameLogic.PlayerState.Dead).ConfigureAwait(false))
            {
                state = this.PlayerState.CurrentState;
                dialog = this.OpenedNpc;
                if (ownedDeath is not null)
                {
                    lock (this._hitLock)
                    {
                        if (this.IsDisposed || this.IsDisposing || this.SelectedCharacter is null
                            || !this._hasReservedLethalHit || !ReferenceEquals(this.LastDeath, ownedDeath))
                        {
                            context.Allowed = false;
                            return false;
                        }
                    }
                }

                if (state == GameLogic.PlayerState.ChangingMap && ownedDeath is not null)
                {
                    this._pendingMapChangeDeath = (killer, ownedDeath);
                    context.Allowed = false;
                    return false;
                }

                if (state != GameLogic.PlayerState.PartyRequest
                    && state != GameLogic.PlayerState.TradeRequested
                    && state != GameLogic.PlayerState.TradeOpened
                    && state != GameLogic.PlayerState.TradeButtonPressed
                    && state != GameLogic.PlayerState.NpcDialogOpened)
                {
                    return context.Allowed;
                }

                context.Allowed = false;
            }

            if (state == GameLogic.PlayerState.PartyRequest)
            {
                this.LastPartyRequester = null;
                await this.PlayerState.TryAdvanceToAsync(GameLogic.PlayerState.EnteredWorld).ConfigureAwait(false);
            }
            else if (state == GameLogic.PlayerState.TradeRequested
                     || state == GameLogic.PlayerState.TradeOpened
                     || state == GameLogic.PlayerState.TradeButtonPressed)
            {
                await this.CloseTradeIfNeededAsync().ConfigureAwait(false);
            }
            else
            {
                if (dialog is not null)
                {
                    await new CloseNpcDialogAction().CloseNpcDialogAsync(this).ConfigureAwait(false);
                }
                else
                {
                    await using var context = await this.PlayerState.TryBeginAdvanceToAsync(GameLogic.PlayerState.EnteredWorld).ConfigureAwait(false);
                    var current = this.GetPlacementDeathSnapshot();
                    context.Allowed &= this.PlayerState.CurrentState == GameLogic.PlayerState.NpcDialogOpened && this.OpenedNpc is null
                        && (ownedDeath is null || (current.Reserved && ReferenceEquals(current.Death, ownedDeath)));
                }
            }

            if (this.PlayerState.CurrentState == state)
            {
                return false;
            }
        }
    }

    private async ValueTask OnDeathAsync(IAttacker? killer, DeathInformation? ownedDeath = null)
    {
        if (!await this.TryEnterDeadStateAsync(killer, ownedDeath).ConfigureAwait(false))
        {
            return;
        }

        await this._movement.StopWalkingAsync().ConfigureAwait(false);
        CancellationTokenSource respawnCancellation;
        CancellationToken respawnToken;
        DeathInformation deathInformation;
        CancellationTokenSource? previousRespawnCancellation;
        await using (var context = await this.PlayerState.TryBeginAdvanceToAsync(this.PlayerState.CurrentState).ConfigureAwait(false))
        {
            context.Allowed = false;
            lock (this._hitLock)
            {
                if (this.IsDisposed || this.IsDisposing || this.SelectedCharacter is null
                    || !this._hasReservedLethalHit || this.LastDeath is not { } currentDeath
                    || (ownedDeath is not null && !ReferenceEquals(currentDeath, ownedDeath))
                    || this.PlayerState.CurrentState.IsDisconnectedOrFinished())
                {
                    return;
                }

                this.IsAlive = false;
                deathInformation = currentDeath;
                respawnCancellation = new CancellationTokenSource();
                respawnToken = respawnCancellation.Token;
                previousRespawnCancellation = this._respawnAfterDeathCts;
                this._respawnAfterDeathCts = respawnCancellation;
                this._pendingDeathRespawn = null;
            }
        }

        // Replacement transfers disposal from the old timer; it only uses its already captured token.
        previousRespawnCancellation?.Dispose();
        await this.ForEachWorldObserverAsync<IObjectGotKilledPlugIn>(p => p.ObjectGotKilledAsync(this, killer), true).ConfigureAwait(false);

        if (killer is Player killerAfterKilled
            && !(killerAfterKilled.GuildWarContext?.Score is { } score && score == this.GuildWarContext?.Score)
            && this.CurrentMiniGame?.AllowPlayerKilling is not true)
        {
            await killerAfterKilled.AfterKilledPlayerAsync(this).ConfigureAwait(false);
        }

        // TODO: Drop items
        async Task RespawnAsync()
        {
            try
            {
                await Task.Delay(3000, respawnToken).ConfigureAwait(false);
                await this.RespawnAfterDeathAsync(expectedSource: respawnCancellation, expectedDeath: deathInformation).ConfigureAwait(false);
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

        _ = RespawnAsync();

        if (this.GameContext.PlugInManager.GetPlugInPoint<IAttackableGotKilledPlugIn>() is { } plugInPoint)
        {
            await plugInPoint.AttackableGotKilledAsync(this, killer).ConfigureAwait(false);
        }

        this.Died?.Invoke(this, deathInformation);
    }

    private async ValueTask<CancellationTokenSource?> TakeRespawnCancellationAsync()
    {
        await using var context = await this.PlayerState.TryBeginAdvanceToAsync(this.PlayerState.CurrentState).ConfigureAwait(false);
        context.Allowed = false;
        var source = this._respawnAfterDeathCts;
        this._respawnAfterDeathCts = null;
        this._pendingDeathRespawn = null;
        this._pendingMapChangeDeath = null;
        return source;
    }

    private async ValueTask<bool> ContinueDeathRespawnAsync(CancellationTokenSource source, DeathInformation death, CancellationToken token, bool claim)
    {
        await using var context = await this.PlayerState.TryBeginAdvanceToAsync(this.PlayerState.CurrentState).ConfigureAwait(false);
        context.Allowed = false;
        lock (this._hitLock)
        {
            if (token.IsCancellationRequested || this.IsDisposed || this.IsDisposing || this.SelectedCharacter is null
                || !ReferenceEquals(this._respawnAfterDeathCts, source) || !ReferenceEquals(this.LastDeath, death)
                || !this._hasReservedLethalHit || this.PlayerState.CurrentState.IsDisconnectedOrFinished())
            {
                return false;
            }

            if (claim && this._pendingDeathRespawn is { Claimed: true })
            {
                return false;
            }

            if (!claim && this._pendingDeathRespawn is not { Claimed: true })
            {
                return false;
            }

            if (this.CurrentMap is null || this.PlayerState.CurrentState == GameLogic.PlayerState.ChangingMap)
            {
                this._pendingDeathRespawn = (source, death, false);
                return false;
            }

            this._pendingDeathRespawn = (source, death, true);
            return true;
        }
    }

    private async ValueTask DeferDeathRespawnClaimAsync(CancellationTokenSource source, DeathInformation death)
    {
        await using var context = await this.PlayerState.TryBeginAdvanceToAsync(this.PlayerState.CurrentState).ConfigureAwait(false);
        context.Allowed = false;
        if (ReferenceEquals(this._respawnAfterDeathCts, source) && ReferenceEquals(this.LastDeath, death)
            && this._pendingDeathRespawn is { Claimed: true } pending
            && ReferenceEquals(pending.Source, source) && ReferenceEquals(pending.Death, death))
        {
            // A later map arrival may retry this death; never release a replacement's claim.
            this._pendingDeathRespawn = (source, death, false);
        }
    }

    // Placement owns state admission before consuming this death; notifications are sent outside state/hit exclusion.
    private bool TryPrepareForRespawn(CancellationTokenSource source, DeathInformation death, CancellationToken token, List<(AttributeDefinition Definition, float Value)> changes)
    {
        lock (this._hitLock)
        {
            if (token.IsCancellationRequested || this.IsDisposed || this.IsDisposing || this.SelectedCharacter is null
                || !ReferenceEquals(this._respawnAfterDeathCts, source) || !ReferenceEquals(this.LastDeath, death)
                || !this._hasReservedLethalHit || this.PlayerState.CurrentState.IsDisconnectedOrFinished()
                || this._pendingDeathRespawn is not { Claimed: true } pending
                || !ReferenceEquals(pending.Source, source) || !ReferenceEquals(pending.Death, death))
            {
                return false;
            }

            if (this.CurrentMap is null || this.PlayerState.CurrentState == GameLogic.PlayerState.ChangingMap)
            {
                return false;
            }

            using (this.CollectAttributeChangesUnderHitLock(changes))
            {
                this.SetReclaimableAttributesToMaximum();
                this._hasReservedLethalHit = false;
                this._pendingMapChangeDeath = null;
                this._pendingDeathRespawn = null;
            }
        }

        return true;
    }

    // Called under placement's state admission, after coordinates commit and before any further await.
    private bool TryCompleteDeathRespawn(CancellationTokenSource source, DeathInformation death)
    {
        lock (this._hitLock)
        {
            if (!ReferenceEquals(this._respawnAfterDeathCts, source) || !ReferenceEquals(this.LastDeath, death))
            {
                return false;
            }

            this._respawnAfterDeathCts = null;
            return true;
        }
    }

    /// <summary>
    /// Called when this player is in a duel and was killed.
    /// Resets the partner only after placement owns the same live duel participant.
    /// </summary>
    private async ValueTask RespawnOfDuelPartnerIfInDuelAsync()
    {
        if (this.DuelRoom is not { State: DuelState.DuelStarted } duelRoom
            || !duelRoom.IsDuelist(this)
            || (duelRoom.Requester == this ? duelRoom.Opponent : duelRoom.Requester) is not { IsAlive: true, CurrentMap: not null } duelPartner
            || duelRoom.GetSpawnGate(duelPartner) is not { } partnerSpawnGate)
        {
            return;
        }

        var character = this.SelectedCharacter;
        var attributes = this.Attributes;
        var partnerCharacter = duelPartner.SelectedCharacter;
        var partnerAttributes = duelPartner.Attributes;
        var initiatorDeath = this.LastDeath;
        if (character is null || attributes is null || partnerCharacter is null || partnerAttributes is null)
        {
            return;
        }

        List<(AttributeDefinition Definition, float Value)> changes = [];
        await duelPartner._mapTransitions.RespawnWithPreparationAsync(
            partnerSpawnGate,
            TryPreparePartner,
            () => duelPartner.PublishAttributeChangesAsync(changes)).ConfigureAwait(false);

        bool TryPreparePartner()
        {
            lock (duelPartner._hitLock)
            {
                if (duelRoom.State != DuelState.DuelStarted || !ReferenceEquals(this.DuelRoom, duelRoom)
                    || !ReferenceEquals(duelPartner.DuelRoom, duelRoom) || !duelRoom.IsDuelist(this) || !duelRoom.IsDuelist(duelPartner)
                    || !ReferenceEquals(this.SelectedCharacter, character) || !ReferenceEquals(this.Attributes, attributes)
                    || !ReferenceEquals(this.LastDeath, initiatorDeath)
                    || !ReferenceEquals(duelPartner.SelectedCharacter, partnerCharacter) || !ReferenceEquals(duelPartner.Attributes, partnerAttributes)
                    || this.IsDisposed || this.IsDisposing || !duelPartner.IsAlive || duelPartner._hasReservedLethalHit)
                {
                    return false;
                }

                using (duelPartner.CollectAttributeChangesUnderHitLock(changes))
                {
                    // Keep the normal false-before-removal hook semantics, but only for this admitted reset.
                    duelPartner.IsAlive = false;
                    duelPartner.SetReclaimableAttributesToMaximum();
                }

                return true;
            }
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
        if (this.Attributes is { } attributes)
        {
            this._mapPowerUps.Apply(map.Definition, attributes);
        }
    }

    private void RaisePlayerLeftMap(GameMap map)
    {
        this._mapPowerUps.Remove();
        this.PlayerLeftMap?.Invoke(this, (this, map));
    }

    /// <summary>
    /// Moves the character to an active map, if its current map is inactive, so that
    /// it doesn't enter a map which is not available in the game anymore.
    /// The target is the safezone map of the inactive map, or the home map of the character class,
    /// or any other active map, see <see cref="GameMapDefinitionExtensions.GetActiveSpawnMap"/>.
    /// </summary>
    /// <param name="character">The character.</param>
    private void MoveOutOfInactiveMap(Character character)
    {
        if (character.CurrentMap is not { IsActive: false } inactiveMap)
        {
            return;
        }

        var targetMap = this.GameContext.Configuration.GetActiveSpawnMap(inactiveMap.SafezoneMap, character.CharacterClass?.HomeMap);
        if (targetMap?.GetSafezoneGate() is not { } spawnGate)
        {
            this.Logger.LogWarning("Character {character} is on the inactive map {map}, but there is no active map to move it to.", character, inactiveMap);
            return;
        }

        this.Logger.LogInformation("Character {character} is on the inactive map {map}, moving it to {targetMap}.", character, inactiveMap, targetMap);
        character.CurrentMap = targetMap;
        character.PositionX = (byte)Rand.NextInt(spawnGate.X1, spawnGate.X2 + 1);
        character.PositionY = (byte)Rand.NextInt(spawnGate.Y1, spawnGate.Y2 + 1);
    }

    /// <summary>
    /// Moves the character to the safezone of a mini game map, if it was saved on one and doesn't
    /// rejoin a running game. E.g. the winners of the Illusion Temple stay on its map until they close the
    /// result dialog; a character which left the game there would otherwise log in on the empty event map.
    /// </summary>
    /// <param name="character">The character.</param>
    private void MoveOutOfMiniGameMap(Character character)
    {
        if (character.CurrentMap is not { } map
            || this.GameContext.Configuration.MiniGameDefinitions?.Any(definition => Equals(definition.Entrance?.Map, map)) is not true)
        {
            return;
        }

        var targetMap = this.GameContext.Configuration.GetActiveSpawnMap(map.SafezoneMap, character.CharacterClass?.HomeMap);
        if (targetMap?.GetSafezoneGate() is not { } spawnGate)
        {
            this.Logger.LogWarning("Character {character} is on the mini game map {map}, but there is no safezone to move it to.", character, map);
            return;
        }

        this.Logger.LogInformation("Character {character} is on the mini game map {map} without a running game, moving it to {targetMap}.", character, map, targetMap);
        character.CurrentMap = targetMap;
        character.PositionX = (byte)Rand.NextInt(spawnGate.X1, spawnGate.X2 + 1);
        character.PositionY = (byte)Rand.NextInt(spawnGate.Y1, spawnGate.Y2 + 1);
    }

    /// <summary>
    /// Releases the attribute system of the character which was in the game.
    /// </summary>
    /// <remarks>
    /// Its power-ups wrap elements of the game configuration (item options, item base values, map
    /// power-ups), which live as long as the server. Until they are disposed, the configuration keeps
    /// the attribute system and, through its event handlers, this whole player alive.
    /// </remarks>
    private void ReleaseAttributeSystem()
    {
        this._mapPowerUps.Remove();
        this._accountAttributeGrantPowerUps.Remove();
        if (this.Attributes is { } attributes)
        {
            // The ammunition amount of some classes is a stat attribute, which belongs to the character.
            attributes.GetOrCreateAttribute(Stats.AmmunitionAmount).ValueChanged -= this.OnAmmunitionAmountChanged;
            attributes.Dispose();
        }
    }

    /// <summary>
    /// Releases the skill list of the character which was in the game, including the power-ups of its passive skills.
    /// </summary>
    private void ReleaseSkillList()
    {
        (this.SkillList as IDisposable)?.Dispose();
        this.SkillList = null;
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

        this.RemoveStatAttributesWithoutDefinition(character);
        this.RemoveDuplicateStatAttributes(character);

        // A class stat without an attribute would be saved as a stat without a definition. This happens
        // when a running server applies a data update whose attribute isn't in its cached configuration yet.
        if (characterClass.StatAttributes.Any(a => a.Attribute is null))
        {
            this.Logger.LogWarning(
                "The character class '{CharacterClass}' has stat attributes without an attribute definition; they're not added to character '{Character}'. Restart the server after installing data updates.",
                characterClass,
                character.Name);
        }

        // The character class itself may define a stat attribute more than once (a data update which
        // added an attribute the class already had), so the missing ones are taken distinctly - otherwise
        // we would create the duplicates we just removed all over again.
        var missingStats = characterClass.StatAttributes
            .Where(a => a.Attribute is not null)
            .DistinctBy(a => a.Attribute)
            .Where(a => character.Attributes.All(c => c.Definition != a.Attribute));

        var attributes = missingStats.Select(a => this.PersistenceContext.CreateNew<StatAttribute>(a.Attribute!, a.BaseValue)).ToList();
        attributes.ForEach(character.Attributes.Add);
    }

    /// <summary>
    /// Removes stat attributes without a definition. They have no meaning, and an attribute system
    /// can't hold them, so the character would be unable to enter the game at all.
    /// </summary>
    /// <param name="character">The character.</param>
    private void RemoveStatAttributesWithoutDefinition(Character character)
    {
        var invalid = character.Attributes.Where(a => a.Definition is null).ToList();
        if (invalid.Count == 0)
        {
            return;
        }

        invalid.ForEach(attribute => character.Attributes.Remove(attribute));
        this.Logger.LogWarning(
            "Removed {Count} stat attribute(s) without a definition of character '{Character}'.",
            invalid.Count,
            character.Name);
    }

    /// <summary>
    /// Removes stat attributes which are assigned to the character more than once, keeping the one with
    /// the highest value. An attribute system holds exactly one attribute per definition, so a duplicate
    /// would make the character unable to enter the game at all.
    /// </summary>
    /// <param name="character">The character.</param>
    private void RemoveDuplicateStatAttributes(Character character)
    {
        var duplicateGroups = character.Attributes
            .GroupBy(a => a.Definition)
            .Where(group => group.Count() > 1)
            .ToList();

        foreach (var duplicates in duplicateGroups)
        {
            // The highest value is kept, so a character never loses points that were invested into a stat.
            var obsolete = duplicates.OrderByDescending(a => a.Value).Skip(1).ToList();
            obsolete.ForEach(attribute => character.Attributes.Remove(attribute));

            this.Logger.LogWarning(
                "Removed {Count} duplicate stat attribute(s) '{Attribute}' of character '{Character}'.",
                obsolete.Count,
                duplicates.Key,
                character.Name);
        }
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

        // For characters which got created on the database, it's possible that they're
        // missing the inventory. In this case, we create it here and initialize it with
        // default items, like for a newly created character.
        if (selectedCharacter!.Inventory is null)
        {
            selectedCharacter.Inventory = this.PersistenceContext.CreateNew<ItemStorage>();
            this.GameContext.PlugInManager.GetPlugInPoint<ICharacterCreatedPlugIn>()?.CharacterCreated(this.Account!, selectedCharacter, this.PersistenceContext, this.GameContext.Configuration, this.Logger);
        }

        selectedCharacter.CurrentMap ??= selectedCharacter.CharacterClass?.HomeMap;
        this.MoveOutOfInactiveMap(selectedCharacter);
        if (!await this.GameContext.MiniGames.TryRejoinAsync(this).ConfigureAwait(false))
        {
            this.MoveOutOfMiniGameMap(selectedCharacter);
        }

        this.AddMissingStatAttributes();

        // The previous character of this connection may still hold its attribute system.
        this.ReleaseAttributeSystem();
        this.Attributes = new ItemAwareAttributeSystem(this.Account!, selectedCharacter, this.GameContext.Configuration);
        this._accountAttributeGrantPowerUps.Apply(this.Account!, this.Attributes);
        this.Attributes[Stats.NearbyPartyMemberCount] = 0;
        this.Attributes[Stats.IsResting] = 0;
        this.LogInvalidInventoryItems();

        this._storages.CreateForCharacter(selectedCharacter);
        this.ReleaseSkillList();
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

        if (this.Attributes[Stats.MaximumShield] > 0)
        {
            this.Attributes.GetComposableAttribute(Stats.ShieldRecoveryHiatus)?.AddElement(new SimpleElement(0, AggregateType.AddRaw));
            this.LastShieldRecoveryHiatusAccrual = DateTime.UtcNow;
        }

        await this.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);
        this.LimitHeroStateRemainingTime();
        this._lastRegenerate = DateTime.UtcNow;
        this._heroStateSecondsRemainder = 0;

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

    /// <summary>
    /// Starts to collect the attribute changes which the current thread makes while it holds <see cref="_hitLock"/>,
    /// until the returned scope is disposed. The caller sends them afterwards with <see cref="PublishAttributeChangesAsync"/>.
    /// </summary>
    /// <remarks>
    /// The attribute system raises its value changed event synchronously, on the thread of the setter, and
    /// <see cref="OnAttributeValueChanged"/> runs synchronously until its first await. Without collecting, that
    /// handler would start the client update (a view plug-in, i.e. a network write which may wait or call back into
    /// game logic) while the setter still holds the hit lock. The changes can't simply be determined after the lock
    /// is released instead: the event is the only place which also reports the attributes derived from the changed
    /// ones, and a later read would already see the values of a concurrent hit.
    /// The lock is a monitor whose scopes never contain an await, so the thread which entered it stays the same
    /// until it leaves; <see cref="Monitor.IsEntered"/> therefore identifies exactly the changes of the committing
    /// thread. Changes made by other threads at the same time are not collected and are sent as usual.
    /// </remarks>
    /// <param name="changes">The list which receives the changes.</param>
    /// <returns>The scope which ends the collection.</returns>
    private HitAttributeChangeCollection CollectAttributeChangesUnderHitLock(List<(AttributeDefinition Definition, float Value)> changes)
    {
        Debug.Assert(this.IsApplyingHitOnCurrentThread, "Attribute changes are only collected under the hit lock.");
        this._hitAttributeChanges = changes;
        return new HitAttributeChangeCollection(this);
    }

    /// <summary>
    /// Adds the change to the collection of <see cref="CollectAttributeChangesUnderHitLock"/>, if the current thread
    /// holds the hit lock and collects.
    /// </summary>
    /// <param name="attribute">The changed attribute.</param>
    /// <returns><c>true</c>, if the change was collected and is sent by the collecting caller.</returns>
    private bool TryCollectAttributeChangeUnderHitLock(IAttribute attribute)
    {
        if (this.IsApplyingHitOnCurrentThread && this._hitAttributeChanges is { } changes)
        {
            changes.Add((attribute.Definition, attribute.Value));
            return true;
        }

        return false;
    }

    /// <summary>
    /// Sends the collected attribute changes to the client after the hit lock was released, one after another and
    /// in the order in which they were made, so that they arrive before the views which follow.
    /// A failing update is logged by <see cref="NotifyAttributeValueChangedAsync"/> and doesn't stop the next ones.
    /// </summary>
    /// <param name="changes">The collected changes.</param>
    private async ValueTask PublishAttributeChangesAsync(List<(AttributeDefinition Definition, float Value)> changes)
    {
        Debug.Assert(!this.IsApplyingHitOnCurrentThread, "Attribute changes are sent after the hit lock was released.");
        foreach (var (definition, value) in changes)
        {
            await this.NotifyAttributeValueChangedAsync(definition, value).ConfigureAwait(false);
        }
    }

    private async ValueTask NotifyAttributeValueChangedAsync(AttributeDefinition definition, float value)
    {
        try
        {
            await this.InvokeViewPlugInAsync<IUpdateStatsPlugIn>(p => p.UpdateStatsAsync(definition, value)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, $"{nameof(this.OnAttributeValueChanged)} failed for attribute {definition}.");
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
    /// Determines whether a plugin prevents the attacking player, or the owner of the attacking pet, from damaging this player.
    /// </summary>
    /// <param name="attacker">The attacker.</param>
    /// <returns><c>true</c>, if the attack is prevented; otherwise, <c>false</c>.</returns>
    private bool IsAttackPreventedByPlugIns(IAttacker attacker)
    {
        if ((attacker as Player ?? (attacker as IPlayerSurrogate)?.Owner) is not { } attackerPlayer
            || attackerPlayer == this
            || this.GameContext.PlugInManager.GetPlugInPoint<IPlayerAttackRestrictionPlugIn>() is not { } plugInPoint)
        {
            return false;
        }

        var eventArgs = new CancelEventArgs();
        plugInPoint.CheckAttack(attackerPlayer, this, eventArgs);
        return eventArgs.Cancel;
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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    private async void OnAttributeValueChanged(object? sender, IAttribute attribute)
    {
        try
        {
            _ = LimitCurrentAttribute(Stats.MaximumHealth, Stats.CurrentHealth)
                || LimitCurrentAttribute(Stats.MaximumMana, Stats.CurrentMana)
                || LimitCurrentAttribute(Stats.MaximumShield, Stats.CurrentShield)
                || LimitCurrentAttribute(Stats.MaximumAbility, Stats.CurrentAbility);

            if (this.TryCollectAttributeChangeUnderHitLock(attribute))
            {
                return;
            }

            await this.NotifyAttributeValueChangedAsync(attribute.Definition, attribute.Value).ConfigureAwait(false);
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

    private async ValueTask DecreaseItemDurabilityAfterHitAsync(float damage, bool isPvpDamage, SkillEntry? skill, float petDamage)
    {
        if (!isPvpDamage)
        {
            var randomDefensiveItem = this.Inventory?.EquippedItems.Where(ItemExtensions.IsDefensiveItem).SelectRandom();
            if (randomDefensiveItem is { Durability: > 0.0 })
            {
                await this.DecreaseDefenseItemDurabilityAsync(randomDefensiveItem, damage).ConfigureAwait(false);
            }
        }

        if (skill?.Skill?.DamageType == DamageType.Fenrir)
        {
            var randomItem = this.Inventory?.EquippedItems.Where(i => i.ItemSlot <= InventoryConstants.BootsSlot).SelectRandom();
            if (randomItem is { Durability: > 0.0 })
            {
                await this.DecreaseItemDurabilityAsync(randomItem, randomItem.Durability * 0.5).ConfigureAwait(false);
            }
        }

        if (Rand.NextRandomBool(skill?.Attributes?[Stats.RagefulBlowMasteryDurabilityDecChance] ?? 0))
        {
            var randomArmorItem = this.Inventory?.EquippedItems.Where(ItemExtensions.IsArmorItem).SelectRandom();
            if (randomArmorItem is { Durability: > 0.0 })
            {
                await this.DecreaseItemDurabilityAsync(
                    randomArmorItem,
                    randomArmorItem.GetMaximumDurabilityOfOnePiece() * this.Attributes![Stats.DurabilityReductionFactor])
                .ConfigureAwait(false);
            }
        }

        if (this.Inventory?.GetItem(InventoryConstants.PetSlot) is { Durability: > 0.0 } pet)
        {
            await this.DecreasePetDurabilityAsync(pet, petDamage).ConfigureAwait(false);
        }
    }

    private async ValueTask DecreaseDefenseItemDurabilityAsync(Item targetItem, float damage)
    {
        if (targetItem.Durability == 0 || this.Attributes is not { } attributes)
        {
            return;
        }

        float itemDurationIncrease = attributes[Stats.WeaponAndArmorDurationIncrease];
        if (itemDurationIncrease == 0)
        {
            itemDurationIncrease = 1;
        }

        float itemDefense;
        int itemDefenseFactor;
        if (targetItem.IsShield())
        {
            itemDefense = attributes[Stats.DefenseShield];
            itemDefenseFactor = 5;
        }
        else
        {
            if (!attributes.ItemPowerUps.TryGetValue(targetItem, out var itemPowerUps))
            {
                return;
            }

            itemDefense = attributes.GetComposableAttribute(Stats.DefenseBase)?.Elements
                .Where(e => e.AggregateType == AggregateType.AddRaw && itemPowerUps.Contains(e))
                .Sum(e => e.Value) ?? 0;

            if (attributes[Stats.ArcheryMinDmg] > 0)
            {
                itemDefenseFactor = 2;
            }
            else if (attributes[Stats.TotalLeadership] > 0)
            {
                itemDefenseFactor = 6;
            }
            else if (attributes[Stats.SkillMultiplier] > 1 && attributes[Stats.WizardryAttackDamageIncrease] > 0)
            {
                itemDefenseFactor = 7; // MG classes
            }
            else
            {
                itemDefenseFactor = 3;
            }
        }

        if (itemDefense == 0)
        {
            return;
        }

        var damageToDefenseRatio = damage / (itemDefense * itemDefenseFactor);
        var decrement = damageToDefenseRatio / (this.GameContext.Configuration.DamagePerOneItemDurability * itemDurationIncrease);
        await this.DecreaseItemDurabilityAsync(targetItem, decrement).ConfigureAwait(false);
    }

    private async ValueTask DecreasePetDurabilityAsync(Item pet, float damage)
    {
        if (pet.Durability == 0 || pet.Definition is null || this.Attributes is not { } attributes)
        {
            return;
        }

        float itemDurationIncrease;
        var identifier = new ItemIdentifier(pet.Definition.Number, pet.Definition.Group);
        if (identifier == ItemConstants.DarkHorse)
        {
            itemDurationIncrease = attributes[Stats.TrainablePetDurationIncrease];
        }
        else
        {
            itemDurationIncrease = attributes[Stats.PetDurationIncrease];
            if (itemDurationIncrease == 0 || identifier.Number >= ItemConstants.Demon.Number)
            {
                // Excludes cash shop pets: demon, spirit of guardian, rudolf, panda, unicorn, skeleton
                itemDurationIncrease = 1;
            }
        }

        double damageFactor = identifier switch
        {
            var item when item == ItemConstants.Angel
                       || item == ItemConstants.Rudolf => 3,
            var item when item == ItemConstants.Imp
                       || item == ItemConstants.DarkHorse
                       || item == ItemConstants.Fenrir => 2,
            var item when item == ItemConstants.Dinorant => 10,
            var item when item == ItemConstants.Demon => 1.5,
            _ => 1,
        };

        double decrement;
        double damagePerOnePetDurability = this.GameContext.Configuration.DamagePerOnePetDurability;
        if (identifier == ItemConstants.DarkHorse)
        {
            decrement = 1 + (damage * damageFactor / damagePerOnePetDurability);
            decrement /= 1500 + itemDurationIncrease;
        }
        else if (identifier == ItemConstants.Fenrir)
        {
            decrement = 1 + (damage * damageFactor / (damagePerOnePetDurability * itemDurationIncrease));
            decrement /= attributes[Stats.ArcheryMaxDmg] > 0 ? 160 : 200;
        }
        else
        {
            decrement = damage * damageFactor / (damagePerOnePetDurability * itemDurationIncrease);
        }

        await this.DecreaseItemDurabilityAsync(pet, decrement).ConfigureAwait(false);

        if (pet.Durability == 0.0)
        {
            if (identifier == ItemConstants.DarkHorse)
            {
                var minimumExp = pet.Definition!.GetExperienceOfPetLevel(pet.Level, pet.Definition.MaximumItemLevel);
                pet.PetExperience = (int)Math.Max((int)(pet.PetExperience * 0.9), minimumExp);
            }
            else
            {
                await this.DestroyInventoryItemAsync(pet).ConfigureAwait(false);
            }

            if ((identifier == ItemConstants.Dinorant || identifier == ItemConstants.DarkHorse || identifier == ItemConstants.Fenrir)
                && (this.CurrentMap?.Definition.MapRequirements.Any(req => req.Attribute == Stats.CanFly) ?? false)
                && attributes[Stats.CanFly] < 1)
            {
                if (this.Inventory?.GetItem(InventoryConstants.PetSlot) is null
                    && this.GameContext.Configuration.Items.FirstOrDefault(i =>
                        i.Group == ItemConstants.Dinorant.Group && i.Number == ItemConstants.Dinorant.Number) is { } dinorantDef
                    && this.Inventory?.FindItemsByDefinition(dinorantDef).FirstOrDefault() is { } dinorantItem)
                {
                    await this._moveAction.MoveItemAsync(this, dinorantItem.ItemSlot, Storages.Inventory, InventoryConstants.PetSlot, Storages.Inventory).ConfigureAwait(false);
                    if (this.Inventory.GetItem(InventoryConstants.PetSlot) == dinorantItem)
                    {
                        await this.InvokeViewPlugInAsync<IUpdateInventoryListPlugIn>(p => p.UpdateInventoryListAsync()).ConfigureAwait(false);
                        await this.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.EquipmentHasChangedMessage)).ConfigureAwait(false);
                        return;
                    }
                }

                await this._movement.StopWalkingAsync().ConfigureAwait(false);
                await this.WarpToSafezoneAsync().ConfigureAwait(false);
            }
        }
    }

    private async ValueTask DecreaseWeaponDurabilityAfterHitAsync(float targetDefense, DamageType damageType)
    {
        var weapon = this.Inventory?.GetRandomWeapon(damageType);
        if (weapon is null || weapon.Durability == 0 || this.Attributes is not { } attributes)
        {
            return;
        }

        float itemDurationIncrease = attributes[Stats.WeaponAndArmorDurationIncrease];
        if (itemDurationIncrease == 0)
        {
            itemDurationIncrease = 1;
        }

        bool isMagicWeapon = false;
        AttributeDefinition weaponAttribute;
        if (weapon.IsWizardryWeapon(out _) && !weapon.CanHaveSkill())
        {
            isMagicWeapon = true;
            weaponAttribute = Stats.StaffRise;
        }
        else if (weapon.IsBook(out _))
        {
            isMagicWeapon = true;
            weaponAttribute = Stats.BookRise;
        }
        else if (attributes[Stats.HasDoubleWield] > 0 && weapon.ItemSlot == InventoryConstants.RightHandSlot)
        {
            weaponAttribute = Stats.MinPhysBaseDmgByRightWeapon;
        }
        else
        {
            weaponAttribute = Stats.MinimumPhysBaseDmgByWeapon;
        }

        if (!attributes.ItemPowerUps.TryGetValue(weapon, out var itemPowerUps))
        {
            return;
        }

        var weaponAttributeValue = attributes.GetComposableAttribute(weaponAttribute)?.Elements
            .Where(e => e.AggregateType == AggregateType.AddRaw && itemPowerUps.Contains(e))
            .Sum(e => e.Value) ?? 0;

        int defenseFactor = 1;
        double weaponDamageFactor;
        double hitsPerOneItemDurability = this.GameContext.Configuration.HitsPerOneItemDurability;
        if (isMagicWeapon)
        {
            weaponDamageFactor = 1.33 * weaponAttributeValue;
            hitsPerOneItemDurability *= 1056f / 564;
        }
        else
        {
            defenseFactor = 2;
            weaponDamageFactor = 1.5 * weaponAttributeValue;

            if (attributes[Stats.ArcheryAttackMode] > 0)
            {
                hitsPerOneItemDurability *= 780f / 564;
            }
        }

        if (weaponDamageFactor == 0)
        {
            return;
        }

        var defenseToDamageRatio = targetDefense * defenseFactor / weaponDamageFactor;
        if (weapon.ItemOptions.FirstOrDefault(o => o.ItemOption?.PowerUpDefinition?.TargetAttribute == Stats.WeaponDurationIncrease) is { } opt
            && opt.ItemOption?.LevelDependentOptions.FirstOrDefault(o => o.Level == opt.Level)?.PowerUpDefinition is { } pu
            && pu.Boost?.ConstantValue is { } socketInc)
        {
            defenseToDamageRatio -= defenseToDamageRatio * socketInc.Value;
        }

        var decrement = defenseToDamageRatio / (hitsPerOneItemDurability * itemDurationIncrease);
        await this.DecreaseItemDurabilityAsync(weapon, decrement).ConfigureAwait(false);
    }

    private async ValueTask CloseTradeIfNeededAsync()
    {
        if (this.PlayerState.CurrentState == GameLogic.PlayerState.TradeRequested
            || this.PlayerState.CurrentState == GameLogic.PlayerState.TradeButtonPressed
            || this.PlayerState.CurrentState == GameLogic.PlayerState.TradeOpened)
        {
            var cancelAction = new TradeCancelAction();
            await cancelAction.CancelTradeAsync(this).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Ends the collection which <see cref="CollectAttributeChangesUnderHitLock"/> started, before the hit lock is released.
    /// </summary>
    private sealed class HitAttributeChangeCollection : IDisposable
    {
        private readonly Player _player;

        /// <summary>
        /// Initializes a new instance of the <see cref="HitAttributeChangeCollection"/> class.
        /// </summary>
        /// <param name="player">The collecting player.</param>
        public HitAttributeChangeCollection(Player player) => this._player = player;

        /// <inheritdoc />
        public void Dispose() => this._player._hitAttributeChanges = null;
    }
}
