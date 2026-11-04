// <copyright file="ItemRegistrationAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items.ItemRegistration;

using System.Linq;

/// <summary>
/// Action to register an item at a registration NPC using strategies.
/// </summary>
public class ItemRegistrationAction
{
    /// <summary>
    /// Registers the item.
    /// </summary>
    /// <param name="player">The player who is registering.</param>
    public async ValueTask<bool> RegisterAsync(Player player)
    {
        if (player.OpenedNpc == null)
        {
            return false;
        }

        short npcNumber = player.OpenedNpc.Definition.Number;

        var strategy = player.GameContext.PlugInManager.GetStrategy<short, IItemRegistrationStrategy>(npcNumber);
        if (strategy == null)
        {
            await player.ShowBlueMessageAsync("Registration for this NPC is not yet available. It can be expanded in the future.").ConfigureAwait(false);
            return false;
        }

        var feature = player.GameContext.FeaturePlugIns.GetPlugIn<PlugIns.ItemRegistration.ItemRegistrationFeaturePlugIn>();
        if (feature?.Configuration is not { } config)
        {
            return false;
        }

        var rule = config.Rules.FirstOrDefault(r => r.NpcNumber == npcNumber);

        if (rule == null)
        {
            return false;
        }

        await strategy.RegisterAsync(player, rule).ConfigureAwait(false);
        return true;
    }
}