// <copyright file="TalkNpcHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for talk npc request packets.
/// </summary>
[PlugIn(nameof(TalkNpcHandlerPlugIn), "Handler for talk npc request packets.")]
[Guid("b196fd5e-706d-41a2-ba07-72a3b184151d")]
[MinimumClient(1, 0, ClientLanguage.Invariant)]
internal class TalkNpcHandlerPlugIn : TalkNpcHandlerPlugInBase
{
    /// <inheritdoc />
    public override bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    protected override TalkNpcAction TalkNpcAction { get; } = new();
}