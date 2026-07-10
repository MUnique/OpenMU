// <copyright file="PKClearChatCommandPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;

/// <summary>
/// Tests for the <see cref="PkClearChatCommandPlugIn"/>.
/// </summary>
[TestFixture]
public class PKClearChatCommandPlugInTest
{
    /// <summary>
    /// Verifies that a regular player is not allowed to run the command if AllowRegularPlayers configuration is false.
    /// </summary>
    [Test]
    public async ValueTask RegularPlayerNotAllowedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.SelectedCharacter!.CharacterStatus = CharacterStatus.Normal;
        player.SelectedCharacter.PlayerKillCount = 3;
        player.SelectedCharacter.State = HeroState.PlayerKiller2ndStage;

        var plugin = new PkClearChatCommandPlugIn
        {
            Configuration = new PkClearChatCommandPlugIn.PKClearConfiguration
            {
                AllowRegularPlayers = false,
                ZenCostPerKill = 10_000_000,
            }
        };

        await plugin.HandleCommandAsync(player, "/pkclear").ConfigureAwait(false);

        // Verify status not cleared
        Assert.That(player.SelectedCharacter.PlayerKillCount, Is.EqualTo(3));
        Assert.That(player.SelectedCharacter.State, Is.EqualTo(HeroState.PlayerKiller2ndStage));
    }

    /// <summary>
    /// Verifies that a regular player successfully clears their PK status and gets charged Zen if AllowRegularPlayers is true.
    /// </summary>
    [Test]
    public async ValueTask RegularPlayerSuccessAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.SelectedCharacter!.CharacterStatus = CharacterStatus.Normal;
        player.SelectedCharacter.PlayerKillCount = 3;
        player.SelectedCharacter.State = HeroState.PlayerKiller2ndStage;
        player.Money = 50_000_000;

        var plugin = new PkClearChatCommandPlugIn
        {
            Configuration = new PkClearChatCommandPlugIn.PKClearConfiguration
            {
                AllowRegularPlayers = true,
                ZenCostPerKill = 10_000_000,
            }
        };

        await plugin.HandleCommandAsync(player, "/pkclear").ConfigureAwait(false);

        // Verify status cleared and money deducted
        Assert.That(player.SelectedCharacter.PlayerKillCount, Is.EqualTo(0));
        Assert.That(player.SelectedCharacter.State, Is.EqualTo(HeroState.Normal));
        Assert.That(player.Money, Is.EqualTo(20_000_000));
    }

    /// <summary>
    /// Verifies that a regular player with insufficient Zen fails to clear their PK status.
    /// </summary>
    [Test]
    public async ValueTask RegularPlayerNotEnoughMoneyAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.SelectedCharacter!.CharacterStatus = CharacterStatus.Normal;
        player.SelectedCharacter.PlayerKillCount = 3;
        player.SelectedCharacter.State = HeroState.PlayerKiller2ndStage;
        player.Money = 5_000_000;

        var plugin = new PkClearChatCommandPlugIn
        {
            Configuration = new PkClearChatCommandPlugIn.PKClearConfiguration
            {
                AllowRegularPlayers = true,
                ZenCostPerKill = 10_000_000,
            }
        };

        await plugin.HandleCommandAsync(player, "/pkclear").ConfigureAwait(false);

        // Verify status not cleared and money not deducted
        Assert.That(player.SelectedCharacter.PlayerKillCount, Is.EqualTo(3));
        Assert.That(player.SelectedCharacter.State, Is.EqualTo(HeroState.PlayerKiller2ndStage));
        Assert.That(player.Money, Is.EqualTo(5_000_000));
    }

    /// <summary>
    /// Verifies that a Game Master can clear a target player's PK status for free.
    /// </summary>
    [Test]
    public async ValueTask GameMasterClearsTargetForFreeAsync()
    {
        // Set up the GM
        var gm = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        gm.SelectedCharacter!.CharacterStatus = CharacterStatus.GameMaster;
        gm.SelectedCharacter.Name = "GM_Character";

        // Set up the target player
        var targetPlayer = await PlayerTestHelper.CreatePlayerAsync(gm.GameContext).ConfigureAwait(false);
        targetPlayer.SelectedCharacter!.CharacterStatus = CharacterStatus.Normal;
        targetPlayer.SelectedCharacter.Name = "Target_Character";
        targetPlayer.SelectedCharacter.PlayerKillCount = 5;
        targetPlayer.SelectedCharacter.State = HeroState.PlayerKiller2ndStage;
        targetPlayer.Money = 0;

        // Register target player in GameContext
        var gameContext = gm.GameContext as GameContext;
        Assert.That(gameContext, Is.Not.Null);
        gameContext!.PlayersByCharacterName.TryAdd(targetPlayer.SelectedCharacter.Name, targetPlayer);

        var plugin = new PkClearChatCommandPlugIn
        {
            Configuration = new PkClearChatCommandPlugIn.PKClearConfiguration
            {
                AllowRegularPlayers = true,
                ZenCostPerKill = 10_000_000,
            }
        };

        await plugin.HandleCommandAsync(gm, "/pkclear Target_Character").ConfigureAwait(false);

        // Verify target status cleared and no money deducted
        Assert.That(targetPlayer.SelectedCharacter.PlayerKillCount, Is.EqualTo(0));
        Assert.That(targetPlayer.SelectedCharacter.State, Is.EqualTo(HeroState.Normal));
        Assert.That(targetPlayer.Money, Is.EqualTo(0));
    }

    /// <summary>
    /// Verifies that a regular player cannot specify another character to clear their status.
    /// </summary>
    [Test]
    public async ValueTask RegularPlayerCannotClearTargetAsync()
    {
        // Set up the calling player
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.SelectedCharacter!.CharacterStatus = CharacterStatus.Normal;
        player.SelectedCharacter.Name = "Player_Character";
        player.SelectedCharacter.PlayerKillCount = 0;
        player.SelectedCharacter.State = HeroState.Normal;
        player.Money = 50_000_000;

        // Set up the target player
        var targetPlayer = await PlayerTestHelper.CreatePlayerAsync(player.GameContext).ConfigureAwait(false);
        targetPlayer.SelectedCharacter!.CharacterStatus = CharacterStatus.Normal;
        targetPlayer.SelectedCharacter.Name = "Target_Character";
        targetPlayer.SelectedCharacter.PlayerKillCount = 3;
        targetPlayer.SelectedCharacter.State = HeroState.PlayerKiller2ndStage;

        // Register target player in GameContext
        var gameContext = player.GameContext as GameContext;
        Assert.That(gameContext, Is.Not.Null);
        gameContext!.PlayersByCharacterName.TryAdd(targetPlayer.SelectedCharacter.Name, targetPlayer);

        var plugin = new PkClearChatCommandPlugIn
        {
            Configuration = new PkClearChatCommandPlugIn.PKClearConfiguration
            {
                AllowRegularPlayers = true,
                ZenCostPerKill = 10_000_000,
            }
        };

        await plugin.HandleCommandAsync(player, "/pkclear Target_Character").ConfigureAwait(false);

        // Target should NOT be cleared since player is regular and target is different character
        Assert.That(targetPlayer.SelectedCharacter.PlayerKillCount, Is.EqualTo(3));
        Assert.That(targetPlayer.SelectedCharacter.State, Is.EqualTo(HeroState.PlayerKiller2ndStage));
        Assert.That(player.Money, Is.EqualTo(50_000_000));
    }

    /// <summary>
    /// Verifies that integer overflow in cost calculation is prevented and capped at int.MaxValue.
    /// </summary>
    [Test]
    public async ValueTask RegularPlayerZenOverflowPreventionAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.SelectedCharacter!.CharacterStatus = CharacterStatus.Normal;
        player.SelectedCharacter.PlayerKillCount = 215;
        player.SelectedCharacter.State = HeroState.PlayerKiller2ndStage;
        player.Money = int.MaxValue - 1;

        var plugin = new PkClearChatCommandPlugIn
        {
            Configuration = new PkClearChatCommandPlugIn.PKClearConfiguration
            {
                AllowRegularPlayers = true,
                ZenCostPerKill = 10_000_000,
            }
        };

        await plugin.HandleCommandAsync(player, "/pkclear").ConfigureAwait(false);

        // Verify status is NOT cleared and money is NOT deducted.
        Assert.That(player.SelectedCharacter.PlayerKillCount, Is.EqualTo(215));
        Assert.That(player.SelectedCharacter.State, Is.EqualTo(HeroState.PlayerKiller2ndStage));
        Assert.That(player.Money, Is.EqualTo(int.MaxValue - 1));
    }
}
