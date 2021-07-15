// <copyright file="Monster.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.PlugIns;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Pathfinding;
    using MUnique.OpenMU.Persistence;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The implementation of a monster, which can attack players.
    /// </summary>
    public sealed class Monster : NonPlayerCharacter, IAttackable, IAttacker, ISupportWalk, IMovable
    {
        private readonly IDropGenerator dropGenerator;
        private readonly object moveLock = new ();
        private readonly INpcIntelligence intelligence;
        private readonly PlugInManager plugInManager;
        private readonly Walker walker;

        private Timer? respawnTimer;
        private int health;
        private bool isCalculatingPath;
        private PathFinder? pathFinder;

        /// <summary>
        /// Initializes a new instance of the <see cref="Monster" /> class.
        /// </summary>
        /// <param name="spawnInfo">The spawn information.</param>
        /// <param name="stats">The stats.</param>
        /// <param name="map">The map on which this instance will spawn.</param>
        /// <param name="dropGenerator">The drop generator.</param>
        /// <param name="npcIntelligence">The monster intelligence.</param>
        /// <param name="plugInManager">The plug in manager.</param>
        public Monster(MonsterSpawnArea spawnInfo, MonsterDefinition stats, GameMap map, IDropGenerator dropGenerator, INpcIntelligence npcIntelligence, PlugInManager plugInManager)
            : base(spawnInfo, stats, map)
        {
            this.dropGenerator = dropGenerator;
            this.Attributes = new MonsterAttributeHolder(this);
            this.MagicEffectList = new MagicEffectsList(this);
            this.walker = new Walker(this, () => this.StepDelay);
            this.intelligence = npcIntelligence;
            this.plugInManager = plugInManager;
            this.intelligence.Npc = this;
            this.intelligence.Start();
        }

        /// <inheritdoc/>
        public MagicEffectsList MagicEffectList { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Monster"/> is walking.
        /// </summary>
        /// <value>
        ///   <c>true</c> if walking; otherwise, <c>false</c>.
        /// </value>
        public bool IsWalking => this.WalkTarget != default;

        /// <summary>
        /// Gets a value indicating whether this <see cref="IAttackable" /> is currently teleporting and can't be directly targeted.
        /// It can still receive damage, if the teleport target coordinates are within an target skill area for area attacks.
        /// </summary>
        /// <value>
        ///   <c>true</c> if teleporting; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Teleporting for monsters oor npcs is not implemented yet.</remarks>
        public bool IsTeleporting => false;

        /// <inheritdoc/>
        public override Point Position
        {
            get => base.Position;
            set
            {
                if (base.Position != value)
                {
                    base.Position = value;
                    this.plugInManager?.GetPlugInPoint<IAttackableMovedPlugIn>()?.AttackableMoved(this);
                }
            }
        }

        /// <summary>
        /// Gets or sets the current health.
        /// </summary>
        public int Health
        {
            get => Math.Max(this.health, 0);
            set => this.health = value;
        }

        /// <summary>
        /// Gets the target by which this instance was summoned by.
        /// </summary>
        public Player? SummonedBy => (this.intelligence as SummonedMonsterIntelligence)?.Owner;

        /// <inheritdoc/>
        public bool IsAlive { get; set; }

        /// <inheritdoc/>
        public DeathInformation? LastDeath { get; private set; }

        /// <inheritdoc cref="IAttackable" />
        public IAttributeSystem Attributes { get; }

        /// <inheritdoc/>
        public Point WalkTarget => this.walker.CurrentTarget;

        /// <inheritdoc/>
        public TimeSpan StepDelay => this.Definition.MoveDelay;

        /// <inheritdoc/>
        public override void Initialize()
        {
            base.Initialize();
            this.respawnTimer?.Dispose();
            this.Health = (int)this.Attributes[Stats.MaximumHealth];
            this.IsAlive = true;
        }

        /// <summary>
        /// Attacks the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        public void Attack(IAttackable target)
        {
            target.AttackBy(this, null);
            this.ForEachWorldObserver(p => p.ViewPlugIns.GetPlugIn<IShowAnimationPlugIn>()?.ShowMonsterAttackAnimation(this, target, this.GetDirectionTo(target)), true);
            if (this.Definition.AttackSkill is { } attackSkill)
            {
                this.ForEachWorldObserver(p => p.ViewPlugIns.GetPlugIn<IShowSkillAnimationPlugIn>()?.ShowSkillAnimation(this, target, attackSkill, true), true);
            }
        }

        /// <summary>
        /// Walks to the target coordinates.
        /// </summary>
        /// <param name="target">The target object.</param>
        public void WalkTo(Point target)
        {
            if (this.isCalculatingPath || this.IsWalking)
            {
                return;
            }

            IList<PathResultNode>? calculatedPath;
            this.isCalculatingPath = true;
            try
            {
                if (this.pathFinder is null)
                {
                    this.pathFinder = new PathFinder(new GridNetwork(this.CurrentMap.Terrain.AIgrid, true));
                }
                else
                {
                    this.pathFinder.ResetPathFinder();
                }

                calculatedPath = this.pathFinder.FindPath(this.Position, target);
                if (calculatedPath is null)
                {
                    return;
                }
            }
            finally
            {
                this.isCalculatingPath = false;
            }

            var targetNode = calculatedPath.Last(); // that's one step before the target coordinates actually are reached.
            Span<WalkingStep> steps = stackalloc WalkingStep[calculatedPath.Count];
            var i = steps.Length;
            foreach (var step in calculatedPath.Select(GetStep))
            {
                i--;
                steps[i] = step;
            }

            this.WalkTo(new Point(targetNode.X, targetNode.Y), steps);
        }

        /// <summary>
        /// Walks to the target object.
        /// </summary>
        /// <param name="target">The target object.</param>
        public void WalkTo(ILocateable target) => this.WalkTo(target.Position);

        /// <summary>
        /// Walks to the specified target coordinates using the specified steps.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="steps">The steps.</param>
        public void WalkTo(Point target, Span<WalkingStep> steps)
        {
            this.walker.Stop();
            this.walker.WalkTo(target, steps);
            this.Move(target, MoveType.Walk);
        }

        /// <inheritdoc/>
        public int GetDirections(Span<Direction> directions)
        {
            return this.walker.GetDirections(directions);
        }

        /// <inheritdoc />
        public int GetSteps(Span<WalkingStep> steps) => this.walker.GetSteps(steps);

        /// <inheritdoc />
        public void AttackBy(IAttacker attacker, SkillEntry? skill)
        {
            var hitInfo = attacker.CalculateDamage(this, skill);
            this.Hit(hitInfo, attacker, skill?.Skill);
            if (hitInfo.HealthDamage > 0)
            {
                attacker.ApplyAmmunitionConsumption(hitInfo);
                (attacker as Player)?.AfterHitTarget();
            }
        }

        /// <inheritdoc />
        public void ReflectDamage(IAttacker reflector, uint damage)
        {
            this.Hit(new HitInfo(damage, 0, DamageAttributes.Reflected), reflector, null);
        }

        /// <inheritdoc />
        public void ApplyPoisonDamage(IAttacker initialAttacker, uint damage)
        {
            this.Hit(new HitInfo(damage, 0, DamageAttributes.Poison), initialAttacker, null);
        }

        /// <inheritdoc/>
        public void Move(Point target)
        {
            this.Move(target, MoveType.Instant);
        }

        /// <summary>
        /// Moves this instance randomly.
        /// </summary>
        internal void RandomMove()
        {
            byte randx = (byte)GameLogic.Rand.NextInt(Math.Max(0, this.Position.X - 1), Math.Min(0xFF, this.Position.X + 2));
            byte randy = (byte)GameLogic.Rand.NextInt(Math.Max(0, this.Position.Y - 1), Math.Min(0xFF, this.Position.Y + 2));
            if (this.CurrentMap.Terrain.AIgrid[randx, randy] == 1)
            {
                var target = new Point(randx, randy);
                var current = this.Position;
                Span<WalkingStep> steps = stackalloc WalkingStep[1];
                steps[0] = new WalkingStep
                {
                    From = current,
                    To = target,
                    Direction = current.GetDirectionTo(target),
                };
                this.WalkTo(target, steps);
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool managed)
        {
            if (managed)
            {
                this.respawnTimer?.Dispose();
                this.walker.Dispose();
                (this.intelligence as IDisposable)?.Dispose();
                this.CurrentMap?.Remove(this);
                this.IsAlive = false;
            }

            base.Dispose(managed);
        }

        /// <inheritdoc />
        protected override void Move(Point target, MoveType type)
        {
            if (type == MoveType.Instant || type == MoveType.Teleport)
            {
                this.walker.Stop();
            }

            this.CurrentMap.Move(this, target, this.moveLock, type);
        }

        private static WalkingStep GetStep(PathResultNode node)
        {
            return new ()
            {
                Direction = node.PreviousPoint.GetDirectionTo(new Point(node.X, node.Y)),
                From = node.PreviousPoint,
                To = new Point(node.X, node.Y),
            };
        }

        private void HandleMoneyDrop(uint amount, Player killer)
        {
            if (!killer.GameContext.Configuration.ShouldDropMoney)
            {
                var party = killer.Party;
                if (party is null)
                {
                    killer.TryAddMoney((int)amount);
                }
                else
                {
                    var players = party.PartyList.OfType<Player>().Where(p => p.CurrentMap == killer.CurrentMap && !p.IsAtSafezone() && p.Attributes is { }).ToList();
                    var moneyPart = amount / players.Count;
                    players.ForEach(p => p.TryAddMoney((int)(moneyPart * p.Attributes![Stats.MoneyAmountRate])));
                }

                return;
            }

            var droppedMoney = new DroppedMoney((uint)(amount * killer.Attributes![Stats.MoneyAmountRate]), this.Position, this.CurrentMap);
            this.CurrentMap.Add(droppedMoney);
        }

        private void DropItem(int exp, Player killer)
        {
            var generatedItems = this.dropGenerator.GenerateItemDrops(this.Definition, exp, killer, out var droppedMoney);
            if (droppedMoney > 0)
            {
                this.HandleMoneyDrop(droppedMoney.Value, killer);
            }

            if (generatedItems is null)
            {
                return;
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
                this.CurrentMap.Add(droppedItem);
            }
        }

        private void OnDeath(IAttacker attacker)
        {
            this.walker.Stop();
            if (this.SpawnArea.SpawnTrigger == SpawnTrigger.Automatic)
            {
                this.respawnTimer = new Timer(o => this.Respawn(), null, (int)this.Definition.RespawnDelay.TotalMilliseconds, System.Threading.Timeout.Infinite);
            }

            this.ObserverLock.EnterWriteLock();
            try
            {
                foreach (IWorldObserver o in this.Observers)
                {
                    o.ViewPlugIns.GetPlugIn<IObjectGotKilledPlugIn>()?.ObjectGotKilled(this, attacker);
                }

                this.Observers.Clear();
            }
            finally
            {
                this.ObserverLock.ExitWriteLock();
            }

            var player = attacker as Player ?? (attacker as Monster)?.SummonedBy;
            if (player is { })
            {
                int exp = player.Party?.DistributeExperienceAfterKill(this, player) ?? player.AddExpAfterKill(this);
                this.DropItem(exp, player);
                if (attacker == player)
                {
                    player.AfterKilledMonster();
                }

                player.GameContext.PlugInManager.GetPlugInPoint<IAttackableGotKilledPlugIn>()?.AttackableGotKilled(this, attacker);
                if (player.SelectedCharacter!.State > HeroState.Normal)
                {
                    player.SelectedCharacter.StateRemainingSeconds -= (int)this.Attributes[Stats.Level];
                }
            }

            if (this.SpawnArea.SpawnTrigger == SpawnTrigger.OnceAtEventStart)
            {
                this.CurrentMap.Remove(this);
                this.Dispose();
                if (this.intelligence is SummonedMonsterIntelligence summonedMonsterIntelligence)
                {
                    summonedMonsterIntelligence.Owner.SummonDied();
                }
            }
        }

        /// <summary>
        /// Respawns this instance on the map.
        /// </summary>
        private void Respawn()
        {
            try
            {
                this.Initialize();
                this.CurrentMap.Respawn(this);
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message, ex.StackTrace);
            }
        }

        private void Hit(HitInfo hitInfo, IAttacker attacker, Skill? skill)
        {
            if (!this.IsAlive)
            {
                return;
            }

            var killed = this.TryHit(hitInfo.HealthDamage + hitInfo.ShieldDamage, attacker);

            var player = attacker as Player ?? (attacker as Monster)?.SummonedBy ?? this.SummonedBy;
            if (player is not null)
            {
                player.ViewPlugIns.GetPlugIn<IShowHitPlugIn>()?.ShowHit(this, hitInfo);
                player.GameContext.PlugInManager.GetPlugInPoint<IAttackableGotHitPlugIn>()?.AttackableGotHit(this, attacker, hitInfo);
            }

            if (killed)
            {
                this.LastDeath = new DeathInformation(attacker.Id, attacker.GetName(), hitInfo, skill?.Number ?? 0);
                this.OnDeath(attacker);
            }
        }

        private bool TryHit(uint damage, IAttacker attacker)
        {
            if (damage > 0)
            {
                this.intelligence.RegisterHit(attacker);
            }

            if (damage >= this.Health)
            {
                this.IsAlive = false;
                this.Health = 0;
                return true;
            }

            try
            {
                Interlocked.Add(ref this.health, -(int)damage);
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
