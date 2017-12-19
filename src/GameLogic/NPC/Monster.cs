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
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Pathfinding;

    /// <summary>
    /// The implementation of a monster, which can attack players.
    /// </summary>
    public sealed class Monster : NonPlayerCharacter, IAttackable, ISupportWalk
    {
        private const byte MonsterAttackAnimation = 0x78;
        private readonly IDropGenerator dropGenerator;
        private readonly object moveLock = new object();
        private readonly IMonsterIntelligence intelligence;
        private readonly Walker walker;

        private Timer respawnTimer;
        private int health;
        private bool isCalculatingPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="Monster"/> class.
        /// </summary>
        /// <param name="spawnInfo">The spawn information.</param>
        /// <param name="stats">The stats.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="map">The map on which this instance will spawn.</param>
        /// <param name="dropGenerator">The drop generator.</param>
        /// <param name="monsterIntelligence">The monster intelligence.</param>
        public Monster(MonsterSpawnArea spawnInfo, MonsterDefinition stats, ushort id, GameMap map, IDropGenerator dropGenerator, IMonsterIntelligence monsterIntelligence)
            : base(spawnInfo, stats, id, map)
        {
            this.dropGenerator = dropGenerator;
            this.Attributes = new MonsterAttributeHolder(this);
            this.walker = new Walker(this);
            this.intelligence = monsterIntelligence;
            this.intelligence.Monster = this;
            this.intelligence.Start();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Monster"/> is walking.
        /// </summary>
        /// <value>
        ///   <c>true</c> if walking; otherwise, <c>false</c>.
        /// </value>
        public bool IsWalking { get; set; }

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
        public IAttributeSystem Attributes { get; }

        /// <inheritdoc/>
        public TimeSpan StepDelay => this.Definition.MoveDelay;

        /// <inheritdoc />
        public Stack<WalkingStep> NextDirections { get; } = new Stack<WalkingStep>(5);

        /// <inheritdoc/>
        public Point WalkTarget { get; set; }

        /// <inheritdoc/>
        public override void Respawn()
        {
            base.Respawn();
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
            this.ForEachObservingPlayer(p => p.PlayerView.WorldView.ShowAnimation(this, MonsterAttackAnimation, player, this.GetDirectionTo(player)), true);
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
                var pathFinder = new PathFinder(new GridNetwork(this.CurrentMap.Terrain.AIgrid, true)); // TODO: Reuse PathFinder?
                calculatedPath = pathFinder.FindPath(new Point(this.X, this.Y), new Point(target.X, target.Y));
                if (calculatedPath == null)
                {
                    return;
                }
            }
            finally
            {
                this.isCalculatingPath = false;
            }

            var targetNode = calculatedPath.Last();
            this.WalkTarget = new Point(targetNode.X, targetNode.Y);
            this.NextDirections.Clear();
            foreach (var step in calculatedPath.Select(GetStep).Reverse())
            {
                this.NextDirections.Push(step);
            }

            this.Move(targetNode.X, targetNode.Y, MoveType.Walk);
            this.walker.Start();
        }

        /// <summary>
        /// Attacks this object by the attacker with the specified skill.
        /// </summary>
        /// <param name="attacker">The attacker.</param>
        /// <param name="skill">The skill.</param>
        public void AttackBy(IAttackable attacker, SkillEntry skill)
        {
            var hitInfo = attacker.CalculateDamage(this, skill);
            bool killed = this.Hit(hitInfo.DamageHP + hitInfo.DamageSD, attacker);
            if (attacker is Player player)
            {
                player.PlayerView.ShowHit(this, hitInfo);
                if (killed)
                {
                    var party = player.Party;
                    int exp = party?.DistributeExperienceAfterKill(this) ?? player.AddExpAfterKill(this);
                    this.DropItem(exp, player);
                    this.OnDeath(player);
                }
            }
        }

        /// <summary>
        /// Moves this instance randomly.
        /// </summary>
        internal void RandomMove()
        {
            byte randx = (byte)GameLogic.Rand.NextInt(Math.Max(0, this.X - 1), Math.Min(0xFF, this.X + 2));
            byte randy = (byte)GameLogic.Rand.NextInt(Math.Max(0, this.Y - 1), Math.Min(0xFF, this.Y + 2));
            if (this.CurrentMap.Terrain.AIgrid[randx, randy] == 1)
            {
                this.Move(randx, randy, MoveType.Walk);
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool dispose)
        {
            base.Dispose(dispose);
            this.respawnTimer.Dispose();
            this.walker.Dispose();
            (this.intelligence as IDisposable)?.Dispose();
        }

        /// <summary>
        /// Moves the instance to the specified position.
        /// </summary>
        /// <param name="newx">The new x coordinate.</param>
        /// <param name="newy">The new y coordinate.</param>
        /// <param name="type">The type of moving.</param>
        protected override void Move(byte newx, byte newy, MoveType type)
        {
            this.CurrentMap.Move(this, newx, newy, this.moveLock, type);
            if (type == MoveType.Instant)
            {
                this.X = newx;
                this.Y = newy;
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
                Point dropCoord;
                if (firstItem)
                {
                    dropCoord = new Point(this.X, this.Y);
                    firstItem = false;
                }
                else
                {
                    dropCoord = this.CurrentMap.Terrain.GetRandomDropCoordinate(this.X, this.Y, 4);
                }

                var droppedItem = new DroppedItem(item, dropCoord.X, dropCoord.Y, this.CurrentMap, null);
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
                    o.WorldView.ObjectGotKilled(this, attacker);
                }

                this.Observers.Clear();
            }
            finally
            {
                this.ObserverLock.ExitWriteLock();
            }

            this.CurrentMap.Remove(this);
        }

        private bool Hit(uint damage, IAttackable attacker)
        {
            if (!this.Alive)
            {
                return false;
            }

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
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
