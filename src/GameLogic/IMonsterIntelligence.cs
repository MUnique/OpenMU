// <copyright file="IMonsterIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using MUnique.OpenMU.GameLogic.NPC;

    /// <summary>
    /// Interface of a monster artificial intelligence.
    /// </summary>
    public interface IMonsterIntelligence
    {
        /// <summary>
        /// Gets or sets the monster which this AI is controlling.
        /// </summary>
        Monster Monster { get; set; }

        /// <summary>
        /// Registers a hit from an attacker.
        /// </summary>
        /// <param name="attacker">The attacker.</param>
        void RegisterHit(IAttackable attacker);

        /// <summary>
        /// Starts the actions.
        /// </summary>
        void Start();
    }
}
