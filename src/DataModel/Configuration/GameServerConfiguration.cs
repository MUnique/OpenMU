// <copyright file="GameServerConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;

/// <summary>
/// Defines the game server configuration.
/// </summary>
[AggregateRoot]
[Cloneable]
public partial class GameServerConfiguration
{
    /// <summary>
    /// Gets or sets the maximum number of players which can connect.
    /// </summary>
    public short MaximumPlayers { get; set; }

    /// <summary>
    /// Gets or sets the maps which should be hosted on the server.
    /// </summary>
    public virtual ICollection<GameMapDefinition> Maps { get; protected set; } = null!;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Default ({this.MaximumPlayers} players)"; // TODO Add Description field
    }
}