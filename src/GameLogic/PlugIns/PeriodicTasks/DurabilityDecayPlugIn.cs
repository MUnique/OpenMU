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
/// <remarks>
/// The jewelry and the wings wear like the known server sources of the original game describe it:
/// every <see cref="WearPeriodSeconds"/> seconds, the wear of the item is added to a counter. When the counter exceeds the threshold, which is
/// <see cref="DataModel.Configuration.GameConfiguration.HitsPerOneItemDurability"/> increased by the duration increase
/// of the character, the item loses one point and the counter starts over at 0, so the excess of the last period is lost.
/// One point therefore takes a whole number of periods, which this plug-in spreads evenly over its ticks of one second.
/// Spreading the wear itself instead would ignore the lost excess, and items with a high wear, like the Wizard's Ring,
/// would wear out noticeably faster.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.DurabilityDecayPlugIn_Name), Description = nameof(PlugInResources.DurabilityDecayPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("D8F4A1C7-3B9E-4D2F-A6C5-1E7B8F3A9C2D")]
public class DurabilityDecayPlugIn : IPeriodicTaskPlugIn
{
    /// <summary>
    /// The seconds between two additions of the wear, as in the known server sources.
    /// </summary>
    private const int WearPeriodSeconds = 10;

    /// <summary>
    /// The durability decrement of a transformation ring per second, multiplied with
    /// <see cref="DataModel.Configuration.GameConfiguration.HitsPerOneItemDurability"/>: 11.28 / 564 = 0.02 per second.
    /// </summary>
    private const double TransformationRingWeight = 11.28;

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
                            var hitsPerOneItemDurability = player.GameContext.Configuration.HitsPerOneItemDurability;
                            double decrement;
                            if (isTransformationRing)
                            {
                                decrement = TransformationRingWeight / hitsPerOneItemDurability;
                            }
                            else
                            {
                                double wearPerPeriod = identifier switch
                                {
                                    var itm when itm == ItemConstants.WizardsRing => 70,
                                    var itm when itm == ItemConstants.MoonstonePendant => 63,
                                    _ => 1,
                                };

                                var durationIncrease = attributes[Stats.JewelryAndWingsDurationIncrease];
                                if (durationIncrease == 0)
                                {
                                    durationIncrease = 1;
                                }

                                decrement = GetDecrementPerSecond(wearPerPeriod, hitsPerOneItemDurability * durationIncrease);
                            }

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

    /// <summary>
    /// Gets the durability decrement per second of an item which wears out by periods.
    /// </summary>
    /// <param name="wearPerPeriod">The wear which is added to the counter every <see cref="WearPeriodSeconds"/> seconds.</param>
    /// <param name="threshold">The threshold which the counter has to exceed, so that the item loses one point.</param>
    /// <returns>The durability decrement per second.</returns>
    internal static double GetDecrementPerSecond(double wearPerPeriod, double threshold)
    {
        var periodsPerPoint = Math.Floor(threshold / wearPerPeriod) + 1;
        return 1 / (periodsPerPoint * WearPeriodSeconds);
    }
}