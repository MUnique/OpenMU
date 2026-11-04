// <copyright file="CastleSiegeOwnerLogoPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="ICastleSiegeOwnerLogoPlugIn"/>
/// which forwards the guild logo of the castle owner to the game client.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeOwnerLogoPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeOwnerLogoPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("27ECC1A2-56DC-4509-8AB4-A92CBE8F9999")]
public class CastleSiegeOwnerLogoPlugIn : ICastleSiegeOwnerLogoPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeOwnerLogoPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public CastleSiegeOwnerLogoPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowOwnerLogoAsync(ReadOnlyMemory<byte> logo)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        int Write()
        {
            var size = CastleSiegeOwnerLogoRef.Length;
            var packet = new CastleSiegeOwnerLogoRef(connection.Output.GetSpan(size)[..size]);

            // An empty logo is sent as all zeros, which the client renders as a transparent mark.
            var target = packet.Logo;
            target.Clear();
            var source = logo.Span;
            source[..Math.Min(source.Length, target.Length)].CopyTo(target);
            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}
