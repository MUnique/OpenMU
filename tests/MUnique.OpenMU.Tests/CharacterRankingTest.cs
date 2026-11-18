// <copyright file="CharacterRankingTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.EntityFrameworkCore;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for the <see cref="IPlayerContext.GetCharacterRankingAsync"/>.
/// </summary>
[TestFixture]
public class CharacterRankingTest
{
    /// <summary>
    /// Tests that the query of the entity framework implementation can be translated to SQL.
    /// </summary>
    [Test]
    public void EntityFrameworkQueryIsTranslatable()
    {
        using var context = new AccountContext();

        var sql = PlayerContext.CreateCharacterRankingQuery(context.Set<Persistence.EntityFramework.Model.Account>(), Stats.Level.Id, Stats.MasterLevel.Id, Stats.Resets.Id, 10).ToQueryString();

        Assert.That(sql, Does.Contain("ORDER BY"));
    }

    /// <summary>
    /// Tests that the query of the accounts of characters can be translated to SQL.
    /// </summary>
    [Test]
    public void AccountIdsByCharacterNamesQueryIsTranslatable()
    {
        using var context = new AccountContext();

        var sql = PlayerContext.CreateAccountIdsByCharacterNamesQuery(context.Set<Persistence.EntityFramework.Model.Account>(), new[] { "Hero", "Elf" }).ToQueryString();

        Assert.That(sql, Does.Contain("JOIN"));
    }

    /// <summary>
    /// Tests that the characters are ranked by resets, master level and level,
    /// and that bots and game masters are not ranked.
    /// </summary>
    [Test]
    public async Task CharactersAreRankedAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        using (var context = contextProvider.CreateNewPlayerContext(new GameConfiguration()))
        {
            var characterClass = context.CreateNew<CharacterClass>();
            characterClass.Name = "Blade Knight";
            AddAccount(context, characterClass, isBot: false, ("Normal", 400, 0, 0, CharacterStatus.Normal), ("Master", 400, 50, 0, CharacterStatus.Normal));
            AddAccount(context, characterClass, isBot: false, ("Reset", 100, 0, 1, CharacterStatus.Normal), ("GameMaster", 400, 200, 5, CharacterStatus.GameMaster));
            AddAccount(context, characterClass, isBot: true, ("Bot", 400, 200, 5, CharacterStatus.Normal));
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        using var queryContext = contextProvider.CreateNewPlayerContext(new GameConfiguration());
        var ranking = await queryContext.GetCharacterRankingAsync(Stats.Level.Id, Stats.MasterLevel.Id, Stats.Resets.Id, 10).ConfigureAwait(false);

        Assert.That(ranking.Select(entry => entry.Name), Is.EqualTo(new[] { "Reset", "Master", "Normal" }));
        Assert.That(ranking[1] with { CharacterClassId = null }, Is.EqualTo(new CharacterSummary("Master", "Blade Knight", 400, 50, 0)));
    }

    /// <summary>
    /// Tests that the query with a class filter of the entity framework implementation can be translated to SQL.
    /// </summary>
    [Test]
    public void EntityFrameworkClassFilterIsTranslatable()
    {
        using var context = new AccountContext();

        var sql = PlayerContext.CreateCharacterRankingQuery(context.Set<Persistence.EntityFramework.Model.Account>(), Stats.Level.Id, Stats.MasterLevel.Id, Stats.Resets.Id, 10, [Guid.NewGuid()]).ToQueryString();

        Assert.That(sql, Does.Contain("CharacterClassId"));
    }

    /// <summary>
    /// Tests that only the characters of the requested classes are ranked.
    /// </summary>
    [Test]
    public async Task RankingIsFilteredByClassAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        Guid knightId;
        using (var context = contextProvider.CreateNewPlayerContext(new GameConfiguration()))
        {
            var knight = context.CreateNew<CharacterClass>();
            knight.Name = "Dark Knight";
            var wizard = context.CreateNew<CharacterClass>();
            wizard.Name = "Dark Wizard";
            knightId = knight.GetId();
            AddAccount(context, knight, isBot: false, ("Knight", 300, 0, 0, CharacterStatus.Normal));
            AddAccount(context, wizard, isBot: false, ("Wizard", 400, 0, 0, CharacterStatus.Normal));
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        using var queryContext = contextProvider.CreateNewPlayerContext(new GameConfiguration());
        var ranking = await queryContext.GetCharacterRankingAsync(Stats.Level.Id, Stats.MasterLevel.Id, Stats.Resets.Id, 10, [knightId]).ConfigureAwait(false);

        Assert.That(ranking.Select(entry => entry.Name), Is.EqualTo(new[] { "Knight" }));
    }

