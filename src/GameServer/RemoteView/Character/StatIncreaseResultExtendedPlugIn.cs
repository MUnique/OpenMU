// <copyright file="StatIncreaseResultExtendedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The extended implementation of the <see cref="IStatIncreaseResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("StatIncreaseResultPlugIn", "The extended implementation of the IStatIncreaseResultPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("4B9AEFF1-B139-45F9-9277-0FBDA7A3C020")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class StatIncreaseResultExtendedPlugIn : IStatIncreaseResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatIncreaseResultExtendedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public StatIncreaseResultExtendedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask StatIncreaseResultAsync(AttributeDefinition attribute, ushort addedPoints)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        await connection.SendCharacterStatIncreaseResponseExtendedAsync(
            attribute.GetStatType(),
            addedPoints,
            (uint)this._player.Attributes![Stats.MaximumHealth],
            (uint)this._player.Attributes![Stats.MaximumMana],
            (uint)this._player.Attributes![Stats.MaximumShield],
            (uint)this._player.Attributes![Stats.MaximumAbility]).ConfigureAwait(false);
    }
}