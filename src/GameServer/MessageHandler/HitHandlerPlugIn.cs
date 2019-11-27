// <copyright file="HitHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for hit packets.
    /// </summary>
    [PlugIn("HitHandlerPlugIn", "Handler for hit packets.")]
    [Guid("698b8db9-472a-42dd-bdfe-f6b4ba45595e")]
    [MinimumClient(0, 90, ClientLanguage.English)]
    internal class HitHandlerPlugIn : HitHandlerPlugInBase
    {
        /// <inheritdoc/>
        public override byte Key => HitRequest.Code;
    }
}
