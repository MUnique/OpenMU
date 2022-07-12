// <copyright file="NpcDialogClosedPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.NPC;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowItemCraftingResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(NpcDialogClosedPlugIn075), "The default implementation of the INpcDialogClosedPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("BAC39AFE-F277-48FD-BB21-A3F905EE0E73")]
[MaximumClient(0, 255, ClientLanguage.Invariant)]
public class NpcDialogClosedPlugIn075 : INpcDialogClosedPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="NpcDialogClosedPlugIn075"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public NpcDialogClosedPlugIn075(RemotePlayer player)
    {
        this._player = player;
    }

    /// <inheritdoc />
    public async ValueTask DialogClosedAsync(MonsterDefinition npc)
    {
        if (npc.NpcWindow == NpcWindow.ChaosMachine)
        {
            await this._player.Connection.SendCraftingDialogClosed075Async().ConfigureAwait(false);
        }
    }
}