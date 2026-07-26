// <copyright file="GoldenArcherRegistrationResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.NPC;

using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IGoldenArcherRegistrationResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Guid("4A1B2C3D-4E5F-6A7B-8C9D-0E1F2A3B4C5D")]
public class GoldenArcherRegistrationResultPlugIn : IGoldenArcherRegistrationResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="GoldenArcherRegistrationResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public GoldenArcherRegistrationResultPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask RegistrationResultAsync()
    {
        var connection = this._player.Connection;
        if (connection != null)
        {
            var openedNpcNumber = this._player.OpenedNpc?.Definition.Number ?? 236;
            var feature = this._player.GameContext.FeaturePlugIns.GetPlugIn<MUnique.OpenMU.GameLogic.PlugIns.ItemRegistration.ItemRegistrationFeaturePlugIn>();
            var config = feature?.Configuration ?? new MUnique.OpenMU.GameLogic.PlugIns.ItemRegistration.ItemRegistrationConfiguration();
            var rule = config.Rules.FirstOrDefault(r => r.NpcNumber == openedNpcNumber);

            // Calculate the number of matching items in the inventory
            int invRenas = this._player.Inventory?.Items.Count(i =>
                rule != null && i.Definition?.Group == rule.AcceptedItemGroup && i.Definition?.Number == rule.AcceptedItemNumber
            ) ?? 0;

            var registeredStat = this._player.SelectedCharacter?.Attributes.FirstOrDefault(a => a.Definition == Stats.RegisteredRenas);
            int registered = (int)(registeredStat?.Value ?? this._player.Attributes?[Stats.RegisteredRenas] ?? 0);

            await connection.SendGoldenArcherRegistrationResultAsync(0, (uint)registered, (ushort)invRenas).ConfigureAwait(false);
        }
    }
}
