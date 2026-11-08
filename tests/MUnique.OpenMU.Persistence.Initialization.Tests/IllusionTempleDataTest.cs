// <copyright file="IllusionTempleDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;
using IIdentifiable = MUnique.OpenMU.Persistence.IIdentifiable;

/// <summary>
/// Tests the data of the illusion temple event.
/// </summary>
/// <remarks>
/// The event configuration reaches a server through two independent paths: the data initialization of a
/// fresh database, and the update plug-in for a database which was created before the event existed. A
/// freshly initialized database marks every known update as installed, so the update plug-in never runs
/// there - which means the two have to be asserted to agree, or one of them silently rots.
/// </remarks>
[TestFixture]
internal class IllusionTempleDataTest
{
    private const short MirageNumber = 385;
    private const short StoneStatueNumber = 380;
    private const short AllianceGuardianNumber = 381;
    private const short IllusionGuardianNumber = 382;
    private const short AllianceItemStorageNumber = 383;
    private const short IllusionItemStorageNumber = 384;
    private const short DeviasNumber = 2;
    private const short ProtectionEffectNumber = 210;
    private const short RestraintEffectNumber = 211;

    private static readonly short[] TempleMapNumbers = [45, 46, 47, 48, 49, 50];

    /// <summary>
    /// The temples which roam with arena monsters, and the NPC numbers each of them uses. Temple 6
    /// has none, so it is not listed here.
    /// </summary>
    private static readonly (short MapNumber, short[] ArenaMonsters)[] TemplesWithArenaMonsters =
    [
        (45, [386, 387, 388]),
        (46, [389, 390, 391]),
        (47, [392, 393, 394]),
        (48, [395, 396, 397]),
        (49, [398, 399]),
    ];

    /// <summary>
    /// Tests that a newly initialized season 6 database contains the illusion temple event.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task NewDatabaseContainsEventAsync()
    {
        var gameConfiguration = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);

        AssertEventData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update adds the illusion temple event to a database which was created before it
    /// existed, and that applying it twice doesn't duplicate anything.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task UpdateAddsEventToExistingDatabaseAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        using var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();

        RevertToStateBeforeTheEvent(gameConfiguration);

