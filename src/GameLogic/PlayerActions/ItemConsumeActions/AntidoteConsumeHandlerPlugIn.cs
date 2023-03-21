// -----------------------------------------------------------------------
// <copyright file="AntidoteConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for the antidote potion. It removes the poison effect from the player.
/// </summary>
[Guid("F838B348-DAA5-475B-BCED-41A076D08948")]
[PlugIn(nameof(AntidoteConsumeHandlerPlugIn), "Plugin which handles the antidote consumption.")]
public class AntidoteConsumeHandlerPlugIn : BaseConsumeHandlerPlugIn
{
    private const short PoisonEffectNumber = 0x37;

    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.Antidote;

    /// <inheritdoc />
    public override async ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        if (await base.ConsumeItemAsync(player, item, targetItem, fruitUsage).ConfigureAwait(false))
        {
            if (player.MagicEffectList.ActiveEffects.TryGetValue(PoisonEffectNumber, out var effect))
            {
                effect.Dispose();
            }

            return true;
        }

        return false;
    }
}