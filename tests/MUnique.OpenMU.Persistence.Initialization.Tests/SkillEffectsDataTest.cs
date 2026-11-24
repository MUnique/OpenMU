// <copyright file="SkillEffectsDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the magic effects which are created for the skills, e.g. the poison and ice effects.
/// </summary>
[TestFixture]
internal class SkillEffectsDataTest
{
    /// <summary>
    /// Tests that the skills with the same effect setting share one magic effect, instead of
    /// creating an identical effect for each skill.
    /// </summary>
    [Test]
    public async Task SkillsShareIdenticalEffectsAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);
        using var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();

        short[] effectNumbers = [(short)MagicEffectNumber.Poisoned, (short)MagicEffectNumber.Iced, (short)MagicEffectNumber.Cold, (short)MagicEffectNumber.Freeze];
        var duplicates = gameConfiguration.MagicEffects
            .Where(effect => effectNumbers.Contains(effect.Number))
            .GroupBy(effect => (
                effect.Number,
                effect.SubType,
                Duration: effect.Duration?.ConstantValue.Value,
                Chance: effect.Chance?.ConstantValue.Value ?? 0,
                TargetAttribute: effect.PowerUpDefinitions.FirstOrDefault()?.TargetAttribute?.Id))
            .Where(group => group.Count() > 1)
            .Select(group => $"{group.Key}: {group.Count()}")
            .ToList();

        Assert.That(duplicates, Is.Empty);
    }
}
