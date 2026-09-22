// <copyright file="MagicEffectsListConcurrencyTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offline;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;

/// <summary>
/// Pins the torn-read fix: concurrent effect add/expiry must never corrupt
/// reads or throw, which the pre-fix lock-free Values.ToArray() snapshots did regularly.
/// </summary>
[TestFixture]
public class MagicEffectsListConcurrencyTests
{
    private IGameContext _gameContext = null!;

    /// <summary>
    /// Sets up a fresh game context before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._gameContext = GameContextTestHelper.CreateGameContext();
    }

    /// <summary>
    /// Hammers <see cref="MagicEffectsList"/> with a writing task interleaved with
    /// lock-free-looking readers. Any torn read, missed removal, or leaked entry fails the test.
    /// </summary>
    [Test]
    public async ValueTask ConcurrentAddExpiryAndReadNeverThrowsNorCorruptsAsync()
    {
        var player = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
        var list = player.MagicEffectList;
        var definitions = Enumerable.Range(1, 8)
            .Select(number => new MagicEffectDefinition { Number = (short)number })
            .ToArray();

        try
        {
            var writer = Task.Run(async () =>
            {
                for (int i = 0; i < 500; i++)
                {
                    var effect = new MagicEffect(TimeSpan.FromMinutes(10), definitions[i % definitions.Length]);
                    await list.AddEffectAsync(effect).ConfigureAwait(false);
                    await effect.DisposeAsync().ConfigureAwait(false);
                }
            });

            var readers = Enumerable.Range(0, 4)
                .Select(reader => Task.Run(() =>
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        foreach (var definition in definitions)
                        {
                            _ = list.ContainsEffect(definition.Number);
                            _ = list.HasEffect(definition);
                        }

                        _ = list.GetActiveEffectsSnapshot();
                        if (list.TryGetEffect(definitions[0].Number, out var effect))
                        {
                            Assert.That(effect.Definition, Is.SameAs(definitions[0]));
                        }
                    }
                }))
                .ToArray();

            await Task.WhenAll(new[] { writer }.Concat(readers)).ConfigureAwait(false);

            var snapshot = list.GetActiveEffectsSnapshot();
            foreach (var effect in snapshot)
            {
                Assert.That(list.ContainsEffect(effect.Id), Is.True);
            }
        }
        finally
        {
            await list.ClearAllEffectsAsync().ConfigureAwait(false);
        }

        Assert.That(list.GetActiveEffectsSnapshot(), Is.Empty);
    }
}
