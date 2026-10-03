// <copyright file="ResetCharacterNpcPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Resets;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Action to reset a character.
/// </summary>
[Guid("08953BE6-DABF-49CC-A500-FDB9DC2C4D80")]
[PlugIn]
[Display(Name = nameof(PlugInResources.ResetCharacterNpcPlugin_Name), Description = nameof(PlugInResources.ResetCharacterNpcPlugin_Description), ResourceType = typeof(PlugInResources))]
public class ResetCharacterNpcPlugin : IPlayerTalkToNpcPlugIn, ISupportCustomConfiguration<ResetCharacterNpcPluginConfiguration>, ISupportDefaultCustomConfiguration
{
    /// <inheritdoc />
    public ResetCharacterNpcPluginConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public async ValueTask PlayerTalksToNpcAsync(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs)
    {
        var configuration = this.Configuration ??= new ResetCharacterNpcPluginConfiguration();
        if (npc.Definition.Number != configuration.ResetNpcNumber)
        {
            return;
        }

        eventArgs.HasBeenHandled = true;
        var resetAction = new ResetCharacterAction(player, npc);
        await resetAction.ResetCharacterAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public object CreateDefaultConfig() => new ResetCharacterNpcPluginConfiguration();
}