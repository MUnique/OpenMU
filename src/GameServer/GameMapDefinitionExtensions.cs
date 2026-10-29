// <copyright file="GameMapDefinitionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Extensions for <see cref="GameMapDefinition"/> which concern the communication with the game client.
/// </summary>
internal static class GameMapDefinitionExtensions
{
    /// <summary>
    /// Gets the discriminator of the map as the extended client gets it, in one byte.
    /// </summary>
    /// <param name="map">The map.</param>
    /// <returns>The discriminator of the map; 0, if it doesn't fit into a byte.</returns>
    /// <remarks>
    /// The discriminator is an integer in the configuration. A value which doesn't fit is sent as 0, which stands
    /// for no variant of the map, instead of being cut to the value of another variant (257 to 1, a stage of Devil Square).
    /// </remarks>
    public static byte GetDiscriminatorForClient(this GameMapDefinition map)
    {
        return map.Discriminator is >= byte.MinValue and <= byte.MaxValue ? (byte)map.Discriminator : (byte)0;
    }
}
