// <copyright file="InventoryExtensionDeliveryPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Delivers the Magic Backpack of the cash shop by adding an inventory extension to the character.
/// </summary>
/// <remarks>
/// The client learns the number of inventory extensions and the server builds the inventory when
/// the character enters the world, so the new extension can be used after selecting the character again.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.InventoryExtensionDeliveryPlugIn_Name), Description = nameof(PlugInResources.InventoryExtensionDeliveryPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("B53546ED-BF6D-43EE-9F51-E8663B4E663D")]
public class InventoryExtensionDeliveryPlugIn : ICashShopProductDeliveryPlugIn, ISupportCustomConfiguration<InventoryExtensionDeliveryConfiguration>, ISupportDefaultCustomConfiguration
{
    /// <summary>
    /// The item of the Magic Backpack.
    /// </summary>
    public static readonly ItemIdentifier MagicBackpack = new(162, 14);

    /// <inheritdoc />
    public ItemIdentifier Key => MagicBackpack;

    /// <inheritdoc />
    public InventoryExtensionDeliveryConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public bool CanDeliver(CashShopProduct product) => product.Duration == TimeSpan.Zero;

    /// <inheritdoc />
    public ValueTask<CashShopUseResult> DeliverAsync(Player player, CashShopProduct product)
    {
        var configuration = this.Configuration ?? new InventoryExtensionDeliveryConfiguration();
        var maximumExtensions = Math.Min(configuration.MaximumExtensions, InventoryConstants.MaximumNumberOfExtensions);
        if (player.SelectedCharacter is not { } character
            || character.InventoryExtensions >= maximumExtensions)
        {
            return ValueTask.FromResult(CashShopUseResult.CannotUse);
        }

        character.InventoryExtensions++;
        return ValueTask.FromResult(CashShopUseResult.Success);
    }

    /// <inheritdoc />
    public ValueTask DeliveredAsync(Player player, CashShopProduct product)
    {
        return player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CashShopInventoryExtended));
    }

    /// <inheritdoc />
    public object CreateDefaultConfig() => new InventoryExtensionDeliveryConfiguration();
}
