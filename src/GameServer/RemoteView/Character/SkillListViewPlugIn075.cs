// <copyright file="SkillListViewPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="ISkillListViewPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    /// <remarks>
    /// Version 0.97d seems to be compatible to this implementation. This may be true until the end of season 2, but we're not sure.
    /// </remarks>
    [PlugIn("SkillListViewPlugIn 0.75", "The implementation of the ISkillListViewPlugIn for version 0.75 which is forwarding everything to the game client with specific data packets.")]
    [Guid("D83A0CD8-AFEB-4782-8523-AF6D093D14CB")]
    [MaximumClient(2, 255, ClientLanguage.Invariant)]
    public class SkillListViewPlugIn075 : SkillListViewPlugIn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkillListViewPlugIn075"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public SkillListViewPlugIn075(RemotePlayer player)
            : base(player)
        {
        }

        /// <summary>
        /// Gets the size of the skill block.
        /// </summary>
        /// <remarks>
        /// Season 6 uses 4 bytes; first for the index, 2 for the skill id, the last one for the (master) level of the skill.
        /// </remarks>
        protected override int SkillBlockSize => 3;

        /// <summary>
        /// Gets the start index of the skill blocks, after the header with the skill count, where <see cref="WriteSkillBlock" /> is used.
        /// </summary>
        /// <remarks>
        /// In earlier versions, the skills blocks start one byte before. I guess it's some kind of a byte alignment change.
        /// </remarks>
        protected override int SkillsStartIndex => 5;

        /// <summary>
        /// Writes a skill into the specified block.
        /// </summary>
        /// <param name="skillBlock">The skill block.</param>
        /// <param name="skill">The skill.</param>
        /// <param name="skillLevel">The skill level.</param>
        /// <param name="skillIndex">Index of the skill.</param>
        protected override void WriteSkillBlock(Span<byte> skillBlock, Skill skill, byte skillLevel, byte skillIndex)
        {
            skillBlock[0] = skillIndex;
            skillBlock[1] = (byte)skill.Number;

            // The next line seems strange but is correct. The same part of the skill number is already set in index 1.
            // Unfortunately it's unclear to us which skill got a level greater than 0 in these early versions.
            // It might be the item level of a weapon with a skill, but this should not be of interest for the client.
            skillBlock[2] = (byte)((skill.Number & 7) | (skillLevel << 3));
        }
    }
}