// <copyright file="Trap.cs" company="MUnique">
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
    using MUnique.OpenMU.GameLogic.Views.World;

    /// <summary>
    /// The implementation of a monster, which can attack players.
    /// </summary>
    public sealed class Trap : NonPlayerCharacter, IAttackable
    {
        private const byte TrapAttackAnimation = 0x78;
        private readonly ITrapIntelligence intelligence;

        /// <summary>
        /// Initializes a new instance of the <see cref="Trap"/> class.
        /// </summary>
        /// <param name="spawnInfo">The spawn information.</param>
        /// <param name="stats">The stats.</param>
        /// <param name="map">The map on which this instance will spawn.</param>
        /// <param name="trapIntelligence">The trap intelligence.</param>
        public Trap(MonsterSpawnArea spawnInfo, MonsterDefinition stats, GameMap map, ITrapIntelligence trapIntelligence)
            : base(spawnInfo, stats, map)
        {
            this.Attributes = new TrapAttributeHolder(this);
            this.MagicEffectList = new MagicEffectsList(this);
            this.intelligence = trapIntelligence;
            this.intelligence.Trap = this;
            this.intelligence.Start();
            this.Initialize();
        }

        /// <inheritdoc/>
        public MagicEffectsList MagicEffectList { get; }

        /// <inheritdoc/>
        public bool Alive { get; set; }

        /// <inheritdoc/>
        public uint LastReceivedDamage { get; private set; }

        /// <inheritdoc/>
        public IAttributeSystem Attributes { get; }

        /// <inheritdoc/>
        public override void Initialize()
        {
            base.Initialize();
            this.Alive = true;
        }

        /// <summary>
        /// Attacks the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        public void Attack(IAttackable player)
        {
            // TODO: Some traps attacks only when player is on them, example losttower traps or dungeon : Add it to TrapBasicIntelligence
            // need to find specific animation
            // Maybe add SpecificAnimation and AttackWhenPlayerOn properties to MonsterDefinition?? or create new TrapDefinition?
            player.AttackBy(this, null);
            this.ForEachWorldObserver(p => p.ViewPlugIns.GetPlugIn<IShowAnimationPlugIn>()?.ShowAnimation(this, TrapAttackAnimation, player, this.Rotation), true);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool managed)
        {
            base.Dispose(managed);
            if (managed)
            {
                (this.intelligence as IDisposable)?.Dispose();
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

        /// <inheritdoc/>
        public void AttackBy(IAttackable attacker, SkillEntry skill)
        {
            throw new NotSupportedException("Traps can't be attacked");
        }

        /// <inheritdoc/>
        public void ReflectDamage(IAttackable reflector, uint damage)
        {
            throw new NotSupportedException("Traps can't be attacked");
        }
    }
}