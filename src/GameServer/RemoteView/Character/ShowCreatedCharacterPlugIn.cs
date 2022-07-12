// <copyright file="ShowCreatedCharacterPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowCreatedCharacterPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("ShowCreatedCharacterPlugIn", "The default implementation of the IShowCreatedCharacterPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("f5494592-ff7b-4f7f-b0c1-bd242a69fb8f")]
public class ShowCreatedCharacterPlugIn : IShowCreatedCharacterPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowCreatedCharacterPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowCreatedCharacterPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowCreatedCharacterAsync(Character character)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        int Write()
        {
            var size = CharacterCreationSuccessfulRef.Length;
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new CharacterCreationSuccessfulRef(span)
            {
                CharacterName = character.Name,
                CharacterSlot = character.CharacterSlot,
                Level = (ushort)(character.Attributes.FirstOrDefault(a => a.Definition == Stats.Level)?.Value ?? 0),
                Class = (CharacterClassNumber)character.CharacterClass!.Number,
                CharacterStatus = (byte)character.CharacterStatus,
            };

            packet.PreviewData.Fill(0xFF);
            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}