        var update = new IllusionTempleDataUpdatePlugIn();
        var installedUpdate = (await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false)).Single(entry => entry.Key == update.Key);
        installedUpdate.Version = 0; // The reverted pre-event fixture has not received version one.
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertEventData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update brings the minimum player count of an already configured event up to a
    /// usable value. The field was added to <see cref="MiniGameDefinition"/> by this event, so it
    /// reads as 0 on every database which got the definitions from an earlier version of the update.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task UpdateFixesMissingMinimumPlayerCountAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        using var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        foreach (var definition in GetTempleDefinitions(gameConfiguration))
        {
            definition.MinimumPlayerCount = 0;
        }

        var update = new IllusionTempleDataUpdatePlugIn();
        var installedUpdate = (await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false)).Single(entry => entry.Key == update.Key);
        installedUpdate.Version = 0; // The missing-field fixture predates version one's repair.
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(GetTempleDefinitions(gameConfiguration), Has.All.Matches<MiniGameDefinition>(d => d.MinimumPlayerCount > 0));
        Assert.That(GetTempleDefinitions(gameConfiguration), Has.Count.EqualTo(TempleMapNumbers.Length), "The update must not duplicate the existing definitions.");
    }

    /// <summary>
    /// Repairs the ticket reference of existing temples both before and after the version-one item renumbering.
    /// The legacy initializer selected the Scroll of Blood definition while it still had number 51.
    /// </summary>
    /// <param name="numbersAlreadyCorrected">Whether version one already corrected the item numbers.</param>
    /// <returns>The task.</returns>
    [TestCase(false)]
    [TestCase(true)]
    public async Task UpdateRepairsExistingTempleTicketBindingAsync(bool numbersAlreadyCorrected)
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        using var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        var covenant = gameConfiguration.Items.Single(item => item is { Group: 13, Number: 51 });
        var scrollOfBlood = gameConfiguration.Items.Single(item => item is { Group: 13, Number: 50 });
        var covenantId = ((IIdentifiable)covenant).Id;
        var scrollId = ((IIdentifiable)scrollOfBlood).Id;
        var definitions = GetTempleDefinitions(gameConfiguration);
        if (!numbersAlreadyCorrected)
        {
            covenant.Number = 50;
            scrollOfBlood.Number = 51;
        }

        // Keep every existing definition: only its legacy ticket binding is reproduced.
        foreach (var definition in definitions)
        {
            definition.TicketItem = scrollOfBlood;
        }

        var itemsBefore = gameConfiguration.Items.ToArray();
        var levelsBefore = definitions.Select(definition => definition.TicketItemLevel).ToArray();
        var update = new IllusionTempleDataUpdatePlugIn();
        var installedUpdate = (await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false)).Single(entry => entry.Key == update.Key);
        installedUpdate.Version = numbersAlreadyCorrected ? 1 : 0;
        for (var application = 0; application < 2; application++)
        {
            await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

            Assert.Multiple(() =>
            {
                Assert.That(definitions.Select(definition => definition.TicketItem), Has.All.SameAs(covenant));
                Assert.That(covenant.Number, Is.EqualTo(51));
                Assert.That(scrollOfBlood.Number, Is.EqualTo(50));
                Assert.That(((IIdentifiable)covenant).Id, Is.EqualTo(covenantId));
                Assert.That(((IIdentifiable)scrollOfBlood).Id, Is.EqualTo(scrollId));
                Assert.That(GetTempleDefinitions(gameConfiguration), Has.Count.EqualTo(definitions.Count));
                Assert.That(definitions.All(original => gameConfiguration.MiniGameDefinitions.Any(current => ReferenceEquals(current, original))), Is.True);
                Assert.That(definitions.Select(definition => definition.TicketItemLevel), Is.EqualTo(levelsBefore));
                Assert.That(gameConfiguration.Items, Has.Count.EqualTo(itemsBefore.Length));
                Assert.That(itemsBefore.All(original => gameConfiguration.Items.Any(current => ReferenceEquals(current, original))), Is.True);
            });
        }
    }

    /// <summary>
    /// Owner-selected tickets and detached definitions with the legacy ticket's id are not defaults to repair.
    /// </summary>
    /// <param name="customization">The owner-selected ticket, detached reference, definition id or ticket level to preserve.</param>
    /// <returns>The task.</returns>
    [TestCase("owned-ticket")]
    [TestCase("detached-ticket")]
    [TestCase("definition-id")]
    [TestCase("ticket-level")]
    public async Task UpdatePreservesCustomizedTempleTicketBindingAsync(string customization)
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        using var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        var scrollOfBlood = gameConfiguration.Items.Single(item => item is { Group: 13, Number: 50 });
        var selectedTicket = customization == "owned-ticket"
            ? gameConfiguration.Items.Single(item => item is { Group: 13, Number: 18 })
            : scrollOfBlood;
        if (customization == "detached-ticket")
        {
            selectedTicket = new MUnique.OpenMU.Persistence.BasicModel.ItemDefinition
            {
                Id = ((IIdentifiable)scrollOfBlood).Id,
                Group = scrollOfBlood.Group,
                Number = scrollOfBlood.Number,
                Name = scrollOfBlood.Name,
            };
        }

        var definition = GetTempleDefinitions(gameConfiguration).Single(temple => temple.GameLevel == 6);
        if (customization == "definition-id")
        {
            ((IIdentifiable)definition).Id = Guid.NewGuid();
        }

        var definitionId = ((IIdentifiable)definition).Id;
        definition.TicketItem = selectedTicket;
        var selectedLevel = customization == "ticket-level" ? 3 : 6;
        definition.TicketItemLevel = selectedLevel;

        var update = new IllusionTempleDataUpdatePlugIn();
        for (var application = 0; application < 2; application++)
        {
            await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

            Assert.That(definition.TicketItem, Is.SameAs(selectedTicket));
            Assert.That(definition.TicketItemLevel, Is.EqualTo(selectedLevel));
            Assert.That(((IIdentifiable)definition).Id, Is.EqualTo(definitionId));
        }
    }

    /// <summary>
    /// Updating an installed version one must not replay its broad content changes over owner customizations.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task VersionTwoPreservesVersionOneContentCustomizationsAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        using var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        var update = new IllusionTempleDataUpdatePlugIn();
        var installedUpdate = (await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false)).Single(entry => entry.Key == update.Key);
        installedUpdate.Version = 1;

        var map = gameConfiguration.Maps.Single(candidate => candidate.Number == 45);
        var safezone = gameConfiguration.Maps.Single(candidate => candidate.Number == 0);
        map.SafezoneMap = safezone;
        var spawn = context.CreateNew<MonsterSpawnArea>();
        spawn.GameMap = map;
        spawn.MonsterDefinition = gameConfiguration.Monsters.Single(monster => monster.Number == 658);
        spawn.Quantity = 1;
        spawn.X1 = spawn.X2 = 150;
        spawn.Y1 = spawn.Y2 = 100;
        map.MonsterSpawns.Add(spawn);
        var spawnSettings = (spawn.Quantity, spawn.X1, spawn.X2, spawn.Y1, spawn.Y2);
        var definition = GetTempleDefinitions(gameConfiguration).Single(temple => temple.GameLevel == 6);
        var ticket = gameConfiguration.Items.Single(item => item is { Group: 13, Number: 18 });
        definition.TicketItem = ticket;

        for (var application = 0; application < 2; application++)
        {
            await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

            Assert.That(map.SafezoneMap, Is.SameAs(safezone));
            Assert.That(map.MonsterSpawns.Any(current => ReferenceEquals(current, spawn)), Is.True);
            Assert.That((spawn.Quantity, spawn.X1, spawn.X2, spawn.Y1, spawn.Y2), Is.EqualTo(spawnSettings));
            Assert.That(definition.TicketItem, Is.SameAs(ticket));
            Assert.That(installedUpdate.Version, Is.EqualTo(2));
        }
    }

    private static async Task<GameConfiguration> CreateSeason6ConfigurationAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        using var context = contextProvider.CreateNewContext();
        return (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
    }

    private static List<MiniGameDefinition> GetTempleDefinitions(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.MiniGameDefinitions.Where(d => d.Type == MiniGameType.IllusionTemple).ToList();
    }

    /// <summary>
    /// Removes everything the update plug-in is responsible for, so that the configuration looks like
    /// one of a server which was installed before the event existed.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration to revert.</param>
    private static void RevertToStateBeforeTheEvent(GameConfiguration gameConfiguration)
    {
        foreach (var definition in GetTempleDefinitions(gameConfiguration))
        {
            gameConfiguration.MiniGameDefinitions.Remove(definition);
        }

        // The ticket and the Scroll of Blood used to have their numbers swapped.
        var covenant = gameConfiguration.Items.Single(item => item is { Group: 13, Number: 51 });
        var scrollOfBlood = gameConfiguration.Items.Single(item => item is { Group: 13, Number: 50 });
        covenant.Number = 50;
        scrollOfBlood.Number = 51;

        gameConfiguration.Items.Remove(gameConfiguration.Items.Single(item => item is { Group: 14, Number: 64 }));

        foreach (var monster in gameConfiguration.Monsters.Where(monster => monster.Number is >= 386 and <= 399).ToList())
        {
            gameConfiguration.Monsters.Remove(monster);
        }

        gameConfiguration.Monsters.Single(monster => monster.Number == MirageNumber).NpcWindow = NpcWindow.Undefined;

        foreach (var effect in gameConfiguration.MagicEffects
                     .Where(effect => effect.Number is ProtectionEffectNumber or RestraintEffectNumber).ToList())
        {
            gameConfiguration.MagicEffects.Remove(effect);
        }

        foreach (var map in gameConfiguration.Maps.Where(map => TempleMapNumbers.Contains(map.Number)))
        {
            // The maps used to point at themselves as their own safezone.
            map.SafezoneMap = map;

            foreach (var spawn in map.MonsterSpawns.ToList())
            {
                map.MonsterSpawns.Remove(spawn);
            }

            // The arena used to be littered with "Captured Stone Statue" spawns, which were based on
            // the wrong NPC numbers and had no function.
            foreach (var obsolete in gameConfiguration.Monsters.Where(monster => monster.Number is >= 658 and <= 668))
            {
                var area = new MUnique.OpenMU.Persistence.BasicModel.MonsterSpawnArea
                {
                    GameMap = map,
                    MonsterDefinition = obsolete,
                    Quantity = 1,
                    SpawnTrigger = SpawnTrigger.AutomaticDuringEvent,
                    X1 = 169,
                    X2 = 169,
                    Y1 = 85,
                    Y2 = 85,
                };
                map.MonsterSpawns.Add(area);
            }
        }
    }

    private static void AssertEventData(GameConfiguration gameConfiguration)
    {
        AssertItems(gameConfiguration);
        AssertDefinitions(gameConfiguration);
        AssertNpcsAndEffects(gameConfiguration);
        AssertMaps(gameConfiguration);
    }

    private static void AssertItems(GameConfiguration gameConfiguration)
    {
        // The event ticket is looked up as group 13, number 51 by the initializer, and the update
        // corrects databases which still carry the two swapped numbers.
        var ticket = gameConfiguration.Items.Single(item => item is { Group: 13, Number: 51 });
        Assert.That(ticket.Name.ValueInNeutralLanguage, Is.EqualTo("Illusion Sorcerer Covenant"));
        var scrollOfBlood = gameConfiguration.Items.Single(item => item is { Group: 13, Number: 50 });
        Assert.That(scrollOfBlood.Name.ValueInNeutralLanguage, Is.EqualTo("Scroll of Blood"));

        // The sacred relic, which the game logic looks up by group and number.
        Assert.That(gameConfiguration.Items.Count(item => item is { Group: 14, Number: 64 }), Is.EqualTo(1), "sacred relic");
    }

    private static void AssertDefinitions(GameConfiguration gameConfiguration)
    {
        var definitions = GetTempleDefinitions(gameConfiguration);
        var ticket = gameConfiguration.Items.Single(item => item is { Group: 13, Number: 51 });

        Assert.That(definitions, Has.Count.EqualTo(TempleMapNumbers.Length));
        Assert.That(definitions.Select(d => (short)d.GameLevel), Is.EquivalentTo(new short[] { 1, 2, 3, 4, 5, 6 }));
        Assert.That(definitions.Select(d => d.Entrance?.Map?.Number), Is.EquivalentTo(TempleMapNumbers.Select(n => (short?)n)));
        Assert.That(definitions, Has.All.Matches<MiniGameDefinition>(d => d.TicketItem == ticket));
        Assert.That(definitions, Has.All.Matches<MiniGameDefinition>(d => d.MapCreationPolicy == MiniGameMapCreationPolicy.Shared));

        // The event is team based and scores per team, so parties are not allowed, and it needs at
        // least two players to have two teams at all.
        Assert.That(definitions, Has.All.Matches<MiniGameDefinition>(d => !d.AllowParty));
        Assert.That(definitions, Has.All.Matches<MiniGameDefinition>(d => d.MinimumPlayerCount >= 2));
        Assert.That(definitions, Has.All.Matches<MiniGameDefinition>(d => d.MaximumPlayerCount >= d.MinimumPlayerCount));

        // Each temple covers its own level range, and the sixth one is the master class temple.
        Assert.That(definitions.Where(d => d.GameLevel < 6), Has.All.Matches<MiniGameDefinition>(d => d.MaximumCharacterLevel > d.MinimumCharacterLevel));
        Assert.That(definitions.Single(d => d.GameLevel == 6).RequiresMasterClass, Is.True);

        // Every temple rewards the winning team with experience.
        Assert.That(
            definitions,
            Has.All.Matches<MiniGameDefinition>(d => d.Rewards.Any(r => r.RewardType == MiniGameRewardType.Experience)));
    }

    private static void AssertNpcsAndEffects(GameConfiguration gameConfiguration)
    {
        Assert.That(
            gameConfiguration.Monsters.Single(monster => monster.Number == MirageNumber).NpcWindow,
            Is.EqualTo(NpcWindow.IllusionTemple));

        Assert.That(
            gameConfiguration.Monsters.Where(monster => monster.Number is >= 386 and <= 399).Select(monster => monster.Number),
            Is.EquivalentTo(Enumerable.Range(386, 14).Select(n => (short)n)),
            "the roaming arena monsters of all temples");

        // The two special skills which apply a lasting effect need their magic effect definitions;
        // Tracking and Weaken act instantly and have none.
        foreach (var effectNumber in new[] { ProtectionEffectNumber, RestraintEffectNumber })
        {
            Assert.That(gameConfiguration.MagicEffects.Count(effect => effect.Number == effectNumber), Is.EqualTo(1), $"magic effect {effectNumber}");
        }
    }

    private static void AssertMaps(GameConfiguration gameConfiguration)
    {
        var temples = gameConfiguration.Maps.Where(map => TempleMapNumbers.Contains(map.Number)).ToList();
        Assert.That(temples, Has.Count.EqualTo(TempleMapNumbers.Length));

        // A player who leaves the arena has to end up outside of it, not back inside.
        Assert.That(temples.Select(map => map.SafezoneMap?.Number), Has.All.EqualTo(DeviasNumber));

        foreach (var map in temples)
        {
            Assert.That(
                map.MonsterSpawns.Any(spawn => spawn.MonsterDefinition?.Number is >= 658 and <= 668),
                Is.False,
                $"map {map.Number} still carries the obsolete statue spawns");

            // The statue is spawned at one of a pool of positions, picked by the game logic, so all of
            // them are configured but only triggered manually.
            var statueSpawns = map.MonsterSpawns.Where(spawn => spawn.MonsterDefinition?.Number == StoneStatueNumber).ToList();
            Assert.That(statueSpawns, Is.Not.Empty, $"map {map.Number}: stone statue");
            Assert.That(statueSpawns, Has.All.Matches<MonsterSpawnArea>(spawn => spawn.SpawnTrigger == SpawnTrigger.ManuallyForEvent));

            foreach (var npcNumber in new[] { AllianceGuardianNumber, IllusionGuardianNumber, AllianceItemStorageNumber, IllusionItemStorageNumber })
            {
                Assert.That(
                    map.MonsterSpawns.Count(spawn => spawn.MonsterDefinition?.Number == npcNumber),
                    Is.EqualTo(1),
                    $"map {map.Number}: NPC {npcNumber}");
            }
        }

        foreach (var (mapNumber, arenaMonsters) in TemplesWithArenaMonsters)
        {
            var map = temples.Single(m => m.Number == mapNumber);
            Assert.That(
                map.MonsterSpawns.Where(spawn => spawn.MonsterDefinition?.Number is >= 386 and <= 399)
                    .Select(spawn => spawn.MonsterDefinition!.Number)
                    .Distinct(),
                Is.EquivalentTo(arenaMonsters),
                $"map {mapNumber}: arena monsters");
        }

        // The sixth temple is the master class one and roams with no arena monsters.
        Assert.That(
            temples.Single(map => map.Number == 50).MonsterSpawns
                .Any(spawn => spawn.MonsterDefinition?.Number is >= 386 and <= 399),
            Is.False,
            "temple 6 has no arena monsters");
    }
}
