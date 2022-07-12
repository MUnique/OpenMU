// <copyright file="PartyHealthViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Party;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Party;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the party view which is forwarding everything to the game client which specific data packets.
/// </summary>
[PlugIn("Party View", "The default implementation of the party view which is forwarding everything to the game client which specific data packets.")]
[Guid("CEE58BCB-FB8C-4AEB-9FC8-5D3A11FA7C03")]
[MinimumClient(0, 90, ClientLanguage.Invariant)]
public class PartyHealthViewPlugIn : IPartyHealthViewPlugIn
{
    private readonly RemotePlayer _player;
    private byte[]? _healthValues;

    /// <summary>
    /// Initializes a new instance of the <see cref="PartyHealthViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public PartyHealthViewPlugIn(RemotePlayer player)
    {
        this._player = player;
    }

    private byte[] HealthValues => this._healthValues ??= new byte[this._player.Party!.MaxPartySize];

    /// <inheritdoc/>
    public bool IsHealthUpdateNeeded()
    {
        return this.UpdateHealthValues();
    }

    /// <inheritdoc/>
    public async ValueTask UpdatePartyHealthAsync()
    {
        var partyList = this._player.Party?.PartyList;
        if (partyList is null)
        {
            return;
        }

        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        var partyListCount = partyList.Count;
        int Write()
        {
            var size = PartyHealthUpdateRef.GetRequiredSize(partyListCount);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new PartyHealthUpdateRef(span)
            {
                Count = (byte)partyListCount,
            };

            for (int i = 0; i < partyListCount; i++)
            {
                var member = packet[i];
                member.Index = (byte)i;
                member.Value = this.HealthValues[i];
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }

    private bool UpdateHealthValues()
    {
        var partyList = this._player.Party?.PartyList;
        if (partyList is null)
        {
            // Party was left in the meantime
            this._healthValues?.ClearToDefaults();
            return false;
        }

        bool updated = false;
        for (int i = partyList.Count - 1; i >= 0; --i)
        {
            var partyMember = partyList[i];
            double value = (double)partyMember.CurrentHealth / partyMember.MaximumHealth;
            var newValue = (byte)(value * 10);
            if (this.HealthValues[i] != newValue)
            {
                updated = true;
                this.HealthValues[i] = newValue;
            }
        }

        return updated;
    }
}