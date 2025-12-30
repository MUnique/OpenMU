// <copyright file="FruitConsumptionResultPlugIn097.cs" company="MUnique">
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
/// Fruit consumption response plugin for 0.97 clients.
/// </summary>
[PlugIn(nameof(FruitConsumptionResultPlugIn097), "Fruit consumption response plugin for 0.97 clients.")]
[Guid("6BE7F3D4-4A56-4AAB-9E5F-0B1D59A6A169")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 97, ClientLanguage.Invariant)]
public class FruitConsumptionResultPlugIn097 : IFruitConsumptionResponsePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="FruitConsumptionResultPlugIn097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public FruitConsumptionResultPlugIn097(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowResponseAsync(FruitConsumptionResult result, byte statPoints, AttributeDefinition statAttribute)
    {
        await Version097CompatibilityProfile.SendFruitConsumptionResultAsync(this._player, result, statPoints, statAttribute).ConfigureAwait(false);
    }
}
