// <copyright file="GensFeaturePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Gens;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The gens system, in which the players can join one of the two families (gens) Duprian and Vanert.
/// When it's deactivated, the players can't join or leave a gens.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.GensFeaturePlugIn_Name), Description = nameof(PlugInResources.GensFeaturePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("3C2B9D57-6E1A-4F84-9B0D-7A5E2C8F1D46")]
public class GensFeaturePlugIn : IFeaturePlugIn, ISupportCustomConfiguration<GensConfiguration>, ISupportDefaultCustomConfiguration
{
    /// <inheritdoc />
    public GensConfiguration? Configuration { get; set; }

    /// <summary>
    /// Gets the configuration of the gens system of the game context.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The configuration; <c>null</c>, if the gens system is deactivated.</returns>
    public static GensConfiguration? GetConfiguration(IGameContext gameContext)
    {
        return gameContext.FeaturePlugIns.GetPlugIn<GensFeaturePlugIn>() is { } plugIn
            ? plugIn.Configuration ?? new GensConfiguration()
            : null;
    }

    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        return new GensConfiguration();
    }
}
