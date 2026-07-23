// <copyright file="GoldenArcherTalkPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Plugin to handle the dialogue with the Golden Archer (NPC 236).
/// </summary>
[Guid("F0C60C89-6F12-40DB-A24F-418E6DE04300")]
[PlugIn]
public class GoldenArcherTalkPlugin : IPlayerTalkToNpcPlugIn
{
    /// <inheritdoc />
    public async ValueTask PlayerTalksToNpcAsync(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs)
    {
        if (npc.Definition.Number != 236)
        {
            return;
        }

        eventArgs.HasBeenHandled = true;
        eventArgs.LeavesDialogOpen = true;

        await player.InvokeViewPlugInAsync<IGoldenArcherRegistrationResultPlugIn>(
            p => p.RegistrationResultAsync()).ConfigureAwait(false);
    }
}
