// <copyright file="MasterSkillLevelChangedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IMasterSkillLevelChangedPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("MasterSkillLevelChangedPlugIn", "The default implementation of the IMasterSkillLevelChangedPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("0eba687e-c7af-421e-8e1e-921fcf31c027")]
    public class MasterSkillLevelChangedPlugIn : IMasterSkillLevelChangedPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterSkillLevelChangedPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public MasterSkillLevelChangedPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc />
        public void MasterSkillLevelChanged(SkillEntry skillEntry)
        {
            var character = this.player.SelectedCharacter;
            using var writer = this.player.Connection.StartSafeWrite(MasterSkillLevelUpdate.HeaderType, MasterSkillLevelUpdate.Length);
            _ = new MasterSkillLevelUpdate(writer.Span)
            {
                Success = true,
                MasterLevelUpPoints = (ushort)character.MasterLevelUpPoints,
                MasterSkillIndex = skillEntry.Skill.GetMasterSkillIndex(character.CharacterClass),
                MasterSkillNumber = (ushort)skillEntry.Skill.Number,
                Level = (byte)skillEntry.Level,
                DisplayValue = skillEntry.CalculateDisplayValue(),
                DisplayValueOfNextLevel = skillEntry.CalculateNextDisplayValue(),
            };

            writer.Commit();
        }
    }
}