// <copyright file="AddLorenMarketJuliaWarpPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update turns Market Union Member Julia (547) into a working warp NPC for the Loren Market,
/// on both sides. It changes her NPC window so that talking to her opens the warp dialog instead of
/// an empty merchant shop, and spawns a second instance of her in Lorencia, which is the entrance.
/// </summary>
/// <remarks>
/// The actual warp is performed server-side by the <c>EnterMarketPlaceHandlerPlugIn</c> when the
/// player uses the 'Warp' button of the (client-side) Julia window.
/// </remarks>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("ed2f1728-b35c-4a3d-810e-eab5b6e12a82")]
public class AddLorenMarketJuliaWarpPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add Loren Market Julia warp";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Makes Market Union Member Julia (547) a working warp NPC between Lorencia and the Loren Market, on both sides, instead of an empty merchant.";

    private const short JuliaNpcNumber = 547;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddLorenMarketJuliaWarp;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 06, 24, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var julia = gameConfiguration.Monsters.FirstOrDefault(m => m.Number == JuliaNpcNumber);
        if (julia is null)
        {
            return default;
        }

        julia.NpcWindow = NpcWindow.JuliaWarpMarketServer;
        julia.MerchantStore = null;

        var lorencia = gameConfiguration.Maps.FirstOrDefault(m => m.Number == Lorencia.Number);
        if (lorencia is null
            || lorencia.MonsterSpawns.Any(s => s.MonsterDefinition?.Number == JuliaNpcNumber))
        {
            return default;
        }

        var juliaSpawn = context.CreateNew<MonsterSpawnArea>();
        lorencia.MonsterSpawns.Add(juliaSpawn);
        juliaSpawn.SetGuid(JuliaNpcNumber);
        juliaSpawn.GameMap = lorencia;
        juliaSpawn.MonsterDefinition = julia;
        juliaSpawn.Quantity = 1;
        juliaSpawn.SpawnTrigger = SpawnTrigger.Automatic;
        juliaSpawn.Direction = Direction.SouthEast;
        juliaSpawn.X1 = 139;
        juliaSpawn.X2 = 139;
        juliaSpawn.Y1 = 138;
        juliaSpawn.Y2 = 138;

        return default;
    }
}
