// <copyright file="RestrictWarpsOutsideOfOriginalWarpWindowPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update makes the warps which the original client doesn't offer in its warp window available to game
/// masters only (see <see cref="WarpInfo.IsWarpable"/>), like a new season 6 database has them: Crywolf (26),
/// which is reached by walking, Loren Market (44), which is reached through an NPC, and LaCleon (48), which
/// the client offers as index 34.
/// </summary>
/// <remarks>
/// It's optional, because it changes what the players of a running server can do: these warps were available
/// to them before, e.g. with the move chat command. A warp whose map was changed is left alone.
/// </remarks>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.RestrictWarpsOutsideOfOriginalWarpWindowPlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.RestrictWarpsOutsideOfOriginalWarpWindowPlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("B1F16CD8-C941-403E-A4B8-18C5F303EF14")]
public class RestrictWarpsOutsideOfOriginalWarpWindowPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Restrict warps outside of the original warp window";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Makes the warps which the original client doesn't offer in its warp window (Crywolf, Loren Market and the second LaCleon warp) available to game masters only.";

    /// <summary>
    /// The indexes of the warps and the numbers of the maps they lead to.
    /// </summary>
    private static readonly IReadOnlyDictionary<int, short> Warps = new Dictionary<int, short>
    {
        { 26, CrywolfFortress.Number },
        { 44, LorenMarket.Number },
        { 48, Raklion.Number },
    };

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 13, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var warp in gameConfiguration.WarpList)
        {
            if (Warps.TryGetValue(warp.Index, out var mapNumber) && warp.Gate?.Map?.Number == mapNumber)
            {
                warp.IsWarpable = false;
            }
        }

        return ValueTask.CompletedTask;
    }
}
