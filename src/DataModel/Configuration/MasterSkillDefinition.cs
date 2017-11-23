// <copyright file="MasterSkillDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// The definition of a master skill. One skill can have 0-n master skill definitions,
    /// for example one skill can be used for different character classes at different Rank-Levels at different Roots.
    /// </summary>
    public class MasterSkillDefinition
    {
        /// <summary>
        /// Gets or sets the root.
        /// </summary>
        public virtual MasterSkillRoot Root { get; set; }

        /// <summary>
        /// Gets or sets a collection with the required skills.
        /// Just one skill (of at least level 10) of this list is required
        /// to meet the requirements when learning this skill.
        /// </summary>
        public virtual ICollection<Skill> RequiredMasterSkills { get; protected set; }

        /// <summary>
        /// Gets or sets the rank.
        /// The rang determines on which level the skill is located.
        /// A skill at a higher rank can be learned, if there is at least
        /// one skill of the same tree root at the direct rank below,
        /// with a level same or greater than 10.
        /// </summary>
        public byte Rank { get; set; }

        /// <summary>
        /// Gets or sets the character class to which this definition belongs.
        /// One master definiton is only valid for one master character class.
        /// One skill can be a master skill of more than one character,
        /// but with different requisitions.
        /// </summary>
        public virtual CharacterClass CharacterClass { get; set; }
    }
}
