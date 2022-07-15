// <copyright file="SkillListViewPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="ISkillListViewPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(SkillListViewPlugIn075), "The implementation of the ISkillListViewPlugIn for version 0.75 which is forwarding everything to the game client with specific data packets.")]
[Guid("D83A0CD8-AFEB-4782-8523-AF6D093D14CB")]
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

    /// <inheritdoc/>
    public override async ValueTask AddSkillAsync(Skill skill)
    {
        var skillIndex = this.AddSkillToList(skill);
        await this.Player.Connection.SendSkillAdded075Async(skillIndex, this.GetSkillNumberAndLevel(skill)).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async ValueTask RemoveSkillAsync(Skill skill)
    {
        var skillIndex = (byte)this.SkillList.IndexOf(skill);
        await this.Player.Connection.SendSkillRemoved075Async(skillIndex, this.GetSkillNumberAndLevel(skill)).ConfigureAwait(false);
        this.SkillList[skillIndex] = null;
    }

    /// <inheritdoc/>
    public override async ValueTask UpdateSkillListAsync()
    {
        var connection = this.Player.Connection;
        if (connection is null)
        {
            return;
        }

        this.BuildSkillList();

        int Write()
        {
            var size = SkillListUpdate075Ref.GetRequiredSize(this.SkillList.Count);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new SkillListUpdate075Ref(span)
            {
                Count = (byte)this.SkillList.Count,
            };

            for (byte i = 0; i < this.SkillList.Count; i++)
            {
                var skillEntry = packet[i];
                skillEntry.SkillIndex = i;
                skillEntry.SkillNumberAndLevel = this.GetSkillNumberAndLevel(this.SkillList[i]);
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the value for the skill number and level.
    /// </summary>
    /// <param name="skill">The skill.</param>
    /// <returns>The value for the skill number and level.</returns>
    protected ushort GetSkillNumberAndLevel(Skill? skill)
    {
        if (skill is null)
        {
            return 0;
        }

        var result = (ushort)((skill.Number & 0xFF) << 8);

        // The next lines seems strange but is correct. The same part of the skill number is already set in the first byte.
        // Unfortunately it's unclear to us which skill got a level greater than 0 in these early versions.
        // It might be the item level of a weapon with a skill, but this should not be of interest for the client.
        // It could be the type of summoning orb skill, too. For now, we just don't send this level.
        var skillLevel = 0;
        var secondByte = (byte)((skill.Number & 7) | (skillLevel << 3));
        return (ushort)(result + secondByte);
    }
}