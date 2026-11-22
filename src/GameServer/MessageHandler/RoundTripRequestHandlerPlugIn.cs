// <copyright file="RoundTripRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for the round trip request, with which the client measures its ping. It is answered right away.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.RoundTripRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.RoundTripRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("C8A7F7C6-E761-4E1C-9EDB-796A8EDA62DB")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
[BelongsToGroup(ChatCommandGroupHandlerPlugIn.GroupKey)]
internal class RoundTripRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => RoundTripRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < RoundTripRequest.Length)
        {
            return;
        }

        RoundTripRequest request = packet;
        await player.InvokeViewPlugInAsync<IRoundTripViewPlugIn>(p => p.AnswerRoundTripAsync(request.Token)).ConfigureAwait(false);
    }
}
