// <copyright file="HideSkillEffectTimesTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the <see cref="HideSkillEffectTimesPlugIn"/>.
/// </summary>
[TestFixture]
internal class HideSkillEffectTimesTest
{
    private const short JackOLanternBlessingsNumber = 35;

    /// <summary>
    /// The update switches off the time of Infinite Arrow and both defense reductions, which have the number of the Fire Slash effect,
    /// and keeps the time of the other effects, e.g. of consumed items; applying it a second time changes nothing.
    /// </summary>
    [Test]
    public async Task UpdateHidesTheTimeOfTheSkillEffectsAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        using var context = contextProvider.CreateNewContext();
        var gameConfiguration = context.CreateNew<GameConfiguration>();
        var fireSlash = AddEffect(context, gameConfiguration, (short)MagicEffectNumber.DefenseReduction);
        var beastUppercut = AddEffect(context, gameConfiguration, (short)MagicEffectNumber.DefenseReduction);
        var infiniteArrow = AddEffect(context, gameConfiguration, (short)MagicEffectNumber.InfiniteArrow);
        var jackOLanternBlessings = AddEffect(context, gameConfiguration, JackOLanternBlessingsNumber);

        var update = new HideSkillEffectTimesPlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(fireSlash.SendDuration, Is.False);
            Assert.That(beastUppercut.SendDuration, Is.False);
            Assert.That(infiniteArrow.SendDuration, Is.False);
            Assert.That(jackOLanternBlessings.SendDuration, Is.True);
            Assert.That(gameConfiguration.MagicEffects, Has.Count.EqualTo(4));
        });
    }

    private static MagicEffectDefinition AddEffect(IContext context, GameConfiguration gameConfiguration, short number)
    {
        var effect = context.CreateNew<MagicEffectDefinition>();
        effect.Number = number;
        effect.SendDuration = true;
        gameConfiguration.MagicEffects.Add(effect);
        return effect;
    }
}
