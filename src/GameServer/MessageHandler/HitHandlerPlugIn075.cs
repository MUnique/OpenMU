// <copyright file="HitHandlerPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for hit packets for version 0.75.
/// </summary>
[PlugIn]
[Display(Name = "HitHandlerPlugIn 0.75", Description = "Handler for hit packets.")]
[Guid("8C904AEB-1A8C-456D-9BC1-8CE44B8E3794")]
internal class HitHandlerPlugIn075 : HitHandlerPlugInBase
{
    /// <inheritdoc/>
    public override byte Key => 0x15;
}