// <copyright file="OfflinePlayerManagerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offline;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Tests for <see cref="OfflinePlayerManager"/>.
/// </summary>
[TestFixture]
public class OfflinePlayerManagerTests
{
    private const string TestUserLoginName = "test";
    private const string TestCharacterName = "testChar";

    private IGameContext _gameContext = null!;
    private IPersistenceContextProvider _contextProvider = null!;

    /// <summary>
    /// Sets up a fresh game context before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._gameContext = GameContextTestHelper.CreateGameContext();
        this._contextProvider = this._gameContext.PersistenceContextProvider;
    }

    /// <summary>
    /// Tests that <see cref="OfflinePlayerManager.StartAsync"/> returns true on success.
    /// </summary>
    [Test]
    public async ValueTask StartAsync_WithValidPlayer_ReturnsTrueAsync()
    {
        // Arrange
        var manager = new OfflinePlayerManager();
        var realPlayer = await this.CreatePlayerWithPersistedAccountAsync().ConfigureAwait(false);
        realPlayer.TryAddMoney(1_000_000);
        realPlayer.Attributes![Stats.Level] = 100;

        // Act
        var result = await manager.StartAsync(realPlayer, TestUserLoginName).ConfigureAwait(false);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(manager.IsActive(TestUserLoginName), Is.True);
    }

    /// <summary>
    /// Tests that <see cref="OfflinePlayerManager.StartAsync"/> fails if the player has insufficient Zen.
    /// </summary>
    [Test]
    public async ValueTask StartAsync_WithInsufficientZen_ReturnsFalseAsync()
    {
        // Arrange
        var manager = new OfflinePlayerManager();
        var realPlayer = await this.CreatePlayerWithPersistedAccountAsync().ConfigureAwait(false);
        realPlayer.Money = 0; // No money
        realPlayer.Attributes![Stats.Level] = 100;

        // Act
        var result = await manager.StartAsync(realPlayer, TestUserLoginName).ConfigureAwait(false);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(manager.IsActive(TestUserLoginName), Is.False);
    }

    /// <summary>
    /// Tests that <see cref="OfflinePlayerManager.StopAsync"/> successfully stops a session.
    /// </summary>
    [Test]
    public async ValueTask StopAsync_WhenSessionActive_StopsSuccessfullyAsync()
    {
        // Arrange
        var manager = new OfflinePlayerManager();
        var realPlayer = await this.CreatePlayerWithPersistedAccountAsync().ConfigureAwait(false);
        realPlayer.TryAddMoney(1_000_000);
        realPlayer.Attributes![Stats.Level] = 100;

        var started = await manager.StartAsync(realPlayer, TestUserLoginName).ConfigureAwait(false);
        Assert.That(started, Is.True);
        Assert.That(manager.IsActive(TestUserLoginName), Is.True);

        // Act
        await manager.StopAsync(TestUserLoginName).ConfigureAwait(false);

        // Assert
        Assert.That(manager.IsActive(TestUserLoginName), Is.False);
    }

    /// <summary>
    /// Creates a player whose account is stored in the in-memory repository,
    /// so that <see cref="OfflinePlayer.InitializeAsync"/> can find it.
    /// </summary>
    private async ValueTask<Player> CreatePlayerWithPersistedAccountAsync()
    {
        var config = this._gameContext.Configuration;

        // Create persisted account + character in the in-memory repository.
        using (var ctx = this._contextProvider.CreateNewPlayerContext(config))
        {
            var account = ctx.CreateNew<MUnique.OpenMU.DataModel.Entities.Account>();
            account.LoginName = TestUserLoginName;

            var character = ctx.CreateNew<MUnique.OpenMU.DataModel.Entities.Character>();
            character.Name = TestCharacterName;

            if (config.CharacterClasses.FirstOrDefault() is { } existingClass)
            {
                character.CharacterClass = existingClass;
            }
            else
            {
                // Build a minimal CharacterClass so OnPlayerEnteredWorldAsync does not throw.
                var characterClass = ctx.CreateNew<MUnique.OpenMU.DataModel.Configuration.CharacterClass>();
                characterClass.HomeMap = config.Maps.FirstOrDefault();
                character.CharacterClass = characterClass;
            }

            account.Characters.Add(character);
            await ctx.SaveChangesAsync().ConfigureAwait(false);
        }

        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        player.Account!.LoginName = TestUserLoginName;

        // Ensure the mock character name matches the persisted one.
        var mockCharacter = player.SelectedCharacter!;
        mockCharacter.Name = TestCharacterName;
        mockCharacter.CharacterClass ??= player.Account.UnlockedCharacterClasses.FirstOrDefault();
        if (mockCharacter.CharacterClass is null && config.CharacterClasses.FirstOrDefault() is { } cc)
        {
            mockCharacter.CharacterClass = cc;
        }

        return player;
    }
}
