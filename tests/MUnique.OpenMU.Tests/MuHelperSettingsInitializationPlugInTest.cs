// <copyright file="MuHelperSettingsInitializationPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameServer.RemoteView.MuHelper;

/// <summary>
/// Tests for the <see cref="MuHelperSettingsInitializationPlugIn"/>.
/// </summary>
[TestFixture]
public class MuHelperSettingsInitializationPlugInTest
{
    private const int TwistingSlash = 41;
    private const int FireBurst = 61;

    private readonly MuHelperSettingsInitializationPlugIn _plugIn = new();

    /// <summary>
    /// A character without saved settings, selected after another one on the same
    /// connection, does not keep the settings of the previous character.
    /// </summary>
    [Test]
    public async ValueTask NewCharacterWithoutConfigurationDoesNotKeepPreviousSettingsAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.MuHelperSettings = MuHelperSettingsSerializer.TryDeserialize(CreateBlob(TwistingSlash));
        player.SelectedCharacter!.MuHelperConfiguration = null;

        await this._plugIn.PlayerStateChangedAsync(player, PlayerState.CharacterSelection, PlayerState.EnteredWorld).ConfigureAwait(false);

        Assert.That(player.MuHelperSettings, Is.Null);
    }

    /// <summary>
    /// A character with saved settings, selected after another one, gets its own settings.
    /// </summary>
    [Test]
    public async ValueTask CharacterWithConfigurationGetsItsOwnSettingsAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.MuHelperSettings = MuHelperSettingsSerializer.TryDeserialize(CreateBlob(TwistingSlash));
        player.SelectedCharacter!.MuHelperConfiguration = CreateBlob(FireBurst);

        await this._plugIn.PlayerStateChangedAsync(player, PlayerState.CharacterSelection, PlayerState.EnteredWorld).ConfigureAwait(false);

        Assert.That(player.MuHelperSettings?.BasicSkillId, Is.EqualTo(FireBurst));
    }

    /// <summary>
    /// Coming back to the world from a dialog (trade, NPC...) keeps the current settings.
    /// </summary>
    [Test]
    public async ValueTask ReturningFromDialogKeepsSettingsAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var settings = MuHelperSettingsSerializer.TryDeserialize(CreateBlob(TwistingSlash));
        player.MuHelperSettings = settings;
        player.SelectedCharacter!.MuHelperConfiguration = null;

        await this._plugIn.PlayerStateChangedAsync(player, PlayerState.NpcDialogOpened, PlayerState.EnteredWorld).ConfigureAwait(false);

        Assert.That(player.MuHelperSettings, Is.SameAs(settings));
    }

    /// <summary>
    /// Offline players and bots get their settings before entering the world; they are kept.
    /// </summary>
    [Test]
    public async ValueTask OfflinePlayerKeepsPresetSettingsAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var player = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var settings = MuHelperSettingsSerializer.TryDeserialize(CreateBlob(TwistingSlash));
        player.MuHelperSettings = settings;
        player.SelectedCharacter!.MuHelperConfiguration = null;

        await this._plugIn.PlayerStateChangedAsync(player, PlayerState.CharacterSelection, PlayerState.EnteredWorld).ConfigureAwait(false);

        Assert.That(player.MuHelperSettings, Is.SameAs(settings));
    }

    private static byte[] CreateBlob(int basicSkill)
    {
        var blob = new byte[257];
        blob[5] = (byte)(basicSkill & 0xFF);
        blob[6] = (byte)(basicSkill >> 8);
        return blob;
    }
}
