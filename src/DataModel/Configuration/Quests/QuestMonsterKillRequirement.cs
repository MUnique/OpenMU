// <copyright file="QuestMonsterKillRequirement.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Quests
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The monster kill requirement of a <see cref="QuestDefinition"/>.
    /// </summary>
    public class QuestMonsterKillRequirement
    {
        /// <summary>
        /// Gets or sets the monster which must be killed.
        /// </summary>
        [Required]
        public virtual MonsterDefinition? Monster { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of killed <see cref="Monster"/>s.
        /// </summary>
        public int MinimumNumber { get; set; }
    }
}