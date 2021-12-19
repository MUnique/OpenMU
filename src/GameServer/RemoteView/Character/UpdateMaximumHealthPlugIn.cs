﻿// <copyright file="UpdateMaximumHealthPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IUpdateMaximumHealthPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("UpdateMaximumHealthPlugIn", "The default implementation of the IUpdateMaximumHealthPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("6f8e7d9a-7d15-4e76-a650-8bfa70c7298e")]
public class UpdateMaximumHealthPlugIn : IUpdateMaximumHealthPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateMaximumHealthPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateMaximumHealthPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public void UpdateMaximumHealth()
    {
        if (this._player.Attributes is null
            || !(this._player.Connection?.Connected ?? false))
        {
            return;
        }

        this._player.Connection?.SendMaximumHealthAndShield(
            (ushort)this._player.Attributes[Stats.MaximumHealth],
            (ushort)this._player.Attributes[Stats.MaximumShield]);
    }
}