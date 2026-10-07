// <copyright file="CashShopFeaturePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The feature plugin of the cash shop. When it's deactivated, the cash shop is closed,
/// and it holds the <see cref="CashShopSettings"/>.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CashShopFeaturePlugIn_Name), Description = nameof(PlugInResources.CashShopFeaturePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("8F5E4988-DB22-4313-BDBD-09A680AD92F8")]
public class CashShopFeaturePlugIn : IFeaturePlugIn, ISupportCustomConfiguration<CashShopSettings>, ISupportDefaultCustomConfiguration
{
    /// <inheritdoc />
    public CashShopSettings? Configuration { get; set; }

    /// <summary>
    /// Gets the settings of the cash shop.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The settings; <c>null</c>, if the cash shop feature is deactivated.</returns>
    public static CashShopSettings? GetSettings(IGameContext gameContext)
    {
        return gameContext.FeaturePlugIns.GetPlugIn<CashShopFeaturePlugIn>() is { } plugIn
            ? plugIn.Configuration ?? new CashShopSettings()
            : null;
    }

    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        return new CashShopSettings();
    }
}
