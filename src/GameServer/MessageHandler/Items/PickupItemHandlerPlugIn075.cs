// <copyright file="PickupItemHandlerPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for item pickup packets.
    /// </summary>
    [PlugIn("PickupItemHandlerPlugIn 0.75", "Handler for item pickup packets for version 0.75.")]
    [Guid("185512C3-F5B7-489F-A1C1-DF07146560A4")]
    [MinimumClient(0, 75, ClientLanguage.Invariant)]
    internal class PickupItemHandlerPlugIn075 : PickupItemHandlerPlugInBase
    {
        /// <inheritdoc />
        public override bool IsEncryptionExpected => false;
    }
}