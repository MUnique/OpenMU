// <copyright file="UpdatePartyListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Party;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Party;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IUpdatePartyListPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(UpdatePartyListPlugIn), "The default implementation of the IUpdatePartyListPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("bf880a4b-f4f6-41f0-adff-6eab0e99d985")]
[MinimumClient(0, 90, ClientLanguage.Invariant)]
public class UpdatePartyListPlugIn : IUpdatePartyListPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdatePartyListPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdatePartyListPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask UpdatePartyListAsync()
    {
        var connection = this._player.Connection;
        var party = this._player.Party;
        if (party is null || connection is null)
        {
            return;
        }

        var partyList = party.PartyList;
        var partyListCount = partyList.Count;
        int Write()
        {
            var size = PartyListRef.GetRequiredSize(partyListCount);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new PartyListRef(span)
            {
                Count = (byte)partyListCount,
            };

            for (byte i = 0; i < partyListCount; i++)
            {
                var partyMember = partyList[i];
                var partyMemberBlock = packet[i];
                partyMemberBlock.Index = i;
                partyMemberBlock.Name = partyMember.Name;
                partyMemberBlock.MapId = (byte)(partyMember.CurrentMap?.MapId ?? 0);
                partyMemberBlock.PositionX = partyMember.Position.X;
                partyMemberBlock.PositionY = partyMember.Position.Y;
                partyMemberBlock.CurrentHealth = partyMember.CurrentHealth;
                partyMemberBlock.MaximumHealth = partyMember.MaximumHealth;
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}