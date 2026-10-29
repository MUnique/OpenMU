// <copyright file="UpdateCharacterStatsExtendedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The extended implementation of the <see cref="IUpdateCharacterStatsPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.UpdateCharacterStatsExtendedPlugIn_Name), Description = nameof(PlugInResources.UpdateCharacterStatsExtendedPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("851C4579-FB3D-454C-A238-217542E8E6B8")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class UpdateCharacterStatsExtendedPlugIn : IUpdateCharacterStatsPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCharacterStatsExtendedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateCharacterStatsExtendedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask UpdateCharacterStatsAsync()
    {
        var connection = this._player.Connection;
        if (connection is null || this._player.Account is null)
        {
            return;
        }

        var maxAttackSpeed = this._player.GameContext.Configuration.Attributes.FirstOrDefault(a => a == Stats.AttackSpeed)?.MaximumValue ?? 200;
        var character = this._player.SelectedCharacter!;
        var attributes = this._player.Attributes!;
        var map = character.CurrentMap!;
        var experienceForNextLevel = this._player.GameServerContext.ExperienceTable[(int)attributes[Stats.Level] + 1];

        // Written by hand instead of with the generated sender, to clear the packet first: its unused
        // byte 89 would keep what an earlier packet left in the reused output buffer.
        int WritePacket()
        {
            var length = CharacterInformationExtendedRef.Length;
            var span = connection.Output.GetSpan(length)[..length];
            span.Clear();
            _ = new CharacterInformationExtendedRef(span)
            {
                X = this._player.Position.X,
                Y = this._player.Position.Y,
                MapId = ShortExtensions.ToUnsigned(map.Number),
                CurrentExperience = (ulong)character.Experience,
                ExperienceForNextLevel = (ulong)experienceForNextLevel,
                LevelUpPoints = (ushort)Math.Max(0, character.LevelUpPoints),
                Strength = (ushort)attributes[Stats.BaseStrength],
                Agility = (ushort)attributes[Stats.BaseAgility],
                Vitality = (ushort)attributes[Stats.BaseVitality],
                Energy = (ushort)attributes[Stats.BaseEnergy],
                Leadership = (ushort)attributes[Stats.BaseLeadership],
                CurrentHealth = (uint)attributes[Stats.CurrentHealth],
                MaximumHealth = (uint)attributes[Stats.MaximumHealth],
                CurrentMana = (uint)attributes[Stats.CurrentMana],
                MaximumMana = (uint)attributes[Stats.MaximumMana],
                CurrentShield = (uint)attributes[Stats.CurrentShield],
                MaximumShield = (uint)attributes[Stats.MaximumShield],
                CurrentAbility = (uint)attributes[Stats.CurrentAbility],
                MaximumAbility = (uint)attributes[Stats.MaximumAbility],
                Money = (uint)this._player.Money,
                HeroState = character.State.Convert(),
                Status = character.CharacterStatus.Convert(),
                UsedFruitPoints = (ushort)character.UsedFruitPoints,
                MaxFruitPoints = character.GetMaximumFruitPoints(),
                UsedNegativeFruitPoints = (ushort)character.UsedNegFruitPoints,
                MaxNegativeFruitPoints = character.GetMaximumFruitPoints(),
                AttackSpeed = (ushort)attributes[Stats.AttackSpeed],
                MagicSpeed = (ushort)attributes[Stats.MagicSpeed],
                MaximumAttackSpeed = (ushort)maxAttackSpeed,
                InventoryExtensions = (byte)character.InventoryExtensions,
                Resets = (ushort)attributes[Stats.Resets],
                MapDiscriminator = map.GetDiscriminatorForClient(),
            };

            return length;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);

        if (character.CharacterClass!.IsMasterClass)
        {
            await this._player.InvokeViewPlugInAsync<IUpdateMasterStatsPlugIn>(p => p.SendMasterStatsAsync()).ConfigureAwait(false);
        }
    }
}