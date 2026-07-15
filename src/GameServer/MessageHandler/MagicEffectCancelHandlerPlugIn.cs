// <copyright file="MagicEffectCancelHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for magic effect cancel packets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.MagicEffectCancelHandlerPlugIn_Name), Description = nameof(PlugInResources.MagicEffectCancelHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("3f25c9a1-4b8d-4e7f-9a2b-1c3d4e5f6a7b")]
[MinimumClient(1, 0, ClientLanguage.Invariant)]
internal class MagicEffectCancelHandlerPlugIn : IPacketHandlerPlugIn
{
    private const int InfinityArrowSkillId = 77;
    private const int ExpansionOfWizardrySkillId = 233;
    private const int ExpansionOfWizardryStrengSkillId = 380;
    private const int ExpansionOfWizardryMasterySkillId = 383;
    private const int InfinityArrowStrengSkillId = 441;

    /// <inheritdoc/>
    public bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => MagicEffectCancelRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        MagicEffectCancelRequest message = packet;
        if (!IsCancellableSkill(message.SkillId) || player.SkillList is null || !player.SkillList.ContainsSkill(message.SkillId))
        {
            return;
        }

        var magicEffect = player.SkillList.GetSkill(message.SkillId)?.Skill?.MagicEffectDef;
        if (magicEffect is null || !player.MagicEffectList.ActiveEffects.TryGetValue(magicEffect.Number, out var effect))
        {
            return;
        }

        await effect.DisposeAsync().ConfigureAwait(false);
    }

    private static bool IsCancellableSkill(ushort skillId) => skillId switch
    {
        ExpansionOfWizardrySkillId => true,
        ExpansionOfWizardryStrengSkillId => true,
        ExpansionOfWizardryMasterySkillId => true,
        InfinityArrowSkillId => true,
        InfinityArrowStrengSkillId => true,
        _ => false,
    };
}