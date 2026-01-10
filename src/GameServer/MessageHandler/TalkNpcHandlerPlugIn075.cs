// <copyright file="TalkNpcHandlerPlugIn075.cs" company="MUnique">
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
[PlugIn]
[Display(Name = nameof(PlugInResources.TalkNpcHandlerPlugIn075_Name), Description = nameof(PlugInResources.TalkNpcHandlerPlugIn075_Description), ResourceType = typeof(PlugInResources))]
[Guid("61732821-5881-41C1-931D-88CFE2A075FE")]
[MaximumClient(0, 99, ClientLanguage.Invariant)]
internal class TalkNpcHandlerPlugIn075 : TalkNpcHandlerPlugInBase
{
    /// <inheritdoc/>
    protected override TalkNpcAction TalkNpcAction { get; } = new TalkNpcAction075();
}