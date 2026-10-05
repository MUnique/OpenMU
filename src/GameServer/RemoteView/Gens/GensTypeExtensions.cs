// <copyright file="GensTypeExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Gens;

using GensType = MUnique.OpenMU.DataModel.Entities.GensType;
using PacketGensType = MUnique.OpenMU.Network.Packets.ServerToClient.GensType;

/// <summary>
/// Extensions to convert the <see cref="GensType"/> into the one of the packets.
/// </summary>
internal static class GensTypeExtensions
{
    /// <summary>
    /// Converts the gens type into the one of the packets.
    /// </summary>
    /// <param name="gens">The gens type.</param>
    /// <returns>The gens type of the packets.</returns>
    public static PacketGensType ToPacketGensType(this GensType gens)
    {
        return gens switch
        {
            GensType.Duprian => PacketGensType.Duprian,
            GensType.Vanert => PacketGensType.Vanert,
            _ => PacketGensType.Undefined,
        };
    }
}
