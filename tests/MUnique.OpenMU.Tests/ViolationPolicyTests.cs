// <copyright file="ViolationPolicyTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic.PlugIns;
using NUnit.Framework;

/// <summary>
/// Tests for the pure violation score policy.
/// </summary>
[TestFixture]
public class ViolationPolicyTests
{
    /// <summary>
    /// Tests that a configured threshold is used as is.
    /// </summary>
    [Test]
    public void ResolveRejectThreshold_ConfiguredValue_ReturnsIt()
    {
        Assert.That(ViolationPolicy.ResolveRejectThreshold(4.0), Is.EqualTo(4.0));
    }

    /// <summary>
    /// Tests that a zero threshold from a legacy database falls back to the default.
    /// </summary>
    [Test]
    public void ResolveRejectThreshold_ZeroFromLegacyDatabase_ReturnsDefault()
    {
        Assert.That(ViolationPolicy.ResolveRejectThreshold(0), Is.EqualTo(ViolationPolicy.DefaultRejectThreshold));
    }

    /// <summary>
    /// Tests that a zero half-life from a legacy database falls back to the default.
    /// </summary>
    [Test]
    public void ResolveHalfLifeMinutes_ZeroFromLegacyDatabase_ReturnsDefault()
    {
        Assert.That(ViolationPolicy.ResolveHalfLifeMinutes(0), Is.EqualTo(ViolationPolicy.DefaultHalfLifeMinutes));
    }

    /// <summary>
    /// Tests that no passed time leaves the score unchanged.
    /// </summary>
    [Test]
    public void DecayScore_NoTimePassed_ReturnsUnchanged()
    {
        Assert.That(ViolationPolicy.DecayScore(3.0, TimeSpan.Zero, 5.0), Is.EqualTo(3.0));
    }

    /// <summary>
    /// Tests that one half-life halves the score.
    /// </summary>
    [Test]
    public void DecayScore_OneHalfLife_HalvesTheScore()
    {
        Assert.That(ViolationPolicy.DecayScore(4.0, TimeSpan.FromMinutes(5.0), 5.0), Is.EqualTo(2.0).Within(0.0001));
    }

    /// <summary>
    /// Tests that a violation without overshoot counts one.
    /// </summary>
    [Test]
    public void WeighViolation_NoDeficit_CountsOne()
    {
        Assert.That(ViolationPolicy.WeighViolation(0, 250.0), Is.EqualTo(1.0));
    }

    /// <summary>
    /// Tests that a violation with full overshoot counts two.
    /// </summary>
    [Test]
    public void WeighViolation_FullDeficit_CountsTwo()
    {
        Assert.That(ViolationPolicy.WeighViolation(250.0, 250.0), Is.EqualTo(2.0));
    }

    /// <summary>
    /// Tests that a score at the threshold escalates.
    /// </summary>
    [Test]
    public void ShouldEscalate_AtThreshold_ReturnsTrue()
    {
        Assert.That(ViolationPolicy.ShouldEscalate(6.0, 6.0), Is.True);
    }

    /// <summary>
    /// Tests that a score below the threshold stays soft.
    /// </summary>
    [Test]
    public void ShouldEscalate_BelowThreshold_ReturnsFalse()
    {
        Assert.That(ViolationPolicy.ShouldEscalate(5.9, 6.0), Is.False);
    }

    /// <summary>
    /// Tests that a warning within the limit only warns the player.
    /// </summary>
    [Test]
    public void DecideSanction_WithinLimit_WarnsOnly()
    {
        var config = new SpeedHackDetectConfiguration { MaxWarnings = 3 };
        Assert.That(ViolationPolicy.DecideSanction(config, 3, true, true), Is.EqualTo(new ViolationPolicy.Sanction(WarnPlayer: true, DisconnectPlayer: false, BanAccount: false)));
    }

    /// <summary>
    /// Tests that exceeding the limit with auto-ban and an account bans.
    /// </summary>
    [Test]
    public void DecideSanction_OverLimitWithAutoBanAndAccount_Bans()
    {
        var config = new SpeedHackDetectConfiguration { MaxWarnings = 3, AutoBan = true, DisconnectOnViolation = true };
        Assert.That(ViolationPolicy.DecideSanction(config, 4, true, true), Is.EqualTo(new ViolationPolicy.Sanction(WarnPlayer: false, DisconnectPlayer: true, BanAccount: true)));
    }

    /// <summary>
    /// Tests that exceeding the limit without an account disconnects instead of banning.
    /// </summary>
    [Test]
    public void DecideSanction_OverLimitWithoutAccount_DisconnectsWithoutBan()
    {
        var config = new SpeedHackDetectConfiguration { MaxWarnings = 3, AutoBan = true, DisconnectOnViolation = true };
        Assert.That(ViolationPolicy.DecideSanction(config, 4, true, false), Is.EqualTo(new ViolationPolicy.Sanction(WarnPlayer: false, DisconnectPlayer: true, BanAccount: false)));
    }

    /// <summary>
    /// Tests that exceeding the limit with disconnect only disconnects.
    /// </summary>
    [Test]
    public void DecideSanction_OverLimitWithDisconnectOnly_Disconnects()
    {
        var config = new SpeedHackDetectConfiguration { MaxWarnings = 3, AutoBan = false, DisconnectOnViolation = true };
        Assert.That(ViolationPolicy.DecideSanction(config, 4, true, true), Is.EqualTo(new ViolationPolicy.Sanction(WarnPlayer: false, DisconnectPlayer: true, BanAccount: false)));
    }

    /// <summary>
    /// Tests that exceeding the limit without ban or disconnect still warns.
    /// </summary>
    [Test]
    public void DecideSanction_OverLimitWithoutBanOrDisconnect_Warns()
    {
        var config = new SpeedHackDetectConfiguration { MaxWarnings = 3, AutoBan = false, DisconnectOnViolation = false };
        Assert.That(ViolationPolicy.DecideSanction(config, 4, true, true), Is.EqualTo(new ViolationPolicy.Sanction(WarnPlayer: true, DisconnectPlayer: false, BanAccount: false)));
    }

    /// <summary>
    /// Tests that exempt players are not punished at all, even when exceeding the limit.
    /// </summary>
    [Test]
    public void DecideSanction_ExemptPlayer_DoesNothing()
    {
        var config = new SpeedHackDetectConfiguration { MaxWarnings = 3, AutoBan = true, DisconnectOnViolation = true };
        Assert.That(ViolationPolicy.DecideSanction(config, 4, false, true), Is.EqualTo(default(ViolationPolicy.Sanction)));
    }
}
