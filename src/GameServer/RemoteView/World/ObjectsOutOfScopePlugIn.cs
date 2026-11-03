// <copyright file="ObjectsOutOfScopePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IObjectsOutOfScopePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.ObjectsOutOfScopePlugIn_Name), Description = nameof(PlugInResources.ObjectsOutOfScopePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("88cea9ce-c186-4fd0-b6cc-6466d8a7531c")]
public class ObjectsOutOfScopePlugIn : IObjectsOutOfScopePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectsOutOfScopePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ObjectsOutOfScopePlugIn(RemotePlayer player) => this._player = player;

    /// <summary>
    /// Gets the most objects one packet can carry: its length is a single byte.
    /// </summary>
    internal static int MaximumObjectsPerPacket { get; } = (byte.MaxValue - MapObjectOutOfScopeRef.GetRequiredSize(0)) / (MapObjectOutOfScopeRef.GetRequiredSize(1) - MapObjectOutOfScopeRef.GetRequiredSize(0));

    /// <inheritdoc/>
    public async ValueTask ObjectsOutOfScopeAsync(IEnumerable<IIdentifiable> objects)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        // The packet's length is a single byte: more objects than fit go in further packets. Otherwise the
        // length wraps around and the client reads garbage, e.g. when 126 or more monsters leave the view at once.
        var ids = objects.Select(o => o.GetId(this._player)).ToList();
        for (var start = 0; start < ids.Count; start += MaximumObjectsPerPacket)
        {
            var chunk = ids.GetRange(start, Math.Min(MaximumObjectsPerPacket, ids.Count - start));
            int Write()
            {
                var size = MapObjectOutOfScopeRef.GetRequiredSize(chunk.Count);
                var span = connection.Output.GetSpan(size)[..size];
                var packet = new MapObjectOutOfScopeRef(span)
                {
                    ObjectCount = (byte)chunk.Count,
                };

                for (var i = 0; i < chunk.Count; i++)
                {
                    var objectId = packet[i];
                    objectId.Id = chunk[i];
                }

                return size;
            }

            await connection.SendAsync(Write).ConfigureAwait(false);
        }
    }
}