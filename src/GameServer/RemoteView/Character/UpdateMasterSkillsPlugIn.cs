﻿// <copyright file="UpdateMasterSkillsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IUpdateMasterSkillsPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("UpdateMasterSkillsPlugIn", "The default implementation of the IUpdateMasterSkillsPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("72942fe8-925d-43b0-a908-b814b2baa1f3")]
public class UpdateMasterSkillsPlugIn : IUpdateMasterSkillsPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateMasterSkillsPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateMasterSkillsPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public void UpdateMasterSkills()
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        var masterSkills = this._player.SkillList?.Skills.Where(s => s.Skill?.MasterDefinition != null).ToList();
        if (masterSkills is null || this._player.SelectedCharacter?.CharacterClass is null)
        {
            return;
        }

        using var writer = connection.StartSafeWrite(MasterSkillList.HeaderType, MasterSkillList.GetRequiredSize(masterSkills.Count));
        var packet = new MasterSkillList(writer.Span)
        {
            MasterSkillCount = (uint)masterSkills.Count,
        };

        int i = 0;
        foreach (var masterSkill in masterSkills)
        {
            var skillsBlock = packet[i];
            skillsBlock.MasterSkillIndex = masterSkill.Skill!.GetMasterSkillIndex(this._player.SelectedCharacter.CharacterClass);
            skillsBlock.Level = (byte)masterSkill.Level;
            skillsBlock.DisplayValue = masterSkill.CalculateDisplayValue();
            skillsBlock.DisplayValueOfNextLevel = masterSkill.CalculateNextDisplayValue();
            i++;
        }

        writer.Commit();
    }
}