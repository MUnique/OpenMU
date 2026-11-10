// <copyright file="LuckyCoinFeaturePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.LuckyCoins;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The lucky coin event of the NPC Delgado: players register lucky coins, or exchange them for items.
/// When it's deactivated, the players can't register or exchange lucky coins.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.LuckyCoinFeaturePlugIn_Name), Description = nameof(PlugInResources.LuckyCoinFeaturePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("3D6F9337-6A79-4F89-B995-4B7E722E63D1")]
public class LuckyCoinFeaturePlugIn : IFeaturePlugIn, ISupportCustomConfiguration<LuckyCoinConfiguration>, ISupportDefaultCustomConfiguration
{
    /// <inheritdoc />
    public LuckyCoinConfiguration? Configuration { get; set; }

    /// <summary>
    /// Gets the configuration of the lucky coin event of the game context.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The configuration; <c>null</c>, if the lucky coin event is deactivated.</returns>
    public static LuckyCoinConfiguration? GetActiveConfiguration(IGameContext gameContext)
    {
        return gameContext.FeaturePlugIns.GetPlugIn<LuckyCoinFeaturePlugIn>() is { } plugIn
            ? plugIn.Configuration ?? new LuckyCoinConfiguration()
            : null;
    }

    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        return new LuckyCoinConfiguration();
    }
}
