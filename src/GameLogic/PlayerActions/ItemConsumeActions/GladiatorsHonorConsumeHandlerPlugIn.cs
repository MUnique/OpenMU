// <copyright file="GladiatorsHonorConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The consume handler for the Gladiator's Honor, which applies its effect only in Vulcanus,
/// like the game client describes it.
/// </summary>
/// <remarks>
/// The experience bonus of its effect is applied by the <see cref="VulcanusExperiencePlugIn"/>,
/// so it only counts while the player stays in Vulcanus.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.GladiatorsHonorConsumeHandlerPlugIn_Name), Description = nameof(PlugInResources.GladiatorsHonorConsumeHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("FBB2F1AB-8097-41B0-A10E-8323C10D053A")]
public class GladiatorsHonorConsumeHandlerPlugIn : ApplyMagicEffectConsumeHandlerPlugIn, ISupportCustomConfiguration<VulcanusMapConfiguration>, ISupportDefaultCustomConfiguration
{
    /// <inheritdoc />
    public VulcanusMapConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.GladiatorsHonor;

    /// <inheritdoc />
    public override async ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        var mapNumber = (this.Configuration ??= new VulcanusMapConfiguration()).MapNumber;
        if (player.CurrentMap?.MapId != mapNumber)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.ItemCantBeUsedOnThisMap)).ConfigureAwait(false);
            return false;
        }

        return await base.ConsumeItemAsync(player, item, targetItem, fruitUsage).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public object CreateDefaultConfig() => new VulcanusMapConfiguration();
}
