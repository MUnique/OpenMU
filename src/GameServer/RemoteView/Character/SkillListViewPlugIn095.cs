// <copyright file="SkillListViewPlugIn095.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="ISkillListViewPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
/// <remarks>
/// Version 0.97d seems to be compatible to this implementation. This may be true until the end of season 2, but we're not sure.
/// </remarks>
[PlugIn(nameof(SkillListViewPlugIn095), "The implementation of the ISkillListViewPlugIn for version 0.95 which is forwarding everything to the game client with specific data packets.")]
[Guid("60126898-8668-4774-B879-0F211CDD3617")]
[MinimumClient(0, 95, ClientLanguage.Invariant)]
public class SkillListViewPlugIn095 : SkillListViewPlugIn075
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SkillListViewPlugIn095"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public SkillListViewPlugIn095(RemotePlayer player)
        : base(player)
    {
    }

    /// <inheritdoc/>
    public override async ValueTask AddSkillAsync(Skill skill)
    {
        var skillIndex = this.AddSkillToList(skill);
        await this.Player.Connection.SendSkillAdded095Async(skillIndex, this.GetSkillNumberAndLevel(skill)).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async ValueTask RemoveSkillAsync(Skill skill)
    {
        var skillIndex = (byte)this.SkillList.IndexOf(skill);
        await this.Player.Connection.SendSkillRemoved095Async(skillIndex, this.GetSkillNumberAndLevel(skill)).ConfigureAwait(false);
        this.SkillList[skillIndex] = null;
    }
}