// <copyright file="ObjectsOutOfScopePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System.Collections.Generic;
    using System.Linq;
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
    [PlugIn("ObjectsOutOfScopePlugIn", "The default implementation of the IObjectsOutOfScopePlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("88cea9ce-c186-4fd0-b6cc-6466d8a7531c")]
    public class ObjectsOutOfScopePlugIn : IObjectsOutOfScopePlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectsOutOfScopePlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ObjectsOutOfScopePlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ObjectsOutOfScope(IEnumerable<IIdentifiable> objects)
        {
            var count = objects.Count();
            using var writer = this.player.Connection.StartSafeWrite(MapObjectOutOfScope.HeaderType, MapObjectOutOfScope.GetRequiredSize(count));
            var packet = new MapObjectOutOfScope(writer.Span)
            {
                ObjectCount = (byte)count,
            };

            int i = 0;
            foreach (var m in objects)
            {
                var objectId = packet[i];
                objectId.Id = m.GetId(this.player);
                i++;
            }

            writer.Commit();
        }
    }
}