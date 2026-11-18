// <copyright file="MagicEffectsListUpdateTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Tests that the owner of an effect hears about a new time or new values (<see cref="IUpdateMagicEffectPlugIn"/>).
/// </summary>
[TestFixture]
public class MagicEffectsListUpdateTests
{
    private const short EffectNumber = 28;

    /// <summary>
    /// A recast restarts the active effect, and the owner gets the active effect with its new time.
    /// </summary>
    [Test]
    public async ValueTask RecastInformsTheOwnerAsync()
    {
        await using var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await using var first = CreateEffect(TimeSpan.FromSeconds(30), new ConstantElement(10));
        await player.MagicEffectList.AddEffectAsync(first).ConfigureAwait(false);

        await using var recast = CreateEffect(TimeSpan.FromSeconds(90), new ConstantElement(20));
        await player.MagicEffectList.AddEffectAsync(recast).ConfigureAwait(false);

        var update = Mock.Get(player.ViewPlugIns.GetPlugIn<IUpdateMagicEffectPlugIn>()!);
        update.Verify(v => v.UpdateMagicEffectAsync(first, player), Times.Once);
        Assert.Multiple(() =>
        {
            Assert.That(first.Value, Is.EqualTo(20));
            Assert.That(first.TimeLeft, Is.GreaterThan(TimeSpan.FromSeconds(60)));
        });
    }

    /// <summary>
    /// A recast by the same caster brings the same value elements (they are cached per skill entry):
    /// only the time restarts, and the owner still hears about it.
    /// </summary>
    [Test]
    public async ValueTask RecastWithTheSameElementsInformsTheOwnerAsync()
    {
        await using var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var casterElement = new ConstantElement(15);
        await using var first = CreateEffect(TimeSpan.FromSeconds(30), casterElement);
        await player.MagicEffectList.AddEffectAsync(first).ConfigureAwait(false);

        await using var recast = CreateEffect(TimeSpan.FromSeconds(90), casterElement);
        await player.MagicEffectList.AddEffectAsync(recast).ConfigureAwait(false);

        var update = Mock.Get(player.ViewPlugIns.GetPlugIn<IUpdateMagicEffectPlugIn>()!);
        update.Verify(v => v.UpdateMagicEffectAsync(first, player), Times.Once);
        Assert.That(first.TimeLeft, Is.GreaterThan(TimeSpan.FromSeconds(60)));
    }

    /// <summary>
    /// A recast with a lower value is rejected, so nothing changes and the owner hears nothing.
    /// </summary>
    [Test]
    public async ValueTask RejectedRecastInformsNobodyAsync()
    {
        await using var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await using var first = CreateEffect(TimeSpan.FromSeconds(30), new ConstantElement(20));
        await player.MagicEffectList.AddEffectAsync(first).ConfigureAwait(false);

        await using var weaker = CreateEffect(TimeSpan.FromSeconds(90), new ConstantElement(10));
        await player.MagicEffectList.AddEffectAsync(weaker).ConfigureAwait(false);

        var update = Mock.Get(player.ViewPlugIns.GetPlugIn<IUpdateMagicEffectPlugIn>()!);
        update.Verify(v => v.UpdateMagicEffectAsync(It.IsAny<MagicEffect>(), It.IsAny<IAttackable>()), Times.Never);
    }

    /// <summary>
    /// A shortened effect tells the owner its new time.
    /// </summary>
    [Test]
    public async ValueTask ShortenedEffectInformsTheOwnerAsync()
    {
        await using var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await using var effect = CreateEffect(TimeSpan.FromSeconds(60), new ConstantElement(1));
        await player.MagicEffectList.AddEffectAsync(effect).ConfigureAwait(false);

        await player.MagicEffectList.ShortenEffectsProducingSpecificStatAsync(Stats.IsFrozen, 10).ConfigureAwait(false);

        var update = Mock.Get(player.ViewPlugIns.GetPlugIn<IUpdateMagicEffectPlugIn>()!);
        update.Verify(v => v.UpdateMagicEffectAsync(effect, player), Times.Once);
        Assert.That(effect.TimeLeft, Is.LessThan(TimeSpan.FromSeconds(51)));
    }

    /// <summary>
    /// An effect shortened to its end tells the owner nothing: its deactivation follows.
    /// </summary>
    [Test]
    public async ValueTask EffectShortenedToItsEndInformsNobodyAsync()
    {
        await using var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await using var effect = CreateEffect(TimeSpan.FromSeconds(10), new ConstantElement(1));
        await player.MagicEffectList.AddEffectAsync(effect).ConfigureAwait(false);

        await player.MagicEffectList.ShortenEffectsProducingSpecificStatAsync(Stats.IsFrozen, 10).ConfigureAwait(false);

        var update = Mock.Get(player.ViewPlugIns.GetPlugIn<IUpdateMagicEffectPlugIn>()!);
        update.Verify(v => v.UpdateMagicEffectAsync(It.IsAny<MagicEffect>(), It.IsAny<IAttackable>()), Times.Never);
    }

    /// <summary>
    /// An effect shortened to under a second still informs the owner: whether an update so close to the end
    /// is worth sending is up to the view.
    /// </summary>
    [Test]
    public async ValueTask EffectShortenedToUnderASecondInformsTheOwnerAsync()
    {
        await using var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await using var effect = CreateEffect(TimeSpan.FromSeconds(10.5), new ConstantElement(1));
        await player.MagicEffectList.AddEffectAsync(effect).ConfigureAwait(false);

        await player.MagicEffectList.ShortenEffectsProducingSpecificStatAsync(Stats.IsFrozen, 10).ConfigureAwait(false);

        var update = Mock.Get(player.ViewPlugIns.GetPlugIn<IUpdateMagicEffectPlugIn>()!);
        update.Verify(v => v.UpdateMagicEffectAsync(effect, player), Times.Once);
    }

    private static MagicEffect CreateEffect(TimeSpan duration, IElement value)
    {
        var definition = new Persistence.BasicModel.MagicEffectDefinition { Number = EffectNumber };
        return new MagicEffect(duration, definition, new MagicEffect.ElementWithTarget(value, Stats.IsFrozen));
    }
}