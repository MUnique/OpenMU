// <copyright file="StatIncreaseResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IStatIncreaseResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("StatIncreaseResultPlugIn", "The default implementation of the IStatIncreaseResultPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("ce603b3c-cf25-426f-9cb9-5cc367843de8")]
public class StatIncreaseResultPlugIn : IStatIncreaseResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatIncreaseResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public StatIncreaseResultPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask StatIncreaseResultAsync(AttributeDefinition attribute, bool success)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        await connection.SendCharacterStatIncreaseResponseAsync(
            success,
            attribute.GetStatType(),
            attribute == Stats.BaseEnergy
                ? (ushort)this._player.Attributes![Stats.MaximumMana]
                : attribute == Stats.BaseVitality
                    ? (ushort)this._player.Attributes![Stats.MaximumHealth]
                    : default,
            (ushort)this._player.Attributes![Stats.MaximumShield],
            (ushort)this._player.Attributes[Stats.MaximumAbility]);
    }
}