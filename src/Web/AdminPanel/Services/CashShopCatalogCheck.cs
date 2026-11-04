// <copyright file="CashShopCatalogCheck.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.CashShop;
using MUnique.OpenMU.Web.AdminPanel.Properties;

/// <summary>
/// Checks the packages of the cash shop catalog for problems, which the catalog editor shows.
/// </summary>
public static class CashShopCatalogCheck
{
    /// <summary>
    /// Determines whether a player can buy the package: it's for sale, and its products can be
    /// delivered — all of them for a bundle, at least one price option otherwise.
    /// </summary>
    /// <param name="package">The package.</param>
    /// <returns><c>true</c>, if the package can be bought; otherwise, <c>false</c>.</returns>
    public static bool IsSellable(CashShopPackage package)
    {
        if (!package.IsForSale || package.Products.Count == 0)
        {
            return false;
        }

        // The game server refuses negative prices, which would add coins.
        return package.IsBundle
            ? package.Price >= 0 && package.Products.All(product => product.CanBeDelivered())
            : package.Products.Any(product => product.CanBeDelivered() && product.Price >= 0);
    }

    /// <summary>
    /// Gets the problems of a package, as localized texts.
    /// </summary>
    /// <param name="configuration">The cash shop configuration which contains the package.</param>
    /// <param name="package">The package.</param>
    /// <returns>The problems; empty, if there are none.</returns>
    public static IReadOnlyList<string> GetIssues(CashShopConfiguration configuration, CashShopPackage package)
    {
        var issues = new List<string>();
        if (package.Products.Count == 0)
        {
            issues.Add(Resources.CashShopIssueNoProducts);
        }

        if (configuration.Packages.Count(p => p.PackageSequence == package.PackageSequence) > 1)
        {
            issues.Add(string.Format(Resources.CashShopIssueDuplicatePackageSequence, package.PackageSequence));
        }

        foreach (var duplicate in package.Products.GroupBy(p => p.PriceSequence).Where(g => g.Count() > 1))
        {
            issues.Add(string.Format(Resources.CashShopIssueDuplicatePriceSequence, duplicate.Key));
        }

        if (package.IsBundle && package.Price < 0)
        {
            issues.Add(Resources.CashShopIssueNegativeBundlePrice);
        }

        foreach (var product in package.Products.OrderBy(p => p.PriceSequence))
        {
            if (!package.IsBundle && product.Price < 0)
            {
                issues.Add(string.Format(Resources.CashShopIssueNegativePrice, product.PriceSequence));
            }

            if (product.Quantity < 1)
            {
                issues.Add(string.Format(Resources.CashShopIssueInvalidQuantity, product.PriceSequence));
            }

            if (product.ItemDefinition is null)
            {
                issues.Add(string.Format(Resources.CashShopIssueMissingItem, product.PriceSequence));
            }
            else if (product.Duration > TimeSpan.Zero)
            {
                issues.Add(string.Format(Resources.CashShopIssueTimeLimited, product.PriceSequence));
            }
        }

        return issues;
    }
}
