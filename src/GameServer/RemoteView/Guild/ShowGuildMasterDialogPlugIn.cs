﻿// <copyright file="ShowGuildMasterDialogPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowGuildMasterDialogPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("ShowGuildMasterDialogPlugIn", "The default implementation of the IShowGuildMasterDialogPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("77d430e0-8bed-425b-8bb5-7bbafa9bfbff")]
public class ShowGuildMasterDialogPlugIn : IShowGuildMasterDialogPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowGuildMasterDialogPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowGuildMasterDialogPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public void ShowGuildMasterDialog()
    {
        this._player.Connection?.SendShowGuildMasterDialog();
    }
}