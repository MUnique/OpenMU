// <copyright file="Monster.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.PlugIns;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Pathfinding;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The implementation of a monster, which can attack players.
    /// </summary>
    public sealed class Monster : NonPlayerCharacter, IAttackable, ISupportWalk, IMovable
    {
        private const byte MonsterAttackAnimation = 0x78;
        private readonly IDropGenerator dropGenerator;
        private readonly object moveLock = new object();
        private readonly IMonsterIntelligence intelligence;
        private readonly PlugInManager plugInManager;
        private readonly Walker walker;

        private Timer respawnTimer;
        private int health;
        private bool isCalculatingPath;
        private PathFinder pathFinder;

        /// <summary>
        /// Initializes a new instance of the <see cref="Monster" /> class.
        /// </summary>
        /// <param name="spawnInfo">The spawn information.</param>
        /// <param name="stats">The stats.</param>
        /// <param name="map">The map on which this instance will spawn.</param>
        /// <param name="dropGenerator">The drop generator.</param>
        /// <param name="monsterIntelligence">The monster intelligence.</param>
        /// <param name="plugInManager">The plug in manager.</param>
        public Monster(MonsterSpawnArea spawnInfo, MonsterDefinition stats, GameMap map, IDropGenerator dropGenerator, IMonsterIntelligence monsterIntelligence, PlugInManager plugInManager)
            : base(spawnInfo, stats, map)
        {
            this.dropGenerator = dropGenerator;
            this.Attributes = new MonsterAttributeHolder(this);
            this.MagicEffectList = new MagicEffectsList(this);
            this.walker = new Walker(this, () => this.StepDelay);
            this.intelligence = monsterIntelligence;
            this.plugInManager = plugInManager;
            this.intelligence.Monster = this;
            this.intelligence.Start();
            this.Initialize();
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

        /// <inheritdoc/>
        public bool Alive { get; set; }

        /// <inheritdoc/>
        public uint LastReceivedDamage { get; private set; }

        /// <inheritdoc/>
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
            this.Alive = true;
        }

        /// <summary>
        /// Attacks the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        public void Attack(IAttackable player)
        {
            player.AttackBy(this, null);
            this.ForEachWorldObserver(p => p.ViewPlugIns.GetPlugIn<IShowAnimationPlugIn>()?.ShowAnimation(this, MonsterAttackAnimation, player, this.GetDirectionTo(player)), true);
        }

        /// <summary>
        /// Walks to the target object.
        /// </summary>
        /// <param name="target">The target object.</param>
        public void WalkTo(ILocateable target)
        {
            if (this.isCalculatingPath || this.IsWalking)
            {
                return;
            }

            IList<PathResultNode> calculatedPath;
            this.isCalculatingPath = true;
            try
            {
                if (this.pathFinder == null)
                {
                    this.pathFinder = new PathFinder(new GridNetwork(this.CurrentMap.Terrain.AIgrid, true));
                }
                else
                {
                    this.pathFinder.ResetPathFinder();
                }

                calculatedPath = this.pathFinder.FindPath(this.Position, target.Position);
                if (calculatedPath == null)
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
        /// Walks to the specified target coordinates using the specified steps.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="steps">The steps.</param>
        public void WalkTo(Point target, Span<WalkingStep> steps)
        {
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
        public void AttackBy(IAttackable attacker, SkillEntry skill)
        {
            var hitInfo = attacker.CalculateDamage(this, skill);
            this.Hit(hitInfo, attacker);
            if (hitInfo.HealthDamage > 0)
            {
                attacker.ApplyAmmunitionConsumption(hitInfo);
                (attacker as Player)?.AfterHitTarget();
            }
        }

        /// <inheritdoc />
        public void ReflectDamage(IAttackable reflector, uint damage)
        {
            this.Hit(new HitInfo(damage, 0, DamageAttributes.Reflected), reflector);
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
            base.Dispose(managed);
            if (managed)
            {
                this.respawnTimer?.Dispose();
                this.walker.Dispose();
                (this.intelligence as IDisposable)?.Dispose();
            }
        }

        /// <inheritdoc />
        protected override void Move(Point target, MoveType type)
        {
            this.CurrentMap.Move(this, target, this.moveLock, type);
            if (type == MoveType.Instant)
            {
                this.walker.Stop();
                this.Position = target;
            }
        }

        private static WalkingStep GetStep(PathResultNode node)
        {
            return new WalkingStep
            {
                Direction = node.PreviousPoint.GetDirectionTo(new Point(node.X, node.Y)),
                From = node.PreviousPoint,
                To = new Point(node.X, node.Y),
            };
        }

        private void DropItem(int exp, Player killer)
        {
            var generatedItems = this.dropGenerator.GetItemDropsOrAddMoney(this.Definition, exp, killer);
            if (generatedItems == null)
            {
                return;
            }

            var firstItem = true;
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
                    dropCoordinates = this.CurrentMap.Terrain.GetRandomDropCoordinate(this.Position, 4);
                }

                var droppedItem = new DroppedItem(item, dropCoordinates, this.CurrentMap, null);
                this.CurrentMap.Add(droppedItem);
            }
        }

        private void OnDeath(IAttackable attacker)
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

            if (attacker is Player player)
            {
                int exp = player.Party?.DistributeExperienceAfterKill(this, player) ?? player.AddExpAfterKill(this);
                this.DropItem(exp, player);
                player.AfterKilledMonster();
                player.GameContext.PlugInManager.GetPlugInPoint<IAttackableGotKilledPlugIn>()?.AttackableGotKilled(this, attacker);
            }
        }

        /// <summary>
        /// Respawns this instance on the map.
        /// </summary>
        private void Respawn()
        {
            this.Initialize();
            this.CurrentMap.Respawn(this);
        }

        private void Hit(HitInfo hitInfo, IAttackable attacker)
        {
            if (!this.Alive)
            {
                return;
            }

            var killed = this.TryHit(hitInfo.HealthDamage + hitInfo.ShieldDamage, attacker);
            if (attacker is Player player)
            {
                player.ViewPlugIns.GetPlugIn<IShowHitPlugIn>()?.ShowHit(this, hitInfo);
                player.GameContext.PlugInManager.GetPlugInPoint<IAttackableGotHitPlugIn>()?.AttackableGotHit(this, attacker, hitInfo);
            }

            if (killed)
            {
                this.OnDeath(attacker);
            }
        }

        private bool TryHit(uint damage, IAttackable attacker)
        {
            if (damage > 0)
            {
                this.intelligence.RegisterHit(attacker);
            }

            if (damage >= this.Health)
            {
                this.Alive = false;
                this.Health = 0;
                return true;
            }

            try
            {
                Interlocked.Add(ref this.health, -(int)damage);
                this.LastReceivedDamage = damage;
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
