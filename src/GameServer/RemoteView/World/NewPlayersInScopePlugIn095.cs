// <copyright file="NewPlayersInScopePlugIn095.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.GameServer.RemoteView.Character;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The implementation of the <see cref="INewPlayersInScopePlugIn"/> which is forwarding everything to the game client of version 0.75 with specific data packets.
/// </summary>
[PlugIn(nameof(NewPlayersInScopePlugIn095), "The default implementation of the INewPlayersInScopePlugIn which is forwarding everything to the game client with specific data packets for version 0.75.")]
[Guid("400ACDFB-7A75-4339-802C-758178DF8305")]
[MinimumClient(0, 95, ClientLanguage.Invariant)]
public class NewPlayersInScopePlugIn095 : INewPlayersInScopePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="NewPlayersInScopePlugIn095"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public NewPlayersInScopePlugIn095(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask NewPlayersInScopeAsync(IEnumerable<Player> newPlayers, bool isSpawned = true)
    {
        if (!newPlayers.Any())
        {
            return;
        }

        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        List<Player>? guildPlayers = null;
        var playerList = newPlayers.ToList();
        foreach (var newPlayer in playerList)
        {
            if (newPlayer.Attributes?[Stats.TransformationSkin] == 0)
            {
                await this.SendAddToScopeMessageAsync(isSpawned, connection, newPlayer).ConfigureAwait(false);
            }
            else
            {
                await this.SendAddTransformedToScopeMessageAsync(isSpawned, connection, newPlayer).ConfigureAwait(false);
            }

            if (newPlayer.GuildStatus != null)
            {
                (guildPlayers ??= new List<Player>()).Add(newPlayer);
            }
        }

        if (guildPlayers != null)
        {
            await this._player.InvokeViewPlugInAsync<IAssignPlayersToGuildPlugIn>(p => p.AssignPlayersToGuildAsync(guildPlayers, true)).ConfigureAwait(false);
        }
    }

    private async ValueTask SendAddTransformedToScopeMessageAsync(bool isSpawned, IConnection connection, Player transformedPlayer)
    {
        int Write()
        {
            var size = AddTransformedCharactersToScope075Ref.GetRequiredSize(1);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new AddTransformedCharactersToScope075Ref(span)
            {
                CharacterCount = 1,
            };

            var playerId = transformedPlayer.GetId(this._player);
            var playerBlock = packet[0];
            playerBlock.Id = playerId;
            if (isSpawned)
            {
                playerBlock.Id |= 0x8000;
            }

            playerBlock.Skin = (byte)(transformedPlayer.Attributes?[Stats.TransformationSkin] ?? 0);

            playerBlock.CurrentPositionX = transformedPlayer.Position.X;
            playerBlock.CurrentPositionY = transformedPlayer.Position.Y;

            playerBlock.IsPoisoned = transformedPlayer.MagicEffectList.ActiveEffects.ContainsKey(EffectNumbers.Poisoned);
            playerBlock.IsIced = transformedPlayer.MagicEffectList.ActiveEffects.ContainsKey(EffectNumbers.Iced);
            playerBlock.IsDamageBuffed = transformedPlayer.MagicEffectList.ActiveEffects.ContainsKey(EffectNumbers.DamageBuff);
            playerBlock.IsDefenseBuffed = transformedPlayer.MagicEffectList.ActiveEffects.ContainsKey(EffectNumbers.DefenseBuff);
            playerBlock.Name = transformedPlayer.SelectedCharacter!.Name;

            if (transformedPlayer.IsWalking)
            {
                playerBlock.TargetPositionX = transformedPlayer.WalkTarget.X;
                playerBlock.TargetPositionY = transformedPlayer.WalkTarget.Y;
            }
            else
            {
                playerBlock.TargetPositionX = transformedPlayer.Position.X;
                playerBlock.TargetPositionY = transformedPlayer.Position.Y;
            }

            playerBlock.Rotation = transformedPlayer.Rotation.ToPacketByte();
            playerBlock.HeroState = transformedPlayer.SelectedCharacter.State.Convert();
            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }

    private async ValueTask SendAddToScopeMessageAsync(bool isSpawned, IConnection connection, Player newPlayer)
    {
        int Write()
        {
            var appearanceSerializer = this._player.AppearanceSerializer;
            var size = AddCharactersToScope095Ref.GetRequiredSize(1);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new AddCharactersToScope095Ref(span)
            {
                CharacterCount = 1,
            };

            var playerId = newPlayer.GetId(this._player);
            var playerBlock = packet[0];
            playerBlock.Id = playerId;
            if (isSpawned || newPlayer == this._player)
            {
                playerBlock.Id |= 0x8000;
            }

            playerBlock.CurrentPositionX = newPlayer.Position.X;
            playerBlock.CurrentPositionY = newPlayer.Position.Y;
            appearanceSerializer.WriteAppearanceData(playerBlock.Appearance, newPlayer.AppearanceData, true); // 4 ... 12

            playerBlock.IsPoisoned = newPlayer.MagicEffectList.ActiveEffects.ContainsKey(EffectNumbers.Poisoned);
            playerBlock.IsIced = newPlayer.MagicEffectList.ActiveEffects.ContainsKey(EffectNumbers.Iced);
            playerBlock.IsDamageBuffed = newPlayer.MagicEffectList.ActiveEffects.ContainsKey(EffectNumbers.DamageBuff);
            playerBlock.IsDefenseBuffed = newPlayer.MagicEffectList.ActiveEffects.ContainsKey(EffectNumbers.DefenseBuff);
            playerBlock.Name = newPlayer.SelectedCharacter!.Name;

            if (newPlayer.IsWalking)
            {
                playerBlock.TargetPositionX = newPlayer.WalkTarget.X;
                playerBlock.TargetPositionY = newPlayer.WalkTarget.Y;
            }
            else
            {
                playerBlock.TargetPositionX = newPlayer.Position.X;
                playerBlock.TargetPositionY = newPlayer.Position.Y;
            }

            playerBlock.Rotation = newPlayer.Rotation.ToPacketByte();
            playerBlock.HeroState = newPlayer.SelectedCharacter.State.Convert();
            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}