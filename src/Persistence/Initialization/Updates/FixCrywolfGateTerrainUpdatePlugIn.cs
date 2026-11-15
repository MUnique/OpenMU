// <copyright file="FixCrywolfGateTerrainUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Makes the south entry of the Crywolf gate in Valley of Loren walkable.
/// </summary>
/// <remarks>
/// Valley of Loren is the Castle Siege map, and the game client switches its terrain with the siege: outside of the
/// siege it loads <c>EncTerrain31.att</c>, which lets the player enter the Crywolf gate (enter gate 117) only from
/// the south, during the siege <c>EncTerrain312.att</c>, which opens it only from the north. The seeded terrain had
/// just the north entry. Because a walk stops at the first blocked tile, players couldn't reach the gate outside of
/// the siege. This update clears the blocked flag of the south entry and keeps the north entry, so the gate works
/// with both terrains of the client. Other flags and all other tiles are kept.
/// </remarks>
[PlugIn]
[Display(Name = nameof(Properties.PlugInResources.FixCrywolfGateTerrainUpdatePlugIn_Name), Description = nameof(Properties.PlugInResources.FixCrywolfGateTerrainUpdatePlugIn_Description), ResourceType = typeof(Properties.PlugInResources))]
[Guid("D8D93F9C-9E73-43D3-92B0-AB6B9096322E")]
public class FixCrywolfGateTerrainUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The size of the header of the terrain data, before the attributes of the tiles.
    /// </summary>
    private const int TerrainHeaderSize = 3;

    /// <summary>
    /// The number of tiles in one row of the terrain.
    /// </summary>
    private const int TerrainSize = 256;

    /// <summary>
    /// Gets the tiles of the south entry of the Crywolf gate, which are walkable in the client's terrain outside of
    /// the siege (<c>EncTerrain31.att</c>).
    /// </summary>
    internal static IReadOnlyList<(byte X, byte Y)> SouthEntryTiles { get; } =
    [
        (161, 40),
        (161, 41), (162, 41), (163, 41), (164, 41),
        (161, 42), (162, 42), (163, 42), (164, 42),
        (161, 43), (162, 43), (163, 43), (164, 43),
    ];

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => Properties.PlugInResources.FixCrywolfGateTerrainUpdatePlugIn_Name;

    /// <inheritdoc />
    public override string Description => Properties.PlugInResources.FixCrywolfGateTerrainUpdatePlugIn_Description;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 14, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        const int minimumLength = TerrainHeaderSize + (TerrainSize * TerrainSize);
        const byte blocked = (byte)TerrainAttributeType.Blocked;
        foreach (var map in gameConfiguration.Maps.Where(map => map is { Number: ValleyOfLoren.Number, Discriminator: 0 }))
        {
            if (map.TerrainData is not { Length: >= minimumLength } terrainData
                || SouthEntryTiles.All(tile => (terrainData[GetOffset(tile)] & blocked) == 0))
            {
                continue;
            }

            // A new array, so that the persistence layer recognizes the change.
            var fixedTerrainData = terrainData.ToArray();
            foreach (var tile in SouthEntryTiles)
            {
                fixedTerrainData[GetOffset(tile)] &= unchecked((byte)~blocked);
            }

            map.TerrainData = fixedTerrainData;
        }

        return ValueTask.CompletedTask;
    }

    private static int GetOffset((byte X, byte Y) tile) => TerrainHeaderSize + (tile.Y * TerrainSize) + tile.X;
}
