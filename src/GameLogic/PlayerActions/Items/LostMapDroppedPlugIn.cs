// <copyright file="LostMapDroppedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin transforms a stack of symbol of kundun into a lost map.
/// todo: implement plugin configuration to resolve magic numbers.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.LostMapDroppedPlugIn_Name), Description = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.LostMapDroppedPlugIn_Description), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
[Guid("F6DB10E0-AE7F-4BC6-914F-B858763C5CF7")]
public sealed class LostMapDroppedPlugIn : IItemDropPlugIn
{
    /// <inheritdoc />
    public async ValueTask HandleItemDropAsync(Player player, Item item, Point target, IItemDropPlugIn.ItemDropArguments cancelArgs)
    {
        if (!item.IsLostMap())
        {
            return;
        }

        cancelArgs.WasHandled = true;
        if (player.CurrentMap is null)
        {
            return;
        }

        if (item.Level is < 1 or > KalimaGate.MaximumLevel)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.InvalidLostMap)).ConfigureAwait(false);
            return;
        }

        cancelArgs.Success = await KalimaGate.TryOpenAsync(player, item.Level, target).ConfigureAwait(false);
    }
}