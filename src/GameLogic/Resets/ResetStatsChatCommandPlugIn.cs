// <copyright file="ResetStatsChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Resets;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles the stat reset command.
/// </summary>
[Guid("A1B2C3D4-E5F6-7890-ABCD-EF1234567891")]
[PlugIn]
[Display(Name = nameof(PlugInResources.ResetStatsChatCommandPlugIn_Name), Description = nameof(PlugInResources.ResetStatsChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, "Resets your character stats to base values and refunds all invested points.", null)]
public class ResetStatsChatCommandPlugIn : IChatCommandPlugIn
{
    private const string Command = "/resetstats";

    /// <inheritdoc />
    public string Key => Command;

    /// <inheritdoc />
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        var statResetFeature = player.GameContext.FeaturePlugIns.GetPlugIn<StatResetFeaturePlugIn>();
        if (statResetFeature?.Configuration is { } configuration && !configuration.ChatCommandEnabled)
        {
            await player.ShowLocalizedBlueMessageAsync(PlayerMessage.StatResetChatCommandDisabled).ConfigureAwait(false);
            return;
        }

        var resetAction = new ResetStatsAction(player);
        await resetAction.ResetStatsAsync().ConfigureAwait(false);
    }
}