    /// <summary>
    /// Tests that the summary of a character is found by its name.
    /// </summary>
    [Test]
    public async Task CharacterSummaryIsFoundAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        using (var context = contextProvider.CreateNewPlayerContext(new GameConfiguration()))
        {
            var characterClass = context.CreateNew<CharacterClass>();
            characterClass.Name = "Dark Wizard";
            AddAccount(context, characterClass, isBot: false, ("Wizard", 250, 0, 2, CharacterStatus.Normal));
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        using var queryContext = contextProvider.CreateNewPlayerContext(new GameConfiguration());

        Assert.That(
            (await queryContext.GetCharacterSummaryAsync("Wizard", Stats.Level.Id, Stats.MasterLevel.Id, Stats.Resets.Id).ConfigureAwait(false))! with { CharacterClassId = null },
            Is.EqualTo(new CharacterSummary("Wizard", "Dark Wizard", 250, 0, 2)));
        Assert.That(await queryContext.GetCharacterSummaryAsync("Unknown", Stats.Level.Id, Stats.MasterLevel.Id, Stats.Resets.Id).ConfigureAwait(false), Is.Null);
    }

    /// <summary>
    /// Tests that the summary query of the entity framework implementation can be translated to SQL.
    /// </summary>
    [Test]
    public void EntityFrameworkSummaryQueryIsTranslatable()
    {
        using var context = new AccountContext();

        var sql = PlayerContext.CreateCharacterSummaryQuery(context.Set<Persistence.EntityFramework.Model.Character>().Where(c => c.Name == "Wizard"), Stats.Level.Id, Stats.MasterLevel.Id, Stats.Resets.Id).ToQueryString();

        Assert.That(sql, Does.Contain("WHERE"));
    }

    /// <summary>
    /// Tests that a character which can be ranked is found, and that the characters of bots,
    /// templates and game masters are not found.
    /// </summary>
    [Test]
    public async Task RankedCharacterIsFoundAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        using (var context = contextProvider.CreateNewPlayerContext(new GameConfiguration()))
        {
            var characterClass = context.CreateNew<CharacterClass>();
            characterClass.Name = "Dark Knight";
            AddAccount(context, characterClass, isBot: false, ("Knight", 400, 0, 0, CharacterStatus.Normal), ("GameMaster", 400, 0, 0, CharacterStatus.GameMaster));
            AddAccount(context, characterClass, isBot: true, ("Bot", 400, 0, 0, CharacterStatus.Normal));
            AddAccount(context, characterClass, isBot: false, ("Template", 400, 0, 0, CharacterStatus.Normal)).IsTemplate = true;
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        using var queryContext = contextProvider.CreateNewPlayerContext(new GameConfiguration());

        var knight = await queryContext.GetRankedCharacterAsync("Knight").ConfigureAwait(false);
        Assert.That(knight?.Name, Is.EqualTo("Knight"));
        foreach (var name in new[] { "GameMaster", "Bot", "Template", "Unknown" })
        {
            Assert.That(await queryContext.GetRankedCharacterAsync(name).ConfigureAwait(false), Is.Null, name);
        }
    }

    /// <summary>
    /// Tests that the query of the identifier of a character which can be ranked can be translated to SQL,
    /// with the same conditions as the ranking.
    /// </summary>
    [Test]
    public void EntityFrameworkRankedCharacterIdQueryIsTranslatable()
    {
        using var context = new AccountContext();

        var sql = PlayerContext.CreateRankedCharacterIdQuery(context.Set<Persistence.EntityFramework.Model.Account>(), "Knight").ToQueryString();

        Assert.That(sql, Does.Contain("IsBot").And.Contain("IsTemplate").And.Contain("CharacterStatus").And.Contain("Name"));
    }

    /// <summary>
    /// Tests that the json query, with which the entity framework implementation loads a ranked character,
    /// loads the items of the character, but not its account with the other characters and the vault.
    /// </summary>
    [Test]
    public void CharacterJsonQueryDoesNotLoadAccount()
    {
        using var context = new EntityDataContext();
        var characterType = context.Model.FindEntityType(typeof(Persistence.EntityFramework.Model.Character))!;

        var query = new Persistence.EntityFramework.Json.JsonQueryBuilder().BuildJsonQueryForEntity(characterType);

        Assert.That(query, Does.Contain("\"ItemStorage\"").And.Contain("\"Item\""));
        Assert.That(query, Does.Not.Contain("\"Account\""));
    }

    private static Account AddAccount(IContext context, CharacterClass characterClass, bool isBot, params (string Name, int Level, int MasterLevel, int Resets, CharacterStatus Status)[] characters)
    {
        var account = context.CreateNew<Account>();
        account.LoginName = Guid.NewGuid().ToString("N")[..10];
        account.IsBot = isBot;
        foreach (var (name, level, masterLevel, resets, status) in characters)
        {
            var character = context.CreateNew<Character>();
            character.Name = name;
            character.CharacterClass = characterClass;
            character.CharacterStatus = status;
            character.Attributes.Add(new Persistence.BasicModel.StatAttribute(Stats.Level, level));
            character.Attributes.Add(new Persistence.BasicModel.StatAttribute(Stats.MasterLevel, masterLevel));
            character.Attributes.Add(new Persistence.BasicModel.StatAttribute(Stats.Resets, resets));
            account.Characters.Add(character);
        }

        return account;
    }
}
