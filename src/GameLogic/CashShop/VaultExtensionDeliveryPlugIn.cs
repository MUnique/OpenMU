// <copyright file="VaultExtensionDeliveryPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Delivers the Vault Expansion Certificate of the cash shop by extending the vault of the account.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.VaultExtensionDeliveryPlugIn_Name), Description = nameof(PlugInResources.VaultExtensionDeliveryPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("1F7F1A32-63DE-429D-96A5-2EEB1B0F2B5B")]
public class VaultExtensionDeliveryPlugIn : ICashShopProductDeliveryPlugIn
{
    /// <summary>
    /// The item of the Vault Expansion Certificate.
    /// </summary>
    public static readonly ItemIdentifier VaultExpansionCertificate = new(163, 14);

    /// <inheritdoc />
    public ItemIdentifier Key => VaultExpansionCertificate;

    /// <inheritdoc />
    public bool CanDeliver(CashShopProduct product) => product.Duration == TimeSpan.Zero;

    /// <inheritdoc />
    public ValueTask<CashShopUseResult> DeliverAsync(Player player, CashShopProduct product)
    {
        if (player.Account is not { IsVaultExtended: false } account)
        {
            return ValueTask.FromResult(CashShopUseResult.CannotUse);
        }

        account.IsVaultExtended = true;
        return ValueTask.FromResult(CashShopUseResult.Success);
    }

    /// <inheritdoc />
    public ValueTask DeliveredAsync(Player player, CashShopProduct product)
    {
        return player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CashShopVaultExtended));
    }
}
