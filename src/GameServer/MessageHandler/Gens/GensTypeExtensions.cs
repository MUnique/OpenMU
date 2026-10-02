// <copyright file="GensTypeExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Gens;

using PacketGensType = MUnique.OpenMU.Network.Packets.ClientToServer.GensType;

/// <summary>
/// Extensions to convert the gens type of the requests into the <see cref="DataModel.Entities.GensType"/>.
/// </summary>
internal static class GensTypeExtensions
{
    /// <summary>
    /// Converts the gens type of a request into the one of the data model.
    /// </summary>
    /// <param name="gens">The gens type of the request.</param>
    /// <returns>The gens type of the data model.</returns>
    public static DataModel.Entities.GensType ToGensType(this PacketGensType gens)
    {
        return gens switch
        {
            PacketGensType.Duprian => DataModel.Entities.GensType.Duprian,
            PacketGensType.Vanert => DataModel.Entities.GensType.Vanert,
            _ => DataModel.Entities.GensType.None,
        };
    }
}
