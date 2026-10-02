// <copyright file="FixVulcanusWarpIndexUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update corrects the index of the warp entry of Vulcanus to 42, which the season 6 game client requests.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FixVulcanusWarpIndexUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FixVulcanusWarpIndexUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("6B0E3D72-8F14-4C59-A2D6-1E9F7C3B5A40")]
public class FixVulcanusWarpIndexUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Vulcanus warp index";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update corrects the index of the warp entry of Vulcanus to 42, which the season 6 game client requests.";

    /// <summary>
    /// The index of the warp entry of Vulcanus in the warp list of the season 6 game client.
    /// </summary>
    internal const ushort VulcanusWarpIndex = 42;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 10, 01, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.WarpList.All(warpInfo => warpInfo.Index != VulcanusWarpIndex)
            && gameConfiguration.WarpList.FirstOrDefault(warpInfo => warpInfo.Gate?.Map?.Number == Vulcanus.Number) is { } vulcanus)
        {
            vulcanus.Index = VulcanusWarpIndex;
        }

        return ValueTask.CompletedTask;
    }
}
