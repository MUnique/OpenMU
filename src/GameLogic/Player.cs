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
    using log4net;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Pathfinding;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// The character pose.
    /// TODO: Use it.
    /// </summary>
    public enum CharacterPose : byte
    {
        /// <summary>
        /// The character is standing (normal).
        /// </summary>
        Standing = 0,

        /// <summary>
        /// The character is sitting on an object.
        /// </summary>
        Sitting = 2,

        /// <summary>
        /// The character is leaning towards something (wall etc).
        /// </summary>
        Leaning = 3,

        /// <summary>
        /// The character is hanging on something.
        /// </summary>
        Hanging = 4
    }

    /// <summary>
    /// The base implementation of a player.
    /// </summary>
    public class Player : IBucketMapObserver, IAttackable, ITrader, IPartyMember, IRotatable, IHasBucketInformation, IDisposable, ISupportWalk
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Player));

        private readonly IGameContext gameContext;

        private readonly object moveLock = new object();

        private readonly Walker walker;

        private readonly AppearanceDataAdapter appearanceData;

        private ObserverToWorldViewAdapter observerToWorldViewAdapter;

        private CancellationToken respawnAfterDeathToken;

        private Character selectedCharacter;

        private IPlayerView playerView;

        /// <summary>
        /// Initializes a new instance of the <see cref="Player" /> class.
        /// </summary>di
        /// <param name="gameContext">The game context.</param>
        /// <param name="playerView">The player view.</param>
        public Player(IGameContext gameContext, IPlayerView playerView)
            : this()
        {
            this.gameContext = gameContext;
            this.PlayerView = playerView;
            this.PersistenceContext = this.gameContext.PersistenceContextProvider.CreateNewPlayerContext(gameContext.Configuration);
            this.walker = new Walker(this, this.GetStepDelay);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        protected Player()
        {
            this.MagicEffectList = new MagicEffectsList(this);
            this.appearanceData = new AppearanceDataAdapter(this);
            this.PlayerEnteredWorld += this.OnPlayerEnteredWorld;
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

        /// <inheritdoc />
        public bool IsWalking => this.walker.CurrentTarget != default;

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
                    this.PlayerView.InventoryView.UpdateMoney();
                }
            }
        }

        /// <inheritdoc/>
        public IWorldView WorldView => this.PlayerView.WorldView;

        /// <summary>
        /// Gets the persistence context.
        /// </summary>
        public IPlayerContext PersistenceContext { get; }

        /// <inheritdoc />
        public ITradeView TradeView => this.PlayerView.TradeView;

        /// <inheritdoc/>
        public ushort Id { get; set; }

        /// <inheritdoc/>
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
                if (value == null)
                {
                    this.appearanceData.RaiseAppearanceChanged();
                    this.PlayerLeftWorld?.Invoke(this, null);
                    this.selectedCharacter = null;
                }
                else
                {
                    this.selectedCharacter = value;
                    this.PlayerEnteredWorld?.Invoke(this, null);
                    this.appearanceData.RaiseAppearanceChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the account.
        /// </summary>
        public Account Account { get; set; }

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
        public GameMap CurrentMap { get; private set; }

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

        /// <inheritdoc />
        public uint LastReceivedDamage { get; private set; }

        /// <inheritdoc/>
        public Point Position
        {
            get => new Point(this.SelectedCharacter?.PositionX ?? 0, this.SelectedCharacter?.PositionY ?? 0);

            set
            {
                this.SelectedCharacter.PositionX = value.X;
                this.SelectedCharacter.PositionY = value.Y;
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

        /// <summary>
        /// Gets or sets the player view.
        /// </summary>
        public IPlayerView PlayerView
        {
            get => this.playerView;

            protected set
            {
                this.playerView = value;
                if (this.playerView != null)
                {
                    this.observerToWorldViewAdapter = new ObserverToWorldViewAdapter(this, this.InfoRange);
                }
            }
        }

        /// <inheritdoc/>
        public IPartyView PartyView => this.PlayerView.PartyView;

        /// <inheritdoc/>
        public IStorage Inventory { get; private set; }

        /// <inheritdoc/>
        public IStorage TemporaryStorage { get; private set; }

        /// <summary>
        /// Gets or sets the vault.
        /// </summary>
        public IStorage Vault { get; set; }

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
        public IGameContext GameContext => this.gameContext;

        /// <inheritdoc/>
        public IList<Bucket<ILocateable>> ObservingBuckets => this.observerToWorldViewAdapter.ObservingBuckets;

        /// <inheritdoc/>
        public int InfoRange => this.gameContext.Configuration.InfoRange;

        /// <summary>
        /// Gets or sets the last guild requester.
        /// </summary>
        public Player LastGuildRequester { get; set; }

        /// <inheritdoc/>
        IAttributeSystem IAttackable.Attributes => this.Attributes;

        /// <summary>
        /// Gets the attribute system.
        /// </summary>
        public ItemAwareAttributeSystem Attributes { get; private set; }

        /// <inheritdoc/>
        public Bucket<ILocateable> NewBucket { get; set; }

        /// <inheritdoc/>
        public Bucket<ILocateable> OldBucket { get; set; }

        /// <inheritdoc/>
        public void AttackBy(IAttackable attacker, SkillEntry skill)
        {
            var hitInfo = attacker.CalculateDamage(this, skill);

            if (hitInfo.HealthDamage == 0)
            {
                this.PlayerView.ShowHit(this, hitInfo);
                (attacker as Player)?.PlayerView.ShowHit(this, hitInfo);
                return;
            }

            if (Rand.NextRandomBool(this.Attributes[Stats.FullyRecoverHealthAfterHitChance]))
            {
                this.Attributes[Stats.CurrentHealth] = this.Attributes[Stats.MaximumHealth];
                this.PlayerView.UpdateCurrentManaAndHp();
            }

            if (Rand.NextRandomBool(this.Attributes[Stats.FullyRecoverManaAfterHitChance]))
            {
                this.Attributes[Stats.CurrentMana] = this.Attributes[Stats.MaximumMana];
                this.PlayerView.UpdateCurrentManaAndHp();
            }

            this.Hit(hitInfo, attacker);

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
        public void ReflectDamage(IAttackable reflector, uint damage)
        {
            this.Hit(this.GetHitInfo(damage, DamageAttributes.Reflected, reflector), reflector);
        }

        /// <summary>
        /// Is called after the player killed a <see cref="Monster"/>.
        /// Adds recovered mana and health to the players attributes.
        /// </summary>
        public void AfterKilledMonster()
        {
            foreach (var recoverAfterMonsterKill in Stats.AfterMonsterKillRegenerationAttributes)
            {
                var additionalValue = (uint)(this.Attributes[recoverAfterMonsterKill.RegenerationMultiplier] * this.Attributes[recoverAfterMonsterKill.MaximumAttribute]);
                this.Attributes[recoverAfterMonsterKill.CurrentAttribute] = (uint)Math.Min(this.Attributes[recoverAfterMonsterKill.MaximumAttribute], this.Attributes[recoverAfterMonsterKill.CurrentAttribute] + additionalValue);
            }

            this.PlayerView.UpdateCurrentManaAndHp();
        }

        /// <summary>
        /// Resets the appearance cache.
        /// </summary>
        public void OnAppearanceChanged() => this.appearanceData.RaiseAppearanceChanged();

        /// <summary>
        /// Determines whether the player complies with the requirements of the specified item.
        /// </summary>
        /// <param name="item">The definition of the item.</param>
        /// <returns><c>True</c>, if the player complies with the requirements of the specified item; Otherwise, <c>false</c>.</returns>
        public bool CompliesRequirements(ItemDefinition item)
        {
            foreach (var requirement in item.Requirements)
            {
                // TODO: Added Requirements of additional Levels and Options
                if (this.Attributes[requirement.Attribute] < requirement.MinimumValue)
                {
                    return false;
                }
            }

            return item.QualifiedCharacters.Contains(this.SelectedCharacter.CharacterClass);
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
        /// Tries to add the money from the players inventory.
        /// </summary>
        /// <param name="value">The value which should be added.</param>
        /// <returns><c>True</c>, if the players inventory had space to add money; Otherwise, <c>false</c>.</returns>
        public virtual bool TryAddMoney(int value)
        {
            if (this.Money + value > this.gameContext?.Configuration?.MaximumInventoryMoney)
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
            if (currentMap == null)
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
            this.CurrentMap = null; // Will be set again, when the client acknoledged the map change by F3 12 packet.
            this.PlayerView.WorldView.MapChange();
            this.Rotation = gate.Direction;

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
            this.Attributes[Stats.CurrentShield] = this.Attributes[Stats.MaximumShield];
            this.Attributes[Stats.CurrentAbility] = this.Attributes[Stats.MaximumAbility] / 2;
            this.CurrentMap = this.gameContext.GetMap(this.SelectedCharacter.CurrentMap.Number.ToUnsigned());
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
            int inexp = (int)killedObject.Attributes[Stats.Level] * 1000 / (int)this.Attributes[Stats.Level];
            inexp = Rand.NextInt((int)(inexp * 0.8), (int)(inexp * 1.2));

            // todo: master exp
            this.AddExperience(inexp, killedObject);
            return inexp;
        }

        /// <summary>
        /// Adds the experience.
        /// </summary>
        /// <param name="experience">The experience which should be added.</param>
        /// <param name="killedObject">The killed object which caused the experience gain.</param>
        public void AddExperience(int experience, IIdentifiable killedObject)
        {
            if (this.Attributes[Stats.Level] < this.gameContext.Configuration.MaximumLevel)
            {
                long exp = experience;

                // Add the Exp
                bool lvlup = false;
                var expTable = this.gameContext.Configuration.ExperienceTable;
                if (expTable[(int)this.Attributes[Stats.Level] + 1] - this.SelectedCharacter.Experience < exp)
                {
                    exp = expTable[(int)this.Attributes[Stats.Level] + 1] - this.SelectedCharacter.Experience;
                    lvlup = true;
                }

                this.SelectedCharacter.Experience += exp;

                // Tell it to the Player
                this.PlayerView.AddExperience((int)exp, killedObject);

                // Check the lvl up
                if (lvlup)
                {
                    this.Attributes[Stats.Level]++;
                    this.SelectedCharacter.LevelUpPoints += this.SelectedCharacter.CharacterClass.PointsPerLevelUp;
                    this.Attributes[Stats.CurrentHealth] = this.Attributes[Stats.MaximumHealth];
                    this.Attributes[Stats.CurrentMana] = this.Attributes[Stats.MaximumMana];
                    this.Attributes[Stats.CurrentShield] = this.Attributes[Stats.MaximumShield];
                    this.Attributes[Stats.CurrentAbility] = this.Attributes[Stats.MaximumAbility];
                    Logger.DebugFormat("Character {0} leveled up to {1}", this.SelectedCharacter.Name, this.Attributes[Stats.Level]);
                    this.PlayerView.UpdateLevel();
                }
            }
            else
            {
                this.PlayerView.ShowMessage("You already reached maximum Level.", MessageType.BlueNormal);
            }
        }

        /// <summary>
        /// Moves the player to the specified coordinate.
        /// </summary>
        /// <param name="target">The target.</param>
        public void Move(Point target)
        {
            Logger.DebugFormat("Move: Player is moving to {0}", target);
            this.walker.Stop();
            this.CurrentMap.Move(this, target, this.moveLock, MoveType.Instant);
            Logger.DebugFormat("Move: Observer Count: {0}", this.Observers.Count);
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
                Logger.DebugFormat("WalkTo: Player is walking to {0}", target);
                this.walker.WalkTo(target, steps);
                currentMap.Move(this, target, this.moveLock, MoveType.Walk);
                Logger.DebugFormat("WalkTo: Observer Count: {0}", this.Observers.Count);
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
            foreach (var r in Stats.IntervalRegenerationAttributes.Where(r => this.Attributes[r.RegenerationMultiplier] > 0))
            {
                this.Attributes[r.CurrentAttribute] = Math.Min(this.Attributes[r.CurrentAttribute] + (this.Attributes[r.MaximumAttribute] * this.Attributes[r.RegenerationMultiplier]), this.Attributes[r.MaximumAttribute]);
            }

            this.PlayerView.UpdateCurrentManaAndHp();
        }

        /// <summary>
        /// Disonnects the player from the game. Remote connections will be closed and data will be saved.
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

            if (skill.ConsumeRequirements.Any(r => r.MinimumValue > this.Attributes[r.Attribute]))
            {
                return false;
            }

            foreach (var requirement in skill.ConsumeRequirements)
            {
                this.Attributes[requirement.Attribute] -= requirement.MinimumValue;
            }

            return true;
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
        /// Gets the step delay depending on the equipped items.
        /// </summary>
        /// <returns></returns>
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
            return spawnTargetMap.ExitGates.Where(g => g.IsSpawnGate).SelectRandom();
        }

        private void Hit(HitInfo hitInfo, IAttackable attacker)
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
            this.LastReceivedDamage = hitInfo.HealthDamage;
            this.PlayerView.ShowHit(this, hitInfo);
            (attacker as Player)?.PlayerView.ShowHit(this, hitInfo);

            if (this.Attributes[Stats.CurrentHealth] < 1)
            {
                this.OnDeath(attacker);
            }

            var reflectPercentage = this.Attributes[Stats.DamageReflection];
            if (reflectPercentage > 0)
            {
                var reflectedDamage = (hitInfo.HealthDamage + hitInfo.ShieldDamage) * reflectPercentage;
                Task.Delay(500).ContinueWith(task =>
                {
                    if (attacker.Alive)
                    {
                        attacker.ReflectDamage(this, (uint)reflectedDamage);
                    }
                });
            }
        }

        private void OnDeath(IAttackable killer)
        {
            if (!this.PlayerState.TryAdvanceTo(GameLogic.PlayerState.Dead))
            {
                return;
            }

            this.walker.Stop();
            this.Alive = false;
            this.respawnAfterDeathToken = default(CancellationToken);
            this.PlayerView.WorldView.ObjectGotKilled(this, killer);

            // TODO: Drop items
            Task.Run(
              async () =>
              {
                  await Task.Delay(3000, this.respawnAfterDeathToken);
                  this.Attributes[Stats.CurrentHealth] = this.Attributes[Stats.MaximumHealth];
                  this.Attributes[Stats.CurrentMana] = this.Attributes[Stats.MaximumMana];
                  this.Attributes[Stats.CurrentShield] = this.Attributes[Stats.MaximumShield];
                  this.Attributes[Stats.CurrentAbility] = this.Attributes[Stats.MaximumAbility];
                  this.WarpTo(this.GetSpawnGateOfCurrentMap());
              },
              this.respawnAfterDeathToken);
        }

        private void OnPlayerEnteredWorld(object sender, EventArgs e)
        {
            this.Attributes = new ItemAwareAttributeSystem(this.SelectedCharacter);
            this.Inventory = new InventoryStorage(this, this.gameContext);
            this.ShopStorage = new ShopStorage(this);
            this.TemporaryStorage = new Storage(0, 0, InventoryConstants.TemporaryStorageSize, new TemporaryItemStorage());
            this.Vault = null; // vault storage is getting set when vault npc is opened.
            this.SkillList = new SkillList(this);
            this.SetMaximumStatAttributes();
            this.PlayerView.UpdateSkillList();
            this.PlayerView.UpdateCharacterStats();
            this.PlayerView.InventoryView.UpdateInventoryList();

            this.Attributes.GetOrCreateAttribute(Stats.MaximumMana).ValueChanged += (a, b) => this.OnMaximumManaOrAbilityChanged();
            this.Attributes.GetOrCreateAttribute(Stats.MaximumAbility).ValueChanged += (a, b) => this.OnMaximumManaOrAbilityChanged();
            this.Attributes.GetOrCreateAttribute(Stats.MaximumHealth).ValueChanged += (a, b) => this.OnMaximumHealthOrShieldChanged();
            this.Attributes.GetOrCreateAttribute(Stats.MaximumShield).ValueChanged += (a, b) => this.OnMaximumHealthOrShieldChanged();

            // TODO: bind own player to guild
            this.ClientReadyAfterMapChange();

            this.PlayerView.WorldView.UpdateRotation();
            this.PlayerView.MessengerView.InitializeMessenger(this.gameContext.Configuration.MaximumLetters);
        }

        private void SetMaximumStatAttributes()
        {
            foreach (var regeneration in Stats.IntervalRegenerationAttributes)
            {
                this.Attributes[regeneration.CurrentAttribute] = Math.Min(this.Attributes[regeneration.CurrentAttribute], this.Attributes[regeneration.MaximumAttribute]);
            }
        }

        private void OnMaximumHealthOrShieldChanged()
        {
            this.Attributes[Stats.CurrentHealth] = Math.Min(this.Attributes[Stats.CurrentHealth], this.Attributes[Stats.MaximumHealth]);
            this.Attributes[Stats.CurrentShield] = Math.Min(this.Attributes[Stats.CurrentShield], this.Attributes[Stats.MaximumShield]);
            this.PlayerView.UpdateMaximumHealth();
            this.PlayerView.UpdateCurrentHealth();
        }

        private void OnMaximumManaOrAbilityChanged()
        {
            this.Attributes[Stats.CurrentMana] = Math.Min(this.Attributes[Stats.CurrentMana], this.Attributes[Stats.MaximumMana]);
            this.Attributes[Stats.CurrentAbility] = Math.Min(this.Attributes[Stats.CurrentAbility], this.Attributes[Stats.MaximumAbility]);
            this.PlayerView.UpdateMaximumMana();
            this.PlayerView.UpdateCurrentMana();
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

            public AppearanceDataAdapter(Player player)
            {
                this.player = player;
            }

            public event EventHandler AppearanceChanged;

            public CharacterClass CharacterClass => this.player.SelectedCharacter?.CharacterClass;

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
            public void RaiseAppearanceChanged() => this.AppearanceChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
