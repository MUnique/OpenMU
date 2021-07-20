// <copyright file="NewPlayersInScopePlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System.Collections.Generic;
    using System.Linq;
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
    [PlugIn(nameof(NewPlayersInScopePlugIn075), "The default implementation of the INewPlayersInScopePlugIn which is forwarding everything to the game client with specific data packets for version 0.75.")]
    [Guid("1B68C660-34DD-4733-834D-DEE8DC1517D3")]
    [MinimumClient(0, 75, ClientLanguage.Invariant)]
    public class NewPlayersInScopePlugIn075 : INewPlayersInScopePlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewPlayersInScopePlugIn075"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public NewPlayersInScopePlugIn075(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void NewPlayersInScope(IEnumerable<Player> newPlayers, bool isSpawned = true)
        {
            if (!newPlayers.Any())
            {
                return;
            }

            var connection = this.player.Connection;
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
                    this.SendAddToScopeMessage(isSpawned, connection, newPlayer);
                }
                else
                {
                    this.SendAddTransformedToScopeMessage(isSpawned, connection, newPlayer);
                }

                if (newPlayer.GuildStatus != null)
                {
                    (guildPlayers ??= new List<Player>()).Add(newPlayer);
                }
            }

            if (guildPlayers != null)
            {
                this.player.ViewPlugIns.GetPlugIn<IAssignPlayersToGuildPlugIn>()?.AssignPlayersToGuild(guildPlayers, true);
            }
        }

        private void SendAddTransformedToScopeMessage(bool isSpawned, IConnection connection, Player transformedPlayer)
        {
            using var writer = connection.StartSafeWrite(AddTransformedCharactersToScope075.HeaderType, AddTransformedCharactersToScope075.GetRequiredSize(1));
            var packet = new AddTransformedCharactersToScope075(writer.Span)
            {
                CharacterCount = 1,
            };

            var playerId = transformedPlayer.GetId(this.player);
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

            writer.Commit();
        }

        private void SendAddToScopeMessage(bool isSpawned, IConnection connection, Player newPlayer)
        {
            var appearanceSerializer = this.player.AppearanceSerializer;
            using var writer = connection.StartSafeWrite(AddCharactersToScope075.HeaderType, AddCharactersToScope075.GetRequiredSize(1));
            var packet = new AddCharactersToScope075(writer.Span)
            {
                CharacterCount = 1,
            };

            var playerId = newPlayer.GetId(this.player);
            var playerBlock = packet[0];
            playerBlock.Id = playerId;
            if (isSpawned)
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

            writer.Commit();
        }
    }
}