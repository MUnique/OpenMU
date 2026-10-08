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
        Assert.That(ranking[1], Is.EqualTo(new CharacterSummary("Master", "Blade Knight", 400, 50, 0)));
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
            await queryContext.GetCharacterSummaryAsync("Wizard", Stats.Level.Id, Stats.MasterLevel.Id, Stats.Resets.Id).ConfigureAwait(false),
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

    private static void AddAccount(IContext context, CharacterClass characterClass, bool isBot, params (string Name, int Level, int MasterLevel, int Resets, CharacterStatus Status)[] characters)
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
    }
}
