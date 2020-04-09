// <copyright file="ITrapIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using MUnique.OpenMU.GameLogic.NPC;

    /// <summary>
    /// Interface of a trap artificial intelligence.
    /// </summary>
    public interface ITrapIntelligence
    {
        /// <summary>
        /// Gets or sets the trap which this AI is controlling.
        /// </summary>
        Trap Trap { get; set; }

        /// <summary>
        /// Starts the actions.
        /// </summary>
        void Start();
    }
}