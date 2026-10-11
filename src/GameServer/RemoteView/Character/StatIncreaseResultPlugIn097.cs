// <copyright file="StatIncreaseResultPlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The implementation of the <see cref="IStatIncreaseResultPlugIn"/> for the clients of version 0.97,
/// which don't know the shield, so the packet has no field for its maximum value.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.StatIncreaseResultPlugIn097_Name), Description = nameof(PlugInResources.StatIncreaseResultPlugIn097_Description), ResourceType = typeof(PlugInResources))]
[Guid("B8A058DB-5F00-4BB1-A35D-116D5C5F41A0")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 255, ClientLanguage.Invariant)]
public class StatIncreaseResultPlugIn097 : StatIncreaseResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatIncreaseResultPlugIn097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public StatIncreaseResultPlugIn097(RemotePlayer player)
        : base(player)
    {
        this._player = player;
    }

    /// <inheritdoc/>
    protected override async ValueTask SendSinglePointResultAsync(IConnection connection, AttributeDefinition attribute, bool success)
    {
        await connection.SendCharacterStatIncreaseResponse097Async(
            success,
            attribute.GetStatType(),
            this.GetDependentMaximumStat(attribute),
            this._player.Attributes![Stats.MaximumAbility].ToUInt16Clamped()).ConfigureAwait(false);
    }
}
