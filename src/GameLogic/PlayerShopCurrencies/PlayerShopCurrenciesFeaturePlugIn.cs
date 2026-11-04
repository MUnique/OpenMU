// <copyright file="PlayerShopCurrenciesFeaturePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerShopCurrencies;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The feature plugin which allows players to define the prices of the items in their
/// personal store in other currencies than zen, e.g. in jewels or account coins.
/// When it's deactivated, only zen prices are possible.
/// </summary>
/// <remarks>
/// Only the open source client offers the currencies in its user interface. With the original client,
/// the prices can be set with a chat command (see <see cref="PlugIns.ChatCommands.ShopPriceChatCommandPlugIn"/>), but it can't
/// see or buy items with other prices than zen.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.PlayerShopCurrenciesFeaturePlugIn_Name), Description = nameof(PlugInResources.PlayerShopCurrenciesFeaturePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("FF970BE2-DF92-4371-9FE0-C16E9DCA7AB9")]
public class PlayerShopCurrenciesFeaturePlugIn : IFeaturePlugIn, IDisabledByDefault, ISupportCustomConfiguration<PlayerShopCurrenciesConfiguration>, ISupportDefaultCustomConfiguration
{
    /// <inheritdoc />
    public PlayerShopCurrenciesConfiguration? Configuration { get; set; }

    /// <summary>
    /// Gets the configuration of the player shop currencies.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The configuration; <c>null</c>, if the feature is deactivated.</returns>
    public static PlayerShopCurrenciesConfiguration? GetActiveConfiguration(IGameContext gameContext)
    {
        return gameContext.FeaturePlugIns.GetPlugIn<PlayerShopCurrenciesFeaturePlugIn>() is { } plugIn
            ? plugIn.Configuration ?? new PlayerShopCurrenciesConfiguration()
            : null;
    }

    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        return new PlayerShopCurrenciesConfiguration();
    }
}
