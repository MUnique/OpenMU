// <copyright file="NewPlayersInScopePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.GameLogic.Views.PlayerShop;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.GameServer.RemoteView.Character;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="INewPlayersInScopePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.NewPlayersInScopePlugIn_Name), Description = nameof(PlugInResources.NewPlayersInScopePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("4cd64537-ae5f-4030-bca1-7fa30ebff6c6")]
[MinimumClient(5, 0, ClientLanguage.Invariant)]
public class NewPlayersInScopePlugIn : INewPlayersInScopePlugIn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NewPlayersInScopePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public NewPlayersInScopePlugIn(RemotePlayer player) => this.Player = player;

    /// <summary>
    /// Gets the player of this view.
    /// </summary>
    protected RemotePlayer Player { get; }

    /// <inheritdoc/>
    public async ValueTask NewPlayersInScopeAsync(IEnumerable<Player> newPlayers, bool isSpawned = true)
    {
        if (newPlayers is null || !newPlayers.Any())
        {
            return;
        }

        var (shopPlayers, guildPlayers) = await this.SendCharactersAsync(newPlayers, isSpawned).ConfigureAwait(false);

        if (shopPlayers != null)
        {
            await this.Player.InvokeViewPlugInAsync<IShowShopsOfPlayersPlugIn>(p => p.ShowShopsOfPlayersAsync(shopPlayers)).ConfigureAwait(false);
        }

        if (guildPlayers != null)
        {
            await this.Player.InvokeViewPlugInAsync<IAssignPlayersToGuildPlugIn>(p => p.AssignPlayersToGuildAsync(guildPlayers, true)).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Sends information about a new player which has come into view.
    /// </summary>
    /// <param name="newPlayer">The new player.</param>
    /// <param name="isSpawned">If the player has spawned.</param>
    /// <returns>A <see cref="ValueTask"/>.</returns>
    protected virtual async ValueTask SendCharacterAsync(Player newPlayer, bool isSpawned)
    {
        var connection = this.Player.Connection;
        if (connection is null)
        {
            return;
        }

        var selectedCharacter = newPlayer.SelectedCharacter;
        if (selectedCharacter is null)
        {
            return;
        }

        var target = newPlayer.IsWalking ? newPlayer.WalkTarget : newPlayer.Position;
        if (newPlayer.Position.X > byte.MaxValue || newPlayer.Position.Y > byte.MaxValue || target.X > byte.MaxValue || target.Y > byte.MaxValue)
        {
            var appearanceSerializer = this.Player.AppearanceSerializer;
            var activeEffects = newPlayer.MagicEffectList.VisibleEffects;
            var appearanceAndEffects = new byte[appearanceSerializer.NeededSpace + activeEffects.Count + 1];
            appearanceSerializer.WriteAppearanceData(appearanceAndEffects.AsSpan(), newPlayer.AppearanceData, true);
            appearanceAndEffects[appearanceSerializer.NeededSpace] = (byte)activeEffects.Count;
            for (var effect = 0; effect < activeEffects.Count; effect++)
            {
                appearanceAndEffects[appearanceSerializer.NeededSpace + 1 + effect] = (byte)activeEffects[effect].Id;
            }

            var id = newPlayer.GetId(this.Player);
            if (isSpawned)
            {
                id |= 0x8000;
            }

            await connection.SendAddCharacterToScopeGlobalAsync(
                id,
                newPlayer.Position.X,
                newPlayer.Position.Y,
                target.X,
                target.Y,
                newPlayer.Rotation.ToPacketByte(),
                selectedCharacter.State.Convert(),
                (ushort)(newPlayer.Attributes?[Stats.AttackSpeed] ?? 0),
                (ushort)(newPlayer.Attributes?[Stats.MagicSpeed] ?? 0),
                selectedCharacter.Name,
                appearanceAndEffects).ConfigureAwait(false);
            return;
        }

        int Write()
        {
            var appearanceSerializer = this.Player.AppearanceSerializer;
            var activeEffects = newPlayer.MagicEffectList.VisibleEffects;
            const int estimatedEffectsPerPlayer = 5;
            var estimatedSizePerCharacter = AddCharactersToScope.CharacterData.GetRequiredSize(Math.Max(estimatedEffectsPerPlayer, activeEffects.Count));
            var estimatedSize = AddCharactersToScope.GetRequiredSize(1, estimatedSizePerCharacter);

            var span = connection.Output.GetSpan(estimatedSize)[..estimatedSize];
            var packet = new AddCharactersToScopeRef(span)
            {
                CharacterCount = 1,
            };

            var playerBlock = packet[0];
            playerBlock.Id = newPlayer.GetId(this.Player);
            if (isSpawned)
            {
                playerBlock.Id |= 0x8000;
            }

            playerBlock.CurrentPositionX = checked((byte)(newPlayer.Position.X));
            playerBlock.CurrentPositionY = checked((byte)(newPlayer.Position.Y));

            appearanceSerializer.WriteAppearanceData(playerBlock.Appearance, newPlayer.AppearanceData, true); // 4 ... 21
            playerBlock.Name = selectedCharacter.Name;
            if (newPlayer.IsWalking)
            {
                playerBlock.TargetPositionX = checked((byte)(newPlayer.WalkTarget.X));
                playerBlock.TargetPositionY = checked((byte)(newPlayer.WalkTarget.Y));
            }
            else
            {
                playerBlock.TargetPositionX = checked((byte)(newPlayer.Position.X));
                playerBlock.TargetPositionY = checked((byte)(newPlayer.Position.Y));
            }

            playerBlock.Rotation = newPlayer.Rotation.ToPacketByte();
            playerBlock.HeroState = selectedCharacter.State.Convert();

            playerBlock.EffectCount = (byte)activeEffects.Count;
            for (int e = playerBlock.EffectCount - 1; e >= 0; e--)
            {
                var effectBlock = playerBlock[e];
                effectBlock.Id = (byte)activeEffects[e].Id;
            }

            // The calculation of the final size is not a requirement, but we do it to save some traffic.
            // The original server also doesn't send more bytes than necessary.
            var finalSize = packet.FinalSize;
            span.Slice(0, finalSize).SetPacketSize();
            return finalSize;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }

    private async ValueTask<(IList<Player>? ShopPlayers, IList<Player>? GuildPlayers)> SendCharactersAsync(IEnumerable<Player> newPlayers, bool isSpawned)
    {
        IList<Player>? shopPlayers = null;
        IList<Player>? guildPlayers = null;

        var connection = this.Player.Connection;
        if (connection is null)
        {
            return (shopPlayers, guildPlayers);
        }

        var newPlayerList = newPlayers.ToList();
        foreach (var newPlayer in newPlayerList)
        {
            if (newPlayer.Attributes?[Stats.TransformationSkin] == 0)
            {
                await this.SendCharacterAsync(newPlayer, isSpawned).ConfigureAwait(false);
            }
            else
            {
                await this.SendTransformedCharacterAsync(newPlayer, isSpawned).ConfigureAwait(false);
            }

            if (newPlayer.ShopStorage?.StoreOpen ?? false)
            {
                (shopPlayers ??= new List<Player>()).Add(newPlayer);
            }

            if (newPlayer.GuildStatus != null)
            {
                (guildPlayers ??= new List<Player>()).Add(newPlayer);
            }
        }

        return (shopPlayers, guildPlayers);
    }

    private async ValueTask SendTransformedCharacterAsync(Player newPlayer, bool isSpawned)
    {
        var connection = this.Player.Connection;
        if (connection is null)
        {
            return;
        }

        var selectedCharacter = newPlayer.SelectedCharacter;
        if (selectedCharacter is null)
        {
            return;
        }

        int Write()
        {
            var appearanceSerializer = this.Player.AppearanceSerializer;
            var activeEffects = newPlayer.MagicEffectList.VisibleEffects;
            const int estimatedEffectsPerPlayer = 5;
            var estimatedSizePerCharacter = AddTransformedCharactersToScopeRef.CharacterDataRef.GetRequiredSize(Math.Max(estimatedEffectsPerPlayer, activeEffects.Count));
            var estimatedSize = AddTransformedCharactersToScopeRef.GetRequiredSize(1, estimatedSizePerCharacter);
            var span = connection.Output.GetSpan(estimatedSize)[..estimatedSize];
            var packet = new AddTransformedCharactersToScopeRef(span)
            {
                CharacterCount = 1,
            };

            var playerBlock = packet[0];
            playerBlock.Id = newPlayer.GetId(this.Player);
            if (isSpawned)
            {
                playerBlock.Id |= 0x8000;
            }

            playerBlock.CurrentPositionX = checked((byte)(newPlayer.Position.X));
            playerBlock.CurrentPositionY = checked((byte)(newPlayer.Position.Y));

            appearanceSerializer.WriteAppearanceData(playerBlock.Appearance, newPlayer.AppearanceData, true); // 4 ... 21
            playerBlock.Name = selectedCharacter.Name;
            if (newPlayer.IsWalking)
            {
                playerBlock.TargetPositionX = checked((byte)(newPlayer.WalkTarget.X));
                playerBlock.TargetPositionY = checked((byte)(newPlayer.WalkTarget.Y));
            }
            else
            {
                playerBlock.TargetPositionX = checked((byte)(newPlayer.Position.X));
                playerBlock.TargetPositionY = checked((byte)(newPlayer.Position.Y));
            }

            playerBlock.Rotation = newPlayer.Rotation.ToPacketByte();
            playerBlock.HeroState = selectedCharacter.State.Convert();

            playerBlock.EffectCount = (byte)activeEffects.Count;
            playerBlock.Skin = (ushort)newPlayer.Attributes![Stats.TransformationSkin];
            for (int e = playerBlock.EffectCount - 1; e >= 0; e--)
            {
                var effectBlock = playerBlock[e];
                effectBlock.Id = (byte)activeEffects[e].Id;
            }

            // The calculation of the final size is not a requirement, but we do it to save some traffic.
            // The original server also doesn't send more bytes than necessary.
            var finalSize = packet.FinalSize;
            span.Slice(0, finalSize).SetPacketSize();
            return finalSize;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}
