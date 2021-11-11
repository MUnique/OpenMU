﻿// <copyright file="NewPlayersInScopePlugIn.cs" company="MUnique">
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
[PlugIn("NewPlayersInScopePlugIn", "The default implementation of the INewPlayersInScopePlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("4cd64537-ae5f-4030-bca1-7fa30ebff6c6")]
[MinimumClient(6, 3, ClientLanguage.Invariant)]
public class NewPlayersInScopePlugIn : INewPlayersInScopePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="NewPlayersInScopePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public NewPlayersInScopePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public void NewPlayersInScope(IEnumerable<Player> newPlayers, bool isSpawned = true)
    {
        if (newPlayers is null || !newPlayers.Any())
        {
            return;
        }

        this.SendCharacters(newPlayers, out var shopPlayers, out var guildPlayers, isSpawned);

        if (shopPlayers != null)
        {
            this._player.ViewPlugIns.GetPlugIn<IShowShopsOfPlayersPlugIn>()?.ShowShopsOfPlayers(shopPlayers);
        }

        if (guildPlayers != null)
        {
            this._player.ViewPlugIns.GetPlugIn<IAssignPlayersToGuildPlugIn>()?.AssignPlayersToGuild(guildPlayers, true);
        }
    }

    private void SendCharacters(IEnumerable<Player> newPlayers, out IList<Player>? shopPlayers, out IList<Player>? guildPlayers, bool isSpawned)
    {
        shopPlayers = null;
        guildPlayers = null;

        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        var newPlayerList = newPlayers.ToList();
        foreach (var newPlayer in newPlayerList)
        {
            if (newPlayer.Attributes?[Stats.TransformationSkin] == 0)
            {
                this.SendCharacter(newPlayer, isSpawned);
            }
            else
            {
                this.SendTransformedCharacter(newPlayer, isSpawned);
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
    }

    private void SendCharacter(Player newPlayer, bool isSpawned)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        var selectedCharacter = newPlayer.SelectedCharacter;
        if (selectedCharacter is null)
        {
            return;
        }

        var appearanceSerializer = this._player.AppearanceSerializer;
        var activeEffects = newPlayer.MagicEffectList.VisibleEffects;
        const int estimatedEffectsPerPlayer = 5;
        var estimatedSizePerCharacter = AddCharactersToScope.CharacterData.GetRequiredSize(Math.Max(estimatedEffectsPerPlayer, activeEffects.Count));
        var estimatedSize = AddCharactersToScope.GetRequiredSize(1, estimatedSizePerCharacter);
        using var writer = connection.StartSafeWrite(AddCharactersToScope.HeaderType, estimatedSize);
        var packet = new AddCharactersToScope(writer.Span)
        {
            CharacterCount = 1,
        };

        var playerBlock = packet[0];
        playerBlock.Id = newPlayer.GetId(this._player);
        if (isSpawned)
        {
            playerBlock.Id |= 0x8000;
        }

        playerBlock.CurrentPositionX = newPlayer.Position.X;
        playerBlock.CurrentPositionY = newPlayer.Position.Y;

        appearanceSerializer.WriteAppearanceData(playerBlock.Appearance, newPlayer.AppearanceData, true); // 4 ... 21
        playerBlock.Name = selectedCharacter.Name;
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
        writer.Span.Slice(0, finalSize).SetPacketSize();
        writer.Commit(finalSize);
    }

    private void SendTransformedCharacter(Player newPlayer, bool isSpawned)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        var selectedCharacter = newPlayer.SelectedCharacter;
        if (selectedCharacter is null)
        {
            return;
        }

        var appearanceSerializer = this._player.AppearanceSerializer;
        var activeEffects = newPlayer.MagicEffectList.VisibleEffects;
        const int estimatedEffectsPerPlayer = 5;
        var estimatedSizePerCharacter = AddTransformedCharactersToScope.CharacterData.GetRequiredSize(Math.Max(estimatedEffectsPerPlayer, activeEffects.Count));
        var estimatedSize = AddTransformedCharactersToScope.GetRequiredSize(1, estimatedSizePerCharacter);
        using var writer = connection.StartSafeWrite(AddTransformedCharactersToScope.HeaderType, estimatedSize);
        var packet = new AddTransformedCharactersToScope(writer.Span)
        {
            CharacterCount = 1,
        };

        var playerBlock = packet[0];
        playerBlock.Id = newPlayer.GetId(this._player);
        if (isSpawned)
        {
            playerBlock.Id |= 0x8000;
        }

        playerBlock.CurrentPositionX = newPlayer.Position.X;
        playerBlock.CurrentPositionY = newPlayer.Position.Y;

        appearanceSerializer.WriteAppearanceData(playerBlock.Appearance, newPlayer.AppearanceData, true); // 4 ... 21
        playerBlock.Name = selectedCharacter.Name;
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
        playerBlock.HeroState = selectedCharacter.State.Convert();

        playerBlock.EffectCount = (byte)activeEffects.Count;
        playerBlock.Skin = (ushort)this._player.Attributes![Stats.TransformationSkin];
        for (int e = playerBlock.EffectCount - 1; e >= 0; e--)
        {
            var effectBlock = playerBlock[e];
            effectBlock.Id = (byte)activeEffects[e].Id;
        }

        // The calculation of the final size is not a requirement, but we do it to save some traffic.
        // The original server also doesn't send more bytes than necessary.
        var finalSize = packet.FinalSize;
        writer.Span.Slice(0, finalSize).SetPacketSize();
        writer.Commit(finalSize);
    }
}