// <copyright file="Player.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.PlugIns;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.GameLogic.Views.Messenger;
    using MUnique.OpenMU.GameLogic.Views.Quest;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Pathfinding;
    using MUnique.OpenMU.Persistence;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The base implementation of a player.
    /// </summary>
    public class Player : IBucketMapObserver, IAttackable, IAttacker, ITrader, IPartyMember, IRotatable, IHasBucketInformation, IDisposable, ISupportWalk, IMovable
    {
        private readonly object moveLock = new object();

        private readonly Walker walker;

        private readonly AppearanceDataAdapter appearanceData;

        private readonly ObserverToWorldViewAdapter observerToWorldViewAdapter;

        private CancellationToken respawnAfterDeathToken;

        private Character selectedCharacter;

        private ICustomPlugInContainer<IViewPlugIn> viewPlugIns;

        private DateTime lastRegenerate = DateTime.UtcNow;

        private GameMap currentMap;

        private IDisposable characterLoggingScope;

        private IDisposable accountLoggingScope;
        private Account account;

        /// <summary>
        /// Initializes a new instance of the <see cref="Player" /> class.
        /// </summary>di
        /// <param name="gameContext">The game context.</param>
        public Player(IGameContext gameContext)
        {
            this.GameContext = gameContext;
            this.Logger = gameContext.LoggerFactory.CreateLogger<Player>();
            this.PersistenceContext = this.GameContext.PersistenceContextProvider.CreateNewPlayerContext(gameContext.Configuration);
            this.walker = new Walker(this, this.GetStepDelay);

            this.MagicEffectList = new MagicEffectsList(this);
            this.appearanceData = new AppearanceDataAdapter(this);
            this.PlayerEnteredWorld += this.OnPlayerEnteredWorld;
            this.PlayerState.StateChanged += (sender, args) => this.GameContext.PlugInManager.GetPlugInPoint<IPlayerStateChangedPlugIn>()?.PlayerStateChanged(this);
            this.PlayerState.StateChanges += (sender, args) => this.GameContext.PlugInManager.GetPlugInPoint<IPlayerStateChangingPlugIn>()?.PlayerStateChanging(this, args);
            this.observerToWorldViewAdapter = new ObserverToWorldViewAdapter(this, this.InfoRange);
        }

        /// <summary>
        /// Occurs when the player has or got disconnected from the game.
        /// </summary>
        public event EventHandler PlayerDisconnected;

        /// <summary>
        /// Occurs when the player entered the world with his selected character.
        /// </summary>
        public event EventHandler PlayerEnteredWorld;

        /// <summary>
        /// Occurs when the player left the world with his selected character.
        /// </summary>
        public event EventHandler PlayerLeftWorld;

        /// <summary>
        /// Gets the logger of this player.
        /// </summary>
        public ILogger<Player> Logger { get; }

        /// <inheritdoc />
        public bool IsWalking => this.walker.CurrentTarget != default;

        /// <inheritdoc />
        public TimeSpan StepDelay => this.GetStepDelay();

        /// <inheritdoc />
        public Point WalkTarget => this.walker.CurrentTarget;

        /// <inheritdoc/>
        public int Money
        {
            get => this.SelectedCharacter?.Inventory.Money ?? 0;

            set
            {
                if (this.SelectedCharacter != null && this.SelectedCharacter.Inventory.Money != value)
                {
                    this.SelectedCharacter.Inventory.Money = value;
                    this.ViewPlugIns.GetPlugIn<IUpdateMoneyPlugIn>()?.UpdateMoney();
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
        public string Name => this.SelectedCharacter.Name;

        /// <inheritdoc/>
        public int Level => (int)this.Attributes[Stats.Level];

        /// <summary>
        /// Gets or sets the selected character.
        /// </summary>
        public Character SelectedCharacter
        {
            get => this.selectedCharacter;

            set
            {
                if (this.selectedCharacter == value)
                {
                    return;
                }

                if (value is null)
                {
                    this.appearanceData.RaiseAppearanceChanged();
                    this.PlayerLeftWorld?.Invoke(this, null);
                    this.selectedCharacter = null;
                    this.characterLoggingScope?.Dispose();
                    this.characterLoggingScope = null;
                }
                else
                {
                    this.selectedCharacter = value;
                    this.characterLoggingScope = this.Logger.BeginScope("Character: {Name}", this.selectedCharacter.Name);
                    this.PlayerEnteredWorld?.Invoke(this, null);
                    this.appearanceData.RaiseAppearanceChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the pose of the currently selected character.
        /// </summary>
        public CharacterPose Pose
        {
            get => this.selectedCharacter?.Pose ?? default;

            set
            {
                var character = this.selectedCharacter;
                if (character is null || character.Pose == this.Pose)
                {
                    return;
                }

                character.Pose = value;
                this.appearanceData.RaiseAppearanceChanged();
            }
        }

        /// <summary>
        /// Gets or sets the account.
        /// </summary>
        public Account Account
        {
            get => this.account;
            set
            {
                if (this.account != value)
                {
                    this.account = value;
                    this.accountLoggingScope?.Dispose();
                    this.accountLoggingScope = this.Logger.BeginScope("Account: {Name}", this.account.LoginName);
                    this.IsVaultLocked = !string.IsNullOrWhiteSpace(this.account.VaultPassword);
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
        public NonPlayerCharacter OpenedNpc { get; set; }

        /// <inheritdoc/>
        public StateMachine PlayerState { get; } = new StateMachine(GameLogic.PlayerState.Initial);

        // TODO: TradeContext-object?

        /// <inheritdoc/>
        public ITrader TradingPartner { get; set; }

        /// <inheritdoc/>
        public int TradingMoney { get; set; }

        /// <inheritdoc/>
        public GameMap CurrentMap
        {
            get => this.currentMap;

            private set
            {
                if (this.currentMap != value)
                {
                    this.currentMap = value;
                    this.GameContext.PlugInManager?.GetPlugInPoint<IAttackableMovedPlugIn>()?.AttackableMoved(this);
                }
            }
        }

        /// <inheritdoc/>
        public ISet<IWorldObserver> Observers { get; } = new HashSet<IWorldObserver>();

        /// <inheritdoc/>
        public ReaderWriterLockSlim ObserverLock { get; } = new ReaderWriterLockSlim();

        /// <inheritdoc/>
        public IPartyMember LastPartyRequester { get; set; }

        /// <summary>
        /// Gets the skill list.
        /// </summary>
        public ISkillList SkillList { get; private set; }

        /// <inheritdoc/>
        public GuildMemberStatus GuildStatus { get; set; }

        /// <inheritdoc/>
        public Direction Rotation { get; set; }

        /// <inheritdoc/>
        public Party Party { get; set; }

        /// <inheritdoc/>
        public bool Alive { get; set; }

        public DeathInformation LastDeath { get; private set; }

        /// <inheritdoc/>
        public Point Position
        {
            get => new Point(this.SelectedCharacter?.PositionX ?? 0, this.SelectedCharacter?.PositionY ?? 0);

            set
            {
                if (this.Position != value)
                {
                    this.SelectedCharacter.PositionX = value.X;
                    this.SelectedCharacter.PositionY = value.Y;
                    this.GameContext.PlugInManager?.GetPlugInPoint<IAttackableMovedPlugIn>()?.AttackableMoved(this);
                }
            }
        }

        /// <inheritdoc/>
        public uint MaximumHealth => (uint)this.Attributes[Stats.MaximumHealth];

        /// <inheritdoc/>
        public uint CurrentHealth => (uint)this.Attributes[Stats.CurrentHealth];

        /// <summary>
        /// Gets or sets a value indicating whether this player is online as friend, and shown as online in its friends friendlists.
        /// </summary>
        public bool OnlineAsFriend { get; set; } = true;

        /// <inheritdoc cref="IWorldObserver"/>
        public ICustomPlugInContainer<IViewPlugIn> ViewPlugIns => this.viewPlugIns ?? (this.viewPlugIns = this.CreateViewPlugInContainer());

        /// <inheritdoc/>
        public IInventoryStorage Inventory { get; private set; }

        /// <inheritdoc/>
        public IStorage TemporaryStorage { get; private set; }

        /// <summary>
        /// Gets or sets the vault.
        /// </summary>
        public IStorage Vault { get; set; }

        /// <summary>
        /// Gets or sets a flag, indicating if the vault of the player is currently locked by a pin.
        /// </summary>
        public bool IsVaultLocked { get; set; }

        /// <summary>
        /// Gets the shop storage.
        /// </summary>
        public IShopStorage ShopStorage { get; private set; }

        /// <inheritdoc/>
        public BackupItemStorage BackupInventory { get; set; }

        /// <summary>
        /// Gets the appearance data.
        /// </summary>
        public IAppearanceData AppearanceData => this.appearanceData;

        /// <summary>
        /// Gets the game context.
        /// </summary>
        public IGameContext GameContext { get; }

        /// <inheritdoc/>
        public IList<Bucket<ILocateable>> ObservingBuckets => this.observerToWorldViewAdapter.ObservingBuckets;

        /// <inheritdoc/>
        public int InfoRange => this.GameContext.Configuration.InfoRange;

        /// <summary>
        /// Gets or sets the last guild requester.
        /// </summary>
        public Player LastGuildRequester { get; set; }

        /// <inheritdoc/>
        IAttributeSystem IAttackable.Attributes => this.Attributes;

        /// <inheritdoc/>
        IAttributeSystem IAttacker.Attributes => this.Attributes;

        /// <summary>
        /// Gets the attribute system.
        /// </summary>
        public ItemAwareAttributeSystem Attributes { get; private set; }

        /// <inheritdoc/>
        public Bucket<ILocateable> NewBucket { get; set; }

        /// <inheritdoc/>
        public Bucket<ILocateable> OldBucket { get; set; }

        /// <inheritdoc/>
        public void AttackBy(IAttacker attacker, SkillEntry skill)
        {
            var hitInfo = attacker.CalculateDamage(this, skill);

            if (hitInfo.HealthDamage == 0)
            {
                this.ViewPlugIns.GetPlugIn<IShowHitPlugIn>()?.ShowHit(this, hitInfo);
                (attacker as Player)?.ViewPlugIns.GetPlugIn<IShowHitPlugIn>()?.ShowHit(this, hitInfo);
                return;
            }

            attacker.ApplyAmmunitionConsumption(hitInfo);

            if (Rand.NextRandomBool(this.Attributes[Stats.FullyRecoverHealthAfterHitChance]))
            {
                this.Attributes[Stats.CurrentHealth] = this.Attributes[Stats.MaximumHealth];
                this.ViewPlugIns.GetPlugIn<IUpdateCurrentHealthPlugIn>()?.UpdateCurrentHealth();
            }

            if (Rand.NextRandomBool(this.Attributes[Stats.FullyRecoverManaAfterHitChance]))
            {
                this.Attributes[Stats.CurrentMana] = this.Attributes[Stats.MaximumMana];
                this.ViewPlugIns.GetPlugIn<IUpdateCurrentManaPlugIn>()?.UpdateCurrentMana();
            }

            this.Hit(hitInfo, attacker, skill?.Skill);

            (attacker as Player)?.AfterHitTarget();
        }

        /// <summary>
        /// Is called after the player successfully hit a target.
        /// </summary>
        public void AfterHitTarget()
        {
            this.Attributes[Stats.CurrentHealth] = Math.Max(this.Attributes[Stats.CurrentHealth] - this.Attributes[Stats.HealthLossAfterHit], 1);
        }

        /// <inheritdoc/>
        public void ReflectDamage(IAttacker reflector, uint damage)
        {
            this.Hit(this.GetHitInfo(damage, DamageAttributes.Reflected, reflector), reflector, null);
        }

        /// <summary>
        /// Is called after the player killed a <see cref="Player"/>.
        /// Increment PK Level.
        /// </summary>
        public void AfterKilledPlayer()
        {
            // TODO: Self Defense System
            if (this.selectedCharacter.State != HeroState.PlayerKiller2ndStage)
            {
                if (this.selectedCharacter.State < HeroState.Normal)
                {
                    this.selectedCharacter.State = HeroState.PlayerKillWarning;
                }
                else
                {
                    this.selectedCharacter.State++;
                }
            }

            this.selectedCharacter.StateRemainingSeconds = this.selectedCharacter.StateRemainingSeconds + (int)TimeSpan.FromHours(1).TotalSeconds;
            this.selectedCharacter.PlayerKillCount += 1;
            this.ForEachWorldObserver(o => o.ViewPlugIns.GetPlugIn<IUpdateCharacterHeroStatePlugIn>()?.UpdateCharacterHeroState(this), true);
        }

        /// <summary>
        /// Is called after the player killed a <see cref="Monster"/>.
        /// Adds recovered mana and health to the players attributes.
        /// </summary>
        public void AfterKilledMonster()
        {
            foreach (var recoverAfterMonsterKill in Stats.AfterMonsterKillRegenerationAttributes)
            {
                var additionalValue = (uint)((this.Attributes[recoverAfterMonsterKill.RegenerationMultiplier] * this.Attributes[recoverAfterMonsterKill.MaximumAttribute]) + this.Attributes[recoverAfterMonsterKill.AbsoluteAttribute]);
                this.Attributes[recoverAfterMonsterKill.CurrentAttribute] = (uint)Math.Min(this.Attributes[recoverAfterMonsterKill.MaximumAttribute], this.Attributes[recoverAfterMonsterKill.CurrentAttribute] + additionalValue);
            }

            this.ViewPlugIns.GetPlugIn<IUpdateCurrentManaPlugIn>()?.UpdateCurrentMana();
            this.ViewPlugIns.GetPlugIn<IUpdateCurrentHealthPlugIn>()?.UpdateCurrentHealth();
        }

        /// <summary>
        /// Resets the appearance cache.
        /// </summary>
        public void OnAppearanceChanged() => this.appearanceData.RaiseAppearanceChanged();

        /// <summary>
        /// Determines whether the player complies with the requirements of the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>True</c>, if the player complies with the requirements of the specified item; Otherwise, <c>false</c>.</returns>
        public bool CompliesRequirements(Item item)
        {
            foreach (var requirement in item.Definition.Requirements.Select(item.GetRequirement))
            {
                if (this.Attributes[requirement.Item1] < requirement.Item2)
                {
                    return false;
                }
            }

            return item.Definition.QualifiedCharacters.Contains(this.SelectedCharacter.CharacterClass);
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
        /// Moves the player to the specified gate.
        /// </summary>
        /// <param name="gate">The gate to which the player should be moved.</param>
        public void WarpTo(ExitGate gate)
        {
            var currentMap = this.CurrentMap;
            if (currentMap is null)
            {
                return;
            }

            currentMap.Remove(this);
            this.Alive = false;
            this.walker.Stop();
            this.observerToWorldViewAdapter.ClearObservingObjectsList();
            this.SelectedCharacter.PositionX = (byte)Rand.NextInt(gate.X1, gate.X2);
            this.SelectedCharacter.PositionY = (byte)Rand.NextInt(gate.Y1, gate.Y2);
            this.SelectedCharacter.CurrentMap = gate.Map;
            this.Rotation = gate.Direction;
            this.CurrentMap = null; // Will be set again, when the client acknowledged the map change by F3 12 packet.
            this.ViewPlugIns.GetPlugIn<IMapChangePlugIn>()?.MapChange();

            // after this, the Client will send us a F3 12 packet, to tell us it loaded
            // the map and is ready to receive the new meet player/monster etc.
            // Then ClientReadyAfterMapChange is called.
        }

        /// <summary>
        /// Signals that the game client of the player is ready after a map change (data has been loaded etc).
        /// On this event, the player enters the game map on the server side, and interacts with the other objects.
        /// </summary>
        /// <remarks>
        /// this method is called after the client sent us the F3 12 packet.
        /// </remarks>
        public void ClientReadyAfterMapChange()
        {
            this.CurrentMap = this.GameContext.GetMap(this.SelectedCharacter.CurrentMap.Number.ToUnsigned());
            this.PlayerState.TryAdvanceTo(GameLogic.PlayerState.EnteredWorld);
            this.Alive = true;
            this.CurrentMap.Add(this);
        }

        /// <summary>
        /// Adds experience points after killing the target object.
        /// </summary>
        /// <param name="killedObject">The killed object.</param>
        /// <returns>The gained experience.</returns>
        public int AddExpAfterKill(IAttackable killedObject)
        {
            // Calculate the Exp
            var experience = killedObject.Attributes[Stats.Level] * 1000 / this.Attributes[Stats.Level];
            experience = Rand.NextInt((int)(experience * 0.8), (int)(experience * 1.2));
            experience *= this.Attributes[Stats.ExperienceRate];
            experience *= this.GameContext.ExperienceRate;

            // todo: master exp
            this.AddExperience((int)experience, killedObject);
            return (int)experience;
        }

        /// <summary>
        /// Adds the experience.
        /// </summary>
        /// <param name="experience">The experience which should be added.</param>
        /// <param name="killedObject">The killed object which caused the experience gain.</param>
        public void AddExperience(int experience, IAttackable killedObject)
        {
            if (this.Attributes[Stats.Level] < this.GameContext.Configuration.MaximumLevel)
            {
                long exp = experience;

                // Add the Exp
                bool lvlup = false;
                var expTable = this.GameContext.Configuration.ExperienceTable;
                if (expTable[(int)this.Attributes[Stats.Level] + 1] - this.SelectedCharacter.Experience < exp)
                {
                    exp = expTable[(int)this.Attributes[Stats.Level] + 1] - this.SelectedCharacter.Experience;
                    lvlup = true;
                }

                this.SelectedCharacter.Experience += exp;

                // Tell it to the Player
                this.ViewPlugIns.GetPlugIn<IAddExperiencePlugIn>()?.AddExperience((int)exp, killedObject);

                // Check the lvl up
                if (lvlup)
                {
                    this.Attributes[Stats.Level]++;
                    this.SelectedCharacter.LevelUpPoints += (int)this.Attributes[Stats.PointsPerLevelUp];
                    this.SetReclaimableAttributesToMaximum();
                    this.Logger.LogDebug("Character {0} leveled up to {1}", this.SelectedCharacter.Name, this.Attributes[Stats.Level]);
                    this.ViewPlugIns.GetPlugIn<IUpdateLevelPlugIn>()?.UpdateLevel();
                }
            }
            else
            {
                this.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("You already reached maximum Level.", MessageType.BlueNormal);
            }
        }

        /// <summary>
        /// Moves the player to the specified coordinate.
        /// </summary>
        /// <param name="target">The target.</param>
        public void Move(Point target)
        {
            this.Logger.LogDebug("Move: Player is moving to {0}", target);
            this.walker.Stop();
            this.CurrentMap.Move(this, target, this.moveLock, MoveType.Instant);
            this.Logger.LogDebug("Move: Observer Count: {0}", this.Observers.Count);
        }

        /// <summary>
        /// Walks to the specified target coordinates using the specified steps.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="steps">The steps.</param>
        public void WalkTo(Point target, Span<WalkingStep> steps)
        {
            var currentMap = this.CurrentMap;
            if (currentMap != null)
            {
                this.Logger.LogDebug("WalkTo: Player is walking to {0}", target);
                this.walker.WalkTo(target, steps);
                currentMap.Move(this, target, this.moveLock, MoveType.Walk);
                this.Logger.LogDebug("WalkTo: Observer Count: {0}", this.Observers.Count);
            }
        }

        /// <inheritdoc />
        public int GetDirections(Span<Direction> directions) => this.walker.GetDirections(directions);

        /// <inheritdoc />
        public int GetSteps(Span<WalkingStep> steps) => this.walker.GetSteps(steps);

        /// <summary>
        /// Regenerates the attributes specified in <see cref="Stats.IntervalRegenerationAttributes"/>.
        /// </summary>
        public void Regenerate()
        {
            try
            {
                foreach (var r in Stats.IntervalRegenerationAttributes.Where(r =>
                    this.Attributes[r.RegenerationMultiplier] > 0 || this.Attributes[r.AbsoluteAttribute] > 0))
                {
                    if (r.CurrentAttribute == Stats.CurrentShield && !this.IsAtSafezone() &&
                        this.Attributes[Stats.ShieldRecoveryEverywhere] < 1)
                    {
                        // Shield recovery is only possible at safe-zone, except the character has an specific attribute which has the effect that it's recovered everywhere.
                        // This attribute is usually provided by a level 380 armor and a Guardian Option.
                        continue;
                    }

                    this.Attributes[r.CurrentAttribute] = Math.Min(
                        this.Attributes[r.CurrentAttribute] +
                        ((this.Attributes[r.MaximumAttribute] * this.Attributes[r.RegenerationMultiplier]) +
                         this.Attributes[r.AbsoluteAttribute]),
                        this.Attributes[r.MaximumAttribute]);
                }

                this.ViewPlugIns.GetPlugIn<IUpdateCurrentHealthPlugIn>()?.UpdateCurrentHealth();
                this.ViewPlugIns.GetPlugIn<IUpdateCurrentManaPlugIn>()?.UpdateCurrentMana();

                this.RegenerateHeroState();
            }
            catch (InvalidOperationException)
            {
                // may happen after a character disconnected in the mean time.
            }
            finally
            {
                this.lastRegenerate = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Disconnects the player from the game. Remote connections will be closed and data will be saved.
        /// </summary>
        public void Disconnect()
        {
            if (this.PlayerState.TryAdvanceTo(GameLogic.PlayerState.Disconnected))
            {
                try
                {
                    this.InternalDisconnect();
                    if (this.PlayerDisconnected != null)
                    {
                        this.PlayerDisconnected(this, EventArgs.Empty);
                        this.PlayerDisconnected = null;
                    }
                }
                finally
                {
                    this.PlayerState.TryAdvanceTo(GameLogic.PlayerState.Finished);
                }
            }
        }

        /// <inheritdoc/>
        public void AddObserver(IWorldObserver observer)
        {
            this.ObserverLock.EnterWriteLock();
            try
            {
                this.Observers.Add(observer);
            }
            finally
            {
                this.ObserverLock.ExitWriteLock();
            }
        }

        /// <inheritdoc/>
        public void RemoveObserver(IWorldObserver observer)
        {
            this.ObserverLock.EnterWriteLock();
            try
            {
                this.Observers.Remove(observer);
            }
            finally
            {
                this.ObserverLock.ExitWriteLock();
            }
        }

        /// <inheritdoc/>
        public void LocateableAdded(object sender, BucketItemEventArgs<ILocateable> eventArgs)
        {
            this.observerToWorldViewAdapter.LocateableAdded(sender, eventArgs);
        }

        /// <inheritdoc/>
        public void LocateableRemoved(object sender, BucketItemEventArgs<ILocateable> eventArgs)
        {
            this.observerToWorldViewAdapter.LocateableRemoved(sender, eventArgs);
        }

        /// <inheritdoc/>
        public void LocateablesOutOfScope(IEnumerable<ILocateable> oldObjects)
        {
            this.observerToWorldViewAdapter.LocateablesOutOfScope(oldObjects);
        }

        /// <inheritdoc/>
        public void NewLocateablesInScope(IEnumerable<ILocateable> newObjects)
        {
            this.observerToWorldViewAdapter.NewLocateablesInScope(newObjects);
        }

        /// <summary>
        /// Tries to consume the <see cref="Skill.ConsumeRequirements"/> of a skill.
        /// </summary>
        /// <param name="skill">The skill which should get performed.</param>
        /// <returns>
        ///     <c>True</c>, if the <see cref="Skill.ConsumeRequirements"/> and <see cref="Skill.Requirements"/>
        ///     are being met, and the <see cref="Skill.ConsumeRequirements"/> have been consumed; Otherwise, <c>false</c>.
        /// </returns>
        public bool TryConsumeForSkill(Skill skill)
        {
            if (skill.Requirements.Any(r => r.MinimumValue > this.Attributes[r.Attribute]))
            {
                return false;
            }

            if (skill.ConsumeRequirements.Any(r => this.GetRequiredValue(r) > this.Attributes[r.Attribute]))
            {
                return false;
            }

            foreach (var requirement in skill.ConsumeRequirements)
            {
                this.Attributes[requirement.Attribute] -= this.GetRequiredValue(requirement);
            }

            this.ViewPlugIns.GetPlugIn<IUpdateCurrentManaPlugIn>()?.UpdateCurrentMana();
            return true;
        }

        /// <summary>
        /// Creates the magic effect power up for the given skill entry.
        /// </summary>
        /// <param name="skillEntry">The skill entry.</param>
        public void CreateMagicEffectPowerUp(SkillEntry skillEntry)
        {
            var skill = skillEntry.Skill;
            var powerUpDef = skill.MagicEffectDef.PowerUpDefinition;
            if (skillEntry.Level > 0)
            {
                var element = this.Attributes.CreateElement(powerUpDef.Boost);
                var additionalValue = new SimpleElement(skillEntry.CalculateValue(), element.AggregateType);
                skillEntry.BuffPowerUp = new CombinedElement(element, additionalValue);
            }
            else
            {
                skillEntry.BuffPowerUp = this.Attributes.CreateElement(powerUpDef.Boost);
            }

            skillEntry.PowerUpDuration = this.Attributes.CreateElement(powerUpDef.Duration);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            string accountName = string.Empty;
            string characterName = string.Empty;
            if (this.Account != null)
            {
                accountName = this.Account.LoginName;
                if (this.selectedCharacter != null)
                {
                    characterName = this.selectedCharacter.Name;
                }
            }

            return $"Account: [{accountName}], Character:[{characterName}]";
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="managed"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "<ObserverLock>k__BackingField", Justification = "Can't access backing field.")]
        protected virtual void Dispose(bool managed)
        {
            if (managed)
            {
                this.PersistenceContext.Dispose();
                if (this.CurrentMap != null)
                {
                    this.CurrentMap.Remove(this);
                    this.CurrentMap = null;
                }

                this.observerToWorldViewAdapter.ClearObservingObjectsList();
                this.observerToWorldViewAdapter.Dispose();
                this.walker.Dispose();
                this.ObserverLock.Dispose();
            }
        }

        /// <summary>
        /// Is called, when <see cref="Disconnect"/> is called.
        /// </summary>
        protected virtual void InternalDisconnect()
        {
            if (this.respawnAfterDeathToken.CanBeCanceled && !this.respawnAfterDeathToken.IsCancellationRequested)
            {
                this.respawnAfterDeathToken.ThrowIfCancellationRequested();
                this.WarpTo(this.GetSpawnGateOfCurrentMap());
            }
        }

        /// <summary>
        /// Creates the view plug in container.
        /// </summary>
        /// <returns>The created view plug in container.</returns>
        protected virtual ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer()
        {
            return null;
        }

        private void RegenerateHeroState()
        {
            var currentCharacter = this.selectedCharacter;
            if (currentCharacter?.StateRemainingSeconds > 0)
            {
                var secondsSinceLastRegenerate = this.lastRegenerate.Subtract(DateTime.UtcNow).TotalSeconds;
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

                    this.ForEachWorldObserver(o => o.ViewPlugIns.GetPlugIn<IUpdateCharacterHeroStatePlugIn>()?.UpdateCharacterHeroState(this), true);
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
            if (this.Inventory.EquippedItems.Any(item => item.Definition.ItemSlot.ItemSlots.Contains(7)))
            {
                // Wings
                return TimeSpan.FromMilliseconds(300);
            }

            // TODO: Consider pets etc.
            return TimeSpan.FromMilliseconds(500);
        }

        private ExitGate GetSpawnGateOfCurrentMap()
        {
            var spawnTargetMap = this.CurrentMap.Definition.SafezoneMap ?? this.CurrentMap.Definition;
            var terrain = this.CurrentMap.Definition == spawnTargetMap ? this.CurrentMap.Terrain : new GameMapTerrain(spawnTargetMap);
            var safeSpawn = spawnTargetMap.ExitGates.Where(g => g.IsSpawnGate && terrain.SafezoneMap[g.X1, g.Y1]).SelectRandom()
                                ?? spawnTargetMap.ExitGates.Where(g => g.IsSpawnGate).SelectRandom();
            return safeSpawn;
        }

        private void Hit(HitInfo hitInfo, IAttacker attacker, Skill skill)
        {
            int oversd = (int)(this.Attributes[Stats.CurrentShield] - hitInfo.ShieldDamage);
            if (oversd < 0)
            {
                this.Attributes[Stats.CurrentShield] = 0;
                hitInfo.HealthDamage += (uint)(oversd * (-1));
            }
            else
            {
                this.Attributes[Stats.CurrentShield] = oversd;
            }

            this.Attributes[Stats.CurrentHealth] -= hitInfo.HealthDamage;
            this.LastDeath = new DeathInformation(attacker.Id, attacker.GetName(), hitInfo, skill?.Number ?? 0);
            this.ViewPlugIns.GetPlugIn<IShowHitPlugIn>()?.ShowHit(this, hitInfo);
            (attacker as Player)?.ViewPlugIns.GetPlugIn<IShowHitPlugIn>()?.ShowHit(this, hitInfo);
            this.GameContext.PlugInManager.GetPlugInPoint<IAttackableGotHitPlugIn>()?.AttackableGotHit(this, attacker, hitInfo);

            if (this.Attributes[Stats.CurrentHealth] < 1)
            {
                this.OnDeath(attacker);
            }

            var reflectPercentage = this.Attributes[Stats.DamageReflection];
            if (reflectPercentage > 0 && attacker is IAttackable attackableAttacker)
            {
                var reflectedDamage = (int)((hitInfo.HealthDamage + hitInfo.ShieldDamage) * reflectPercentage);
                if (reflectedDamage == 0)
                {
                    return;
                }

                Task.Delay(500).ContinueWith(task =>
                {
                    if (attackableAttacker.Alive)
                    {
                        attackableAttacker.ReflectDamage(this, (uint)reflectedDamage);
                    }
                });
            }
        }

        private void OnDeath(IAttacker killer)
        {
            if (!this.PlayerState.TryAdvanceTo(GameLogic.PlayerState.Dead))
            {
                return;
            }

            this.walker.Stop();
            this.Alive = false;
            this.respawnAfterDeathToken = default;
            this.ForEachObservingPlayer(p => p.ViewPlugIns.GetPlugIn<IObjectGotKilledPlugIn>()?.ObjectGotKilled(this, killer), true);

            if (killer is Player killerAfterKilled)
            {
                killerAfterKilled.AfterKilledPlayer();
            }

            // TODO: Drop items
            Task.Run(
              async () =>
              {
                  await Task.Delay(3000, this.respawnAfterDeathToken).ConfigureAwait(false);
                  this.SetReclaimableAttributesToMaximum();
                  this.WarpTo(this.GetSpawnGateOfCurrentMap());
              },
              this.respawnAfterDeathToken);

            this.GameContext.PlugInManager.GetPlugInPoint<IAttackableGotKilledPlugIn>()?.AttackableGotKilled(this, killer);
        }

        private Item GetAmmunitionItem()
        {
            if (this.Inventory.GetItem(InventoryConstants.LeftHandSlot) is { } leftItem
                && leftItem.Definition.IsAmmunition)
            {
                return leftItem;
            }

            if (this.Inventory.GetItem(InventoryConstants.RightHandSlot) is { } rightItem
                && rightItem.Definition.IsAmmunition)
            {
                return rightItem;
            }

            return null;
        }

        private void OnPlayerEnteredWorld(object sender, EventArgs e)
        {
            this.Attributes = new ItemAwareAttributeSystem(this.SelectedCharacter);
            this.Inventory = new InventoryStorage(this, this.GameContext);
            this.ShopStorage = new ShopStorage(this);
            this.TemporaryStorage = new Storage(InventoryConstants.TemporaryStorageSize, new TemporaryItemStorage());
            this.Vault = null; // vault storage is getting set when vault npc is opened.
            this.SkillList = new SkillList(this);
            this.SetReclaimableAttributesBeforeEnterGame();
            this.ViewPlugIns.GetPlugIn<ISkillListViewPlugIn>()?.UpdateSkillList();
            this.ViewPlugIns.GetPlugIn<IUpdateCharacterStatsPlugIn>()?.UpdateCharacterStats();
            this.ViewPlugIns.GetPlugIn<IUpdateInventoryListPlugIn>()?.UpdateInventoryList();
            this.ViewPlugIns.GetPlugIn<IQuestStateResponsePlugIn>()?.ShowQuestState(null); // Legacy quest system
            this.ViewPlugIns.GetPlugIn<ICurrentlyActiveQuestsPlugIn>()?.ShowActiveQuests(); // New quest system

            this.Attributes.GetOrCreateAttribute(Stats.MaximumMana).ValueChanged += (a, b) => this.OnMaximumManaOrAbilityChanged();
            this.Attributes.GetOrCreateAttribute(Stats.MaximumAbility).ValueChanged += (a, b) => this.OnMaximumManaOrAbilityChanged();
            this.Attributes.GetOrCreateAttribute(Stats.MaximumHealth).ValueChanged += (a, b) => this.OnMaximumHealthOrShieldChanged();
            this.Attributes.GetOrCreateAttribute(Stats.MaximumShield).ValueChanged += (a, b) => this.OnMaximumHealthOrShieldChanged();
            var ammoAttribute = this.Attributes.GetOrCreateAttribute(Stats.AmmunitionAmount);
            this.Attributes[Stats.AmmunitionAmount] = this.GetAmmunitionItem()?.Durability ?? 0;
            ammoAttribute.ValueChanged += (a, b) => this.OnAmmunitionAmountChanged();

            this.ClientReadyAfterMapChange();

            this.ViewPlugIns.GetPlugIn<IUpdateRotationPlugIn>()?.UpdateRotation();
            Task.Delay(1000).ContinueWith(_ =>
            {
                try
                {
                    this.ViewPlugIns.GetPlugIn<IInitializeMessengerPlugIn>()?.InitializeMessenger(this.GameContext.Configuration.MaximumLetters);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, "Error occured when initializing the messenger.");
                }
            });
        }

        /// <summary>
        /// Sets the reclaimable attributes before a character enters the game.
        /// Current shield and mana is set to their maximum values.
        /// Current ability starts at the half of the maximum (as at the original server).
        /// The current health value was restored from the previous session and is not set to the maximum value - it's just limited by the maximum value.
        /// </summary>
        private void SetReclaimableAttributesBeforeEnterGame()
        {
            this.Attributes[Stats.CurrentShield] = this.Attributes[Stats.MaximumShield];
            this.Attributes[Stats.CurrentMana] = this.Attributes[Stats.MaximumMana];
            this.Attributes[Stats.CurrentAbility] = this.Attributes[Stats.MaximumAbility] / 2;
            this.Attributes[Stats.CurrentHealth] = Math.Min(this.Attributes[Stats.CurrentHealth], this.Attributes[Stats.MaximumHealth]);
        }

        private void SetReclaimableAttributesToMaximum()
        {
            foreach (var regeneration in Stats.IntervalRegenerationAttributes)
            {
                this.Attributes[regeneration.CurrentAttribute] = this.Attributes[regeneration.MaximumAttribute];
            }
        }

        private void OnMaximumHealthOrShieldChanged()
        {
            this.Attributes[Stats.CurrentHealth] = Math.Min(this.Attributes[Stats.CurrentHealth], this.Attributes[Stats.MaximumHealth]);
            this.Attributes[Stats.CurrentShield] = Math.Min(this.Attributes[Stats.CurrentShield], this.Attributes[Stats.MaximumShield]);
            this.ViewPlugIns.GetPlugIn<IUpdateMaximumHealthPlugIn>()?.UpdateMaximumHealth();
            this.ViewPlugIns.GetPlugIn<IUpdateCurrentHealthPlugIn>()?.UpdateCurrentHealth();
        }

        private void OnAmmunitionAmountChanged()
        {
            var value = Math.Max((byte)this.Attributes[Stats.AmmunitionAmount], (byte)0);
            if (this.GetAmmunitionItem() is { } ammoItem
                && ammoItem.Durability != value)
            {
                ammoItem.Durability = value;
                if (ammoItem.Durability == 0)
                {
                    this.Inventory.RemoveItem(ammoItem);
                    this.ViewPlugIns.GetPlugIn<IItemRemovedPlugIn>()?.RemoveItem(ammoItem.ItemSlot);
                    this.GameContext.PlugInManager.GetPlugInPoint<IItemDestroyedPlugIn>()?.ItemDestroyed(ammoItem);
                }
                else
                {
                    this.ViewPlugIns.GetPlugIn<IItemDurabilityChangedPlugIn>()?.ItemDurabilityChanged(ammoItem, false);
                }
            }
        }

        private void OnMaximumManaOrAbilityChanged()
        {
            this.Attributes[Stats.CurrentMana] = Math.Min(this.Attributes[Stats.CurrentMana], this.Attributes[Stats.MaximumMana]);
            this.Attributes[Stats.CurrentAbility] = Math.Min(this.Attributes[Stats.CurrentAbility], this.Attributes[Stats.MaximumAbility]);
            this.ViewPlugIns.GetPlugIn<IUpdateMaximumManaPlugIn>()?.UpdateMaximumMana();
            this.ViewPlugIns.GetPlugIn<IUpdateCurrentManaPlugIn>()?.UpdateCurrentMana();
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
            private readonly Player player;
            private bool? fullAncientSetEquipped;

            public AppearanceDataAdapter(Player player)
            {
                this.player = player;
            }

            public event EventHandler AppearanceChanged;

            public CharacterClass CharacterClass => this.player.SelectedCharacter?.CharacterClass;

            public CharacterPose Pose => this.player.SelectedCharacter?.Pose ?? default;

            public bool FullAncientSetEquipped => (this.fullAncientSetEquipped ?? (this.fullAncientSetEquipped = this.player.SelectedCharacter.HasFullAncientSetEquipped())).Value;

            public IEnumerable<ItemAppearance> EquippedItems
            {
                get
                {
                    if (this.player.Inventory != null)
                    {
                        return this.player.Inventory.EquippedItems.Select(item => item.GetAppearance());
                    }

                    return Enumerable.Empty<ItemAppearance>();
                }
            }

            /// <summary>
            /// Raises the <see cref="AppearanceChanged"/> event.
            /// </summary>
            public void RaiseAppearanceChanged()
            {
                this.fullAncientSetEquipped = null;
                this.AppearanceChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
