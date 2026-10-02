// <copyright file="SkillHitValidatorTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.GameLogic;

/// <summary>
/// Tests the animation counter of the <see cref="SkillHitValidator"/>.
/// </summary>
[TestFixture]
public class SkillHitValidatorTest
{
    private const ushort SkillId = 8;
    private const byte MaximumCounterValue = 0x32;

    /// <summary>
    /// Tests that the first animation counter is accepted, because the client doesn't reset its counter,
    /// and that the following animations have to continue it.
    /// </summary>
    [Test]
    public void AnimationCounterContinuesFromFirstAnimation()
    {
        var validator = new SkillHitValidator(NullLogger.Instance);

        Assert.That(validator.TryRegisterAnimation(SkillId, 10), Is.True);
        Assert.That(validator.TryRegisterAnimation(SkillId, 11), Is.True);
        Assert.That(validator.TryRegisterAnimation(SkillId, 13), Is.False);
    }

    /// <summary>
    /// Tests that the expected animation counter wraps from its maximum back to 1.
    /// </summary>
    [Test]
    public void AnimationCounterWrapsAtMaximum()
    {
        var validator = new SkillHitValidator(NullLogger.Instance);

        Assert.That(validator.TryRegisterAnimation(SkillId, MaximumCounterValue), Is.True);
        Assert.That(validator.TryRegisterAnimation(SkillId, 1), Is.True);
    }
}
