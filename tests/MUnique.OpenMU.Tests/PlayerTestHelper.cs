// <copyright file="PlayerTestHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.IO;
using System.IO.Pipelines;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using Nito.AsyncEx;

/// <summary>
/// Helper functions to create test players.
/// </summary>
public static class PlayerTestHelper
{
    /// <summary>
    /// Gets a test player with a new in-memory game context.
    /// </summary>
    /// <param name="dropGenerator">The drop generator of the game context. Defaults to <see cref="NullDropGenerator"/>.</param>
    /// <returns>The test player.</returns>
    public static async ValueTask<Player> CreatePlayerAsync(IDropGenerator? dropGenerator = null)
    {
        dropGenerator ??= NullDropGenerator.Instance;
        var gameConfiguration = CreateGameConfiguration();
        var mapInitializer = new MapInitializer(gameConfiguration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var gameContext = new GameContext(gameConfiguration, new InMemoryPersistenceContextProvider(), mapInitializer, new NullLoggerFactory(), new PlugInManager(null, new NullLoggerFactory(), null, null), dropGenerator, new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = gameContext.PlugInManager;
        mapInitializer.PathFinderPool = gameContext.PathFinderPool;
        return await CreatePlayerAsync(gameContext).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a minimal game configuration with one map with the number 0, which is sufficient for test players.
    /// </summary>
    /// <returns>The game configuration.</returns>
    public static GameConfiguration CreateGameConfiguration()
    {
        var gameConfig = new Mock<GameConfiguration>();
        gameConfig.SetupAllProperties();
        gameConfig.Setup(c => c.Maps).Returns(new List<GameMapDefinition>());
        gameConfig.Setup(c => c.Items).Returns(new List<ItemDefinition>());
        gameConfig.Setup(c => c.Skills).Returns(new List<Skill>());
        gameConfig.Setup(c => c.PlugInConfigurations).Returns(new List<PlugInConfiguration>());
        gameConfig.Setup(c => c.CharacterClasses).Returns(new List<CharacterClass>());
        gameConfig.Setup(c => c.WarpList).Returns(new List<WarpInfo>());
        gameConfig.Setup(c => c.GlobalAttributeCombinations).Returns(new List<AttributeRelationship>());
        gameConfig.Setup(c => c.GlobalBaseAttributeValues).Returns(new List<ConstValueAttribute>
        {
            new(1, Stats.MoneyAmountRate),
        });
        var map = new Mock<GameMapDefinition>();
        map.SetupAllProperties();
        map.Setup(m => m.DropItemGroups).Returns(new List<DropItemGroup>());
        map.Setup(m => m.MonsterSpawns).Returns(new List<MonsterSpawnArea>());
        map.Setup(m => m.ExitGates).Returns(new List<ExitGate>());
        map.Object.TerrainData = new byte[ushort.MaxValue + 3];
        gameConfig.Object.RecoveryInterval = int.MaxValue;
        gameConfig.Object.ItemDropDuration = TimeSpan.FromMinutes(1);
        gameConfig.Object.Maps.Add(map.Object);
        return gameConfig.Object;
    }

    /// <summary>
    /// Gets a test player with the specified game context.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The test player.</returns>
    public static async ValueTask<Player> CreatePlayerAsync(IGameContext gameContext)
    {
        var player = new TestPlayer(gameContext);
        await EnterWorldAsync(player, CreateCharacter(gameContext)).ConfigureAwait(false);
        return player;
    }

    /// <summary>
    /// Creates a remote player with an in-memory packet output and a minimal game server context.
    /// </summary>
    /// <param name="clientVersion">The client version of the player.</param>
    /// <returns>The remote player and its output stream.</returns>
    public static (RemotePlayer Player, MemoryStream Output) CreateRemotePlayer(ClientVersion clientVersion = default)
    {
        var plugInManager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        return CreateRemotePlayer(plugInManager, clientVersion);
    }

    /// <summary>
    /// Creates a remote player with an in-memory packet output and a minimal game server context
    /// which uses the specified plug-in manager.
    /// </summary>
    /// <param name="plugInManager">The plug-in manager of the game server context.</param>
    /// <param name="clientVersion">The client version of the player.</param>
    /// <returns>The remote player and its output stream.</returns>
    public static (RemotePlayer Player, MemoryStream Output) CreateRemotePlayer(PlugInManager plugInManager, ClientVersion clientVersion = default)
    {
        var gameContext = new Mock<IGameServerContext>();
        gameContext.Setup(c => c.PersistenceContextProvider)
            .Returns(new Mock<IPersistenceContextProvider> { DefaultValue = DefaultValue.Mock }.Object);
        gameContext.Setup(c => c.Configuration).Returns(new GameConfiguration());
        gameContext.Setup(c => c.PlugInManager).Returns(plugInManager);
        gameContext.Setup(c => c.FeaturePlugIns).Returns(new FeaturePlugInContainer(plugInManager));
        gameContext.Setup(c => c.LoggerFactory).Returns(new NullLoggerFactory());
        return CreateRemotePlayer(gameContext.Object, clientVersion);
    }

    /// <summary>
    /// Creates a remote player with an in-memory packet output for an existing game server context.
    /// </summary>
    /// <param name="gameContext">The game server context.</param>
    /// <param name="clientVersion">The client version of the player.</param>
    /// <returns>The remote player and its output stream.</returns>
    public static (RemotePlayer Player, MemoryStream Output) CreateRemotePlayer(IGameServerContext gameContext, ClientVersion clientVersion = default)
    {
        var output = new MemoryStream();
        var writer = PipeWriter.Create(output, new StreamPipeWriterOptions(leaveOpen: true));
        var connection = new Mock<IConnection>();
        connection.SetupGet(c => c.Connected).Returns(true);
        connection.SetupGet(c => c.Output).Returns(writer);
        connection.SetupGet(c => c.OutputLock).Returns(new AsyncLock());
        return (new RemotePlayer(gameContext, connection.Object, clientVersion), output);
    }

    /// <summary>
    /// Creates a character of a minimal character class on the map with the number 0, which is sufficient for test players.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The character.</returns>
    public static Character CreateCharacter(IGameContext gameContext)
    {
        var characterMock = new Mock<Character>();
        characterMock.SetupAllProperties();
        characterMock.Setup(c => c.LearnedSkills).Returns(new List<SkillEntry>());
        characterMock.Setup(c => c.Attributes).Returns(new List<StatAttribute>());
        characterMock.Setup(c => c.DropItemGroups).Returns(new List<DropItemGroup>());

        var inventoryMock = new Mock<ItemStorage>();
        inventoryMock.SetupAllProperties();
        inventoryMock.Setup(i => i.Items).Returns(new List<Item>());

        var character = characterMock.Object;
        character.Inventory = inventoryMock.Object;
        character.CurrentMap = gameContext.Configuration.Maps.FirstOrDefault(m => m.Number == 0);
        var characterClassMock = new Mock<CharacterClass>();
        characterClassMock.Setup(c => c.StatAttributes).Returns(
            new List<StatAttributeDefinition>
            {
                new (Stats.Level, 0, false),
                new (Stats.MasterLevel, 0, false),
                new (Stats.BaseStrength, 28, true),
                new (Stats.BaseAgility, 20, true),
                new (Stats.BaseVitality, 25, true),
                new (Stats.BaseEnergy, 10, true),
                new (Stats.CurrentHealth, 0, false),
                new (Stats.CurrentMana, 0, false),
                new (Stats.CurrentShield, 0, false),
                new (Stats.Resets, 0, false),
                new (Stats.PointsPerReset, 0, false),
            });
        characterClassMock.Setup(c => c.AttributeCombinations).Returns(new List<AttributeRelationship>
        {
            new (Stats.TotalStrength, 1, Stats.BaseStrength),
            new (Stats.TotalAgility, 1, Stats.BaseAgility),
            new (Stats.TotalVitality, 1, Stats.BaseVitality),
            new (Stats.TotalEnergy, 1, Stats.BaseEnergy),

            new (Stats.MaximumAbility, 1, Stats.TotalEnergy),
            new (Stats.MaximumAbility, 0.3f, Stats.TotalVitality),
            new (Stats.MaximumAbility, 0.2f, Stats.TotalAgility),
            new (Stats.MaximumAbility, 0.15f, Stats.TotalStrength),

            new (Stats.MaximumShield, 1.2f, Stats.TotalEnergy),
            new (Stats.MaximumShield, 1.2f, Stats.TotalVitality),
            new (Stats.MaximumShield, 1.2f, Stats.TotalAgility),
            new (Stats.MaximumShield, 1.2f, Stats.TotalStrength),
            new (Stats.MaximumShield, 0.5f, Stats.DefenseBase),

            new (Stats.MaximumMana, 1, Stats.TotalEnergy),
            new (Stats.MaximumMana, 0.5f, Stats.Level),
            new (Stats.MaximumHealth, 2, Stats.Level),
            new (Stats.MaximumHealth, 3, Stats.TotalVitality),
        });
        characterClassMock.Setup(c => c.BaseAttributeValues).Returns(new List<ConstValueAttribute>
        {
            new (10, Stats.MaximumMana),
            new (35, Stats.MaximumHealth),
            new (2, Stats.SkillMultiplier),
            new (2, Stats.AbilityRecoveryMultiplier),
            new (1, Stats.DamageReceiveDecrement),
            new (1, Stats.AttackDamageIncrease),
        });
        character.CharacterClass = characterClassMock.Object;

        foreach (var attributeDef in character.CharacterClass.StatAttributes)
        {
            character.Attributes.Add(new StatAttribute(attributeDef.Attribute!, attributeDef.BaseValue));
        }

        return character;
    }

    /// <summary>
    /// Logs the player in with a new account and lets it enter the world with the character.
    /// </summary>
    /// <param name="player">The player, e.g. a remote player whose packets are tested.</param>
    /// <param name="character">The character, see <see cref="CreateCharacter"/>.</param>
    public static async ValueTask EnterWorldAsync(Player player, Character character)
    {
        var accountMock = new Mock<Account>();
        accountMock.Setup(mock => mock.Attributes).Returns(new List<StatAttribute>());
        accountMock.Setup(mock => mock.UnlockedCharacterClasses).Returns(new List<CharacterClass>());
        player.Account = accountMock.Object;
        await player.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.CharacterSelection).ConfigureAwait(false);
        await player.SetSelectedCharacterAsync(character).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates an offline player at <see cref="PlayerState.EnteredWorld"/>.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The offline player.</returns>
    public static async ValueTask<OfflinePlayer> CreateOfflineLevelingPlayerAsync(IGameContext gameContext)
    {
        var regularPlayer = await CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var offlinePlayer = new OfflinePlayer(gameContext) { Account = regularPlayer.Account };
        await offlinePlayer.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await offlinePlayer.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
        await offlinePlayer.PlayerState.TryAdvanceToAsync(PlayerState.CharacterSelection).ConfigureAwait(false);
        await offlinePlayer.SetSelectedCharacterAsync(regularPlayer.SelectedCharacter!).ConfigureAwait(false);
        return offlinePlayer;
    }

    private class TestPlayer : Player
    {
        public TestPlayer(IGameContext gameContext)
            : base(gameContext)
        {
        }

        protected override ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer()
        {
            return new MockViewPlugInContainer();
        }
    }
}
