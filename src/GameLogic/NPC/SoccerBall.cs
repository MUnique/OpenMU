// <copyright file="SoccerBall.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC
{
    using System;
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Pathfinding;

    /// <summary>
    /// The implementation of a soccer ball.
    /// They can be attacked, but take no damage. Instead, they move into the direction in which they got hit.
    /// </summary>
    public sealed class SoccerBall : NonPlayerCharacter, IAttackable, IMovable
    {
        private readonly object moveLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="SoccerBall"/> class.
        /// </summary>
        /// <param name="spawnInfo">The spawn information.</param>
        /// <param name="stats">The stats.</param>
        /// <param name="map">The map on which this instance will spawn.</param>
        public SoccerBall(MonsterSpawnArea spawnInfo, MonsterDefinition stats, GameMap map)
            : base(spawnInfo, stats, map)
        {
            this.MagicEffectList = new MagicEffectsList(this);
            this.Attributes = new AttributeSystem(Enumerable.Empty<IAttribute>(), Enumerable.Empty<IAttribute>(), Enumerable.Empty<AttributeRelationship>());
        }

        /// <summary>
        /// Occurs when it has been moved.
        /// </summary>
        public event EventHandler<(Point From, Point To)>? Moved;

        /// <inheritdoc />
        public IAttributeSystem Attributes { get; }

        /// <inheritdoc />
        public MagicEffectsList MagicEffectList { get; }

        /// <inheritdoc />
        public bool IsAlive => true;

        /// <inheritdoc />
        public bool IsTeleporting => false;

        /// <inheritdoc />
        public DeathInformation? LastDeath => null;

        /// <inheritdoc />
        public void AttackBy(IAttacker attacker, SkillEntry? skill)
        {
            var direction = attacker.GetDirectionTo(this);
            this.MoveToDirection(direction, skill is { });
        }

        private void MoveToDirection(Direction direction, bool withSkill)
        {
            var terrain = this.CurrentMap.Terrain;
            var range = withSkill ? 3 : 2;
            var finalTarget = this.Position;
            for (int i = 0; i < range; i++)
            {
                var target = finalTarget.CalculateTargetPoint(direction);
                if (terrain.AIgrid[target.X, target.Y] == 1)
                {
                    finalTarget = target;
                }
                else
                {
                    break;
                }
            }

            if (finalTarget != this.Position)
            {
                this.Move(finalTarget);
            }
        }

        /// <inheritdoc />
        public void ReflectDamage(IAttacker reflector, uint damage)
        {
            // A ball doesn't attack, so it doesn't reflect.
        }

        /// <inheritdoc />
        public void ApplyPoisonDamage(IAttacker initialAttacker, uint damage)
        {
            // A ball doesn't take any damage
        }

        /// <inheritdoc />
        public void Move(Point target)
        {
            var old = this.Position;
            this.CurrentMap.Move(this, target, this.moveLock, MoveType.Instant);
            this.Moved?.Invoke(this, (From: old, To: target));
        }
    }
}