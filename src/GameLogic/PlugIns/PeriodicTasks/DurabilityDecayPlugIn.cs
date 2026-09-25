// <copyright file="DurabilityDecayPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Updates the durabilty of time-decaying items.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.DurabilityDecayPlugIn_Name), Description = nameof(PlugInResources.DurabilityDecayPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("D8F4A1C7-3B9E-4D2F-A6C5-1E7B8F3A9C2D")]
public class DurabilityDecayPlugIn : IPeriodicTaskPlugIn
{
    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        await gameContext.ForEachPlayerAsync(async player =>
        {
            if (player.SelectedCharacter != null
                && !player.PlayerState.CurrentState.IsDisconnectedOrFinished()
                && player.Attributes is { } attributes)
            {
                foreach (var item in player.Inventory?.EquippedItems.Where(item => item.IsTimeDecaying()) ?? [])
                {
                    if (item.Durability > 0.0)
                    {
                        var identifier = new ItemIdentifier(item.Definition!.Number, item.Definition.Group);
                        if (identifier == ItemConstants.WizardsRing && item.Level > 0)
                        {
                            // Rings of warrior
                            continue;
                        }

                        var isTransformationRing = item.IsTransformationRing();
                        if (attributes[Stats.IsInSafezone] < 1 || isTransformationRing || item.IsWing())
                        {
                            double decrementWeight = identifier switch
                            {
                                var _ when isTransformationRing => 11.28, // 11.28 / 564 = 0.02
                                var itm when itm == ItemConstants.WizardsRing => 7,
                                var itm when itm == ItemConstants.MoonstonePendant => 6.3,
                                _ => 0.1,
                            };

                            var durationIncrease = attributes[Stats.JewelryAndWingsDurationIncrease];
                            if (durationIncrease == 0 || isTransformationRing)
                            {
                                durationIncrease = 1;
                            }

                            var decrement = decrementWeight / (player.GameContext.Configuration.HitsPerOneItemDurability * durationIncrease);
                            await player.DecreaseItemDurabilityAsync(item, decrement).ConfigureAwait(false);
                        }

                        if (item.Durability == 0.0 && isTransformationRing)
                        {
                            await player.DestroyInventoryItemAsync(item).ConfigureAwait(false);
                        }
                    }
                }
            }

            return;
        }).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void ForceStart()
    {
        // do nothing.
    }
}