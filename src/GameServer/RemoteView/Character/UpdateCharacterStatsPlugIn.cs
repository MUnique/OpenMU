// <copyright file="UpdateCharacterStatsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IUpdateCharacterStatsPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("UpdateCharacterStatsPlugIn", "The default implementation of the IUpdateCharacterStatsPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("6eb967c2-b5a2-4510-9d88-5eccc963a6ea")]
[MinimumClient(5, 0, ClientLanguage.Invariant)]
public class UpdateCharacterStatsPlugIn : IUpdateCharacterStatsPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCharacterStatsPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateCharacterStatsPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask UpdateCharacterStatsAsync()
    {
        var connection = this._player.Connection;
        if (connection is null || this._player.Account is null)
        {
            return;
        }

        await connection.SendCharacterInformationAsync(
            this._player.Position.X,
            this._player.Position.Y,
            this._player.SelectedCharacter!.CurrentMap!.Number.ToUnsigned(),
            (ulong)this._player.SelectedCharacter.Experience,
            (ulong)this._player.GameServerContext.ExperienceTable[(int)this._player.Attributes![Stats.Level] + 1],
            (ushort)Math.Max(0, this._player.SelectedCharacter.LevelUpPoints),
            (ushort)this._player.Attributes[Stats.BaseStrength],
            (ushort)this._player.Attributes[Stats.BaseAgility],
            (ushort)this._player.Attributes[Stats.BaseVitality],
            (ushort)this._player.Attributes[Stats.BaseEnergy],
            (ushort)this._player.Attributes[Stats.CurrentHealth],
            (ushort)this._player.Attributes[Stats.MaximumHealth],
            (ushort)this._player.Attributes[Stats.CurrentMana],
            (ushort)this._player.Attributes[Stats.MaximumMana],
            (ushort)this._player.Attributes[Stats.CurrentShield],
            (ushort)this._player.Attributes[Stats.MaximumShield],
            (ushort)this._player.Attributes[Stats.CurrentAbility],
            (ushort)this._player.Attributes[Stats.MaximumAbility],
            (uint)this._player.Money,
            this._player.SelectedCharacter.State.Convert(),
            this._player.SelectedCharacter.CharacterStatus.Convert(),
            (ushort)this._player.SelectedCharacter.UsedFruitPoints,
            this._player.SelectedCharacter.GetMaximumFruitPoints(),
            (ushort)this._player.Attributes[Stats.BaseLeadership],
            (ushort)this._player.SelectedCharacter.UsedNegFruitPoints,
            this._player.SelectedCharacter.GetMaximumFruitPoints(),
            (byte)this._player.SelectedCharacter.InventoryExtensions)
            .ConfigureAwait(false);

        if (this._player.SelectedCharacter.CharacterClass!.IsMasterClass)
        {
            await this._player.InvokeViewPlugInAsync<IUpdateMasterStatsPlugIn>(p => p.SendMasterStatsAsync()).ConfigureAwait(false);
        }
    }
}