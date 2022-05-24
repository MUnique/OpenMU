// <copyright file="ChangeTerrainAttributesViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MiniGames;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;
using System.Runtime.InteropServices;

/// <summary>
/// The default implementation of the <see cref="IChangeTerrainAttributesViewPlugin"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ChangeTerrainAttributesViewPlugIn), "The default implementation of the IChangeTerrainAttributesViewPlugin which is forwarding everything to the game client with specific data packets.")]
[Guid("D408B6C5-E4DE-496F-B911-F2DA893E9A96")]
public class ChangeTerrainAttributesViewPlugIn : IChangeTerrainAttributesViewPlugin
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChangeTerrainAttributesViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ChangeTerrainAttributesViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public void ChangeAttributes(bool type, TerrainAttributeType attribute, bool setAttribute, IReadOnlyCollection<(byte StartX, byte StartY, byte EndX, byte EndY)> areas)
    {
        if (this._player.Connection is null)
        {
            return;
        }

        const int maxAreas = 10;
        using var writer = this._player.Connection.StartSafeWrite(0xC1, ChangeTerrainAttributes.GetRequiredSize(Math.Min(maxAreas, areas.Count)));
        var message = new ChangeTerrainAttributes(writer.Span)
        {
            Type = type,
            Attribute = Convert(attribute),
            SetAttribute = setAttribute,
            AreaCount = (byte)areas.Count,
        };

        var i = 0;
        foreach (var (startX, startY, endX, endY) in areas.Take(maxAreas))
        {
            var item = message[i];
            item.StartX = startX;
            item.StartY = startY;
            item.EndX = endX;
            item.EndY = endY;
            i++;
        }

        writer.Commit();
    }

    private static ChangeTerrainAttributes.TerrainAttributeType Convert(TerrainAttributeType type)
    {
        return type switch
        {
            TerrainAttributeType.Safezone => ChangeTerrainAttributes.TerrainAttributeType.Safezone,
            TerrainAttributeType.Character => ChangeTerrainAttributes.TerrainAttributeType.Character,
            TerrainAttributeType.Blocked => ChangeTerrainAttributes.TerrainAttributeType.Blocked,
            TerrainAttributeType.NoGround => ChangeTerrainAttributes.TerrainAttributeType.NoGround,
            TerrainAttributeType.Water => ChangeTerrainAttributes.TerrainAttributeType.Water,
            _ => throw new ArgumentException($"Unhandled enum value {type}."),
        };
    }
}