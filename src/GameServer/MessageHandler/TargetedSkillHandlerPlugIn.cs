// <copyright file="TargetedSkillHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Implements the targeted skill packet handler.
/// </summary>
[PlugIn("TargetedSkillHandlerPlugIn", "Handler for targeted skill packets.")]
[Guid("5b07d03c-509c-4aec-972c-a99db77561f2")]
[MinimumClient(3, 0, ClientLanguage.Invariant)]
internal class TargetedSkillHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly ITargetedSkillPlugin _defaultStrategy = new TargetedSkillDefaultPlugin();

    /// <inheritdoc/>
    public virtual bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => TargetedSkill.Code;

    /// <inheritdoc/>
    public virtual async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        TargetedSkill message = packet;

        await this.HandleAsync(player, message.SkillId, message.TargetId).ConfigureAwait(false);
    }

    /// <summary>
    /// Handles the skill request of the specified player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="skillId">The skill identifier.</param>
    /// <param name="targetId">The target identifier.</param>
    protected async ValueTask HandleAsync(Player player, ushort skillId, ushort targetId)
    {
        var strategy = player.GameContext.
            PlugInManager.GetStrategy<short, ITargetedSkillPlugin>((short)skillId) ??
            this._defaultStrategy;

        // Note: The target can be the own player too, for example when using buff skills.
        if (player.GetObject(targetId) is IAttackable target)
        {
            await strategy.PerformSkillAsync(player, target, skillId).ConfigureAwait(false);
        }
    }
}