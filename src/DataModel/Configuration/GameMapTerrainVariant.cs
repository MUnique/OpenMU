// <copyright file="GameMapTerrainVariant.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;

/// <summary>
/// Defines an alternative terrain of a map, which replaces its <see cref="GameMapDefinition.TerrainData"/>
/// in a certain state, e.g. while the crywolf fortress is occupied.
/// </summary>
[Cloneable]
public partial class GameMapTerrainVariant
{
    /// <summary>
    /// Gets or sets the number, which identifies the variant on its map, e.g. the state in which it applies.
    /// </summary>
    public short Number { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the terrain data.
    /// </summary>
    /// <remarks>
    /// Content of the *.att file in the original server.
    /// </remarks>
    public byte[]? TerrainData { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.Number}: {this.Description}";
    }
}
