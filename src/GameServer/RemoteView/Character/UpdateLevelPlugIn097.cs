// <copyright file="UpdateLevelPlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameServer.Compatibility;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Level update plugin for 0.97 clients.
/// </summary>
[PlugIn(nameof(UpdateLevelPlugIn097), "Level update plugin for 0.97 clients.")]
[Guid("D04B2700-3F1D-4A9E-A7F0-84D7F9D76A0F")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 97, ClientLanguage.Invariant)]
public class UpdateLevelPlugIn097 : IUpdateLevelPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateLevelPlugIn097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateLevelPlugIn097(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask UpdateLevelAsync()
    {
        await Version097CompatibilityProfile.SendLevelUpdateAsync(this._player).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public ValueTask UpdateMasterLevelAsync()
    {
        return ValueTask.CompletedTask;
    }

    
}
