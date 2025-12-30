// <copyright file="UpdateCharacterStatsPlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using MUnique.OpenMU.GameServer.Compatibility;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IUpdateCharacterStatsPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(UpdateCharacterStatsPlugIn097), "The default implementation of the IUpdateCharacterStatsPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("8ACD9D6B-6FA7-42C3-8C07-E137655CB92F")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
public class UpdateCharacterStatsPlugIn097 : IUpdateCharacterStatsPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCharacterStatsPlugIn097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateCharacterStatsPlugIn097(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask UpdateCharacterStatsAsync()
    {
        await Version097CompatibilityProfile.SendCharacterStatsAsync(this._player).ConfigureAwait(false);
    }
}
