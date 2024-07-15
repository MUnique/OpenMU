// <copyright file="ShowChainLightningPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowChainLightningPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowChainLightningPlugIn), "The default implementation of the IShowChainLightningPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("8A78EE23-7AD5-4D08-BE17-B0F6B0CB7309")]
[MinimumClient(4, 0, ClientLanguage.Invariant)]
public class ShowChainLightningPlugIn : IShowChainLightningPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowChainLightningPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowChainLightningPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowLightningChainAnimationAsync(IAttacker attacker, Skill skill, IReadOnlyCollection<IAttackable> targets)
    {
        if (this._player?.Connection is not { Connected: true } connection)
        {
            return;
        }

        int WritePacket()
        {
            var length = ChainLightningHitInfoRef.GetRequiredSize(targets.Count);
            var packet = new ChainLightningHitInfoRef(connection.Output.GetSpan(length)[..length]);
            packet.SkillNumber = (ushort)skill.Number;
            packet.TargetCount = (byte)targets.Count;
            packet.PlayerId = attacker.GetId(this._player);

            var i = 0;
            foreach (var target in targets)
            {
                var objectId = packet[i];
                objectId.TargetId = target.GetId(this._player);
                i++;
            }

            return length;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);
    }
}