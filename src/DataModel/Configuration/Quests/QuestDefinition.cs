// <copyright file="QuestDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Quests
{
    using System.Collections.Generic;

    /// <summary>
    /// The definition of a quest.
    /// </summary>
    public class QuestDefinition
    {
        /// <summary>
        /// Gets or sets the NPC which gives the quest.
        /// </summary>
        public virtual MonsterDefinition QuestGiver { get; set; }

        /// <summary>
        /// Gets or sets the name of the quest.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the group identifier of the quest.
        /// </summary>
        public short Group { get; set; }

        /// <summary>
        /// Gets or sets the number of the quest which should be unique within the group.
        /// </summary>
        public short Number { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="QuestDefinition"/> is repeatable
        /// which means if the character can start this quest multiple times even after it already
        /// completed it once.
        /// </summary>
        /// <value>
        ///   <c>true</c> if repeatable; otherwise, <c>false</c>.
        /// </value>
        public bool Repeatable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this quest requires a client action.
        /// </summary>
        public bool RequiresClientAction { get; set; }

        /// <summary>
        /// Gets or sets the minimum character level.
        /// </summary>
        public int MinimumCharacterLevel { get; set; }

        /// <summary>
        /// Gets or sets the maximum character level.
        /// </summary>
        public int MaximumCharacterLevel { get; set; }

        /// <summary>
        /// Gets or sets the required monster kills to be able to complete this quest.
        /// </summary>
        /// <value>
        /// The required monster kills.
        /// </value>
        public virtual ICollection<QuestMonsterKillRequirement> RequiredMonsterKills { get; protected set; }

        /// <summary>
        /// Gets or sets the required items which should be in the characters inventory when the
        /// player requests to complete the quest.
        /// </summary>
        /// <value>
        /// The required items.
        /// </value>
        public virtual ICollection<QuestItemRequirement> RequiredItems { get; protected set; }

        /// <summary>
        /// Gets or sets the rewards when completing the quest successfully.
        /// </summary>
        /// <value>
        /// The rewards when completing the quest successfully.
        /// </value>
        public virtual ICollection<QuestReward> Rewards { get; protected set; }
    }
}
