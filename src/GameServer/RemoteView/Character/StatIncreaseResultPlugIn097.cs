// <copyright file="StatIncreaseResultPlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameServer.Compatibility;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Stat increase result plugin for 0.97 clients.
/// </summary>
[PlugIn(nameof(StatIncreaseResultPlugIn097), "Stat increase result plugin for 0.97 clients.")]
[Guid("8B019191-4C4C-4FBD-8A65-1AE0E0490CE3")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 97, ClientLanguage.Invariant)]
public class StatIncreaseResultPlugIn097 : IStatIncreaseResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatIncreaseResultPlugIn097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public StatIncreaseResultPlugIn097(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask StatIncreaseResultAsync(AttributeDefinition attribute, ushort addedPoints)
    {
        await Version097CompatibilityProfile.SendStatIncreaseResultAsync(this._player, attribute, addedPoints).ConfigureAwait(false);
    }
}
