// <copyright file="ChineseCharacterClassNamesTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Globalization;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Verifies the character class names of fresh configurations.
/// </summary>
[TestFixture]
[NonParallelizable]
internal class ChineseCharacterClassNamesTests
{
    private static readonly CultureInfo Chinese = CultureInfo.GetCultureInfo("zh-CN");
    private static readonly CultureInfo German = CultureInfo.GetCultureInfo("de");
    private static readonly IReadOnlyDictionary<byte, string> OfficialNames = new Dictionary<byte, string>
    {
        [0] = "魔法师",
        [2] = "魔导师",
        [3] = "神导师",
        [4] = "剑士",
        [6] = "骑士",
        [7] = "神骑士",
        [8] = "弓箭手",
        [10] = "圣射手",
        [11] = "神射手",
        [12] = "魔剑士",
        [13] = "剑圣",
        [16] = "圣导师",
        [17] = "祭祀",
        [20] = "召唤术师",
        [22] = "召唤导师",
        [23] = "召唤巫师",
        [24] = "格斗家",
        [25] = "格斗大师",
    };

    /// <summary>
    /// New configurations include the names, linked to their resource source.
    /// </summary>
    /// <param name="version">The supported initialization version.</param>
    /// <param name="classCount">The number of classes in that version.</param>
    [TestCase("075", 3)]
    [TestCase("095d", 4)]
    [TestCase("Season6", 18)]
    public async Task InitializationIncludesOfficialNamesAsync(string version, int classCount)
    {
        var provider = new InMemoryPersistenceContextProvider();
        DataInitializationBase initializer = version switch
        {
            "075" => new Version075.DataInitialization(provider, NullLoggerFactory.Instance),
            "095d" => new Version095d.DataInitialization(provider, NullLoggerFactory.Instance),
            _ => new VersionSeasonSix.DataInitialization(provider, NullLoggerFactory.Instance),
        };
        await initializer.CreateInitialDataAsync(1, false).ConfigureAwait(false);
        using var context = provider.CreateNewContext();
        var configuration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        Assert.That(configuration.CharacterClasses, Has.Count.EqualTo(classCount));
        foreach (var characterClass in configuration.CharacterClasses)
        {
            Assert.That(characterClass.Name.GetTranslation(Chinese, false), Is.EqualTo(OfficialNames[characterClass.Number]));
            Assert.That(characterClass.Name.ValueInNeutralLanguage, Does.Not.Contain("||"));
        }
    }
}
